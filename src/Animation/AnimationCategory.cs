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
    /// Animation category
    /// </summary>
    public class AnimationCategory : Script
    {

        // CATEGORY ATTRIBUTES

        /// <summary>
        /// Database id
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Category name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Parent category, -1 if has no parent
        /// </summary>
        public int parent { get; set; }

        /// <summary>
        /// Loads animation categories from the database
        /// </summary>
        /// <returns>True if success, false if failure</returns>
        public static Boolean Load()
        {
            try
            {
                // New category list
                Global.AnimationCategories = new List<AnimationCategory>();

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
                        tempCmd.CommandText = "SELECT * FROM AnimationCategories";
                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            while (tempReader.HasRows)
                            {
                                tempReader.Read();
                                AnimationCategory newCategory = new AnimationCategory();
                                newCategory.sqlid = tempReader.GetInt32("id");
                                newCategory.name = tempReader.GetString("categoryName");
                                newCategory.parent = tempReader.GetInt32("parent");

                                // Add new animation category to the global list
                                Global.AnimationCategories.Add(newCategory);
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
        /// Generates the category animation menu
        /// </summary>
        /// <param name="categoryId">The category id</param>
        /// <param name="player">The player instance</param>
        public static void GenerateCategoryMenu(int categoryId, Player player)
        {
            string title;
            AnimationCategory currentCategory;
            if(categoryId == -1)
            {
                title = "Animaciones";
                currentCategory = null;
            }
            else
            {
                title = Global.AnimationCategories[categoryId-1].name;
                currentCategory = Global.AnimationCategories[categoryId - 1];
            }

            Menu menu = new Menu("", "Interacción propia", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnAnimationSelected));
            menu.miscData = new List<int>();
            menu.miscData2 = currentCategory;

            foreach (AnimationCategory cat in Global.AnimationCategories)
            {
                if(cat.parent == categoryId)
                {
                    menu.menuModel.items.Add(new MenuItemModel("~b~" + cat.name));
                    menu.miscData.Add(-1 * cat.sqlid);
                }
            }
            
            foreach(Animation anim in Global.Animations)
            {
                if(anim.categoryId == categoryId)
                {
                    menu.menuModel.items.Add(new MenuItemModel(anim.animationName, "Comando: " + anim.command));
                    menu.miscData.Add(anim.sqlid);
                }
            }

            menu.menuModel.items.Add(new MenuItemModel("~r~< Volver atrás"));
            menu.miscData.Add(0);

            // Create the animations menu
            GuiController.CreateMenu(player, menu);
        }

        public static void OnAnimationSelected(Player player, string option, int actionId)
        {
            int action = player.menu.miscData[actionId];

            // It's a category
            if (action <= 0)
            {
                if (action == 0)
                {
                    if (player.menu.miscData2 != null)
                    {
                        int parentCategory = (int)player.menu.miscData2.parent;
                        GenerateCategoryMenu(parentCategory, player);
                    }
                    else
                    {
                        SelfInteraction.GenerateMenu(player);
                    }
                }
                else
                {
                    GenerateCategoryMenu(-1 * action, player);
                }
            }
            else
            {
                if (!player.forcedAnimation)
                {
                    Animation anim = Global.Animations[action - 1];
                    anim.start(player);
                }
            }
        }

    }

}
