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
    /// PlayerListController
    /// </summary>
    public class PlayerListController : Events.Script
    {
        /// <summary>
        /// PlayerListController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string data;

        /// <summary>
        /// PlayerListController initialization
        /// </summary>
        public PlayerListController()
        {
            Events.Add("showPlayerList", Show);
            Events.Add("hidePlayerList", Hide);

            Events.Add("requestPlayerListData", RequestInitializationData);
        }

        /// <summary>
        /// Shows the player list
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            data = args[0].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/playerList.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Hides the login interface
        /// </summary>
        /// <param name="args"></param>
        private void Hide(object[] args)
        {
            window.Destroy();
            Main.playerBrowserActive = false;
            RAGE.Ui.Cursor.Visible = false;
        }

        /// <summary>
        /// Sends initialization data
        /// </summary>
        /// <param name="args"></param>
        private void RequestInitializationData(object[] args)
        {
            window.ExecuteJs("setPlayerData(" + data + ")");
            window.ExecuteJs("setVisible()");
        }
    }
}
