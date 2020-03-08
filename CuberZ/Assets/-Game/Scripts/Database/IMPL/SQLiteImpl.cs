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
    private string dbPath_ = Application.dataPath + "/-Game/Scripts/Database/SQLiteDatabase";
    private IDbConnection dbConnection;
    private IDbCommand dbCommand;

    public void CreateDatabase(string dbName)
    {
        if (Directory.GetFiles(dbPath_, ".sqlite").Length == 0)
        {
            Debug.LogWarning("No database file found");
            Debug.Log("Creating new database");
            SQLiteConnection.CreateFile(dbName + ".sqlite");
        }
        else Debug.Log("There is a database created");
    }

    public void OpenConnection(string dbName)
    {
        if (dbConnection.State == ConnectionState.Closed)
        {
            dbConnection = new SqliteConnection(dbPath_ + dbName + ".sqlite");
            dbConnection.Open();
        }
        else Debug.Log("A database instance is opened already");
    }

    public void CloseConnection()
    {
        dbConnection.Close();
        dbConnection = null;
    }

    public void CreateTable(string tableName, string sqlQuery)
    {
        if (dbConnection.State == ConnectionState.Open) 
        {
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "CREATE TABLE IF NOT EXISTS " + tableName + "(" + sqlQuery + ")";
            dbCommand.ExecuteNonQuery();
            dbCommand.Dispose();
            dbCommand = null;
        }
        else Debug.Log("Connect to a database first");
    }

    public void ShowData(string tableName)
    {
        if (dbConnection.State != ConnectionState.Open)
        {
            dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = "SELECT * FROM " + tableName;
            dbCommand.ExecuteNonQuery();
            dbCommand.Dispose();
            dbCommand = null;
        }
        else Debug.Log("Connect to a database first");
    }
}
