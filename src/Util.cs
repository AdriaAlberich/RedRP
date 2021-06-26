/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using System.Net.Mail;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace redrp
{
    /// <summary>
    /// Util static methods
    /// </summary>
    public static class Util
    {

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Returns bool if string is a number or not
        /// </summary>
        /// <param name="input">The string to test</param>
        /// <returns>True if is numeric, false otherwise</returns>
        public static bool IsNumeric(string input)
        {
            int test;
            return int.TryParse(input, out test);
        }

        /// <summary>
        /// Sends a 3D chat message to nearby players
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="origin">Position where the message is broadcasted</param>
        /// <param name="radius">Radius where the message will be visible</param>
        /// <param name="dimension">Dimension where the message will be visible</param>
        public static void SendChatMessage3D(string message, Vector3 origin, float radius, uint dimension = 0)
        {
            foreach(Player player in Player.Players)
            {
                if(origin.DistanceTo(player.user.Position) <= radius && player.user.Dimension == dimension)
                {
                    NAPI.Chat.SendChatMessageToPlayer(player.user, message);
                }
            }
        }

        /// <summary>
        /// Checks if string is email address
        /// </summary>
        /// <param name="email">The string to test</param>
        /// <returns>True if is email, false otherwise</returns>
        public static bool IsEmailAddress(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a 2D coordinate inside a vector giving a distance from the origin
        /// </summary>
        /// <param name="pos">Origin position</param>
        /// <param name="angle">Vector angle</param>
        /// <param name="distance">Distance from the origin</param>
        /// <returns></returns>
        public static Vector3 GetPositionInsideVector(Vector3 origin, float angle, float distance)
        {
            angle *= (float)Math.PI / 180;
            origin.X += (distance * (float)Math.Sin(-angle));
            origin.Y += (distance * (float)Math.Cos(-angle));
            Vector3 position = new Vector3(origin.X, origin.Y, origin.Z);
            return origin;
        }

        /// <summary>
        /// Calculates the UNIX timestamp
        /// </summary>
        /// <returns>The unix timestamp as long</returns>
        public static long GetCurrentTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(Epoch)).TotalSeconds;
        }

        /// <summary>
        /// Converts a datetime to timestamp
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ConvertToTimestamp(DateTime value)
        {
            TimeSpan elapsedTime = value - Epoch;
            return (long)elapsedTime.TotalSeconds;
        }

        /// <summary>
        /// Fixes bad tilde formating in some non UTF chats
        /// </summary>
        /// <param name="text">The text to analyze</param>
        /// <returns>The text with the corrections</returns>
        public static string ProcessAccents(string text)
        {
            text = text.Replace("´a", "á");
            text = text.Replace("´e", "é");
            text = text.Replace("´i", "í");
            text = text.Replace("´o", "ó");
            text = text.Replace("´u", "ú");

            text = text.Replace("`a", "à");
            text = text.Replace("`e", "è");
            text = text.Replace("`o", "ò");

            return text;
        }

        /// <summary>
        /// Generates a pseudorandom number
        /// </summary>
        /// <param name="max">Max random number from 0</param>
        /// <returns>A pseudorandom number</returns>
        public static int Random(int max)
        {
            return new Random().Next(0, max);
        }

        /// <summary>
        /// Checks if database connection is alive
        /// </summary>
        /// <returns>True if is alive, false otherwise</returns>
        public static bool CheckDBConnection()
        {
            bool check = false;
            //We instantiate a new disposable connection
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                //Try to connect to database
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                //Check if connection returns ping
                check = tempConnection.Ping();
            }

            return check;
        }

        /// <summary>
        /// Gets a single column value from database
        /// </summary>
        /// <param name="table">The database table</param>
        /// <param name="column">The column name</param>
        /// <param name="key">The column that will act as a key</param>
        /// <param name="keyValue">The string value of the key</param>
        /// <returns>Returns the value as a string if found, otherwise returns null</returns>
        public static string GetDBField(string table, string column, string key, string keyValue)
        {
            try
            {
                string result = null;
                //Disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    //Connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    //Success
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        //Command query
                        tempCmd.CommandText = "SELECT " + column + " FROM " + table + " WHERE " + key + " = " + "@" + key + " LIMIT 1";
                        tempCmd.Parameters.AddWithValue("@" + key, keyValue);
                        //Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.HasRows)
                            {
                                tempReader.Read();
                                result = tempReader.GetString(column);
                            }

                            return result;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Updates a single column value from database
        /// </summary>
        /// <param name="table">The database table</param>
        /// <param name="column">The column name</param>
        /// <param name="columnValue">The new column value</param>
        /// <param name="key">The column that will act as a key</param>
        /// <param name="keyValue">The string value of the key</param>
        /// <returns>Returns true if success, false if not found or in case of error</returns>
        public static bool UpdateDBField(string table, string column, string columnValue, string key, string keyValue)
        {
            try
            {
                bool success = false;
                //Disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    //Connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    //Success
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        //Command query
                        tempCmd.CommandText = "UPDATE " + table + " SET " + column + " = " + "@" + column + " WHERE " + key + " = " + "@" + key + " LIMIT 1";
                        tempCmd.Parameters.AddWithValue("@" + column, columnValue);
                        tempCmd.Parameters.AddWithValue("@" + key, keyValue);
                        int rows = tempCmd.ExecuteNonQuery();

                        if (rows > 0)
                        {
                            success = true;
                        }

                        return success;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Creates a SHA256 hash representation of a string
        /// </summary>
        /// <param name="rawData">The raw string</param>
        /// <returns>The hash</returns>
        public static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

    }
}
