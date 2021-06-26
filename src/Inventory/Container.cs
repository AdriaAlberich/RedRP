/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
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
    /// Garbage Container class and methods
    /// </summary>
    public class Container : Script
    {
        // Debug mode
        public static bool debugMode = false;

        // CONTAINER ATTRIBUTES
        public int sqlid { get; set; }
        public Vector3 position { get; set; }
        public uint dimension { get; set; }
        public int garbage { get; set; } // Non-persistant
        public Inventory inventory { get; set; }
        public Marker debugMarker { get; set; } // Non-persistant

        /// <summary>
        /// Creates a new garbage container
        /// </summary>
        /// <param name="position">Container position</param>
        /// <param name="dimension">Container dimension</param>
        /// <returns>True if created successfully, false otherwise</returns>
        public static bool Create(Vector3 position, uint dimension)
        {
            try
            {
                Container newContainer = new Container();
                newContainer.position = position;
                newContainer.dimension = dimension;
                newContainer.garbage = Global.MaxGarbagePerContainer;

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
                        tempCmd.CommandText = "INSERT INTO Containers (x, y, z, dimension, inventory) " +
                            "VALUES (@x, @y, @z, @dimension, @inventory)";
                        tempCmd.Parameters.AddWithValue("@x", position.X);
                        tempCmd.Parameters.AddWithValue("@y", position.Y);
                        tempCmd.Parameters.AddWithValue("@z", position.Z);
                        tempCmd.Parameters.AddWithValue("@dimension", dimension);

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            int newContainerId = (int)tempCmd.LastInsertedId;
                            Log.Debug("[CONTAINERS] Creado contenedor de basura " + newContainerId);

                            newContainer.sqlid = newContainerId;

                            // Create a new inventory
                            newContainer.inventory = Inventory.Create(newContainerId, (int)Global.InventoryType.GarbageContainer, newContainer);
                            Util.UpdateDBField("Containers", "inventory", newContainer.inventory.sqlid.ToString(), "id", newContainer.sqlid.ToString());

                            Global.Containers.Add(newContainer);

                            if (debugMode)
                            {
                                newContainer.debugMarker = NAPI.Marker.CreateMarker(28, newContainer.position, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.5f, new Color(255, 0, 0));
                            }

                            return true;
                        }
                        else
                        {
                            Log.Debug("[CONTAINERS] No se pudo crear el contenedor de basura.");
                            return false;
                        }

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
        /// Load all the garbage containers from database
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        public static bool Load()
        {
            try
            {
                // Instantiate a new disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // Instantiate a new disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Sets the command query text
                        tempCmd.CommandText = "SELECT * FROM Containers";
                        // Reader disposable object
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            // Read all the result 
                            while (tempReader.Read())
                            {
                                // New garbage container
                                Container newContainer = new Container();

                                // Initialization
                                newContainer.sqlid = tempReader.GetInt32("id");
                                newContainer.position = new Vector3(tempReader.GetFloat("x"), tempReader.GetFloat("y"), tempReader.GetFloat("z"));
                                newContainer.dimension = tempReader.GetUInt32("dimension");
                                newContainer.garbage = Global.MaxGarbagePerContainer;
                                newContainer.inventory = Inventory.Load(tempReader.GetInt32("inventory"), newContainer);

                                // Add new container to the global list
                                Global.Containers.Add(newContainer);
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
        /// Generate garbage for all containers
        /// </summary>
        public static void GenerateGarbage()
        {
            foreach(Container container in Global.Containers)
            {
                if(container.garbage < Global.MaxGarbagePerContainer)
                {
                    container.garbage++;
                }
            }
        }

        /// <summary>
        /// Returns the nearest garbage container from a player
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="distance">The maximum distance</param>
        /// <returns>The container object</returns>
        public static Container GetNearest(Player player, double distance)
        {
            // Define a null pointer
            Container containerFound = null;

            // Iterate all containers
            foreach (Container container in Global.Containers)
            {
                // If the container is in range
                if (container.position.DistanceTo(player.user.Position) <= distance)
                {
                    // Get the instance pointer
                    containerFound = container;
                    break;
                }
            }

            return containerFound;
        }

        /// <summary>
        /// Shows the garbage container menu to a player
        /// </summary>
        /// <param name="player">The player instance</param>
        public void ShowMenu(Player player)
        {
            Menu menu = new Menu("", "Contenedor", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnMenuResponse));

            menu.menuModel.items.Add(new MenuItemModel("Examinar", "Revisar el contenido"));

            menu.menuModel.items.Add(new MenuItemModel("Recoger basura", "Recoger la basura con una bolsa"));

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Manage the container menu actions
        /// </summary>
        /// <param name="player">The player who interacts</param>
        /// <param name="option">The selected option</param>
        public void OnMenuResponse(Player player, string option, int optionId)
        {
            switch (optionId)
            {
                case 0:
                    {
                        Inventory.OpenInventoryForPlayer(player, this.inventory, "Contenedor");
                        break;
                    }
            }
        }

    }
}
