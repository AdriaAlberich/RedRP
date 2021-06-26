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
    /// GeneralHelpController
    /// </summary>
    public class GeneralHelpController : Events.Script
    {
        /// <summary>
        /// GeneralHelpController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;

        /// <summary>
        /// GeneralHelpController initialization
        /// </summary>
        public GeneralHelpController()
        {
            // Server events
            Events.Add("showGeneralHelp", Show);
            Events.Add("hideGeneralHelp", Hide);
        }

        /// <summary>
        /// Shows the help page
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/generalHelp.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Destroys the help page
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
