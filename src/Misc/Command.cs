/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace redrp
{
    /// <summary>
    /// Raw command handler (legacy style)
    /// </summary>
    public class Command : Script
    {

        /// <summary>
        /// Command event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="command"></param>
        [RemoteEvent("onChatCommand")]
        public void OnChatCommand(Client sender, string command)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                // CMD Logging
                Log.CmdLog(player.name + " (" + player.character.name + "): " + command);

                // ANIMATION CMD handling
                if (!player.forcedAnimation)
                {
                    foreach (Animation anim in Global.Animations)
                    {
                        if (anim.command.ToLower() == command.ToLower())
                        {
                            anim.start(player);
                        }
                    }
                }
            }
        }

    }
}
