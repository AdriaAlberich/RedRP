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
    /// LoginInterfaceController
    /// </summary>
    public class LoginInterfaceController : Events.Script
    {
        /// <summary>
        /// LoginInterfaceController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string data;

        /// <summary>
        /// LoginInterfaceController initialization
        /// </summary>
        public LoginInterfaceController()
        {
            // Server
            Events.Add("showLoginInterface", Show);
            Events.Add("hideLoginInterface", Hide);
            Events.Add("showErrorLoginInterface", ShowError);
            Events.Add("switchToCharacterSelectorLoginInterface", SwitchToCharacterSelector);

            // UI
            Events.Add("requestLoginData", RequestInitializationData);
            Events.Add("sendLogin", SendLogin);
            Events.Add("sendSpawnRequest", SendSpawnRequest);
        }

        /// <summary>
        /// Shows the login interface
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            data = args[0].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/login.html");
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
        /// Shows an error on the login interface
        /// </summary>
        /// <param name="args"></param>
        private void ShowError(object[] args)
        {
            window.ExecuteJs("showError(" + args[0] + ")");
        }

        /// <summary>
        /// Switches to the character selector
        /// </summary>
        /// <param name="args"></param>
        private void SwitchToCharacterSelector(object[] args)
        {
            window.ExecuteJs("switchToCharacterSelector(" + args[0] + ")");
        }

        /// <summary>
        /// Sends initialization data
        /// </summary>
        /// <param name="args"></param>
        private void RequestInitializationData(object[] args)
        {
            if(data == "login")
            {
                window.ExecuteJs("initialization(0," + Player.LocalPlayer.Name + ")");
            }
            else
            {
                window.ExecuteJs("initialization(1," + data + ")");
            }
        }

        /// <summary>
        /// Sends login attempt to the server
        /// </summary>
        /// <param name="args"></param>
        private void SendLogin(object[] args)
        {
            Events.CallRemote("loginAttempt", args[0].ToString(), args[1].ToString());
        }

        /// <summary>
        /// Sends a character spawn attempt to the server
        /// </summary>
        /// <param name="args"></param>
        private void SendSpawnRequest(object[] args)
        {
            Events.CallRemote("characterSpawnAttempt", args[0].ToString(), args[1].ToString());
        }
    }
}
