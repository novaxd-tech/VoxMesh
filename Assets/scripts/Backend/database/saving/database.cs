using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;


public class database: MonoBehaviour
{
    private static string folderPath;
    private static string databaseFile;
    static IDbCommand dbcmd;
    static IDbConnection dbcon;
    static IDataReader reader;
    static string connection;

    private void Start()
    {
       
    }

    public static void init()
    {
        
        string appData = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        folderPath = Path.Combine(appData, "Hendrix");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        connection = "URI=file:" + folderPath + "/" + "config";
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        Debug.Log("db connectado"); 

    }

    public static object executeCommand(string dbcommand, bool share_command = false, bool returnOutput = false)
    {
        //open();
        IDbCommand cmd = dbcon.CreateCommand();
        cmd.CommandText = dbcommand;
        if (!returnOutput) 
        {
            cmd.ExecuteNonQuery();
            if(share_command)dbsync(dbcommand);
            //close();
            return null;
        }
        else
        {
            reader = cmd.ExecuteReader();
            //close();
            return reader;
        }
        
        
    }

    public static int checkIfTableHasContents(string table_name)
    {
        //retorna o numero de elementos dentro de uma tabela
        reader = (IDataReader)executeCommand($"SELECT COUNT (*) FROM {table_name}",false, true);
        return int.Parse(reader[0].ToString());
    }

    public static void createTable(string tableName, string params_, bool db=false)
    {
        dbcmd = dbcon.CreateCommand();
        dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + " " + "(id INTEGER PRIMARY KEY, " + params_ + ")";
        reader = dbcmd.ExecuteReader();

        if (db)
        {
            dbsync(dbcmd.CommandText);
        }
    }

    private static void dbsync(string command)
    {
        netHandler.sendMessage($"dbsync,{command}");
    }


    public static void close()
    {
        dbcon.Close();
    }
    public static void open()
    {
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
    }

}
