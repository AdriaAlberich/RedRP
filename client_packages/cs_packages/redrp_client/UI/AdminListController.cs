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
    /// AdminListController
    /// </summary>
    public class AdminListController : Events.Script
    {
        /// <summary>
        /// AdminListController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string adminData;

        /// <summary>
        /// AdminListController initialization
        /// </summary>
        public AdminListController()
        {
            Events.Add("showAdminList", Show);
            Events.Add("hideAdminList", Hide);

            Events.Add("requestAdminListData", RequestInitializationData);
        }

        /// <summary>
        /// Shows the admin list
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            adminData = args[0].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/adminList.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Hides the admin list
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
            window.ExecuteJs("setAdminData(" + adminData + ")");
            window.ExecuteJs("setVisible()");
        }
    }
}
