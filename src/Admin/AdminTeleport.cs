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
using MySql.Data.MySqlClient;

namespace redrp
{
    /// <summary>
    /// Admin fixed teleport system
    /// </summary>
    public class AdminTeleport : Script
    {

        // TELEPORT ATTRIBUTES

        /// <summary>
        /// Database id
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Teleport name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Teleport position
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// Teleport heading
        /// </summary>
        public double heading { get; set; }

        /// <summary>
        /// Teleport dimension
        /// </summary>
        public uint dimension { get; set; }
        

        /// <summary>
        /// Load admin teleports from database
        /// </summary>
        /// <returns>True if success, false if failure</returns>
        public static bool Load()
        {
            try
            {
                // New teleport list
                Global.AdminTeleports = new List<AdminTeleport>();

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
                        tempCmd.CommandText = "SELECT * FROM AdminTeleports WHERE active = 1";
                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            while (tempReader.HasRows)
                            {
                                tempReader.Read();
                                AdminTeleport newTeleport = new AdminTeleport();
                                newTeleport.sqlid = tempReader.GetInt32("id");
                                newTeleport.name = tempReader.GetString("tpName");
                                newTeleport.position = new Vector3(tempReader.GetDouble("x"), tempReader.GetDouble("y"), tempReader.GetDouble("z"));
                                newTeleport.heading = tempReader.GetDouble("a");
                                newTeleport.dimension = tempReader.GetUInt32("dimension");

                                // Add new teleport to the global list
                                Global.AdminTeleports.Add(newTeleport);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }

        }

        /// <summary>
        /// Creates a new admin teleport
        /// </summary>
        /// <param name="name">Name of the new teleport</param>
        /// <param name="position">Teleport position</param>
        /// <param name="heading">Teleport heading</param>
        /// <param name="dimension">Teleport dimension</param>
        /// <returns>True if success and teleports reloaded successfully, false otherwise</returns>
        public static bool Create(string name, Vector3 position, double heading, uint dimension)
        {
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    tempCmd.CommandText = "INSERT INTO AdminTeleports (tpName, x, y, z, a, dimension) VALUES (@name, @posx, @posy, @posz, @heading, @dimension)";
                    tempCmd.Parameters.AddWithValue("@name", name);
                    tempCmd.Parameters.AddWithValue("@posx", position.X);
                    tempCmd.Parameters.AddWithValue("@posy", position.Y);
                    tempCmd.Parameters.AddWithValue("@posz", position.Z);
                    tempCmd.Parameters.AddWithValue("@heading", heading);
                    tempCmd.Parameters.AddWithValue("@dimension", dimension);

                    tempCmd.ExecuteNonQuery();
                }
            }

            return Load();
        }

        /// <summary>
        /// Update teleport data
        /// </summary>
        public void Save()
        {
            using (MySqlConnection tempConnection = new MySqlConnection())
            {
                tempConnection.ConnectionString = Global.ConnectionString;
                tempConnection.Open();
                using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                {
                    tempCmd.CommandText = "UPDATE AdminTeleports SET tpName = @name, x = @posx, y = @posy, z = @posz, a = @heading, dimension = @dimension WHERE id = @sqlid";
                    tempCmd.Parameters.AddWithValue("@name", this.name);
                    tempCmd.Parameters.AddWithValue("@posx", this.position.X);
                    tempCmd.Parameters.AddWithValue("@posy", this.position.Y);
                    tempCmd.Parameters.AddWithValue("@posz", this.position.Z);
                    tempCmd.Parameters.AddWithValue("@heading", this.heading);
                    tempCmd.Parameters.AddWithValue("@dimension", this.dimension);
                    tempCmd.Parameters.AddWithValue("@sqlid", this.sqlid);

                    tempCmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Gets a teleport instance by its id
        /// </summary>
        /// <param name="id">The teleport id</param>
        /// <returns>The teleport instance, null if not found</returns>
        public static AdminTeleport GetById(int id)
        {
            AdminTeleport teleportFound = null;
            foreach(AdminTeleport teleport in Global.AdminTeleports)
            {
                if(teleport.sqlid == id)
                {
                    teleportFound = teleport;
                    break;
                }
            }

            return teleportFound;
        }

        /// <summary>
        /// Generates the teleport selection menu
        /// </summary>
        /// <param name="player">The target player</param>
        public static void GenerateSelectionMenu(Player player)
        {
            Menu menu = new Menu("", "Localizaciones", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnAdminTeleport));

            int count = 0;
            foreach (AdminTeleport teleport in Global.AdminTeleports)
            {
                menu.menuModel.items.Add(new MenuItemModel("[" + teleport.sqlid + "] " + teleport.name));
                count++;
            }

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Manages admin teleport menu responses
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="option">The selected option as string</param>
        /// <param name="actionId">The selected option</param>
        public static void OnAdminTeleport(Player player, string option, int actionId)
        {
            AdminTeleport teleport = Global.AdminTeleports[actionId];
            if (teleport != null)
            {
                teleport.Teleport(player);
            }
            else
            {
                player.DisplayNotification("~r~Esta localización ya no existe.");
            }
        }

        /// <summary>
        /// Performs a safe teleport to the destination
        /// </summary>
        /// <param name="player">The target player</param>
        public void Teleport(Player player)
        {
            player.SetPosition(this.position, new Vector3(0.0, 0.0, this.heading));
            player.user.Dimension = this.dimension;
        }
    }
}
