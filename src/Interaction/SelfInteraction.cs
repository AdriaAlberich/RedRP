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

namespace redrp
{
    /// <summary>
    /// Self interaction system
    /// </summary>
    public class SelfInteraction : Script
    {
        /// <summary>
        /// List of available actions
        /// </summary>
        public enum action
        {
            AcceptDeal,
            RefuseDeal,
            ManageRightHand,
            ManageLeftHand,
            ThrowCoin,
            ThrowDice,
            ChangeLanguage,
            AnimationMenu,
            Respawn,
            ChangeMood,
            ChangeWalkingStyle
        }

        /// <summary>
        /// Generates the self interaction menu for a player depending on his current conditions
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void GenerateMenu(Player player)
        {
            Menu menu = new Menu("", "Interacción propia", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnSelfInteraction));
            menu.miscData = new List<uint>();

            // Deal actions
            if(player.character.dealOwner != null)
            {
                menu.menuModel.items.Add(new MenuItemModel("~g~Aceptar trato", player.character.dealDescription));
                menu.miscData.Add((uint)action.AcceptDeal);

                menu.menuModel.items.Add(new MenuItemModel("~r~Rechazar trato", player.character.dealDescription));
                menu.miscData.Add((uint)action.RefuseDeal);
            }

            if (!player.user.IsInVehicle)
            {
                if (player.character.dying == 1 && player.character.dyingCounter == 0)
                {
                    // Respawn option
                    menu.menuModel.items.Add(new MenuItemModel("~g~Reaparecer", "Reapareces en el hospital"));
                    menu.miscData.Add((uint)action.Respawn);
                }
                else
                {
                    // Animations menu
                    menu.menuModel.items.Add(new MenuItemModel("Animaciones", "Abre el menú de animaciones"));
                    menu.miscData.Add((uint)action.AnimationMenu);
                }
            }

            // Inventory actions
            if (player.character.cuffed == 0 && player.character.dying == 0)
            {
                // Manage right hand item action
                if (player.character.inventory.rightHand != null || player.character.inventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user)) != null)
                {
                    menu.menuModel.items.Add(new MenuItemModel("Mano derecha >", "Realizar acción con el item"));
                    menu.miscData.Add((uint)action.ManageRightHand);
                }

                // Manage left hand item action
                if (player.character.inventory.leftHand != null)
                {
                    menu.menuModel.items.Add(new MenuItemModel("Mano izquierda >", "Realizar acción con el item"));
                    menu.miscData.Add((uint)action.ManageLeftHand);
                }

                // Throw a coin
                if (player.character.money > 0)
                {
                    menu.menuModel.items.Add(new MenuItemModel("Cara o cruz", "Jugar a cara o cruz"));
                    menu.miscData.Add((uint)action.ThrowCoin);
                }

                // Throw a dice
                if (player.character.inventory.HasItem(134) != null)
                {
                    menu.menuModel.items.Add(new MenuItemModel("Tirar el dado", "Jugar con el dado"));
                    menu.miscData.Add((uint)action.ThrowDice);
                }

                // Change language option
                menu.menuModel.items.Add(new MenuItemModel("Cambiar idioma", "Abre el menú de cambio de idioma"));
                menu.miscData.Add((uint)action.ChangeLanguage);

                // Change mood
                menu.menuModel.items.Add(new MenuItemModel("Cambiar estado de ánimo"));
                menu.miscData.Add((uint)action.ChangeMood);

                // Change movement clipset
                menu.menuModel.items.Add(new MenuItemModel("Cambiar estilo de caminar"));
                menu.miscData.Add((uint)action.ChangeWalkingStyle);

            }

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Manages the self interaction menu responses
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="option">The selected option as string</param>
        /// <param name="actionId">The action id</param>
        public static void OnSelfInteraction(Player player, string option, int actionId)
        {
            uint optionId = (uint)player.menu.miscData[actionId];
            switch(optionId)
            {
                // Accept deal
                case (uint)action.AcceptDeal:
                    {
                        player.AcceptDeal();
                        break;
                    }
                // Refuse deal
                case (uint)action.RefuseDeal:
                    {
                        player.RefuseDeal();
                        break;
                    }
                // Manage right item selected
                case (uint)action.ManageRightHand:
                    {
                        Inventory.OpenItemManagementMenu(player, 1);
                        break;
                    }
                // Manage left item selected
                case (uint)action.ManageLeftHand:
                    {
                        Inventory.OpenItemManagementMenu(player, 0);
                        break;
                    }
                // Throw coin selected
                case (uint)action.ThrowCoin:
                    {
                        player.ThrowCoin();
                        break;
                    }
                // Throw dice selected
                case (uint)action.ThrowDice:
                    {
                        player.ThrowDice();
                        break;
                    }
                // Language change selected
                case (uint)action.ChangeLanguage:
                    {
                        Language.GenerateSelectionMenu(player);
                        break;
                    }
                // Animation menu selected
                case (uint)action.AnimationMenu:
                    {
                        AnimationCategory.GenerateCategoryMenu(-1, player);
                        break;
                    }
                // Respawn option selected
                case (uint)action.Respawn:
                    {
                        player.Revive(true);
                        break;
                    }
                // Change mood
                case (uint)action.ChangeMood:
                    {
                        player.OpenMoodMenu();
                        break;
                    }
                // Change movement clipset
                case (uint)action.ChangeWalkingStyle:
                    {
                        player.OpenWalkingStyleMenu();
                        break;
                    }

            }
        }

    }
}
