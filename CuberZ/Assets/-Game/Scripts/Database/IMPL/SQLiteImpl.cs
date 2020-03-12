/******************************************************************************
* Copyright (c) 2020 Herolix Studio
* All rights reserved.
******************************************************************************/

using UnityEngine;
using System.Data;
using System.IO;
using System.Data.SQLite;
using Mono.Data.SqliteClient;

public class SQLiteImpl : MonoBehaviour, IDatabase
{
    private string dbPath_;
    private IDbConnection dbConnection_;
    private IDbCommand dbCommand_;
    private IDataReader reader_;

    void Awake ()
    {
        dbPath_ =  Application.dataPath + "/-Game/Scripts/Database/SQLiteDatabase/";
        OpenConnection("test");
        CreateTable("client", "name VARCHAR(10)");
    }
 
    public void CreateDatabase(string dbName)
    {
        if (Directory.GetFiles(dbPath_, "*.db").Length == 0)
        {
            Debug.LogWarning("No database file found");
            Debug.Log("Creating new database");
            SQLiteConnection.CreateFile(Path.Combine(dbPath_, dbName + ".db"));
        }
        else Debug.Log("There is a database created");
    }

    public void OpenConnection(string dbName)
    {
        dbConnection_ = new SqliteConnection();
        dbConnection_.ConnectionString = "URI=file:" + dbPath_ + dbName + ".db";
        dbConnection_.Open();

        if (dbConnection_.State == ConnectionState.Open)
            Debug.Log("Connection opened successfully"); 
    }

    public void CloseConnection()
    {
        dbConnection_.Close();
        dbConnection_ = null;
        Debug.Log("Connection closed successfully");
    }

    public void CreateTable(string tableName, string sqlQuery)
    {
        try
        {
            dbCommand_ = dbConnection_.CreateCommand();
            dbCommand_.CommandText =
                "CREATE TABLE IF NOT EXISTS " + tableName + "(" + sqlQuery + ")";
            reader_ = dbCommand_.ExecuteReader();
            Debug.Log("New table created");
            dbCommand_.Dispose();
            dbCommand_ = null;
        }
        catch { Debug.Log("Can't create table without a database connected"); }
    }

    public void ShowData(string tableName)
    {
        dbCommand_ = dbConnection_.CreateCommand();
        dbCommand_.CommandText = "SELECT * FROM " + tableName;
        reader_ = dbCommand_.ExecuteReader();
        dbCommand_.Dispose();
        dbCommand_ = null;
    }
}
