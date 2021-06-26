/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace redrp
{
    /// <summary>
    /// MenuItem data model
    /// </summary>
    public class MenuItemModel
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

        // Class constructor
        public MenuItemModel(string itemName = "Dummy", string itemDescription = "", int itemType = 0, 
            int itemRightBadge = 0, int itemLeftBadge = 0, string itemRightLabel = "", List<dynamic> listItems = null,
            bool isChecked = false, Color mainColor = new Color(), Color highlightColor = new Color())
        {
            this.itemName = itemName;
            this.itemDescription = itemDescription;
            this.itemType = itemType;
            this.itemRightBadge = itemRightBadge;
            this.itemLeftBadge = itemLeftBadge;
            this.itemRightLabel = itemRightLabel;
            this.listItems = listItems;
            this.isChecked = isChecked;
            this.mainColor = mainColor;
            this.highlightColor = highlightColor;
        }
    }
}
