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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace redrp
{
    /// <summary>
    /// Bank account class and banking system
    /// </summary>
    public class BankAccount : Script
    {

        // BANK ACCOUNT ATTRIBUTES

        /// <summary>
        /// Database id
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Account number
        /// </summary>
        public string accountNumber { get; set; }

        /// <summary>
        /// Account owner id
        /// </summary>
        public int ownerId { get; set; }

        /// <summary>
        /// Account owner type
        /// </summary>
        public int ownerType { get; set; }

        /// <summary>
        /// Bank id
        /// </summary>
        public int bank { get; set; }

        /// <summary>
        /// Account type
        /// </summary>
        public int accountType { get; set; }

        /// <summary>
        /// If is primary account
        /// </summary>
        public int isPrimaryAccount { get; set; }

        /// <summary>
        /// Account available cash
        /// </summary>
        public int cash { get; set; }

        /// <summary>
        /// Account acumulated debt
        /// </summary>
        public int debt { get; set; }

        /// <summary>
        /// Amount of cash withdrawn from ATM on the last hour.
        /// </summary>
        public int withdrawnCash { get; set; }

        /// <summary>
        /// Last withraw movement timestamp
        /// </summary>
        public int withdrawnCashTimestamp { get; set; }

        /// <summary>
        /// If account is blocked and until when
        /// </summary>
        public int lockedUntil { get; set; }

        /// <summary>
        /// Loads a bank account from database
        /// </summary>
        /// <param name="id">The account database id</param>
        /// <returns>The bank account instance</returns>
        public static BankAccount Load(int id)
        {
            try
            {
                if (!IsLoaded(id))
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
                            tempCmd.CommandText = "SELECT * FROM BankAccount WHERE id = @id";
                            tempCmd.Parameters.AddWithValue("@id", id);

                            // Reader
                            using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                            {
                                if (tempReader.HasRows)
                                {
                                    tempReader.Read();

                                    // New account instance
                                    BankAccount newAccount = new BankAccount();

                                    newAccount.sqlid = tempReader.GetInt32("id");
                                    newAccount.accountNumber = tempReader.GetString("number");
                                    newAccount.ownerId = tempReader.GetInt32("ownerId");
                                    newAccount.ownerType = tempReader.GetInt32("ownerType");
                                    newAccount.bank = tempReader.GetInt32("bank");
                                    newAccount.accountType = tempReader.GetInt32("accountType");
                                    newAccount.isPrimaryAccount = tempReader.GetInt32("isPrimary");
                                    newAccount.cash = tempReader.GetInt32("cash");
                                    newAccount.debt = tempReader.GetInt32("debt");
                                    newAccount.withdrawnCash = tempReader.GetInt32("withdrawnCash");
                                    newAccount.withdrawnCashTimestamp = tempReader.GetInt32("withdrawnCashTimestamp");
                                    newAccount.lockedUntil = tempReader.GetInt32("lockedUntil");

                                    return newAccount;
                                }

                            }

                        }

                    }  
                }
                else
                {
                    return FindById(id);
                }

                return null;

            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return null;
            }

        }

        /// <summary>
        /// Creates a new bank account for the specified owner
        /// </summary>
        /// <param name="ownerId">Owner database id</param>
        /// <param name="ownerType">Owner type</param>
        /// <param name="bank">The bank id</param>
        /// <param name="accountType">The account type</param>
        /// <param name="isPrimary">If its primary or not</param>
        /// <returns>The bank account instance or null</returns>
        public static BankAccount Create(int ownerId, int ownerType, int bank, int accountType, bool isPrimary)
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
                        tempCmd.CommandText = "INSERT INTO BankAccounts (ownerId, ownerType, bank, accountType, isPrimary) " +
                            "VALUES (@ownerId, @ownerType, @bank, @accountType, @isPrimary)";
                        tempCmd.Parameters.AddWithValue("@ownerId", ownerId);
                        tempCmd.Parameters.AddWithValue("@ownerType", ownerType);
                        tempCmd.Parameters.AddWithValue("@bank", bank);
                        tempCmd.Parameters.AddWithValue("@accountType", accountType);
                        tempCmd.Parameters.AddWithValue("@isPrimary", isPrimary);

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            int newAccountId = (int)tempCmd.LastInsertedId;
                            Log.Debug("[BANKING] Creada cuenta bancaria num " + newAccountId + ", prop: " + ownerId + ", tipo prop: " + ownerType);

                            // Try to load the new account
                            BankAccount newAccount = Load(newAccountId);

                            // Generate the account number
                            newAccount.GenerateNumber();
                            Util.UpdateDBField("BankAccounts", "number", newAccount.accountNumber.ToString(), "id", newAccount.sqlid.ToString());
                            Log.Debug("[BANKING] Generado numero de cuenta " + newAccount.accountNumber);

                            return newAccount;
                        }
                        else
                        {
                            Log.Debug("[BANKING] Error al crear cuenta bancaria para " + ownerId + " " + ownerType);
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
        /// Makes a deposit to the specified account
        /// </summary>
        /// <param name="player">The player who makes the deposit</param>
        /// <param name="number">The target account number</param>
        /// <param name="amount">The amount of cash</param>
        /// <param name="concept">The deposit concept</param>
        /// <param name="atm">The ATM id</param>
        /// <returns>1 if ok, 0 if account is not valid, -1 if account does not exist, -2 if deposit failed</returns>
        public static int MakeDeposit(Player player, string number, int amount, int atm, string concept)
        {
            // First we check if account number is valid
            if(IsValidNumber(number))
            {
                // Try to find a loaded account with the same ID
                BankAccount loadedAccount = GetAccountByNumber(number);

                // If we find account already loaded
                if(loadedAccount != null)
                {
                    if (loadedAccount.InsertMovement((int)Global.AccountMovementType.Deposit, amount, atm, concept))
                    {
                        loadedAccount.Deposit(amount);

                        // Subtract the money from player
                        player.SetMoney(player.character.money - amount);
                        return 1;
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
        /// Makes a money withdraw from the specified account
        /// </summary>
        /// <param name="player">The player who makes the withdraw</param>
        /// <param name="number">The account number</param>
        /// <param name="amount">The withdraw amount</param>
        /// <param name="concept">The withdraw concept</param>
        /// <param name="atm">The ATM id</param>
        /// <returns>1 if ok, 0 if account is not valid, -1 if account does not Exists, -2 if there is not enough money, -3 if money withdraw limit is reached, -4 if the amount is excessive</returns>
        public static int MakeWithdraw(Player player, string number, int amount, int atm, string concept)
        {
            // First we check if the account number is valid
            if (IsValidNumber(number))
            {
                // Get the actual timestamp
                int currentTimestamp = (int)Util.GetCurrentTimestamp();

                // Try to get a loaded instance from the account ID
                BankAccount loadedAccount = GetAccountByNumber(number);

                // If account is already loaded
                if (loadedAccount != null)
                {
                    // Here we calculate the withdraw comission depending on the account type and amount
                    int comission = CalculateWithdrawComission(loadedAccount.accountType, amount);
                    // We check if there is enough money in the account
                    if ((amount + comission) <= loadedAccount.cash)
                    {
                        // Check if the amount is less than the per hour limitation
                        if (amount + loadedAccount.withdrawnCash < Global.MaxWithrawMoneyPerHour)
                        {
                            loadedAccount.Withdraw(amount + comission);
                        }
                        else
                        {
                            // Then we check if the time has already passed
                            if (currentTimestamp - loadedAccount.withdrawnCashTimestamp >= 3600)
                            {
                                // If so, check if amount is less than the limit
                                if (amount <= Global.MaxWithrawMoneyPerHour)
                                {
                                    loadedAccount.withdrawnCash = amount;
                                    loadedAccount.Withdraw(amount + comission);
                                }
                                else
                                {
                                    return -4;
                                }
                            }
                            else
                            {
                                return -3;
                            }
                        }

                        // If all goes well we save the last timestamp and the limits
                        loadedAccount.withdrawnCashTimestamp = currentTimestamp;
                        Util.UpdateDBField("BankAccounts", "withdrawnCash", loadedAccount.withdrawnCash.ToString(), "id", loadedAccount.sqlid.ToString());
                        Util.UpdateDBField("BankAccounts", "withdrawnCashTimestamp", loadedAccount.withdrawnCashTimestamp.ToString(), "id", loadedAccount.sqlid.ToString());
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

                // Update account status
                loadedAccount.lockedUntil = (int)Util.GetCurrentTimestamp() + (3600 * Global.blockingTime[loadedAccount.accountType]);
                Util.UpdateDBField("BankAccounts", "lockedUntil", loadedAccount.lockedUntil.ToString(), "id", loadedAccount.sqlid.ToString());

                // Insert movement to account history
                loadedAccount.InsertMovement((int)Global.AccountMovementType.Withdraw, amount * -1, atm, concept);

                // Finally we give the money to player
                player.SetMoney(player.character.money + amount);

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Makes a money transfer from one account to another
        /// </summary>
        /// <param name="senderAccountNumber">The sender account</param>
        /// <param name="beneficiaryAccountNumber">The destination account</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="concept">The transfer concept</param>
        /// <param name="atm">The ATM id</param>
        /// <returns>1 if ok, 0 if one or both accounts are not valid, -1 if one or both accounts don't exist, -2 if there is not enough money on the sender account</returns>
        public static int MakeTransfer(string senderAccountNumber, string beneficiaryAccountNumber, int amount, int atm, string concept)
        {
            // Check if both account numbers are valid
            if (IsValidNumber(senderAccountNumber) && IsValidNumber(beneficiaryAccountNumber))
            {

                // Try to get their instances
                BankAccount loadedSenderAccount = GetAccountByNumber(senderAccountNumber);
                BankAccount loadedBeneficiaryAccount = GetAccountByNumber(beneficiaryAccountNumber);

                // If the sender account is already loaded
                if (loadedSenderAccount != null)
                {
                    // If the beneficiary account is already loaded
                    if (loadedBeneficiaryAccount != null)
                    {
                        // Check if the bank is the same or not
                        bool sameBank = loadedSenderAccount.bank == loadedBeneficiaryAccount.bank;
                        // Calculate the transfer comission depending on the account type
                        int comission = CalculateTransferComission(loadedSenderAccount.accountType, amount, sameBank);

                        // Check if there is enough money on the sender account
                        if ((amount + comission) <= loadedSenderAccount.cash)
                        {
                            // If so, we make the transfer
                            loadedSenderAccount.transfer(loadedBeneficiaryAccount, amount, comission);
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
                    return -1;
                }

                // Update account status
                loadedSenderAccount.lockedUntil = (int)Util.GetCurrentTimestamp() + (3600 * Global.blockingTime[loadedSenderAccount.accountType]);
                Util.UpdateDBField("BankAccounts", "lockedUntil", loadedSenderAccount.lockedUntil.ToString(), "id", loadedSenderAccount.sqlid.ToString());

                // Register the movement to both accounts
                loadedSenderAccount.InsertMovement((int)Global.AccountMovementType.Transfer, amount * -1, atm, "Transferencia a " + beneficiaryAccountNumber + " - Concepto: " + concept);
                loadedBeneficiaryAccount.InsertMovement((int)Global.AccountMovementType.Transfer, amount, atm, "Transferencia de " + senderAccountNumber + " - Concepto: " + concept);

                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Makes a salary transfer to the target account
        /// </summary>
        /// <param name="account">The target account instance</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="businessOrOrganization">The business or organization name</param>
        /// <param name="concept">The concept</param>
        public static void MakeSalaryTransfer(BankAccount account, int amount, string businessOrOrganization, string concept)
        {
            // If there is a pending debt we calculate the remaining money
            if (account.debt > 0)
            {
                int remainingMoney = account.debt - amount;
                if (remainingMoney >= 0)
                {
                    account.debt = remainingMoney;
                }
                else
                {
                    // If there is remaining money then we deposit it on the account
                    account.debt = 0;
                    account.cash = Math.Abs(remainingMoney);
                }
            }
            else
            {
                // If there is no debt, we deposit the money directly
                account.cash += amount;
            }

            // Save the account data
            account.Save();

            account.InsertMovement((int)Global.AccountMovementType.Transfer, amount, -1, "Transferencia de " + businessOrOrganization + " - Concepto: " + concept);

        }

        /// <summary>
        /// Saves the account money values
        /// </summary>
        public void Save()
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
                        tempCmd.CommandText = "UPDATE BankAccounts SET cash = @cash, debt = @debt WHERE id = @id";
                        tempCmd.Parameters.AddWithValue("@cash", cash);
                        tempCmd.Parameters.AddWithValue("@debt", debt);
                        tempCmd.Parameters.AddWithValue("@id", sqlid);

                        tempCmd.ExecuteNonQuery();
                    }

                }

            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// Generates the account number
        /// </summary>
        public void GenerateNumber()
        {
            string routingNumber = new Random().Next(1, 9999).ToString("D4"); // Random number without use
            string checkNumber = this.bank.ToString(); // The bank id
            string accountNumber = this.sqlid.ToString("D6"); // The account id 
            
            string number = routingNumber + " " + checkNumber + " " + accountNumber;

            this.accountNumber = number;
        }

        /// <summary>
        /// Deposit some money to the account
        /// </summary>
        /// <param name="amount">The amount of cash</param>
        public void Deposit(int amount)
        {
            int remainingMoney = 0;
            if (this.debt > 0)
            {
                remainingMoney = this.debt - amount;
                if(remainingMoney >= 0)
                {
                    this.debt = remainingMoney;
                }
                else
                {
                    this.debt = 0;
                    this.cash = Math.Abs(remainingMoney);
                }
            }
            else
            {
                this.cash += amount;
            }

            this.Save();
        }

        /// <summary>
        /// Withdraw some money from the account
        /// </summary>
        /// <param name="amount">The amount of cash</param>
        public void Withdraw(int amount)
        {
            this.cash -= amount;
            this.Save();
        }

        /// <summary>
        /// Transfer some money from the account to another one
        /// </summary>
        /// <param name="beneficiary">The destination account</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="comission">The comission</param>
        public void transfer(BankAccount beneficiary, int amount, int comission)
        {

            if (beneficiary.debt > 0)
            {
                int remainingMoney = beneficiary.debt - amount;
                if (remainingMoney >= 0)
                {
                    beneficiary.debt = remainingMoney;
                }
                else
                {
                    beneficiary.debt = 0;
                    beneficiary.cash = Math.Abs(remainingMoney);
                }
            }
            else
            {
                beneficiary.cash += amount;
            }

            this.cash -= amount + comission;

            beneficiary.Save();
            this.Save();
        }

        /// <summary>
        /// Returns the account instance by its number
        /// </summary>
        /// <param name="number">The account number</param>
        /// <returns></returns>
        public static BankAccount GetAccountByNumber(string number)
        {
            return FindById(GetId(number));
        }

        /// <summary>
        /// Checks if the account is already loaded
        /// </summary>
        /// <param name="id">The account id</param>
        /// <returns>True if yes, false otherwise</returns>
        public static bool IsLoaded(int id)
        {
            foreach (BankAccount account in Global.BankAccounts)
            {
                if (account.sqlid == id)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the bank account instance by its id
        /// </summary>
        /// <param name="id">The account id</param>
        /// <returns>The account instance or null if not found</returns>
        public static BankAccount FindById(int id)
        {
            BankAccount accountFound = null;
            foreach(BankAccount account in Global.BankAccounts)
            {
                if(account.sqlid == id)
                {
                    accountFound = account;
                    break;
                }
            }

            if(accountFound == null)
            {
                accountFound = Load(id);
            }

            return accountFound;
        }

        /// <summary>
        /// Gets the account if from the account number
        /// </summary>
        /// <param name="number">The account number</param>
        /// <returns></returns>
        public static int GetId(string number)
        {
            try
            {
                return int.Parse(number.Split(' ')[2]);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// Checks if the account number is valid (correct format)
        /// </summary>
        /// <param name="number">The account number</param>
        /// <returns>True if yes, false otherwise</returns>
        public static bool IsValidNumber(string number)
        {
            try
            {
                Regex pattern = new Regex(@"^\d{4}\s\d{1}\s\d{6}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                return pattern.IsMatch(number);
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Returns the withdraw comission type
        /// </summary>
        /// <returns>The comission type</returns>
        public double GetWithdrawComissionType()
        {
            double comissionIndex = Global.baseWithdrawComission[accountType];

            return comissionIndex;
        }

        /// <summary>
        /// Calculates the withdraw comission amount
        /// </summary>
        /// <param name="accountType">The account type</param>
        /// <param name="amount">The withdraw amount</param>
        /// <returns>The withdraw comission amount</returns>
        public static int CalculateWithdrawComission(int accountType, int amount)
        {
            double comissionIndex = Global.baseWithdrawComission[accountType];

            if(comissionIndex != 0)
            {
                double comission = amount * comissionIndex / 100;

                int roundedComission = (int)Math.Ceiling(comission);

                return roundedComission;
            }

            return 0;
            
        }

        /// <summary>
        /// Returns the transfer comission type
        /// </summary>
        /// <param name="isSameBank">If the transfer is from the same bank</param>
        /// <returns>The comission type</returns>
        public double GetTransferComissionType(bool isSameBank)
        {
            double comissionIndex;
            if (isSameBank)
            {
                comissionIndex = Global.baseTransferComissionSameBank[accountType];
            }
            else
            {
                comissionIndex = Global.baseTransferComissionDifferentBank[accountType];
            }

            return comissionIndex;
        }

        /// <summary>
        /// Calculate the transfer comission amount
        /// </summary>
        /// <param name="accountType">The account type</param>
        /// <param name="amount">The transfer amount</param>
        /// <param name="isSameBank">If the accounts are from same bank</param>
        /// <returns>The transfer comission amount</returns>
        public static int CalculateTransferComission(int accountType, int amount, bool isSameBank)
        {
            double comissionIndex;
            if(isSameBank)
            {
                comissionIndex = Global.baseTransferComissionSameBank[accountType];
            }
            else
            {
                comissionIndex = Global.baseTransferComissionDifferentBank[accountType];
            }
            
            if(comissionIndex != 0)
            {
                double comission = amount * comissionIndex / 100;

                int roundedComission = (int)Math.Ceiling(comission);

                return roundedComission;
            }

            return 0;
            
        }

        /// <summary>
        /// Generates the account interests
        /// </summary>
        /// <returns>The interest amount</returns>
        public int GenerateInterests()
        {
            if(!HasDebt())
            {
                double interestIndex = Global.baseInterests[accountType];

                if (interestIndex > 0)
                {
                    double interest = cash * interestIndex / 100;

                    int roundedInterest = (int)Math.Ceiling(interest);

                    cash += roundedInterest;
                    Save();

                    return roundedInterest;
                }
            }

            return 0;
        }

        /// <summary>
        /// Generates the maintenance fee
        /// </summary>
        /// <returns>The fee amount</returns>
        public int GenerateMaintenanceFee()
        {
            int maintenanceFee = Global.baseMaintenanceFee[accountType];

            if (maintenanceFee > 0)
            {
                if(cash - maintenanceFee < 0)
                {
                    if(cash > 0)
                    {
                        int rest = maintenanceFee - cash;
                        cash = 0;
                        debt += rest;
                    }
                    else
                    {
                        cash = 0;
                        debt += maintenanceFee;
                    }
                }
                else
                {
                    cash -= maintenanceFee;
                }

                Save();

                return maintenanceFee;
            }

            return 0;
        }

        /// <summary>
        /// Checks if the account has debt or not
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool HasDebt()
        {
            if(debt > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the account type as string
        /// </summary>
        /// <returns>The account type</returns>
        public string GetTypeAsString()
        {
            string type = "";
            switch(accountType)
            {
                case (int)Global.AccountTypes.Checking: type = "Cuenta corriente"; break;
                case (int)Global.AccountTypes.Saving: type = "Cuenta ahorro"; break;
                case (int)Global.AccountTypes.Deposit: type = "Cuenta inversión"; break;
            }

            return type;
        }

        /// <summary>
        /// Returns the movement type as string
        /// </summary>
        /// <param name="typeId">The movement type id</param>
        /// <returns>The movement type</returns>
        public static string GetMovementTypeAsString(int typeId)
        {
            string type = "";
            switch (typeId)
            {
                case (int)Global.AccountMovementType.Deposit: type = "Deposito"; break;
                case (int)Global.AccountMovementType.Withdraw: type = "Retiro"; break;
                case (int)Global.AccountMovementType.Transfer: type = "Transferencia"; break;
            }

            return type;
        }

        /// <summary>
        /// Returns the last movement as string
        /// </summary>
        /// <returns>The last movement string</returns>
        public string GetLastMovement()
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
                        tempCmd.CommandText = "SELECT MAX(movementTimestamp) FROM BankAccountsHistory WHERE account = @account";
                        tempCmd.Parameters.AddWithValue("@account", accountNumber);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            if (tempReader.HasRows)
                            {
                                tempReader.Read();
                                int timestamp = tempReader.GetInt32("movementTimestamp");
                                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                dateTime = dateTime.AddSeconds(timestamp);
                                return dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
                            }
                            else
                            {
                                return "Nunca";
                            }
                        }
                    }

                }

            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return "Error";
            }
        }

        /// <summary>
        /// Returns the last history data as JSON
        /// </summary>
        /// <param name="limit">The limit of entries</param>
        /// <returns>The data as JSON</returns>
        public string GetLastMovements(int limit)
        {
            List<string[]> historyData = new List<string[]>();

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
                        tempCmd.CommandText = "SELECT movementType, amount, concept, movementTimestamp FROM BankAccountsHistory WHERE account = @account ORDER BY movementTimestamp LIMIT @limit";
                        tempCmd.Parameters.AddWithValue("@account", accountNumber);

                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            // Read all the result 
                            while (tempReader.Read())
                            {
                                string[] data = new string[4];

                                // Set the retrieved data to an array
                                data[1] = GetMovementTypeAsString(tempReader.GetInt32("movementType"));
                                data[2] = tempReader.GetInt32("amount").ToString() + " USD";
                                data[3] = tempReader.GetString("concept");
                                int timestamp = tempReader.GetInt32("movementTimestamp");
                                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                                dateTime = dateTime.AddSeconds(timestamp);

                                data[0] = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();

                                // Set the array data to the list
                                historyData.Add(data);
                            }

                            return NAPI.Util.ToJson(historyData);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return "Error";
            }
        }

        /// <summary>
        /// Inserts a new record on the account history
        /// </summary>
        /// <param name="movementType">Type of movement</param>
        /// <param name="amount">Amount of the movement</param>
        /// <param name="atm">The ATM id</param>
        /// <param name="concept">The movement concept</param>
        /// <returns>Returns true if successful, false otherwise</returns>
        public bool InsertMovement(int movementType, int amount, int atm, string concept)
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
                        tempCmd.CommandText = "INSERT INTO BankAccountsHistory (account, movementType, amount, atm, concept, movementTimestamp) " +
                            "VALUES (@account, @movementType, @amount, @atm, @concept, @movementTimestamp)";
                        tempCmd.Parameters.AddWithValue("@account", accountNumber);
                        tempCmd.Parameters.AddWithValue("@movementType", movementType);
                        tempCmd.Parameters.AddWithValue("@amount", amount);
                        tempCmd.Parameters.AddWithValue("@atm", atm);
                        tempCmd.Parameters.AddWithValue("@concept", concept);
                        tempCmd.Parameters.AddWithValue("@movementTimestamp", Util.GetCurrentTimestamp());

                        if (tempCmd.ExecuteNonQuery() > 0)
                        {
                            return true;
                        }
                        else
                        {
                            Log.Debug("[BANKING] Error al crear registro en historico bancario para cuenta " + accountNumber);
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
        /// Returns the account holder as string
        /// </summary>
        /// <returns>The account holder name</returns>
        public string GetHolder()
        {
            string holder = "Nadie";

            switch(this.ownerType)
            {
                case (int)Global.AccountOwnerTypes.Player:
                    {
                        holder = Util.GetDBField("Characters", "characterName", "id", ownerId.ToString());
                        break;
                    }
                case (int)Global.AccountOwnerTypes.Organization:
                    {
                        break;
                    }
                case (int)Global.AccountOwnerTypes.Business:
                    {
                        break;
                    }
            }

            return holder;
        }

        /// <summary>
        /// Check if the two account numbers are from the same bank
        /// </summary>
        /// <param name="account1">The first account number</param>
        /// <param name="account2">The second account number</param>
        /// <returns>True if yes, false otherwise</returns>
        public static bool IsSameBank(string account1, string account2)
        {
            try
            {
                return account1.Split(' ')[1] == account2.Split(' ')[1];
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Check if the account is from the same bank
        /// </summary>
        /// <param name="otherAccount">The other account instance</param>
        /// <returns>True if yes, false otherwise</returns>
        public bool SameBankAs(BankAccount otherAccount)
        {
            return bank == otherAccount.bank;
        }

        /// <summary>
        /// Check if account is locked
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsLocked()
        {
            return (int)Util.GetCurrentTimestamp() < lockedUntil;
        }


    }

}
