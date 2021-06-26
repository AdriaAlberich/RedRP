/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Drawing;

using RAGE;
using RAGE.Elements;

namespace redrp
{

    /// <summary>
    /// InventoryInterfaceController
    /// </summary>
    public class InventoryInterfaceController : Events.Script
    {
        /// <summary>
        /// InventoryInterfaceController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string leftTitle;
        private static string leftCapacity;
        private static string leftCapacityMax;
        private static string leftItems;
        private static bool leftLocked;
        private static string rightTitle;
        private static string rightCapacity;
        private static string rightCapacityMax;
        private static string rightItems;

        /// <summary>
        /// InventoryInterfaceController initialization
        /// </summary>
        public InventoryInterfaceController()
        {
            // Server events
            Events.Add("showInventoryInterface", Show);
            Events.Add("hideInventoryInterface", Hide);
            Events.Add("addItemInventory", AddItem);
            Events.Add("removeItemInventory", RemoveItem);
            Events.Add("addSpecialItemInventory", AddSpecialItem);
            Events.Add("removeSpecialItemInventory", RemoveSpecialItem);
            Events.Add("openContainerInventory", OpenContainer);
            Events.Add("closeContainerInventory", CloseContainer);

            // UI events
            Events.Add("requestInventoryData", RequestInitializationData);
            Events.Add("closeInventory", CloseInventory);
            Events.Add("sendItemClick", SendItemClick);
            Events.Add("sendItemDoubleClick", SendItemDoubleClick);
            Events.Add("sendItemRightClick", SendItemRightClick);
            Events.Add("sendReturnClick", SendReturnClick);
        }

        /// <summary>
        /// Shows the main interface
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            leftTitle = args[0].ToString();
            leftCapacity = args[1].ToString();
            leftCapacityMax = args[2].ToString();
            leftItems = args[3].ToString();
            leftLocked = bool.Parse(args[4].ToString());
            leftTitle = args[5].ToString();
            leftCapacity = args[6].ToString();
            leftCapacityMax = args[7].ToString();
            leftItems = args[8].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/inventoryInterface.html");
            Main.playerBrowserActive = true;
            RAGE.Chat.Activate(false);
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Destroys the interface
        /// </summary>
        /// <param name="args"></param>
        private void Hide(object[] args)
        {
            window.Destroy();
            Main.playerBrowserActive = false;
            RAGE.Chat.Activate(true);
            RAGE.Ui.Cursor.Visible = false;
        }

        /// <summary>
        /// Adds an item to the inventory interface
        /// </summary>
        /// <param name="args"></param>
        private void AddItem(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            string itemName = args[1].ToString();
            string itemImage = args[2].ToString();
            string itemQuantity = args[3].ToString();
            string capacity = args[4].ToString();

            if(isRight)
            {
                window.ExecuteJs("addRightItem(" + itemName + ", " + itemImage + ", " + itemQuantity + ", " + capacity + ")");
            }
            else
            {
                window.ExecuteJs("addLeftItem(" + itemName + ", " + itemImage + ", " + itemQuantity + ", " + capacity + ")");
            }
        }

        /// <summary>
        /// Removes an item to the inventory interface
        /// </summary>
        /// <param name="args"></param>
        private void RemoveItem(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            string itemId = args[1].ToString();
            string capacity = args[2].ToString();

            if (isRight)
            {
                window.ExecuteJs("removeRightItem(" + itemId + ", " + capacity + ")");
            }
            else
            {
                window.ExecuteJs("removeLeftItem(" + itemId + ", " + capacity + ")");
            }
        }

        /// <summary>
        /// Adds a special item
        /// </summary>
        /// <param name="args"></param>
        private void AddSpecialItem(object[] args)
        {
            string index = args[0].ToString();
            string itemName = args[1].ToString();
            string itemImage = args[2].ToString();
            string itemQuantity = args[3].ToString();
            string capacity = args[4].ToString();

            window.ExecuteJs("putSpecialItem(" + index + ", " + itemName + ", " + itemImage + ", " + itemQuantity + ", " + capacity + ")");
        }

        /// <summary>
        /// Removes a special item
        /// </summary>
        /// <param name="args"></param>
        private void RemoveSpecialItem(object[] args)
        {
            string index = args[0].ToString();
            string capacity = args[1].ToString();

            window.ExecuteJs("removeSpecialItem(" + index + ", " + capacity + ")");
        }

        /// <summary>
        /// Opens an item container
        /// </summary>
        /// <param name="args"></param>
        private void OpenContainer(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            string title = args[1].ToString();
            string weight = args[2].ToString();
            string capacity = args[3].ToString();
            string items = args[4].ToString();

            if (isRight)
            {
                window.ExecuteJs("openRightContainer(" + title + ", " + weight + ", " + capacity + ", " + items + ")");
            }
            else
            {
                window.ExecuteJs("openLeftContainer(" + title + ", " + weight + ", " + capacity + ", " + items + ")");
            }
        }

        /// <summary>
        /// Closes an item container
        /// </summary>
        /// <param name="args"></param>
        private void CloseContainer(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            string title = args[1].ToString();
            string weight = args[2].ToString();
            string capacity = args[3].ToString();
            string items = args[4].ToString();

            if (isRight)
            {
                window.ExecuteJs("closeRightContainer(" + title + ", " + weight + ", " + capacity + ", " + items + ")");
            }
            else
            {
                window.ExecuteJs("closeLeftContainer(" + title + ", " + weight + ", " + capacity + ", " + items + ")");
            }
        }

        /// <summary>
        /// Request initialization data when ready
        /// </summary>
        /// <param name="args"></param>
        private void RequestInitializationData(object[] args)
        {
            window.ExecuteJs("inventoryInitialization(" + leftTitle + ", " + leftCapacity + ", " + leftCapacityMax + ", " + leftItems + ", " + leftLocked + ", " + rightTitle + ", " + rightCapacity + ", " + rightCapacityMax + ", " + rightItems + ")");
        }

        /// <summary>
        /// Closes the inventory on server
        /// </summary>
        /// <param name="args"></param>
        private void CloseInventory(object[] args)
        {
            Events.CallRemote("inventoryClose");
        }

        /// <summary>
        /// Sends an item click event
        /// </summary>
        /// <param name="args"></param>
        private void SendItemClick(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            int itemId = int.Parse(args[1].ToString());
            Events.CallRemote("inventoryItemClick", isRight ? 1 : 0, itemId);
        }

        /// <summary>
        /// Sends an item double click event
        /// </summary>
        /// <param name="args"></param>
        private void SendItemDoubleClick(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            int itemId = int.Parse(args[1].ToString());
            Events.CallRemote("inventoryItemDoubleClick", isRight ? 1 : 0, itemId);
        }

        /// <summary>
        /// Sends an item right click event
        /// </summary>
        /// <param name="args"></param>
        private void SendItemRightClick(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            int itemId = int.Parse(args[1].ToString());
            Events.CallRemote("inventoryItemRightClick", isRight ? 1 : 0, itemId);
        }

        /// <summary>
        /// Sends a return click event
        /// </summary>
        /// <param name="args"></param>
        private void SendReturnClick(object[] args)
        {
            bool isRight = bool.Parse(args[0].ToString());
            Events.CallRemote("inventoryReturnClick", isRight ? 1 : 0);
        }
    }
}
