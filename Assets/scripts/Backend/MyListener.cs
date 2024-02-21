using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Data;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityText2Speech;

public class MyListener : MonoBehaviour
{
    public int connectionPort = 4444;
    public static string role;
    [HideInInspector]
    public Toggle aiSettings; 
    [HideInInspector]
    public Toggle deviceSettings;   

    TcpListener server;
    TcpClient client;
    TextMeshProUGUI ID;
    Process process;
    Thread thread;
    IPEndPoint endPoint;

    //MESHES ATIVOS
    public static List<string> meshes = new List<string>();

    //SETUP DO CARGO E DO ID
    public static bool ppRole;
    bool defineID;
    public static bool listening;
    bool update;

    public static string id;
    public static string[] localServer = new string[] { "127.0.0.1", "31415" };
    public static string masterIP;

    string msg;
    private bool debug_bool;

    //iniciar servidor automaticamente
    public bool startServer = true;

    public bool mesh_add { get; private set; }

    //TTS
    private USgs SGS;
    private tts tts;


    private void OnDestroy()
    {
        netHandler.sendMessage("shutdown");
        thread.Abort();
        server.Stop();
        if(client != null) client.Dispose();
        database.close();
        print("db fechado");
    }

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        string pythonInterpreterPath = "python";
        string pythonScriptPath = "Assets/server.py";
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = pythonInterpreterPath,
            Arguments = pythonScriptPath,
            UseShellExecute = true,
            RedirectStandardOutput = false,
            CreateNoWindow = true
        };

        process = new Process();
        process.StartInfo = startInfo;
    }

    private void Update()
    {

        
        
        // se a cena for o painel 
        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Painel")
        {
            if (tts == null) tts = GameObject.Find("SGS").GetComponent<tts>();
            if(deviceSettings == null) deviceSettings = GameObject.Find("Devices").GetComponent<Toggle>();
            if(aiSettings == null) aiSettings = GameObject.Find("ai-settings").GetComponent<Toggle>();
            if(ID == null) ID = GameObject.Find("ID").GetComponent<TextMeshProUGUI>();
            if(SGS == null) SGS = GameObject.Find("SGS").GetComponent<USgs>();

            if (defineID)
            {
                ID.text = $"ID: <u>{id}</u>";
                defineID = false;
            }

            if (ppRole)
            {
                PlayerPrefs.SetString("Role", role);
                if (role == "slave")
                {
                    //print("slave");
                    //desabilita o botão de devices
                    deviceSettings.isOn = false;
                    deviceSettings.interactable = false;
                    aiSettings.isOn = true;
                }

                if (role == "master")
                {
                    //print("master");
                    deviceSettings.interactable = true;
                                    
                }
                ppRole = false;
            }

            if (mesh_add)
            {
                Group[] grupos = FindObjectsOfType<Group>();
                foreach (Group grupo in grupos)
                {
                    grupo.addToAccessVlg(msg);
                    mesh_add = false;
                }
            }

            if (debug_bool)
            {
                print(msg);
                debug_bool = false;
            }
        }
    }

    void Start()
    {
        // Receive on a separate thread so Unity doesn't freeze waiting for data
        ThreadStart ts = new ThreadStart(GetData);
        thread = new Thread(ts);
        thread.Start();
    }

    void GetData()
    {
        // Create the server
        server = new TcpListener(IPAddress.Any, connectionPort);
        server.Start();
        if(startServer) process.Start();
        client = server.AcceptTcpClient();
        


        while (true)
        {
            // Read data from the network stream
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);

            // Decode the bytes into a string
            string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            int port = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

            
            Connection(msg, port.ToString());
        }
    }

   

    void Connection(string dataReceived, string sender_port)
    {
        
        // Make sure we're not getting an empty string
        if (dataReceived != null && dataReceived != "")
        {
            //UnityEngine.Debug.Log(dataReceived);
            string[] dataReceivedSplit = dataReceived.Split(',');
            
            if (dataReceivedSplit[0] == "active_mesh" && !meshes.Contains(dataReceivedSplit[1]))
            {
                meshes.Add(dataReceivedSplit[1]);
                mesh_add = true;
                msg = dataReceivedSplit[1];
            }
            
            if (dataReceivedSplit[0] == "hear")
            {
                UnityEngine.Debug.Log("CONECTADO AO SERVIDOR LOCAL");
                listening = true;
                return;
            }

            if (dataReceivedSplit[0] == "local_mesh_ping"){
                //ENVIA PONG DE RESPOSTA AO SERVIDOR PYTHON
                netHandler.send("local_mesh_pong", "127.0.0.1", sender_port);
            }
            
            if (dataReceivedSplit[0] == "role")
            {
                role = dataReceivedSplit[1];
                ppRole = true;
                return;
            }

            if (dataReceivedSplit[0] == "my_id")
            {
                id = dataReceivedSplit[1];
                defineID = true;
                return;
            }

            if (dataReceivedSplit[0] == "plea" && role == "master")
            {
                //SELECIONA NO BANCO DE DADOS O DISPOSITIVO QUE DÁ MATCH NO NOME, E NO ROOM_ID. PEGUE AS INFORMAÇÕES: IP, PORTA, COMANDO
                string voice_command = dataReceivedSplit[1].ToLower();
                string asker = dataReceivedSplit[2];
                string room_id = getIdFromIP(asker);
                bool isDevice = false;
                bool isGroup = false;
                try
                {
                    string sql_command = $"SELECT devices.ip, device_commands.port, device_commands.server_command, device_commands.ai_response " +
                        $"FROM device_commands INNER JOIN devices " +
                        $"ON devices.id = device_commands.device_id WHERE device_commands.voice_command = '{voice_command}' AND devices.room_id = {room_id}";

                    IDataReader reader = (IDataReader)database.executeCommand(sql_command, false, true);

                    while (reader.Read())
                    {
                        isDevice = true;
                        string ip = reader.GetString(reader.GetOrdinal("ip"));
                        string port = reader.GetString(reader.GetOrdinal("port"));
                        string command = reader.GetString(reader.GetOrdinal("server_command"));
                        string ai_response = reader.GetString(reader.GetOrdinal("ai_response"));

                        //ENVIA PACOTE PARA O DISPOSITIVO
                        netHandler.sendMessage($"bestow,{ip},{port},{command},{asker},{ai_response}");
                    }

                    //SE FOR UM DISPOSITIVO
                    if (isDevice) return;



                    //PROCURA NA TABELA DE GRUPOS
                    string sql_cmd = $"SELECT nome, devices, access, activate_voice_command, deactivate_voice_command, activate_command, deactivate_command, activate_command_port, deactivate_command_port " +
                                    $"FROM groups WHERE activate_voice_command = '{voice_command}' OR deactivate_voice_command = '{voice_command}'";
                    
                    reader = (IDataReader)database.executeCommand(sql_cmd,false, true);
                    
                    while(reader.Read()) 
                    {
                        isGroup = true;
                        string group_name = reader.GetString(reader.GetOrdinal("nome"));
                        string[] devices = reader.GetString(reader.GetOrdinal("devices")).Split(';');
                        string[] access = reader.GetString(reader.GetOrdinal("access")).Split(',');
                        string activate_voice_command = reader.GetString(reader.GetOrdinal("activate_voice_command"));
                        string deactivate_voice_command = reader.GetString(reader.GetOrdinal("deactivate_voice_command"));
                        string activate_command = reader.GetString(reader.GetOrdinal("activate_command"));
                        string deactivate_command = reader.GetString(reader.GetOrdinal("deactivate_command"));
                        string activate_command_port = reader.GetString(reader.GetOrdinal("activate_command_port"));
                        string deactivate_command_port = reader.GetString(reader.GetOrdinal("deactivate_command_port"));

                        //O ASKER ESTÁ NO ACESSO DO GRUPO?
                        if (access.Contains(room_id))
                        {
                            foreach(string device in devices)
                            {
                                reader = (IDataReader)database.executeCommand($"SELECT ip FROM devices WHERE id = {device}", false, true);
                                string ip_device = reader[0].ToString();
                            
                                if(voice_command == activate_voice_command)
                                {
                                    netHandler.sendMessage($"group_msg,{ip_device},{activate_command_port},{activate_command}");    
                                }
                                else if (voice_command == deactivate_voice_command)
                                {
                                    netHandler.sendMessage($"group_msg,{ip_device},{deactivate_command_port},{deactivate_command}");
                                }
                            }
                        }
                    }

                    //SE FOR UM GRUPO
                    if (isGroup) return;

                    //dispositivo não existe
                    netHandler.sendMessage($"device_not_found,{asker}");
                }
                catch (SqliteException e)
                {
                    msg = e.ToString();
                    debug_bool = true;
                }
                return;
            }

            if (dataReceivedSplit[0] == "dbsync")
            {
                
                string command = string.Join(",", dataReceivedSplit.Skip(1).ToArray());
                database.executeCommand(command);
                return;
            }

            if (dataReceivedSplit[0] == "device_not_found")
            {
                tts.fala("dispositivo ou grupo não existe ou não está disponível");
            }

            if (dataReceivedSplit[0] == "bestow")
            {
                string command = dataReceivedSplit[1];
                tts.fala(command);
            }
        
            if (dataReceivedSplit[0] == "make_myself_slave")
            {
                //desativa devices tab
                role = "slave";
                ppRole = true;

                //atualiza db
                database.executeCommand("DELETE FROM role");
                database.executeCommand("INSERT INTO role (role) VALUES ('slave')");
            }
        }
    }

    public static string getIdFromIP(string ip)
    {
        string[] bytes = ip.Split('.');
        return bytes[3];
    }

    
} 