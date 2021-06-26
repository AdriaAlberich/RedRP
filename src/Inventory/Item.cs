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
    /// Item class and methods
    /// </summary>
    public class Item : Script
    {

        // ITEM ATTRIBUTES

        /// <summary>
        /// The itemdata identifier
        /// </summary>
        public int id { get; set; }

        /// <summary>
        /// The item quantity
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// The item content list (if can contain other items)
        /// </summary>
        public List<Item> contentList { get; set; }

        /// <summary>
        /// The accessory list (if is a weapon)
        /// </summary>
        public List<Item> accessoryList { get; set; }

        /// <summary>
        /// The finger print list
        /// </summary>
        public List<string> fingerPrintList { get; set; }

        /// <summary>
        /// Other data relevant to the item goes here
        /// </summary>
        public List<string> miscData { get; set; }

        /// <summary>
        /// If is occuping a special slot
        /// </summary>
        public int slot { get; set; }

        //Dummy classes
        /*
        private class RootObject
        {
            public List<DummyItem> items;
        }

        private class RootString
        {
            public List<string> items;
        }

        private class DummyItem
        {
            public int id;
            public int uses;
            public List<DummyItem> accessoryList;
            public string[] fingerPrintList;
            public string[] miscInfo;
        }*/

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uses"></param>
        /// <param name="contentList"></param>
        /// <param name="accessoryList"></param>
        /// <param name="fingerPrintList"></param>
        /// <param name="miscInfo"></param>
        /// <returns></returns>
        public static Item CreateItem(int id, int quantity = -1, List<Item> contentList = null, List<Item> accessoryList = null, List<string> fingerPrintList = null, List<string> miscData = null)
        {
            Item newItem = null;
            ItemData itemData = ItemData.GetById(id);
            if(itemData != null)
            {
                newItem = new Item();
                newItem.id = id;
                if (quantity != -1)
                    newItem.quantity = quantity;
                else
                    newItem.quantity = itemData.quantity;
                if (contentList != null)
                    newItem.contentList = contentList;
                else
                    newItem.contentList = new List<Item>();
                if(accessoryList != null)
                    newItem.accessoryList = accessoryList;
                else
                    newItem.accessoryList = new List<Item>();
                if (fingerPrintList != null)
                    newItem.fingerPrintList = fingerPrintList;
                else
                    newItem.fingerPrintList = new List<string>();
                if (miscData != null)
                    newItem.miscData = miscData;
                else
                    newItem.miscData = new List<string>();

                newItem.slot = 1;
            }

            return newItem;
        }


        ///**
        //* toSerializedContent()
        //* Returns the serialized content
        //*/
        //public string toSerializedContent()
        //{
        //    RootObject root = new RootObject();
        //    root.items = new List<DummyItem>();
        //    Logs.debug("TOSERIALIZEDCONTENT1");
        //    foreach(Item item in contentList)
        //    {
        //        Logs.debug("TOSERIALIZEDCONTENT2");
        //        DummyItem newDummyItem = new DummyItem();
        //        newDummyItem.id = item.id;
        //        newDummyItem.uses = item.uses;
        //        newDummyItem.accessoryList = new List<DummyItem>();
        //        Logs.debug("TOSERIALIZEDCONTENT3");
        //        foreach (Item acc in item.accessoryList)
        //        {
        //            DummyItem newDummyAcc = new DummyItem();
        //            newDummyAcc.id = acc.id;
        //            newDummyAcc.uses = acc.uses;
        //            newDummyAcc.accessoryList = new List<DummyItem>();
        //            newDummyAcc.fingerPrintList = acc.fingerPrintList.ToArray();
        //            newDummyAcc.miscInfo = acc.miscInfo.ToArray();
        //            newDummyItem.accessoryList.Add(newDummyAcc);
        //        }
        //        Logs.debug("TOSERIALIZEDCONTENT4");
        //        newDummyItem.fingerPrintList = item.fingerPrintList.ToArray();
        //        Logs.debug("TOSERIALIZEDCONTENT5");
        //        newDummyItem.miscInfo = item.miscInfo.ToArray();
        //        Logs.debug("TOSERIALIZEDCONTENT6");
        //        root.items.Add(newDummyItem);

        //    }
        //    Logs.debug("TOSERIALIZEDCONTENT7");
        //    return API.toJson(root);
        //}

        ///**
        //* fromSerializedContent()
        //* Sets the content list from the serialized one
        //*/
        //public void fromSerializedContent(string serializedContent)
        //{
        //    dynamic content = API.fromJson(serializedContent);
        //    RootObject contentResult = content.ToObject<RootObject>();

        //    contentList.Clear();

        //    foreach (DummyItem dummyItem in contentResult.items)
        //    {
        //        Item newItem = createItem(dummyItem.id, dummyItem.uses);
        //        newItem.contentList = new List<Item>(); ;
        //        newItem.accessoryList = new List<Item>();

        //        foreach (DummyItem acc in dummyItem.accessoryList)
        //        {
        //            Item newAccessory = createItem(acc.id, acc.uses);
        //            newAccessory.contentList = new List<Item>();
        //            newAccessory.accessoryList = new List<Item>(); 
        //            newAccessory.fingerPrintList = acc.fingerPrintList.ToList();
        //            newAccessory.miscInfo = acc.miscInfo.ToList();
        //            newItem.accessoryList.Add(newAccessory);
        //        }

        //        newItem.fingerPrintList = dummyItem.fingerPrintList.ToList();
        //        newItem.miscInfo = dummyItem.miscInfo.ToList();

        //        contentList.Add(newItem);
        //    }
        //}


        ///**
        //* toSerializedAccessories()
        //* Returns the serialized accessories
        //*/
        //public string toSerializedAccessories()
        //{
        //    RootObject root = new RootObject();
        //    root.items = new List<DummyItem>();
        //    foreach (Item item in accessoryList)
        //    {
        //        DummyItem newDummyItem = new DummyItem();
        //        newDummyItem.id = item.id;
        //        newDummyItem.uses = item.uses;
        //        newDummyItem.accessoryList = new List<DummyItem>();
        //        newDummyItem.fingerPrintList = item.fingerPrintList.ToArray();
        //        newDummyItem.miscInfo = item.miscInfo.ToArray();
        //        root.items.Add(newDummyItem);
        //    }

        //    return API.toJson(root);
        //}

        ///**
        //* fromSerializedAccessories()
        //* Sets the accessory list from the serialized one
        //*/
        //public void fromSerializedAccessories(string serializedAccessories)
        //{
        //    dynamic accessories = API.fromJson(serializedAccessories);
        //    RootObject accessoryResult = accessories.ToObject<RootObject>();

        //    accessoryList.Clear();

        //    foreach (DummyItem dummyItem in accessoryResult.items)
        //    {
        //        Item newItem = createItem(dummyItem.id, dummyItem.uses);
        //        newItem.contentList = new List<Item>();
        //        newItem.accessoryList = new List<Item>(); 

        //        newItem.fingerPrintList = dummyItem.fingerPrintList.ToList();
        //        newItem.miscInfo = dummyItem.miscInfo.ToList();

        //        accessoryList.Add(newItem);
        //    }
        //}

        ///**
        //* serializeStringList()
        //* Returns the serialized form of a string list
        //*/
        //public static string serializeStringList(List<string> list)
        //{
        //    RootString root = new RootString();
        //    root.items = new List<string>();
        //    foreach (string str in list)
        //    {
        //        root.items.Add(str);
        //    }

        //    return API.shared.toJson(root);
        //}

        ///**
        //* deserializeStringList()
        //* Returns the deserialized list of a serialized string list
        //*/
        //public static List<string> deserializeStringList(string serializedList)
        //{
        //    List<string> list = new List<string>();
        //    dynamic prints = API.shared.fromJson(serializedList);
        //    RootString result = prints.ToObject<RootString>();

        //    foreach (string str in result.items)
        //    {
        //        list.Add(str);
        //    }

        //    return list;
        //}

        ///**
        //* toSerializedFingerPrints()
        //* Returns the serialized finger prints
        //*/
        //public string toSerializedFingerPrints()
        //{
        //    return serializeStringList(fingerPrintList);
        //}

        ///**
        //* fromSerializedFingerPrints()
        //* Sets the finger print list from serialized one.
        //*/
        //public void fromSerializedFingerPrints(string serializedFingerPrints)
        //{
        //    fingerPrintList = deserializeStringList(serializedFingerPrints);
        //}

        ///**
        //* toSerializedMiscInfo()
        //* Returns the serialized misc info
        //*/
        //public string toSerializedMiscInfo()
        //{
        //    return serializeStringList(miscInfo);
        //}

        ///**
        //* fromSerializedMiscInfo()
        //* Sets the misc info list from serialized one.
        //*/
        //public void fromSerializedMiscInfo(string serializedMiscInfo)
        //{
        //    miscInfo = deserializeStringList(serializedMiscInfo);
        //}

        

        /// <summary>
        /// Returns true if item is heavy
        /// </summary>
        /// <returns>True if is heavy, false if not</returns>
        public bool IsHeavy()
        {
            return ItemData.GetById(this.id).isHeavy;
        }

        /// <summary>
        /// Checks if item is a weapon
        /// </summary>
        /// <returns>True if is a weapon, false otherwise</returns>
        public bool IsWeapon()
        {
            ItemData itemData = ItemData.GetById(this.id);
            return itemData.type == (int)Global.ItemType.MeleeWeapon || itemData.type == (int)Global.ItemType.FireWeapon || itemData.type == (int)Global.ItemType.ThrowableWeapon || itemData.type == (int)Global.ItemType.SpecialWeapon;
        }

        /// <summary>
        /// Checks if item is a weapon accessory
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsWeaponAccessory()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.WeaponAccessory;
        }

        /// <summary>
        /// Checks if the weapon already has the same attachment
        /// </summary>
        /// <param name="accessory">The accessory instance</param>
        /// <returns>True if yes, false otherwise</returns>
        public bool HasWeaponSameAccessory(Item accessory)
        {
            bool hasWeaponSameAccessory = false;
            foreach(Item acc in this.accessoryList)
            {
                if (ItemData.GetById(acc.id).weaponModel == ItemData.GetById(accessory.id).weaponModel)
                {
                    hasWeaponSameAccessory = true;
                }
            }

            return hasWeaponSameAccessory;
        }

        /// <summary>
        /// Checks if the accessory is compatible with the weapon
        /// </summary>
        /// <param name="accessory">The accessory instance</param>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsWeaponAccessoryCompatible(Item accessory)
        {
            bool isWeaponAccessoryCompatible = false;
            string weaponModel = ItemData.GetById(this.id).weaponModel;
            WeaponComponent[] comp = NAPI.Util.GetAllWeaponComponents(NAPI.Util.WeaponNameToModel(weaponModel));
            for(int i = 0; i<comp.Length; i++)
            {
                string accessoryHash = comp[i].ToString();
                Log.Debug("ACCESSORY: " + accessoryHash + " " + ItemData.GetById(accessory.id).weaponModel);
                if(accessoryHash == ItemData.GetById(accessory.id).weaponModel)
                {
                    isWeaponAccessoryCompatible = true;
                }
            }

            return isWeaponAccessoryCompatible;
        }

        /// <summary>
        /// Checks if item is bodyarmor
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsBodyarmor()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Bodyarmor;
        }

        /// <summary>
        /// Checks if item is a backpack
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsBackpack()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Backpack;
        }

        /// <summary>
        /// Checks if item are gloves
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsGloves()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Gloves;
        }

        /// <summary>
        /// Checks if item is a hat
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsHat()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Hat;
        }

        /// <summary>
        /// Check if item are glasses
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsGlasses()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Glasses;
        }

        /// <summary>
        /// Check if item is a mask
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsMask()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Mask;
        }

        /// <summary>
        /// Checks if item is an accessory (clothing)
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsAccessory()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Accessory;
        }

        /// <summary>
        /// Checks if item is an ear accessory
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsEarAccessory()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.EarAccessory;
        }

        /// <summary>
        /// Checks if item is torso clothing
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsTorso()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.TorsoClothes;
        }

        /// <summary>
        /// Checks if item are trousers
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsTrousers()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.LegClothes;
        }

        /// <summary>
        /// Checks if item are shoes
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsShoes()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Shoes;
        }

        /// <summary>
        /// Checks if item is a watch
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsWatch()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Watch;
        }

        /// <summary>
        /// Checks if item is a bracelet
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsBracelet()
        {
            return ItemData.GetById(this.id).type == (int)Global.ItemType.Bracelet;
        }

        /// <summary>
        /// Checks if item is a helmet and can hide the identity
        /// </summary>
        /// <param name="sex">The player sex</param>
        /// <returns></returns>
        public bool IsHelmetAndHidesIdentity(int sex)
        {
            bool hideIdentity = false;
            ItemData itemData = ItemData.GetById(this.id);
            if (itemData.type == (int)Global.ItemType.Hat)
            {
                if(sex == 0)
                {
                    switch (itemData.maleVariation)
                    {
                        case 18:
                        case 38:
                        case 47:
                        case 50:
                        case 51:
                        case 52:
                        case 53:
                        case 62:
                        case 73:
                        case 78:
                        case 80:
                        case 82:
                        case 91:
                        case 111:
                        case 115:
                            {
                                hideIdentity = true;
                                break;
                            }
                        default: hideIdentity = false; break;
                    }
                }
                else
                {
                    switch (itemData.femaleVariation)
                    {
                        case 18:
                        case 37:
                        case 46:
                        case 49:
                        case 50:
                        case 51:
                        case 52:
                        case 62:
                        case 72:
                        case 77:
                        case 79:
                        case 81:
                        case 90:
                        case 110:
                        case 114:
                            {
                                hideIdentity = true;
                                break;
                            }
                        default: hideIdentity = false; break;
                    }
                }
                
            }

            return hideIdentity;
        }

        /// <summary>
        /// Checks if item is a container of items
        /// </summary>
        /// <returns>True if yes, false otherwise</returns>
        public bool IsItemContainer()
        {
            return ItemData.GetById(this.id).maxContentWeight > 0;
        }

        /// <summary>
        /// Adds a finger print to an item
        /// </summary>
        /// <param name="print">The print register</param>
        public void AddFingerPrint(string print)
        {
            if (!this.fingerPrintList.Contains(print))
            {
                this.fingerPrintList.Add(print);

                if (this.fingerPrintList.Count() > Global.ItemMaxFingerPrints)
                {
                    this.fingerPrintList.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Clear all finger prints of an item
        /// </summary>
        public void CleanFingerPrints()
        {
            this.fingerPrintList.Clear();
        }

        /// <summary>
        /// Returns the item content as JSON
        /// </summary>
        /// <returns>The content as JSON</returns>
        public string GetContentJSON()
        {
            List<string> content = new List<string>();

            // Then we process all container items
            foreach (Item item in this.contentList)
            {
                ItemData itemData = ItemData.GetById(item.id);
                content.Add(itemData.nameSingular + ";" + itemData.image + ";" + itemData.quantity);
            }

            return NAPI.Util.ToJson(content.ToArray());
        }

        /// <summary>
        /// Get content weight of an item
        /// </summary>
        /// <returns>The content weight</returns>
        public int GetContentWeight()
        {
            int contentWeight = 0;
            foreach (Item item in this.contentList)
            {
                ItemData itemData = ItemData.GetById(item.id);
                contentWeight += itemData.itemWeight;
            }

            return contentWeight;
        }

        /// <summary>
        /// Returns a text depending on the type of item for the inventory interface
        /// </summary>
        /// <returns>The item overview as string</returns>
        public string GetOverviewText()
        {
            ItemData itemData = ItemData.GetById(this.id);
            string overviewText;
            switch(this.id)
            {
                default:
                    {
                        overviewText = itemData.nameSingular;
                        break;
                    }
            }

            return overviewText;
        }

        /// <summary>
        /// Returns a use action text depending on the type of item for the inventory interface
        /// </summary>
        /// <returns>The action text</returns>
        public string GetUseText()
        {
            ItemData itemData = ItemData.GetById(this.id);
            string useText;
            switch (itemData.type)
            {
                case (int)Global.ItemType.FireWeapon:
                    {
                        useText = "Modificar";
                        break;
                    }
                case (int)Global.ItemType.Book:
                    {
                        useText = "Leer";
                        break;
                    }
                case (int)Global.ItemType.Food:
                    {
                        useText = "Comer";
                        break;
                    }
                case (int)Global.ItemType.Drink:
                    {
                        useText = "Beber";
                        break;
                    }
                default:
                    {
                        useText = "Usar";
                        break;
                    }
            }

            return useText;
        }

        /// <summary>
        /// Returns a use action description depending on the type of item for the inventory interface
        /// </summary>
        /// <returns></returns>
        public string GetUseDescriptionText()
        {
            ItemData itemData = ItemData.GetById(this.id);
            string useDescriptionText;
            switch (itemData.type)
            {
                case (int)Global.ItemType.FireWeapon:
                    {
                        useDescriptionText = "Modificar el arma";
                        break;
                    }
                case (int)Global.ItemType.Book:
                    {
                        useDescriptionText = "Leer el libro";
                        break;
                    }
                case (int)Global.ItemType.Food:
                    {
                        useDescriptionText = "Comer de la comida";
                        break;
                    }
                case (int)Global.ItemType.Drink:
                    {
                        useDescriptionText = "Beber de la bebida";
                        break;
                    }
                default:
                    {
                        useDescriptionText = "Usar el item";
                        break;
                    }
            }

            return useDescriptionText;
        }

        /// <summary>
        /// Sets the weapon serial
        /// </summary>
        /// <param name="identifier">The identifier</param>
        /// <param name="sqlid">The player sqlid</param>
        /// <param name="bizsqlid">The business sqlid</param>
        public void SetWeaponSerial(string identifier, int sqlid, int bizsqlid)
        {
            string serial = identifier + "_" + Util.GetCurrentTimestamp();
            if(sqlid != -1)
            {
                serial += "_" + sqlid;
            }
            if(bizsqlid != -1)
            {
                serial += "_" + bizsqlid;
            }

            if (this.miscData.Count() >= 2)
            {
                this.miscData[1] = serial;
            }
            else
            {
                this.miscData.Add("0");
                this.miscData.Add(serial);
            }
        }

        /// <summary>
        /// Removes the weapon serial
        /// </summary>
        public void RemoveWeaponSerial()
        {
            if(this.miscData.Count() >= 2)
            {
                this.miscData[1] = "";
            }
        }

        /// <summary>
        /// Gets the weapon compatible ammo case (item id)
        /// </summary>
        /// <returns>The ammo case item id</returns>
        public int GetAmmoCase()
        {
            foreach(ItemData itemData in Global.ItemsData)
            {
                if(itemData.IsAmmoOfWeapon(this.id))
                {
                    return itemData.sqlid;
                }
            }

            return -1;
        }

        /// <summary>
        /// Creates a container item and fills it with default objects
        /// </summary>
        /// <param name="containerId">The container item id</param>
        /// <param name="itemId">The content item id</param>
        /// <returns>The container item instance</returns>
        public static Item CreateContainerAndFill(int containerId, int itemId)
        {
            Item container = CreateItem(containerId);
            if(container != null)
            {
                if (container.IsItemContainer())
                {
                    ItemData contentData = ItemData.GetById(itemId);
                    ItemData containerData = ItemData.GetById(containerId);
                    while ((container.GetContentWeight() + contentData.itemWeight) <= containerData.maxContentWeight)
                    {
                        Item tempItem = CreateItem(itemId);
                        container.contentList.Add(tempItem);
                    }

                    return container;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a debit card for the given bank account and PIN
        /// </summary>
        /// <param name="account">The bank account</param>
        /// <param name="pin">The card PIN</param>
        /// <returns></returns>
        public static Item CreateDebitCard(BankAccount account, string pin)
        {
            Item newCard = null;
            string accountNumber = account.accountNumber;
            if (BankAccount.IsValidNumber(accountNumber))
            {
                if (Util.IsNumeric(pin))
                {
                    if (pin.Length == 4)
                    {
                        newCard = CreateItem(129);
                        newCard.miscData.Add(accountNumber);
                        newCard.miscData.Add(pin);
                        newCard.miscData.Add("0");
                    }
                }
            }

            return newCard;
        }
    }

}
