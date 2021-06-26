/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace redrp
{
    /// <summary>
    /// Menu data model
    /// </summary>
    public class MenuModel
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
        public List<MenuItemModel> items;

        // Class constructor
        public MenuModel(string title = "Test Menu", string subtitle = "Test Menu",
            bool noExit = true, bool noButtons = true, int posX = 0, int posY = 0,
            int anchor = 0, string customBanner = "", int widthOffset = 0)
        {
            this.title = title;
            this.subtitle = subtitle;
            this.noExit = noExit;
            this.noButtons = noButtons;
            this.posX = posX;
            this.posY = posY;
            this.anchor = anchor;
            this.customBanner = customBanner;
            this.widthOffset = widthOffset;
            this.items = new List<MenuItemModel>();
        }
    }
}
