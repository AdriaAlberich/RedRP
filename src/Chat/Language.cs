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
    /// Language system
    /// </summary>
    public class Language : Script
    {

        // LANGUAGE ATTRIBUTES 

        /// <summary>
        /// Database id
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Language name
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// Abreviation 
        /// </summary>
        public string abreviation { get; set; }

        /// <summary>
        /// Talk world
        /// </summary>
        public string talk { get; set; }

        /// <summary>
        /// Whisper word
        /// </summary>
        public string whisper { get; set; }

        /// <summary>
        /// Shout word
        /// </summary>
        public string shout { get; set; }

        /// <summary>
        /// Load languages from database
        /// </summary>
        /// <returns>True if success, false if failure</returns>
        public static Boolean Load()
        {
            try
            {
                // New language list
                Global.Languages = new List<Language>();

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
                        tempCmd.CommandText = "SELECT * FROM Languages";
                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            while (tempReader.HasRows)
                            {
                                tempReader.Read();
                                Language newLanguage = new Language();
                                newLanguage.sqlid = tempReader.GetInt32("id");
                                newLanguage.fullName = tempReader.GetString("fullName");
                                newLanguage.abreviation = tempReader.GetString("abreviation");
                                newLanguage.talk = tempReader.GetString("talk");
                                newLanguage.whisper = tempReader.GetString("whisper");
                                newLanguage.shout = tempReader.GetString("shout");

                                // Add new language to the global list
                                Global.Languages.Add(newLanguage);
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
        /// Generates the language selection menu
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void GenerateSelectionMenu(Player player)
        {
            Menu menu = new Menu("", "Cambiar idioma", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnLanguageSelected));

            if (player.character.activeLanguage != null)
            {
                menu.menuModel.items.Add(new MenuItemModel("Inglés"));
            }
            else
            {
                menu.menuModel.items.Add(new MenuItemModel("~g~Inglés"));
            }

            foreach(Language lang in Global.Languages)
            {
                if(player.character.activeLanguage != null)
                {
                    if (player.character.activeLanguage.Equals(lang))
                    {
                        menu.menuModel.items.Add(new MenuItemModel("~g~" + lang.fullName));
                    }
                    else
                    {
                        menu.menuModel.items.Add(new MenuItemModel(lang.fullName));
                    }
                }
                else
                {
                    menu.menuModel.items.Add(new MenuItemModel(lang.fullName));
                }
            }
            menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Manages language menu responses
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="option">The selected option as string</param>
        /// <param name="actionId">The selected option</param>
        public static void OnLanguageSelected(Player player, string option, int actionId)
        {
            if (actionId == 0)
            {
                player.character.activeLanguage = null;
                NAPI.ClientEvent.TriggerClientEvent(player.user, "hudSetLanguage", "Inglés");
            }
            else
            {
                if (actionId > Global.Languages.Count())
                {
                    SelfInteraction.GenerateMenu(player);
                }
                else
                {
                    Language selectedLanguage = Global.Languages[actionId - 1];
                    player.character.activeLanguage = selectedLanguage;
                    NAPI.ClientEvent.TriggerClientEvent(player.user, "hudSetLanguage", selectedLanguage.fullName);
                }
            }
        }

    }

}
