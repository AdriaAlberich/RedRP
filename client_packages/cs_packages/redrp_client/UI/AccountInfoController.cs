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
    /// AccountInfoController
    /// </summary>
    public class AccountInfoController : Events.Script
    {
        /// <summary>
        /// AccountInfoController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string accountName;
        private static string accountData;
        private static string characterName;
        private static string characterGeneralData;
        private static string characterPropertyData;
        private static string characterExperienceData;

        /// <summary>
        /// AccountInfoController initialization
        /// </summary>
        public AccountInfoController()
        {
            // Server events
            Events.Add("showAccountInfo", Show);
            Events.Add("hideAccountInfo", Hide);

            // UI events
            Events.Add("requestAccountInfoData", RequestInitializationData);
        }

        /// <summary>
        /// Shows the account info page
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            accountName = args[0].ToString();
            accountData = args[1].ToString();
            characterName = args[2].ToString();
            characterGeneralData = args[3].ToString();
            characterPropertyData = args[4].ToString();
            characterExperienceData = args[5].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/accountInfo.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Hides the account info page
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
            window.ExecuteJs("setAccountName(" + accountName + ")");
            window.ExecuteJs("setAccountData(" + accountData + ")");
            window.ExecuteJs("setCharacterName(" + characterName + ")");
            window.ExecuteJs("setCharacterGeneralData(" + characterGeneralData + ")");
            window.ExecuteJs("setCharacterProperties(" + characterPropertyData + ")");
            window.ExecuteJs("setCharacterExperience(" + characterExperienceData + ")");
            window.ExecuteJs("setVisible()");
        }
    }
}
