/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redrp
{
    /// <summary>
    /// Custom text logs
    /// </summary>
    public class Log
    {
        /// <summary>
        /// Relative logs path
        /// </summary>
        private static string dirLogs = @"logs";

        /// <summary>
        /// Log system initialization
        /// </summary>
        public static void Initialize()
        {
            try
            {
                List<string> logs = new List<string>();
                // Add here the list of available log types.
                logs.Add("debug");
                logs.Add("general");
                logs.Add("admin");
                logs.Add("commands");

                // Check if log directories exist, if not then creates them
                foreach (string log in logs)
                {
                    string tempPath = dirLogs + "/" + log;
                    if (!File.Exists(tempPath))
                    {
                        Directory.CreateDirectory(tempPath);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

        }

        /// <summary>
        /// Registers a log line
        /// </summary>
        /// <param name="type">The type of log</param>
        /// <param name="text">The text</param>
        public static void Register(string type, string text)
        {
            try
            {
                // Get time and date
                DateTime date = DateTime.UtcNow;

                // Give format to the path and file name
                string tempPath = string.Format("{0}/{1}/{2}-{3}-{4}.log", dirLogs, type, date.Day, date.Month, date.Year);

                // Check if log file exist, otherwise create it.
                if (!File.Exists(tempPath))
                {
                    StreamWriter tempWr = File.CreateText(tempPath);
                    tempWr.Close();

                }

                // Open file to write
                StreamWriter wr = File.AppendText(tempPath);

                // Give format to the message
                string finalLog = string.Format("\r[{0:D2}:{1:D2}:{2:D2}] {3}", date.Hour, date.Minute, date.Second, text);

                // Write the message on the file
                wr.WriteLine(finalLog);

                // Clean the buffer and close writer.
                wr.Flush();
                wr.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// General log method
        /// </summary>
        /// <param name="text">Log text</param>
        public static void GeneralLog(string text)
        {
            Console.WriteLine(text);
            Register("general", text);
        }

        /// <summary>
        /// Debug log
        /// </summary>
        /// <param name="text">The debug text</param>
        public static void Debug(string text)
        {
            Console.WriteLine("[DEBUG] " + text);
            Register("debug", text);
        }

        /// <summary>
        /// Admin log
        /// </summary>
        /// <param name="text">The log text</param>
        public static void AdminLog(string text)
        {
            Register("admin", text);
        }

        /// <summary>
        /// Command log
        /// </summary>
        /// <param name="text">The log text</param>
        public static void CmdLog(string text)
        {
            Register("commands", text);
        }

        /// <summary>
        /// Connection log (SQL)
        /// </summary>
        /// <param name="connectionType">0 = panel, 1 = game, 2 = character</param>
        /// <param name="targetId">The target sql id</param>
        /// <param name="targetName">The target name</param>
        /// <param name="connectionIp">The connection IP</param>
        public static void ConnectionLog(int connectionType, int targetId, string targetName, string connectionIp)
        {
            // Instantiate a new disposable connection
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                // Try to connect to database
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                // If success, instantiate a new disposable command
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    // Command query
                    tempCmd.CommandText = "INSERT INTO ConnectionLog VALUES (@connectionType, @targetId, @targetName, @connectionIp)";
                    tempCmd.Parameters.AddWithValue("@connectionType", connectionType);
                    tempCmd.Parameters.AddWithValue("@targetId", targetId);
                    tempCmd.Parameters.AddWithValue("@targetName", targetName);
                    tempCmd.Parameters.AddWithValue("@connectionIp", connectionIp);

                    if (tempCmd.ExecuteNonQuery() == 0)
                    {
                        Debug("[ConnectionLog] Error al insertar en el log de conexiones: " + connectionType + " " + targetId + " " + targetName + " " + connectionIp);
                    }
                }
            }
        }

        /// <summary>
        /// Kick log (SQL)
        /// </summary>
        /// <param name="playerId">The player sql id</param>
        /// <param name="playerName">The player name</param>
        /// <param name="adminId">The admin sql id</param>
        /// <param name="adminName">The admin name</param>
        /// <param name="reason">The reason</param>
        public static void KickLog(int playerId, string playerName, int adminId, string adminName, string reason)
        {
            // Instantiate a new disposable connection
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                // Try to connect to database
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                // If success, instantiate a new disposable command
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    // Command query
                    tempCmd.CommandText = "INSERT INTO KickLog VALUES (@playerId, @playerName, @adminId, @adminName, @reason)";
                    tempCmd.Parameters.AddWithValue("@playerId", playerId);
                    tempCmd.Parameters.AddWithValue("@playerName", playerName);
                    tempCmd.Parameters.AddWithValue("@adminId", adminId);
                    tempCmd.Parameters.AddWithValue("@adminName", adminName);
                    tempCmd.Parameters.AddWithValue("@reason", reason);

                    if (tempCmd.ExecuteNonQuery() == 0)
                    {
                        Debug("[KickLog] Error al insertar en el log de expulsiones: " + playerId + " " + playerName + " " + adminId + " " + adminName + " " + reason);
                    }
                }
            }
        }

        /// <summary>
        /// Ban log (SQL)
        /// </summary>
        /// <param name="playerId">The player sql id</param>
        /// <param name="playerName">The player name</param>
        /// <param name="adminId">The admin sql id</param>
        /// <param name="adminName">The admin name</param>
        /// <param name="reason">The reason</param>
        /// <param name="banDuration">The ban duration</param>
        public static void BanLog(int playerId, string playerName, int adminId, string adminName, string reason, int banDuration)
        {
            // Instantiate a new disposable connection
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                // Try to connect to database
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                // If success, instantiate a new disposable command
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    // Command query
                    tempCmd.CommandText = "INSERT INTO BanLog VALUES (@playerId, @playerName, @adminId, @adminName, @reason, @banDuration)";
                    tempCmd.Parameters.AddWithValue("@playerId", playerId);
                    tempCmd.Parameters.AddWithValue("@playerName", playerName);
                    tempCmd.Parameters.AddWithValue("@adminId", adminId);
                    tempCmd.Parameters.AddWithValue("@adminName", adminName);
                    tempCmd.Parameters.AddWithValue("@reason", reason);
                    tempCmd.Parameters.AddWithValue("@banDuration", banDuration);

                    if (tempCmd.ExecuteNonQuery() == 0)
                    {
                        Debug("[BanLog] Error al insertar en el log de bloqueos: " + playerId + " " + playerName + " " + adminId + " " + adminName + " " + reason + " " + banDuration);
                    }
                }
            }
        }

        /// <summary>
        /// Event log (SQL)
        /// </summary>
        /// <param name="playerId">The player sql id</param>
        /// <param name="playerName">The player name</param>
        /// <param name="adminId">The admin sql id</param>
        /// <param name="adminName">The admin name</param>
        /// <param name="reason">The reason</param>
        /// <param name="banDuration">The ban duration</param>
        public static void EventLog(int entityId, int entityType, int targetEntityId, int targetEntityType, string eventDescription, string miscData)
        {
            // Instantiate a new disposable connection
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                // Try to connect to database
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                // If success, instantiate a new disposable command
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    // Command query
                    tempCmd.CommandText = "INSERT INTO EventLog VALUES (@entityId, @entityType, @targetEntityId, @targetEntityType, @eventDescription, @miscData)";
                    tempCmd.Parameters.AddWithValue("@entityId", entityId);
                    tempCmd.Parameters.AddWithValue("@entityType", entityType);
                    tempCmd.Parameters.AddWithValue("@targetEntityId", targetEntityId);
                    tempCmd.Parameters.AddWithValue("@targetEntityType", targetEntityType);
                    tempCmd.Parameters.AddWithValue("@eventDescription", eventDescription);
                    tempCmd.Parameters.AddWithValue("@miscData", miscData);

                    if (tempCmd.ExecuteNonQuery() == 0)
                    {
                        Debug("[EventLog] Error al insertar en el log de eventos: " + entityId + " " + entityType + " " + targetEntityId + " " + targetEntityType + " " + eventDescription + " " + miscData);
                    }
                }
            }
        }
    }
}
