/******************************************************************************
* Copyright (c) 2020 Herolix Studio
* All rights reserved.
******************************************************************************/

public interface IDatabase
{
    void CreateDatabase(string dbName);
    void OpenConnection(string dbName);
    void CloseConnection();
    void CreateTable(string tableName, string sqlQuery);
    void ShowData(string tableName);
}
