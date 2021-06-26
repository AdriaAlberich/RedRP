/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
 */


using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace redrp
{
    public class Whitelist
    {
        public static bool Check(string socialClub)
        {
            bool isWhitelisted = false;
            try
            {
                // Disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // Success
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Command query
                        tempCmd.CommandText = "SELECT * FROM redrp_whitelist WHERE socialClub = @socialClub";
                        tempCmd.Parameters.AddWithValue("@socialClub", socialClub);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.Read())
                            {
                                isWhitelisted = true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }

            return isWhitelisted;
        }

        public static bool Add(string socialClub)
        {
            try
            {
                // Disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // Success
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Command query
                        tempCmd.CommandText = "INSERT INTO redrp_whitelist VALUES (@socialClub)";
                        tempCmd.Parameters.AddWithValue("@socialClub", socialClub);
                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            Log.Debug("[Whitelist] Player " + socialClub + " added to whitelist.");
                            return true;
                        }
                        else
                        {
                            Log.Debug("[Whitelist] Player " + socialClub + " already in whitelist.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }

            return false;
        }

        public static bool Remove(string socialClub)
        {
            try
            {
                // Disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // Success
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Command query
                        tempCmd.CommandText = "DELETE FROM redrp_whitelist WHERE socialClub = @socialClub";
                        tempCmd.Parameters.AddWithValue("@socialClub", socialClub);
                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            Log.Debug("[Whitelist] Player " + socialClub + " was removed from whitelist.");
                            return true;
                        }
                        else
                        {
                            Log.Debug("[Whitelist] Player " + socialClub + " was already removed from whitelist.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }

            return false;
        }
    }
}
