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
using System.Threading;
using System.Drawing;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

#pragma warning disable CS0168

namespace redrp
{
    /// <summary>
    /// Player base class that contains character management, initialization, events, and other related systems and commands
    /// </summary>
    public class Player : Script
    {
        /// <summary>
        /// Global player list, contains all the player instances authenticated correctly
        /// </summary>
        public static List<Player> Players = new List<Player>();

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////ATTRIBUTES//////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // PLAYER ATTRIBUTES

        /// <summary>
        /// Client instance
        /// </summary>
        public Client user { get; set; }

        /// <summary>
        /// Ingame player ID
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// Player unique identifier (database Id)
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Player account name (not characters name)
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Player account admin level
        /// </summary>
        public int admin { get; set; }

        /// <summary>
        /// Player current active character instance
        /// </summary>
        public Character character { get; set; }

        /// <summary>
        /// GUI system control variable
        /// </summary>
        public int showingGui { get; set; }

        /// <summary>
        /// GUI system interface identifier
        /// </summary>
        public int showingGuiId { get; set; }

        /// <summary>
        /// Target player for payments
        /// </summary>
        public Player payDialogTarget { get; set; }

        /// <summary>
        /// Player beacon instance (for admins)
        /// </summary>
        public Blip playerBeacon { get; set; }

        /// <summary>
        /// Controls if player is in character selection process or not
        /// </summary>
        public bool isInCharacterSelection { get; set; }

        /// <summary>
        /// Controls if player has a forced animation or not
        /// </summary>
        public bool forcedAnimation { get; set; }

        /// <summary>
        /// Controls if player has a forced clipset or not
        /// </summary>
        public bool forcedClipset { get; set; }

        /// <summary>
        /// Current player menu
        /// </summary>
        public Menu menu { get; set; }

        /// <summary>
        /// Current player dialog
        /// </summary>
        public Dialog dialog { get; set; }

        /// <summary>
        /// External interaction target instance (player, vehicle, etc)
        /// </summary>
        public object externalInteractionTarget { get; set; }

        /// <summary>
        /// Deprecated skin system variable (to be deleted)
        /// </summary>
        public int skinSelectorType { get; set; }

        /// <summary>
        /// Deprecated skin system variable (to be deleted)
        /// </summary>
        public List<string> skinSelector { get; set; }

        /// <summary>
        /// Autokick control variable
        /// </summary>
        public int autokick { get; set; }

        /// <summary>
        /// Current camera Id
        /// </summary>
        public uint cameraID { get; set; }

        /// <summary>
        /// If player is in admin duty or not
        /// </summary>
        public bool adminDuty { get; set; }

        /// <summary>
        /// If player has teleported with the system or not (anticheat)
        /// </summary>
        public bool teleport { get; set; }

        /// <summary>
        /// Deactivates weapon cheat detection for a number of seconds
        /// </summary>
        public int disableWeaponAnticheatSeconds { get; set; }

        /// <summary>
        /// Player has just removed a weapon or not
        /// </summary>
        public bool justRemovedWeapon { get; set; }

        /// <summary>
        /// Current AFK registered position
        /// </summary>
        public Vector3 afkPosition { get; set; }

        /// <summary>
        /// AFK acumulated time
        /// </summary>
        public int afkTime { get; set; }

        /// <summary>
        /// Number of ping warnings
        /// </summary>
        public int pingWarns { get; set; }

        /// <summary>
        /// Current private admin channel
        /// </summary>
        public int adminChannel { get; set; }

        /// <summary>
        /// Player being looted
        /// </summary>
        public Player lootingPlayer { get; set; }

        // OBJECT EDITOR

        /// <summary>
        /// If object editor is active
        /// </summary>
        public bool objectEditorActive { get; set; }

        /// <summary>
        /// Current object editor mode
        /// </summary>
        public int objectEditorMode { get; set; }

        /// <summary>
        /// Current object editor selected component
        /// </summary>
        public int objectEditorComponent { get; set; }

        /// <summary>
        /// Original object position
        /// </summary>
        public Vector3 objectEditorOriginalPosition { get; set; }

        /// <summary>
        /// Original object rotation
        /// </summary>
        public Vector3 objectEditorOriginalRotation { get; set; }

        /// <summary>
        /// Original object offset from bone
        /// </summary>
        public Vector3 objectEditorOriginalOffset { get; set; }

        /// <summary>
        /// Object editor item instance
        /// </summary>
        public Item objectEditorItem { get; set; }

        /// <summary>
        /// Original object position
        /// </summary>
        public NetHandle objectEditorHandle { get; set; }

        /// <summary>
        /// Object editor attached part
        /// </summary>
        public int objectEditorAttachedPart { get; set; }

        /// <summary>
        /// Object editor current attached bone
        /// </summary>
        public string objectEditorBone { get; set; }

        /// <summary>
        /// Player is in crouch mode or not
        /// </summary>
        public bool crouchMode { get; set; }

        /// <summary>
        /// Player animation timer
        /// </summary>
        public Timer animationTimer { get; set; }

        /// <summary>
        /// Player animation
        /// </summary>
        public Animation currentAnimation { get; set; }

        /// <summary>
        /// Experience types
        /// </summary>
        public enum ExperienceTypes
        {
            Security,
            Mechanic,
            Gta,
            Criminal,
            Transport,
            Taxi,
            Fishing
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////EVENTHANDLERS/////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Performs a player login attempt
        /// </summary>
        /// <param name="user">The player client instance</param>
        /// <param name="nameOrEmail">The player account name or email</param>
        /// <param name="password">The player account password</param>
        [RemoteEvent("onLoginAttempt")]
        public void OnLoginAttempt(Client user, string nameOrEmail, string password)
        {
            int loginResult = Login(user, nameOrEmail, password);

            if (loginResult > 0)
            {

                // Now we show the character list to the recent logged player
                Player loggedPlayer = Exists(user);
                if (loggedPlayer != null)
                {
                    Log.Debug("Player instance: " + loggedPlayer.name);
                    loggedPlayer.SwitchToCharacterList();
                }

            }
            else
            {
                // If there was an error during login, we show the correct error message.
                string errorMessage = "";
                switch (loginResult)
                {
                    case 0:
                        {
                            errorMessage = "Ha habido un error en el servidor.";
                            break;
                        }
                    case -1:
                        {
                            errorMessage = "La cuenta no existe.";
                            break;
                        }
                    case -2:
                        {
                            int loginAttempts = NAPI.Data.GetEntityData(user, "PLAYER_LOGIN_ATTEMPTS") - 1;
                            if (loginAttempts > 0)
                            {
                                errorMessage = "Contraseña incorrecta. Te quedan " + loginAttempts + " intentos.";
                                NAPI.Data.SetEntityData(user, "PLAYER_LOGIN_ATTEMPTS", loginAttempts);
                            }
                            else
                            {
                                errorMessage = "";
                                NAPI.Player.KickPlayer(user, "Intentos agotados");
                            }
                            break;
                        }
                    case -3:
                        {
                            errorMessage = "Hay alguien jugando con ésta misma cuenta...";
                            break;
                        }
                    case -4:
                        {
                            errorMessage = "La cuenta no está activada.";
                            break;
                        }
                    case -5:
                        {
                            errorMessage = "La cuenta está bloqueada temporalmente.";
                            break;
                        }
                    case -6:
                        {
                            errorMessage = "La cuenta está bloqueada permanentemente.";
                            break;
                        }
                }

                NAPI.ClientEvent.TriggerClientEvent(user, "showLoginErrorMessage", errorMessage);
            }
        }

        /// <summary>
        /// Performs an attempt to spawn a character
        /// </summary>
        /// <param name="user">The player client instance</param>
        /// <param name="characterId">The character DB identifier</param>
        /// <param name="toHome">If it will spawn on the character's home or on the last known position</param>
        [RemoteEvent("onCharacterSpawnAttempt")]
        public void OnCharacterSpawnAttempt(Client user, int characterId, bool toHome)
        {
            Player player = Exists(user);
            if (player != null)
            {
                player.CharacterPreInit(characterId, toHome);
            }
        }

        /// <summary>
        /// Triggered when player joins the server
        /// </summary>
        /// <param name="user">The Client instance of the player</param>
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client user)
        {
            // Initialize some temporary data (like autokick)
            NAPI.Data.SetEntityData(user, "PLAYER_AUTOKICK", 0);

            // Set the login camera
            NAPI.ClientEvent.TriggerClientEvent(user, "setLoginCamera");

            // Freeze the player
            NAPI.Player.FreezePlayer(user, true);

            // If server is open
            if (Global.ServerStatus == 1)
            {
                if (Whitelist.Check(user.SocialClubName))
                {
                    // Autokick to 30 seconds
                    NAPI.Data.SetEntityData(user, "PLAYER_AUTOKICK", Global.LoginAutokickTime);

                    // Login attempts tickets
                    NAPI.Data.SetEntityData(user, "PLAYER_LOGIN_ATTEMPTS", 5);

                    // Call client to show login page
                    NAPI.ClientEvent.TriggerClientEvent(user, "showLoginInterface", "login");
                }
                else
                {
                    // If player is not whitelisted
                    NAPI.Chat.SendChatMessageToPlayer(user, "~r~No estás en la whitelist.");
                    NAPI.Data.SetEntityData(user, "PLAYER_AUTOKICK", 5);
                }
            }
            else
            {
                // If server is closed
                NAPI.Chat.SendChatMessageToPlayer(user, "~r~El servidor está cerrado, contacta con un administrador.");
                NAPI.Data.SetEntityData(user, "PLAYER_AUTOKICK", 5);
            }
        }

        /// <summary>
        /// Triggered when a player disconnects from the server
        /// </summary>
        /// <param name="user">The Client instance of the player</param>
        /// <param name="type">The disconnection type</param>
        /// <param name="reason">The disconnection explicit reason</param>
        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client user, DisconnectionType type, string reason)
        {
            Player player = Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    try
                    {
                        string customReason;
                        switch (type)
                        {
                            case DisconnectionType.Left: customReason = "desconexión voluntaria."; break;
                            case DisconnectionType.Timeout: customReason = "ha perdido la conexión."; break;
                            case DisconnectionType.Kicked: customReason = "ha sido expulsado."; break;
                            default: customReason = "desconexión forzosa."; break;
                        }
                         
                        // Warn nearby players
                        if (player.character.hiddenIdentity)
                        {
                            Util.SendChatMessage3D(player.character.showName + " se ha desconectado. Razón: " + customReason, player.user.Position, 30f, player.user.Dimension);
                        }
                        else
                        {
                            Util.SendChatMessage3D(player.name + " - " + player.character.showName + "(" + player.id + ")" + " ha desconectado. Razón: " + customReason, player.user.Position, 30f, player.user.Dimension);
                        }

                        // Deletes the player beacon on disconnect.
                        if (player.playerBeacon != null)
                        {
                            NAPI.Entity.DeleteEntity(player.playerBeacon);
                        }

                        // Clear all GUI
                        if (player.showingGui != -1)
                        {
                            GuiController.ClearGui(player);
                        }

                        // Close admin-user private chat if open
                        if (player.adminChannel == player.id)
                        {
                            foreach (Player target in Players)
                            {
                                if (target.adminChannel == player.id)
                                {
                                    target.adminChannel = -1;
                                    if (target.id != player.id)
                                    {
                                        NAPI.Chat.SendChatMessageToPlayer(target.user, "~y~" + player.name + " ha cerrado su canal admin (desconectado).");
                                    }
                                }
                            }
                        }

                        //Cancel object editor
                        if (player.objectEditorActive)
                        {
                            ObjectEditor.CancelChanges(player);
                        }

                        // Remove attached props
                        player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.RightHand);
                        player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.LeftHand);
                        player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);
                        player.character.inventory.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);

                        // Save inventory
                        player.character.inventory.Save();
                        Global.Inventories.Remove(player.character.inventory);

                        Log.Debug("[Desconexión] El jugador " + player.name + " se ha desconectado del servidor. Razón: " + customReason);

                        // Save character data
                        player.SaveCharacter();
                    }
                    catch (Exception e)
                    {
                        Log.Debug("ERROR AL DESCONECTAR.");
                        Log.Debug(e.Message);
                        Log.Debug(e.StackTrace);
                        player.Delete();
                    }
                }

                // Delete player instance
                player.Delete();
            }
        }

        /// <summary>
        /// Triggered when a player dies
        /// </summary>
        /// <param name="victim">The victim Client instance</param>
        /// <param name="killer">The killer Client instance (null if is not a murder)</param>
        /// <param name="reason">The reason id</param>
        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client victim, Client killer, uint reason)
        {
            Player player = Exists(victim);
            if (player != null)
            {

                //We disable the default death system and force player to resurrect
                /*
                NAPI.Native.SendNativeToPlayer(player.user, Hash._RESET_LOCALPLAYER_STATE, player.user);
                NAPI.Native.SendNativeToPlayer(player.user, Hash.RESET_PLAYER_ARREST_STATE, player.user);

                NAPI.Native.SendNativeToPlayer(player.user, Hash.IGNORE_NEXT_RESTART, true);
                NAPI.Native.SendNativeToPlayer(player.user, Hash._DISABLE_AUTOMATIC_RESPAWN, true);

                NAPI.Native.SendNativeToPlayer(player.user, Hash.SET_FADE_IN_AFTER_DEATH_ARREST, true);
                NAPI.Native.SendNativeToPlayer(player.user, Hash.SET_FADE_OUT_AFTER_DEATH, false);
                NAPI.Native.SendNativeToPlayer(player.user, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, player.user);

                //NAPI.sendNativeToPlayer(player.user, Hash.FREEZE_ENTITY_POSITION, player.user, false);
                NAPI.Native.SendNativeToPlayer(player.user, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, player.user.Position.X, player.user.Position.Y, player.user.Position.Z, player.user.Rotation.Z, false, false);
                NAPI.Native.SendNativeToPlayer(player.user, Hash.RESURRECT_PED, player.user);
                */

                // Apply the dead state.
                player.SetDead();

            }
        }

        /// <summary>
        /// Triggered when a player receives damage
        /// </summary>
        /// <param name="user">The player Client instance</param>
        /// <param name="healthLoss">The health that has been lost</param>
        /// <param name="armorLoss">The armor that has been lost</param>
        [ServerEvent(Event.PlayerDamage)]
        public void OnPlayerDamage(Client user, float healthLoss, float armorLoss)
        { 
            Player player = Exists(user);
            if (player != null)
            {
                int currentHealth = player.user.Health;
                int currentArmor = player.user.Armor;
                if (healthLoss > 0)
                {
                    player.character.health = currentHealth;
                }
                else if(healthLoss < 0)
                {
                    if (currentHealth > player.character.health && player.admin == 0)
                    {
                        Admin.Notification("[ATUNBOT] Detectada posible trampa de vida de " + player.name + " (" + player.character.cleanName + ") " + player.sqlid);
                        Admin.Notification("[ATUNBOT] Puntos de vida anteriores: " + player.character.health + " | Puntos de vida actuales: " + currentHealth);
                        NAPI.Player.SetPlayerHealth(player.user, player.character.health);
                    }
                }

                if(armorLoss > 0)
                {
                    if (player.character.inventory.bodyarmor != null)
                    {
                        player.character.inventory.bodyarmor.quantity = currentArmor;
                    }
                }
                else if(armorLoss < 0)
                {
                    if (currentArmor > player.character.inventory.bodyarmor.quantity)
                    {
                        Admin.Notification("[ATUNBOT] Detectada posible trampa de armadura de " + player.name + " (" + player.character.cleanName + ") " + player.sqlid);
                        Admin.Notification("[ATUNBOT] Puntos de armadura del chaleco: " + player.character.inventory.bodyarmor.quantity + " | Puntos de armadura reales: " + currentArmor);
                        NAPI.Player.SetPlayerArmor(player.user, player.character.inventory.bodyarmor.quantity);
                    }
                }
                
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////MAIN METHODS//////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
        /// <summary>
        /// Triggered every 500 miliseconds for every player instance
        /// Controls weapon cheats and update item models
        /// </summary>
        public void HalfSecond()
        {
            if (this.character != null)
            {
                if (this.character.spawned)
                {
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Weapon control
                    if (this.character.inventory != null)
                    {
                        if (this.disableWeaponAnticheatSeconds == 0)
                        {
                            WeaponHash[] hashes = NAPI.Player.GetPlayerWeapons(this.user);

                            // Weapon anticheat
                            for(int i=0; i<hashes.Length; i++)
                            {
                                try
                                {
                                    WeaponHash hash = hashes[i];
                                    if (hash != WeaponHash.Unarmed && hash != WeaponHash.Parachute)
                                    {
                                        Item weapon = this.character.inventory.GetWeaponItem(hash);
                                        if (weapon != null)
                                        {
                                            int currentAmmo = NAPI.Player.GetPlayerWeaponAmmo(this.user, hash);
                                            int ammoDiff = currentAmmo - weapon.quantity;
                                            if (ammoDiff <= 0)
                                            {
                                                ItemData itemData = ItemData.GetById(weapon.id);
                                                if (ammoDiff < 0)
                                                {
                                                    if (itemData.type == (int)Global.ItemType.ThrowableWeapon)
                                                    {
                                                        this.character.inventory.RemoveItem(weapon);
                                                    }
                                                    else
                                                    {
                                                        weapon.quantity = currentAmmo;
                                                        this.character.inventory.save = true;
                                                    }
                                                }

                                                // Weapon prop models
                                                if (itemData.type == (int)Global.ItemType.FireWeapon && itemData.isHeavy)
                                                {
                                                    if (NAPI.Player.GetPlayerCurrentWeapon(this.user) != NAPI.Util.WeaponNameToModel(itemData.weaponModel))
                                                    {
                                                        if (this.character.inventory.heavyWeaponProp != null)
                                                            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);

                                                        this.character.inventory.SetPropModel((int)Global.InventoryBodypart.HeavyWeapon);
                                                    }
                                                    else
                                                    {
                                                        if (this.character.inventory.heavyWeaponProp != null)
                                                            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);
                                                    }
                                                }

                                                // Melee weapon prop model
                                                if (itemData.type == (int)Global.ItemType.MeleeWeapon && itemData.isHeavy)
                                                {
                                                    if (NAPI.Player.GetPlayerCurrentWeapon(this.user) != NAPI.Util.WeaponNameToModel(itemData.weaponModel))
                                                    {
                                                        if (this.character.inventory.meleeHeavyWeapon != null)
                                                            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);

                                                        this.character.inventory.SetPropModel((int)Global.InventoryBodypart.MeleeWeapon);
                                                    }
                                                    else
                                                    {
                                                        if (this.character.inventory.meleeHeavyWeapon != null)
                                                            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);
                                                    }
                                                }

                                            }
                                            else
                                            {
                                                Admin.Notification("[ATUNBOT] Detectado cambio brusco de munición de " + this.name + " (" + this.character.cleanName + "), ID: " + this.id);
                                                Admin.Notification("[ATUNBOT] Arma: " + hash + " | Munición anterior: " + weapon.quantity + " | Munición actual: " + currentAmmo + " | Diferencia: " + ammoDiff);
                                            }
                                        }
                                        else
                                        {
                                            if (!this.justRemovedWeapon)
                                            {
                                                Admin.Notification("[ATUNBOT] Detectada arma no registrada de " + this.name + " (" + this.character.cleanName + "), ID: " + this.id);
                                                Admin.Notification("[ATUNBOT] Arma: " + hash + " | Munición: " + NAPI.Player.GetPlayerWeaponAmmo(this.user, hash));
                                            }

                                            NAPI.Player.RemovePlayerWeapon(this.user, hash);
                                            this.justRemovedWeapon = false;
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    break;
                                }
                            }

                            if (this.character.inventory.rightHand != null && NAPI.Player.GetPlayerCurrentWeapon(this.user) != WeaponHash.Unarmed)
                            {
                                Item item = this.character.inventory.rightHand;
                                this.character.inventory.RemoveItem(item);
                                if (item.IsHeavy())
                                {
                                    ItemData itemData = ItemData.GetById(item.id);
                                    WorldItem.Create(item, this.GetFootPos(), itemData.worldRotation, this.user.Dimension);
                                }
                                else
                                {
                                    if (this.character.inventory.AddItem(item) != 1)
                                    {
                                        ItemData itemData = ItemData.GetById(item.id);
                                        WorldItem.Create(item, this.GetFootPos(), itemData.worldRotation, this.user.Dimension);
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.disableWeaponAnticheatSeconds--;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Triggered every second for every player instance
        /// </summary>
        public void OneSecond()
        {
            if (this.character != null)
            {
                if(this.character.spawned)
                {
                    // Anti-teleport system
                    this.character.position = this.user.Position;
                    if (this.character.position.DistanceTo(this.user.Position) > 200.0)
                    {
                        if (!this.teleport)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(this.user, "~r~ Pillado por trampa de teletransporte.");
                            Admin.Notification("[ATUNBOT] Detectada posible trampa de teletransporte de " + this.name + " (" + this.character.cleanName + ") " + this.sqlid);
                        }
                    }

                    this.teleport = false;

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Anti flood
                    if (this.character.antiFlood > 0)
                    {
                        this.character.antiFlood--;
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Ame message counter
                    if (this.character.ameCounter > 0)
                    {
                        this.character.ameCounter--;
                        if (this.character.ameCounter == 0)
                        {
                            NAPI.Entity.DeleteEntity(this.character.ameTextLabel);
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Death system counter
                    if (this.character.dyingCounter > 0)
                    {
                        this.character.dyingCounter--;
                        if (this.character.dyingCounter == 0)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(this.user, "~g~Ya puedes reaparecer desde el menú de interacción.");
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Anti AFK system
                    this.CheckAFK();

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    // Deal system counter
                    if (this.character.dealRemainingTime > 0)
                    {
                        this.character.dealRemainingTime--;
                        if (this.character.dealRemainingTime == 0)
                        {
                            this.RefuseDeal();
                        }
                    }

                    // Deal relative distance check
                    if (this.character.dealRemainingTime > 0)
                    {
                        if(this.user.Position.DistanceTo(this.character.dealOwner.user.Position) > Global.DealMaxDistance)
                        {
                            this.RefuseDeal();
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                }
            }
        }

        /// <summary>
        /// Triggered every minute on every player instance
        /// </summary>
        public void OneMinute()
        {
            if (this.character != null)
            {
                // Anti Ping system
                this.CheckPing();

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                // Payday trigger
                this.character.paydayTime++;
                if (this.character.paydayTime == 60)
                {
                    this.character.paydayTime = 0;
                    this.Payday();
                }

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }

        /// <summary>
        /// Registers a new player
        /// </summary>
        /// <param name="socialClub"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="adminLevel"></param>
        /// <returns></returns>
        public static bool Register(string socialClub, string name, string password, int adminLevel = 0)
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
                        tempCmd.CommandText = "INSERT INTO redrp_player (playerName, socialClub, password, adminLevel) " +
                                                "VALUES (@playerName, @socialClub, @password, @adminLevel)";
                        tempCmd.Parameters.AddWithValue("@playerName", name);
                        tempCmd.Parameters.AddWithValue("@socialClub", socialClub);
                        tempCmd.Parameters.AddWithValue("@password", Util.ComputeSha256Hash(password));
                        tempCmd.Parameters.AddWithValue("@adminLevel", adminLevel);

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            Log.Debug("[PlayerController] Registered new player " + name + ", social club: " + socialClub);

                            return true;
                        }
                        else
                        {
                            Log.Debug("[PlayerController] Can't register new player " + name + ", social club: " + socialClub);
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
        /// Logs player in using player instance and provided password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns>Returns the player sqlid in case of success, or next error codes:
        /// -6 in case of perm ban
        /// -5 in case of temp ban
        /// -4 in case of account not active
        /// -3 in case of duplicate login
        /// -2 in case of incorrect password
        /// -1 in case of account not Exists
        /// 0 in case of exception
        /// </returns>
        public static int Login(Client user, string name, string password)
        {
            try
            {
                int code = 1;

                List<string> results;

                // We instantiate a new disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // If success, we instantiate a new disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Sets the command query text
                        tempCmd.CommandText = "SELECT * FROM redrp_player WHERE playerName = @name AND socialClub = @socialClub LIMIT 1";
                        tempCmd.Parameters.AddWithValue("@name", name);
                        tempCmd.Parameters.AddWithValue("@socialClub", user.SocialClubName);

                        // Then we create the mysql reader disposable object
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {

                            // Read the result
                            if (tempReader.Read())
                            {

                                // If Exists we get encrypted password
                                string passwordReceived = tempReader.GetString("password");

                                if (Util.ComputeSha256Hash(passwordReceived).Equals(password))
                                {

                                    // Get basic data
                                    int sqlid = tempReader.GetInt32("id");
                                    int admin = tempReader.GetInt32("admin");
                                    int activated = tempReader.GetInt32("active");
                                    int banStatus = tempReader.GetInt32("banStatus");
                                    long banDate = tempReader.GetInt32("banDate");
                                    int banDuration = tempReader.GetInt32("banDuration");

                                    //Check if the player is already logged in
                                    if (GetPlayerBySqlid(sqlid) == null)
                                    {
                                        // If account is activated
                                        if (activated == 1)
                                        {
                                            // If there is any active ban
                                            if (banStatus != 0)
                                            {
                                                // It's perm
                                                if (banDuration == -1)
                                                {
                                                    code = -6;
                                                }
                                                else // It's temp.
                                                {
                                                    int banSeconds = banDuration * 3600;
                                                    long currentDate = Util.GetCurrentTimestamp();
                                                    long timePassed = currentDate - banDate;

                                                    if (timePassed >= banSeconds)
                                                    {
                                                        // Unban the player and let him continue
                                                        Unban(sqlid);

                                                        // Player initialization
                                                        code = InitializePlayer(user, sqlid, name, admin);
                                                    }
                                                    else
                                                    {
                                                        code = -5;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Player initialization
                                                code = InitializePlayer(user, sqlid, name, admin);
                                            }
                                        }
                                        else
                                        {
                                            code = -4;
                                        }

                                    }
                                    else
                                    {
                                        code = -3;
                                    }
                                }
                                else
                                {
                                    code = -2;
                                }
                            }
                            else
                            {
                                code = -1;
                            }

                            return code;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// Initializes player instance.
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="playerData"></param>
        /// <returns></returns>
        public static int InitializePlayer(Client user, int sqlid, string name, int adminLevel)
        {
            try
            {
                // If all goes well we instantiate a new player and initialize his basic data
                Player newPlayer = new Player();

                newPlayer.user = user;
                newPlayer.id = user.Value;
                newPlayer.sqlid = sqlid;
                newPlayer.name = name;
                newPlayer.admin = adminLevel;
                newPlayer.character = null;

                // Non-persistant data
                newPlayer.showingGui = -1;
                newPlayer.showingGuiId = -1;
                newPlayer.autokick = -1;
                newPlayer.adminDuty = false;
                newPlayer.teleport = false;
                newPlayer.disableWeaponAnticheatSeconds = 0;
                newPlayer.justRemovedWeapon = false;
                newPlayer.forcedAnimation = false;
                newPlayer.forcedClipset = false;
                newPlayer.externalInteractionTarget = null;
                newPlayer.afkTime = 0;
                newPlayer.pingWarns = 0;
                newPlayer.afkPosition = null;
                newPlayer.adminChannel = -1;
                newPlayer.lootingPlayer = null;
                newPlayer.crouchMode = false;

                // Object editor data
                newPlayer.objectEditorActive = false;

                // We add the instance to the main pool
                Players.Add(newPlayer);

                //Log connection
                //int actualTimestamp = Utils.GetCurrentTimestamp();
                //DB.query(false, "INSERT INTO logs_conexiones (tipo, idelemento, nombre, ip, fecha) VALUES (1, " + newPlayer.sqlid + ", '" + newPlayer.name + "', '-', " + actualTimestamp + ")");

                return newPlayer.sqlid;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// Returns a list of character names in JSON
        /// </summary>
        /// <returns></returns>
        public string GetCharacterList()
        {
            try
            {
                List<string> characterList = new List<string>();
                // We instantiate a new disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // If success, we instantiate a new disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Sets the command query text
                        tempCmd.CommandText = "SELECT id, characterName FROM redrp_character WHERE playerId = @id";
                        tempCmd.Parameters.AddWithValue("@id", this.sqlid);

                        // Then we create the mysql reader disposable object
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {

                            // We read all the result 
                            while (tempReader.Read())
                            {
                                characterList.Add(tempReader[0].ToString() + "," + tempReader[1].ToString());
                            }
                        }
                    }
                }

                //Now we return the character list in JSON format
                return NAPI.Util.ToJson(characterList.ToArray());
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Send the player to the character selection interface
        /// </summary>
        public void ToCharacterSelection()
        {
            // Warn nearby players
            if (this.character.hiddenIdentity)
            {
                Util.SendChatMessage3D(this.character.showName + " está cambiando de personaje.", this.user.Position, 20f, this.user.Dimension);
            }
            else
            {
                Util.SendChatMessage3D(this.name + " (" + this.character.showName + ") está cambiando de personaje.", this.user.Position, 20f, this.user.Dimension);
            }

            // Deletes the player beacon.
            if (this.playerBeacon != null)
            {
                NAPI.Entity.DeleteEntity(this.playerBeacon);
            }

            // Clear all GUI
            if (this.showingGui != -1)
            {
                GuiController.ClearGui(this);
            }

            // Close admin-user private chat if open
            if (this.adminChannel == this.id)
            {
                foreach (Player target in Players)
                {
                    if (target.adminChannel == this.id)
                    {
                        target.adminChannel = -1;
                        if (target.id != this.id)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(target.user, "~y~" + this.name + " ha cerrado su canal admin (desconectado).");
                        }
                    }
                }
            }

            // Remove attached props
            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.RightHand);
            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.LeftHand);
            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.HeavyWeapon);
            this.character.inventory.RemovePropModel((int)Global.InventoryBodypart.MeleeWeapon);

            this.character.inventory.Save();

            // Clear player's chat
            this.CleanChat();

            // Saves character data
            this.SaveCharacter();

            // Delete character data
            this.character = null;

            // We initialize autokick
            NAPI.Data.SetEntityData(this.user, "PLAYER_AUTOKICK", Global.LoginAutokickTime);

            // Create login camera
            NAPI.ClientEvent.TriggerClientEvent(this.user, "setLoginCamera");

            // Force another dimension
            NAPI.Entity.SetEntityDimension(this.user, 666);

            // Force player position inside the airport
            NAPI.Entity.SetEntityPosition(this.user, new Vector3(-1037.7, -2737.8, 20.1));

            // Freeze player
            NAPI.Player.FreezePlayer(this.user, true);

            // Show character list
            this.ShowCharacterList();

            this.isInCharacterSelection = true;
        }

        /// <summary>
        /// Shows the character list of a player
        /// </summary>
        public void ShowCharacterList()
        {
            // We get the list of characters of the player
            string charactersData = GetCharacterList();
            if (charactersData != null)
            {
                // Creates the character selection menu
                NAPI.ClientEvent.TriggerClientEvent(user, "showLoginInterface", charactersData);
                // Hides the hud
                NAPI.ClientEvent.TriggerClientEvent(user, "hudHide");
            }
            else
            {
                NAPI.Chat.SendChatMessageToPlayer(user, "~r~Error al generar la lista de personajes.");
            }
        }

        /// <summary>
        /// Shows the character list of a player directly from the login page
        /// </summary>
        public void SwitchToCharacterList()
        {
            // We get the list of characters of the player
            string charactersData = GetCharacterList();
            if (charactersData != null)
            {
                // Creates the character selection menu
                NAPI.ClientEvent.TriggerClientEvent(user, "switchToCharacterSelectorLoginInterface", charactersData);
            }
            else
            {
                NAPI.Chat.SendChatMessageToPlayer(user, "~r~Error al generar la lista de personajes.");
            }
        }

        /// <summary>
        /// Hide the character list of a player
        /// </summary>
        public void HideCharacterList()
        {
            // Close character selector
            NAPI.ClientEvent.TriggerClientEvent(user, "hideLoginInterface");
        }

        /// <summary>
        /// Checks and initializes de character
        /// </summary>
        /// <param name="sqlid">The character id</param>
        /// <param name="home"></param>
        public void CharacterPreInit(int sqlid, bool home)
        {
            if (CharacterExists(sqlid))
            {
                Task.Factory.StartNew(() =>
                {
                    LoadCharacter(sqlid, home);
                });
            }
        }

        /// <summary>
        /// Check if the character Exists
        /// </summary>
        /// <param name="sqlid">The character sqlid</param>
        /// <returns></returns>
        public static bool CharacterExists(int sqlid)
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
                        tempCmd.CommandText = "SELECT id FROM redrp_character WHERE id = @sqlid";
                        tempCmd.Parameters.AddWithValue("@sqlid", sqlid);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.Read())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
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
        /// Load character data from sqlid and assign it to the player instance
        /// </summary>
        /// <param name="sqlid">The character sql id</param>
        public void LoadCharacter(int sqlid, bool home)
        {
            try
            {
                // Main character data

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
                        tempCmd.CommandText = "SELECT * FROM redrp_character WHERE id = @sqlid";
                        tempCmd.Parameters.AddWithValue("@sqlid", sqlid);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.Read())
                            {
                                this.character = new Character();
                                this.character.sqlid = tempReader.GetInt32("id");
                                this.character.ownerSqlid = tempReader.GetInt32("playerId");
                                this.character.showName = this.character.sqlid.ToString();
                                this.character.cleanName = tempReader.GetString("characterName");
                                this.character.name = tempReader.GetString("characterName").Replace(' ', '_');
                                this.character.playedHours = tempReader.GetInt32("playTime");
                                this.character.logins = tempReader.GetInt32("logins");
                                this.character.sex = tempReader.GetInt32("sex");
                                this.character.age = tempReader.GetInt32("age");
                                this.character.position = new Vector3(tempReader.GetDouble("x"), tempReader.GetDouble("y"), tempReader.GetDouble("z"));
                                this.character.heading = tempReader.GetFloat("heading");
                                this.character.dimension = tempReader.GetUInt32("dimension");
                                this.character.health = tempReader.GetInt32("health");
                                this.character.dying = tempReader.GetInt32("dying");
                                this.character.money = tempReader.GetInt32("cash");
                                this.character.paydayTime = tempReader.GetInt32("paydayTime");
                                this.character.cuffed = tempReader.GetInt32("cuffed");
                                this.character.walkingStyle = tempReader.GetString("walkingAnimation");
                                this.character.mood = tempReader.GetInt32("mood");
                            }
                        }
                    }
                }

                // Non persistant data initialization

                this.character.hiddenIdentity = false;
                this.character.spawned = false;
                this.character.mp = true; // MP enabled by default
                this.character.antiFlood = 0;
                this.character.activeLanguage = null; // Null = English (default)
                this.character.ameCounter = 0;
                this.character.voiceType = 0; // Default voice is normal channel
                this.character.dyingCounter = 0; // Dying counter
                this.character.usingATM = null;
                this.character.usingAccountATM = null;
                this.character.usingPinATM = -1;
                this.character.authenticatedATM = false;
                this.character.operationATM = -1;
                this.character.operationDataATM = null;
                this.character.dealOwner = null;
                this.character.dealType = -1;
                this.character.dealRemainingTime = 0;
                this.character.dealDescription = "";
                this.character.otherInventory = null;
                this.character.nearWorldItems = null;
                this.character.rightOpenedContainer = null;
                this.character.leftOpenedContainer = null;

                // Experience

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
                        tempCmd.CommandText = "SELECT * FROM redrp_character_experience WHERE characterId = @sqlid";
                        tempCmd.Parameters.AddWithValue("@sqlid", sqlid);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.Read())
                            { 
                                this.character.securityExp = tempReader.GetInt32("security");
                                this.character.mechanicExp = tempReader.GetInt32("mechanic");
                                this.character.grandTheftAutoExp = tempReader.GetInt32("vehicleTheft");
                                this.character.criminalExp = tempReader.GetInt32("criminal");
                                this.character.transportistExp = tempReader.GetInt32("delivery");
                                this.character.taxistExp = tempReader.GetInt32("taxi");
                                this.character.fishingExp = tempReader.GetInt32("fish");
                            }
                        }
                    }
                }

                // Bank accounts

                this.character.bankAccounts = new List<BankAccount>();
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
                        tempCmd.CommandText = "SELECT id FROM redrp_bankaccount WHERE ownerId = @sqlid AND ownerType = 0";
                        tempCmd.Parameters.AddWithValue("@sqlid", sqlid);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            while (tempReader.HasRows)
                            {
                                tempReader.Read();
                                this.character.bankAccounts.Add(BankAccount.Load(id));
                            }
                        }
                    }
                }

                // Increment login count
                this.character.logins++;
                Util.UpdateDBField("redrp_character", "logins", this.character.logins.ToString(), "id", this.character.sqlid.ToString());

                // Log connection
                // int actualTimestamp = Utils.GetCurrentTimestamp();
                //DB.query(false, "INSERT INTO logs_conexiones (tipo, idelemento, nombre, ip, fecha) VALUES (2, " + this.character.sqlid + ", '" + this.character.cleanName + "', '-'," + actualTimestamp + ")");

                this.CharacterPostInit(home);
            }
            catch (Exception e)
            {
                NAPI.ClientEvent.TriggerClientEvent(user, "showErrorLoginInterface", "Error al cargar el personaje.");
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// Finalize character initialization
        /// </summary>
        /// <param name="sqlid">The character id</param>
        /// <param name="home"></param>
        public void CharacterPostInit(bool home)
        {
            Log.Debug("Personaje cargado");
            this.HideCharacterList();

            this.CharacterSpawn(home);
            Log.Debug("Personaje spawn");
        }

        /// <summary>
        /// Forces character to spawn
        /// </summary>
        /// <param name="home">If spawn in the first owned house</param>
        public void CharacterSpawn(bool home)
        {
            // Start loading prompt
            this.ShowLoadingPrompt("Cargando personaje...", 4);

            // Normal dimension
            this.character.dimension = 1;
            NAPI.Entity.SetEntityDimension(this.user, this.character.dimension);

            // UnFreeze player
            NAPI.Player.FreezePlayer(this.user, false);

            // Deactivate autokick
            NAPI.Data.SetEntityData(this.user, "PLAYER_AUTOKICK", 0);

            // Set nametag
            this.SetNormalNametag();

            // Cleans the player chat
            this.CleanChat();

            // Set money indicator
            this.SetMoney(this.character.money);

            // Set language indicator
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetLanguage", "Inglés");

            // Set voice indicator
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetVoiceType", "Hablar");

            if (this.adminDuty)
            {
                this.SetAdminDuty(false);
            }

            this.isInCharacterSelection = false;

            // If it's the first time to spawn
            if (this.character.logins == 1)
            {
                this.CharacterSpawnFirstTime();
            }
            else
            {
                // If it's not the first time, we put the player on the last known position
                if (this.character.position.X != 0 && this.character.position.Y != 0 && this.character.position.Z != 0)
                {
                    this.CharacterSpawnNormal();
                }
                else
                {
                    this.CharacterSpawnError();
                }
            }

            // Now we put the correct player model
            if(this.character.sex == 0)
            {
                NAPI.Player.SetPlayerSkin(this.user, PedHash.FreemodeMale01);
            }
            else
            {
                NAPI.Player.SetPlayerSkin(this.user, PedHash.FreemodeFemale01);
            }

            // Shared data
            NAPI.Data.SetEntitySharedData(this.user, "playerSex", this.character.sex);

            // Set mood
            this.SetMood(this.character.mood);

            // Set movement clipset
            this.SetMovementClipset(this.character.walkingStyle);

            // Gameplay camera
            NAPI.ClientEvent.TriggerClientEvent(this.user, "setGameplayCamera");

            // Shows hud
            NAPI.ClientEvent.TriggerClientEvent(user, "hudShow");

            // Mostramos los mensajes de bienvenida.
            NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~Bienvenido a Grand Theft Auto: Roleplay.");
            NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~Si necesitas ayuda pulsa la tecla F2.");

            if (this.admin > 0)
            {
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~g~Formas parte del equipo administrativo.");
            }

            // Inventory loading process
            int inventoryId = int.Parse(Util.GetDBField("redrp_character", "inventory", "id", this.character.sqlid.ToString()));
            if (inventoryId != -1)
            {
                Task.Factory.StartNew(() =>
                {
                    this.character.inventory = Inventory.Load(inventoryId, this);
                });
            }

            this.character.spawned = true;
            this.HideLoadingPrompt();

            if(this.character.cuffed != 0)
            {
                this.SetCuffed(this.character.cuffed, false);
            }

            // If player was in death state we set this state, if not, we set the health value.
            if (this.character.dying == 1)
            {
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~b~Estabas muerto la ultima vez que desconectaste.");
                NAPI.Player.SetPlayerHealth(this.user, -1);
            }
            else
            {
                NAPI.Player.SetPlayerHealth(this.user, this.character.health);
            }
            
        }

        /// <summary>
        /// Forces character to spawn for first time
        /// </summary>
        public void CharacterSpawnFirstTime()
        {
            // Airport entrance
            this.SetPosition(new Vector3(-1037.7, -2737.8, 20.1), new Vector3(this.user.Rotation.X, this.user.Rotation.Y, 324.0));

            // Change the player position and heading
            NAPI.Entity.SetEntityPosition(this.user, this.character.position);
            NAPI.Entity.SetEntityRotation(this.user, new Vector3(this.user.Rotation.X, this.user.Rotation.Y, this.character.heading));

        }

        /// <summary>
        /// Forces character to spawn in normal conditions
        /// </summary>
        public void CharacterSpawnNormal()
        {
            // Change the player position and heading
            this.SetPosition(this.character.position, new Vector3(this.user.Rotation.X, this.user.Rotation.Y, this.character.heading));

        }

        /// <summary>
        /// Forces character to spawn when there is an error (coordinates 0 0 0)
        /// </summary>
        public void CharacterSpawnError()
        {
            // If coordinates are not valid we put the player on default spawn (airport)
            // Airport entrance
            this.SetPosition(new Vector3(-1037.7, -2737.8, 20.1), new Vector3(this.user.Rotation.X, this.user.Rotation.Y, 324.0));

            // Change the player position and heading
            NAPI.Entity.SetEntityPosition(this.user, this.character.position);
            NAPI.Entity.SetEntityRotation(this.user, new Vector3(this.user.Rotation.X, this.user.Rotation.Y, this.character.heading));
        }

        /// <summary>
        /// Save some critical data from the character.
        /// </summary>
        /// <returns></returns>
        public bool SaveCharacter()
        {
            try
            {
                // Update the last known position
                this.character.position = this.user.Position;

                this.character.heading = this.user.Heading;

                // Update the values on the DB
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
                        tempCmd.CommandText = "UPDATE redrp_character SET " +
                            "x = @x, " + 
                            "y = @y, " + 
                            "z = @z, " + 
                            "heading = @heading, " + 
                            "health = @health, " + 
                            "paydayTime = @paydayTime " + 
                            "WHERE id = @id";
                        tempCmd.Parameters.AddWithValue("@x", this.character.position.X);
                        tempCmd.Parameters.AddWithValue("@y", this.character.position.Y);
                        tempCmd.Parameters.AddWithValue("@z", this.character.position.Z);
                        tempCmd.Parameters.AddWithValue("@heading", this.character.heading);
                        tempCmd.Parameters.AddWithValue("@id", this.character.sqlid);

                        if (tempCmd.ExecuteNonQuery() == 0)
                        {
                            Log.Debug("[SAVE CHARACTER] No se ha guardado correctamente el personaje " + this.character.sqlid);
                            return false;
                        }
                        else
                        {
                            Log.Debug("[SAVE CHARACTER] Personaje " + this.character.sqlid + " guardado correctamente.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug("[SAVE CHARACTER] Error al guardar personaje " + this.character.sqlid);
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
            return false;
        }

        /// <summary>
        /// Calculates taxes, salary and others.
        /// </summary>
        public void Payday()
        {
            // Testing message
            NAPI.Chat.SendChatMessageToPlayer(this.user, "[DEBUG] INICIO PAYDAY");

            // Add one played hour
            this.character.playedHours++;
            Util.UpdateDBField("redrp_character", "playTime", this.character.playedHours.ToString(), "id", this.character.sqlid.ToString());

            // Generate interests and maintenance fees over player bank account's
            this.PlayerBankAccountsPaydayCheck();

            NAPI.Chat.SendChatMessageToPlayer(this.user, "[DEBUG] FIN PAYDAY");
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////METHODS/////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Sets the player's hidden identity
        /// </summary>
        /// <param name="toggle"></param>
        public void SetHiddenIdentity(bool toggle)
        {
            if (toggle)
            {
                this.character.showName = "Desconocido " + this.sqlid;
            }
            else
            {
                this.character.showName = this.character.sqlid.ToString();
            }

            if(this.adminDuty)
            {
                this.SetAdminNametag();
            }
            else
            {
                this.SetNormalNametag();
            }

            this.character.hiddenIdentity = toggle;
        }

        /// <summary>
        /// Updates name tag using character name and player ID.
        /// </summary>
        public void SetNormalNametag()
        {
            NAPI.Player.ResetPlayerNametag(this.user);
            NAPI.Player.ResetPlayerNametagColor(this.user);
            NAPI.Player.SetPlayerNametag(this.user, this.character.showName + " (" + this.id + ")");
            NAPI.Player.SetPlayerNametagColor(this.user, 255, 255, 255);
        }

        /// <summary>
        /// Updates name tag using character name and player ID and admin format.
        /// </summary>
        public void SetAdminNametag()
        {
            NAPI.Player.ResetPlayerNametagColor(this.user);
            //We determine the type of the admin based on his rank
            switch (this.admin)
            {
                case 1:
                    {
                        NAPI.Player.SetPlayerNametagColor(this.user, 75, 8, 138);
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        NAPI.Player.SetPlayerNametagColor(this.user, 11, 97, 11);
                        break;
                    }
                default:
                    {
                        NAPI.Player.SetPlayerNametagColor(this.user, 180, 4, 4);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the player in the admin service status
        /// </summary>
        /// <param name="toggle"></param>
        public void SetAdminDuty(bool toggle)
        {
            if (toggle)
            {
                // First we change the players nametag
                this.SetAdminNametag();

                /*
                int colour = 1;
                string rank = "";
                //We determine the type of the admin based on his rank
                switch (this.admin)
                {
                    case 1:
                        {
                            colour = 58;
                            rank = "Colaborador";
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                        {
                            colour = 52;
                            rank = "Game Master";
                            break;
                        }
                    default:
                        {
                            colour = 1;
                            rank = "Administrador";
                            break;
                        }
                }
                */

                //We attach a blip to the player
                /*
                this.playerBeacon = NAPI.createBlip(this.user);
                NAPI.setBlipSprite(this.playerBeacon, 76);
                NAPI.setBlipColor(this.playerBeacon, colour);
                NAPI.setBlipShortRange(this.playerBeacon, false);
                NAPI.setBlipName(this.playerBeacon, rank + " " + this.name);
                */

                // Make the player invincible
                NAPI.Entity.SetEntityInvincible(this.user, true);

                // Change admin service status
                this.adminDuty = true;

                // Update hud indicator
                NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetAdminDuty", "Servicio Admin");

                this.DisplayNotification("~g~Activado servicio admin");
            }
            else
            {
                // First we change the players nametag
                this.SetNormalNametag();

                //We delete the admin blip
                //NAPI.deleteEntity(this.playerBeacon);

                // Make the player vincible again
                NAPI.Entity.SetEntityInvincible(this.user, false);

                // Change admin service status
                this.adminDuty = false;

                // Update hud indicator
                NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetAdminDuty", "");

                this.DisplayNotification("~g~Desactivado servicio admin");
            }
        }

        /// <summary>
        /// Bans the player from the server
        /// </summary>
        /// <param name="admin">The admin player instance</param>
        /// <param name="duration">The ban duration in seconds, -1 for permanent global ban, 0 for permanent account ban, > 0 for temporary ban.</param>
        /// <param name="reason">The reason</param>
        public void Ban(Player admin, int duration, string reason)
        {
            try
            {
                long currentTimestamp = Util.GetCurrentTimestamp();
                string banType, banDuration;

                int adminId;
                string adminName;
                if(admin == null)
                {
                    adminId = -1;
                    adminName = "SERVIDOR";
                }
                else
                {
                    adminId = admin.sqlid;
                    adminName = admin.name;
                }

                switch (duration)
                {
                    // Permanent account ban with un-whitelisting (this ban is only for players that won't be allowed to play in future).
                    case -1:
                        {
                            string socialClub = Util.GetDBField("redrp_player", "socialClub", "id", this.sqlid.ToString());

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
                                    tempCmd.CommandText = "UPDATE redrp_player SET banStatus = @banStatus, banReason = @banReason WHERE id = @id";
                                    tempCmd.Parameters.AddWithValue("@banStatus", -1);
                                    tempCmd.Parameters.AddWithValue("@banReason", reason);
                                    tempCmd.Parameters.AddWithValue("@id", this.sqlid);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BAN] Error al bloquear globalmente el jugador " + this.sqlid);
                                        return;
                                    }
                                }
                            }

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
                                    tempCmd.CommandText = "INSERT INTO redrp_banlog VALUES (@playerId, @playerName, @adminId, @adminName, @reason, @timestamp, @duration, @active)";
                                    tempCmd.Parameters.AddWithValue("@playerId", this.sqlid);
                                    tempCmd.Parameters.AddWithValue("@playerName", this.name);
                                    tempCmd.Parameters.AddWithValue("@adminId", adminId);
                                    tempCmd.Parameters.AddWithValue("@adminName", adminName);
                                    tempCmd.Parameters.AddWithValue("@reason", reason);
                                    tempCmd.Parameters.AddWithValue("@timestamp", currentTimestamp);
                                    tempCmd.Parameters.AddWithValue("@duration", -1);
                                    tempCmd.Parameters.AddWithValue("@active", 1);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BANLOG] Error al insertar bloqueo global en el log de bloqueos para el jugador " + this.sqlid);
                                    }
                                }
                            }

                            // Un-whitelist
                            Whitelist.Remove(socialClub);

                            banType = "Permanente global";
                            banDuration = "-";
                            break;
                        }
                    // Permanent account ban
                    case 0:
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
                                    tempCmd.CommandText = "UPDATE redrp_player SET banStatus = @banStatus, banReason = @banReason WHERE id = @id";
                                    tempCmd.Parameters.AddWithValue("@banStatus", 0);
                                    tempCmd.Parameters.AddWithValue("@banReason", reason);
                                    tempCmd.Parameters.AddWithValue("@id", this.sqlid);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BAN] Error al bloquear permanentemente el jugador " + this.sqlid);
                                        return;
                                    }
                                }
                            }

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
                                    tempCmd.CommandText = "INSERT INTO redrp_banlog VALUES (@playerId, @playerName, @adminId, @adminName, @reason, @timestamp, @duration, @active)";
                                    tempCmd.Parameters.AddWithValue("@playerId", this.sqlid);
                                    tempCmd.Parameters.AddWithValue("@playerName", this.name);
                                    tempCmd.Parameters.AddWithValue("@adminId", adminId);
                                    tempCmd.Parameters.AddWithValue("@adminName", adminName);
                                    tempCmd.Parameters.AddWithValue("@reason", reason);
                                    tempCmd.Parameters.AddWithValue("@timestamp", currentTimestamp);
                                    tempCmd.Parameters.AddWithValue("@duration", 0);
                                    tempCmd.Parameters.AddWithValue("@active", 1);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BANLOG] Error al insertar bloqueo permanente en el log de bloqueos para el jugador " + this.sqlid);
                                    }
                                }
                            }

                            banType = "Permanente";
                            banDuration = "-";
                            break;
                        }
                    // Temporary account ban
                    default:
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
                                    tempCmd.CommandText = "UPDATE redrp_player SET banStatus = @banStatus, banReason = @banReason, banDate = @banTimestamp, banDuration = @banDuration WHERE id = @id";
                                    tempCmd.Parameters.AddWithValue("@banStatus", 1);
                                    tempCmd.Parameters.AddWithValue("@banReason", reason);
                                    tempCmd.Parameters.AddWithValue("@banTimestamp", currentTimestamp);
                                    tempCmd.Parameters.AddWithValue("@banDuration", duration);
                                    tempCmd.Parameters.AddWithValue("@id", this.sqlid);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BAN] Error al bloquear permanentemente el jugador " + this.sqlid);
                                        return;
                                    }
                                }
                            }

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
                                    tempCmd.CommandText = "INSERT INTO redrp_banlog VALUES (@playerId, @playerName, @adminId, @adminName, @reason, @timestamp, @duration, @active)";
                                    tempCmd.Parameters.AddWithValue("@playerId", this.sqlid);
                                    tempCmd.Parameters.AddWithValue("@playerName", this.name);
                                    tempCmd.Parameters.AddWithValue("@adminId", adminId);
                                    tempCmd.Parameters.AddWithValue("@adminName", adminName);
                                    tempCmd.Parameters.AddWithValue("@reason", reason);
                                    tempCmd.Parameters.AddWithValue("@timestamp", currentTimestamp);
                                    tempCmd.Parameters.AddWithValue("@duration", duration / 3600);
                                    tempCmd.Parameters.AddWithValue("@active", 1);

                                    if (tempCmd.ExecuteNonQuery() == 0)
                                    {
                                        Log.Debug("[BANLOG] Error al insertar bloqueo permanente en el log de bloqueos para el jugador " + this.sqlid);
                                    }
                                }
                            }
                            
                            banType = "Temporal";
                            banDuration = (duration / 3600).ToString() + " horas";
                            break;
                        }
                }

                NAPI.Player.KickPlayer(this.user, "~r~Has sido bloqueado~n~~n~~y~Tipo: " + banType + "~n~Duración: " + banDuration + "~n~Motivo: " + reason + "~n~Puedes presentar una apelación en el foro.");
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// Unbans a player
        /// </summary>
        public static void Unban(int sqlid)
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
                    tempCmd.CommandText = "UPDATE redrp_player SET banStatus = 0, banReason = '', banDate = 0, banDuration = 0 WHERE id = @id";
                    tempCmd.Parameters.AddWithValue("@id", sqlid);

                    if (tempCmd.ExecuteNonQuery() == 0)
                    {
                        Log.Debug("[BANLOG] Error al desbloquear usuario id " + sqlid);
                    }
                }
            }
        }

        /// <summary>
        /// Kicks the player out of the server
        /// </summary>
        /// <param name="admin">The admin player instance</param>
        /// <param name="reason">The reason</param>
        public void Kick(Player admin, string reason)
        {
            try
            {
                long currentTimestamp = Util.GetCurrentTimestamp();

                int adminId;
                string adminName;
                if (admin == null)
                {
                    adminId = -1;
                    adminName = "SERVIDOR";
                }
                else
                {
                    adminId = admin.sqlid;
                    adminName = admin.name;
                }

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
                        tempCmd.CommandText = "INSERT INTO redrp_kicklog VALUES (@playerId, @playerName, @adminId, @adminName, @reason, @timestamp)";
                        tempCmd.Parameters.AddWithValue("@playerId", this.sqlid);
                        tempCmd.Parameters.AddWithValue("@playerName", this.name);
                        tempCmd.Parameters.AddWithValue("@adminId", adminId);
                        tempCmd.Parameters.AddWithValue("@adminName", adminName);
                        tempCmd.Parameters.AddWithValue("@reason", reason);
                        tempCmd.Parameters.AddWithValue("@timestamp", currentTimestamp);

                        if (tempCmd.ExecuteNonQuery() == 0)
                        {
                            Log.Debug("[KICKLOG] Error al insertar en el log de expulsiones: " + this.sqlid);
                        }
                    }
                }

                NAPI.Player.KickPlayer(this.user, reason);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// Updates money value server-side, DB, and updates the hud indicator. Returns 1 if success, 0 if value exceeds maximum and -1 if it's negative.
        /// </summary>
        /// <param name="value">The money value</param>
        /// <returns></returns>
        public int SetMoney(int value)
        {
            int result = 1;
            if (value >= 0)
            {
                if (value <= Global.PocketMoneyLimit)
                {
                    this.character.money = value;

                    Util.UpdateDBField("redrp_character", "money", value.ToString(), "id", this.character.sqlid.ToString());

                    string money = String.Format("{0:D6}", value);
                    NAPI.ClientEvent.TriggerClientEvent(this.user, "hudUpdateMoney", money);
                }
                else
                {
                    result = 0;
                }

            }
            else
            {
                result = -1;
            }

            return result;
        }

        /// <summary>
        /// Pay money to another player (pocket money).
        /// </summary>
        /// <param name="target">Another player</param>
        /// <param name="value">Money amount</param>
        /// <returns></returns>
        public int PayTo(Player target, int value)
        {
            int result = this.SetMoney(this.character.money - value);
            if (result == 1)
            {
                target.SetMoney(target.character.money + value);
            }

            return result;
        }

        /// <summary>
        /// Show the payTo dialog (pocket money).
        /// </summary>
        /// <param name="target">Another player</param>
        public void PayToDialog(Player target)
        {
            this.payDialogTarget = target;
            GuiController.CreateDialog(this, new Dialog("Pagar dinero a " + target.character.showName, "Introduce la cantidad a pagar:", true, "Pagar", "Cancelar", OnPayToDialogResponse));
        }

        /// <summary>
        /// Processes the payTo dialog response (pocket money).
        /// </summary>
        /// <param name="response"></param>
        public void OnPayToDialogResponse(Player player, string amount)
        {
            if (Util.IsNumeric(amount))
            {
                int value = Math.Abs(int.Parse(amount));

                if (this.payDialogTarget != null)
                {
                    Vector3 inFrontOf = this.GetInFrontOf(Global.ExternalInteractionPlayerDistance);
                    if (this.payDialogTarget.user.Position.DistanceTo(inFrontOf) <= Global.ExternalInteractionPlayerDistance && this.user.Dimension == this.payDialogTarget.user.Dimension)
                    {
                        int result = this.PayTo(this.payDialogTarget, value);

                        switch (result)
                        {
                            case -1: this.DisplayNotification("~r~Valor 0 o negativo no válido"); break;
                            case 0: this.DisplayNotification("~r~No puede llevar más dinero encima"); break;
                            case 1:
                                {
                                    this.DisplayNotification("~p~Has pagado " + value + " USD a " + this.payDialogTarget.character.showName);
                                    this.payDialogTarget.DisplayNotification("~p~" + this.character.showName + " te ha pagado " + value + " USD");
                                    this.SendAme("entrega algo a " + this.payDialogTarget.character.showName);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        this.DisplayNotification("~r~Jugador demasiado lejos");
                    }
                }
            }
            else
            {
                this.DisplayNotification("~r~Valor incorrecto");
            }
        }

        /// <summary>
        /// Updates health value server-side (anticheat).
        /// </summary>
        /// <param name="value">The health new value</param>
        public void SetHealth(int value)
        {
            this.character.health = value;
            NAPI.Player.SetPlayerHealth(this.user, this.character.health);
        }

        /// <summary>
        /// Sets the player position (anticheat).
        /// </summary>
        /// <param name="newPosition">The player new position</param>
        /// <param name="newRotation">The player new rotation</param>
        public void SetPosition(Vector3 newPosition, Vector3 newRotation)
        {
            this.teleport = true;
            NAPI.Entity.SetEntityPosition(this.user, newPosition);
            NAPI.Entity.SetEntityRotation(this.user, newRotation);
        }

        /// <summary>
        /// Returns the player instance if Exists, otherwise returns null.
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <returns></returns>
        public static Player Exists(Client user)
        {
            Player playerFound = null;
            foreach (Player tmpPlayer in Players)
            {
                if (tmpPlayer.user.Equals(user))
                {
                    playerFound = tmpPlayer;
                    break;
                }
            }

            return playerFound;
        }

        /// <summary>
        /// Returns the player instance if matches with ID or part of the name.
        /// </summary>
        /// <param name="test">The string to test</param>
        /// <returns></returns>
        public static Player GetByIdOrName(string test)
        {
            Player playerFound = null;
            foreach (Player tmpPlayer in Players)
            {
                if (Util.IsNumeric(test))
                {
                    if (tmpPlayer.id == int.Parse(test))
                    {
                        playerFound = tmpPlayer;
                        break;
                    }
                }
                else
                {
                    if (tmpPlayer.character.showName.ToLower().Contains(test.ToLower()))
                    {
                        playerFound = tmpPlayer;
                        break;
                    }
                }
            }

            return playerFound;

        }

        /// <summary>
        /// Removes the player instance completely (only when disconnect).
        /// </summary>
        public void Delete()
        {
            Players.Remove(this);
        }

        /// <summary>
        /// Cleans the chat.
        /// </summary>
        public void CleanChat()
        {
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
            NAPI.Chat.SendChatMessageToPlayer(this.user, " ");
        }

        /// <summary>
        /// Returns a point in front of the player with the given distance
        /// </summary>
        /// <param name="distance">The offset distance</param>
        /// <returns></returns>
        public Vector3 GetInFrontOf(float distance)
        {
            return Util.GetPositionInsideVector(this.user.Position, this.user.Rotation.Z, distance);
        }

        /// <summary>
        /// Get the distance between players
        /// </summary>
        /// <param name="anotherPlayer">Another player</param>
        /// <returns></returns>
        public double GetDistanceBetweenPlayers(Player anotherPlayer)
        {
            return this.user.Position.DistanceTo(anotherPlayer.user.Position);
        }

        /// <summary>
        /// Returns a bool if player it's near position or not
        /// </summary>
        /// <param name="position">The position to test</param>
        /// <param name="distance">The maximum distance</param>
        /// <returns></returns>
        public bool NearPosition(Vector3 position, double distance)
        {
            return (this.user.Position.DistanceTo(position) <= distance);
        }

        /// <summary>
        /// Sends a ME message
        /// </summary>
        /// <param name="text">The action's text</param>
        public void SendMe(string text)
        {
            Chat.ProcessPlayerChat((int)Global.ChatChannel.Action, this, text);
        }

        /// <summary>
        /// Sends an AME message to the player.
        /// </summary>
        /// <param name="text">The action's text</param>
        public void SendAme(string text)
        {
            if (text.Length > 0 && text.Length <= Global.AmeLength)
            {
                if (this.character.ameTextLabel != null)
                {
                    NAPI.Entity.DeleteEntity(this.character.ameTextLabel);
                    this.character.ameCounter = 0;
                }

                this.character.ameTextLabel = NAPI.TextLabel.CreateTextLabel("~p~ * " + text, this.user.Position, Global.AmeRange, 0.5f, 1, new GTANetworkAPI.Color(255,255,255), false, this.user.Dimension);
                NAPI.Entity.AttachEntityToEntity(this.character.ameTextLabel, this.user, "31086", new Vector3(0, 0, 1.2), new Vector3(0, 0, 0)); //12844
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~" + Global.ChatActionColor + "~", "> " + text);
                this.character.ameCounter = Global.AmeDuration;
            }
        }

        /// <summary>
        /// Sets the YO description of the player.
        /// </summary>
        /// <param name="text"></param>
        public void SetYo(string text)
        {
            if (text.Length > 0 && text.Length <= Global.YoLength)
            {
                if (this.character.yoTextLabel != null)
                    NAPI.Entity.DeleteEntity(this.character.yoTextLabel);

                this.character.yoTextLabel = NAPI.TextLabel.CreateTextLabel("~r~" + text, this.user.Position, Global.YoRange, 0.5f, 1, new GTANetworkAPI.Color(255,255,255), false, this.user.Dimension);
                NAPI.Entity.AttachEntityToEntity(this.character.yoTextLabel, this.user, "31086", new Vector3(0, 0, 1.35), new Vector3(0, 0, 0)); //12844
            }
        }

        /// <summary>
        /// Deletes the YO description of the player.
        /// </summary>
        public void DeleteYo()
        {
            if (this.character.yoTextLabel != null)
                NAPI.Entity.DeleteEntity(this.character.yoTextLabel);
        }

        /// <summary>
        /// Switch voice type
        /// </summary>
        public void ChangeVoiceType()
        {
            if (this.character != null)
            {
                if (this.character.voiceType == 0)
                {
                    this.character.voiceType = 1;
                    NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetVoiceType", "Gritar");
                }
                else if (this.character.voiceType == 1)
                {
                    this.character.voiceType = 2;
                    NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetVoiceType", "Susurrar");
                }
                else if (this.character.voiceType == 2)
                {
                    this.character.voiceType = 0;
                    NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetVoiceType", "Hablar");
                }

            }
        }

        /// <summary>
        /// Sets the player on dead state (this should only be called in the playerDeath event)
        /// </summary>
        public void SetDead()
        {
            if (this.character != null)
            {
                // First we save the basic character data.
                this.SaveCharacter();

                // Freeze the player
                // NAPI.freezePlayer(this.user, true);

                // Set player invincible
                NAPI.Entity.SetEntityInvincible(this.user, true);

                // We save the player dying state
                this.character.dying = 1;
                Util.UpdateDBField("redrp_character", "dying", this.character.dying.ToString(), "id", this.character.sqlid.ToString());

                // Set the dying counter
                this.character.dyingCounter = Global.DeathTime;

                // Information messages
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~r~Tu personaje se encuentra inconsciente y necesita asistencia médica.");
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~r~Dentro de cinco minutos podrás reaparecer desde el menú de interacción propia.");
            }
        }

        /// <summary>
        /// Revives the player
        /// </summary>
        /// <param name="hospital">If must spawn on the hospital before revive</param>
        public void Revive(bool hospital)
        {
            if (this.character != null)
            {
                // Set player vincible
                NAPI.Entity.SetEntityInvincible(this.user, false);

                // Remove the player dying state
                this.character.dying = 0;
                Util.UpdateDBField("redrp_character", "dying", this.character.dying.ToString(), "id", this.character.sqlid.ToString());

                // Reset the dying counter
                this.character.dyingCounter = 0;

                // Heal the player
                this.SetHealth(100);

                // If we set respawning to hospital the player will be teleported to the main hospital
                if (hospital)
                {
                    this.SetPosition(new Vector3(339.0326, -1394.908, 32.50927), new Vector3(this.user.Rotation.X, this.user.Rotation.Y, 46.97203));
                    this.SaveCharacter();
                }
            }
        }

        /// <summary>
        /// Shows the help menu
        /// </summary>
        public void ShowHelpMenu()
        {
            Menu menu = new Menu("", "Menú de ayuda", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnHelpMenuResponse));

            menu.menuModel.items.Add(new MenuItemModel("Ayuda general", "Comandos y teclas"));
            menu.menuModel.items.Add(new MenuItemModel("Cuenta", "Información de la cuenta"));
            menu.menuModel.items.Add(new MenuItemModel("Admins", "Lista de admins activos"));
            menu.menuModel.items.Add(new MenuItemModel("Reportar", "Contactar con los admins"));
            if(admin > 0)
            {
                menu.menuModel.items.Add(new MenuItemModel("Comandos admin"));
                menu.menuModel.items.Add(new MenuItemModel("Ver reportes"));
                menu.menuModel.items.Add(new MenuItemModel("Ir a"));
            }

            // Create help menu
            GuiController.CreateMenu(this, menu);
        }

        /// <summary>
        /// Manages help menu responses
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The selected option as string</param>
        /// <param name="actionId">The selected option</param>
        public void OnHelpMenuResponse(Player player, string option, int actionId)
        {
            switch (actionId)
            {
                // OPEN HELP PAGE
                case 0:
                    {
                        player.ShowHelpPage();
                        break;
                    }
                // OPEN ACCOUNT PAGE
                case 1:
                    {
                        Player.ShowAccountInfoPage(player, player);
                        break;
                    }
                // OPEN ADMIN LIST PAGE
                case 2:
                    {
                        player.ShowAdminListPage();
                        break;
                    }
                // OPEN SEND ADMIN REPORT DIALOG
                case 3:
                    {
                        if (!AdminReport.HasReport(player))
                        {
                            
                            GuiController.CreateDialog(player, new Dialog("Nuevo reporte administrativo", "Escribe tu reporte, trata de ser explícito con lo que quieres.", true, "Enviar", "Cancelar", AdminReport.OnReportSent));
                        }
                        else
                        {
                            player.DisplayNotification("~r~Ya tienes un reporte abierto, espera que se solucione.");
                        }
                        break;
                    }
                // OPEN ADMIN HELP (FOR ADMINS)
                case 4:
                    {
                        Admin.ShowAdminHelp(player);
                        break;
                    }
                // OPEN REPORTS LIST (FOR ADMINS)
                case 5:
                    {
                        if (Global.AdminReports.Count > 0)
                        {
                            AdminReport.GenerateReportsMenu(player);
                        }
                        else
                        {
                            player.DisplayNotification("~g~No hay reportes pendientes.");
                        }

                        break;
                    }
                // OPEN LOCATIONS LIST (FOR ADMINS)
                case 6:
                    {
                        AdminTeleport.GenerateSelectionMenu(player);

                        break;
                    }
            }
        }

        /// <summary>
        /// Shows the help page
        /// </summary>
        public void ShowHelpPage()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "showGeneralHelp");
            this.showingGui = (int)GuiController.GuiTypes.HelpPage;
            this.showingGuiId = 0;
        }

        /// <summary>
        /// Hide the help page
        /// </summary>
        public void HideHelpPage()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hideGeneralHelp");
            this.showingGui = -1;
            this.showingGuiId = -1;
        }

        /// <summary>
        /// Shows the account info page
        /// </summary>
        /// <param name="player">The player from we will obtain the data</param>
        /// <param name="showTo">The player we will show the data to</param>
        public static void ShowAccountInfoPage(Player player, Player showTo)
        {
            // ACCOUNT DATA

            string accountName = player.name;

            List<string[]> accountDataList = new List<string[]>();

            string[] data = new string[2];
            data = new string[2];
            data[0] = "Nivel admin";
            data[1] = player.admin.ToString();
            accountDataList.Add(data);

            string accountData = NAPI.Util.ToJson(accountDataList);

            // CHARACTER GENERAL DATA
            string characterName = player.character.cleanName;

            List<string[]> characterGeneralDataList = new List<string[]>();

            data = new string[2];
            data[0] = "Horas jugadas";
            data[1] = player.character.playedHours.ToString();
            characterGeneralDataList.Add(data);
            data = new string[2];
            data[0] = "Logins";
            data[1] = player.character.logins.ToString();
            characterGeneralDataList.Add(data);
            data = new string[2];
            data[0] = "Sexo";
            data[1] = player.GetSex();
            characterGeneralDataList.Add(data);
            data = new string[2];
            data[0] = "Raza";
            data[1] = player.GetRace();
            characterGeneralDataList.Add(data);
            data = new string[2];
            data[0] = "Edad";
            data[1] = player.character.age.ToString();
            characterGeneralDataList.Add(data);

            string characterGeneralData = NAPI.Util.ToJson(characterGeneralDataList);

            // CHARACTER PROPERTY DATA
            List<string[]> characterPropertyDataList = new List<string[]>();

            data = new string[2];
            data[0] = "Vivienda #1";
            data[1] = "-";
            characterPropertyDataList.Add(data);
            data = new string[2];
            data[0] = "Vivienda #2";
            data[1] = "-";
            characterPropertyDataList.Add(data);
            data = new string[2];
            data[0] = "Vehiculo #1";
            data[1] = "-";
            characterPropertyDataList.Add(data);
            data = new string[2];
            data[0] = "Vehiculo #2";
            data[1] = "-";
            characterPropertyDataList.Add(data);

            string characterPropertyData = NAPI.Util.ToJson(characterPropertyDataList);

            //CHARACTER EXPERIENCE DATA
            string characterExperienceData = player.GetExperienceData();

            NAPI.ClientEvent.TriggerClientEvent(showTo.user, "showAccountInfo", accountName, accountData, characterName, characterGeneralData, characterPropertyData, characterExperienceData);
            showTo.showingGui = (int)GuiController.GuiTypes.AccountPage;
            showTo.showingGuiId = 0;
        }

        /// <summary>
        /// Hide the account info page
        /// </summary>
        public void HideAccountInfoPage()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hideAccountInfo");
            this.showingGui = -1;
            this.showingGuiId = -1;
        }

        /// <summary>
        /// Gets the player race in string
        /// </summary>
        /// <returns></returns>
        public string GetRace()
        {
            string race;

            switch (this.character.race)
            {
                case 0: race = "Caucásica"; break;
                case 1: race = "Afroamericana"; break;
                case 2: race = "Hispana"; break;
                case 3: race = "Asiática"; break;
                case 4: race = "Árabe"; break;
                default: race = "Caucásica"; break;
            }

            return race;
        }

        /// <summary>
        /// Gets the player sex in string
        /// </summary>
        /// <returns></returns>
        public string GetSex()
        {
            string sex;

            switch (this.character.sex)
            {
                case 0: sex = "Hombre"; break;
                case 1: sex = "Mujer"; break;
                default: sex = "Hombre"; break;
            }

            return sex;
        }

        /// <summary>
        /// Calculates new experience index depending on success
        /// </summary>
        /// <param name="experienceId">The experience type</param>
        /// <param name="probability">The probability</param>
        /// <returns></returns>
        public bool CalculateExperience(uint experienceId, int probability = 99)
        {
            bool success = false;

            success = new Random().Next(100 - probability) == 0;

            if (success)
            {
                int experience = 1;

                switch (experienceId)
                {
                    case (uint)ExperienceTypes.Security:
                        {
                            if ((this.character.securityExp + experience) < 100)
                            {
                                this.character.securityExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "seguridad", this.character.securityExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Mechanic:
                        {
                            if ((this.character.mechanicExp + experience) < 100)
                            {
                                this.character.mechanicExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "mecanica", this.character.mechanicExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Gta:
                        {
                            if ((this.character.grandTheftAutoExp + experience) < 100)
                            {
                                this.character.grandTheftAutoExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "robocoches", this.character.grandTheftAutoExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Criminal:
                        {
                            if ((this.character.criminalExp + experience) < 100)
                            {
                                this.character.criminalExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "delincuente", this.character.criminalExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Transport:
                        {
                            if ((this.character.transportistExp + experience) < 100)
                            {
                                this.character.transportistExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "transportista", this.character.transportistExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Taxi:
                        {
                            if ((this.character.taxistExp + experience) < 100)
                            {
                                this.character.taxistExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "taxista", this.character.taxistExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                    case (uint)ExperienceTypes.Fishing:
                        {
                            if ((this.character.fishingExp + experience) < 100)
                            {
                                this.character.fishingExp += experience;
                                Util.UpdateDBField("personajes_experiencia", "pesca", this.character.fishingExp.ToString(), "id", this.character.sqlid.ToString());
                            }
                            break;
                        }
                }
            }

            return success;
        }

        /// <summary>
        /// Gets the character experience in JSON string format
        /// </summary>
        /// <returns></returns>
        public string GetExperienceData()
        {

            List<string[]> experienceDataList = new List<string[]>();

            string[] data = new string[2];
            data[0] = "Seguridad";
            data[1] = this.character.securityExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Mecánica";
            data[1] = this.character.mechanicExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Ladrón de vehiculos";
            data[1] = this.character.grandTheftAutoExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Criminal";
            data[1] = this.character.criminalExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Transportista";
            data[1] = this.character.transportistExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Taxista";
            data[1] = this.character.taxistExp.ToString();
            experienceDataList.Add(data);
            data = new string[2];
            data[0] = "Pesca";
            data[1] = this.character.fishingExp.ToString();
            experienceDataList.Add(data);

            string experienceData = NAPI.Util.ToJson(experienceDataList);

            return experienceData;
        }

        /// <summary>
        /// Shows the player list page
        /// </summary>
        public void ShowPlayerListPage()
        {
            List<string[]> playerList = new List<string[]>();
            foreach (Player player in Players)
            {
                string[] data = new string[3];
                data[0] = player.name;
                data[1] = player.character.cleanName + " (#" + player.id + ")";
                data[2] = NAPI.Player.GetPlayerPing(player.user).ToString();
                playerList.Add(data);
            }

            string playerListData = NAPI.Util.ToJson(playerList);

            NAPI.ClientEvent.TriggerClientEvent(this.user, "showPlayerList", playerListData);
            this.showingGui = (int)GuiController.GuiTypes.PlayerListPage;
            this.showingGuiId = 0;
        }

        /// <summary>
        /// Hide the player list page
        /// </summary>
        public void HidePlayerListPage()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hidePlayerList");
            this.showingGui = -1;
            this.showingGuiId = -1;
        }

        /// <summary>
        /// Shows the admin list page
        /// </summary>
        public void ShowAdminListPage()
        {
            List<string[]> adminList = new List<string[]>();

            int count = 0;
            foreach (Player player in Players)
            {
                if (player.admin > 0)
                {
                    if (player.adminDuty)
                    {
                        string rank = "";
                        string rankColor = "";
                        switch (player.admin)
                        {
                            case 1:
                                {
                                    rank = "Colaborador";
                                    rankColor = Global.CollaboratorColor;
                                    break;
                                }
                            case 2:
                            case 3:
                            case 4:
                                {
                                    rank = "Game Master";
                                    rankColor = Global.GameMasterColor;
                                    break;
                                }
                            default:
                                {
                                    rank = "Administrador";
                                    rankColor = Global.AdministratorColor;
                                    break;
                                }
                        }

                        string[] data = new string[4];
                        data[0] = player.name;
                        data[1] = player.character.cleanName + " (#" + player.id + ")";
                        data[2] = rank;
                        data[3] = rankColor;
                        adminList.Add(data);
                        count++;
                    }
                }

            }

            if (count == 0)
            {
                this.DisplayNotification("~r~Actualmente no hay administradores en servicio");
            }
            else
            {
                string adminListData = NAPI.Util.ToJson(adminList);

                NAPI.ClientEvent.TriggerClientEvent(this.user, "showAdminList", adminListData);
                this.showingGui = (int)GuiController.GuiTypes.AdminListPage;
                this.showingGuiId = 0;
            }
        }

        /// <summary>
        /// Hide the admin list page
        /// </summary>
        public void HideAdminListPage()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hideAdminList");
            this.showingGui = -1;
            this.showingGuiId = -1;
        }

        /// <summary>
        /// Displays a notification.
        /// </summary>
        /// <param name="text">The notification text</param>
        public void DisplayNotification(string text, bool flashing = false)
        {
            if (text.Length > 0)
            {
                NAPI.Notification.SendNotificationToPlayer(this.user, text, flashing);
            }
        }

        /// <summary>
        /// Displays a picture notification.
        /// </summary>
        /// <param name="text">The notification text</param>
        public void DisplayPictureNotification(string text, string picture, int flash, int iconType, string sender, string subject)
        {
            if (text.Length > 0)
            {
                NAPI.Notification.SendPictureNotificationToPlayer(this.user, text, picture, flash, iconType, sender, subject);
            }
        }

        /// <summary>
        /// Displays a shard.
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="timeout">The duration</param>
        public void ShowShard(string text, int timeout = 3000)
        {
            if (text.Length > 0 && timeout >= 3000)
            {
                NAPI.ClientEvent.TriggerClientEvent(this.user, "hudShowShard", text, timeout);
            }
        }

        /// <summary>
        /// Shows the loading prompt
        /// </summary>
        /// <param name="text">The text</param>
        /// <param name="type">The loading prompt type</param>
        public void ShowLoadingPrompt(string text, int type)
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudShowLoadingPrompt", text, type);
        }

        /// <summary>
        /// Hides the loading prompt
        /// </summary>
        public void HideLoadingPrompt()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudHideLoadingPrompt");
        }

        /// <summary>
        /// Sets the player map waypoint.
        /// </summary>
        /// <param name="x">The X component</param>
        /// <param name="y">The Y component</param>
        public void SetWaypoint(double x, double y)
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudSetMapWaypoint", x, y);
        }

        /// <summary>
        /// Removes the player map waypoint.
        /// </summary>
        public void RemoveWaypoint()
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "hudRemoveMapWaypoint");
        }

        /// <summary>
        /// Checks if player is in AFK and acts in consequence if so
        /// </summary>
        public void CheckAFK()
        {
            if (this.afkPosition != null)
            {
                if (this.user.Position.DistanceTo(this.afkPosition) == 0.0 && this.character.dying == 0 && this.admin == 0)
                {
                    this.afkTime++;
                    if (this.afkTime >= Global.AntiAfkTime / 2)
                    {
                        NAPI.Notification.SendNotificationToPlayer(this.user, "~r~Detectado AFK", true);
                    }
                    else if (this.afkTime == Global.AntiAfkTime)
                    {
                        NAPI.Player.KickPlayer(this.user, "AFK");
                    }
                }
                else
                {
                    this.afkTime = 0;
                }
            }

            this.afkPosition = this.user.Position;
        }

        /// <summary>
        /// Checks player's ping and acts in consequence if it's so high
        /// </summary>
        public void CheckPing()
        {
            if (NAPI.Player.GetPlayerPing(this.user) >= Global.AntiPingLimit)
            {
                this.pingWarns++;
                if (this.pingWarns == Global.AntiPingMaxWarns)
                {
                    NAPI.Player.KickPlayer(this.user, "Ping elevado");
                }
                else
                {
                    NAPI.Notification.SendNotificationToPlayer(this.user, "~r~Ping elevado");
                }
            }
            else
            {
                if (this.pingWarns > 0)
                {
                    this.pingWarns--;
                }
            }
        }

        /// <summary>
        /// Calculates maintenance fees and interests over player account's
        /// </summary>
        public void PlayerBankAccountsPaydayCheck()
        {
            foreach(BankAccount account in this.character.bankAccounts)
            {
                int interests = account.GenerateInterests();
                int maintenanceFee = account.GenerateMaintenanceFee();

                NAPI.Chat.SendChatMessageToPlayer(this.user, "[DEBUG] Calculando cuenta bancaria " + account.accountNumber + " | Interés: " + interests + " | Mantenimiento: " + maintenanceFee);
            }
        }

        /// <summary>
        /// Returns the player instance by his sqlid
        /// </summary>
        /// <param name="sqlid">The player sqlid</param>
        /// <returns></returns>
        public static Player GetPlayerBySqlid(int sqlid)
        {
            Player playerFound = null;
            foreach(Player player in Players)
            {
                if(player.sqlid == sqlid)
                {
                    playerFound = player;
                    break;
                }
            }

            return playerFound;
        }

        /// <summary>
        /// Returns the player's foot position
        /// </summary>
        /// <returns></returns>
        public Vector3 GetFootPos()
        {
            return new Vector3(this.user.Position.X, this.user.Position.Y, this.user.Position.Z - 0.8);
        }

        /// <summary>
        /// Returns if the player can use the inventory or not in the current conditions
        /// </summary>
        /// <returns></returns>
        public bool CanUseInventory()
        {
            return !(NAPI.Player.IsPlayerInAnyVehicle(this.user) || NAPI.Player.IsPlayerInFreefall(this.user) || NAPI.Player.IsPlayerOnLadder(this.user) || NAPI.Player.IsPlayerParachuting(this.user) || this.character.cuffed != 0 || this.character.dying == 1);
        }

        /// <summary>
        /// Blocks and unblock player weapon wheel interactions
        /// </summary>
        /// <param name="toggle">Toggle weapon wheel usage</param>
        public void DisableWeaponWheel(bool toggle)
        {
            NAPI.ClientEvent.TriggerClientEvent(this.user, "toggleWeaponWheel", toggle);
        }

        /// <summary>
        /// Plays with a coin
        /// </summary>
        public void ThrowCoin()
        {
            int rnd = Util.Random(2);
            switch (rnd)
            {
                case 0: this.SendMe("tira una moneda y sale cara."); break;
                case 1: this.SendMe("tira una moneda y sale cruz."); break; 
            }
        }

        /// <summary>
        /// Throws a dice and shows the result
        /// </summary>
        public void ThrowDice()
        {
            int rnd = Util.Random(6);
            rnd++;
            this.SendMe("tira un dado y sale un " + rnd + ".");
        }

        /// <summary>
        /// Returns false if have an active deal with another player, true if not.
        /// </summary>
        /// <returns></returns>
        public bool CanStartDeal()
        {
            bool canStartDeal = true;
            foreach (Player target in Players)
            {
                if (target.character != null)
                {
                    if (target.character.dealOwner != null)
                    {
                        if (target.character.dealOwner.Equals(this))
                        {
                            canStartDeal = false;
                            break;
                        }
                    }
                }
            }

            return canStartDeal;
        }

        /// <summary>
        /// Executes a deal just before being accepted
        /// </summary>
        public void AcceptDeal()
        {
            if(this.character.dealOwner != null)
            {
                switch (this.character.dealType)
                {
                    case (int)Global.Deals.SoftCuff:
                        {
                            this.SetCuffed(1, true);
                            this.DisplayNotification("~r~" + this.character.dealOwner.character.showName + " te ha atado las manos.");
                            this.character.dealOwner.DisplayNotification("~g~" + this.character.showName + " ha aceptado que le ates las manos.");
                            break;
                        }
                    case (int)Global.Deals.HardCuff:
                        {
                            this.SetCuffed(2, true);
                            this.DisplayNotification("~r~" + this.character.dealOwner.character.showName + " te ha esposado.");
                            this.character.dealOwner.DisplayNotification("~g~" + this.character.showName + " ha aceptado ser esposado.");
                            break;
                        }
                    case (int)Global.Deals.Search:
                        {
                            Player searcher = this.character.dealOwner;
                            Inventory.OpenPlayerLootingMenu(searcher, this);
                            this.DisplayNotification("~r~" + searcher.character.showName + " te está cacheando.");
                            searcher.DisplayNotification("~g~" + this.character.showName + " ha aceptado ser cacheado.");
                            break;
                        }
                    case (int)Global.Deals.Rob:
                        {
                            Player searcher = this.character.dealOwner;
                            Inventory.OpenPlayerLootingMenu(searcher, this);
                            this.DisplayNotification("~r~" + searcher.character.showName + " te está atracando.");
                            searcher.DisplayNotification("~g~" + this.character.showName + " ha aceptado el atraco.");
                            break;
                        }
                }

                this.character.dealOwner = null;
                this.character.dealRemainingTime = 0;
                this.character.dealType = -1;
            }
            else
            {
                this.DisplayNotification("~r~El trato ha expirado.");
            }
        }

        /// <summary>
        /// Refuses a deal
        /// </summary>
        public void RefuseDeal()
        {
            if (this.character.dealOwner != null)
            {
                this.character.dealOwner.DisplayNotification("~r~" + this.character.showName + " ha rechazado el trato.");
                this.character.dealOwner = null;
                this.character.dealRemainingTime = 0;
                this.character.dealType = -1;
                this.character.dealDescription = "";
            }
            else
            {
                this.DisplayNotification("~r~El trato ha expirado.");
            }
        }

        /// <summary>
        /// Starts a rob deal with another player
        /// </summary>
        /// <param name="targetPlayer">Another player</param>
        public void RobDeal(Player targetPlayer)
        {
            if (this.CanStartDeal())
            {
                targetPlayer.character.dealOwner = this;
                targetPlayer.character.dealRemainingTime = Global.DealExpiringTime;

                targetPlayer.character.dealType = (int)Global.Deals.Rob;
                targetPlayer.character.dealDescription = this.character.showName + " quiere robarte.";
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~ Estas tratando de atracar a " + this.character.showName + " espera que acepte la acción.");
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~ Si rechaza el trato, puedes actuar en consecuencia.");
                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~b~" + this.character.showName + " quiere robarte. Puedes ~g~/aceptar~b~ o ~r~/rechazar~b~.");
                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~ Si rechazas el trato, atente a las consecuencias.");
            }
            else
            {
                this.DisplayNotification("~r~Ya tienes un trato activo, espera un poco.");
            }
        }

        /// <summary>
        /// Starts a search deal with another player
        /// </summary>
        /// <param name="targetPlayer">Another player</param>
        public void SearchDeal(Player targetPlayer)
        {
            if (this.CanStartDeal())
            {
                targetPlayer.character.dealOwner = this;
                targetPlayer.character.dealRemainingTime = Global.DealExpiringTime;

                targetPlayer.character.dealType = (int)Global.Deals.Search;
                targetPlayer.character.dealDescription = this.character.showName + " quiere cachearte.";
                NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~ Estas tratando de cachear a " + this.character.showName + " espera que acepte la acción.");
                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~b~" + this.character.showName + " quiere cachearte. Puedes ~g~/aceptar~b~ o ~r~/rechazar~b~.");
            }
            else
            {
                this.DisplayNotification("~r~Ya tienes un trato activo, espera un poco.");
            }
        }

        /// <summary>
        /// Starts a cuff deal with another player
        /// </summary>
        /// <param name="targetPlayer">Another player</param>
        /// <param name="type">Type of cuff deal</param>
        public void CuffDeal(Player targetPlayer, bool type)
        {
            if (this.CanStartDeal())
            {
                targetPlayer.character.dealOwner = this;
                targetPlayer.character.dealRemainingTime = Global.DealExpiringTime;

                if (type)
                {
                    targetPlayer.character.dealType = (int)Global.Deals.SoftCuff;
                    targetPlayer.character.dealDescription = this.character.showName + " quiere atarte las manos.";
                    NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~ Estas tratando de atar las manos a " + targetPlayer.character.showName + " espera que acepte la acción.");
                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~b~" + this.character.showName + " quiere atarte las manos. Puedes ~g~/aceptar~b~ o ~r~/rechazar~b~.");
                }
                else
                {
                    targetPlayer.character.dealType = (int)Global.Deals.HardCuff;
                    targetPlayer.character.dealDescription = this.character.showName + " quiere esposarte.";
                    NAPI.Chat.SendChatMessageToPlayer(this.user, "~y~ Estás tratando de esposar a " + targetPlayer.character.showName + " espera que acepte la acción.");
                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~b~" + this.character.showName + " quiere esposarte. Puedes ~g~/aceptar~b~ o ~r~/rechazar~b~.");
                }
            }
            else
            {
                this.DisplayNotification("~r~Ya tienes un trato activo, espera un poco.");
            }
        }

        /// <summary>
        /// Uncuffs another player
        /// </summary>
        /// <param name="targetPlayer">Another player</param>
        public void Uncuff(Player targetPlayer)
        {
            int cuffedState = targetPlayer.character.cuffed;
            switch (cuffedState)
            {
                case 0:
                    {
                        this.DisplayNotification("~r~No está esposado.");
                        break;
                    }
                case 1:
                    {
                        targetPlayer.SetCuffed(0, true);
                        this.DisplayNotification("~g~Liberado.");
                        targetPlayer.DisplayNotification("~g~" + this.character.showName + " te ha liberado.");
                        break;
                    }
                case 2:
                    {
                        if(this.character.inventory.HasItem(137) != null)
                        {
                            targetPlayer.SetCuffed(0, true);
                            this.DisplayNotification("~g~Liberado.");
                            targetPlayer.DisplayNotification("~g~" + this.character.showName + " te ha quitado las esposas.");
                        }
                        else
                        {
                            this.DisplayNotification("~g~Necesitas una llave.");
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets or not the player to be cuffed. States: 0 not cuffed, 1 soft cuffed, 2 hard cuffed
        /// </summary>
        /// <param name="state">The cuffed state</param>
        /// <param name="save">If must be saved or not</param>
        public void SetCuffed(int state, bool save)
        {
            switch (state)
            {
                case 0:
                    {
                        this.ToggleCuffedAnimation(false);
                        this.character.cuffed = 0;
                        this.DisableWeaponWheel(false);
                        break;
                    }
                case 1:
                    {
                        this.ToggleCuffedAnimation(true);
                        this.character.cuffed = 1;
                        this.DisableWeaponWheel(true);
                        break;
                    }
                case 2:
                    {
                        this.ToggleCuffedAnimation(true);
                        this.character.cuffed = 2;
                        this.DisableWeaponWheel(true);
                        break;
                    }
            }

            if(save)
                Util.UpdateDBField("redrp_character", "cuffed", this.character.cuffed.ToString(), "id", this.character.sqlid.ToString());
        }

        /// <summary>
        /// Toggle the cuff animation and props
        /// </summary>
        /// <param name="toggle">True or false</param>
        public void ToggleCuffedAnimation(bool toggle)
        {
            if (toggle)
            {
                this.forcedAnimation = true;
                NAPI.Player.PlayPlayerAnimation(this.user, (int)(Animation.Flags.Loop | Animation.Flags.OnlyAnimateUpperBody | Animation.Flags.AllowPlayerControl), "mp_arresting", "idle");
                if(this.character.sex == 0)
                {
                    NAPI.Player.SetPlayerClothes(this.user, 7, 41, 0);
                }
                else
                {
                    NAPI.Player.SetPlayerClothes(this.user, 7, 25, 0);
                }
            }
            else
            {
                this.forcedAnimation = false;
                NAPI.Player.StopPlayerAnimation(this.user);
                if(this.character.inventory.accessory != null)
                {
                    this.character.inventory.SetVariation((int)Global.InventoryBodypart.Accessory, false);
                }
                else
                {
                    this.character.inventory.RemoveVariation((int)Global.InventoryBodypart.Accessory);
                }
                
            }
        }

        /// <summary>
        /// Opens the mood menu
        /// </summary>
        public void OpenMoodMenu()
        {
            Menu menu = new Menu("", "Estado de animo", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnMoodMenuResponse));

            foreach (KeyValuePair<int, string> style in Global.PlayerMoods)
            {
                if (style.Key == this.character.mood)
                {
                    menu.menuModel.items.Add(new MenuItemModel(style.Value, "", 1, 0, 0, "", null, false, System.Drawing.Color.Green));
                }
                else
                {
                    menu.menuModel.items.Add(new MenuItemModel(style.Value));
                }
            }

            // Go back
            menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

            GuiController.CreateMenu(this, menu);
        }

        /// <summary>
        /// Receives the mood menu responses
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The option as string</param>
        /// <param name="actionId">The option id</param>
        public static void OnMoodMenuResponse(Player player, string option, int actionId)
        {
            if(actionId <= Global.PlayerMoods.Count)
            {
                int mood = Global.PlayerMoods.Keys.ElementAt(actionId);
                player.SetMood(mood);
            }
            else
            {
                SelfInteraction.GenerateMenu(player);
            }
        }

        /// <summary>
        /// Sets the character's mood
        /// </summary>
        /// <param name="mood">The mood animation</param>
        public void SetMood(int mood)
        {
            this.character.mood = mood;

            this.ApplyFaceAnimation();

            Util.UpdateDBField("redrp_character", "mood", this.character.mood.ToString(), "id", this.character.sqlid.ToString());
        }

        /// <summary>
        /// Apply the mood animation
        /// </summary>
        public void ApplyFaceAnimation()
        {
            NAPI.Data.SetEntitySharedData(this.user, "playerMood", this.character.mood);
        }

        /// <summary>
        /// Opens the movement clipset menu
        /// </summary>
        public void OpenWalkingStyleMenu()
        {
            Menu menu = new Menu("", "Estilo de caminar", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnWalkingStyleMenuResponse));
            
            foreach(KeyValuePair<string, string> style in Global.PlayerWalkingStyles)
            {
                if(style.Key == this.character.walkingStyle)
                {
                    menu.menuModel.items.Add(new MenuItemModel(style.Value, "", 1, 0, 0, "", null, false, System.Drawing.Color.Green));
                }
                else
                {
                    menu.menuModel.items.Add(new MenuItemModel(style.Value));
                }
            }

            // Go back
            menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

            GuiController.CreateMenu(this, menu);
        }

        /// <summary>
        /// Receives the walking style menu responses
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="option">The option as string</param>
        /// <param name="actionId">The option id</param>
        public static void OnWalkingStyleMenuResponse(Player player, string option, int actionId)
        {
            if (actionId <= Global.PlayerWalkingStyles.Count)
            {
                if (!player.forcedClipset)
                {
                    string clipset = Global.PlayerWalkingStyles.Keys.ElementAt(actionId);
                    player.SetMovementClipsetAnim(clipset);
                }
                else
                {
                    player.DisplayNotification("~r~No puedes cambiar el estilo de caminar ahora mismo.");
                }
            }
            else
            {
                SelfInteraction.GenerateMenu(player);
            }
        }

        /// <summary>
        /// Activates/deactivates crouch mode
        /// </summary>
        public void ToggleCrouchMode()
        {
            if (this.crouchMode)
            {
                this.SetMovementClipset(this.character.walkingStyle);
                this.crouchMode = false;
                this.forcedClipset = false;
            }
            else
            {
                this.SetMovementClipset("move_ped_crouched");
                this.crouchMode = true;
                this.forcedClipset = true;
            }
        }

        /// <summary>
        /// Sets the current walking animation and saves to the database
        /// </summary>
        /// <param name="clipset">The clipset</param>
        public void SetMovementClipsetAnim(string clipset)
        {
            this.character.walkingStyle = clipset;

            this.SetMovementClipset(this.character.walkingStyle);

            Util.UpdateDBField("redrp_character", "walkingAnimation", this.character.walkingStyle.ToString(), "id", this.character.sqlid.ToString());
        }

        /// <summary>
        /// Sets the movement clipset for the player
        /// </summary>
        /// <param name="clipset">The clipset</param>
        public void SetMovementClipset(string clipset)
        {
            NAPI.Data.SetEntitySharedData(this.user, "playerMovementClipset", clipset);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////COMMANDS//////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Commands

        /**
         * /q
         * Disconnects from the server
         */
        [Command("q")]
        public void Q(Client sender)
        {
            Player player = Exists(sender);
            if (player.character != null)
            {
                NAPI.Player.KickPlayer(player.user, "Desconectado");
            }
        }

        /**
        * /moneda
        * Throws a coin
        */
        [Command("moneda")]
        public void Moneda(Client sender)
        {
            Player player = Exists(sender);
            if (player.character != null)
            {
                if (player.character.money > 0)
                {
                    player.ThrowCoin();
                }
                else
                {
                    player.DisplayNotification("~r~Ni una sola moneda te queda, desgraciado.");
                }
            }
        }

        /**
        * /dado
        * Throws a dice
        */
        [Command("dado")]
        public void Dado(Client sender)
        {
            Player player = Exists(sender);
            if (player.character != null)
            {
                if(player.character.inventory.HasItem(134) != null)
                {
                    player.ThrowDice();
                }
                else
                {
                    player.DisplayNotification("~r~No tienes un dado.");
                }
            }
        }

        /**
        * /aceptar
        * Accepts the active trait
        */
        [Command("aceptar")]
        public void Aceptar(Client sender)
        {
            Player player = Exists(sender);
            if (player.character != null)
            {
                if(player.character.dealOwner != null)
                {
                    player.AcceptDeal();
                }
                else
                {
                    player.DisplayNotification("~r~No estás en ningún trato.");
                }
            }
        }

        /**
        * /rechazar
        * Refuses the active trait
        */
        [Command("rechazar")]
        public void Rechazar(Client sender)
        {
            Player player = Exists(sender);
            if (player.character != null)
            {
                if (player.character.dealOwner != null)
                {
                    player.RefuseDeal();
                }
                else
                {
                    player.DisplayNotification("~r~No estás en ningún trato.");
                }
            }
        }

        #endregion

    }
}
