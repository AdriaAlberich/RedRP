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


namespace redrp
{
    /// <summary>
    /// Main GUI controller
    /// </summary>
    public class GuiController : Script
    {
        /// <summary>
        /// Gui types
        /// </summary>
        public enum GuiTypes
        {
            Menu,
            Dialog,
            HelpPage,
            AdminHelpPage,
            AccountPage,
            PlayerListPage,
            AdminListPage,
            AtmInterface,
            InventoryInterface
        }

        /// <summary>
        /// Menu types
        /// </summary>
        public enum MenuType
        {
            ManageRightHandMenu,
            ManageLeftHandMenu,
            ManageWeaponMenu,
            PlayerLootingMenu,
            AddWeaponAccessoryMenu,
            RemoveWeaponAccessoryMenu,
            AddWeaponTintMenu,
            HelpMenu,
            LanguageMenu,
            AnimMenu,
            SelfInteractionMenu,
            ExternalInteractionMenu,
            AdminReportsMenu,
            AdminTeleportsMenu,
            TrashContainerMenu,
            SelectWeaponAnimationMenu,
            SelectMoodMenu,
            SelectWalkingStyleMenu
        }

        /// <summary>
        /// Dialog types
        /// </summary>
        public enum DialogType
        {
            TestDialog,
            PayToPlayerDialog,
            SendAdminReportDialog,
            ConfirmWeaponTintDialog
        }

        /// <summary>
        /// Triggered when a player selects a menu item
        /// </summary>
        /// <param name="callbackid">The menu id</param>
        /// <param name="itemSelected">The item selected</param>
        /// <param name="itemSelectedIndex">The item selected index</param>
        [RemoteEvent("onMenuItemSelected")]
        public void OnMenuItemSelected(Client user, string itemSelected, int itemSelectedIndex)
        {
            Player player = Player.Exists(user);
            if(player != null)
            {
                if(player.menu != null)
                {
                    MenuItemSelected(player, itemSelected, itemSelectedIndex);
                }
                /*
                switch (callbackId)
                {
                    // RIGHT HAND ITEM MANAGEMENT MENU RESPONSES
                    case (uint)MenuType.MANAGE_RIGHT_HAND_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onItemManagementMenuResponse(player, 1, itemSelected);
                            break;
                        }
                    // LEFT HAND ITEM MANAGEMENT MENU RESPONSES
                    case (uint)MenuID.MANAGE_LEFT_HAND_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onItemManagementMenuResponse(player, 0, itemSelected);
                            break;
                        }
                    // WEAPON MANAGEMENT MENU RESPONSES
                    case (uint)MenuID.MANAGE_WEAPON_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onWeaponManagementMenuResponse(player, itemSelected);
                            break;
                        }
                    // ADD WEAPON ACCESSORY MENU RESPONSES
                    case (uint)MenuID.ADD_WEAPON_ACCESSORY_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onAddWeaponComponentMenuResponse(player, itemSelected);
                            break;
                        }
                    // REMOVE WEAPON ACCESSORY MENU RESPONSES
                    case (uint)MenuID.REMOVE_WEAPON_ACCESSORY_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onRemoveWeaponComponentMenuResponse(player, itemSelected);
                            break;
                        }
                    // ADD WEAPON TINT MENU RESPONSES
                    case (uint)MenuID.ADD_WEAPON_TINT_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onAddWeaponTintMenuResponse(player, itemSelected);
                            break;
                        }
                    // HELP MENU RESPONSES
                    case (uint)MenuID.HELP_MENU:
                        {
                            destroyMenu(player);
                            switch (itemSelected)
                            {
                                //OPEN HELP PAGE
                                case 0:
                                    {
                                        player.showHelpPage();
                                        break;
                                    }
                                //OPEN ACCOUNT PAGE
                                case 1:
                                    {
                                        Player.showAccountInfoPage(player, player);
                                        break;
                                    }
                                //OPEN ADMIN LIST PAGE
                                case 2:
                                    {
                                        player.showAdminListPage();
                                        break;
                                    }
                                //OPEN SEND ADMIN REPORT DIALOG
                                case 3:
                                    {
                                        if (!AdminReport.hasReport(player))
                                        {
                                            createDialog(player, (uint)DialogID.SEND_ADMIN_REPORT_DIALOG, "Nuevo reporte administrativo", "Escribe tu reporte, trata de ser explícito con lo que quieres.", true, "Enviar", "Cancelar");
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Ya tienes un reporte abierto, espera que se solucione.");
                                        }
                                        break;
                                    }
                                //OPEN ADMIN HELP (FOR ADMINS)
                                case 4:
                                    {
                                        Admin.showAdminHelp(player);
                                        break;
                                    }
                                //OPEN REPORTS LIST (FOR ADMINS)
                                case 5:
                                    {
                                        if (AdminReport.Reports.Count > 0)
                                        {
                                            AdminReport.generateReportsMenu(player);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~g~No hay reportes pendientes.");
                                        }

                                        break;
                                    }
                                //OPEN LOCATIONS LIST (FOR ADMINS)
                                case 6:
                                    {
                                        AdminTeleport.generateSelectionMenu(player);

                                        break;
                                    }
                            }
                            break;
                        }
                    //LANGUAGE MENU RESPONSES
                    case (uint)MenuID.LANGUAGE_MENU:
                        {
                            destroyMenu(player);
                            if (itemSelected == 0)
                            {
                                player.character.activeLanguage = null;
                                API.triggerClientEvent(player.user, "hud_set_language", "Inglés");
                            }
                            else
                            {
                                if (itemSelected > Language.Languages.Count())
                                {
                                    SelfInteraction.generateMenu(player);
                                }
                                else
                                {
                                    Language selectedLanguage = Language.Languages[itemSelected - 1];
                                    player.character.activeLanguage = selectedLanguage;
                                    API.triggerClientEvent(player.user, "hud_set_language", selectedLanguage.fullName);
                                }
                            }

                            break;
                        }
                    //ANIM MENU RESPONSES
                    case (uint)MenuID.ANIM_MENU:
                        {
                            int idSelected = player.menuAnimationOptions[itemSelected];

                            destroyMenu(player);

                            //It's a category
                            if (idSelected <= 0)
                            {
                                if (idSelected == 0)
                                {
                                    if (player.menuAnimationCurrentCategory != null)
                                    {
                                        AnimationCategory.generateCategoryMenu(player.menuAnimationCurrentCategory.parent, player);
                                    }
                                    else
                                    {
                                        SelfInteraction.generateMenu(player);
                                    }
                                }
                                else
                                {
                                    AnimationCategory.generateCategoryMenu(-1 * idSelected, player);
                                }
                            }
                            else
                            {
                                if (!player.forcedAnimation)
                                {
                                    Animation anim = Animation.Animations[idSelected - 1];
                                    anim.start(player);
                                }
                            }

                            break;
                        }
                    //SELF INTERACTION MENU RESPONSES
                    case (uint)MenuID.SELF_INTERACTION_MENU:
                        {
                            destroyMenu(player);
                            SelfInteraction.onSelfInteraction(player, itemSelected);
                            break;
                        }
                    //EXTERNAL INTERACTION MENU RESPONSES
                    case (uint)MenuID.EXTERNAL_INTERACTION_MENU:
                        {
                            destroyMenu(player);
                            ExternalInteraction.onExternalInteraction(player, itemSelected);
                            break;
                        }
                    //REPORTS MENU SELECTED
                    case (uint)MenuID.ADMIN_REPORTS_MENU:
                        {
                            AdminReport report = AdminReport.Reports[itemSelected];
                            if (report != null)
                            {
                                if (report.admin == null)
                                {
                                    report.admin = player;
                                    Admin.notification(player.name + " ha atendido el reporte de " + report.reporter.character.cleanName + " (" + report.reporter.id + ").");
                                    API.sendChatMessageToPlayer(player.user, "~b~Reporte: " + report.reportText);
                                }
                                else
                                {
                                    AdminReport.Reports.Remove(report);
                                    player.DisplayNotification("~g~Reporte eliminado.");
                                }
                            }
                            else
                            {
                                player.DisplayNotification("~g~Este reporte ya fue eliminado.");
                            }

                            AdminReport.generateReportsMenu(player);

                            break;
                        }
                    //TELEPORTS MENU SELECTED
                    case (uint)MenuID.ADMIN_TELEPORTS_MENU:
                        {
                            AdminTeleport teleport = AdminTeleport.Teleports[itemSelected];
                            if (teleport != null)
                            {
                                teleport.teleport(player);
                            }
                            else
                            {
                                player.DisplayNotification("~r~Esta localización ya no existe.");
                            }

                            destroyMenu(player);

                            break;
                        }
                    //TRASH CONTAINER MENU SELECTED
                    case (uint)MenuID.TRASH_CONTAINER_MENU:
                        {
                            destroyMenu(player);
                            TrashContainer nearContainer = TrashContainer.getNearest(player, TrashContainer.DISTANCE_INTERACTION);
                            if (nearContainer != null)
                            {
                                nearContainer.onMenuResponse(player, itemSelected);
                            }

                            break;
                        }
                    //PLAYER LOOTING MENU SELECTED
                    case (uint)MenuID.PLAYER_LOOTING_MENU:
                        {
                            destroyMenu(player);
                            Inventory.onPlayerLootingMenuResponse(player, itemSelected);
                            break;
                        }
                    //PLAYER WEAPON ANIMATION MODE MENU SELECTED
                    case (uint)MenuID.SELECT_WEAPON_ANIMATION_MODE_MENU:
                        {
                            destroyMenu(player);
                            player.onWeaponAnimationModeMenuResponse(itemSelected);
                            break;
                        }
                    //PLAYER MOOD MENU SELECTED
                    case (uint)MenuID.SELECT_MOOD_MENU:
                        {
                            destroyMenu(player);
                            player.onMoodMenuResponse(itemSelected);
                            break;
                        }
                    //PLAYER MOVEMENT CLIPSET MENU SELECTED
                    case (uint)MenuID.SELECT_MOVEMENT_CLIPSET_MENU:
                        {
                            destroyMenu(player);
                            player.onMovementClipsetMenuResponse(itemSelected);
                            break;
                        }
                }
                break;
            }*/
            }
        }

        [RemoteEvent("onMenuIndexChange")]
        public void OnMenuIndexChange(Client user, int index)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MenuIndexChange(player, index);
            }
        }

        [RemoteEvent("onMenuItemCheckboxChange")]
        public void OnMenuItemCheckboxChange(Client user, string item, bool checked_)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MenuItemCheckboxChange(player, item, checked_);
            }
        }

        [RemoteEvent("onMenuItemListChange")]
        public void OnMenuItemListChange(Client user, string list, int listIndex, string listItem)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                MenuItemListChange(player, list, listIndex, listItem);
            }
        }

        [RemoteEvent("onDialogAccept")]
        public void OnDialogAccept(Client user, string input)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                DialogAccept(player, input);
            }
        }

        [RemoteEvent("onDialogCancel")]
        public void OnDialogCancel(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                DialogCancel(player);
            }
        }

        /*
        public void onClientTriggerGuiEvent(Client user, string eventName, params object[] arguments)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                switch(eventName)
                {
                    //DIALOG RESPONSES (ACCEPT)
                    case "dialog_accept":
                        {
                            uint callbackId = uint.Parse(arguments[0].ToString());
                            string input = "";
                            if (arguments.Length > 1)
                                input = arguments[1].ToString();

                            switch (callbackId)
                            {
                                //TEST DIALOG
                                case (uint)DialogID.TEST_DIALOG:
                                    {
                                        if (input.Length > 0)
                                        {
                                            API.shared.sendChatMessageToPlayer(player.user, "RESPUESTA DIALOGO: " + input);
                                        }
                                        else
                                        {
                                            API.shared.sendChatMessageToPlayer(player.user, "RESPUESTA DIALOGO VACIA");
                                        }


                                        destroyDialog(player);
                                        break;
                                    }
                                //PAY DIALOG
                                case (uint)DialogID.PAY_TO_PLAYER_DIALOG:
                                    {
                                        player.processPayToDialogResponse(input);

                                        destroyDialog(player);
                                        break;
                                    }
                                //SEND ADMIN REPORT DIALOG
                                case (uint)DialogID.SEND_ADMIN_REPORT_DIALOG:
                                    {

                                        if (input.Trim().Length > 2)
                                        {
                                            if (AdminReport.Reports.Count < AdminReport.MAX_REPORTS)
                                            {
                                                if (input.Trim().Length <= 64)
                                                {
                                                    AdminReport report = new AdminReport(player, input.Trim());
                                                    Admin.notification("Nuevo reporte de " + player.character.cleanName + " (" + player.id + "), /atender " + report.id + ".");
                                                    player.DisplayNotification("~b~Reporte enviado, espera a que te atiendan.");
                                                }
                                                else
                                                {
                                                    player.DisplayNotification("~r~Reporte demasiado largo (máximo 64 carácteres).");
                                                }
                                            }
                                            else
                                            {
                                                player.DisplayNotification("~r~Bandeja de reportes llena, espera que se vacíen.");
                                            }
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Elabora tu reporte porfavor.");
                                        }

                                        destroyDialog(player);

                                        break;
                                    }

                                //CONFIRM REMOVE WEAPON TINT
                                case (uint)DialogID.CONFIRM_REMOVE_WEAPON_TINT:
                                    {
                                        Inventory.onRemoveWeaponTintConfirmation(player);
                                        destroyDialog(player);
                                        break;
                                    }
                            }
                            break;
                        }
                    //DIALOG RESPONSES (CANCEL)
                    case "dialog_cancel":
                        {
                            destroyDialog(player);
                            break;
                        }
                }
            }
        }
        */
        
        /// <summary>
        /// Creates a NativeUI menu for the player
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="menu">The menu model</param>
        public static void CreateMenu(Player player, Menu menu)
        {
            ClearGui(player);
            NAPI.ClientEvent.TriggerClientEvent(player.user, "createMenu", NAPI.Util.ToJson(menu.menuModel));
            player.menu = menu;
            player.showingGui = (int)GuiTypes.Menu;
            player.showingGuiId = -1;
        }

        /// <summary>
        /// Destroys the current menu
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void DestroyMenu(Player player)
        {
            NAPI.ClientEvent.TriggerClientEvent(player.user, "destroyMenu");
            player.menu = null;
            player.showingGui = -1;
            player.showingGuiId = -1;
        }

        /// <summary>
        /// Calls the player's menu item selected delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="itemSelected">The item</param>
        /// <param name="itemSelectedIndex">The item index</param>
        public static void MenuItemSelected(Player player, string itemSelected, int itemSelectedIndex)
        {
            if(player.menu != null)
            {
                player.menu.onItemSelectedCallback(player, itemSelected, itemSelectedIndex);
            }
        }

        /// <summary>
        /// Calls the player's menu index change delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="index">The menu index</param>
        public static void MenuIndexChange(Player player, int index)
        {
            if (player.menu != null)
            {
                player.menu.onIndexChangeCallback(player, index);
            }
        }

        /// <summary>
        /// Calls the player's menu checkbox  change delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="item">The item</param>
        /// <param name="checked_">The new checked status</param>
        public static void MenuItemCheckboxChange(Player player, string item, bool checked_)
        {
            if (player.menu != null)
            {
                player.menu.onCheckboxChangeCallback(player, item, checked_);
            }
        }

        /// <summary>
        /// Calls the player's menu list change delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="list">The list</param>
        /// <param name="listIndex">The list index</param>
        /// <param name="listItem">The list item</param>
        public static void MenuItemListChange(Player player, string list, int listIndex, string listItem)
        {
            if (player.menu != null)
            {
                player.menu.onItemListChangeCallback(player, list, listIndex, listItem);
            }
        }

        /// <summary>
        /// Creates a new dialog for the player
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="dialog">The dialog data model</param>
        public static void CreateDialog(Player player, Dialog dialog)
        {
            ClearGui(player);
            NAPI.ClientEvent.TriggerClientEvent(player.user, "createDialog", NAPI.Util.ToJson(dialog.dialogModel));
            player.dialog = dialog;
            player.showingGui = (int)GuiTypes.Dialog;
            player.showingGuiId = -1;
        }

        /// <summary>
        /// Destroys the player dialog
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void DestroyDialog(Player player)
        {
            NAPI.ClientEvent.TriggerClientEvent(player.user, "destroyDialog");
            player.dialog = null;
            player.showingGui = -1;
            player.showingGuiId = -1;
        }

        /// <summary>
        /// Calls the player's dialog accept delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="input">The dialog input</param>
        public static void DialogAccept(Player player, string input)
        {
            if(player.dialog != null)
            {
                player.dialog.onAcceptCallback(player, input);
            }
        }

        /// <summary>
        /// Calls the player's dialog cancel delegate
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void DialogCancel(Player player)
        {
            if (player.dialog != null)
            {
                player.dialog.onCancelCallback(player);
            }
        }

        /// <summary>
        /// Destroys the current Gui element
        /// </summary>
        /// <param name="player">The player instance</param>
        public static void ClearGui(Player player)
        {
            switch(player.showingGui)
            {
                case (int)GuiTypes.Menu:
                    {
                        DestroyMenu(player);
                        break;
                    }
                case (int)GuiTypes.Dialog:
                    {
                        DestroyDialog(player);
                        break;
                    }
                case (int)GuiTypes.HelpPage:
                    {
                        player.HideHelpPage();
                        break;
                    }
                case (int)GuiTypes.AdminHelpPage:
                    {
                        Admin.HideAdminHelp(player);
                        break;
                    }
                case (int)GuiTypes.AccountPage:
                    {
                        player.HideAccountInfoPage();
                        break;
                    }
                case (int)GuiTypes.PlayerListPage:
                    {
                        player.HidePlayerListPage();
                        break;
                    }
                case (int)GuiTypes.AdminListPage:
                    {
                        player.HideAdminListPage();
                        break;
                    }
                case (int)GuiTypes.AtmInterface:
                    {
                        player.character.usingATM.StopSession(player);
                        break;
                    }
                case (int)GuiTypes.InventoryInterface:
                    {
                        Inventory.CloseInventoryForPlayer(player);
                        break;
                    }
            }
        }

    }
}
