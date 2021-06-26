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
    /// Main class (initialization and main events)
    /// </summary>
    public class Main : Events.Script
    {
        /// <summary>
        /// Local player control data
        /// </summary>
        public bool playerLogged;
        public static bool playerBrowserActive;

        /// <summary>
        /// Client initialization
        /// </summary>
        public Main()
        {
            playerBrowserActive = false;
            Events.OnGuiReady += OnGuiReadyHandler;
            Events.Tick += TickHandler;

            Events.OnPlayerCommand += OnPlayerCommandHandler;
        }

        /// <summary>
        /// Game initialization entry point
        /// </summary>
        private static void OnGuiReadyHandler()
        {
            // Remove health regeneration
            RAGE.Game.Player.SetPlayerHealthRechargeMultiplier(0.0f);

            // Remove weapons from the vehicles
            RAGE.Game.Player.DisablePlayerVehicleRewards();

            // Remove the fade out after player's death
            RAGE.Game.Misc.SetFadeOutAfterDeath(false);

            // Freeze the player until he logs in
            Player.LocalPlayer.FreezePosition(true);
        }

        /// <summary>
        /// Executes code every tick (game frame)
        /// </summary>
        /// <param name="nametags"></param>
        private void TickHandler(List<Events.TickNametagData> nametags)
        {
            DateTime dateTime = DateTime.UtcNow;
            if (!playerLogged) return;

            int keyPressed = Keybinds.Check(dateTime.Ticks);

            if (keyPressed >= 0)
            {
                Keybinds.SendKeyPressed(keyPressed);
            }
        }

        /// <summary>
        /// Workaround for legacy command detection (for logs and dynamic commands)
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="cancel"></param>
        private void OnPlayerCommandHandler(string cmd, Events.CancelEventArgs cancel)
        {
            Events.CallRemote("onChatCommand", cmd);
        }
    }
}
