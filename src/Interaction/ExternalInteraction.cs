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
    /// External interaction system
    /// </summary>
    public class ExternalInteraction : Script
    {

        /// <summary>
        /// List of available actions
        /// </summary>
        public enum action
        {
            Pay,
            GiveRightHandItem,
            GiveLeftHandItem,
            Loot,
            Search,
            Rob,
            SoftCuff,
            HardCuff,
            UnCuff
        }

        /// <summary>
        /// Gets the closest entity to the player in a certain radious.
        /// </summary>
        /// <param name="player">The player</param>
        public static void NearestTarget(Player player)
        {
            object target = null;
            if(player.character.cuffed == 0)
            {
                if (!player.user.IsInVehicle)
                {
                    Vector3 inFrontOf = player.GetInFrontOf(Global.ExternalInteractionPlayerVectorDistance);
                    // Search for players in front
                    foreach (Player playerTarget in Player.Players)
                    {
                        if (playerTarget.user.Position.DistanceTo(inFrontOf) <= Global.ExternalInteractionPlayerVectorDistance)
                        {
                            if (!player.Equals(playerTarget))
                            {
                                if (!playerTarget.user.IsInVehicle)
                                {
                                    if (player.user.Dimension == playerTarget.user.Dimension)
                                    {
                                        target = playerTarget;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (target == null)
                    {
                        // Search for players in radious
                        foreach (Player playerTarget in Player.Players)
                        {
                            if (playerTarget.user.Position.DistanceTo(player.user.Position) <= Global.ExternalInteractionPlayerDistance)
                            {
                                if (!player.Equals(playerTarget))
                                {
                                    if (!playerTarget.user.IsInVehicle)
                                    {
                                        if (player.user.Dimension == playerTarget.user.Dimension)
                                        {
                                            target = playerTarget;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (target != null)
                    {
                        GenerateMenu(player, target);
                    }
                    else
                    {

                        // ATM
                        ATM nearATM = ATM.GetNearestAvailable(player, Global.AtmInteractionDistance);
                        if (nearATM != null)
                        {
                            nearATM.StartSession(player);
                        }
                        else
                        {
                            // Garbage Containers
                            Container nearContainer = Container.GetNearest(player, Global.GarbageContainerInteractionDistance);
                            if (nearContainer != null)
                            {
                                nearContainer.ShowMenu(player);
                            }
                            else
                            {
                                // Other entities
                            }
                        }
                    }
                }
                else
                {
                    // Special options if player is inside a vehicle
                }
            }
        }

        /// <summary>
        /// Generates the self interaction menu for a player depending on his actual conditions
        /// </summary>
        /// <param name="player">The player</param>
        /// <param name="target">The target instance</param>
        public static void GenerateMenu(Player player, object target)
        {
            Menu menu = new Menu("", "", true, true, 0, 0, 6, "", 0, new Action<Player, string, int>(OnExternalInteraction));
            menu.miscData = new List<uint>();

            // If target is a player instance
            if (target is Player)
            {
                Player targetPlayer = (Player)target;
                menu.miscData2 = target;

                menu.menuModel.title = targetPlayer.character.showName;

                // Pay money
                menu.menuModel.items.Add(new MenuItemModel("Pagar", "Pagar dinero en efectivo"));
                menu.miscData.Add((uint)action.Pay);

                if (player.character.inventory.rightHand != null || player.character.inventory.GetWeaponItem(NAPI.Player.GetPlayerCurrentWeapon(player.user)) != null)
                {
                    // Give player right hand item or weapon
                    menu.menuModel.items.Add(new MenuItemModel("Ceder derecha", "Ceder el item de tu mano derecha"));
                    menu.miscData.Add((uint)action.GiveRightHandItem);
                }

                if (player.character.inventory.leftHand != null)
                {
                    // Give player left hand item or weapon
                    menu.menuModel.items.Add(new MenuItemModel("Ceder izquierda", "Ceder el item de tu mano izquierda"));
                    menu.miscData.Add((uint)action.GiveLeftHandItem);
                }

                // Loot/search/rob player
                if(targetPlayer.character.dying == 1)
                {
                    menu.menuModel.items.Add(new MenuItemModel("Saquear", "Saquear en busca de algo útil"));
                    menu.miscData.Add((uint)action.Loot);
                }
                else
                {
                    menu.menuModel.items.Add(new MenuItemModel("Cachear", "Revisar el inventario"));
                    menu.miscData.Add((uint)action.Search);

                    if (player.character.inventory.rightHand != null)
                    {
                        if (player.character.inventory.rightHand.IsWeapon())
                        {
                            menu.menuModel.items.Add(new MenuItemModel("Atracar", "Atracar al jugador"));
                            menu.miscData.Add((uint)action.Rob);
                        }
                    }
                }

                
                if(targetPlayer.character.cuffed == 0)
                {
                    // Soft cuff
                    if (player.character.inventory.HasItem(136) != null)
                    {
                        menu.menuModel.items.Add(new MenuItemModel("Atar manos", "Atar las manos del jugador"));
                        menu.miscData.Add((uint)action.SoftCuff);
                    }

                    // Hard cuff
                    if (player.character.inventory.HasItem(135) != null)
                    {
                        menu.menuModel.items.Add(new MenuItemModel("Poner esposas", "Esposar al jugador"));
                        menu.miscData.Add((uint)action.HardCuff);
                    }
                }
                else
                {
                    menu.menuModel.items.Add(new MenuItemModel("Liberar", "Liberas al jugador"));
                    menu.miscData.Add((uint)action.UnCuff);
                }
            }

            GuiController.CreateMenu(player, menu);
        }

        /// <summary>
        /// Processes all responses from external interaction menu options
        /// </summary>
        /// <param name="player"></param>
        /// <param name="option"></param>
        /// <param name="actionId"></param>
        public static void OnExternalInteraction(Player player, string option, int actionId)
        {
            uint optionId = player.menu.miscData[actionId];
            object target = player.menu.miscData2;

            if(target != null)
            {
                if (target is Player)
                {
                    Player playerTarget = (Player)target;
                    Vector3 inFrontOf = player.GetInFrontOf(Global.ExternalInteractionPlayerVectorDistance);
                    if (playerTarget.user.Position.DistanceTo(inFrontOf) <= Global.ExternalInteractionPlayerDistance && player.user.Dimension == playerTarget.user.Dimension)
                    {
                        switch (optionId)
                        {
                            // Pay to another player
                            case (uint)action.Pay:
                                {
                                    player.PayToDialog(playerTarget);
                                    break;
                                }
                            // Give player right hand item
                            case (uint)action.GiveRightHandItem:
                                {
                                    Inventory.GivePlayerItemToPlayerHand(player, playerTarget, true);
                                    break;
                                }
                            // Give player left hand item
                            case (uint)action.GiveLeftHandItem:
                                {
                                    Inventory.GivePlayerItemToPlayerHand(player, playerTarget, false);
                                    break;
                                }
                            // Open the loot menu
                            case (uint)action.Loot:
                                {
                                    Inventory.OpenPlayerLootingMenu(player, playerTarget);
                                    break;
                                }
                            // Start a search deal
                            case (uint)action.Search:
                                {
                                    player.SearchDeal(playerTarget);
                                    break;
                                }
                            // Start a rob deal
                            case (uint)action.Rob:
                                {
                                    player.RobDeal(playerTarget);
                                    break;
                                }
                            // Start a soft cuff deal
                            case (uint)action.SoftCuff:
                                {
                                    player.CuffDeal(playerTarget, true);
                                    break;
                                }
                            // Start a hard cuff deal
                            case (uint)action.HardCuff:
                                {
                                    player.CuffDeal(playerTarget, false);
                                    break;
                                }
                            // Uncuff player
                            case (uint)action.UnCuff:
                                {
                                    player.Uncuff(playerTarget);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Jugador demasiado lejos");
                    }
                }
            }
        }

    }
}
