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
    /// Dialog data model
    /// </summary>
    public class DialogModel
    {
        public string title;
        public string text;
        public bool hasInput;
        public string acceptButtonText;
        public string cancelButtonText;

        // Class constructor
        public DialogModel(string title = "Test dialog", string text = "Test", bool hasInput = false,
            string acceptButtonText = "Aceptar", string cancelButtonText = "Cancelar")
        {
            this.title = title;
            this.text = text;
            this.hasInput = hasInput;
            this.acceptButtonText = acceptButtonText;
            this.cancelButtonText = cancelButtonText;
        }
    }
}