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
    /// ItemData class and methods, contain all the item data from database
    /// </summary>
    public class ItemData : Script
    {

        // ITEM DATA ATTRIBUTES

        public int sqlid { get; set; }
        public string nameSingular { get; set; }
        public string namePlural { get; set; }
        public int sex { get; set; }
        public int type { get; set; }
        public int quantity { get; set; }
        public int itemWeight { get; set; } // In grams
        public string image { get; set; }
        public bool isHeavy { get; set; }
        public int maxContentWeight { get; set; }
        public string[] canContainTheseItems { get; set; }
        public string[] isAmmoOf { get; set; }
        public int distributionPrice { get; set; }
        public int sellPrice { get; set; }
        public uint propModel { get; set; }
        public int addictionLevel { get; set; }
        public int nutritionalLevel { get; set; }
        public int thirstLevel { get; set; }
        public int alcoholLevel { get; set; }
        public string weaponModel { get; set; }
        public int maleVariation { get; set; }
        public int maleVariationTexture { get; set; }
        public int maleAlternativeVariation { get; set; }
        public int femaleVariation { get; set; }
        public int femaleVariationTexture { get; set; }
        public int femaleAlternativeVariation { get; set; }
        public Vector3 rightHandOffset { get; set; }
        public Vector3 rightHandRotation { get; set; }
        public Vector3 leftHandOffset { get; set; }
        public Vector3 leftHandRotation { get; set; }
        public Vector3 chestOffset { get; set; }
        public Vector3 chestRotation { get; set; }
        public Vector3 backOffset { get; set; }
        public Vector3 backRotation { get; set; }
        public Vector3 worldRotation { get; set; }
        public double worldZOffset { get; set; }

        /// <summary>
        /// Load all item data from database
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        public static bool Load()
        {
            try
            {
                // New item info list
                Global.ItemsData = new List<ItemData>();

                // New disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // New disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Set the command query text
                        tempCmd.CommandText = "SELECT * FROM Items WHERE nameSingular <> 'DummyItem'";

                        // Reader disposable object
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {

                            // Read all the result 
                            while (tempReader.Read())
                            {
                                // New item
                                ItemData newItem = new ItemData();

                                // Initialization of item data
                                newItem.sqlid = tempReader.GetInt32("id");
                                newItem.nameSingular = tempReader.GetString("nameSingular");
                                newItem.namePlural = tempReader.GetString("namePlural");
                                newItem.sex = tempReader.GetInt32("sex");
                                newItem.type = tempReader.GetInt32("itemType");
                                newItem.quantity = tempReader.GetInt32("quantity");
                                newItem.itemWeight = tempReader.GetInt32("itemWeight");
                                newItem.image = tempReader.GetString("itemImage");
                                newItem.isHeavy = tempReader.GetBoolean("isHeavy");
                                newItem.maxContentWeight = tempReader.GetInt32("maxContentWeight");
                                string canContainTheseItems = tempReader.GetString("canContainTheseItems");
                                if (canContainTheseItems.Length > 0)
                                {
                                    if (canContainTheseItems.Length > 0 && !canContainTheseItems.Contains(","))
                                    {
                                        newItem.canContainTheseItems = new string[1];
                                        newItem.canContainTheseItems[0] = canContainTheseItems;
                                    }
                                    else
                                    {
                                        newItem.canContainTheseItems = canContainTheseItems.Split(',');
                                    }
                                }
                                else
                                {
                                    newItem.canContainTheseItems = null;
                                }
                                string isAmmoOf = tempReader.GetString("isAmmoOf");
                                if (isAmmoOf.Length > 0)
                                {
                                    if (isAmmoOf.Length > 0 && !isAmmoOf.Contains(","))
                                    {
                                        newItem.isAmmoOf = new string[1];
                                        newItem.isAmmoOf[0] = isAmmoOf;
                                    }
                                    else
                                    {
                                        newItem.isAmmoOf = isAmmoOf.Split(',');
                                    }
                                }
                                else
                                {
                                    newItem.isAmmoOf = null;
                                }
                                newItem.distributionPrice = tempReader.GetInt32("distributionPrice");
                                newItem.sellPrice = tempReader.GetInt32("sellPrice");
                                newItem.propModel = tempReader.GetUInt32("propModel");
                                newItem.addictionLevel = tempReader.GetInt32("addictionLevel");
                                newItem.nutritionalLevel = tempReader.GetInt32("nutritionalLevel");
                                newItem.thirstLevel = tempReader.GetInt32("thirstLevel");
                                newItem.alcoholLevel = tempReader.GetInt32("alcoholLevel");
                                newItem.weaponModel = tempReader.GetString("weaponModel");
                                newItem.maleVariation = tempReader.GetInt32("maleVariation");
                                newItem.maleVariationTexture = tempReader.GetInt32("maleVariationTexture");
                                newItem.maleAlternativeVariation = tempReader.GetInt32("maleAlternativeVariation");
                                newItem.femaleVariation = tempReader.GetInt32("femaleVariation");
                                newItem.femaleVariationTexture = tempReader.GetInt32("femaleVariationTexture");
                                newItem.femaleAlternativeVariation = tempReader.GetInt32("femaleAlternativeVariation");
                                newItem.rightHandOffset = new Vector3();
                                newItem.rightHandOffset.X = tempReader.GetFloat("rightHandXOffset");
                                newItem.rightHandOffset.Y = tempReader.GetFloat("rightHandYOffset");
                                newItem.rightHandOffset.Z = tempReader.GetFloat("rightHandZOffset");
                                newItem.rightHandRotation = new Vector3();
                                newItem.rightHandRotation.X = tempReader.GetFloat("rightHandXRotation");
                                newItem.rightHandRotation.Y = tempReader.GetFloat("rightHandYRotation");
                                newItem.rightHandRotation.Z = tempReader.GetFloat("rightHandZRotation");
                                newItem.leftHandOffset = new Vector3();
                                newItem.leftHandOffset.X = tempReader.GetFloat("leftHandXOffset");
                                newItem.leftHandOffset.Y = tempReader.GetFloat("leftHandYOffset");
                                newItem.leftHandOffset.Z = tempReader.GetFloat("leftHandZOffset");
                                newItem.leftHandRotation = new Vector3();
                                newItem.leftHandRotation.X = tempReader.GetFloat("leftHandXRotation");
                                newItem.leftHandRotation.Y = tempReader.GetFloat("leftHandYRotation");
                                newItem.leftHandRotation.Z = tempReader.GetFloat("leftHandZRotation");
                                newItem.chestOffset = new Vector3();
                                newItem.chestOffset.X = tempReader.GetFloat("chestXOffset");
                                newItem.chestOffset.Y = tempReader.GetFloat("chestYOffset");
                                newItem.chestOffset.Z = tempReader.GetFloat("chestZOffset");
                                newItem.chestRotation = new Vector3();
                                newItem.chestRotation.X = tempReader.GetFloat("chestXRotation");
                                newItem.chestRotation.Y = tempReader.GetFloat("chestYRotation");
                                newItem.chestRotation.Z = tempReader.GetFloat("chestZRotation");
                                newItem.backOffset = new Vector3();
                                newItem.backOffset.X = tempReader.GetFloat("backXOffset");
                                newItem.backOffset.Y = tempReader.GetFloat("backYOffset");
                                newItem.backOffset.Z = tempReader.GetFloat("backZOffset");
                                newItem.backRotation = new Vector3();
                                newItem.backRotation.X = tempReader.GetFloat("backXRotation");
                                newItem.backRotation.Y = tempReader.GetFloat("backYRotation");
                                newItem.backRotation.Z = tempReader.GetFloat("backZRotation");
                                newItem.worldRotation = new Vector3();
                                newItem.worldRotation.X = tempReader.GetFloat("worldXRotation");
                                newItem.worldRotation.Y = tempReader.GetFloat("worldYRotation");
                                newItem.worldRotation.Z = tempReader.GetFloat("worldZRotation");
                                newItem.worldZOffset = tempReader.GetDouble("worldZOffset");

                                // Add new item to the global list
                                Global.ItemsData.Add(newItem);
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
        /// Makes a composed name to make phrases
        /// </summary>
        /// <param name="plural">If the item should be spelled as plural</param>
        /// <returns>The composed name</returns>
        public string GetComposedName(bool plural)
        {
            string composedName;
            if(plural)
            {
                if(this.sex == (int)Global.Sex.Male)
                {
                    composedName = "unos " + this.namePlural;
                }
                else
                {
                    composedName = "unas " + this.namePlural;
                }
            }
            else
            {
                if (this.sex == (int)Global.Sex.Male)
                {
                    composedName = "un " + this.nameSingular;
                }
                else
                {
                    composedName = "una " + this.nameSingular;
                }
            }

            return composedName;
        }

        /// <summary>
        /// Returns the item data by its database identifier
        /// </summary>
        /// <param name="id">The database identifier</param>
        /// <returns>The item data object</returns>
        public static ItemData GetById(int id)
        {
            ItemData itemFound = null;
            foreach(ItemData item in Global.ItemsData)
            {
                if(item.sqlid == id)
                {
                    itemFound = item;
                }
            }

            return itemFound;
        }

        /// <summary>
        /// Checks if the item can contain other items of the specified identifier
        /// </summary>
        /// <param name="id">The other item id</param>
        /// <returns>true if yes, false if no</returns>
        public bool CanContainItem(int id)
        {
            bool canContainItem = false;
            if(this.canContainTheseItems != null)
            {
                foreach (string itemId in this.canContainTheseItems)
                {
                    if (int.Parse(itemId) == id)
                    {
                        canContainItem = true;
                        break;
                    }
                }
            }
            else
            {
                canContainItem = true;
            }

            return canContainItem;
        }

        /// <summary>
        /// Checks if this item is compatible ammo of the specified item id
        /// </summary>
        /// <param name="id">The weapon item id</param>
        /// <returns>true if yes, false if no</returns>
        public bool IsAmmoOfWeapon(int id)
        {
            bool isAmmoOfWeapon = false;
            if (this.isAmmoOf != null)
            {
                foreach (string itemId in this.isAmmoOf)
                {
                    if (int.Parse(itemId) == id)
                    {
                        isAmmoOfWeapon = true;
                        break;
                    }
                }
            }

            return isAmmoOfWeapon;
        }

    }

}
