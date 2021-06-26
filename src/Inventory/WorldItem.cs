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
using MySql.Data.MySqlClient;

namespace redrp
{
    /// <summary>
    /// WorldItem class and methods
    /// </summary>
    public class WorldItem : Script
    {

        // WORLD ITEM ATTRIBUTES
        public int id { get; set; }
        public int quantity { get; set; }
        public List<Item> contentList { get; set; }
        public List<Item> accessoryList { get; set; }
        public List<string> fingerPrintList { get; set; }
        public List<string> miscData { get; set; }

        public Vector3 position { get; set; }
        public Vector3 rotation { get; set; }
        public uint dimension { get; set; }
        public GTANetworkAPI.Object prop { get; set; }

        private class RootObject
        {
            public List<WorldItem> items;
        }

        /// <summary>
        /// Create a world item
        /// </summary>
        /// <param name="item">The item instance</param>
        /// <param name="position">The object position</param>
        /// <param name="rotation">The object rotation</param>
        /// <param name="dimension">The object dimension</param>
        /// <returns></returns>
        public static WorldItem Create(Item item, Vector3 position, Vector3 rotation, uint dimension)
        {
            WorldItem newWorldItem = new WorldItem();
            newWorldItem.id = item.id;
            newWorldItem.quantity = item.quantity;
            newWorldItem.contentList = item.contentList;
            newWorldItem.accessoryList = item.accessoryList;
            newWorldItem.fingerPrintList = item.fingerPrintList;
            newWorldItem.miscData = item.miscData;

            newWorldItem.position = new Vector3(position.X, position.Y, position.Z);
            newWorldItem.dimension = dimension;
            newWorldItem.prop = NAPI.Object.CreateObject(ItemData.GetById(item.id).propModel, position, rotation, 255, dimension);
            NAPI.Entity.SetEntityInvincible(newWorldItem.prop, true);
            NAPI.Entity.SetEntityCollisionless(newWorldItem.prop, true);

            Global.WorldItems.Add(newWorldItem);
            return newWorldItem;
        }

        /// <summary>
        /// Destroy this world item
        /// </summary>
        public void destroy()
        {
            NAPI.Entity.DeleteEntity(this.prop);
            Global.WorldItems.Remove(this);
        }

        /// <summary>
        /// Converts a world item to a simple item
        /// </summary>
        /// <returns>The item instance</returns>
        public Item ToItem()
        {
            Item newItem = new Item();
            newItem.id = this.id;
            newItem.quantity = this.quantity;
            newItem.contentList = this.contentList;
            newItem.accessoryList = this.accessoryList;
            newItem.fingerPrintList = this.fingerPrintList;
            newItem.miscData = this.miscData;

            return newItem;
        }

        /// <summary>
        /// Returns the nearest world items
        /// </summary>
        /// <param name="position">The center position</param>
        /// <param name="distance">The distance</param>
        /// <param name="dimension">The dimension</param>
        /// <returns>A list of world items</returns>
        public static List<WorldItem> GetNearItems(Vector3 position, float distance, uint dimension)
        {
            List<WorldItem> nearItems = new List<WorldItem>();
            foreach(WorldItem item in Global.WorldItems)
            {
                if(item.position.DistanceTo(position) <= distance && item.dimension == dimension)
                {
                    nearItems.Add(item);
                }
            }

            return nearItems;
        }

        /// <summary>
        /// Returns the closest world item
        /// </summary>
        /// <param name="position">The center position</param>
        /// <param name="distance">The distance</param>
        /// <param name="dimension">The dimension</param>
        /// <returns>The world item instance</returns>
        public static WorldItem GetClosestItem(Vector3 position, float distance, uint dimension)
        {
            List<WorldItem> nearItems = new List<WorldItem>();
            WorldItem closestItem = null;
            foreach (WorldItem item in Global.WorldItems)
            {
                if (item.position.DistanceTo(position) <= distance && item.dimension == dimension)
                {
                    if(closestItem != null)
                    {
                        if(item.position.DistanceTo(position) < closestItem.position.DistanceTo(position))
                        {
                            closestItem = item;
                        }
                    }
                    else
                    {
                        closestItem = item;
                    }
                }
            }

            return closestItem;
        }

        /// <summary>
        /// Returns a list of world item strings in JSON to be used by clientside interfaces
        /// </summary>
        /// <param name="items">The world items list</param>
        /// <returns>The world item data as JSON</returns>
        public static string ToJSON(List<WorldItem> items)
        {
            List<string> content = new List<string>();
            foreach (WorldItem item in items)
            {
                ItemData info = ItemData.GetById(item.id);
                content.Add(info.nameSingular + ";" + info.image + ";" + item.quantity);
            }

            return NAPI.Util.ToJson(content);
        }

        /// <summary>
        /// Destroy all world items
        /// </summary>
        public static void DestroyAll()
        {
            for(int i = 0; i < Global.WorldItems.Count(); i++)
            {
                NAPI.Entity.DeleteEntity(Global.WorldItems[i].prop);
            }

            Global.WorldItems = new List<WorldItem>();
        }

        /// <summary>
        /// Initializes the world items from database
        /// </summary>
        public static bool InitializeWorldItems()
        {
            try
            {
                string jsonData = Util.GetDBField("GlobalData", "dataContent", "dataKey", "worlditems");

                if(jsonData != "")
                {
                    dynamic worldItemData = NAPI.Util.FromJson(jsonData);
                    RootObject worldItemRoot = worldItemData.ToObject<RootObject>();

                    Global.WorldItems = worldItemRoot.items.ToList();

                    foreach (WorldItem item in Global.WorldItems)
                    {
                        item.prop = NAPI.Object.CreateObject(ItemData.GetById(item.id).propModel, item.position, item.rotation, 255, item.dimension);
                        NAPI.Entity.SetEntityInvincible(item.prop, true);
                        NAPI.Entity.SetEntityCollisionless(item.prop, true);
                    }

                    return true;
                }
                else
                {
                    Global.WorldItems = new List<WorldItem>();
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Saves world items to database
        /// </summary>
        public static bool SaveWorldItems()
        {
            try
            {
                if(Global.WorldItems.Count > 0)
                {
                    RootObject root = new RootObject();
                    root.items = Global.WorldItems.ToList();

                    string jsonData = NAPI.Util.ToJson(root);

                    Util.UpdateDBField("GlobalData", "dataContent", jsonData, "dataKey", "worlditems");

                    return true;
                }
                else
                {
                    Util.UpdateDBField("GlobalData", "dataContent", "", "dataKey", "worlditems");
                    return true;
                }
            }
            catch(Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

    }
}
