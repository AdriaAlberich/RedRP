/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace redrp
{
    /// <summary>
    /// Menu class
    /// </summary>
    public class Menu
    {
        public MenuModel menuModel;
        public Action<Player, string, int> onItemSelectedCallback;
        public Action<Player, int> onIndexChangeCallback;
        public Action<Player, string, bool> onCheckboxChangeCallback;
        public Action<Player, string, int, string> onItemListChangeCallback;
        public dynamic miscData;
        public dynamic miscData2;

        // Class constructor
        public Menu(string title = "Test Menu", string subtitle = "Test Menu",
            bool noExit = true, bool noButtons = true, int posX = 0, int posY = 0,
            int anchor = 0, string customBanner = "", int widthOffset = 0,
            Action<Player, string, int> onItemSelectedCallback = null, Action<Player, int> onIndexChangeCallback = null, 
            Action<Player, string, bool> onCheckboxChangeCallback = null, Action<Player, string, int, string> onItemListChangeCallback = null)
        {
            this.menuModel = new MenuModel();
            this.menuModel.title = title;
            this.menuModel.subtitle = subtitle;
            this.menuModel.noExit = noExit;
            this.menuModel.noButtons = noButtons;
            this.menuModel.posX = posX;
            this.menuModel.posY = posY;
            this.menuModel.anchor = anchor;
            this.menuModel.customBanner = customBanner;
            this.menuModel.widthOffset = widthOffset;
            this.menuModel.items = new List<MenuItemModel>();
            this.onItemSelectedCallback = onItemSelectedCallback;
            this.onIndexChangeCallback = onIndexChangeCallback;
            this.onCheckboxChangeCallback = onCheckboxChangeCallback;
            this.onItemListChangeCallback = onItemListChangeCallback;
        }
    }
}
