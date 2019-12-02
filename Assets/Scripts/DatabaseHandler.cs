using UnityEngine;
using System;
using System.Data;
using System.Text;

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

using MySql.Data;
using MySql.Data.MySqlClient;

public class DatabaseHandler : MonoBehaviour
{
    public string host = "beemo.at";
    public string database = "crowddefensewebsite_development";
    public string user = "crowddefense_dev";
    public string password = "x5nZf%J9wFAnB*#zZEFh%bkabbAQ^FQir^6xidyM*x^JW54e54czVZ*hpAXih^5t";
    public bool pooling = true;

    private string connectionString;
    private MySqlConnection con = null;
    private MySqlCommand cmd = null;
    private MySqlDataReader rdr = null;

    private MD5 _md5Hash;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        connectionString = "Server=" + host + ";Port=3307;Database=" + database + ";User=" + user + ";Password=" + password + ";Pooling=";
        if (pooling) {
            connectionString += "True";
        }
        else {
            connectionString += "False";
        }
        try {
            con = new MySqlConnection(connectionString);
            con.Open();
            Debug.Log("Mysql state: " + con.State);

                
            string sql = "SELECT * FROM schema_migration";
            cmd = new MySqlCommand(sql, con);                    
            rdr = cmd.ExecuteReader();

            int i = 0;
            while (rdr.Read()) {
                Debug.Log("???");
                Debug.Log(rdr+ " -- ");
            }
            rdr.Close();

        }
        catch (Exception e) {
            Debug.Log(e);
        }
    }
    void onApplicationQuit()
    {
        if (con != null) {
            if (con.State.ToString() != "Closed") {
                con.Close();
                Debug.Log("Mysql connection closed");
            }
            con.Dispose();
        }
    }

    public string getFirstShops()
    {
        using (rdr = cmd.ExecuteReader()) {
            while (rdr.Read()) {
                return rdr[0] + " -- " + rdr[1];
            }
        }
        return "empty";
    }
    public string GetConnectionState()
    {
        return con.State.ToString();
    }
}