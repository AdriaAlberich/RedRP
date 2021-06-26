/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Drawing;
using Newtonsoft.Json;
using RAGE;
using RAGE.Elements;

namespace redrp
{

    /// <summary>
    /// DialogController
    /// </summary>
    public class DialogController : Events.Script
    {
        /// <summary>
        /// DialogController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string Title;
        private static string Text;
        private static bool HasInput;
        private static string AcceptButtonText;
        private static string CancelButtonText;

        private class Dialog
        {
            public string title;
            public string text;
            public bool hasInput;
            public string acceptButtonText;
            public string cancelButtonText;
        }

        /// <summary>
        /// DialogController initialization
        /// </summary>
        public DialogController()
        {
            // Server events
            Events.Add("showDialog", Show);
            Events.Add("hideDialog", Hide);

            // UI events
            Events.Add("requestDialogData", RequestDialogData);
            Events.Add("sendDialogAccept", SendDialogAccept);
            Events.Add("sendDialogCancel", SendDialogCancel);
        }

        /// <summary>
        /// Shows the dialog
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            dynamic results = JsonConvert.DeserializeObject<dynamic>(args[0].ToString());
            Dialog dialogData = results.ToObject<Dialog>();

            Title = dialogData.title;
            Text = dialogData.text;
            HasInput = dialogData.hasInput;
            AcceptButtonText = dialogData.acceptButtonText;
            CancelButtonText = dialogData.cancelButtonText;

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/dialog.html");
            Main.playerBrowserActive = true;
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Destroys the dialog
        /// </summary>
        /// <param name="args"></param>
        private void Hide(object[] args)
        {
            window.Destroy();
            Main.playerBrowserActive = false;
            RAGE.Ui.Cursor.Visible = false;
        }

        /// <summary>
        /// Request dialog data
        /// </summary>
        /// <param name="args"></param>
        private void RequestDialogData(object[] args)
        {
            window.ExecuteJs("dialogInit(" + Title + ", " + Text + ", " + HasInput + ", " + AcceptButtonText + ", " + CancelButtonText + ")");
        }

        /// <summary>
        /// Sends an accept request to server
        /// </summary>
        /// <param name="args"></param>
        private void SendDialogAccept(object[] args)
        {
            Events.CallRemote("dialogAccept", args[0].ToString());
        }

        /// <summary>
        /// Sends a cancel request to server
        /// </summary>
        /// <param name="args"></param>
        private void SendDialogCancel(object[] args)
        {
            Events.CallRemote("dialogCancel");
        }
    }
}
