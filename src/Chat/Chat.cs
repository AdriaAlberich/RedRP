/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
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
    /// Chat system and commands
    /// </summary>
    public class Chat : Script
    {
        
        /// <summary>
        /// Chat eventhandler
        /// </summary>
        /// <param name="sender">The player who sends the chat message</param>
        /// <param name="text">The message</param>
        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                player.afkTime = 0;
                if (player.character.dying == 0)
                {
                    //text = Utils.processAccents(text);
                    switch(player.character.voiceType)
                    {
                        case 0: ProcessPlayerChat((int)Global.ChatChannel.Normal, player, text); break;
                        case 1: ProcessPlayerChat((int)Global.ChatChannel.Shout, player, text); break;
                        case 2: ProcessPlayerChat((int)Global.ChatChannel.Whisper, player, text); break;
                    }
                    
                }
            }
        }

        /// <summary>
        /// Process the player chat input
        /// </summary>
        /// <param name="channel">The chat channel</param>
        /// <param name="player">The player who sent the message</param>
        /// <param name="text">The message</param>
        public static void ProcessPlayerChat(int channel, Player player, string text)
        {
            string secondLine = string.Empty;
            if (player.character.antiFlood == 0)
            {
                if(text.Length > Global.MaxChatLineLength)
                {
                    secondLine = text.Substring(Global.MaxChatLineLength, text.Length - Global.MaxChatLineLength);
                    text = text.Remove(Global.MaxChatLineLength, secondLine.Length);
                }

                // Process the text entered by player
                foreach (Player target in Player.Players)
                {
                    // Same dimension
                    if (target.user.Dimension == player.user.Dimension)
                    {
                        // If player is nearby
                        if (target.NearPosition(player.user.Position, Global.ChatChannelDistance[channel]))
                        {
                            switch (channel)
                            {
                                case (int)Global.ChatChannel.Ooc:
                                    {
                                        if(secondLine.Length > 0)
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "[OOC] " + player.character.showName + ": (( " + text + " ...");
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "... " + secondLine + " ))");
                                        }
                                        else
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "[OOC] " + player.character.showName + ": (( " + text + " ))");
                                        }
                                        break;
                                    }
                                case (int)Global.ChatChannel.Action:
                                    {
                                        if (secondLine.Length > 0)
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + player.character.showName + " " + text + " ...");
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "... " + secondLine);
                                        }
                                        else
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + player.character.showName + " " + text);
                                        }
                                        break;
                                    }
                                case (int)Global.ChatChannel.Environment:
                                    {
                                        if (secondLine.Length > 0)
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "[" + player.character.showName + "] " + text + " ...");
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "..." + secondLine);
                                        }
                                        else
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatChannelColors[channel] + "}" + "[" + player.character.showName + "] " + text);
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        // First we choose the colour tone
                                        string color;

                                        double distance = target.user.Position.DistanceTo(player.user.Position) + 1.0;

                                        if (distance <= Global.ChatChannelDistance[channel] / 5)
                                        {
                                            color = Global.ChatGradientColor1;
                                        }
                                        else if (distance <= Global.ChatChannelDistance[channel] / 4)
                                        {
                                            color = Global.ChatGradientColor2;
                                        }
                                        else if (distance <= Global.ChatChannelDistance[channel] / 3)
                                        {
                                            color = Global.ChatGradientColor3;
                                        }
                                        else if (distance <= Global.ChatChannelDistance[channel] / 2)
                                        {
                                            color = Global.ChatGradientColor4;
                                        }
                                        else
                                        {
                                            color = Global.ChatGradientColor5;
                                        }

                                        // Send message
                                        switch (channel)
                                        {
                                            case (int)Global.ChatChannel.Normal:
                                                {
                                                    if (player.character.activeLanguage != null)
                                                    {
                                                        if (secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.talk + ": " + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine);
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.talk + ": " + text);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " dice: " + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine);
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " dice: " + text);
                                                        }
                                                    }

                                                    break;
                                                }
                                            case (int)Global.ChatChannel.Whisper:
                                                {
                                                    if (player.character.activeLanguage != null)
                                                    {
                                                        if (secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.whisper + ": " + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine);
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.whisper + ": " + text);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if(secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " susurra: " + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine);
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " susurra: " + text);
                                                        }
                                                        
                                                    }

                                                    break;
                                                }
                                            case (int)Global.ChatChannel.Shout:
                                                {
                                                    if (player.character.activeLanguage != null)
                                                    {
                                                        if (secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.shout + ": ¡" + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine + "!");
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "[" + player.character.activeLanguage.abreviation + "] " + player.character.showName + " " + player.character.activeLanguage.shout + ": ¡" + text + "!");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (secondLine.Length > 0)
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " grita: ¡" + text + " ...");
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + "... " + secondLine + "!");
                                                        }
                                                        else
                                                        {
                                                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.character.showName + " grita: ¡" + text + "!");
                                                        }
                                                    }

                                                    break;
                                                }
                                            default:
                                                {
                                                    NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + color + "}" + player.name + " dice: " + text);
                                                }

                                                break;
                                        }

                                        break;
                                    }
                            }

                            player.character.antiFlood = 2;
                        }
                    }
                }
            }
        }

        #region Commands

        /// <summary>
        /// Out of character chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("ooc", "~y~Uso: /o <texto>", Alias = "o", GreedyArg = true)]
        public void OocCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                //text = Utils.processAccents(text);
                ProcessPlayerChat((int)Global.ChatChannel.Ooc, player, text);
            }
        }

        /// <summary>
        /// Action chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("me", "~y~Uso: /me <texto>", GreedyArg = true)]
        public void MeCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.dying == 0)
                {
                    //text = Utils.processAccents(text);
                    ProcessPlayerChat((int)Global.ChatChannel.Action, player, text);
                }  
            }
        }

        /// <summary>
        /// Near action message command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("ame", "~y~Uso: /ame <texto>", GreedyArg = true)]
        public void AmeCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.dying == 0)
                {
                    //text = Utils.processAccents(text);
                    player.SendAme(text);
                }
            }
        }

        /// <summary>
        /// Environment chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("do", "~y~Uso: /do <texto>", Alias = "cme", GreedyArg = true)]
        public void DoCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                //text = Utils.processAccents(text);
                ProcessPlayerChat((int)Global.ChatChannel.Environment, player, text);
            }
        }

        /// <summary>
        /// Shout chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("gritar", "~y~Uso: /g <texto>", Alias = "g", GreedyArg = true)]
        public void ShoutCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.dying == 0)
                {
                    //text = Utils.processAccents(text);
                    ProcessPlayerChat((int)Global.ChatChannel.Shout, player, text);
                }
            }
        }

        /// <summary>
        /// Close chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("cerca", "~y~Uso: /c <texto>", Alias = "c", GreedyArg = true)]
        public void CloseCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.dying == 0)
                {
                    //text = Utils.processAccents(text);
                    ProcessPlayerChat((int)Global.ChatChannel.Whisper, player, text);
                }
            }
        }

        /// <summary>
        /// Whisper command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="target">The target name or id</param>
        /// <param name="text">The message</param>
        [Command("susurro", "~y~Uso: /s <id> <texto>", Alias = "s", GreedyArg = true)]
        public void WhisperCmd(Client sender, string target, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.dying == 0)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);
                    if (targetPlayer != null)
                    {
                        //text = Utils.processAccents(text);
                        if (player.Equals(targetPlayer))
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~No puedes mandarte susurros a ti mismo.");
                        }
                        else
                        {
                            if (player.GetDistanceBetweenPlayers(targetPlayer) <= 2.0)
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "!{" + Global.ChatPMColor + "}" + "[SUSURRO] Para " + targetPlayer.character.showName + ": " + text);
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "!{" + Global.ChatPMReceivedColor + "}" + "[SUSURRO] De " + player.character.showName + ": " + text);
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Demasiado lejos para escucharte.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Admin chat command
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("a", "~y~Uso: /a <texto>", GreedyArg = true)]
        public void AdminChatCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 0)
                {
                    //text = Utils.processAccents(text);
                    foreach (Player target in Player.Players)
                    {
                        if (target.admin > 0)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(target.user, "!{" + Global.ChatAdminColor + "}" + "[Admin " + player.admin + "] " + player.name + ": " + text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Admin-user private chat
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("ca", "~y~Uso: /ca <texto>", GreedyArg = true)]
        public void AdminChannelCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.adminChannel != -1)
                {
                    foreach (Player target in Player.Players)
                    {
                        if (target.character != null)
                        {
                            if (target.adminChannel == player.adminChannel)
                            {
                                //text = Utils.processAccents(text);
                                if (player.admin > 0)
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(target.user, "~o~[CA] Admin " + player.name + ": " + text);
                                }
                                else
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(target.user, "~o~[CA] Usuario " + player.name + ": " + text);
                                }
                            }
                        }
                    }
                }
                else
                {
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~No tienes un canal admin abierto.");
                }
            }
        }

        /// <summary>
        /// Global admin announces
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="text">The message</param>
        [Command("ma", "~y~Uso: /ma <texto>", GreedyArg = true)]
        public void AdminMessageCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 0)
                {
                    //text = Utils.processAccents(text);
                    NAPI.Chat.SendChatMessageToAll("!{" + Global.ChatGlobalColor + "~" + "[Administración] " + text);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender">The player who sends the message</param>
        /// <param name="target">The target name or id</param>
        /// <param name="text">The message</param>
        [Command("mp", "~y~Uso: /mp <id> <texto>", GreedyArg = true)]
        public void PrivateMessageCmd(Client sender, string target, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                Player targetPlayer = Player.GetByIdOrName(target);

                if (targetPlayer != null)
                {
                    if (targetPlayer.character.mp)
                    {
                        if (player.character.antiFlood == 0)
                        {
                            if (player.Equals(targetPlayer))
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~No puedes mandarte mensajes privados a ti mismo.");
                            }
                            else
                            {
                                //text = Utils.processAccents(text);
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "!{" + Global.ChatPMColor + "}" + "[MP] Para " + targetPlayer.character.showName + " (" + targetPlayer.name + "): " + text);
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "!{" + Global.ChatPMReceivedColor + "}" + "[MP] " + player.character.showName + " (" + player.name + "): " + text);
                                player.character.antiFlood = 2;
                            }
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~¡Ops! Tiene el canal de mensajes privados desactivado.");
                    }
                }   
            }
        }

        /// <summary>
        /// Activate/deactivate the private messages channel
        /// </summary>
        /// <param name="sender"></param>
        [Command("dmp")]
        public void DmpCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.character.mp)
                {
                    player.character.mp = false;
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Canal de mensajes privados desactivado.");
                }
                else
                {
                    player.character.mp = true;
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~g~Canal de mensajes privados activado.");
                }
            }
        }

        /// <summary>
        /// Shows a short description above player's head
        /// </summary>
        /// <param name="sender">The player who sends the command</param>
        /// <param name="text">The description</param>
        [Command("yo", "~y~Uso: /yo <texto> | Descripción corta encima de la cabeza.", GreedyArg = true)]
        public void YoCmd(Client sender, string text)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                //text = Utils.processAccents(text);
                player.SetYo(text);
                NAPI.Chat.SendChatMessageToPlayer(player.user, "~g~Nueva descripción: " + text);
            }
        }

        /// <summary>
        /// Deletes the short description above player's head
        /// </summary>
        /// <param name="sender">The player who sends the command</param>
        [Command("yob", GreedyArg = true)]
        public void Yob(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                player.DeleteYo();
                NAPI.Notification.SendNotificationToPlayer(player.user, "~r~Descripción borrada.");
            }
        }

        #endregion

    }
}
