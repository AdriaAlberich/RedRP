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
    /// HUD related stuff
    /// </summary>
    public class HudManager : Events.Script
    {
        /// <summary>
        /// HUD data
        /// </summary>
        public static bool visible = false;
        private static string money = "";
        private static string language = "";
        private static string voice = "";
        private static string adminDuty = "";

        /// <summary>
        /// Editor initialization
        /// </summary>
        public HudManager()
        {
            Events.Add("setHudVisible", SetVisible);
            Events.Add("updateHudMoney", UpdateMoney);
            Events.Add("setHudLanguage", SetLanguage);
            Events.Add("setHudVoiceType", SetVoiceType);
            Events.Add("setHudAdminDuty", SetAdminDuty);
            Events.Add("displayHudSubtitle", DisplaySubtitle);
            Events.Add("showHudLoadingPrompt", ShowLoadingPrompt);
            Events.Add("hideHudLoadingPrompt", HideLoadingPrompt);
            Events.Add("setHudMapWaypoint", SetMapWaypoint);
            Events.Add("removeHudMapWaypoint", RemoveMapWaypoint);

            Events.Tick += Render;
        }

        /// <summary>
        /// Executes code every tick (game frame)
        /// </summary>
        /// <param name="nametags"></param>
        private void Render(List<Events.TickNametagData> nametags)
        {
            if (visible)
            {
                // Money indicator
                if(money != "")
                {
                    RAGE.NUI.UIResText.Draw(money, 320, 870, RAGE.Game.Font.Pricedown, 0.7f, Color.DarkGreen, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                }

                // Language indicator
                if (language != "")
                {
                    RAGE.NUI.UIResText.Draw(money, 320, 920, RAGE.Game.Font.HouseScript, 0.6f, Color.LightBlue, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
                }

                // Voice indicator
                if (voice != "")
                {
                    switch(voice)
                    {
                        case "Susurrar": RAGE.NUI.UIResText.Draw(voice, 320, 960, RAGE.Game.Font.HouseScript, 0.6f, Color.Blue, RAGE.NUI.UIResText.Alignment.Left, false, true, 0); break;
                        case "Hablar": RAGE.NUI.UIResText.Draw(voice, 320, 960, RAGE.Game.Font.HouseScript, 0.6f, Color.Green, RAGE.NUI.UIResText.Alignment.Left, false, true, 0); break;
                        case "Gritar": RAGE.NUI.UIResText.Draw(voice, 320, 960, RAGE.Game.Font.HouseScript, 0.6f, Color.Red, RAGE.NUI.UIResText.Alignment.Left, false, true, 0); break;
                    }
                }

                // Situtional awareness
                Vector3 playerPosition = Player.LocalPlayer.Position;
                string zone = RAGE.Game.Zone.GetNameOfZone(playerPosition.X, playerPosition.Y, playerPosition.Z);
                int streetHash = 0, crossing = 0;
                RAGE.Game.Pathfind.GetStreetNameAtCoord(playerPosition.X, playerPosition.Y, playerPosition.Z, ref streetHash, ref crossing);
                string streetName = RAGE.Game.Invoker.Invoke<string>(RAGE.Game.Natives.GetStreetNameFromHashKey, streetHash);

                float heading = Player.LocalPlayer.GetHeading();
                string direction;
                if (heading < 10 && heading > -10)
                {
                    direction = "N";
                }
                else if (heading > 10 && heading < 80)
                {
                    direction = "NO";
                }
                else if (heading > 80 && heading < 100)
                {
                    direction = "O";
                }
                else if (heading > 100 && heading < 170)
                {
                    direction = "SO";
                }
                else if ((heading > 170 && heading < 180) || (heading < -170 && heading > -180))
                {
                    direction = "S";
                }
                else if (heading > -170 && heading < -100)
                {
                    direction = "SE";
                }
                else if (heading > -100 && heading < -80)
                {
                    direction = "E";
                }
                else if (heading > -80 && heading < -10)
                {
                    direction = "NE";
                }
                else
                {
                    direction = "N";
                }

                string situationalAwareness = zone + " | " + streetName + " | " + direction;

                RAGE.NUI.UIResText.Draw(situationalAwareness, 320, 1000, RAGE.Game.Font.HouseScript, 0.6f, Color.White, RAGE.NUI.UIResText.Alignment.Left, false, true, 0);
            }
        }

        /// <summary>
        /// Toggles the HUD visibility
        /// </summary>
        public static void SetVisible(object[] args)
        {
            visible = (bool)args[0];
        }

        /// <summary>
        /// Update the money indicator
        /// </summary>
        public static void UpdateMoney(object[] args)
        {
            money = "$" + args[0].ToString();
        }

        /// <summary>
        /// Sets the HUD language indicator
        /// </summary>
        public static void SetLanguage(object[] args)
        {
            language = args[0].ToString();
        }

        /// <summary>
        /// Sets the HUD voice type indicator
        /// </summary>
        public static void SetVoiceType(object[] args)
        {
            voice = args[0].ToString();
        }

        /// <summary>
        /// Sets the HUD admin duty indicator
        /// </summary>
        public static void SetAdminDuty(object[] args)
        {
            adminDuty = args[0].ToString();
        }

        /// <summary>
        /// Displays a timed subtitle
        /// </summary>
        public static void DisplaySubtitle(object[] args)
        {
            string subtitle = args[0].ToString();
        }

        /// <summary>
        /// Shows the loading prompt
        /// </summary>
        public static void ShowLoadingPrompt(object[] args)
        {
            int type = (int)args[0];
            RAGE.Game.Ui.ShowLoadingPrompt(type);
        }

        /// <summary>
        /// Hides the loading prompt
        /// </summary>
        public static void HideLoadingPrompt(object[] args)
        {
            RAGE.Game.Ui.RemoveLoadingPrompt();
        }

        /// <summary>
        /// Sets a new map waypoint
        /// </summary>
        public static void SetMapWaypoint(object[] args)
        {
            if(RAGE.Game.Ui.IsWaypointActive())
            {
                RAGE.Game.Ui.ClearGpsPlayerWaypoint();
                RAGE.Game.Ui.SetWaypointOff();
            }

            float x = (float)args[0];
            float y = (float)args[1];
            
            RAGE.Game.Ui.SetNewWaypoint(x, y);
        }

        /// <summary>
        /// Removes the current map waypoint
        /// </summary>
        public static void RemoveMapWaypoint(object[] args)
        {
            if (RAGE.Game.Ui.IsWaypointActive())
            {
                RAGE.Game.Ui.ClearGpsPlayerWaypoint();
                RAGE.Game.Ui.SetWaypointOff();
            }
        }

    }
}
