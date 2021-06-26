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
    public class Keybinds : Script
    {

        /// <summary>
        /// Character selection keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("changeCharacterKeyPressed")]
        public void ChangeCharacterKeybind(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    player.ToCharacterSelection();
                }
            }
        }

        /// <summary>
        /// Change voice type keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("changeVoiceTypeKeyPressed")]
        public void ChangeVoiceType(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    player.ChangeVoiceType();
                }
            }
        }

        /// <summary>
        /// Close all GUI keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("closeAllGuiKeyPressed")]
        public void CloseAllGui(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                }
            }
        }

        /// <summary>
        /// Show help menu keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("showHelpMenuKeyPressed")]
        public void ShowHelpMenu(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                    player.ShowHelpMenu();
                }
            }
        }

        /// <summary>
        /// Show player list
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("showPlayerListKeyPressed")]
        public void ShowPlayerList(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                    player.ShowPlayerListPage();
                }
            }
        }

        /// <summary>
        /// Open / Close inventory
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("toggleInventoryKeyPressed")]
        public void ToggleInventory(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                    Inventory.OpenInventoryForPlayer(player);
                }
            }
        }

        /// <summary>
        /// Self interaction menu keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("showSelfInteractionMenuKeyPressed")]
        public void SelfInteractionMenu(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                    SelfInteraction.GenerateMenu(player);
                }
            }
        }

        /// <summary>
        /// Self interaction menu keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("showExternalInteractionMenuKeyPressed")]
        public void ExternalInteractionMenu(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    GuiController.ClearGui(player);
                    if (player.character.cuffed == 0 && player.character.dying == 0)
                    {
                        ExternalInteraction.NearestTarget(player);
                    }
                }
            }
        }

        /// <summary>
        /// Crouch mode keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("crouchModeKeyPressed")]
        public void CrouchMode(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    player.ToggleCrouchMode();
                }
            }
        }

        /// <summary>
        /// Cancel animations keybind
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("cancelAllAnimationsKeyPressed")]
        public void CancelAllAnimations(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    if (!player.forcedAnimation)
                    {
                        NAPI.Player.StopPlayerAnimation(player.user);
                    }
                }
            }
        }

        /// <summary>
        /// Commit object editor changes
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("objectEditorEnterKeyPressed")]
        public void CommitObjectEditorChanges(Client user, Vector3 offset, Vector3 rotation, Vector3 position)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    if (player.objectEditorActive)
                        ObjectEditor.CommitChanges(player, offset, rotation, position);
                }
            }
        }

        /// <summary>
        /// Cancel object editor changes
        /// </summary>
        /// <param name="user">The client instance</param>
        [RemoteEvent("objectEditorBackspaceKeyPressed")]
        public void CancelObjectEditorChanges(Client user)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                if (player.character != null)
                {
                    if (player.objectEditorActive)
                        ObjectEditor.CancelChanges(player);
                }
            }
        }


        /*
        public Keybinds() {
            API.onClientEventTrigger += onKeyPressed;
        }

        public void onKeyPressed(Client user, string eventName, params object[] arguments)
        {
            Player player = Player.Exists(user);
            if (player != null)
            {
                //Special conditions that not allow keybinds
                bool allowKeybinds = true;
                if(player.character != null)
                {
                    //Player using ATM
                    if(player.character.usingATM != null)
                    {
                        allowKeybinds = false;
                    }
                }

                if (!player.isInCharacterSelection)
                {
                    if (allowKeybinds || eventName == "key_close_all_gui")
                    {
                        if (player.showingGui != (int)Gui.GuiTypes.DIALOG)
                        {
                            switch (eventName)
                            {
                                //EDITOR KEYS
                                //ENTER
                                case "key_enter":
                                    {
                                        if (player.objectEditorActive)
                                            ObjectEditor.commitChanges(player, (Vector3)arguments[0], (Vector3)arguments[1], (Vector3)arguments[2]);

                                        break;
                                    }
                                //BACKSPACE
                                case "key_back":
                                    {
                                        if (player.objectEditorActive)
                                            ObjectEditor.cancelChanges(player);

                                        break;
                                    }

                                //NORMAL KEYS
                                //CHANGE CHARACTER KEYBIND
                                case "key_change_character":
                                    {
                                        if (player.character != null)
                                        {
                                            player.toCharacterSelection();
                                        }

                                        break;
                                    }

                                //CHANGE VOICE TYPE KEYBIND
                                case "key_change_voice_type":
                                    {
                                        player.changeVoiceType();
                                        break;
                                    }

                                //CLOSE ALL GUI
                                case "key_close_all_gui":
                                    {
                                        Gui.clearGui(player);
                                        break;
                                    }

                                //SHOW HELP MENU KEYBIND
                                case "key_show_help_menu":
                                    {
                                        if (player.showingGui == (int)Gui.GuiTypes.MENU && player.showingGuiId == (int)Gui.MenuID.HELP_MENU)
                                        {
                                            Gui.destroyMenu(player);
                                        }
                                        else
                                        {
                                            if (player.showingGui != -1)
                                            {
                                                Gui.clearGui(player);
                                            }
                                            else
                                            {
                                                player.showHelpMenu();
                                            }
                                        }

                                        break;
                                    }

                                //SHOW PLAYER LIST KEYBIND
                                case "key_show_player_list":
                                    {
                                        if (player.showingGui == (int)Gui.GuiTypes.PLAYER_LIST_PAGE)
                                        {
                                            player.hidePlayerListPage();
                                        }
                                        else
                                        {
                                            if (player.showingGui != -1)
                                            {
                                                Gui.clearGui(player);
                                            }
                                            else
                                            {
                                                player.showPlayerListPage();
                                            }
                                        }

                                        break;
                                    }
                                //SHOW SELF INTERACTION KEYBIND
                                case "key_show_self_interaction_menu":
                                    {
                                        if (player.showingGui == (int)Gui.MenuID.SELF_INTERACTION_MENU)
                                        {
                                            Gui.destroyMenu(player);
                                        }
                                        else
                                        {
                                            SelfInteraction.generateMenu(player);
                                        }

                                        break;
                                    }

                                //SHOW EXTERNAL INTERACTION KEYBIND
                                case "key_show_external_interaction_menu":
                                    {
                                        if (player.showingGui == (int)Gui.MenuID.EXTERNAL_INTERACTION_MENU)
                                        {
                                            Gui.destroyMenu(player);
                                        }
                                        else
                                        {
                                            if (player.character.cuffed == 0 && player.character.dying == 0)
                                            {
                                                ExternalInteraction.nearestTarget(player);
                                            }
                                        }

                                        break;
                                    }
                                //CANCEL ALL ANIMATIONS KEYBIND
                                case "key_cancel_all_animations":
                                    {
                                        if (!player.forcedAnimation)
                                        {
                                            API.stopPlayerAnimation(player.user);
                                        }

                                        break;
                                    }
                                //TOGGLE INVENTORY OPEN
                                case "key_toggle_inventory":
                                    {
                                        if (player.showingGui == (int)Gui.GuiTypes.INVENTORY_INTERFACE)
                                        {
                                            Inventory.closeInventoryForPlayer(player);
                                        }
                                        else
                                        {
                                            if (player.showingGui != -1)
                                            {
                                                Gui.clearGui(player);
                                            }
                                            else
                                            {
                                                Inventory.openInventoryForPlayer(player);
                                            }
                                        }

                                        break;
                                    }
                                //CROUCH MODE
                                case "key_crouch_mode":
                                    {
                                        player.toggleCrouchMode();
                                        break;
                                    }
                            }
                        }
                    }
                }
            }
        } */
    } 
}
