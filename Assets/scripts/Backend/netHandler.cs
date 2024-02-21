using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Data;
using System.Threading;
using System;
using System.Collections;


public class netHandler : MonoBehaviour
{
    private static string ip = "127.0.0.1";
    private static string port = "31415";
    
    //ESSE ROLE É SÓ PARA COMUNICAR COM O SERVIDOR, O ROLE USADO EM VERIFICAÇÕES DEVE SER O DA CLASSE `MyListener`
    public string role;
    Thread thread;
    
    

    private void Start()
    {

        database.init();
        MyListener.ppRole = true;

        database.createTable("role", "role TEXT");
        database.createTable("credentials", "login TEXT, password TEXT");
        database.createTable("ai_name", "name TEXT");
        database.createTable("responses", "response TEXT");
        database.createTable("voice_type", "genre TEXT");
        database.createTable("lang", "language TEXT");
        database.createTable("devices", "ip TEXT, device_name TEXT, room_id TEXT");
        database.createTable("device_commands", "device_id TEXT, voice_command TEXT, server_command TEXT, port TEXT, ai_response TEXT");
        database.createTable("groups", "nome TEXT, devices TEXT, access TEXT, activate_voice_command TEXT, deactivate_voice_command TEXT, activate_command TEXT, deactivate_command TEXT, activate_command_port INT, deactivate_command_port INT");


        
        IDataReader reader = (IDataReader)database.executeCommand("SELECT role FROM role",false, true);
        role = reader[0].ToString();
        if(role == "")
        {
            //PRIMEIRA VEZ LIGANDO O HENDRIX
            role = "slave";
            MyListener.role = "slave";
            database.executeCommand("DELETE FROM role");
            database.executeCommand("INSERT INTO role (role) VALUES ('slave')");
        }
        ThreadStart ths = new ThreadStart(waitForServer);
        thread = new Thread(ths);
        thread.Start();
    }

    private void waitForServer()
    {
        while (true)
        {
            if(MyListener.listening == true)
            {
                if(role == "master")
                {
                    Debug.Log("MANDANDO MAKE MYSELF MASTER");
                    sendMessage("make_myself_master");
          
                }else if(role == "slave")
                {
                    MyListener.role = "slave";
                    sendMessage("make_myself_slave");
                }
                thread.Abort();
            }
        }
    }

    public static void setKingRoleByDefault()
    {
        database.executeCommand("DELETE FROM role");
        database.executeCommand($"INSERT INTO role (role) VALUES ('master')");
        sendMessage("make_myself_master");
        MyListener.role = "master";
        MyListener.ppRole = true;
    }


    public static void sendMessage(string message)
    { 
        send(message, ip, port);
    } 
     
    public static void send(string message, string ip, string port)
    {
        try
        {

            using (TcpClient tcpClient = new TcpClient(ip, int.Parse(port)))
            {
                NetworkStream networkStream = tcpClient.GetStream();

                byte[] data = Encoding.UTF8.GetBytes(message);

                networkStream.Write(data, 0, data.Length);
            }
        }catch (Exception e)
        {
            print(e);
        }
    }

}
