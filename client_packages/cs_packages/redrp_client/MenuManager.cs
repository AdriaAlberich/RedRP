/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */

using System;
using System.Collections.Generic;
using System.Drawing;

using RAGE.NUI;
using Newtonsoft.Json;

namespace redrp
{
    /// <summary>
    /// NativeUI menu manager system
    /// </summary>
    public class MenuManager : RAGE.Events.Script
    {

        /// <summary>
        /// Menu pool object
        /// </summary>
        private MenuPool menuPool;

        /// <summary>
        /// Current menu instance
        /// </summary>
        private UIMenu menu;

        /// <summary>
        /// Menu item types definitions
        /// </summary>
        public enum MenuItemTypes
        {
            Normal,
            Colored,
            Checkbox,
            List
        }

        /// <summary>
        /// JSON menu model helper class
        /// </summary>
        private class Menu
        {
            public string title;
            public string subtitle;
            public bool noExit;
            public bool noButtons;
            public int posX;
            public int posY;
            public int anchor;
            public string customBanner;
            public int widthOffset;
            public List<MenuItem> items;
        }

        /// <summary>
        /// JSON MenuItem helper class
        /// </summary>
        private class MenuItem
        {
            public string itemName;
            public string itemDescription;
            public int itemType;
            public int itemRightBadge;
            public int itemLeftBadge;
            public string itemRightLabel;
            public List<dynamic> listItems;
            public bool isChecked;
            public Color mainColor;
            public Color highlightColor;
        }

        /// <summary>
        /// System initialization
        /// </summary>
        public MenuManager()
        {
            // Define eventhandlers
            RAGE.Events.Add("createMenu", CreateMenu);
            RAGE.Events.Add("destroyMenu", DestroyMenu);
            RAGE.Events.Add("menuSelectionHotkeyPressed", MenuSelectionHotkey);

            // Instance the pool
            menuPool = new MenuPool();

            // Initialize global variables
            menu = null;

            // Add tick eventhandler delegate
            RAGE.Events.Tick += DrawMenu;
        }

        /// <summary>
        /// Creates a new menu
        /// </summary>
        /// <param name="args"></param>
        public void CreateMenu(object[] args)
        {
            // Deserialize JSON menu data
            dynamic results = JsonConvert.DeserializeObject<dynamic>(args[0].ToString());
            Menu menuData = results.ToObject<Menu>();

            // Creates the new menu
            menu = new UIMenu(menuData.title, menuData.subtitle, new Point(menuData.posX, menuData.posY));

            // Freeze input
            menu.FreezeAllInput = true;

            // If there is a custom banner (url)
            if (menuData.customBanner != string.Empty)
            {
                menu.SetBannerType(menuData.customBanner);
            }

            // Disable instructional buttons
            if (menuData.noButtons)
            {
                menu.DisableInstructionalButtons(true);
            }

            // Prevents menu from being closed by default action
            if (menuData.noExit)
            {
                menu.ResetKey(UIMenu.MenuControls.Back);
            }

            // Set menu width offset
            if (menuData.widthOffset > 0)
            {
                menu.SetMenuWidthOffset(menuData.widthOffset);
            }

            // Creation of menu items
            foreach (MenuItem item in menuData.items)
            {
                // Select item type
                switch (item.itemType)
                {
                    case (int)MenuItemTypes.Normal:
                        {
                            // Create the item with the basic data
                            UIMenuItem newItem = new UIMenuItem(item.itemName, item.itemDescription);

                            // Set the right badge
                            if (item.itemRightBadge != -1)
                            {
                                newItem.SetRightBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            // Set the left badge
                            if (item.itemLeftBadge != -1)
                            {
                                newItem.SetLeftBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            // Set the right label
                            if (item.itemRightLabel != string.Empty)
                            {
                                newItem.SetRightLabel(item.itemRightLabel);
                            }

                            // Add item to menu
                            menu.AddItem(newItem);
                            break;
                        }
                    case (int)MenuItemTypes.Colored:
                        {
                            
                            UIMenuColoredItem newItem = new UIMenuColoredItem(item.itemName, item.itemDescription, item.mainColor, item.highlightColor);

                            if (item.itemRightBadge != -1)
                            {
                                newItem.SetRightBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            if (item.itemLeftBadge != -1)
                            {
                                newItem.SetLeftBadge((UIMenuItem.BadgeStyle)item.itemLeftBadge);
                            }

                            if (item.itemRightLabel != string.Empty)
                            {
                                newItem.SetRightLabel(item.itemRightLabel);
                            }
                            menu.AddItem(newItem);
                            
                            break;
                        }
                    case (int)MenuItemTypes.Checkbox:
                        {
                            UIMenuCheckboxItem newItem = new UIMenuCheckboxItem(item.itemName, item.isChecked, item.itemDescription);
                            if (item.itemRightBadge != -1)
                            {
                                newItem.SetRightBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            if (item.itemLeftBadge != -1)
                            {
                                newItem.SetLeftBadge((UIMenuItem.BadgeStyle)item.itemLeftBadge);
                            }

                            if (item.itemRightLabel != string.Empty)
                            {
                                newItem.SetRightLabel(item.itemRightLabel);
                            }
                            menu.AddItem(newItem);
                            break;
                        }
                    case (int)MenuItemTypes.List:
                        {
                            UIMenuListItem newItem = new UIMenuListItem(item.itemName, item.listItems, 0, item.itemDescription);
                            if (item.itemRightBadge != -1)
                            {
                                newItem.SetRightBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            if (item.itemLeftBadge != -1)
                            {
                                newItem.SetLeftBadge((UIMenuItem.BadgeStyle)item.itemLeftBadge);
                            }

                            if (item.itemRightLabel != string.Empty)
                            {
                                newItem.SetRightLabel(item.itemRightLabel);
                            }
                            menu.AddItem(newItem);
                            break;
                        }
                    default:
                        {
                            UIMenuItem newItem = new UIMenuItem(item.itemName, item.itemDescription);
                            if (item.itemRightBadge != -1)
                            {
                                newItem.SetRightBadge((UIMenuItem.BadgeStyle)item.itemRightBadge);
                            }

                            if (item.itemLeftBadge != -1)
                            {
                                newItem.SetLeftBadge((UIMenuItem.BadgeStyle)item.itemLeftBadge);
                            }

                            if (item.itemRightLabel != string.Empty)
                            {
                                newItem.SetRightLabel(item.itemRightLabel);
                            }
                            menu.AddItem(newItem);
                            break;
                        }
                }

                // On item select (click) eventhandler
                menu.OnItemSelect += (sender, itemSelected, index) =>
                {
                    RAGE.Events.CallRemote("onMenuItemSelected", itemSelected.Text, index);
                };

                // On index change eventhandler (item hovered)
                menu.OnIndexChange += (sender, index) =>
                {
                    RAGE.Events.CallRemote("onMenuIndexChange", index);
                };

                // On checkbox state change eventhandler
                menu.OnCheckboxChange += (sender, checkbox, checked_) =>
                {
                    RAGE.Events.CallRemote("onMenuItemCheckboxChange", checkbox.Text, checked_);
                };

                // On list item changed eventhandler
                menu.OnListChange += (sender, list, index) =>
                {
                    RAGE.Events.CallRemote("onMenuItemListChange", list.Text, list.Index, list.IndexToItem(index).ToString());
                };

                // Add menu to pool
                menuPool.Add(menu);

                // Refresh menu index
                menuPool.RefreshIndex();

                // Set menu visible
                menu.Visible = true;
            }
        }

        /// <summary>
        /// Destroys the default menu
        /// </summary>
        /// <param name="args"></param>
        public void DestroyMenu(object[] args)
        {
            // If menu exists
            if (menu != null)
            {
                // Set menu invisible
                menu.Visible = false;

                // Close menu from pool
                menuPool.CloseAllMenus();

                // Clear menu pointer
                menu = null;
            }
        }

        /// <summary>
        /// Draw menu per game tick
        /// </summary>
        /// <param name="nametags"></param>
        public void DrawMenu(List<RAGE.Events.TickNametagData> nametags)
        {
            if(menuPool != null) {
                menuPool.ProcessMenus();
            }
        }

        /// <summary>
        /// Menu selection hotkey handler
        /// </summary>
        /// <param name="args"></param>
        public void MenuSelectionHotkey(object[] args)
        {
            // If menu exists
            if (menu != null)
            {
                int selectedIndex = int.Parse(args[0].ToString());

                menu.CurrentSelection = selectedIndex;
                menu.SelectItem();
            }
        }

    }
}

    /*
//Update menu
API.onUpdate.connect(function () {
    if (visible) {
        API.disableControlThisFrame(24);
        API.disableControlThisFrame(1);
        API.disableControlThisFrame(2);
    }
});

//Key up (menu hotkeys)
API.onKeyUp.connect(function (sender, args) {
    if (visible) {
        switch (args.KeyCode) {
            case Keys.NumPad0: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 0);
                break;
            }
            case Keys.NumPad1: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 1);
                break;
            }
            case Keys.NumPad2: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 2);
                break;
            }
            case Keys.NumPad3: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 3);
                break;
            }
            case Keys.NumPad4: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 4);
                break;
            }
            case Keys.NumPad5: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 5);
                break;
            }
            case Keys.NumPad6: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 6);
                break;
            }
            case Keys.NumPad7: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 7);
                break;
            }
            case Keys.NumPad8: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 8);
                break;
            }
            case Keys.NumPad9: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 9);
                break;
            }
        }
    }
});
*/