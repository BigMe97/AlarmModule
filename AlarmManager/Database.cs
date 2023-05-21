using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AlarmManager
{
    public static class Database
    {
        private static string connetionString;
        private static SqlConnection connection;
        public static bool connected;
        public static string dbHost;
        public static string dbName;
        public static string dbUserName;
        public static string dbPassword;
        private static int DeviceChecksum;
        private static int MeasurementChecksum;


        public static void LoadFromXML(string filepath)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(filepath);
            XmlNode SQLNodes = xml.SelectSingleNode("SQL");

            string dbHost = SQLNodes.SelectSingleNode("DBHost").InnerText;
            string dbName = SQLNodes.SelectSingleNode("DBName").InnerText;
            string dbUserName = SQLNodes.SelectSingleNode("User").InnerText;
            string dbPassword = SQLNodes.SelectSingleNode("Password").InnerText;
            if (SQLNodes.SelectSingleNode("LocalHost").InnerText == "true")
            {
                dbHost = System.Environment.MachineName + dbHost;
            }
            connetionString = String.Format("Data Source={0}; Initial Catalog={1}; User id={2}; Password={3};", dbHost, dbName, dbUserName, dbPassword);
            Console.WriteLine($"Connection String:   {connetionString}");
            connection = new SqlConnection(connetionString);
            connection.Open();
            connected = true;
        }


        private static object OneLiner(string query)
        {
            object line;
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                line = reader[0];
                reader.Close();
            }
            
            return line;
        }



        // Alarm
        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
        // 
        public static bool ChecksumDevices()
        {
            string query = $"SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM DEVICE";

            int checksum = Convert.ToInt32(OneLiner(query));
            if (checksum == DeviceChecksum)
            {
                return true;
            }
            else
            {
                DeviceChecksum = checksum;
                return false;
            }
            
            
        }

        public static bool ChecksumMeasurements()
        {
            string query = "SELECT CHECKSUM_AGG(BINARY_CHECKSUM(*)) FROM MEASUREMENT";
            
            int checksum = Convert.ToInt32(OneLiner(query));
            if (checksum == MeasurementChecksum)
                return true;
            else
            {
                MeasurementChecksum = checksum;
                return false;
            }
            

        }


        public static List<Device> ReadDevices()
        {
            List<Device> devices = new List<Device>();
            string query = "SELECT * FROM [DEVICE]";
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Device newDevice = new Device((int)reader[2]);
                    newDevice.unit = (string)reader[0];
                    newDevice.deviceName = (string)reader[1];
                    newDevice.hiLimit = (double)reader[4];
                    newDevice.lowLimit = (double)reader[5];
                    newDevice.hiHiLimit = (double)reader[6];
                    newDevice.lowLowLimit = (double)reader[7];
                    devices.Add(newDevice);
                    Thread.Sleep(300);
                }
                reader.Close();
            }
            return devices;
            
        }


        public static Device ReadMeasurement(Device device)
        {
            string query = $"SELECT LoggedValue, LoggedTime FROM LAST_MEASUREMENT WHERE DeviceID = {device.ID}";
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                object[] values;
                reader.Read();
                try
                {
                    device.value = (double)reader[0];
                    device.time = (DateTime)reader[1];
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Fault : {e.Message}");
                }
                reader.Close();
                return device;

            }
        }


        public static List<Alarm> ReadAlarms()
        {
            List<Alarm> list = new List<Alarm>();


            return list;
        }

        // Users
        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
        //

        public static bool ValidateUser(int ID, string pass)
        {
            string storedPassword;
            string query = $"SELECT UserPassword FROM [USER] WHERE UserID = {ID}";
            SqlDataReader reader;

            storedPassword = Convert.ToString(OneLiner(query));
            if (pass == storedPassword)
                return true;
            else 
                return false;
           
            
        }

        public static void SetUser(int userID)
        {
            string query = $"SELECT UserName, UserRole, PermissionLevel FROM [USER] WHERE UserID = {userID}";
            using (SqlCommand command = new SqlCommand(query, connection))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                reader.Read();
                User.Name = (string)reader["UserName"];
                User.Role = (string)reader["UserRole"];
                User.Permission = (int)reader["PermissionLevel"];
                reader.Close();
            }
        }


        




        // Get values
        // # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # # #
        //

        public static string GetDBHost()
        {
            return dbHost;
        }
        public static string GetDBName()
        {
            return dbName;
        }
        public static string GetDBUserName()
        {
            return dbUserName;
        }
        public static string GetDBPassword()
        {
            return dbPassword;
        }

    }
}
