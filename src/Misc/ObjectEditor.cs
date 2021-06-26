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
    /// Object editor system
    /// </summary>
    public class ObjectEditor : Script
    {


        /// <summary>
        /// Editor modes
        /// </summary>
        public enum Mode
        {
            AttachedItem,
            Furniture,
            Barrier
        }

        /// <summary>
        /// Editor components
        /// </summary>
        public enum Component
        {
            XOffset,
            YOffset,
            ZOffset,
            XRotation,
            YRotation,
            ZRotation
        }

        /// <summary>
        /// Triggered when the edited object components change
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="offset">The object offset</param>
        /// <param name="rotation">The object rotation</param>
        /// <param name="position">The object position</param>
        [RemoteEvent("onObjectEditorUpdate")]
        public void OnObjectEditorUpdate(Client user, Vector3 offset, Vector3 rotation, Vector3 position)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.objectEditorMode == (int)Mode.AttachedItem)
                {
                    ItemData itemData = ItemData.GetById(player.objectEditorItem.id);
                    switch (player.objectEditorAttachedPart)
                    {
                        case (int)Global.InventoryBodypart.RightHand:
                            {
                                itemData.rightHandOffset = offset;
                                itemData.rightHandRotation = rotation;
                                player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.RightHand);
                                player.character.inventory.SetPropModel((int)Global.InventoryBodypart.RightHand);
                                break;
                            }
                        case (int)Global.InventoryBodypart.LeftHand:
                            {
                                itemData.leftHandOffset = offset;
                                itemData.leftHandRotation = rotation;
                                player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.LeftHand);
                                player.character.inventory.SetPropModel((int)Global.InventoryBodypart.LeftHand);
                                break;
                            }
                        case (int)Global.InventoryBodypart.HeavyWeapon:
                            {
                                itemData.chestOffset = offset;
                                itemData.chestRotation = rotation;
                                player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);
                                player.character.inventory.SetPropModel((int)Global.InventoryBodypart.HeavyWeapon);
                                break;
                            }
                        case (int)Global.InventoryBodypart.MeleeWeapon:
                            {
                                itemData.backOffset = offset;
                                itemData.backRotation = rotation;
                                player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);
                                player.character.inventory.SetPropModel((int)Global.InventoryBodypart.MeleeWeapon);
                                break;
                            }
                    }
                }
                else
                {
                    NAPI.Entity.SetEntityPosition(player.objectEditorHandle, position);
                    NAPI.Entity.SetEntityRotation(player.objectEditorHandle, rotation);
                }
            }
        }

        /// <summary>
        /// Initializes the editor for a player with the given mode and arguments
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="mode">The editor mode</param>
        /// <param name="args">Other arguments</param>
        public static void StartEditor(Player player, int mode, params object[] args)
        {
            switch (mode)
            {
                // If attached item
                case (int)Mode.AttachedItem:
                    {
                        // Set the default data
                        player.objectEditorActive = true;
                        string title = "Editor de objetos";
                        player.objectEditorMode = mode;
                        player.objectEditorComponent = (int)Component.XOffset;

                        player.objectEditorAttachedPart = int.Parse(args[0].ToString());
                        player.objectEditorBone = Global.PlayerBodypartBones[player.objectEditorAttachedPart];

                        // Player inventory
                        Inventory playerInventory = player.character.inventory;

                        Item item;
                        NetHandle handle;

                        switch(player.objectEditorAttachedPart)
                        {
                            case (int)Global.InventoryBodypart.RightHand:
                                {
                                    item = playerInventory.rightHand;
                                    handle = playerInventory.rightHandProp;

                                    ItemData itemData = ItemData.GetById(item.id);

                                    player.objectEditorOriginalOffset = itemData.rightHandOffset;
                                    player.objectEditorOriginalRotation = itemData.rightHandRotation;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.LeftHand:
                                {
                                    item = playerInventory.leftHand;
                                    handle = playerInventory.leftHandProp;

                                    ItemData itemData = ItemData.GetById(item.id);

                                    player.objectEditorOriginalOffset = itemData.leftHandOffset;
                                    player.objectEditorOriginalRotation = itemData.leftHandRotation;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.HeavyWeapon:
                                {
                                    item = playerInventory.heavyWeapon;
                                    handle = playerInventory.heavyWeaponProp;

                                    ItemData itemData = ItemData.GetById(item.id);

                                    player.objectEditorOriginalOffset = itemData.chestOffset;
                                    player.objectEditorOriginalRotation = itemData.chestRotation;
                                    break;
                                }
                            case (int)Global.InventoryBodypart.MeleeWeapon:
                                {
                                    item = playerInventory.meleeHeavyWeapon;
                                    handle = playerInventory.meleeHeavyWeaponProp;

                                    ItemData itemData = ItemData.GetById(item.id);

                                    player.objectEditorOriginalOffset = itemData.backOffset;
                                    player.objectEditorOriginalRotation = itemData.backRotation;

                                    break;
                                }
                            default:
                                {
                                    item = playerInventory.rightHand;
                                    handle = playerInventory.rightHandProp;

                                    ItemData itemData = ItemData.GetById(item.id);

                                    player.objectEditorOriginalOffset = itemData.rightHandOffset;
                                    player.objectEditorOriginalRotation = itemData.rightHandRotation;

                                    break;
                                }
                        }

                        player.objectEditorItem = item;
                        player.objectEditorHandle = handle;

                        player.objectEditorOriginalPosition = NAPI.Entity.GetEntityPosition(handle);

                        NAPI.ClientEvent.TriggerClientEvent(player.user, "objectEditorInitialization", player.objectEditorActive, title, player.objectEditorMode, player.objectEditorComponent, player.objectEditorOriginalOffset, player.objectEditorOriginalRotation, player.objectEditorOriginalPosition);
                        
                        break;
                    }
                case (int)Mode.Furniture:
                    {

                        break;
                    }
                case (int)Mode.Barrier:
                    {

                        break;
                    }
            }
        }

        /// <summary>
        /// Commits the editing changes
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="offset">The object offset</param>
        /// <param name="rotation">The object rotation</param>
        /// <param name="position">The object position</param>
        public static void CommitChanges(Player player, Vector3 offset, Vector3 rotation, Vector3 position)
        {
            switch (player.objectEditorMode)
            {
                case (int)Mode.AttachedItem:
                    {
                        ItemData itemData = ItemData.GetById(player.objectEditorItem.id);
                        switch (player.objectEditorAttachedPart)
                        {
                            case (int)Global.InventoryBodypart.RightHand:
                                {
                                    itemData.rightHandOffset = offset;
                                    itemData.rightHandRotation = rotation;

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
                                                tempCmd.CommandText = "UPDATE Items SET rightHandXOffset = @rightHandXOffset " +
                                                                    "rightHandYOffset = @rightHandYOffset " +
                                                                    "rightHandZOffset = @rightHandZOffset " +
                                                                    "rightHandXRotation = @rightHandXRotation " +
                                                                    "rightHandYRotation = @rightHandYRotation " +
                                                                    "rightHandZRotation = @rightHandZRotation " +
                                                                    "WHERE id = @id";
                                                tempCmd.Parameters.AddWithValue("@rightHandXOffset", itemData.rightHandOffset.X);
                                                tempCmd.Parameters.AddWithValue("@rightHandYOffset", itemData.rightHandOffset.Y);
                                                tempCmd.Parameters.AddWithValue("@rightHandZOffset", itemData.rightHandOffset.Z);
                                                tempCmd.Parameters.AddWithValue("@rightHandXRotation", itemData.rightHandRotation.X);
                                                tempCmd.Parameters.AddWithValue("@rightHandYRotation", itemData.rightHandRotation.Y);
                                                tempCmd.Parameters.AddWithValue("@rightHandZRotation", itemData.rightHandRotation.Z);
                                                tempCmd.Parameters.AddWithValue("@id", itemData.sqlid);

                                                if (tempCmd.ExecuteNonQuery() == 0)
                                                {
                                                    Log.Debug("[OBJECT EDITOR] Error al editar objeto en mano derecha: " + itemData.sqlid);
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug(e.Message);
                                        Log.Debug(e.StackTrace);
                                    }

                                    break;
                                }
                            case (int)Global.InventoryBodypart.LeftHand:
                                {
                                    itemData.leftHandOffset = offset;
                                    itemData.leftHandRotation = rotation;

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
                                                tempCmd.CommandText = "UPDATE Items SET leftHandXOffset = @leftHandXOffset " +
                                                                    "leftHandYOffset = @leftHandYOffset " +
                                                                    "leftHandZOffset = @leftHandZOffset " +
                                                                    "leftHandXRotation = @leftHandXRotation " +
                                                                    "leftHandYRotation = @leftHandYRotation " +
                                                                    "leftHandZRotation = @leftHandZRotation " +
                                                                    "WHERE id = @id";
                                                tempCmd.Parameters.AddWithValue("@leftHandXOffset", itemData.leftHandOffset.X);
                                                tempCmd.Parameters.AddWithValue("@leftHandYOffset", itemData.leftHandOffset.Y);
                                                tempCmd.Parameters.AddWithValue("@leftHandZOffset", itemData.leftHandOffset.Z);
                                                tempCmd.Parameters.AddWithValue("@leftHandXRotation", itemData.leftHandRotation.X);
                                                tempCmd.Parameters.AddWithValue("@leftHandYRotation", itemData.leftHandRotation.Y);
                                                tempCmd.Parameters.AddWithValue("@leftHandZRotation", itemData.leftHandRotation.Z);
                                                tempCmd.Parameters.AddWithValue("@id", itemData.sqlid);

                                                if (tempCmd.ExecuteNonQuery() == 0)
                                                {
                                                    Log.Debug("[OBJECT EDITOR] Error al editar objeto en mano izquierda: " + itemData.sqlid);
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug(e.Message);
                                        Log.Debug(e.StackTrace);
                                    }

                                    break;
                                }
                            case (int)Global.InventoryBodypart.HeavyWeapon:
                                {
                                    itemData.chestOffset = offset;
                                    itemData.chestRotation = rotation;

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
                                                tempCmd.CommandText = "UPDATE Items SET chestXOffset = @chestXOffset " +
                                                                    "chestYOffset = @chestYOffset " +
                                                                    "chestZOffset = @chestZOffset " +
                                                                    "chestXRotation = @chestXRotation " +
                                                                    "chestYRotation = @chestYRotation " +
                                                                    "chestZRotation = @chestZRotation " +
                                                                    "WHERE id = @id";
                                                tempCmd.Parameters.AddWithValue("@chestXOffset", itemData.chestOffset.X);
                                                tempCmd.Parameters.AddWithValue("@chestYOffset", itemData.chestOffset.Y);
                                                tempCmd.Parameters.AddWithValue("@chestZOffset", itemData.chestOffset.Z);
                                                tempCmd.Parameters.AddWithValue("@chestXRotation", itemData.chestRotation.X);
                                                tempCmd.Parameters.AddWithValue("@chestYRotation", itemData.chestRotation.Y);
                                                tempCmd.Parameters.AddWithValue("@chestZRotation", itemData.chestRotation.Z);
                                                tempCmd.Parameters.AddWithValue("@id", itemData.sqlid);

                                                if (tempCmd.ExecuteNonQuery() == 0)
                                                {
                                                    Log.Debug("[OBJECT EDITOR] Error al editar objeto en pecho: " + itemData.sqlid);
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug(e.Message);
                                        Log.Debug(e.StackTrace);
                                    }

                                    break;
                                }
                            case (int)Global.InventoryBodypart.MeleeWeapon:
                                {
                                    itemData.backOffset = offset;
                                    itemData.backRotation = rotation;

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
                                                tempCmd.CommandText = "UPDATE Items SET backXOffset = @backXOffset " +
                                                                    "backYOffset = @backYOffset " +
                                                                    "backZOffset = @backZOffset " +
                                                                    "backXRotation = @backXRotation " +
                                                                    "backYRotation = @backYRotation " +
                                                                    "backZRotation = @backZRotation " +
                                                                    "WHERE id = @id";
                                                tempCmd.Parameters.AddWithValue("@backXOffset", itemData.backOffset.X);
                                                tempCmd.Parameters.AddWithValue("@backYOffset", itemData.backOffset.Y);
                                                tempCmd.Parameters.AddWithValue("@backZOffset", itemData.backOffset.Z);
                                                tempCmd.Parameters.AddWithValue("@backXRotation", itemData.backRotation.X);
                                                tempCmd.Parameters.AddWithValue("@backYRotation", itemData.backRotation.Y);
                                                tempCmd.Parameters.AddWithValue("@backZRotation", itemData.backRotation.Z);
                                                tempCmd.Parameters.AddWithValue("@id", itemData.sqlid);

                                                if (tempCmd.ExecuteNonQuery() == 0)
                                                {
                                                    Log.Debug("[OBJECT EDITOR] Error al editar objeto en espalda: " + itemData.sqlid);
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        Log.Debug(e.Message);
                                        Log.Debug(e.StackTrace);
                                    }

                                    break;
                                }
                        }

                        player.DisplayNotification("~g~Posición editada correctamente.");

                        break;
                    }
            }

            player.objectEditorActive = false;
            player.objectEditorMode = -1;
            player.objectEditorComponent = -1;
            player.objectEditorOriginalOffset = null;
            player.objectEditorOriginalRotation = null;
            player.objectEditorOriginalPosition = null;
            player.objectEditorAttachedPart = -1;
            player.objectEditorBone = "";
            player.objectEditorItem = null;

        }

        /// <summary>
        /// Cancels the editing changes
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void CancelChanges(Player player)
        {
            if (player.objectEditorMode == (int)Mode.AttachedItem)
            {
                ItemData itemData = ItemData.GetById(player.objectEditorItem.id);
                switch (player.objectEditorAttachedPart)
                {
                    case (int)Global.InventoryBodypart.RightHand:
                        {
                            itemData.rightHandOffset = player.objectEditorOriginalOffset;
                            itemData.rightHandRotation = player.objectEditorOriginalRotation;
                            break;
                        }
                    case (int)Global.InventoryBodypart.LeftHand:
                        {
                            itemData.leftHandOffset = player.objectEditorOriginalOffset;
                            itemData.leftHandRotation = player.objectEditorOriginalRotation;
                            break;
                        }
                    case (int)Global.InventoryBodypart.HeavyWeapon:
                        {
                            itemData.chestOffset = player.objectEditorOriginalOffset;
                            itemData.chestRotation = player.objectEditorOriginalRotation;
                            break;
                        }
                    case (int)Global.InventoryBodypart.MeleeWeapon:
                        {
                            itemData.backOffset = player.objectEditorOriginalOffset;
                            itemData.backRotation = player.objectEditorOriginalRotation;
                            break;
                        }
                }
            }
            else
            {
                NAPI.Entity.SetEntityPosition(player.objectEditorHandle, player.objectEditorOriginalPosition);
                NAPI.Entity.SetEntityRotation(player.objectEditorHandle, player.objectEditorOriginalRotation);
            }

            player.objectEditorActive = false;
            player.objectEditorMode = -1;
            player.objectEditorComponent = -1;
            player.objectEditorOriginalOffset = null;
            player.objectEditorOriginalRotation = null;
            player.objectEditorOriginalPosition = null;
            player.objectEditorAttachedPart = -1;
            player.objectEditorBone = "";
            player.objectEditorItem = null;

        }


    }

}
