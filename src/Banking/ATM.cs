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
    /// ATM class and methods
    /// </summary>
    public class ATM : Script
    {

        // ATM ATTRIBUTES

        /// <summary>
        /// Database identifier
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// ATM position
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// ATM player heading
        /// </summary>
        public double heading { get; set; }

        /// <summary>
        /// ATM dimension
        /// </summary>
        public uint dimension { get; set; }

        /// <summary>
        /// ATM owner
        /// </summary>
        public int bank { get; set; }

        /// <summary>
        /// ATM available cash
        /// </summary>
        public int cash { get; set; }

        /// <summary>
        /// ATM has map blip or not
        /// </summary>
        public bool hasBlip { get; set; }

        /// <summary>
        /// If ATM is in use or not
        /// </summary>
        public bool isInUse { get; set; }

        /// <summary>
        /// ATM map blip instance
        /// </summary>
        public Blip blip { get; set; }

        // EVENTS

        /// <summary>
        /// Triggered when a player selects a card on the ATM menu
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="account">The account number of the card</param>
        [RemoteEvent("atmSelectCard")]
        public void OnSelectCard(Client user, string account)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                SelectCard(player, account);
            }
        }

        /// <summary>
        /// Triggered when a player sends the card PIN
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="pin">The PIN</param>
        [RemoteEvent("atmSendPIN")]
        public void OnSendPIN(Client user, string pin)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                CheckPIN(player, pin);
            }
        }

        /// <summary>
        /// Triggered when a player tries to open the deposit subpage
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmOpenDeposit")]
        public void OnOpenDeposit(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                OpenDeposit(player);
            }
        }

        /// <summary>
        /// Triggered when a player tries to open the withdraw subpage
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmOpenWithdraw")]
        public void OnOpenWithdraw(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                OpenWithdraw(player);
            }
        }

        /// <summary>
        /// Triggered when a player tries to open the transfer subpage
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmOpenTransfer")]
        public void OnOpenTransfer(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                OpenTransfer(player);
            }
        }

        /// <summary>
        /// Triggered when a player tries to open the history subpage
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmOpenHistory")]
        public void OnOpenHistoric(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                OpenHistory(player);
            }
        }

        /// <summary>
        /// Triggered when a player commits a deposit
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="amount">The deposit amount</param>
        /// <param name="concept">A brief concept</param>
        [RemoteEvent("atmMakeDeposit")]
        public void OnMakeDeposit(Client user, string amount, string concept)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MakeDeposit(player, amount, concept);
            }
        }

        /// <summary>
        /// Triggered when a player commits a withdraw
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="amount">The withdraw amount</param>
        /// <param name="concept">A brief concept</param>
        [RemoteEvent("atmMakeWithdraw")]
        public void OnMakeWithdraw(Client user, string amount, string concept)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MakeWithdraw(player, amount, concept);
            }
        }

        /// <summary>
        /// Triggered when a player commits a transfer
        /// </summary>
        /// <param name="user">The client instance</param>
        /// <param name="otherAccount">The other account number</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="concept">A brief concept</param>
        [RemoteEvent("atmMakeTransfer")]
        public void OnMakeTransfer(Client user, string otherAccount, string amount, string concept)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MakeTransfer(player, otherAccount, amount, concept);
            }
        }

        /// <summary>
        /// Triggered when a player confirms a deposit
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmConfirmDeposit")]
        public void OnConfirmDeposit(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                ConfirmDeposit(player);
            }
        }

        /// <summary>
        /// Triggered when a player confirms a withdraw
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmConfirmWithdraw")]
        public void OnConfirmWithdraw(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                ConfirmWithdraw(player);
            }
        }

        /// <summary>
        /// Triggered when a player confirms a transfer
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmConfirmTransfer")]
        public void OnConfirmTransfer(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                ConfirmTransfer(player);
            }
        }

        /// <summary>
        /// Triggered when a player cancels the current operation
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("atmCancelOperation")]
        public void OnCancelOperation(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                CancelOperation(player);
            }
        }

        /// <summary>
        /// Triggered when player closes the ATM interface
        /// </summary>
        /// <param name="user"></param>
        [RemoteEvent("atmClose")]
        public void OnClose(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                StopSession(player);
            }
        }

        /// <summary>
        /// Loads all ATM instances from database
        /// </summary>
        /// <returns>True if successful, false otherwise</returns>
        public static bool Load()
        {
            try
            {
                if(Global.ATMs != null)
                {
                    foreach (ATM atm in Global.ATMs)
                    {
                        if(atm.hasBlip)
                        {
                            NAPI.Entity.DeleteEntity(atm.blip);
                        }
                    }

                    Global.ATMs.Clear();
                }
                else
                {
                    // New ATM list
                    Global.ATMs = new List<ATM>();
                }
                
                // Instantiate a new disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // If success, we instantiate a new disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Sets the command query text
                        tempCmd.CommandText = "SELECT * FROM cajeros";
                        // Then we create the mysql reader disposable object
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {

                            // We read all the result 
                            while (tempReader.Read())
                            {
                                // New ATM
                                ATM newATM = new ATM();

                                // Initialization of ATM data
                                newATM.sqlid = tempReader.GetInt32("id");
                                newATM.position = new Vector3(tempReader.GetDouble("x"), tempReader.GetDouble("y"), tempReader.GetDouble("z"));
                                newATM.heading = tempReader.GetDouble("angle");
                                newATM.dimension = tempReader.GetUInt32("dimension");
                                newATM.cash = tempReader.GetInt32("cash");
                                newATM.bank = tempReader.GetInt32("bank");
                                newATM.hasBlip = tempReader.GetBoolean("hasBlip");

                                newATM.isInUse = false;

                                if (newATM.hasBlip)
                                {
                                    string bankName;
                                    byte color = 4;
                                    switch (newATM.bank)
                                    {
                                        case -1: color = 4; break;
                                        case 0: color = 2; break;
                                        case 1: color = 1; break;
                                        case 2: color = 3; break;
                                        case 3: color = 6; break;
                                    }

                                    switch(newATM.bank)
                                    {
                                        case -1: bankName = "libre"; break;
                                        case 0: bankName = "Fleeca"; break;
                                        case 1: bankName = "Maze Bank"; break;
                                        case 2: bankName = "Pacific Standard"; break;
                                        case 3: bankName = "Lombank"; break;
                                        default: bankName = "no definido"; break;
                                    }

                                    newATM.blip = NAPI.Blip.CreateBlip(207, newATM.position, 0.5f, color, "Cajero automático " + bankName, 255, 0, true, 0, newATM.dimension);
                                }

                                // Adding new ATM to the global list
                                Global.ATMs.Add(newATM);
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
        /// Create a new ATM at the player's position
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="bank">The bank business id</param>
        /// <param name="hasBlip">If has map blip visible or not</param>
        /// <returns>true if success, false otherwise</returns>
        public static bool Create(Player player, int bank, bool hasBlip)
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
                        tempCmd.CommandText = "INSERT INTO ATMs (x, y, z, heading, dimension, cash, bank, hasBlip) " +
                            "VALUES (@x, @y, @z, @heading, @dimension, @cash, @bank, @hasBlip)";
                        tempCmd.Parameters.AddWithValue("@x", player.user.Position.X);
                        tempCmd.Parameters.AddWithValue("@y", player.user.Position.Y);
                        tempCmd.Parameters.AddWithValue("@z", player.user.Position.Z);
                        tempCmd.Parameters.AddWithValue("@heading", player.user.Heading);
                        tempCmd.Parameters.AddWithValue("@dimension", player.user.Dimension);
                        tempCmd.Parameters.AddWithValue("@cash", 0);
                        tempCmd.Parameters.AddWithValue("@bank", bank);
                        tempCmd.Parameters.AddWithValue("@hasBlip", hasBlip);

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            int newATMId = (int)tempCmd.LastInsertedId;
                            Log.Debug("[BANKING] Creado cajero automático num " + newATMId + ", bank: " + bank + ", hasBlip: " + hasBlip);

                            return true;
                        }
                        else
                        {
                            Log.Debug("[BANKING] No se pudo crear el cajero automático.");
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
        /// Gets the nearest ATM
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="distance">The maximum distance</param>
        /// <returns>The ATM instance
        /// </returns>
        public static ATM GetNearestAvailable(Player player, double distance)
        {
            // Define a null pointer
            ATM atmFound = null;

            // Iterate all ATMs
            foreach (ATM atm in Global.ATMs)
            {
                // If the ATM is in range
                if (atm.position.DistanceTo(player.user.Position) <= distance)
                {
                    // Player is facing the ATM
                    if (player.user.Heading <= (atm.heading + 20.0) && player.user.Heading >= (atm.heading - 20.0))
                    {
                        // And ATM is not in use (by another player)
                        if (!atm.isInUse)
                        {
                            // We get the instance pointer
                            atmFound = atm;
                            break;
                        }
                    }
                }
            }

            return atmFound;
        }

        /// <summary>
        /// Starts an ATM session
        /// </summary>
        /// <param name="player">The client instance</param>
        public void StartSession(Player player)
        {
            Inventory playerInventory = player.character.inventory;
            List<string> bankAccountsData = new List<string>();
            List<Item> cards = playerInventory.HasItems(129);
            foreach (Item card in cards)
            {
                if(card.miscData.Count() > 0)
                {
                    if (!bankAccountsData.Contains(card.miscData[0]))
                    {
                        BankAccount account = BankAccount.GetAccountByNumber(card.miscData[0]);

                        if (account != null)
                        {
                            if(account.bank == this.bank || this.bank == -1)
                            {
                                bankAccountsData.Add(account.accountNumber);
                            }
                        }
                    }
                }
            }

            if(bankAccountsData.Count != 0)
            {

                // We set the animation depending on the character's sex.
                if (player.character.sex == 0)
                {
                    NAPI.Player.PlayPlayerAnimation(player.user, (int)Animation.Flags.Loop, "amb@prop_human_atm@male@idle_a", "idle_b");
                }
                else
                {
                    NAPI.Player.PlayPlayerAnimation(player.user, (int)Animation.Flags.Loop, "amb@prop_human_atm@female@idle_a", "idle_a");
                }

                player.forcedAnimation = true;

                // Freeze the player
                NAPI.Player.FreezePlayer(player.user, true);

                // Initialize the ATM interface
                player.showingGui = (int)GuiController.GuiTypes.AtmInterface;
                player.showingGuiId = 0;
                NAPI.ClientEvent.TriggerClientEvent(player.user, "showAtmInterface", this.bank, NAPI.Util.ToJson(bankAccountsData));

                // We set the ATM in use
                this.isInUse = true;
                player.character.usingATM = this;
            }
            else
            {
                player.DisplayNotification("~r~No tienes tarjetas de éste banco. Busca otro cajero.");
            }
        }

        /// <summary>
        /// Stops an ATM session
        /// </summary>
        /// <param name="player">The client instance</param>
        public void StopSession(Player player)
        {

            // Release the ATM
            player.character.usingATM.isInUse = false;
            player.character.usingATM = null;

            // Finish animation
            if (player.character.sex == 0)
            {
                NAPI.Player.PlayPlayerAnimation(player.user, 0, "amb@prop_human_atm@male@exit", "exit");
            }
            else
            {
                NAPI.Player.PlayPlayerAnimation(player.user, 0, "amb@prop_human_atm@female@exit", "exit");
            }

            player.forcedAnimation = false;

            // Unfreeze the player
            NAPI.Player.FreezePlayer(player.user, false);

            // Release other ATM player data

            // Close the ATM interface
            player.showingGui = -1;
            player.showingGuiId = -1;
            NAPI.ClientEvent.TriggerClientEvent(player.user, "hideAtmInterface");
        }

        /// <summary>
        /// Selects a card
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="accountNumber">The card account number</param>
        public void SelectCard(Player player, string accountNumber)
        {
            bool hasValidCard = false;
            Inventory playerInventory = player.character.inventory;
            List<Item> cards = playerInventory.HasItems(129);
            foreach (Item card in cards)
            {
                if (card.miscData.Count() > 0)
                {
                    if(card.miscData[0] == accountNumber)
                    {
                        hasValidCard = true;
                    }
                }
            }

            if (hasValidCard)
            {
                BankAccount account = BankAccount.GetAccountByNumber(accountNumber);
                if (account != null)
                {
                    player.character.usingAccountATM = account;
                    player.character.authenticatedATM = false;

                    NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 1, "", "");
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ cuenta bancaria no disponible.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ tarjeta no válida.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Checks the card's PIN
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="pin">The inserted PIN</param>
        public void CheckPIN(Player player, string pin)
        {
            Item usedCard = null;
            Inventory playerInventory = player.character.inventory;
            List<Item> cards = playerInventory.HasItems(129);
            foreach (Item card in cards)
            {
                if (card.miscData.Count() > 0)
                {
                    if (card.miscData[0] == player.character.usingAccountATM.accountNumber)
                    {
                        usedCard = card;
                    }
                }
            }

            if (pin == usedCard.miscData[1])
            {
                usedCard.miscData[2] = "0";

                player.character.authenticatedATM = true;

                LoadMainMenu(player);
            }
            else
            {
                int attempts = int.Parse(usedCard.miscData[2]);
                if(attempts + 1 > 2)
                {
                    usedCard.miscData.Clear();
                    StopSession(player);
                    player.DisplayNotification("~g~Voz cajero:~b~ PIN incorrecto, tarjeta bloqueada por exceso de intentos.");
                }
                else
                {
                    attempts++;
                    usedCard.miscData[2] = attempts.ToString();
                    player.DisplayNotification("~g~Voz cajero:~b~ el PIN introducido es incorrecto.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
        }

        /// <summary>
        /// Load and shows the main menu
        /// </summary>
        /// <param name="player">The client instance</param>
        public void LoadMainMenu(Player player)
        {
            // Process player bank account's data.
            List<string> bankAccountData = new List<string>();

            bankAccountData.Add(player.character.usingAccountATM.accountNumber);
            bankAccountData.Add(player.character.usingAccountATM.GetTypeAsString());
            bankAccountData.Add(player.character.usingAccountATM.GetHolder());
            bankAccountData.Add(player.character.usingAccountATM.cash.ToString() + " USD");
            bankAccountData.Add(player.character.usingAccountATM.debt.ToString() + " USD");
            bankAccountData.Add(player.character.usingAccountATM.GetLastMovement());

            NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 2, NAPI.Util.ToJson(bankAccountData), false);
        }

        /// <summary>
        /// Open the deposit form
        /// </summary>
        /// <param name="player">The client instance</param>
        public void OpenDeposit(Player player)
        {
            player.character.operationATM = (int)Global.AccountMovementType.Deposit;

            NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 3, "", "");
        }

        /// <summary>
        /// Open the withdraw form
        /// </summary>
        /// <param name="player">The client instance</param>
        public void OpenWithdraw(Player player)
        {
            if (!player.character.usingAccountATM.HasDebt())
            {
                if (!player.character.usingAccountATM.IsLocked())
                {
                    player.character.operationATM = (int)Global.AccountMovementType.Withdraw;

                    NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 4, player.character.usingAccountATM.cash + " USD", "");
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ cuenta a plazo fijo bloqueada.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ cuenta bloqueada por deuda.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Open the transfer form
        /// </summary>
        /// <param name="player">The client instance</param>
        public void OpenTransfer(Player player)
        {
            if(!player.character.usingAccountATM.HasDebt())
            {
                if (!player.character.usingAccountATM.IsLocked())
                {
                    player.character.operationATM = (int)Global.AccountMovementType.Transfer;

                    NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 5, player.character.usingAccountATM.cash + " USD", "");
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ cuenta a plazo fijo bloqueada.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ cuenta bloqueada por deuda.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Open the history page
        /// </summary>
        /// <param name="player">The client instance</param>
        public void OpenHistory(Player player)
        {
            if(player.character.usingAccountATM.GetLastMovement() != "Nunca")
            {
                string historyData = player.character.usingAccountATM.GetLastMovements(Global.AtmMaxHistoryRegisters);
                NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 9, historyData, "");
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ aún no hay movimientos registrados.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Make a deposit
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="amount">The deposit amount</param>
        /// <param name="concept">The concept</param>
        public void MakeDeposit(Player player, string amount, string concept)
        {
            // Check if amount is a number
            if(Util.IsNumeric(amount))
            {
                // Cast amount to integer
                int previewAmount = int.Parse(amount);
                // Check if amount is positive
                if(previewAmount > 0)
                {
                    // Check if player have enough money
                    if(player.character.money >= previewAmount)
                    {
                        // Set operation data
                        List<string> operationData = new List<string>();

                        operationData.Add(amount);
                        operationData.Add(concept);

                        player.character.operationDataATM = operationData;

                        // Put all necessary data to an string list
                        List<string> sendOperationData = new List<string>();

                        sendOperationData.Add(player.character.usingAccountATM.accountNumber);
                        sendOperationData.Add(player.character.usingAccountATM.cash.ToString() + " USD");
                        sendOperationData.Add(player.character.usingAccountATM.debt.ToString() + " USD");
                        sendOperationData.Add(amount + " USD");

                        // If account has a debt, calculate the remaining money.
                        if (player.character.usingAccountATM.HasDebt())
                        {
                            int remainingMoney = 0;
                            if (player.character.usingAccountATM.debt >= previewAmount)
                            {
                                sendOperationData.Add("0 USD");
                            }
                            else
                            {
                                remainingMoney = Math.Abs(player.character.usingAccountATM.debt - previewAmount);
                                sendOperationData.Add(remainingMoney.ToString() + " USD");
                            }
                        }
                        else
                        {
                            sendOperationData.Add((player.character.usingAccountATM.cash + previewAmount).ToString() + " USD");
                        }
                        
                        sendOperationData.Add(concept);

                        // Send all the data to the client
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 6, NAPI.Util.ToJson(sendOperationData), "");
                    }
                    else
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ importe introducido insuficiente.");
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                    }
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ el importe debe ser positivo.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ el importe introducido no es válido.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Make a withdraw
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="amount">The withdraw amount</param>
        /// <param name="concept">The concept</param>
        public void MakeWithdraw(Player player, string amount, string concept)
        {
            // Check if amount is a number
            if (Util.IsNumeric(amount))
            {
                // Cast amount to integer
                int previewAmount = int.Parse(amount);
                // Check if amount is positive
                if (previewAmount > 0)
                {
                    // Check if account have enough money
                    if (player.character.usingAccountATM.cash >= previewAmount)
                    {
                        // Set operation data
                        List<string> operationData = new List<string>();

                        operationData.Add(amount);
                        operationData.Add(concept);

                        player.character.operationDataATM = operationData;

                        // Put all necessary data to an string list
                        List<string> sendOperationData = new List<string>();
                        int comission = BankAccount.CalculateWithdrawComission(player.character.usingAccountATM.accountType, previewAmount);

                        sendOperationData.Add(player.character.usingAccountATM.accountNumber);
                        sendOperationData.Add(player.character.usingAccountATM.GetTypeAsString());
                        sendOperationData.Add(player.character.usingAccountATM.cash.ToString() + " USD");
                        sendOperationData.Add(player.character.usingAccountATM.GetWithdrawComissionType().ToString() + "%");
                        sendOperationData.Add(comission.ToString() + " USD");
                        sendOperationData.Add(amount + " USD");
                        sendOperationData.Add((player.character.usingAccountATM.cash - (previewAmount + comission)).ToString() + " USD");
                        sendOperationData.Add(concept);

                        // Send all the data to the client
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 7, NAPI.Util.ToJson(sendOperationData), "");
                    }
                    else
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ saldo insuficiente.");
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                    }
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ el importe debe ser positivo.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ el importe introducido no es válido.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Make a transfer
        /// </summary>
        /// <param name="player">The client instance</param>
        /// <param name="account">The target account</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="concept">The transfer concept</param>
        public void MakeTransfer(Player player, string account, string amount, string concept)
        {
            if(BankAccount.IsValidNumber(account))
            {
                BankAccount recipient = BankAccount.GetAccountByNumber(account);
                if(recipient != null)
                {
                    // Check if amount is a number
                    if (Util.IsNumeric(amount))
                    {
                        // Cast amount to integer
                        int previewAmount = int.Parse(amount);
                        // Check if amount is positive
                        if (previewAmount > 0)
                        {
                            // Check if account have enough money
                            if (player.character.usingAccountATM.cash >= previewAmount)
                            {
                                // Set operation data
                                List<string> operationData = new List<string>();

                                operationData.Add(account);
                                operationData.Add(amount);
                                operationData.Add(concept);

                                player.character.operationDataATM = operationData;

                                // Put all necessary data to an string list
                                List<string> sendOperationData = new List<string>();

                                bool sameBank = BankAccount.IsSameBank(player.character.usingAccountATM.accountNumber, account);
                                int comission = BankAccount.CalculateTransferComission(player.character.usingAccountATM.accountType, previewAmount, sameBank);

                                sendOperationData.Add(player.character.usingAccountATM.accountNumber);
                                sendOperationData.Add(account);
                                sendOperationData.Add(player.character.usingAccountATM.cash.ToString() + " USD");
                                sendOperationData.Add(player.character.usingAccountATM.GetTransferComissionType(sameBank).ToString() + "%");
                                sendOperationData.Add(comission.ToString() + " USD");
                                sendOperationData.Add(amount + " USD");
                                sendOperationData.Add((player.character.usingAccountATM.cash - (previewAmount + comission)).ToString() + " USD");
                                sendOperationData.Add(concept);

                                // Send all the data to the client
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "switchPageATMInterface", 8, NAPI.Util.ToJson(sendOperationData), "");
                            }
                            else
                            {
                                player.DisplayNotification("~g~Voz cajero:~b~ saldo insuficiente.");
                                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~g~Voz cajero:~b~ el importe debe ser positivo.");
                            NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ el importe introducido no es válido.");
                        NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                    }
                }
                else
                {
                    player.DisplayNotification("~g~Voz cajero:~b~ la cuenta destino no está disponible.");
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
                }
            }
            else
            {
                player.DisplayNotification("~g~Voz cajero:~b~ formato de cuenta destino inválido.");
                NAPI.ClientEvent.TriggerClientEvent(player.user, "activateButtonsAtmInterface");
            }
        }

        /// <summary>
        /// Confirm a deposit
        /// </summary>
        /// <param name="player">The client instance</param>
        public void ConfirmDeposit(Player player)
        {
            // Prepare all necessary data
            string number = player.character.usingAccountATM.accountNumber;
            int amount = int.Parse(player.character.operationDataATM[0]);
            string concept = player.character.operationDataATM[1];
            int atmId = player.character.usingATM.sqlid;
            // Try to make a deposit
            int result = BankAccount.MakeDeposit(player, number, amount, atmId, concept);

            // Giving a response depending on the operation result
            switch(result)
            {
                case -2:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ dinero insuficiente, operación cancelada.");
                        break;
                    }
                case -1:
                case 0:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ formato de cuenta destino inválido, operación cancelada.");
                        break;
                    }
                case 1:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ operación realizada correctamente.");
                        break;
                    }
            }

            // When operation is done, data operation is cleaned and player redirected to main menu.
            CancelOperation(player);
        }

        /// <summary>
        /// Confirm a withdraw
        /// </summary>
        /// <param name="player">The client instance</param>
        public void ConfirmWithdraw(Player player)
        {
            // Prepare all necessary data
            string number = player.character.usingAccountATM.accountNumber;
            int amount = int.Parse(player.character.operationDataATM[0]);
            string concept = player.character.operationDataATM[1];
            int atmId = player.character.usingATM.sqlid;
            // Try to make a deposit
            int result = BankAccount.MakeWithdraw(player, number, amount, atmId, concept);

            // Giving a response depending on the operation result
            switch (result)
            {
                case -4:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ cuantía excesiva, operación cancelada.");
                        break;
                    }
                case -3:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ cuantía límite alcanzada, operación cancelada.");
                        break;
                    }
                case -2:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ saldo insuficiente, operación cancelada.");
                        break;
                    }
                case -1:
                case 0:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ formato de cuenta inválido, operación cancelada.");
                        break;
                    }
                case 1:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ operación realizada correctamente.");
                        break;
                    }
            }

            // When operation is done, data operation is cleaned and player redirected to main menu.
            CancelOperation(player);
        }

        /// <summary>
        /// Confirm a transfer
        /// </summary>
        /// <param name="player">The client instance</param>
        public void ConfirmTransfer(Player player)
        {
            // Prepare all necessary data
            string number = player.character.usingAccountATM.accountNumber;
            string beneficiaryNumber = player.character.operationDataATM[0];
            int amount = int.Parse(player.character.operationDataATM[1]);
            string concept = player.character.operationDataATM[2];
            int atmId = player.character.usingATM.sqlid;
            // Try to make a deposit
            int result = BankAccount.MakeTransfer(number, beneficiaryNumber, amount, atmId, concept);

            // Giving a response depending on the operation result
            switch (result)
            {
                case -2:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ saldo insuficiente, operación cancelada.");
                        break;
                    }
                case -1:
                case 0:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ formato de cuenta destino inválido, operación cancelada.");
                        break;
                    }
                case 1:
                    {
                        player.DisplayNotification("~g~Voz cajero:~b~ operación realizada correctamente.");
                        break;
                    }
            }

            // When operation is done, data operation is cleaned and player redirected to main menu.
            CancelOperation(player);
        }

        /// <summary>
        /// Cancel the current operation and sends the player to the main menu
        /// </summary>
        /// <param name="player">The client instance</param>
        public void CancelOperation(Player player)
        {
            player.character.operationATM = -1;
            player.character.operationDataATM = null;

            LoadMainMenu(player);
        }

    }
}
