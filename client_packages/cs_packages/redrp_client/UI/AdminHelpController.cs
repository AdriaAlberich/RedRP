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
    /// AdminHelpController
    /// </summary>
    public class AdminHelpController : Events.Script
    {
        /// <summary>
        /// AdminHelpController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;

        /// <summary>
        /// AccountInfoController initialization
        /// </summary>
        public AdminHelpController()
        {
            Events.Add("showAdminHelp", Show);
            Events.Add("hideAdminHelp", Hide);
        }

        /// <summary>
        /// Shows the admin help
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/adminHelp.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Hides the admin help
        /// </summary>
        /// <param name="args"></param>
        private void Hide(object[] args)
        {
            window.Destroy();
            Main.playerBrowserActive = false;
            RAGE.Ui.Cursor.Visible = false;
        }
    }
}
