/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Text;

namespace redrp
{
    /// <summary>
    /// Dialog class
    /// </summary>
    public class Dialog
    {
        public DialogModel dialogModel;
        public Action<Player, string> onAcceptCallback;
        public Action<Player> onCancelCallback;

        // Class constructor
        public Dialog(string title = "Test dialog", string text = "Test", bool hasInput = false,
            string acceptButtonText = "Aceptar", string cancelButtonText = "Cancelar",
            Action<Player, string> onAcceptCallback = null, Action<Player> onCancelCallback = null)
        {
            this.dialogModel = new DialogModel();
            this.dialogModel.title = title;
            this.dialogModel.text = text;
            this.dialogModel.hasInput = hasInput;
            this.dialogModel.acceptButtonText = acceptButtonText;
            this.dialogModel.cancelButtonText = cancelButtonText;
            this.onAcceptCallback = onAcceptCallback;
            this.onCancelCallback = onCancelCallback;
        }
    }
}