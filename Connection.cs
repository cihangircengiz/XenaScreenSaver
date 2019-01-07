using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Screensaver.Computer
{
    class Connection
    {
        private static string serverip
        {
            get { return Form1.serverip?.ToString() ?? "****.com.tr"; }
        }

        private static string username="***";
        private static string password="***";
        private static string connectionString = "SERVER=" + serverip + ";" + "DATABASE='screensaver';" + "UID=" + username +
                                          ";" + "PASSWORD=" + password + ";";
        private MySqlConnection connection = new MySqlConnection(connectionString);
        public List<string> multiValueQuery(string sql)
        {
            List<string> _tmp = new List<string>();
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader dataReader;
                //Create a data reader and Execute the command
                try
                {
                    dataReader = cmd.ExecuteReader();
                }
                catch (Exception)
                {
                    CheckTables();
                    dataReader = cmd.ExecuteReader();
                }
                
                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        try
                        {
                            _tmp.Add((string)dataReader.GetValue(i));
                        }
                        catch (Exception)
                        {
                            _tmp.Add(Convert.ToString(dataReader.GetValue(i)));
                        }
                        
                    }
                }
                dataReader.Close();
                this.CloseConnection();
                return _tmp;
            }
            else
            {
                return _tmp;
            }       
        }
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private void CheckTables()
        {                string sql = "ALTER TABLE settings CONVERT TO CHARACTER SET utf32 COLLATE utf32_general_ci";
                //Create Command
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                var x = cmd.ExecuteNonQuery();
        }
    }
}
