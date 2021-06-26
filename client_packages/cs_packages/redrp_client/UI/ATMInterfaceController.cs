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
    /// ATMInterfaceController
    /// </summary>
    public class ATMInterfaceController : Events.Script
    {
        /// <summary>
        /// ATMInterfaceController data
        /// </summary>
        private RAGE.Ui.HtmlWindow window;
        private static string data;
        private static string moreData;
        private static int bank;
        private static string nextPage ;

        /// <summary>
        /// ATMInterfaceController initialization
        /// </summary>
        public ATMInterfaceController()
        {
            // Server events
            Events.Add("showATMInterface", Show);
            Events.Add("hideATMInterface", Hide);
            Events.Add("switchPageATMInterface", SwitchPage);
            Events.Add("activateButtonsATMInterface", ActivateButtons);

            // UI events
            Events.Add("requestATMData", RequestInitializationData);
            Events.Add("sendSelectedCard", SendSelectedCard);
            Events.Add("sendPin", SendPin);
            Events.Add("openDeposit", OpenDeposit);
            Events.Add("openWithdraw", OpenWithdraw);
            Events.Add("openTransfer", OpenTransfer);
            Events.Add("openHistory", OpenHistory);
            Events.Add("cancelOperation", CancelOperation);
            Events.Add("makeDeposit", MakeDeposit);
            Events.Add("makeWithdraw", MakeWithdraw);
            Events.Add("makeTransfer", MakeTransfer);
            Events.Add("confirmDeposit", ConfirmDeposit);
            Events.Add("confirmWithdraw", ConfirmWithdraw);
            Events.Add("confirmTransfer", ConfirmTransfer);
            Events.Add("closeATM", CloseATM);
        }

        /// <summary>
        /// Shows the main interface
        /// </summary>
        /// <param name="args"></param>
        private void Show(object[] args)
        {
            bank = int.Parse(args[0].ToString());
            data = args[0].ToString();

            RAGE.Ui.HtmlWindow window = new RAGE.Ui.HtmlWindow("html/atmInterface.html");
            Main.playerBrowserActive = true;
            RAGE.Chat.Activate(false);
            RAGE.Ui.Cursor.Visible = true;
        }

        /// <summary>
        /// Destroys the interface
        /// </summary>
        /// <param name="args"></param>
        private void Hide(object[] args)
        {
            window.Destroy();
            Main.playerBrowserActive = false;
            RAGE.Chat.Activate(true);
            RAGE.Ui.Cursor.Visible = false;
        }

        /// <summary>
        /// Switch to a different ATM page
        /// </summary>
        /// <param name="args"></param>
        private void SwitchPage(object[] args)
        {
            window.ExecuteJs("switchToPage(" + args[0].ToString() + ", " + args[1].ToString() + ", " + args[2].ToString() + ")");
        }

        /// <summary>
        /// Activate ATM buttons
        /// </summary>
        /// <param name="args"></param>
        private void ActivateButtons(object[] args)
        {
            window.ExecuteJs("activateButtons()");
        }

        /// <summary>
        /// Request initialization data when ready
        /// </summary>
        /// <param name="args"></param>
        private void RequestInitializationData(object[] args)
        {
            window.ExecuteJs("atmInitialization(" + bank + ", " + data + ")");
        }

        /// <summary>
        /// Send selected card number to server
        /// </summary>
        /// <param name="args"></param>
        private void SendSelectedCard(object[] args)
        {
            Events.CallRemote("atmSendCard", args[0].ToString());
        }

        /// <summary>
        /// Send card PIN to server
        /// </summary>
        /// <param name="args"></param>
        private void SendPin(object[] args)
        {
            Events.CallRemote("atmSendPin", args[0].ToString());
        }

        /// <summary>
        /// Request a new deposit
        /// </summary>
        /// <param name="args"></param>
        private void OpenDeposit(object[] args)
        {
            Events.CallRemote("atmOpenDeposit");
        }

        /// <summary>
        /// Request a new withdraw
        /// </summary>
        /// <param name="args"></param>
        private void OpenWithdraw(object[] args)
        {
            Events.CallRemote("atmOpenWithdraw");
        }

        /// <summary>
        /// Request a new transfer
        /// </summary>
        /// <param name="args"></param>
        private void OpenTransfer(object[] args)
        {
            Events.CallRemote("atmOpenTransfer");
        }

        /// <summary>
        /// Request open de account history
        /// </summary>
        /// <param name="args"></param>
        private void OpenHistory(object[] args)
        {
            Events.CallRemote("atmOpenHistory");
        }

        /// <summary>
        /// Request cancel the current operation
        /// </summary>
        /// <param name="args"></param>
        private void CancelOperation(object[] args)
        {
            Events.CallRemote("atmCancelOperation");
        }

        /// <summary>
        /// Send deposit data
        /// </summary>
        /// <param name="args"></param>
        private void MakeDeposit(object[] args)
        {
            Events.CallRemote("atmMakeDeposit", args[0].ToString(), args[1].ToString());
        }

        /// <summary>
        /// Sends withdraw data
        /// </summary>
        /// <param name="args"></param>
        private void MakeWithdraw(object[] args)
        {
            Events.CallRemote("atmMakeWithdraw", args[0].ToString(), args[1].ToString());
        }

        /// <summary>
        /// Sends transfer data
        /// </summary>
        /// <param name="args"></param>
        private void MakeTransfer(object[] args)
        {
            Events.CallRemote("atmMakeTransfer", args[0].ToString(), args[1].ToString(), args[2].ToString());
        }

        /// <summary>
        /// Confirms current deposit
        /// </summary>
        /// <param name="args"></param>
        private void ConfirmDeposit(object[] args)
        {
            Events.CallRemote("atmConfirmDeposit");
        }

        /// <summary>
        /// Confirms current withdraw
        /// </summary>
        /// <param name="args"></param>
        private void ConfirmWithdraw(object[] args)
        {
            Events.CallRemote("atmConfirmWithdraw");
        }

        /// <summary>
        /// Confirms current transfer
        /// </summary>
        /// <param name="args"></param>
        private void ConfirmTransfer(object[] args)
        {
            Events.CallRemote("atmConfirmTransfer");
        }

        /// <summary>
        /// Finish ATM session
        /// </summary>
        /// <param name="args"></param>
        private void CloseATM(object[] args)
        {
            Events.CallRemote("atmClose");
        }
    }
}
