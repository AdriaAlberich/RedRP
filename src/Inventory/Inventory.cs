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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GTANetworkAPI;
using Object = GTANetworkAPI.Object;
using MySql.Data.MySqlClient;

namespace redrp
{
    /// <summary>
    /// Inventory system
    /// </summary>
    public class Inventory : Script
    {

        private class RootObject
        {
            public Item[] items;
        }

        // INVENTORY ATTRIBUTES

        /// <summary>
        /// Database identifier
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Database owner identifier
        /// </summary>
        public int ownerId { get; set; }

        /// <summary>
        /// Owner type
        /// </summary>
        public int ownerType { get; set; }

        /// <summary>
        /// Owner game object instance
        /// </summary>
        public object ownerInstance { get; set; }

        /// <summary>
        /// Inventory items
        /// </summary>
        public List<Item> items { get; set; }

        /// <summary>
        /// Inventory total weight in grams
        /// </summary>
        public int weight { get; set; } 

        /// <summary>
        /// Control variable for ammo
        /// </summary>
        public int ammoCount = 0;

        /// <summary>
        /// Control variable for saving
        /// </summary>
        public bool save = true;

        /// <summary>
        /// Right hand slot
        /// </summary>
        public Item rightHand { get; set; }

        /// <summary>
        /// Right hand slot prop instance
        /// </summary>
        public Object rightHandProp { get; set; }

        /// <summary>
        /// Left hand slot
        /// </summary>
        public Item leftHand { get; set; }

        /// <summary>
        /// Left hand slot prop instance
        /// </summary>
        public Object leftHandProp { get; set; }

        /// <summary>
        /// Bodyarmor slot
        /// </summary>
        public Item bodyarmor { get; set; }

        /// <summary>
        /// Backpack slot
        /// </summary>
        public Item backpack { get; set; }

        /// <summary>
        /// Gloves slot
        /// </summary>
        public Item gloves { get; set; }

        /// <summary>
        /// Hat slot
        /// </summary>
        public Item hat { get; set; }

        /// <summary>
        /// Glasses slot
        /// </summary>
        public Item glasses { get; set; }

        /// <summary>
        /// Mask slot
        /// </summary>
        public Item mask { get; set; }

        /// <summary>
        /// Accessory slot
        /// </summary>
        public Item accessory { get; set; }

        /// <summary>
        /// Ears slot
        /// </summary>
        public Item ears { get; set; }

        /// <summary>
        /// Torso slot
        /// </summary>
        public Item torso { get; set; }

        /// <summary>
        /// Legs slot
        /// </summary>
        public Item legs { get; set; }

        /// <summary>
        /// Feet slot
        /// </summary>
        public Item feet { get; set; }

        /// <summary>
        /// Watch slot
        /// </summary>
        public Item watch { get; set; }

        /// <summary>
        /// Bracelet slot
        /// </summary>
        public Item bracelet { get; set; }

        /// <summary>
        /// Light weapon first slot
        /// </summary>
        public Item lightWeapon1 { get; set; }

        /// <summary>
        /// Light weapon second slot
        /// </summary>
        public Item lightWeapon2 { get; set; }

        /// <summary>
        /// Heavy weapon slot
        /// </summary>
        public Item heavyWeapon { get; set; }

        /// <summary>
        /// Heavy weapon slot prop instance
        /// </summary>
        public Object heavyWeaponProp { get; set; }

        /// <summary>
        /// Melee light weapon first slot
        /// </summary>
        public Item meleeLightWeapon1 { get; set; }

        /// <summary>
        /// Melee light weapon second slot
        /// </summary>
        public Item meleeLightWeapon2 { get; set; }

        /// <summary>
        /// Melee heavy weapon slot
        /// </summary>
        public Item meleeHeavyWeapon { get; set; }

        /// <summary>
        /// Melee heavy weapon slot prop instance
        /// </summary>
        public Object meleeHeavyWeaponProp { get; set; }

        /// <summary>
        /// Throwable weapon first slot
        /// </summary>
        public Item throwableWeapon1 { get; set; }

        /// <summary>
        /// Throwable weapon second slot
        /// </summary>
        public Item throwableWeapon2 { get; set; }

        /// <summary>
        /// Special weapon slot
        /// </summary>
        public Item specialWeapon { get; set; }

        // EVENTS

        /// <summary>
        /// Triggered when player clicks an object on the inventory interface
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        [RemoteEvent("invItemClick")]
        public void OnItemClick(Client user, int isRight, int itemId)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                HandleItemClickEvent(player, isRight, itemId);
            }
        }

        /// <summary>
        /// Triggered when player double clicks an object on the inventory interface
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        [RemoteEvent("invItemDoubleClick")]
        public void OnItemDoubleClick(Client user, int isRight, int itemId)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                HandleItemDoubleClickEvent(player, isRight, itemId);
            }
        }

        /// <summary>
        /// Triggered when player right clicks an object on the inventory interface
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        [RemoteEvent("invItemRightClick")]
        public void OnItemRightClick(Client user, int isRight, int itemId)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                HandleItemRightClickEvent(player, isRight, itemId);
            }
        }

        /// <summary>
        /// Triggered when player clicks the return button on the inventory interface
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        [RemoteEvent("invReturnClick")]
        public void OnReturnClick(Client user, int isRight)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                HandleReturnClickEvent(player, isRight);
            }
        }

        /// <summary>
        /// Load an inventory from database
        /// </summary>
        /// <param name="id">The inventory identifier</param>
        /// <param name="owner">The owner instance</param>
        /// <returns></returns>
        public static Inventory Load(int id, object owner)
        {
            try
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
                        tempCmd.CommandText = "SELECT * FROM Inventories WHERE id = @id LIMIT 1";
                        tempCmd.Parameters.AddWithValue("@id", id);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.HasRows)
                            {
                                tempReader.Read();

                                // New inventory instance
                                Inventory newInventory = new Inventory();

                                // Initialization of inventory data
                                newInventory.sqlid = id;
                                newInventory.ownerId = tempReader.GetInt32("ownerId");
                                newInventory.ownerType = tempReader.GetInt32("ownerId");
                                newInventory.weight = 0;

                                string inventoryContentJSON = tempReader.GetString("content");

                                if (inventoryContentJSON != string.Empty)
                                {
                                    dynamic inventoryContent = NAPI.Util.FromJson(inventoryContentJSON);
                                    RootObject inventoryContentObject = inventoryContent.ToObject<RootObject>();

                                    newInventory.items = inventoryContentObject.items.ToList();

                                    foreach (Item item in newInventory.items)
                                    {
                                        // If inventory owner is a player, then we check if the current slot is an special one.
                                        if (newInventory.ownerType == (int)Global.InventoryType.Player)
                                        {
                                            if (item.IsWeapon())
                                            {
                                                newInventory.GiveWeapon(item, false);
                                            }
                                            else
                                            {
                                                switch (item.slot)
                                                {
                                                    case (int)Global.InventoryBodypart.RightHand:
                                                        {
                                                            newInventory.rightHand = item;
                                                            if (!newInventory.rightHand.IsWeapon())
                                                            {
                                                                // If is not a weapon but has a prop model, we set it visible to the right hand.
                                                                if (ItemData.GetById(item.id).propModel != 0)
                                                                {
                                                                    newInventory.SetPropModel((int)Global.InventoryBodypart.RightHand);
                                                                }
                                                            }

                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.LeftHand:
                                                        {
                                                            newInventory.leftHand = item;
                                                            // If has a prop model, we set it visible to the left hand.
                                                            if (ItemData.GetById(item.id).propModel != 0)
                                                            {
                                                                newInventory.SetPropModel((int)Global.InventoryBodypart.LeftHand);
                                                            }
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Bodyarmor:
                                                        {
                                                            // If it's bodyarmor then we set the variation visible.
                                                            // TODO: SET PLAYER BODY ARMOR VALUE
                                                            newInventory.backpack = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Bodyarmor, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Backpack:
                                                        {
                                                            // If it's backpack then we set the variation visible.
                                                            newInventory.backpack = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Backpack, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Gloves:
                                                        {
                                                            // If it's a pair of gloves then we set the variation visible
                                                            newInventory.gloves = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Gloves, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Hat:
                                                        {
                                                            // If it's a hat then we set the variation visible
                                                            newInventory.hat = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Hat, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Glasses:
                                                        {
                                                            // If it's a glasses then we set the variation visible
                                                            newInventory.glasses = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Glasses, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Mask:
                                                        {
                                                            // If it's a mask then we set the variation visible
                                                            // TODO: HIDE PLAYER IDENTITY ALSO
                                                            newInventory.mask = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Mask, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Accessory:
                                                        {
                                                            // If it's an accessory then we set the variation visible
                                                            newInventory.accessory = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Accessory, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Ears:
                                                        {
                                                            // If it's an ear accessory then we set the variation visible
                                                            newInventory.ears = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Ears, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Torso:
                                                        {
                                                            // If are torso clothes then we set the variation visible
                                                            newInventory.torso = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Torso, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Legs:
                                                        {
                                                            // If are trousers then we set the variation visible
                                                            newInventory.legs = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Legs, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Feet:
                                                        {
                                                            // If are shoes then we set the variation visible
                                                            newInventory.feet = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Feet, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Watch:
                                                        {
                                                            // If are shoes then we set the variation visible
                                                            newInventory.watch = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Watch, false);
                                                            break;
                                                        }
                                                    case (int)Global.InventoryBodypart.Bracelet:
                                                        {
                                                            // If are shoes then we set the variation visible
                                                            newInventory.bracelet = item;
                                                            newInventory.SetVariation((int)Global.InventoryBodypart.Bracelet, false);
                                                            break;
                                                        }

                                                }
                                            }
                                        }

                                        // Increment of the weight.
                                        newInventory.weight += ItemData.GetById(item.id).itemWeight;

                                    }
                                }
                                else
                                {
                                    newInventory.items = new List<Item>();
                                }

                                newInventory.ownerInstance = owner;

                                // Finally return the inventory instance
                                Log.Debug("Cargado inventario " + newInventory.sqlid);
                                Global.Inventories.Add(newInventory);
                                return newInventory;
                            }
                            else
                            {
                                return null;
                            }
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
        /// Creates a new inventory
        /// </summary>
        /// <param name="ownerId">The owner's database id</param>
        /// <param name="ownerType">The owner type</param>
        /// <param name="owner">The owner instance</param>
        /// <returns>The inventory instance</returns>
        public static Inventory Create(int ownerId, int ownerType, object owner)
        {
            try
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
                        tempCmd.CommandText = "INSERT INTO Inventories (ownerId, ownerType, content) " +
                            "VALUES (@ownerId, @ownerType, '')";
                        tempCmd.Parameters.AddWithValue("@ownerId", ownerId);
                        tempCmd.Parameters.AddWithValue("@ownerType", ownerType);

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            int newInventoryId = (int)tempCmd.LastInsertedId;
                            Log.Debug("[INVENTORY] Creado nuevo inventario num " + newInventoryId);

                            // Try to load the new inventory
                            Inventory newInventory = Load(ownerId, owner);

                            return newInventory;
                        }
                        else
                        {
                            Log.Debug("[INVENTORY] Error al crear nuevo inventario para " + ownerId + " " + ownerType);
                            return null;
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
        /// Returns the inventory max weight
        /// </summary>
        /// <returns></returns>
        public int GetMaxWeight()
        {
            int maxWeight = 0;
            switch(this.ownerType)
            {
                case (int)Global.InventoryType.Player:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.Player];
                        break;
                    }
                case (int)Global.InventoryType.VehicleTrunk:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.VehicleTrunk];
                        break;
                    }
                case (int)Global.InventoryType.VehicleGlover:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.VehicleGlover];
                        break;
                    }
                case (int)Global.InventoryType.House:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.House];
                        break;
                    }
                case (int)Global.InventoryType.Business:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.Business];
                        break;
                    }
                case (int)Global.InventoryType.GarbageContainer:
                    {
                        maxWeight = Global.InventoryCapacity[(int)Global.InventoryType.GarbageContainer];
                        break;
                    }
            }

            maxWeight *= 1000;
            return maxWeight;
        }

        /// <summary>
        /// Adds a new item to the inventory
        /// </summary>
        /// <param name="itemId">The item identifier</param>
        /// <param name="quantity">The item quantity</param>
        /// <param name="toSpecialSlot">If must be given to a special slot</param>
        /// <param name="equip">If must be equipped (weapons)</param>
        /// <returns>1 if success, 0 if inventory is full, -1 if item ID doesn't Exists, -2 if item is too heavy and isn't enough space</returns>
        public int AddNewItem(int itemId, int quantity = -1, int toSpecialSlot = -1, bool equip = false)
        {
            Item newItem = Item.CreateItem(itemId);
            if (quantity != -1)
                newItem.quantity = quantity;

            return AddItem(newItem, toSpecialSlot, equip);
        }

        /// <summary>
        /// Adds an already created item to the inventory
        /// </summary>
        /// <param name="item">The item instance</param>
        /// <param name="toSpecialSlot">If must be given to a special slot</param>
        /// <param name="equip">If must be equipped (weapons)</param>
        /// <returns>1 if success, 0 if inventory is full, -1 if item is null, -2 if item is too heavy and isn't enough space</returns>
        public int AddItem(Item item, int toSpecialSlot = -1, bool equip = false)
        {
            // Item Exists
            if (item != null)
            {
                // There is enough space to store the item
                if (this.GetMaxWeight() >= ItemData.GetById(item.id).itemWeight + this.weight)
                {
                    // Owner type
                    switch (this.ownerType)
                    {
                        // If owner is a player
                        case (int)Global.InventoryType.Player:
                            {
                                Player player = (Player)ownerInstance;
                                if (item.IsWeapon())
                                {
                                    ItemData data = ItemData.GetById(item.id);
                                    if (this.GetWeaponItem(NAPI.Util.WeaponNameToModel(data.weaponModel)) == null)
                                    {
                                        if(this.GiveWeapon(item, false))
                                        {
                                            // Finger prints
                                            if (this.gloves == null)
                                            {
                                                item.AddFingerPrint(player.character.cleanName);
                                            }
                                        }
                                        else
                                        {
                                            return -2;
                                        }
                                        
                                    }
                                    else
                                    {
                                        return -2;
                                    }
                                }
                                else
                                {
                                    if (toSpecialSlot != -1)
                                    {
                                        ItemData itemData = ItemData.GetById(item.id);
                                        switch (toSpecialSlot)
                                        {
                                            case (int)Global.InventoryBodypart.RightHand:
                                                {
                                                    // Right hand is free
                                                    if (this.rightHand == null)
                                                    {
                                                        if (this.leftHand != null)
                                                        {
                                                            if (this.leftHand.IsHeavy())
                                                            {
                                                                return -2;
                                                            }
                                                        }

                                                        this.rightHand = item;

                                                        // If is not a weapon but has a prop model, we set it visible to the right hand.
                                                        if (itemData.propModel != 0)
                                                        {
                                                            this.SetPropModel((int)Global.InventoryBodypart.RightHand);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.LeftHand:
                                                {
                                                    if (this.leftHand == null)
                                                    {
                                                        if (this.rightHand != null)
                                                        {
                                                            if (this.rightHand.IsHeavy())
                                                            {
                                                                return -2;
                                                            }
                                                        }

                                                        this.leftHand = item;

                                                        // If has a prop model, we set it visible to the left hand.
                                                        if (itemData.propModel != 0)
                                                        {
                                                            this.SetPropModel((int)Global.InventoryBodypart.LeftHand);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Bodyarmor:
                                                {
                                                    if (this.bodyarmor == null)
                                                    {
                                                        this.bodyarmor = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Bodyarmor, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }

                                                        if (this.bodyarmor.quantity > 0)
                                                        {
                                                            NAPI.Player.SetPlayerArmor(player.user, this.bodyarmor.quantity);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Backpack:
                                                {
                                                    if (this.backpack == null)
                                                    {
                                                        this.backpack = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Backpack, false);
                                                        }

                                                        // Parachute
                                                        if(item.id == 58)
                                                        {
                                                            NAPI.Player.GivePlayerWeapon(player.user, WeaponHash.Parachute, 1);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Gloves:
                                                {
                                                    if (this.gloves == null)
                                                    {
                                                        this.gloves = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Gloves, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Hat:
                                                {
                                                    if (this.hat == null)
                                                    {
                                                        this.hat = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            if (item.miscData[0] == "closed")
                                                            {
                                                                this.SetVariation((int)Global.InventoryBodypart.Hat, true);
                                                                if (item.IsHelmetAndHidesIdentity(player.character.sex))
                                                                {
                                                                    player.SetHiddenIdentity(true);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                this.SetVariation((int)Global.InventoryBodypart.Hat, false);
                                                            }
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Glasses:
                                                {
                                                    if (this.glasses == null)
                                                    {
                                                        this.glasses = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Glasses, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Mask:
                                                {
                                                    if (this.mask == null)
                                                    {
                                                        this.mask = item;
                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Mask, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }

                                                        player.SetHiddenIdentity(true);
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Accessory:
                                                {
                                                    if (this.accessory == null)
                                                    {
                                                        this.accessory = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Accessory, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Ears:
                                                {
                                                    if (this.ears == null)
                                                    {
                                                        this.ears = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Ears, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Torso:
                                                {
                                                    if (this.torso == null)
                                                    {
                                                        this.torso = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Torso, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Legs:
                                                {
                                                    if (this.legs == null)
                                                    {
                                                        this.legs = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Legs, false);
                                                        }

                                                        // Finger prints
                                                        if (this.legs == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Feet:
                                                {
                                                    if (this.feet == null)
                                                    {
                                                        this.feet = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Feet, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Watch:
                                                {
                                                    if (this.watch == null)
                                                    {
                                                        this.watch = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Watch, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            case (int)Global.InventoryBodypart.Bracelet:
                                                {
                                                    if (this.bracelet == null)
                                                    {
                                                        this.bracelet = item;

                                                        // If has a variation 
                                                        if ((itemData.maleVariation != -1 && player.character.sex == 0) || (itemData.femaleVariation != -1 && player.character.sex == 1))
                                                        {
                                                            this.SetVariation((int)Global.InventoryBodypart.Bracelet, false);
                                                        }

                                                        // Finger prints
                                                        if (this.gloves == null)
                                                        {
                                                            item.AddFingerPrint(player.character.cleanName);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        return -2;
                                                    }
                                                    break;
                                                }
                                            default:
                                                {
                                                    return -2;
                                                }
                                        }

                                        item.slot = toSpecialSlot;
                                    }
                                    else
                                    {
                                        // If item is heavy
                                        if (item.IsHeavy())
                                        {
                                            // The two hands must be free
                                            if (this.rightHand == null && this.leftHand == null)
                                            {
                                                // Set the object to the right hand (left hand is disabled due to the object being heavy)
                                                item.slot = (int)Global.InventoryBodypart.RightHand;
                                                this.rightHand = item;
                                                // If is not a weapon but has a prop model, we set it visible to the right hand.
                                                if (ItemData.GetById(item.id).propModel != 0)
                                                {
                                                    this.SetPropModel((int)Global.InventoryBodypart.RightHand);
                                                }

                                                // Finger prints
                                                if (this.gloves == null)
                                                {
                                                    item.AddFingerPrint(player.character.cleanName);
                                                }
                                            }
                                            else
                                            {
                                                return -2;
                                            }
                                        }
                                        // If it's not heavy we just add it to the inventory content
                                        else
                                        {
                                            item.slot = -1;
                                            this.items.Add(item);

                                            // Finger prints
                                            if (this.gloves == null)
                                            {
                                                item.AddFingerPrint(player.character.cleanName);
                                            }
                                        }
                                    }
                                }

                                // Update weight
                                this.weight += ItemData.GetById(item.id).itemWeight;
                                break;
                            }
                        // If item type is not a player (FOR NOW THERE IS NO DIFFERENCE, BUT MAYBE IN FUTURE)
                        case (int)Global.InventoryType.VehicleTrunk:
                        case (int)Global.InventoryType.VehicleGlover:
                        case (int)Global.InventoryType.House:
                        case (int)Global.InventoryType.Business:
                        case (int)Global.InventoryType.GarbageContainer:
                            {
                                item.slot = -1;
                                this.items.Add(item);
                                this.weight += ItemData.GetById(item.id).itemWeight;
                                break;
                            }
                    }

                    // Save the inventory
                    this.save = true;

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the error message string for each error code
        /// </summary>
        /// <param name="err">The error code</param>
        /// <returns>The message</returns>
        public static string GetAddItemErrorMessage(int err)
        {
            string errMsg = "";
            switch (err)
            {
                case 0:
                    {
                        errMsg = "El inventario está lleno.";
                        break;
                    }
                case -1:
                    {
                        errMsg = "Item no válido.";
                        break;
                    }
                case -2:
                    {
                        errMsg = "Item demasiado pesado.";
                        break;
                    }
                case -3:
                    {
                        errMsg = "Ropa o complemento incompatible con tu sexo.";
                        break;
                    }
            }


            return errMsg;
        }

        /// <summary>
        /// Remove item from inventory
        /// </summary>
        /// <param name="item">The item instance</param>
        public void RemoveItem(Item item)
        {
            // Get the item slot
            int slot = item.slot;
            bool removed = false;
            // If owner is a player then we check the special slots
            if (this.ownerType == (int)Global.InventoryType.Player)
            {
                Player player = (Player)this.ownerInstance;
                if (item.IsWeapon())
                {
                    this.RemoveWeapon(item);
                    removed = true;
                }
                else
                {
                    // Right hand
                    if (item.Equals(this.rightHand))
                    {
                        // If has a prop model we delete it
                        if (ItemData.GetById(item.id).propModel != 0)
                        {
                            this.RemovePropModel((int)Global.InventoryBodypart.RightHand);
                        }

                        this.rightHand = null;
                        removed = true;
                    }
                    // Left hand
                    else if (item.Equals(this.leftHand))
                    {
                        // If has a prop model we delete it
                        if (ItemData.GetById(item.id).propModel != 0)
                        {
                            this.RemovePropModel((int)Global.InventoryBodypart.LeftHand);
                        }

                        this.leftHand = null;
                        removed = true;
                    }
                    // Bodyarmor
                    else if (item.Equals(this.bodyarmor))
                    {
                        NAPI.Player.SetPlayerArmor(player.user, 0);
                        this.RemoveVariation((int)Global.InventoryBodypart.Bodyarmor);
                        this.bodyarmor = null;
                        removed = true;
                    }
                    // Backpack
                    else if (item.Equals(this.backpack))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Backpack);

                        if(this.backpack.id == 58)
                        {
                            NAPI.Player.RemovePlayerWeapon(player.user, WeaponHash.Parachute);
                        }

                        this.backpack = null;
                        removed = true;

                    }
                    // Gloves
                    else if (item.Equals(this.gloves))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Gloves);
                        this.gloves = null;
                        removed = true;
                    }
                    // Hat
                    else if (item.Equals(this.hat))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Hat);
                        this.gloves = null;
                        removed = true;
                    }
                    // Glasses
                    else if (item.Equals(this.glasses))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Glasses);
                        this.glasses = null;
                        removed = true;
                    }
                    // Mask
                    else if (item.Equals(this.mask))
                    {
                        player.SetHiddenIdentity(false);
                        this.RemoveVariation((int)Global.InventoryBodypart.Mask);
                        this.mask = null;
                        removed = true;
                    }
                    // Accessory
                    else if (item.Equals(this.accessory))
                    {
                        
                        this.RemoveVariation((int)Global.InventoryBodypart.Accessory);
                        this.accessory = null;
                        removed = true;
                    }
                    // Ears
                    else if (item.Equals(this.ears))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Ears);
                        this.ears = null;
                        removed = true;
                    }
                    // Torso
                    else if (item.Equals(this.torso))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Torso);
                        this.torso = null;
                        removed = true;
                    }
                    // Legs
                    else if (item.Equals(this.legs))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Legs);
                        this.legs = null;
                        removed = true;
                    }
                    // Feet
                    else if (item.Equals(this.feet))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Feet);
                        this.feet = null;
                        removed = true;
                    }
                    // Watch
                    else if (item.Equals(this.watch))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Watch);
                        this.watch = null;
                        removed = true;
                    }
                    // Bracelet
                    else if (item.Equals(this.bracelet))
                    {
                        this.RemoveVariation((int)Global.InventoryBodypart.Bracelet);
                        this.bracelet = null;
                        removed = true;
                    }
                    else
                    {
                        this.items.Remove(item);
                        removed = true;
                    }
                }
            }
            // Other types of inventories
            else
            {
                this.items.Remove(item);
                removed = true;
            }

            if(removed)
            {
                // Update weight
                this.weight -= ItemData.GetById(item.id).itemWeight;

                // Save the inventory
                this.save = true;
            }
        }


        /// <summary>
        /// Checks if inventory has an item of a certain id (does not include special slots)
        /// </summary>
        /// <param name="itemId">Item id</param>
        /// <returns>The item instance or null</returns>
        public Item HasItem(int itemId)
        {
            Item itemFound = null;
            foreach (Item item in items)
            {
                if (ItemData.GetById(item.id).sqlid == itemId)
                {
                    itemFound = item;
                }
                else
                {
                    if (item.contentList.Count() > 0)
                    {
                        foreach (Item itemInside in item.contentList)
                        {
                            if (ItemData.GetById(item.id).sqlid == itemId)
                            {
                                itemFound = item;
                            }
                        }
                    }
                }

                if (itemFound != null)
                {
                    break;
                }
            }

            return itemFound;
        }

        /// <summary>
        /// Checks if inventory has items of a certain id (does not include special slots)
        /// </summary>
        /// <param name="itemId">The item id</param>
        /// <returns>A list with all the instances or an empty one</returns>
        public List<Item> HasItems(int itemId)
        {
            List<Item> itemsFound = new List<Item>();
            
            foreach (Item item in items)
            {
                if (ItemData.GetById(item.id).sqlid == itemId)
                {
                    itemsFound.Add(item);
                }
                else
                {
                    foreach (Item itemInside in item.contentList)
                    {
                        if (ItemData.GetById(item.id).sqlid == itemId)
                        {
                            itemsFound.Add(item);
                        }
                    }
                }
            }

            return itemsFound;
        }

        /// <summary>
        /// Checks if inventory has items of a certain type (does not include special slots)
        /// </summary>
        /// <param name="type">The item type</param>
        /// <returns>A list with the item instances or a empty one</returns>
        public List<Item> HasItemsFromType(int type)
        {
            List<Item> itemsFound = new List<Item>();

            foreach (Item item in items)
            {
                if (ItemData.GetById(item.id).type == type)
                {
                    itemsFound.Add(item);
                }
                else
                {
                    foreach (Item itemInside in item.contentList)
                    {
                        if (ItemData.GetById(item.id).type == type)
                        {
                            itemsFound.Add(item);
                        }
                    }
                }
            }

            return itemsFound;
        }

        /// <summary>
        /// Saves the current inventory content to the database
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        public bool Save()
        {
            try
            {
                RootObject inventoryJSON = new RootObject();

                inventoryJSON.items = items.ToArray();

                Util.UpdateDBField("Inventories", "content", NAPI.Util.ToJson(inventoryJSON), "id", sqlid.ToString());

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
        /// Deletes all the inventory content, also in the database
        /// </summary>
        /// <returns>True if success, false otherwise</returns>
        public bool Wipe()
        {
            try
            {
                // Delete all objects from database
                Util.UpdateDBField("Inventories", "content", string.Empty, "id", sqlid.ToString());
                 
                // Then clear all inventory items and special slots for players.
                this.items.Clear();

                if(this.ownerType == (int)Global.InventoryType.Player)
                {
                    // Special slots

                    this.RemoveItem(this.rightHand);
                    this.RemoveItem(this.leftHand);
                    this.RemoveItem(this.bodyarmor);
                    this.RemoveItem(this.backpack);
                    this.RemoveItem(this.gloves);
                    this.RemoveItem(this.hat);
                    this.RemoveItem(this.glasses);
                    this.RemoveItem(this.mask);
                    this.RemoveItem(this.accessory);
                    this.RemoveItem(this.ears);
                    this.RemoveItem(this.torso);
                    this.RemoveItem(this.legs);
                    this.RemoveItem(this.feet);
                    this.RemoveItem(this.watch);
                    this.RemoveItem(this.bracelet);

                    // Weapons

                    this.RemoveWeapon(this.lightWeapon1);
                    this.RemoveWeapon(this.lightWeapon2);
                    this.RemoveWeapon(this.meleeLightWeapon1);
                    this.RemoveWeapon(this.meleeLightWeapon2);
                    this.RemoveWeapon(this.heavyWeapon);
                    this.RemoveWeapon(this.meleeHeavyWeapon);
                    this.RemoveWeapon(this.throwableWeapon1);
                    this.RemoveWeapon(this.throwableWeapon2);
                    this.RemoveWeapon(this.specialWeapon);
                }
                
                this.weight = 0;

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
        /// Gives a weapon to a player
        /// </summary>
        /// <param name="weapon">The weapon item instance</param>
        /// <param name="equip">If must be equiped on hands</param>
        /// <returns></returns>
        public bool GiveWeapon(Item weapon, bool equip)
        {
            // Cast the instance to a player one
            Player player = (Player)ownerInstance;

            // Verify that the item is a weapon
            if (weapon.IsWeapon())
            {
                ItemData itemData = ItemData.GetById(weapon.id);
                bool fitsIn = true;
                switch (itemData.type)
                {
                    case (int)Global.ItemType.MeleeWeapon:
                        {
                            if (!itemData.isHeavy)
                            {
                                if (this.meleeLightWeapon1 == null)
                                {
                                    this.meleeLightWeapon1 = weapon;
                                }
                                else
                                {
                                    if (this.meleeLightWeapon2 == null)
                                    {
                                        this.meleeLightWeapon2 = weapon;
                                    }
                                    else
                                    {
                                        fitsIn = false;
                                    }
                                }
                            }
                            else
                            {
                                if (this.meleeHeavyWeapon == null)
                                {
                                    this.meleeHeavyWeapon = weapon;
                                }
                                else
                                {
                                    fitsIn = false;
                                }
                            }
                            break;
                        }
                    case (int)Global.ItemType.FireWeapon:
                        {
                            if (!itemData.isHeavy)
                            {
                                if (this.lightWeapon1 == null)
                                {
                                    this.lightWeapon1 = weapon;
                                }
                                else
                                {
                                    if (this.lightWeapon2 == null)
                                    {
                                        this.lightWeapon2 = weapon;
                                    }
                                    else
                                    {
                                        fitsIn = false;
                                    }
                                }
                            }
                            else
                            {
                                if (this.heavyWeapon == null)
                                {
                                    this.heavyWeapon = weapon;
                                }
                                else
                                {
                                    fitsIn = false;
                                }
                            }
                            break;
                        }
                    case (int)Global.ItemType.ThrowableWeapon:
                        {
                            if (this.throwableWeapon1 == null)
                            {
                                this.throwableWeapon1 = weapon;
                            }
                            else
                            {
                                if (this.throwableWeapon2 == null)
                                {
                                    this.throwableWeapon2 = weapon;
                                }
                                else
                                {
                                    fitsIn = false;
                                }
                            }
                            break;
                        }
                    case (int)Global.ItemType.SpecialWeapon:
                        {
                            if (this.specialWeapon == null)
                            {
                                this.specialWeapon = weapon;
                            }
                            else
                            {
                                fitsIn = false;
                            }

                            break;
                        }
                }


                if (fitsIn)
                {
                    // Get the weapon hash
                    WeaponHash hash = NAPI.Util.WeaponNameToModel(itemData.weaponModel);

                    // Give the weapon to the player
                    player.disableWeaponAnticheatSeconds = 2;
                    NAPI.Player.GivePlayerWeapon(player.user, hash, weapon.quantity);

                    // If the weapon has a special tint
                    if (weapon.miscData.Count() > 0)
                    {
                        WeaponTint tint = (WeaponTint)Enum.Parse(typeof(WeaponTint), weapon.miscData[0]);

                        NAPI.Player.SetPlayerWeaponTint(player.user, hash, tint);
                    }

                    // Attach the accessories one by one
                    foreach (Item accessory in weapon.accessoryList)
                    {
                        WeaponComponent weaponComponent = (WeaponComponent)Enum.Parse(typeof(WeaponComponent), ItemData.GetById(accessory.id).weaponModel);
                        NAPI.Player.SetPlayerWeaponComponent(player.user, hash, weaponComponent);
                    }

                    return true;

                }

            }
            return false;
        }

        /// <summary>
        /// Returns the item instance for the weapon hash
        /// </summary>
        /// <param name="hash">The weapon hash</param>
        /// <returns>The weapon item instance</returns>
        public Item GetWeaponItem(WeaponHash hash)
        {
            WeaponHash otherHash;
            if (this.lightWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.lightWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.lightWeapon1;
                }
            }

            if (this.lightWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.lightWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.lightWeapon2;
                }
            }

            if (this.heavyWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.heavyWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.heavyWeapon;
                }
            }

            if (this.meleeLightWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeLightWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.meleeLightWeapon1;
                }
            }

            if (this.meleeLightWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeLightWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.meleeLightWeapon2;
                }
            }

            if (this.meleeHeavyWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeHeavyWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.meleeHeavyWeapon;
                }
            }

            if (this.throwableWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.throwableWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.throwableWeapon1;
                }
            }

            if (this.throwableWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.throwableWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.throwableWeapon2;
                }
            }

            if (this.specialWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.specialWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    return this.specialWeapon;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds an accessory to the current holding weapon if compatible
        /// </summary>
        /// <param name="accessory">The accessory instance</param>
        /// <returns>True if weapon Exists, false if no weapon</returns>
        public bool AddWeaponAccessory(Item accessory)
        {
            // Get player instance
            Player player = (Player)ownerInstance;
            Item currentWeapon = this.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            
            // Current weapon Exists
            if (currentWeapon != null)
            { 
                // Calculate weapon and component hash
                WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(currentWeapon.id).weaponModel);
                WeaponComponent weaponComponent = (WeaponComponent)Enum.Parse(typeof(WeaponComponent), ItemData.GetById(accessory.id).weaponModel);
                
                // Apply the component to the weapon
                NAPI.Player.SetPlayerWeaponComponent(player.user, weaponHash, weaponComponent);
                
                // Add the accessory instance to the accessory list
                currentWeapon.accessoryList.Add(accessory);

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes an accessory from the current weapon
        /// </summary>
        /// <param name="accessory">The accessory instance (Item)</param>
        /// <returns></returns>
        public int RemoveWeaponAccessory(Item accessory)
        {
            // Get player instance
            Player player = (Player)ownerInstance;
            Item currentWeapon = this.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            // Current weapon Exists
            if (currentWeapon != null)
            {
                // Check if item is an accessory
                if (accessory.IsWeaponAccessory())
                {
                    // Check if accessory is compatible with the current weapon
                    if (currentWeapon.IsWeaponAccessoryCompatible(accessory))
                    {
                        // Check if weapon has already an identical accessory
                        if (currentWeapon.HasWeaponSameAccessory(accessory))
                        {
                            // Calculate weapon and component hash
                            WeaponHash weaponHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(currentWeapon.id).weaponModel);
                            WeaponComponent weaponComponent = (WeaponComponent)Enum.Parse(typeof(WeaponComponent), ItemData.GetById(accessory.id).weaponModel);
                            
                            // Remove the component to the weapon
                            NAPI.Player.RemovePlayerWeaponComponent(player.user, weaponHash, weaponComponent);
                            
                            // Remove the accessory instance from the accessory list
                            currentWeapon.accessoryList.Remove(accessory);
                            return 1;
                        }
                        else
                        {
                            return -3;
                        }
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Sets the current weapon tint
        /// </summary>
        /// <param name="tint">The tint type</param>
        /// <returns></returns>
        public int SetWeaponTint(WeaponTint tint)
        {
            // Get player instance
            Player player = (Player)ownerInstance;
            WeaponHash hash = NAPI.Player.GetPlayerCurrentWeapon(player.user);
            Item currentWeapon = this.GetWeaponItem(hash);

            // Current weapon Exists
            if (currentWeapon != null)
            {
                // Set the weapon's tint
                NAPI.Player.SetPlayerWeaponTint(player.user, hash, tint);

                // Save the weapon tint
                if (currentWeapon.miscData.Count() > 0)
                {
                    currentWeapon.miscData[0] = tint.ToString();
                }
                else
                {
                    currentWeapon.miscData.Add(tint.ToString());
                }

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Sets the weapon ammo
        /// </summary>
        /// <param name="ammo">The ammo quantity</param>
        public void SetWeaponAmmo(int ammo)
        {
            // Get player instance
            Player player = (Player)ownerInstance;
            Item currentWeapon = this.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            // Current weapon Exists
            if (currentWeapon != null)
            {
                ItemData data = ItemData.GetById(currentWeapon.id);
                currentWeapon.quantity = ammo;
                NAPI.Player.SetPlayerWeaponAmmo(player.user, NAPI.Util.WeaponNameToModel(data.weaponModel), ammo);
            }

        }

        /// <summary>
        /// Removes a weapon from the inventory
        /// </summary>
        /// <param name="weapon">The weapon item</param>
        /// <returns></returns>
        public bool RemoveWeapon(Item weapon)
        {
            // Get player instance
            Player player = (Player)ownerInstance;
            WeaponHash hash = NAPI.Util.WeaponNameToModel(ItemData.GetById(weapon.id).weaponModel);
            bool remove = false;
            WeaponHash otherHash;
            if (this.lightWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.lightWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    this.lightWeapon1 = null;
                    remove = true;
                }
            }

            if (this.lightWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.lightWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    this.lightWeapon2 = null;
                    remove = true;
                }
            }

            if (this.heavyWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.heavyWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    this.heavyWeapon = null;
                    this.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);
                    remove = true;
                }
            }

            if (this.meleeLightWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeLightWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    this.meleeLightWeapon1 = null;
                    remove = true;
                }
            }

            if (this.meleeLightWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeLightWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    this.meleeLightWeapon2 = null;
                    remove = true;
                }
            }

            if (this.meleeHeavyWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.meleeHeavyWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    this.meleeHeavyWeapon = null;
                    this.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);
                    remove = true;
                }
            }

            if (this.throwableWeapon1 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.throwableWeapon1.id).weaponModel);
                if (hash == otherHash)
                {
                    this.throwableWeapon1 = null;
                    remove = true;
                }
            }

            if (this.throwableWeapon2 != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.throwableWeapon2.id).weaponModel);
                if (hash == otherHash)
                {
                    this.throwableWeapon2 = null;
                    remove = true;
                }
            }

            if (this.specialWeapon != null)
            {
                otherHash = NAPI.Util.WeaponNameToModel(ItemData.GetById(this.specialWeapon.id).weaponModel);
                if (hash == otherHash)
                {
                    this.specialWeapon = null;
                    remove = true;
                }
            }

            if (remove)
            {
                player.disableWeaponAnticheatSeconds = 2;
                player.justRemovedWeapon = true;
                NAPI.Player.RemovePlayerWeapon(player.user, hash);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates and attaches the prop model on the player's body
        /// </summary>
        /// <param name="bodyPart">The player bodypart</param>
        public void SetPropModel(int bodyPart)
        {
            string bone = Global.PlayerBodypartBones[bodyPart];
            Player player = (Player)ownerInstance;
            switch (bodyPart)
            {
                case (int)Global.InventoryBodypart.RightHand:
                    {
                        ItemData itemData = ItemData.GetById(this.rightHand.id);
                        Vector3 offsetPos = itemData.rightHandOffset;
                        Vector3 offsetRot = itemData.rightHandRotation;
                        this.rightHandProp = NAPI.Object.CreateObject(itemData.propModel, player.user.Position, new Vector3(0, 0, 0), 255, player.user.Dimension);
                        NAPI.Entity.AttachEntityToEntity(this.rightHandProp, player.user, bone, offsetPos, offsetRot);
                        break;
                    }
                case (int)Global.InventoryBodypart.LeftHand:
                    {
                        ItemData itemData = ItemData.GetById(this.leftHand.id);
                        Vector3 offsetPos = itemData.leftHandOffset;
                        Vector3 offsetRot = itemData.leftHandRotation;
                        this.leftHandProp = NAPI.Object.CreateObject(itemData.propModel, player.user.Position, new Vector3(0, 0, 0), 255, player.user.Dimension);
                        NAPI.Entity.AttachEntityToEntity(this.leftHandProp, player.user, bone, offsetPos, offsetRot);
                        break;
                    }
                case (int)Global.InventoryBodypart.HeavyWeapon:
                    {
                        ItemData itemData = ItemData.GetById(this.heavyWeapon.id);
                        Vector3 offsetPos = itemData.chestOffset;
                        Vector3 offsetRot = itemData.chestRotation;
                        this.heavyWeaponProp = NAPI.Object.CreateObject(itemData.propModel, player.user.Position, new Vector3(0, 0, 0), 255, player.user.Dimension);
                        NAPI.Entity.AttachEntityToEntity(this.heavyWeaponProp, player.user, bone, offsetPos, offsetRot);
                        break;
                    }
                case (int)Global.InventoryBodypart.MeleeWeapon:
                    {
                        ItemData itemData = ItemData.GetById(this.meleeHeavyWeapon.id);
                        Vector3 offsetPos = itemData.backOffset;
                        Vector3 offsetRot = itemData.backRotation;
                        this.meleeHeavyWeaponProp = NAPI.Object.CreateObject(itemData.propModel, player.user.Position, new Vector3(0, 0, 0), 255, player.user.Dimension);
                        NAPI.Entity.AttachEntityToEntity(this.meleeHeavyWeaponProp, player.user, bone, offsetPos, offsetRot);
                        break;
                    }
            }
        }

        /// <summary>
        /// Removes the prop model from the player's body
        /// </summary>
        /// <param name="bodyPart">The player bodypart</param>
        public void RemovePropModel(int bodyPart)
        {
            switch (bodyPart)
            {
                case (int)Global.InventoryBodypart.RightHand:
                    {
                        NAPI.Entity.DetachEntity(this.rightHandProp, false);
                        NAPI.Entity.DeleteEntity(this.rightHandProp);
                        break;
                    }
                case (int)Global.InventoryBodypart.LeftHand:
                    {
                        NAPI.Entity.DetachEntity(this.leftHandProp, false);
                        NAPI.Entity.DeleteEntity(this.leftHandProp);
                        break;
                    }
                case (int)Global.InventoryBodypart.HeavyWeapon:
                    {
                        NAPI.Entity.DetachEntity(this.heavyWeaponProp, false);
                        NAPI.Entity.DeleteEntity(this.heavyWeaponProp);
                        break;
                    }
                case (int)Global.InventoryBodypart.MeleeWeapon:
                    {
                        NAPI.Entity.DetachEntity(this.meleeHeavyWeaponProp, false);
                        NAPI.Entity.DeleteEntity(this.meleeHeavyWeaponProp);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the player clothing variation visible
        /// </summary>
        /// <param name="bodyPart">The player bodypart</param>
        /// <param name="alternative">Use alternative variation or not</param>
        public void SetVariation(int bodyPart, bool alternative)
        {
            Player player = (Player)this.ownerInstance;
            switch (bodyPart)
            {
                case (int)Global.InventoryBodypart.Bodyarmor:
                    {
                        ItemData itemData = ItemData.GetById(this.bodyarmor.id);
                        if(player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 9, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 9, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        
                        break;
                    }
                case (int)Global.InventoryBodypart.Backpack:
                    {
                        ItemData itemData = ItemData.GetById(this.backpack.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 5, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 5, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Gloves:
                    {
                        ItemData itemData = ItemData.GetById(this.gloves.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 3, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 3, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Hat:
                    {
                        ItemData itemData = ItemData.GetById(this.hat.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 0, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 0, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Glasses:
                    {
                        ItemData itemData = ItemData.GetById(this.glasses.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 1, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 1, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Mask:
                    {
                        ItemData itemData = ItemData.GetById(this.mask.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 1, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 1, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Accessory:
                    {
                        ItemData itemData = ItemData.GetById(this.accessory.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 7, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 7, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Ears:
                    {
                        ItemData itemData = ItemData.GetById(this.ears.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 2, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 2, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Torso:
                    {
                        ItemData itemData = ItemData.GetById(this.torso.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 11, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 11, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Legs:
                    {
                        ItemData itemData = ItemData.GetById(this.legs.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 4, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 4, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Feet:
                    {
                        ItemData itemData = ItemData.GetById(this.feet.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 6, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 6, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Watch:
                    {
                        ItemData itemData = ItemData.GetById(this.watch.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 6, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 6, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Bracelet:
                    {
                        ItemData itemData = ItemData.GetById(this.bracelet.id);
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 7, itemData.maleVariation, itemData.maleVariationTexture);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerAccessory(player.user, 7, itemData.femaleVariation, itemData.femaleVariationTexture);
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Removes the player clothing variation
        /// </summary>
        /// <param name="bodyPart">The player bodypart</param>
        public void RemoveVariation(int bodyPart)
        {
            Player player = (Player)this.ownerInstance;
            switch (bodyPart)
            {
                case (int)Global.InventoryBodypart.Bodyarmor:
                    {
                        NAPI.Player.SetPlayerClothes(player.user, 9, 0, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Backpack:
                    {
                        NAPI.Player.SetPlayerClothes(player.user, 5, 0, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Gloves:
                    {
                        NAPI.Player.SetPlayerClothes(player.user, 3, 0, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Hat:
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Glasses:
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 1);
                        break;
                    }
                case (int)Global.InventoryBodypart.Mask:
                    {
                        NAPI.Player.SetPlayerClothes(player.user, 1, 0, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Accessory:
                    {
                        NAPI.Player.SetPlayerClothes(player.user, 7, 0, 0);
                        break;
                    }
                case (int)Global.InventoryBodypart.Ears:
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 2);
                        break;
                    }
                case (int)Global.InventoryBodypart.Torso:
                    {
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 11, 15, 0);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 11, 5, 0);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Legs:
                    {
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 4, 14, 0);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 4, 17, 0);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Feet:
                    {
                        if (player.character.sex == 0)
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 6, 34, 0);
                        }
                        else
                        {
                            NAPI.Player.SetPlayerClothes(player.user, 6, 35, 0);
                        }
                        break;
                    }
                case (int)Global.InventoryBodypart.Watch:
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 6);
                        break;
                    }
                case (int)Global.InventoryBodypart.Bracelet:
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 7);
                        break;
                    }
            }
        }

        /// <summary>
        /// Opens the self inventory of a player and the other specified one.
        /// If otherInventory is null, then we show the nearests ground items
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="otherInventory">The other inventory instance</param>
        /// <param name="otherInventoryTitle">The other inventory title</param>
        /// <param name="otherInventoryLocked">If the other inventory must be locked</param>
        public static void OpenInventoryForPlayer(Player player, Inventory otherInventory = null, string otherInventoryTitle = "Cerca", bool otherInventoryLocked = false)
        {
            if (player.CanUseInventory())
            {
                if (otherInventory == null)
                {
                    player.character.nearWorldItems = WorldItem.GetNearItems(player.user.Position, Global.WorldItemsMaxDistanceDetection, player.user.Dimension);
                    string worldItemsJSON = WorldItem.ToJSON(player.character.nearWorldItems);
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "showInventoryInterface", otherInventoryTitle, player.character.nearWorldItems.Count(), "-", worldItemsJSON, otherInventoryLocked,
                                                                                    "Inventario", player.character.inventory.weight, player.character.inventory.GetMaxWeight(), player.character.inventory.GetJSON(player));
                }
                else
                {
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "showInventoryInterface", otherInventoryTitle, otherInventory.weight, otherInventory.GetMaxWeight(), otherInventory.GetJSON(), otherInventoryLocked,
                                                                                    "Inventario", player.character.inventory.weight, player.character.inventory.GetMaxWeight(), player.character.inventory.GetJSON(player));
                    player.character.otherInventory = otherInventory;
                }

                player.showingGui = (int)GuiController.GuiTypes.InventoryInterface;
                player.showingGuiId = 0;
            }
        }

        /// <summary>
        /// Returns the inventory contents in JSON to be send to the clientside interface.
        /// </summary>
        /// <param name="player">The player interface</param>
        /// <returns></returns>
        public string GetJSON(Player player = null)
        {
            List<string> content = new List<string>();

            // If is a player first we process all special slots
            if (this.ownerType == (int)Global.InventoryType.Player && player != null)
            {
                // RIGHT HAND OR WEAPON
                if (this.rightHand != null)
                {
                    ItemData itemData = ItemData.GetById(this.rightHand.id);
                    content.Add(this.rightHand.GetOverviewText() + ";" + itemData.image + ";" + this.rightHand.quantity);
                }
                else
                {
                    Item weapon = this.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                    if (weapon != null)
                    {
                        ItemData itemData = ItemData.GetById(weapon.id);
                        content.Add(weapon.GetOverviewText() + ";" + itemData.image + ";" + weapon.quantity);
                    }
                    else
                    {
                        content.Add("null");
                    }
                }

                // LEFT HAND
                if (this.leftHand != null)
                {
                    ItemData itemData = ItemData.GetById(this.leftHand.id);
                    content.Add(this.leftHand.GetOverviewText() + ";" + itemData.image + ";" + this.leftHand.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // BODY ARMOR
                if (this.bodyarmor != null)
                {
                    ItemData itemData = ItemData.GetById(this.bodyarmor.id);
                    content.Add(this.bodyarmor.GetOverviewText() + ";" + itemData.image + ";" + this.bodyarmor.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // BACKPACK
                if (this.backpack != null)
                {
                    ItemData itemData = ItemData.GetById(this.backpack.id);
                    content.Add(this.backpack.GetOverviewText() + ";" + itemData.image + ";" + this.backpack.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // GLOVES
                if (this.gloves != null)
                {
                    ItemData itemData = ItemData.GetById(this.gloves.id);
                    content.Add(this.gloves.GetOverviewText() + ";" + itemData.image + ";" + this.gloves.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // HAT
                if (this.hat != null)
                {
                    ItemData itemData = ItemData.GetById(this.hat.id);
                    content.Add(this.hat.GetOverviewText() + ";" + itemData.image + ";" + this.hat.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // GLASSES
                if (this.glasses != null)
                {
                    ItemData itemData = ItemData.GetById(this.glasses.id);
                    content.Add(this.glasses.GetOverviewText() + ";" + itemData.image + ";" + this.glasses.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // MASK
                if (this.mask != null)
                {
                    ItemData itemData = ItemData.GetById(this.mask.id);
                    content.Add(this.mask.GetOverviewText() + ";" + itemData.image + ";" + this.mask.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // ACCESSORY
                if (this.accessory != null)
                {
                    ItemData itemData = ItemData.GetById(this.accessory.id);
                    content.Add(this.accessory.GetOverviewText() + ";" + itemData.image + ";" + this.accessory.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // EARS
                if (this.ears != null)
                {
                    ItemData itemData = ItemData.GetById(this.ears.id);
                    content.Add(this.ears.GetOverviewText() + ";" + itemData.image + ";" + this.ears.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // TORSO
                if (this.torso != null)
                {
                    ItemData itemData = ItemData.GetById(this.torso.id);
                    content.Add(this.torso.GetOverviewText() + ";" + itemData.image + ";" + this.torso.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // LEGS
                if (this.legs != null)
                {
                    ItemData itemData = ItemData.GetById(this.legs.id);
                    content.Add(this.legs.GetOverviewText() + ";" + itemData.image + ";" + this.legs.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // FEET
                if (this.feet != null)
                {
                    ItemData itemData = ItemData.GetById(this.feet.id);
                    content.Add(this.feet.GetOverviewText() + ";" + itemData.image + ";" + this.feet.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // WATCH
                if (this.watch != null)
                {
                    ItemData itemData = ItemData.GetById(this.watch.id);
                    content.Add(this.watch.GetOverviewText() + ";" + itemData.image + ";" + this.watch.quantity);
                }
                else
                {
                    content.Add("null");
                }

                // BRACELET
                if (this.bracelet != null)
                {
                    ItemData itemData = ItemData.GetById(this.bracelet.id);
                    content.Add(this.bracelet.GetOverviewText() + ";" + itemData.image + ";" + this.bracelet.quantity);
                }
                else
                {
                    content.Add("null");
                }
            }

            // Then we process all inventory items
            foreach (Item item in items)
            {
                ItemData itemData = ItemData.GetById(item.id);
                content.Add(item.GetOverviewText() + ";" + itemData.image + ";" + item.quantity);
            }

            return NAPI.Util.ToJson(content.ToArray());
        }

        /// <summary>
        /// Closes the inventory for the player
        /// </summary>
        /// <param name="player">The player instances</param>
        public static void CloseInventoryForPlayer(Player player)
        {
            player.character.nearWorldItems = null;
            player.character.otherInventory = null;
            player.character.leftOpenedContainer = null;
            player.character.leftOpenedContainerWorld = null;
            player.character.rightOpenedContainer = null;
            player.showingGui = -1;
            player.showingGuiId = 0;
            NAPI.ClientEvent.TriggerClientEvent(player.user, "hideInventoryInterface");
        }

        /// <summary>
        /// Handles the item click event for a player
        /// </summary>
        /// <param name="player">The player that triggers the event</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        public static void HandleItemClickEvent(Player player, int isRight, int itemId)
        {
            Inventory playerInventory = player.character.inventory;
            if (playerInventory.rightHand == null && playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user)) == null)
            {
                bool freeHands = true;
                if (playerInventory.leftHand != null)
                {
                    if (playerInventory.leftHand.IsHeavy())
                    {
                        freeHands = false;
                    }
                }

                if (freeHands)
                {
                    // Is right grid
                    if (isRight == 1)
                    {
                        Item itemSelected = null;

                        // If there is an opened container
                        if (player.character.rightOpenedContainer != null)
                        {
                            try
                            {
                                itemSelected = player.character.rightOpenedContainer.contentList.ElementAt(itemId);
                            }
                            catch (Exception e)
                            {
                                Log.Debug(e.Message);
                                Log.Debug(e.StackTrace);
                                itemSelected = null;
                            }

                            if (itemSelected != null)
                            {
                                ItemData itemData = ItemData.GetById(itemSelected.id);

                                // Remove the item from the inventory and save
                                playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand);
                                player.character.rightOpenedContainer.contentList.Remove(itemSelected);

                                NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", true, itemId, itemData.itemWeight);
                            }
                        }
                        else
                        {
                            // If ID is a special slot
                            if (itemId <= Global.PlayerSpecialSlotsCount - 1)
                            {
                                switch (itemId)
                                {
                                    case (int)Global.InventoryBodypart.RightHand:
                                        {
                                            itemSelected = null;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.LeftHand:
                                        {
                                            itemSelected = playerInventory.leftHand;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Bodyarmor:
                                        {
                                            itemSelected = playerInventory.bodyarmor;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Backpack:
                                        {
                                            itemSelected = playerInventory.backpack;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Gloves:
                                        {
                                            itemSelected = playerInventory.gloves;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Hat:
                                        {
                                            itemSelected = playerInventory.hat;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Glasses:
                                        {
                                            itemSelected = playerInventory.glasses;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Mask:
                                        {
                                            itemSelected = playerInventory.mask;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Accessory:
                                        {
                                            itemSelected = playerInventory.accessory;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Ears:
                                        {
                                            itemSelected = playerInventory.ears;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Torso:
                                        {
                                            itemSelected = playerInventory.torso;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Legs:
                                        {
                                            itemSelected = playerInventory.legs;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Feet:
                                        {
                                            itemSelected = playerInventory.feet;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Watch:
                                        {
                                            itemSelected = playerInventory.watch;
                                            break;
                                        }
                                    case (int)Global.InventoryBodypart.Bracelet:
                                        {
                                            itemSelected = playerInventory.bracelet;
                                            break;
                                        }
                                }

                                if (itemSelected != null)
                                {
                                    ItemData itemData = ItemData.GetById(itemSelected.id);

                                    // We make the change effect on the interface
                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.RightHand, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "removeSpecialItemInventory", itemId, itemData.itemWeight);

                                    // Remove the item from the inventory and save
                                    playerInventory.RemoveItem(itemSelected);
                                    playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand);
                                }
                            }
                            else
                            {
                                try
                                {
                                    itemSelected = playerInventory.items.ElementAt(itemId - Global.PlayerSpecialSlotsCount);
                                }
                                catch (Exception e)
                                {
                                    Log.Debug(e.Message);
                                    Log.Debug(e.StackTrace);
                                    itemSelected = null;
                                }

                                if (itemSelected != null)
                                {
                                    ItemData itemData = ItemData.GetById(itemSelected.id);

                                    // We make the change effect on the interface
                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.RightHand, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", true, itemId, itemData.itemWeight);

                                    // Remove the item from the inventory and save
                                    playerInventory.RemoveItem(itemSelected);
                                    playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand);
                                }
                            }
                        }
                    }
                    else
                    {
                        Item itemSelected = null;
                        ItemData itemData;
                        int weight = 0;
                        if (player.character.leftOpenedContainer != null)
                        {
                            try
                            {
                                itemSelected = player.character.leftOpenedContainer.contentList.ElementAt(itemId);
                            }
                            catch (Exception e)
                            {
                                Log.Debug(e.Message);
                                Log.Debug(e.StackTrace);
                                itemSelected = null;
                            }

                            if (itemSelected != null)
                            {
                                itemData = ItemData.GetById(itemSelected.id);

                                // Remove the item from the container and save
                                playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand);
                                player.character.leftOpenedContainer.contentList.Remove(itemSelected);

                                if (player.character.leftOpenedContainerWorld != null)
                                {
                                    player.character.leftOpenedContainerWorld.contentList.Remove(itemSelected);
                                    player.character.leftOpenedContainer = player.character.leftOpenedContainerWorld.ToItem();
                                }

                                NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", false, itemId, itemData.quantity);

                                if (player.character.rightOpenedContainer == null)
                                {
                                    if (!(itemSelected.IsWeapon() && itemSelected.quantity == 0))
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.RightHand, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                }
                            }
                        }
                        else
                        {
                            int maxWeight = playerInventory.GetMaxWeight();
                            int currentWeight = playerInventory.weight;
                            bool avoidAdding = true;
                            WorldItem worldItem = null;
                            if (player.character.otherInventory == null)
                            {
                                // If its a world item we destroy it
                                try
                                {
                                    worldItem = player.character.nearWorldItems.ElementAt(itemId);
                                }
                                catch (Exception e)
                                {
                                    Log.Debug(e.Message);
                                    Log.Debug(e.StackTrace);
                                    worldItem = null;
                                }

                                if (worldItem != null)
                                {
                                    itemSelected = worldItem.ToItem();
                                    itemData = ItemData.GetById(itemSelected.id);
                                    if (currentWeight + itemData.itemWeight <= maxWeight)
                                    {
                                        avoidAdding = false;
                                    }
                                }
                            }
                            else
                            {
                                // Else we remove it from the other inventory
                                try
                                {
                                    itemSelected = player.character.otherInventory.items.ElementAt(itemId);
                                }
                                catch (Exception e)
                                {
                                    Log.Debug(e.Message);
                                    Log.Debug(e.StackTrace);
                                    itemSelected = null;
                                }

                                if (itemSelected != null)
                                {
                                    itemData = ItemData.GetById(itemSelected.id);

                                    if (currentWeight + itemData.itemWeight <= maxWeight)
                                    {
                                        avoidAdding = false;
                                    }
                                }
                            }

                            if (!avoidAdding)
                            {
                                itemData = ItemData.GetById(itemSelected.id);
                                // Add the item to player's inventory
                                if (playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand) == 1)
                                {
                                    // Remove the world or other inventory item just after adding it to the inventory
                                    if(worldItem != null)
                                    {
                                        player.character.nearWorldItems.Remove(worldItem);
                                        worldItem.destroy();
                                    }
                                    else
                                    { 
                                        if(player.character.otherInventory != null)
                                        {
                                            player.character.otherInventory.RemoveItem(itemSelected);
                                        }
                                    }

                                    // Make the change effect on the interface
                                    if (player.character.rightOpenedContainer == null)
                                    {
                                        if (!(itemSelected.IsWeapon() && itemSelected.quantity == 0))
                                            NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.RightHand, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                    }

                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", false, itemId, weight);
                                }
                                else
                                {
                                    player.DisplayNotification("~g~No te cabe en el inventario.");
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (isRight == 1 && itemId == (int)Global.InventoryBodypart.RightHand && player.character.rightOpenedContainer == null && playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user)) == null)
                {
                    Item item = playerInventory.rightHand;
                    ItemData itemData = ItemData.GetById(item.id);
                    bool remove = false;
                    switch (itemData.type)
                    {
                        case (int)Global.ItemType.Bodyarmor:
                            {
                                if (playerInventory.bodyarmor == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Bodyarmor) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Bodyarmor, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Backpack:
                            {
                                if (playerInventory.backpack == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Backpack) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Backpack, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Gloves:
                            {
                                if (playerInventory.gloves == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Gloves) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Gloves, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Hat:
                            {
                                if (playerInventory.hat == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Hat) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Hat, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Glasses:
                            {
                                if (playerInventory.glasses == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Glasses) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Glasses, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Mask:
                            {
                                if (playerInventory.mask == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Mask) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Mask, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Accessory:
                            {
                                if (playerInventory.accessory == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Accessory) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Accessory, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.EarAccessory:
                            {
                                if (playerInventory.ears == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Ears) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Ears, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.TorsoClothes:
                            {
                                if (playerInventory.torso == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Torso) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Torso, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.LegClothes:
                            {
                                if (playerInventory.legs == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Legs) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Legs, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Shoes:
                            {
                                if (playerInventory.feet == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Feet) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Feet, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Watch:
                            {
                                if (playerInventory.watch == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Watch) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Watch, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        case (int)Global.ItemType.Bracelet:
                            {
                                if (playerInventory.bracelet == null)
                                {
                                    if (playerInventory.AddItem(item, (int)Global.InventoryBodypart.Bracelet) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", (int)Global.InventoryBodypart.Bracelet, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }
                                break;
                            }
                        default:
                            {
                                if (!item.IsWeapon())
                                {
                                    if (playerInventory.AddItem(item) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", true, item.GetOverviewText(), itemData.image, item.quantity, itemData.itemWeight);
                                        remove = true;
                                    }
                                }

                                break;
                            }
                    }

                    if (remove)
                    {
                        playerInventory.RemoveItem(item);
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "removeSpecialItemInventory", itemId, itemData.itemWeight);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the item double click event for a player
        /// </summary>
        /// <param name="player">The player that triggers the event</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        public static void HandleItemDoubleClickEvent(Player player, int isRight, int itemId)
        {
            Inventory playerInventory = player.character.inventory;
            // Is right grid
            if (isRight == 1)
            {
                Item itemSelected = null;
                // If ID is a special slot
                if (itemId <= Global.PlayerSpecialSlotsCount - 1)
                {
                    switch (itemId)
                    {
                        case (int)Global.InventoryBodypart.RightHand:
                            {
                                itemSelected = playerInventory.rightHand;
                                break;
                            }
                        case (int)Global.InventoryBodypart.LeftHand:
                            {
                                itemSelected = playerInventory.leftHand;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Bodyarmor:
                            {
                                itemSelected = playerInventory.bodyarmor;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Backpack:
                            {
                                itemSelected = playerInventory.backpack;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Gloves:
                            {
                                itemSelected = playerInventory.gloves;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Hat:
                            {
                                itemSelected = playerInventory.hat;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Glasses:
                            {
                                itemSelected = playerInventory.glasses;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Mask:
                            {
                                itemSelected = playerInventory.mask;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Accessory:
                            {
                                itemSelected = playerInventory.accessory;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Ears:
                            {
                                itemSelected = playerInventory.ears;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Torso:
                            {
                                itemSelected = playerInventory.torso;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Legs:
                            {
                                itemSelected = playerInventory.legs;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Feet:
                            {
                                itemSelected = playerInventory.feet;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Watch:
                            {
                                itemSelected = playerInventory.watch;
                                break;
                            }
                        case (int)Global.InventoryBodypart.Bracelet:
                            {
                                itemSelected = playerInventory.bracelet;
                                break;
                            }
                    }
                }
                else
                {
                    // If item is not inside a special slot..
                    try
                    {
                        itemSelected = playerInventory.items.ElementAt(itemId - Global.PlayerSpecialSlotsCount);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        itemSelected = null;
                    }
                }

                // If item is not null
                if (itemSelected != null)
                {
                    // If item is an item container, then we open it
                    if (itemSelected.IsItemContainer())
                    {
                        if (player.character.rightOpenedContainer == null)
                        {
                            OpenItemContainerForPlayer(player, 1, itemSelected, null);
                        }
                    }
                }
            }
            else
            {
                Item itemSelected = null;
                if (player.character.otherInventory == null)
                {
                    WorldItem worldItem = null;
                    // If its a world item
                    try
                    {
                        worldItem = player.character.nearWorldItems.ElementAt(itemId);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        worldItem = null;
                    }

                    if (worldItem != null)
                    {
                        itemSelected = worldItem.ToItem();

                        // If item is an item container, then we open it
                        if (itemSelected.IsItemContainer())
                        {
                            if (player.character.leftOpenedContainer == null)
                            {
                                OpenItemContainerForPlayer(player, 0, itemSelected, worldItem);
                            }
                        }
                    }
                }
                else
                {
                    // Else we remove it from the other inventory
                    try
                    {
                        itemSelected = player.character.otherInventory.items.ElementAt(itemId);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        itemSelected = null;
                    }

                    if (itemSelected != null)
                    {
                        player.character.otherInventory.RemoveItem(itemSelected);

                        // If item is an item container, then we open it
                        if (itemSelected.IsItemContainer())
                        {
                            if (player.character.leftOpenedContainer == null)
                            {
                                OpenItemContainerForPlayer(player, 0, itemSelected, null);
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Handles the item right click event for a player
        /// </summary>
        /// <param name="player">The player that triggers the event</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemId">The item id</param>
        public static void HandleItemRightClickEvent(Player player, int isRight, int itemId)
        {
            Inventory playerInventory = player.character.inventory;
            // Is right grid
            if (isRight == 1)
            {
                // If there is an opened container
                if (player.character.rightOpenedContainer != null)
                {
                    Item itemSelected = null;
                    // Get the selected item and container info
                    try
                    {
                        itemSelected = player.character.rightOpenedContainer.contentList.ElementAt(itemId);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        itemSelected = null;
                    }

                    if (itemSelected != null)
                    {
                        ItemData itemData = ItemData.GetById(itemSelected.id);
                        int weight = itemData.itemWeight;
                        int currentWeight;
                        int maxWeight;
                        bool remove = false;
                        // If there is another opened container at the other grid
                        if (player.character.leftOpenedContainer != null)
                        {
                            // Get the other container's info and check if the item fits in
                            ItemData containerData = ItemData.GetById(player.character.leftOpenedContainer.id);
                            maxWeight = containerData.maxContentWeight;
                            currentWeight = player.character.leftOpenedContainer.GetContentWeight();
                            if (currentWeight + weight <= maxWeight && containerData.CanContainItem(itemSelected.id))
                            {
                                // If the opened container is a world item
                                if (player.character.leftOpenedContainerWorld != null)
                                {
                                    player.character.leftOpenedContainerWorld.contentList.Add(itemSelected);
                                    player.character.leftOpenedContainer = player.character.leftOpenedContainerWorld.ToItem();
                                    remove = true;
                                }
                                else
                                {
                                    // If the opened container is part of another inventory
                                    player.character.leftOpenedContainer.contentList.Add(itemSelected);
                                    remove = true;
                                }
                            }
                        }
                        else
                        {
                            // If there is no container opened and no inventory opened at the left grid
                            if (player.character.otherInventory == null)
                            {
                                // Create the new world item
                                Vector3 footPos = player.GetFootPos();
                                Vector3 worldPos = new Vector3(footPos.X, footPos.Y, footPos.Z - itemData.worldZOffset);
                                Vector3 worldRotation = new Vector3(itemData.worldRotation.X, itemData.worldRotation.Y, player.user.Rotation.Z);
                                WorldItem newWorldItem = WorldItem.Create(itemSelected, worldPos, worldRotation, player.user.Dimension);
                                player.character.nearWorldItems.Add(newWorldItem);
                                remove = true;
                            }
                            else
                            {
                                // First check if the item fits in the other inventory
                                maxWeight = player.character.otherInventory.GetMaxWeight();
                                currentWeight = player.character.otherInventory.weight;
                                if (currentWeight + weight <= maxWeight)
                                {
                                    // If so then add it and save
                                    if (player.character.otherInventory.AddItem(itemSelected) == 1)
                                        remove = true;
                                }
                            }
                        }

                        if (remove)
                        {
                            // Make the change effect on the interface
                            NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", false, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, weight);
                            NAPI.ClientEvent.TriggerClientEvent(player.user, "remove_item_inv_interface", true, itemId, itemData.itemWeight);

                            // Remove the item from the container and save player inventory
                            player.character.rightOpenedContainer.contentList.Remove(itemSelected);
                        }
                        else
                        {
                            player.DisplayNotification("~g~No cabe en el inventario.");
                        }
                    }
                }
                else
                {
                    // If ID is a special slot
                    if (itemId <= Global.PlayerSpecialSlotsCount - 1)
                    {
                        Item itemSelected = null;
                        switch (itemId)
                        {
                            case (int)Global.InventoryBodypart.RightHand:
                                {
                                    Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                                    if (weapon != null)
                                    {
                                        itemSelected = weapon;
                                    }
                                    else
                                    {
                                        itemSelected = playerInventory.rightHand;
                                    }
                                    break;
                                }
                            case (int)Global.InventoryBodypart.LeftHand:
                                {
                                    itemSelected = playerInventory.leftHand;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Bodyarmor:
                                {
                                    itemSelected = playerInventory.bodyarmor;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Backpack:
                                {
                                    itemSelected = playerInventory.backpack;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Gloves:
                                {
                                    itemSelected = playerInventory.gloves;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Hat:
                                {
                                    itemSelected = playerInventory.hat;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Glasses:
                                {
                                    itemSelected = playerInventory.glasses;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Mask:
                                {
                                    itemSelected = playerInventory.mask;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Accessory:
                                {
                                    itemSelected = playerInventory.accessory;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Ears:
                                {
                                    itemSelected = playerInventory.ears;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Torso:
                                {
                                    itemSelected = playerInventory.torso;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Legs:
                                {
                                    itemSelected = playerInventory.legs;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Feet:
                                {
                                    itemSelected = playerInventory.feet;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Watch:
                                {
                                    itemSelected = playerInventory.watch;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.Bracelet:
                                {
                                    itemSelected = playerInventory.bracelet;
                                    break;
                                }
                        }

                        if (itemSelected != null)
                        {
                            // Get the selected item and container info
                            ItemData itemData = ItemData.GetById(itemSelected.id);
                            int weight = itemData.itemWeight;
                            int currentWeight;
                            int maxWeight;
                            bool remove = false;
                            // If there is an opened container at the other grid
                            if (player.character.leftOpenedContainer != null)
                            {
                                // Get the other container's info and check if the item fits in
                                ItemData containerData = ItemData.GetById(player.character.leftOpenedContainer.id);
                                maxWeight = containerData.maxContentWeight;
                                currentWeight = player.character.leftOpenedContainer.GetContentWeight();
                                if (currentWeight + weight <= maxWeight && containerData.CanContainItem(itemSelected.id))
                                {
                                    // If the opened container is a world item
                                    if (player.character.leftOpenedContainerWorld != null)
                                    {
                                        player.character.leftOpenedContainerWorld.contentList.Add(itemSelected);
                                        player.character.leftOpenedContainer = player.character.leftOpenedContainerWorld.ToItem();
                                        remove = true;
                                    }
                                    else
                                    {
                                        // If the opened container is part of another inventory
                                        player.character.leftOpenedContainer.contentList.Add(itemSelected);
                                        remove = true;
                                    }
                                }
                            }
                            else
                            {
                                // If there is no container opened and no inventory opened at the left grid
                                if (player.character.otherInventory == null)
                                {
                                    // Create the new world item
                                    Vector3 footPos = player.GetFootPos();
                                    Vector3 worldPos = new Vector3(footPos.X, footPos.Y, footPos.Z - itemData.worldZOffset);
                                    Vector3 worldRotation = new Vector3(itemData.worldRotation.X, itemData.worldRotation.Y, player.user.Rotation.Z);
                                    WorldItem newWorldItem = WorldItem.Create(itemSelected, worldPos, worldRotation, player.user.Dimension);
                                    player.character.nearWorldItems.Add(newWorldItem);
                                    remove = true;
                                }
                                else
                                {
                                    // First check if the item fits in the other inventory
                                    maxWeight = player.character.otherInventory.GetMaxWeight();
                                    currentWeight = player.character.otherInventory.weight;
                                    if (currentWeight + weight <= maxWeight)
                                    {
                                        // If so then add it and save
                                        if (player.character.otherInventory.AddItem(itemSelected) == 1)
                                            remove = true;
                                    }
                                }

                            }

                            if (remove)
                            {
                                // Make the change effect on the interface
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", false, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, weight);
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "removeSpecialItemInventory", itemId, itemData.itemWeight);

                                // Remove the item from the inventory and save
                                playerInventory.RemoveItem(itemSelected);
                            }
                            else
                            {
                                player.DisplayNotification("~g~No cabe en el inventario.");
                            }
                        }
                    }
                    else
                    {
                        Item itemSelected = null;
                        // Get the selected item and container info
                        try
                        {
                            itemSelected = playerInventory.items.ElementAt(itemId - Global.PlayerSpecialSlotsCount);
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                            Log.Debug(e.StackTrace);
                            itemSelected = null;
                        }

                        if (itemSelected != null)
                        {
                            ItemData itemData = ItemData.GetById(itemSelected.id);
                            int weight = itemData.itemWeight;
                            int currentWeight;
                            int maxWeight;
                            bool remove = false;
                            // If there is an opened container at the other grid
                            if (player.character.leftOpenedContainer != null)
                            {
                                // Get the other container's info and check if the item fits in
                                ItemData containerData = ItemData.GetById(player.character.leftOpenedContainer.id);
                                maxWeight = containerData.maxContentWeight;
                                currentWeight = player.character.leftOpenedContainer.GetContentWeight();
                                if (currentWeight + weight <= maxWeight && containerData.CanContainItem(itemSelected.id))
                                {
                                    // If the opened container is a world item
                                    if (player.character.leftOpenedContainerWorld != null)
                                    {
                                        player.character.leftOpenedContainerWorld.contentList.Add(itemSelected);
                                        player.character.leftOpenedContainer = player.character.leftOpenedContainerWorld.ToItem();
                                        remove = true;
                                    }
                                    else
                                    {
                                        // If the opened container is part of another inventory
                                        player.character.leftOpenedContainer.contentList.Add(itemSelected);
                                        remove = true;
                                    }
                                }
                            }
                            else
                            {
                                // If there is no container opened and no inventory opened at the left grid
                                if (player.character.otherInventory == null)
                                {
                                    // Create the new world item
                                    Vector3 footPos = player.GetFootPos();
                                    Vector3 worldPos = new Vector3(footPos.X, footPos.Y, footPos.Z - itemData.worldZOffset);
                                    Vector3 worldRotation = new Vector3(itemData.worldRotation.X, itemData.worldRotation.Y, player.user.Rotation.Z);
                                    WorldItem newWorldItem = WorldItem.Create(itemSelected, worldPos, worldRotation, player.user.Dimension);
                                    player.character.nearWorldItems.Add(newWorldItem);
                                    remove = true;
                                }
                                else
                                {
                                    // First check if the item fits in the other inventory
                                    maxWeight = player.character.otherInventory.GetMaxWeight();
                                    currentWeight = player.character.otherInventory.weight;
                                    if (currentWeight + weight <= maxWeight)
                                    {
                                        // If so then add it and save
                                        if (player.character.otherInventory.AddItem(itemSelected) == 1)
                                            remove = true;
                                    }
                                }
                            }

                            if (remove)
                            {
                                // Make the change effect on the interface
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", false, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, weight);
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "remove_item_inv_interface", true, itemId, itemData.itemWeight);

                                // Remove the item from the inventory and save
                                playerInventory.RemoveItem(itemSelected);
                            }
                            else
                            {
                                player.DisplayNotification("~g~No cabe en el inventario.");
                            }
                        }
                    }
                }
            }
            else
            {
                // If there is an opened container
                if (player.character.leftOpenedContainer != null)
                {
                    Item itemSelected = null;
                    // Get the selected item and container info
                    try
                    {
                        itemSelected = player.character.leftOpenedContainer.contentList.ElementAt(itemId);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        itemSelected = null;
                    }

                    if (itemSelected != null)
                    {
                        ItemData itemData = ItemData.GetById(itemSelected.id);
                        int weight = itemData.itemWeight;
                        int currentWeight;
                        int maxWeight;
                        bool addInterface = true;
                        bool avoidAdding = false;
                        bool remove = false;

                        if (itemSelected.IsWeapon())
                        {
                            if(playerInventory.GetWeaponItem(NAPI.Util.WeaponNameToModel(itemData.weaponModel)) != null)
                            {
                                avoidAdding = true;
                            }
                        }
                        
                        if (!avoidAdding)
                        {
                            int specialSlot = -1;

                            // If there is another opened container at the other grid
                            if (player.character.rightOpenedContainer != null)
                            {
                                // Get the other container's info and check if the item fits in
                                ItemData containerData = ItemData.GetById(player.character.rightOpenedContainer.id);
                                maxWeight = containerData.maxContentWeight;
                                currentWeight = player.character.rightOpenedContainer.GetContentWeight();
                                if (currentWeight + weight <= maxWeight && containerData.CanContainItem(itemSelected.id))
                                {
                                    player.character.rightOpenedContainer.contentList.Add(itemSelected);

                                    remove = true;
                                }
                            }
                            else
                            {
                                // First check if the item fits in the inventory
                                maxWeight = playerInventory.GetMaxWeight();
                                currentWeight = playerInventory.weight;
                                if (currentWeight + weight <= maxWeight)
                                {

                                    switch (itemData.type)
                                    {
                                        case (int)Global.ItemType.Backpack:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Backpack;
                                                break;
                                            }
                                        case (int)Global.ItemType.Bodyarmor:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Bodyarmor;
                                                break;
                                            }
                                        case (int)Global.ItemType.Glasses:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Glasses;
                                                break;
                                            }
                                        case (int)Global.ItemType.Gloves:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Gloves;
                                                break;
                                            }
                                        case (int)Global.ItemType.Hat:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Hat;
                                                break;
                                            }
                                        case (int)Global.ItemType.Mask:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Mask;
                                                break;
                                            }
                                        case (int)Global.ItemType.TorsoClothes:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Torso;
                                                break;
                                            }
                                        case (int)Global.ItemType.LegClothes:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Legs;
                                                break;
                                            }
                                        case (int)Global.ItemType.Shoes:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Feet;
                                                break;
                                            }
                                        case (int)Global.ItemType.EarAccessory:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Ears;
                                                break;
                                            }
                                        case (int)Global.ItemType.Accessory:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Accessory;
                                                break;
                                            }
                                        case (int)Global.ItemType.Watch:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Watch;
                                                break;
                                            }
                                        case (int)Global.ItemType.Bracelet:
                                            {
                                                specialSlot = (int)Global.InventoryBodypart.Bracelet;
                                                break;
                                            }
                                    }

                                    if (itemSelected.IsWeapon())
                                    {
                                        specialSlot = (int)Global.InventoryBodypart.RightHand;
                                    }

                                    // If so then add it and save
                                    if (playerInventory.AddItem(itemSelected, specialSlot) == 1)
                                        remove = true;
                                }
                            }

                            if (remove)
                            {
                                // Make the change effect on the interface
                                if (addInterface)
                                {
                                    NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", false, itemId, weight);
                                    if (specialSlot != -1)
                                    {
                                        if(!(itemSelected.IsWeapon() && itemSelected.quantity == 0))
                                            NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", specialSlot, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, weight);
                                    }
                                    else
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", true, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, weight);
                                    }
                                }

                                // Remove the item from the container and save inventory
                                if (player.character.leftOpenedContainerWorld != null)
                                {
                                    player.character.leftOpenedContainerWorld.contentList.Remove(itemSelected);
                                    player.character.leftOpenedContainer = player.character.leftOpenedContainerWorld.ToItem();
                                }
                                else
                                {
                                    player.character.leftOpenedContainer.contentList.Remove(itemSelected);
                                }
                            }
                            else
                            {
                                player.DisplayNotification("~g~No te cabe en el inventario.");
                            }
                        }
                    }
                }
                else
                {

                    Item itemSelected = null;
                    ItemData itemData;
                    int weight = 0;
                    int currentWeight;
                    int maxWeight;
                    bool avoidAdding = true;
                    bool addInterface = false;
                    if (player.character.otherInventory == null)
                    {
                        WorldItem worldItem = null;
                        // If its a world item we destroy it
                        try
                        {
                            worldItem = player.character.nearWorldItems.ElementAt(itemId);
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                            Log.Debug(e.StackTrace);
                            worldItem = null;
                        }

                        if (worldItem != null)
                        {
                            itemSelected = worldItem.ToItem();
                            itemData = ItemData.GetById(itemSelected.id);
                            avoidAdding = false;
                            addInterface = true;

                            if (itemSelected.IsWeapon())
                            {
                                if (playerInventory.GetWeaponItem(NAPI.Util.WeaponNameToModel(itemData.weaponModel)) != null)
                                {
                                    avoidAdding = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Else we remove it from the other inventory
                        try
                        {
                            itemSelected = player.character.otherInventory.items.ElementAt(itemId);
                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                            Log.Debug(e.StackTrace);
                            itemSelected = null;
                        }

                        if (itemSelected != null)
                        {
                            itemData = ItemData.GetById(itemSelected.id);
                            weight = itemData.itemWeight;
                            avoidAdding = false;
                            addInterface = true;

                            if (itemSelected.IsWeapon())
                            {
                                if (playerInventory.GetWeaponItem(NAPI.Util.WeaponNameToModel(itemData.weaponModel)) != null)
                                {
                                    avoidAdding = true;
                                }
                            }
                        }
                    }

                    if (!avoidAdding)
                    {
                        itemData = ItemData.GetById(itemSelected.id);
                        // If there is another opened container at the other grid
                        if (player.character.rightOpenedContainer != null)
                        {
                            // Get the other container's info and check if the item fits in
                            ItemData containerData = ItemData.GetById(player.character.rightOpenedContainer.id);
                            maxWeight = containerData.maxContentWeight;
                            currentWeight = player.character.rightOpenedContainer.GetContentWeight();
                            if (currentWeight + weight <= maxWeight && containerData.CanContainItem(itemSelected.id))
                            {
                                player.character.rightOpenedContainer.contentList.Add(itemSelected);
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", true, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", false, itemId, weight);

                                if (player.character.otherInventory == null)
                                {
                                    WorldItem worldItem = null;
                                    // If its a world item we destroy it
                                    try
                                    {
                                        worldItem = player.character.nearWorldItems.ElementAt(itemId);
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug(e.Message);
                                        Log.Debug(e.StackTrace);
                                        worldItem = null;
                                    }

                                    if (worldItem != null)
                                    {
                                        worldItem.destroy();
                                        player.character.nearWorldItems.Remove(worldItem);
                                    }
                                }
                                else
                                {
                                    // Else remove it from the other inventory
                                    player.character.otherInventory.RemoveItem(itemSelected);
                                }
                            }
                        }
                        else
                        {
                            // First check if the item fits in the inventory
                            int specialSlot = -1;
                            maxWeight = playerInventory.GetMaxWeight();
                            currentWeight = playerInventory.weight;
                            if (currentWeight + weight <= maxWeight)
                            {
                                switch (itemData.type)
                                {
                                    case (int)Global.ItemType.Backpack:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Backpack;
                                            break;
                                        }
                                    case (int)Global.ItemType.Bodyarmor:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Bodyarmor;
                                            break;
                                        }
                                    case (int)Global.ItemType.Glasses:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Glasses;
                                            break;
                                        }
                                    case (int)Global.ItemType.Gloves:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Gloves;
                                            break;
                                        }
                                    case (int)Global.ItemType.Hat:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Hat;
                                            break;
                                        }
                                    case (int)Global.ItemType.Mask:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Mask;
                                            break;
                                        }
                                    case (int)Global.ItemType.TorsoClothes:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Torso;
                                            break;
                                        }
                                    case (int)Global.ItemType.LegClothes:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Legs;
                                            break;
                                        }
                                    case (int)Global.ItemType.Shoes:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Feet;
                                            break;
                                        }
                                    case (int)Global.ItemType.EarAccessory:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Ears;
                                            break;
                                        }
                                    case (int)Global.ItemType.Accessory:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Accessory;
                                            break;
                                        }
                                    case (int)Global.ItemType.Watch:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Watch;
                                            break;
                                        }
                                    case (int)Global.ItemType.Bracelet:
                                        {
                                            specialSlot = (int)Global.InventoryBodypart.Bracelet;
                                            break;
                                        }
                                }

                                if (itemSelected.IsWeapon())
                                {
                                    Item hasWeapon = playerInventory.GetWeaponItem(NAPI.Util.WeaponNameToModel(ItemData.GetById(itemSelected.id).weaponModel));
                                    if (hasWeapon == null)
                                    {
                                        specialSlot = (int)Global.InventoryBodypart.RightHand;
                                    }
                                    else
                                    {
                                        addInterface = false;
                                    }
                                }

                                // Make the change effect on the interface
                                if (addInterface)
                                {
                                    // If so then add it and save
                                    if (playerInventory.AddItem(itemSelected, specialSlot) == 1)
                                    {
                                        NAPI.ClientEvent.TriggerClientEvent(player.user, "removeItemInventory", false, itemId, weight);

                                        if (specialSlot != -1)
                                        {
                                            if (!(itemSelected.IsWeapon() && itemSelected.quantity == 0))
                                                NAPI.ClientEvent.TriggerClientEvent(player.user, "addSpecialItemInventory", specialSlot, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                        }
                                        else
                                        {
                                            NAPI.ClientEvent.TriggerClientEvent(player.user, "addItemInventory", true, itemSelected.GetOverviewText(), itemData.image, itemSelected.quantity, itemData.itemWeight);
                                        }

                                        if (player.character.otherInventory == null)
                                        {
                                            WorldItem worldItem = null;
                                            // If its a world item we destroy it
                                            Log.Debug("ITEM: " + itemId);
                                            try
                                            {
                                                worldItem = player.character.nearWorldItems.ElementAt(itemId);
                                            }
                                            catch (Exception e)
                                            {
                                                Log.Debug(e.Message);
                                                Log.Debug(e.StackTrace);
                                                worldItem = null;
                                            }

                                            if (worldItem != null)
                                            {
                                                worldItem.destroy();
                                                player.character.nearWorldItems.Remove(worldItem);
                                            }
                                        }
                                        else
                                        {
                                            // Else we remove it from the other inventory
                                            player.character.otherInventory.RemoveItem(itemSelected);
                                        }
                                    }
                                    else
                                    {
                                        player.DisplayNotification("~g~No te cabe en el inventario.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the return click event for a player
        /// </summary>
        /// <param name="player">The player that triggers the event</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        public static void HandleReturnClickEvent(Player player, int isRight)
        {
            CloseItemContainerForPlayer(player, isRight);
        }

        /// <summary>
        /// Opens an item container to the player inventory interface
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="itemContainer">The item container</param>
        /// <param name="worldItem">The world item</param>
        public static void OpenItemContainerForPlayer(Player player, int isRight, Item itemContainer, WorldItem worldItem)
        {
            ItemData itemData = ItemData.GetById(itemContainer.id);
            int contentWeight = itemContainer.GetContentWeight();
            if (isRight == 1)
            {
                player.character.rightOpenedContainer = itemContainer;
                NAPI.ClientEvent.TriggerClientEvent(player.user, "openContainerInventory", true, itemData.nameSingular, contentWeight, itemData.maxContentWeight, itemContainer.GetContentJSON());
            }
            else
            {
                if (player.character.otherInventory == null)
                {
                    player.character.leftOpenedContainerWorld = worldItem;
                }

                player.character.leftOpenedContainer = itemContainer;

                NAPI.ClientEvent.TriggerClientEvent(player.user, "openContainerInventory", false, itemData.nameSingular, contentWeight, itemData.maxContentWeight, itemContainer.GetContentJSON());
            }
        }

        /// <summary>
        /// Close an item container to the player inventory interface
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        public static void CloseItemContainerForPlayer(Player player, int isRight)
        {
            if (isRight == 1)
            {
                player.character.rightOpenedContainer = null;
                NAPI.ClientEvent.TriggerClientEvent(player.user, "closeContainerInventory", true, "Inventario", player.character.inventory.weight, player.character.inventory.GetMaxWeight(), player.character.inventory.GetJSON(player));
            }
            else
            {
                player.character.leftOpenedContainer = null;
                player.character.leftOpenedContainerWorld = null;
                if (player.character.otherInventory == null)
                {
                    string worldItemsJSON = WorldItem.ToJSON(player.character.nearWorldItems);
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "closeContainerInventory", false, "Cerca", player.character.nearWorldItems.Count(), "-", worldItemsJSON);
                }

            }
        }

        /// <summary>
        /// Does something when a player 'uses' the item on his right hand
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void UseItemForPlayer(Player player)
        {
            if (player.CanUseInventory())
            {
                Inventory playerInventory = player.character.inventory;
                Item item = playerInventory.rightHand;

                if(item != null)
                {
                    ItemData itemData = ItemData.GetById(item.id);
                    switch(itemData.type)
                    {
                        case (int)Global.ItemType.Backpack:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Backpack);
                                break;
                            }
                        case (int)Global.ItemType.Bodyarmor:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Bodyarmor);
                                break;
                            }
                        case (int)Global.ItemType.Glasses:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Glasses);
                                break;
                            }
                        case (int)Global.ItemType.Gloves:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Gloves);
                                break;
                            }
                        case (int)Global.ItemType.Hat:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Hat);
                                break;
                            }
                        case (int)Global.ItemType.Mask:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Mask);
                                break;
                            }
                        case (int)Global.ItemType.TorsoClothes:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Torso);
                                break;
                            }
                        case (int)Global.ItemType.LegClothes:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Legs);
                                break;
                            }
                        case (int)Global.ItemType.Shoes:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Feet);
                                break;
                            }
                        case (int)Global.ItemType.EarAccessory:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Ears);
                                break;
                            }
                        case (int)Global.ItemType.Accessory:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Accessory);
                                break;
                            }
                        case (int)Global.ItemType.Watch:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Watch);
                                break;
                            }
                        case (int)Global.ItemType.Bracelet:
                            {
                                playerInventory.RemoveItem(item);
                                playerInventory.AddItem(item, (int)Global.InventoryBodypart.Bracelet);
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// Opens the item management menu for the item in the specified hand
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left hand</param>
        public static void OpenItemManagementMenu(Player player, int isRight)
        {
            Inventory playerInventory = player.character.inventory;

            if (isRight == 1)
            {
                Menu menu = new Menu("", "Mano derecha", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnRightItemManagementMenuResponse));
                Item item = null;
                if (playerInventory.rightHand != null)
                {
                    item = playerInventory.rightHand;
                }
                else
                {
                    Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                    if (weapon != null)
                    {
                        item = weapon;
                    }
                }

                if (item != null)
                {
                    // Use action
                    menu.menuModel.items.Add(new MenuItemModel(item.GetUseText(), item.GetUseDescriptionText()));

                    // Throw action
                    menu.menuModel.items.Add(new MenuItemModel("Tirar", "Tirar el objeto al suelo"));

                    // Change hand action
                    if (!item.IsWeapon())
                    {
                        // Save action
                        menu.menuModel.items.Add(new MenuItemModel("Guardar", "Guardar el objeto en tu inventario"));

                        menu.menuModel.items.Add(new MenuItemModel("Pasar a mano izquierda", "Pasar el objeto a la mano izquierda"));
                    }

                    menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

                    GuiController.CreateMenu(player, menu);
                }
            }
            else
            {
                Menu menu = new Menu("", "Mano izquierda", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnLeftItemManagementMenuResponse));
                Item item = playerInventory.leftHand;
                if (item != null)
                {
                    // Save action
                    menu.menuModel.items.Add(new MenuItemModel("Guardar", "Guardar el objeto en tu inventario"));

                    // Throw action
                    menu.menuModel.items.Add(new MenuItemModel("Tirar", "Tirar el objeto al suelo"));

                    // Change hand action
                    menu.menuModel.items.Add(new MenuItemModel("Pasar a mano derecha", "Pasar el objeto a la mano derecha"));

                    menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

                    GuiController.CreateMenu(player, menu);
                }
            }
        }

        /// <summary>
        /// Receives the right hand item management menu resposes
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The selected option</param>
        /// <param name="action">The selected action id</param>
        public static void OnRightItemManagementMenuResponse(Player player, string option, int action)
        {
            Inventory playerInventory = player.character.inventory;
            Item item = null;
            if (playerInventory.rightHand != null)
            {
                item = playerInventory.rightHand;
            }
            else
            {
                Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                if (weapon != null)
                {
                    item = weapon;
                }
            }

            if (item != null)
            {
                if (!item.IsWeapon())
                {
                    switch (action)
                    {
                        case 0:
                            {
                                UseItemForPlayer(player);
                                break;
                            }
                        case 1:
                            {
                                ThrowItemToGround(player, true);
                                break;
                            }
                        case 2:
                            {
                                SaveItemToSelfInventory(player, true);
                                break;
                            }
                        case 3:
                            {
                                ChangeItemHand(player, (int)Global.InventoryBodypart.RightHand);
                                break;
                            }
                        case 4:
                            {
                                SelfInteraction.GenerateMenu(player);
                                break;
                            }

                    }
                }
                else
                {
                    switch (action)
                    {
                        case 0:
                            {
                                OpenWeaponManagementMenu(player);
                                break;
                            }
                        case 1:
                            {
                                ThrowItemToGround(player, true);
                                break;
                            }
                        case 2:
                            {
                                SelfInteraction.GenerateMenu(player);
                                break;
                            }

                    }
                }
            }
            else
            {
                player.DisplayNotification("Ya no tienes ningun objeto aqui.");
            }
        }

        /// <summary>
        /// Receives the left hand item management menu resposes
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The selected option</param>
        /// <param name="action">The selected action id</param>
        public static void OnLeftItemManagementMenuResponse(Player player, string option, int action)
        {
            Inventory playerInventory = player.character.inventory;
            if (playerInventory.leftHand != null)
            {

                switch (action)
                {
                    case 0:
                        {
                            SaveItemToSelfInventory(player, false);
                            break;
                        }
                    case 1:
                        {
                            ThrowItemToGround(player, false);
                            break;
                        }
                    case 2:
                        {
                            ChangeItemHand(player, (int)Global.InventoryBodypart.LeftHand);
                            break;
                        }
                    case 3:
                        {
                            SelfInteraction.GenerateMenu(player);
                            break;
                        }

                }
            }
            else
            {
                player.DisplayNotification("Ya no tienes ningun objeto aqui.");
            }

        }

        /// <summary>
        /// Saves a hand item to the self inventory
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        public static void SaveItemToSelfInventory(Player player, bool isRight)
        {
            if (player.CanUseInventory())
            {
                Inventory playerInventory = player.character.inventory;
                if (isRight)
                {
                    Item item = null;
                    if (playerInventory.rightHand != null)
                    {
                        item = playerInventory.rightHand;
                        playerInventory.RemoveItem(item);
                        playerInventory.AddItem(item);
                    }
                }
                else
                {
                    Item item = null;
                    if (playerInventory.leftHand != null)
                    {
                        item = playerInventory.leftHand;
                        playerInventory.RemoveItem(item);
                        playerInventory.AddItem(item);
                    }
                }
            }
        }

        /// <summary>
        /// Throws a hand item to the ground
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        public static void ThrowItemToGround(Player player, bool isRight)
        {
            if (player.CanUseInventory())
            {
                Inventory playerInventory = player.character.inventory;
                if (isRight)
                {
                    Item item = null;
                    if (playerInventory.rightHand != null)
                    {
                        item = playerInventory.rightHand;
                    }
                    else
                    {
                        Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                        if (weapon != null)
                        {
                            item = weapon;
                        }
                    }

                    if (item != null)
                    {
                        ItemData itemData = ItemData.GetById(item.id);
                        // Create the new world item
                        Vector3 footPos = player.GetFootPos();
                        Vector3 worldPos = new Vector3(footPos.X, footPos.Y, footPos.Z - itemData.worldZOffset);
                        Vector3 worldRotation = new Vector3(itemData.worldRotation.X, itemData.worldRotation.Y, player.user.Rotation.Z);
                        WorldItem newWorldItem = WorldItem.Create(item, worldPos, worldRotation, player.user.Dimension);
                        playerInventory.RemoveItem(item);
                        player.SendAme("Tira " + itemData.GetComposedName(false) + " al suelo.");
                    }
                }
                else
                {
                    Item item = null;
                    if (playerInventory.leftHand != null)
                    {
                        item = playerInventory.leftHand;

                        ItemData itemData = ItemData.GetById(item.id);
                        // Create the new world item
                        Vector3 footPos = player.GetFootPos();
                        Vector3 worldPos = new Vector3(footPos.X, footPos.Y, footPos.Z - itemData.worldZOffset);
                        Vector3 worldRotation = new Vector3(itemData.worldRotation.X, itemData.worldRotation.Y, player.user.Rotation.Z);
                        WorldItem newWorldItem = WorldItem.Create(item, worldPos, worldRotation, player.user.Dimension);
                        playerInventory.RemoveItem(item);
                        player.SendAme("Tira " + itemData.GetComposedName(false) + " al suelo.");
                    }
                }
            }
        }

        /// <summary>
        /// Takes an item from one of the pockets to the hand
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="isRight">If is right or left inventory grid</param>
        /// <param name="pocketId">The pocket id</param>
        public static void TakeItemFromPocketToHand(Player player, bool isRight, int pocketId)
        {
            if (player.CanUseInventory())
            {
                // Player inventory
                Inventory playerInventory = player.character.inventory;
                // Search the item
                Item itemSelected = null;
                switch (pocketId)
                {
                    case (int)Global.InventoryBodypart.Bodyarmor - 1:
                        {
                            itemSelected = playerInventory.bodyarmor;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Backpack - 1:
                        {
                            itemSelected = playerInventory.backpack;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Gloves - 1:
                        {
                            itemSelected = playerInventory.gloves;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Hat - 1:
                        {
                            itemSelected = playerInventory.hat;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Glasses - 1:
                        {
                            itemSelected = playerInventory.glasses;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Mask - 1:
                        {
                            itemSelected = playerInventory.mask;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Accessory - 1:
                        {
                            itemSelected = playerInventory.accessory;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Ears - 1:
                        {
                            itemSelected = playerInventory.ears;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Torso - 1:
                        {
                            itemSelected = playerInventory.torso;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Legs - 1:
                        {
                            itemSelected = playerInventory.legs;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Feet - 1:
                        {
                            itemSelected = playerInventory.feet;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Watch - 1:
                        {
                            itemSelected = playerInventory.watch;
                            break;
                        }
                    case (int)Global.InventoryBodypart.Bracelet - 1:
                        {
                            itemSelected = playerInventory.bracelet;
                            break;
                        }

                }

                // If item is not in special slots then check the pockets
                if (itemSelected == null && pocketId > Global.PlayerSpecialSlotsCount)
                {
                    pocketId = pocketId - (Global.PlayerSpecialSlotsCount - 2);
                    try
                    {
                        itemSelected = playerInventory.items.ElementAt(pocketId);
                    }
                    catch (Exception e)
                    {
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        itemSelected = null;
                    }
                }

                // If item is found
                if (itemSelected != null)
                {
                    ItemData itemData = ItemData.GetById(itemSelected.id);
                    // If selected hand is right
                    if (isRight)
                    {
                        // Check if there is a weapon
                        Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                        if (weapon == null)
                        {
                            // Right hand is free
                            if (playerInventory.rightHand == null)
                            {
                                // Move the item to the player's hand
                                playerInventory.RemoveItem(itemSelected);
                                playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.RightHand);
                                player.DisplayNotification("~g~Sacas " + itemData.GetComposedName(false));
                            }
                            else
                            {
                                player.DisplayNotification("~r~Mano derecha ocupada.");
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~r~Tienes un arma en la mano, guardala antes de sacar algo.");
                        }
                    }
                    else
                    {
                        // If hand is left and is free
                        if (playerInventory.leftHand == null)
                        {
                            // Move the item to the player's left hand
                            playerInventory.RemoveItem(itemSelected);
                            playerInventory.AddItem(itemSelected, (int)Global.InventoryBodypart.LeftHand);
                            player.DisplayNotification("~g~Sacas " + itemData.GetComposedName(false));
                        }
                        else
                        {
                            player.DisplayNotification("~r~Mano izquierda ocupada.");
                        }
                    }
                }
                else
                {
                    player.DisplayNotification("~r~No hay nada en éste hueco.");
                }
            }
        }

        /// <summary>
        /// Switches the item's hand
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="fromHand">The origin hand</param>
        public static void ChangeItemHand(Player player, int fromHand)
        {
            if (player.CanUseInventory())
            {
                // Player's inventory
                Inventory playerInventory = player.character.inventory;
                if (fromHand == (int)Global.InventoryBodypart.RightHand)
                {
                    if (playerInventory.rightHand != null)
                    {
                        Item rightHandItem = playerInventory.rightHand;
                        if (playerInventory.leftHand != null)
                        {
                            Item leftHandItem = playerInventory.leftHand;

                            playerInventory.RemoveItem(rightHandItem);
                            playerInventory.RemoveItem(leftHandItem);
                            playerInventory.AddItem(rightHandItem, (int)Global.InventoryBodypart.LeftHand);
                            playerInventory.AddItem(leftHandItem, (int)Global.InventoryBodypart.RightHand);
                        }
                        else
                        {
                            playerInventory.RemoveItem(rightHandItem);
                            playerInventory.AddItem(rightHandItem, (int)Global.InventoryBodypart.LeftHand);
                        }
                    }

                }
                else if (fromHand == (int)Global.InventoryBodypart.LeftHand)
                {
                    if (playerInventory.leftHand != null)
                    {
                        Item leftHandItem = playerInventory.leftHand;
                        if (playerInventory.rightHand != null)
                        {
                            Item rightHandItem = playerInventory.leftHand;
                            playerInventory.RemoveItem(rightHandItem);
                            playerInventory.RemoveItem(leftHandItem);
                            playerInventory.AddItem(rightHandItem, (int)Global.InventoryBodypart.LeftHand);
                            playerInventory.AddItem(leftHandItem, (int)Global.InventoryBodypart.RightHand);
                        }
                        else
                        {
                            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                            if (weapon == null)
                            {
                                playerInventory.RemoveItem(leftHandItem);
                                playerInventory.AddItem(leftHandItem, (int)Global.InventoryBodypart.RightHand);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Takes the closest world item with the right hand
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void TakeClosestWorldItem(Player player)
        {
            if (player.CanUseInventory())
            {
                Inventory playerInventory = player.character.inventory;
                Item item = null;
                if (playerInventory.rightHand != null)
                {
                    item = playerInventory.rightHand;
                }
                else
                {
                    Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                    if (weapon != null)
                    {
                        item = weapon;
                    }
                }

                if (item == null)
                {
                    WorldItem worldItem = WorldItem.GetClosestItem(player.user.Position, 1f, player.user.Dimension);
                    if (worldItem != null)
                    {
                        Item toItem = worldItem.ToItem();
                        string name = ItemData.GetById(toItem.id).GetComposedName(false);
                        if (playerInventory.AddItem(toItem) == 1)
                        {
                            worldItem.destroy();
                            player.SendAme("Coge algo del suelo");
                            player.DisplayNotification("~g~Coges " + name + " del suelo.");
                        }
                        else
                        {
                            player.DisplayNotification("~g~No te cabe en el inventario.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~No hay nada cerca.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~Tienes algo en la mano derecha.");
                }
            }
        }

        /// <summary>
        /// Opens the weapon management menu for the current holding weapon
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void OpenWeaponManagementMenu(Player player)
        {
            Menu menu = new Menu("", "Modificar arma", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnWeaponManagementMenuResponse));
            Inventory playerInventory = player.character.inventory;
            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                // Examine weapon
                menu.menuModel.items.Add(new MenuItemModel("Examinar", "Muestra el numero de serie"));

                // Load ammo
                menu.menuModel.items.Add(new MenuItemModel("Cargar munición", "Recarga el arma"));

                // Unload ammo
                menu.menuModel.items.Add(new MenuItemModel("Descargar munición", "Descarga el arma"));

                // Add component
                menu.menuModel.items.Add(new MenuItemModel("Añadir accesorio", "Añade un accesorio al arma"));

                // Remove component
                menu.menuModel.items.Add(new MenuItemModel("Quitar accesorio", "Quita un accesorio al arma"));

                // Add tint
                menu.menuModel.items.Add(new MenuItemModel("Pintar arma", "Pinta el arma de un color o trama"));

                // Remove tint
                menu.menuModel.items.Add(new MenuItemModel("Quitar pintura", "Limpia la pintura del arma"));

                menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

                GuiController.CreateMenu(player, menu);
            }
        }

        /// <summary>
        /// Receives the weapon management menu resposes
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="action">The selected action</param>
        public static void OnWeaponManagementMenuResponse(Player player, string option, int action)
        {
            Inventory playerInventory = player.character.inventory;
            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                switch (action)
                {
                    case 0:
                        {
                            if (weapon.miscData.Count >= 2)
                            {
                                if (weapon.miscData[1] != "")
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Número de serie: ~g~" + weapon.miscData[1]);
                                }
                                else
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~El número de serie ha sido borrado.");
                                }
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Arma sin número de serie.");
                            }
                            break;
                        }
                    case 1:
                        {
                            LoadWeaponAmmo(player, weapon);
                            break;
                        }
                    case 2:
                        {
                            UnloadWeaponAmmo(player, weapon);
                            break;
                        }
                    case 3:
                        {
                            OpenAddWeaponComponentMenu(player, weapon);
                            break;
                        }
                    case 4:
                        {
                            OpenRemoveWeaponComponentMenu(player, weapon);
                            break;
                        }
                    case 5:
                        {
                            OpenAddWeaponTintMenu(player);
                            break;
                        }
                    case 6:
                        {
                            OpenRemoveWeaponTintPrompt(player);
                            break;
                        }
                    case 7:
                        {
                            OpenItemManagementMenu(player, 1);
                            break;
                        }
                }
            }

        }

        /// <summary>
        /// Loads ammo to the current weapon from an ammo case
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="weapon">The weapon</param>
        public static void LoadWeaponAmmo(Player player, Item weapon)
        {
            Inventory playerInventory = player.character.inventory;
            int ammoId = weapon.GetAmmoCase();

            if (ammoId != -1)
            {
                ItemData itemData = ItemData.GetById(weapon.id);
                int neededAmmo = 0;
                if (weapon.quantity < itemData.quantity)
                {
                    neededAmmo = itemData.quantity - weapon.quantity;
                }

                if (neededAmmo > 0)
                {
                    int availableAmmo = 0;
                    List<Item> ammoItems = playerInventory.HasItems(ammoId);
                    foreach (Item ammoItem in ammoItems)
                    {
                        if (availableAmmo < neededAmmo)
                        {
                            if (ammoItem.quantity > 0)
                            {
                                if (ammoItem.quantity < neededAmmo)
                                {
                                    availableAmmo += ammoItem.quantity;
                                    ammoItem.quantity = 0;
                                }
                                else
                                {
                                    availableAmmo = neededAmmo;
                                    ammoItem.quantity -= neededAmmo;
                                }
                            }
                        }
                    }

                    if (availableAmmo > 0)
                    {
                        playerInventory.SetWeaponAmmo(availableAmmo);
                        player.DisplayNotification("~g~Arma cargada con " + availableAmmo + " cartuchos.");
                        player.SendAme("Recarga su arma con munición.");
                    }
                    else
                    {
                        player.DisplayNotification("~r~No tienes munición para ésta arma.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~El arma está llena.");
                }
            }
            else
            {
                player.DisplayNotification("~r~Ésta arma no necesita munición.");
            }
        }

        /// <summary>
        /// Unloads all ammo of the current weapon
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="weapon">The weapon</param>
        public static void UnloadWeaponAmmo(Player player, Item weapon)
        {
            Inventory playerInventory = player.character.inventory;
            int ammoId = weapon.GetAmmoCase();

            if (ammoId != -1)
            {
                if (weapon.quantity > 0)
                {
                    List<Item> ammoItems = playerInventory.HasItems(ammoId);
                    int leftAmmo = weapon.quantity;
                    foreach (Item ammoItem in ammoItems)
                    {
                        if (leftAmmo > 0)
                        {
                            ItemData itemData = ItemData.GetById(ammoItem.id);
                            if (ammoItem.quantity < itemData.quantity)
                            {
                                int rounds = Math.Abs(ammoItem.quantity - itemData.quantity);
                                if (rounds <= leftAmmo)
                                {
                                    int canFit = Math.Abs(rounds - leftAmmo);
                                    leftAmmo -= canFit;
                                    ammoItem.quantity += canFit;
                                }
                                else
                                {
                                    ammoItem.quantity += leftAmmo;
                                    leftAmmo = 0;
                                }
                            }
                        }
                    }

                    if (leftAmmo > 0)
                    {
                        if (playerInventory.AddNewItem(ammoId, leftAmmo) == 1)
                        {
                            player.DisplayNotification("~g~Arma descargada, obtienes " + weapon.quantity + " cartuchos.");
                            playerInventory.SetWeaponAmmo(0);
                        }
                        else
                        {
                            player.DisplayNotification("~r~No te cabe nada más en los bolsillos.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~g~Arma descargada, obtienes " + weapon.quantity + " cartuchos.");
                        playerInventory.SetWeaponAmmo(0);
                    }
                }
                else
                {
                    player.DisplayNotification("~r~Arma sin munición.");
                }
            }
        }

        /// <summary>
        /// Lists available attachable components
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="weapon">The weapon</param>
        public static void OpenAddWeaponComponentMenu(Player player, Item weapon)
        {
            Inventory playerInventory = player.character.inventory;
            Menu menu = new Menu("", "Añadir accesorio", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnAddWeaponComponentMenuResponse));

            List<Item> availableComponents = playerInventory.HasItemsFromType((int)Global.ItemType.WeaponAccessory);

            if (availableComponents.Count() > 0)
            {
                foreach (Item component in availableComponents)
                {
                    if (weapon.IsWeaponAccessoryCompatible(component))
                    {
                        // Check if weapon has already an identical accessory
                        if (!weapon.HasWeaponSameAccessory(component))
                        {
                            // Add component to menu
                            menu.menuModel.items.Add(new MenuItemModel(component.GetOverviewText()));
                        }
                    }
                }

                menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

                if (menu.menuModel.items.Count() > 0)
                {
                    GuiController.CreateMenu(player, menu);
                }
                else
                {
                    player.DisplayNotification("~r~No tienes accesorios compatibles.");
                }

            }
            else
            {
                player.DisplayNotification("~r~No tienes accesorios.");
            }
        }

        /// <summary>
        /// Receives the response of the AddWeaponComponent menu
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="component">The selected component</param>
        /// <param name="id">The component id</param>
        public static void OnAddWeaponComponentMenuResponse(Player player, string componentName, int id)
        {
            Inventory playerInventory = player.character.inventory;
            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                List<Item> availableComponents = playerInventory.HasItemsFromType((int)Global.ItemType.WeaponAccessory);
                List<Item> compatibleComponents = new List<Item>();
                foreach (Item component in availableComponents)
                {
                    if (weapon.IsWeaponAccessoryCompatible(component))
                    {
                        // Check if weapon has already an identical accessory
                        if (!weapon.HasWeaponSameAccessory(component))
                        {
                            compatibleComponents.Add(component);
                        }
                    }
                }

                try
                {
                    if(id < compatibleComponents.Count())
                    {
                        Item componentSelected = compatibleComponents.ElementAt(id);

                        playerInventory.AddWeaponAccessory(componentSelected);
                        playerInventory.RemoveItem(componentSelected);
                        ItemData itemData = ItemData.GetById(componentSelected.id);
                        player.DisplayNotification("~g~Añades " + itemData.GetComposedName(false) + " a tu arma.");
                        player.SendAme("Añade " + itemData.GetComposedName(false) + " a su arma.");
                    }
                    else
                    {
                        OpenWeaponManagementMenu(player);
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                    Log.Debug(e.StackTrace);
                    player.DisplayNotification("~r~Ya no tienes el accesorio seleccionado.");
                }
            }
            else
            {
                player.DisplayNotification("~r~Ya no tienes el arma en mano.");
            }
        }

        /// <summary>
        /// Lists attached components to the current weapon
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="weapon">The weapon</param>
        public static void OpenRemoveWeaponComponentMenu(Player player, Item weapon)
        {
            Inventory playerInventory = player.character.inventory;
            Menu menu = new Menu("", "Quitar accesorio", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnRemoveWeaponComponentMenuResponse));

            if (weapon.accessoryList.Count() > 0)
            {
                foreach (Item component in weapon.accessoryList)
                {
                    menu.menuModel.items.Add(new MenuItemModel(component.GetOverviewText()));
                }

                menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));
            
                GuiController.CreateMenu(player, menu);
            }
            else
            {
                player.DisplayNotification("~r~El arma no tiene accesorios.");
            }
        }

        /// <summary>
        /// Receives the response of the RemoveWeaponComponent menu
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="componentName">The component name</param>
        /// <param name="id">The component id</param>
        public static void OnRemoveWeaponComponentMenuResponse(Player player, string componentName, int id)
        {
            Inventory playerInventory = player.character.inventory;
            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                
                try
                {
                    if(id < weapon.accessoryList.Count())
                    {
                        Item componentSelected = weapon.accessoryList.ElementAt(id);
                        ItemData componentData = ItemData.GetById(componentSelected.id);

                        if ((playerInventory.weight + componentData.itemWeight) <= playerInventory.GetMaxWeight())
                        {
                            playerInventory.RemoveWeaponAccessory(componentSelected);
                            playerInventory.AddItem(componentSelected);
                            player.DisplayNotification("~g~Quitas " + componentData.GetComposedName(false) + " de tu arma.");
                            player.SendAme("Quita " + componentData.GetComposedName(false) + " de su arma.");
                        }
                        else
                        {
                            player.DisplayNotification("~r~Bolsillos llenos.");
                        }
                    }
                    else
                    {
                        OpenWeaponManagementMenu(player);
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                    Log.Debug(e.StackTrace);
                    player.DisplayNotification("~r~El arma ya no tiene el accesorio seleccionado.");
                }
            }
            else
            {
                player.DisplayNotification("~r~Ya no tienes el arma en mano.");
            }
        }

        /// <summary>
        /// Lists available sprays
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void OpenAddWeaponTintMenu(Player player)
        {
            Inventory playerInventory = player.character.inventory;
            Menu menu = new Menu("", "Pintar arma", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnAddWeaponTintMenuResponse));

            List<Item> availableSprays = playerInventory.HasItemsFromType((int)Global.ItemType.Spray);
            if (availableSprays.Count() == 0)
            {
                foreach (Item spray in availableSprays)
                {
                    // Add spray to menu
                    menu.menuModel.items.Add(new MenuItemModel(spray.GetOverviewText()));
                }

                menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

                GuiController.CreateMenu(player, menu);

            }
            else
            {
                player.DisplayNotification("~r~No tienes ningun spray de pintura.");
            }
        }

        /// <summary>
        /// Receives the response of the AddWeaponTint menu
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="sprayName">The spray name</param>
        /// <param name="id">The spray id</param>
        public static void OnAddWeaponTintMenuResponse(Player player, string sprayName, int id)
        {
            Inventory playerInventory = player.character.inventory;

            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                List<Item> availableSprays = playerInventory.HasItemsFromType((int)Global.ItemType.Spray);

                try
                {
                    if(id < availableSprays.Count())
                    {
                        Item spraySelected = availableSprays.ElementAt(id);

                        if (spraySelected.quantity > 0)
                        {
                            ItemData itemData = ItemData.GetById(spraySelected.id);
                            playerInventory.SetWeaponTint((WeaponTint)Enum.Parse(typeof(WeaponTint), itemData.weaponModel));
                            spraySelected.quantity -= 1;
                            playerInventory.save = true;
                            player.DisplayNotification("~g~Has pintado el arma con el spray.");
                            player.SendAme("Pinta su arma con un spray de pintura.");
                        }
                        else
                        {
                            player.DisplayNotification("~r~No queda pintura en el spray.");
                        }
                    }
                    else
                    {
                        OpenWeaponManagementMenu(player);
                    }
                }
                catch (Exception e)
                {
                    Log.Debug(e.Message);
                    Log.Debug(e.StackTrace);
                    player.DisplayNotification("~r~Ya no tienes el spray seleccionado.");
                }
            }
            else
            {
                player.DisplayNotification("~r~Ya no tienes el arma en mano.");
            }
        }

        /// <summary>
        /// Shows a dialog prompting for removing weapon's tint
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void OpenRemoveWeaponTintPrompt(Player player)
        {
            Inventory playerInventory = player.character.inventory;
            if(playerInventory.HasItem(125) != null && playerInventory.HasItem(126) != null)
            {
                Item has = playerInventory.HasItem(125);
                if(has.quantity > 0)
                {
                    Dialog dialog = new Dialog("Quitar pintura", "¿Seguro que quieres quitar al pintura del arma?", false, "Sí", "No", new Action<Player, string>(OnRemoveWeaponTintConfirmation), new Action<Player>(GuiController.DestroyDialog));
                    GuiController.CreateDialog(player, dialog);
                }
                else
                {
                    player.DisplayNotification("~r~No te queda suficiente aguarrás.");
                }
            }
            else
            {
                player.DisplayNotification("~r~Necesitas aguarrás y una esponja.");
            }
        }

        /// <summary>
        /// Removes the current weapon's tint
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="none"></param>
        public static void OnRemoveWeaponTintConfirmation(Player player, string none)
        {
            Inventory playerInventory = player.character.inventory;

            Item weapon = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
            if (weapon != null)
            {
                playerInventory.SetWeaponTint(WeaponTint.Normal);
                Item item = playerInventory.HasItem(125);
                item.quantity--;
                playerInventory.save = true;
            }
            else
            {
                player.DisplayNotification("~r~Ya no tienes el arma en mano.");
            }
        }

        /// <summary>
        /// Gives a player item on one hand to others player hand
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="target">The target player instance</param>
        /// <param name="sourceHand">The hand</param>
        public static void GivePlayerItemToPlayerHand(Player player, Player target, bool sourceHand)
        {
            if (player.CanUseInventory())
            {
                Inventory playerInventory = player.character.inventory;
                Inventory targetInventory = target.character.inventory;

                if (target != null)
                {
                    if (target.character != null)
                    {
                        if (target.user.Position.DistanceTo(player.user.Position) <= Global.ExternalInteractionPlayerDistance && !target.user.IsInVehicle && player.user.Dimension == target.user.Dimension)
                        {
                            // Source item is in right hand
                            if (sourceHand)
                            {
                                Item sourceItem = playerInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user));
                                if (sourceItem == null)
                                {
                                    sourceItem = playerInventory.rightHand;
                                }

                                if (sourceItem != null)
                                {
                                    ItemData itemData = ItemData.GetById(sourceItem.id);
                                    if (targetInventory.rightHand == null && targetInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(target.user)) == null)
                                    {
                                        if (targetInventory.AddItem(sourceItem, (int)Global.InventoryBodypart.RightHand) == 1)
                                        {
                                            playerInventory.RemoveItem(sourceItem);
                                            player.DisplayNotification("~g~Entregas tu " + itemData.nameSingular + " a " + target.character.showName + " en su mano derecha.");
                                            target.DisplayNotification("~g~" + player.character.showName + " te ha entregado " + itemData.nameSingular + " en tu mano derecha.");
                                            player.SendAme("Entrega su " + itemData.nameSingular + " a " + target.character.showName);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~El jugador tiene el inventario lleno.");
                                        }
                                    }
                                    else
                                    {
                                        if (!(sourceItem.IsWeapon() && sourceItem.IsHeavy()) && (targetInventory.leftHand == null))
                                        {
                                            if (targetInventory.AddItem(sourceItem, (int)Global.InventoryBodypart.LeftHand) == 1)
                                            {
                                                playerInventory.RemoveItem(sourceItem);
                                                player.DisplayNotification("~g~Entregas tu " + itemData.nameSingular + " a " + target.character.showName + " en su mano izquierda.");
                                                target.DisplayNotification("~g~" + player.character.showName + " te ha entregado " + itemData.nameSingular + " en tu mano izquierda.");
                                                player.SendAme("Entrega su " + itemData.nameSingular + " a " + target.character.showName);
                                            }
                                            else
                                            {
                                                player.DisplayNotification("~r~El jugador tiene el inventario lleno.");
                                            }
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Tiene las manos ocupadas.");
                                        }
                                    }
                                }
                                else
                                {
                                    player.DisplayNotification("~r~No tienes nada en tu mano derecha.");
                                }
                            }
                            else
                            {
                                // Source item is in left hand
                                Item sourceItem = playerInventory.leftHand;
                                if (sourceItem != null)
                                {
                                    ItemData itemData = ItemData.GetById(sourceItem.id);
                                    if (targetInventory.leftHand == null)
                                    {
                                        if (targetInventory.AddItem(sourceItem, (int)Global.InventoryBodypart.RightHand) == 1)
                                        {
                                            playerInventory.RemoveItem(sourceItem);
                                            player.DisplayNotification("~g~Entregas tu " + itemData.nameSingular + " a " + target.character.showName + " en su mano izquierda.");
                                            target.DisplayNotification("~g~" + player.character.showName + " te ha entregado " + itemData.nameSingular + " en tu mano izquierda.");
                                            player.SendAme("Entrega su " + itemData.nameSingular + " a " + target.character.showName);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~El jugador tiene el inventario lleno.");
                                        }
                                    }
                                    else
                                    {
                                        if (targetInventory.rightHand == null && targetInventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(target.user)) == null)
                                        {
                                            if (targetInventory.AddItem(sourceItem, (int)Global.InventoryBodypart.LeftHand) == 1)
                                            {
                                                playerInventory.RemoveItem(sourceItem);
                                                player.DisplayNotification("~g~Entregas tu " + itemData.nameSingular + " a " + target.character.showName + " en su mano derecha.");
                                                target.DisplayNotification("~g~" + player.character.showName + " te ha entregado " + itemData.nameSingular + " en tu mano derecha.");
                                                player.SendAme("Entrega su " + itemData.nameSingular + " a " + target.character.showName);
                                            }
                                            else
                                            {
                                                player.DisplayNotification("~r~El jugador tiene el inventario lleno.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~r~El jugador está lejos de ti.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~El jugador está cambiando de personaje.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~El jugador desconectó.");
                }
            }
        }

        /// <summary>
        /// Opens the player looting menu for a player
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="target">The target player instance</param>
        public static void OpenPlayerLootingMenu(Player player, Player target)
        {
            Inventory playerInventory = player.character.inventory;
            Inventory targetInventory = target.character.inventory;

            if(target != null)
            {
                if(target.character != null)
                {
                    if (target.user.Position.DistanceTo(player.user.Position) <= Global.ExternalInteractionPlayerDistance && !target.user.IsInVehicle && player.user.Dimension == target.user.Dimension)
                    {
                        Menu menu = new Menu("", target.character.showName, true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnPlayerLootingMenuResponse));
                        menu.miscData = new List<uint>();

                        menu.menuModel.items.Add(new MenuItemModel("Ver dinero"));
                        menu.miscData.Add(Global.InventoryLootingOptions.GetMoney);

                        menu.menuModel.items.Add(new MenuItemModel("Coger dinero"));
                        menu.miscData.Add(Global.InventoryLootingOptions.RobMoney);


                        if (targetInventory.rightHand != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.rightHand.GetOverviewText(), "Mano derecha"));
                            menu.miscData.Add(Global.InventoryLootingOptions.RightHand);
                        }

                        if (targetInventory.leftHand != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.leftHand.GetOverviewText(), "Mano izquierda"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Lefthand);
                        }

                        if (targetInventory.lightWeapon1 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.lightWeapon1.GetOverviewText(), "Arma ligera"));
                            menu.miscData.Add(Global.InventoryLootingOptions.LightWeapon1);
                        }

                        if (targetInventory.lightWeapon2 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.lightWeapon2.GetOverviewText(), "Arma ligera"));
                            menu.miscData.Add(Global.InventoryLootingOptions.LightWeapon2);
                        }

                        if (targetInventory.meleeLightWeapon1 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.meleeLightWeapon1.GetOverviewText(), "Arma melee ligera"));
                            menu.miscData.Add(Global.InventoryLootingOptions.LightMeleeWeapon1);
                        }

                        if (targetInventory.meleeLightWeapon2 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.meleeLightWeapon2.GetOverviewText(), "Arma melee ligera"));
                            menu.miscData.Add(Global.InventoryLootingOptions.LightMeleeWeapon2);
                        }

                        if (targetInventory.heavyWeapon != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.heavyWeapon.GetOverviewText(), "Arma pesada"));
                            menu.miscData.Add(Global.InventoryLootingOptions.HeavyWeapon);
                        }

                        if (targetInventory.heavyWeapon != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.meleeHeavyWeapon.GetOverviewText(), "Arma melee pesada"));
                            menu.miscData.Add(Global.InventoryLootingOptions.HeavyMeleeWeapon);
                        }

                        if (targetInventory.throwableWeapon1 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.throwableWeapon1.GetOverviewText(), "Arma arrojadiza"));
                            menu.miscData.Add(Global.InventoryLootingOptions.ThrowableWeapon1);
                        }

                        if (targetInventory.throwableWeapon2 != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.throwableWeapon2.GetOverviewText(), "Arma arrojadiza"));
                            menu.miscData.Add(Global.InventoryLootingOptions.ThrowableWeapon2);
                        }

                        if (targetInventory.specialWeapon != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.specialWeapon.GetOverviewText(), "Arma especial"));
                            menu.miscData.Add(Global.InventoryLootingOptions.SpecialWeapon);
                        }

                        if (targetInventory.bodyarmor != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.bodyarmor.GetOverviewText(), "Chaleco antibalas"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Bodyarmor);
                        }

                        if (targetInventory.backpack != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.backpack.GetOverviewText(), "Mochila"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Backpack);
                        }

                        if (targetInventory.gloves != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.gloves.GetOverviewText(), "Guantes"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Gloves);
                        }

                        if (targetInventory.hat != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.hat.GetOverviewText(), "Gorro/Casco"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Hat);
                        }

                        if (targetInventory.glasses != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.glasses.GetOverviewText(), "Gafas"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Glasses);
                        }

                        if (targetInventory.mask != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.mask.GetOverviewText(), "Mascara"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Mask);
                        }

                        if (targetInventory.accessory != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.accessory.GetOverviewText(), "Accesorio"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Accessory);
                        }

                        if (targetInventory.ears != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.ears.GetOverviewText(), "Orejas"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Ears);
                        }

                        if (targetInventory.torso != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.torso.GetOverviewText(), "Ropa torso"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Torso);
                        }

                        if (targetInventory.legs != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.legs.GetOverviewText(), "Pantalones"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Legs);
                        }

                        if (targetInventory.feet != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.feet.GetOverviewText(), "Zapatos"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Feet);
                        }

                        if (targetInventory.watch != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.watch.GetOverviewText(), "Reloj"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Watch);
                        }

                        if (targetInventory.bracelet != null)
                        {
                            menu.menuModel.items.Add(new MenuItemModel(targetInventory.bracelet.GetOverviewText(), "Brazalete"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Bracelet);
                        }

                        if (targetInventory.items.Count() > 0)
                        {
                            menu.menuModel.items.Add(new MenuItemModel("Revisar bolsillos", "Muestra el contenido de los bolsillos"));
                            menu.miscData.Add(Global.InventoryLootingOptions.Pockets);
                        }

                        menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));
                        menu.miscData.Add(Global.InventoryLootingOptions.Back);

                        player.lootingPlayer = target;

                        GuiController.CreateMenu(player, menu);
                    }
                    else
                    {
                        player.DisplayNotification("~r~Jugador demasiado lejos.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~El jugador está cambiando de personaje.");
                }
            }
            else
            {
                player.DisplayNotification("~r~El jugador desconectó.");
            }

        }

        /// <summary>
        /// Receives the player looting menu resposes
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The option as string</param>
        /// <param name="action">The action id</param>
        public static void OnPlayerLootingMenuResponse(Player player, string option, int actionId)
        {
            Inventory playerInventory = player.character.inventory;

            if(player.lootingPlayer != null)
            {
                if(player.lootingPlayer.character != null)
                {
                    if (player.lootingPlayer.user.Position.DistanceTo(player.user.Position) <= Global.ExternalInteractionPlayerDistance && !player.lootingPlayer.user.IsInVehicle && player.user.Dimension == player.lootingPlayer.user.Dimension)
                    {
                        Inventory targetInventory = player.lootingPlayer.character.inventory;

                        try
                        {
                            int action = player.menu.miscData.ElementAt(actionId);
                            Item item = null;
                            bool refresh = true;
                            switch (action)
                            {
                                case (int)Global.InventoryLootingOptions.GetMoney:
                                    {
                                        if (player.lootingPlayer.character.money > 0)
                                        { 
                                            player.DisplayNotification("~g~Cuentas en total " + player.lootingPlayer.character.money + "$ de los bolsillos de " + player.lootingPlayer.character.showName);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~No tiene ni un centavo.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.RobMoney:
                                    {
                                        if (player.lootingPlayer.character.money > 0)
                                        {
                                            if (player.character.money <= Global.PocketMoneyLimit)
                                            {
                                                player.SetMoney(player.character.money + player.lootingPlayer.character.money);
                                                player.lootingPlayer.SetMoney(0);
                                                player.DisplayNotification("~g~Coges " + player.lootingPlayer.character.money + "$ de " + player.lootingPlayer.character.showName);
                                                player.SendAme("Coge algo de " + player.lootingPlayer.character.showName);
                                            }
                                            else
                                            {
                                                player.DisplayNotification("~r~No puedes llevar más dinero encima, se notaría demasiado.");
                                            }
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~No tiene ni un centavo.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.RightHand:
                                    {
                                        if (targetInventory.rightHand != null)
                                        {
                                            item = targetInventory.rightHand;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Lefthand:
                                    {
                                        if (targetInventory.leftHand != null)
                                        {
                                            item = targetInventory.leftHand;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.LightWeapon1:
                                    {
                                        if (targetInventory.lightWeapon1 != null)
                                        {
                                            item = targetInventory.lightWeapon1;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.LightWeapon2:
                                    {
                                        if (targetInventory.lightWeapon2 != null)
                                        {
                                            item = targetInventory.lightWeapon2;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.LightMeleeWeapon1:
                                    {
                                        if (targetInventory.meleeLightWeapon1 != null)
                                        {
                                            item = targetInventory.meleeLightWeapon1;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.LightMeleeWeapon2:
                                    {
                                        if (targetInventory.meleeLightWeapon2 != null)
                                        {
                                            item = targetInventory.meleeLightWeapon2;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.HeavyWeapon:
                                    {
                                        if (targetInventory.heavyWeapon != null)
                                        {
                                            item = targetInventory.heavyWeapon;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.HeavyMeleeWeapon:
                                    {
                                        if (targetInventory.meleeHeavyWeapon != null)
                                        {
                                            item = targetInventory.meleeHeavyWeapon;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.ThrowableWeapon1:
                                    {
                                        if (targetInventory.throwableWeapon1 != null)
                                        {
                                            item = targetInventory.throwableWeapon1;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.ThrowableWeapon2:
                                    {
                                        if (targetInventory.throwableWeapon2 != null)
                                        {
                                            item = targetInventory.throwableWeapon2;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.SpecialWeapon:
                                    {
                                        if (targetInventory.specialWeapon != null)
                                        {
                                            item = targetInventory.specialWeapon;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Bodyarmor:
                                    {
                                        if (targetInventory.bodyarmor != null)
                                        {
                                            item = targetInventory.bodyarmor;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Backpack:
                                    {
                                        if (targetInventory.backpack != null)
                                        {
                                            item = targetInventory.backpack;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Gloves:
                                    {
                                        if (targetInventory.gloves != null)
                                        {
                                            item = targetInventory.gloves;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Hat:
                                    {
                                        if (targetInventory.hat != null)
                                        {
                                            item = targetInventory.hat;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Glasses:
                                    {
                                        if (targetInventory.glasses != null)
                                        {
                                            item = targetInventory.glasses;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Mask:
                                    {
                                        if (targetInventory.mask != null)
                                        {
                                            item = targetInventory.mask;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Accessory:
                                    {
                                        if (targetInventory.accessory != null)
                                        {
                                            item = targetInventory.accessory;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Ears:
                                    {
                                        if (targetInventory.ears != null)
                                        {
                                            item = targetInventory.ears;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Torso:
                                    {
                                        if (targetInventory.torso != null)
                                        {
                                            item = targetInventory.torso;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Legs:
                                    {
                                        if (targetInventory.legs != null)
                                        {
                                            item = targetInventory.legs;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Feet:
                                    {
                                        if (targetInventory.feet != null)
                                        {
                                            item = targetInventory.feet;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Watch:
                                    {
                                        if (targetInventory.watch != null)
                                        {
                                            item = targetInventory.watch;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Bracelet:
                                    {
                                        if (targetInventory.bracelet != null)
                                        {
                                            item = targetInventory.bracelet;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya no tiene nada aqui.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Pockets:
                                    {
                                        if (targetInventory.items.Count() > 0)
                                        {
                                            OpenInventoryForPlayer(player, targetInventory, "Bolsillos de " + player.lootingPlayer.character.showName);
                                            refresh = false;
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~No tiene nada en los bolsillos.");
                                        }
                                        break;
                                    }
                                case (int)Global.InventoryLootingOptions.Back:
                                    {
                                        SelfInteraction.GenerateMenu(player);
                                        refresh = false;
                                        return;
                                    }
                            }

                            if (action != (int)Global.InventoryLootingOptions.GetMoney && action != (int)Global.InventoryLootingOptions.RobMoney)
                            {
                                if (item != null)
                                {
                                    if (playerInventory.AddItem(item) == 1)
                                    {
                                        ItemData itemData = ItemData.GetById(item.id);
                                        targetInventory.RemoveItem(item);
                                        player.DisplayNotification("~g~Coges " + itemData.nameSingular + " de " + player.lootingPlayer.character.showName);
                                        player.SendAme("Coge algo de " + player.lootingPlayer.character.showName);
                                    }
                                    else
                                    {
                                        player.DisplayNotification("~r~No tienes espacio en el inventario.");
                                    }
                                }
                                else
                                {
                                    player.DisplayNotification("~r~No tiene nada aqui.");
                                }
                            }

                            if (refresh)
                            {
                                OpenPlayerLootingMenu(player, player.lootingPlayer);
                            }

                        }
                        catch (Exception e)
                        {
                            Log.Debug(e.Message);
                            Log.Debug(e.StackTrace);
                            player.DisplayNotification("~r~No tiene nada aqui.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Jugador demasiado lejos.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~El jugador está cambiando de personaje.");
                }
            }
            else
            {
                player.DisplayNotification("~r~El jugador desconectó.");
            }
            
        }

        // COMMANDS

        #region Commands

        /// <summary>
        /// Throw item to the floor (right hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("tirar")]
        public void Tirar(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                ThrowItemToGround(player, true);
            }
        }

        /// <summary>
        /// Throw item to the floor (left hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("tirari")]
        public void Tirari(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                ThrowItemToGround(player, false);
            }
        }

        /// <summary>
        /// Save item to the inventory pockets (right hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("guardar")]
        public void Guardar(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                SaveItemToSelfInventory(player, true);
            }
        }

        /// <summary>
        /// Save item to the inventory pockets (left hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("guardari")]
        public void Guardari(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                SaveItemToSelfInventory(player, false);
            }
        }

        /// <summary>
        /// Takes an item from the pockets to the hand (right hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        /// <param name="pocketId">Pocket id</param>
        [Command("sacar", "~y~USO: /sacar (ID) - Sacas un item de tu inventario.")]
        public void Sacar(Client sender, int pocketId)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                TakeItemFromPocketToHand(player, true, pocketId);
            }
        }

        /// <summary>
        /// Takes an item from the pockets to the hand (left hand)
        /// </summary>
        /// <param name="sender">The client instance</param>
        /// <param name="pocketId">Pocket id</param>
        [Command("sacari", "~y~USO: /sacari (ID) - Sacas un item de tu inventario.")]
        public void SacarI(Client sender, int pocketId)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                TakeItemFromPocketToHand(player, false, pocketId);
            }
        }

        /// <summary>
        /// Takes the closest world item
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("recoger")]
        public void Recoger(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                TakeClosestWorldItem(player);
            }
        }

        /// <summary>
        /// Change the item hand
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("mano")]
        public void Mano(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if(player.character.inventory.rightHand != null)
                {
                    ChangeItemHand(player, (int)Global.InventoryBodypart.RightHand);
                }
                else
                {
                    if(player.character.inventory.leftHand != null)
                    {
                        ChangeItemHand(player, (int)Global.InventoryBodypart.LeftHand);
                    }
                }
            }
        }

        /// <summary>
        /// Use the item on the right hand
        /// </summary>
        /// <param name="sender">The client instance</param>
        [Command("usar")]
        public void Usar(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                UseItemForPlayer(player);
            }
        }

        /// <summary>
        /// Give the right hand item to another player
        /// </summary>
        /// <param name="sender">The client instance</param>
        /// <param name="target">Another player</param>
        [Command("ceder", "~y~USO: /ceder (ID) - Entregas el item de tu mano derecha a alguien.")]
        public void Ceder(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                Player targetPlayer = Player.GetByIdOrName(target);

                if (targetPlayer != null)
                {
                    if(targetPlayer.character != null)
                    {
                        GivePlayerItemToPlayerHand(player, targetPlayer, true);
                    }
                    else
                    {
                        player.DisplayNotification("~r~El jugador todavia no ha aparecido.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~Jugador desconectado.");
                }
            }
        }

        /// <summary>
        /// Give the left hand item to another player
        /// </summary>
        /// <param name="sender">The client instance</param>
        /// <param name="target">Another player</param>
        [Command("cederi", "~y~USO: /cederi (ID) - Entregas el item de tu mano izquierda a alguien.")]
        public void Cederi(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                Player targetPlayer = Player.GetByIdOrName(target);

                if (targetPlayer != null)
                {
                    if (targetPlayer.character != null)
                    {
                        GivePlayerItemToPlayerHand(player, targetPlayer, false);
                    }
                    else
                    {
                        player.DisplayNotification("~r~El jugador todavia no ha aparecido.");
                    }
                }
                else
                {
                    player.DisplayNotification("~r~Jugador desconectado.");
                }
            }
        }


        #endregion

    }

}
