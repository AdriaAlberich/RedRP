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
    /// Admin commands and auxiliary functions
    /// </summary>
    public class Admin : Script
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////COMMANDS//////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Commands
        /// <summary>
        /// Admin help command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("aa")]
        public void AdminHelp(Client sender)
        {
            Player player = Player.Exists(sender);
            if(player.character != null)
            {
                if (player.admin > 0)
                {
                    if(player.showingGui == -1)
                    {
                        ShowAdminHelp(player);
                    }

                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~g~COMANDOS TESTING:");
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~/pos, /skin, /god, /curarse, /reparar, /veh, /vehcolor, /arma");
                }
            }
        }

        /// <summary>
        /// Check id command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target player name or id</param>
        [Command("id", "~y~Uso: /id <id>")]
        public void IdCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if(player.admin > 0)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if (targetPlayer.character != null)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, targetPlayer.name + " - " + targetPlayer.character.cleanName + " (" + targetPlayer.id + ")");
                        }
                        else
                        {
                            player.DisplayNotification("~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Admin service command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("servicioadmin", Alias = "sa")]
        public void AdminServiceCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 0)
                {
                    player.SetAdminDuty(!player.adminDuty);
                }
            }
        }

        /// <summary>
        /// Kick command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="reason">Kick reason</param>
        [Command("echar", "~y~Uso: /echar <id> <razón>", GreedyArg = true)]
        public void KickCmd(Client sender, string target, string reason)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if(targetPlayer != null)
                    {
                        NAPI.Chat.SendChatMessageToAll("~r~[Administración] " + targetPlayer.name + " (" + targetPlayer.character.cleanName + ") ha sido expulsado por " + player.character.cleanName + ". Razón: " + reason);
                        targetPlayer.Kick(player, reason);
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Ban command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="time">Ban time in hours</param>
        /// <param name="reason">Ban reason</param>
        [Command("bloquear", "~y~Uso: /bloquear <id> <duración> <razón> | -1 = permanente global, 0 = permanente", GreedyArg = true)]
        public void BanCmd(Client sender, string target, int duration, string reason)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if(duration > -2 && duration <= 8784)
                        {
                            switch(duration)
                            {
                                case -1:
                                    {
                                        NAPI.Chat.SendChatMessageToAll("~r~[Administración] " + targetPlayer.name + " (" + targetPlayer.character.cleanName + ") ha sido bloqueado globalmente por " + player.name + ". Razón: " + reason);
                                        targetPlayer.Ban(player, -1, reason);
                                        break;
                                    }
                                case 0:
                                    {
                                        NAPI.Chat.SendChatMessageToAll("~r~[Administración] " + targetPlayer.name + " (" + targetPlayer.character.cleanName + ") ha sido bloqueado permanentemente por " + player.name + ". Razón: " + reason);
                                        targetPlayer.Ban(player, 0, reason);
                                        break;
                                    }
                                default:
                                    {
                                        NAPI.Chat.SendChatMessageToAll("~r~[Administración] " + targetPlayer.name + " (" + targetPlayer.character.cleanName + ") ha sido bloqueado temporalmente por " + player.name + ". Duración: " + duration + " horas. Razón: " + reason);
                                        targetPlayer.Ban(player, duration * 3600, reason);
                                        break;
                                    }

                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Duración tiene que ser superior o igual a -1 e inferior o igual a 8784 (un año).");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Goto fixed location command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="id">The target</param>
        [Command("ir", "~y~Uso: /ir | /ir <id>")]
        public void GotoLocCmd(Client sender, int id = 0)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    if(id > 0)
                    {
                        AdminTeleport teleport = AdminTeleport.GetById(id);
                        if(teleport != null)
                        {
                            teleport.Teleport(player);
                        }
                        else
                        {
                            player.DisplayNotification("~r~Localización inexistente.");
                        }
                    }
                    else
                    {
                        if(id == 0)
                        {
                            AdminTeleport.GenerateSelectionMenu(player);
                        }
                        else
                        {
                            player.DisplayNotification("~r~ID debe ser mayor que 0.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create a custom teleport
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="name">The name of the teleport</param>
        [Command("clocadmin", "~y~Uso: /clocadmin <nombre>", GreedyArg = true)]
        public void CLocAdmin(Client sender, string name)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    if(name.Length > 0 && name.Length <= 32)
                    {
                        if(AdminTeleport.Create(name, player.user.Position, player.user.Rotation.Z, player.user.Dimension))
                        {
                            player.DisplayNotification("~g~Creada nueva localización admin.");
                        }
                        else
                        {
                            player.DisplayNotification("~r~No se pudo crear la nueva localización admin.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~No se pudo crear la nueva localización admin.");
                    }
                }
            }
        }

        /// <summary>
        /// Edit a custom teleport
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="id">Teleport id</param>
        /// <param name="option">Edit option</param>
        /// <param name="name">Name of the teleport</param>
        [Command("elocadmin", "~y~Uso: /elocadmin <id> <'nombre' o 'posicion'> [nuevoNombre]", GreedyArg = true)]
        public void ELocAdmin(Client sender, int id, string option, string name = "")
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    if (id > 0)
                    {
                        AdminTeleport teleport = AdminTeleport.GetById(id);
                        if (teleport != null)
                        {
                            switch(option)
                            {
                                case "nombre":
                                    {
                                        if(name.Length > 0 && name.Length <= 32)
                                        {
                                            teleport.name = name;
                                            teleport.Save();
                                            player.DisplayNotification("~g~Editado nombre correctamente.");
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~r~Nombre no válido.");
                                        }

                                        break;
                                    }
                                case "posicion":
                                    {
                                        teleport.position = player.user.Position;
                                        teleport.heading = player.user.Rotation.Z;
                                        teleport.dimension = player.user.Dimension;
                                        teleport.Save();

                                        player.DisplayNotification("~g~Editada posición correctamente.");

                                        break;
                                    }
                                default:
                                    {
                                        player.DisplayNotification("~r~Opción inexistente.");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~r~Localización inexistente.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~ID debe ser mayor que 0.");
                    }
                }
            }
        }

        /// <summary>
        /// Classic Goto command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("ira", "~y~Uso: /ira <id>")]
        public void GotoCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        Vector3 position = new Vector3(targetPlayer.user.Position.X + 2.0, targetPlayer.user.Position.Y, targetPlayer.user.Position.Z);

                        player.SetPosition(position, player.user.Rotation);

                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Te has teletransportado a la posición de " + targetPlayer.character.cleanName + ".");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Gethere command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("traer", "~y~Uso: /traer <id>")]
        public void GethereCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        Vector3 position = new Vector3(player.user.Position.X + 2.0, player.user.Position.Y, player.user.Position.Z);

                        targetPlayer.SetPosition(position, targetPlayer.user.Rotation);

                        NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Te han teletransportado.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Freeze command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("congelar", "~y~Uso: /congelar <id>")]
        public void FreezeCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        NAPI.Player.FreezePlayer(targetPlayer.user, true);

                        NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Te han congelado.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Unfreeze command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("dcongelar", "~y~Uso: /dcongelar <id>")]
        public void UnfreezeCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        NAPI.Player.FreezePlayer(targetPlayer.user, false);

                        NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Te han descongelado.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Spectate command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("espiar", "~y~Uso: /espiar <id>")]
        public void SpectateCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Estas espiando a " + targetPlayer.character.cleanName + " (" + targetPlayer.name + "). ID: " + targetPlayer.id + ".");
                        NAPI.Player.SetPlayerToSpectatePlayer(player.user, targetPlayer.user);
                    }
                }
            }
        }

        /// <summary>
        /// Unspectat command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("noespiar")]
        public void NoEspiarCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    NAPI.Player.UnspectatePlayer(player.user);
                }
            }
        }

        /// <summary>
        /// Check player's data
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("revisar", "~y~Uso: /revisar <id> | Muestra información del jugador y personaje.")]
        public void CheckCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        Player.ShowAccountInfoPage(targetPlayer, player);
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Override weather command (disables automatic system)
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="weatherId">Weather id</param>
        [Command("clima", "~y~Uso: /clima <numero 1-12> | No usar el 9 | -1 vuelve al modo automático.")]
        public void WeatherCmd(Client sender, int weatherId)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 2)
                {
                    if(weatherId >= 0 && weatherId < 13)
                    {
                        World.overrideWeather = true;
                        World.currentWeather = weatherId;
                        NAPI.World.SetWeather(weatherId.ToString());
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~g~Cambiado clima a " + weatherId);
                    }
                    else
                    {
                        if(weatherId == -1)
                        {
                            World.overrideWeather = false;
                            World.ProcessCurrentWeather();
                            NAPI.World.SetWeather(World.currentWeather.ToString());
                            NAPI.Notification.SendNotificationToPlayer(player.user, "~g~Clima automático");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Goto coords command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="X">X component</param>
        /// <param name="Y">Y component</param>
        /// <param name="Z">Z component</param>
        [Command("irac", "~y~Uso: /irac <x> <y> <z>")]
        public void GotoCoordCmd(Client sender, double X, double Y, double Z)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Vector3 position = new Vector3(X, Y, Z);

                    player.SetPosition(position, player.user.Rotation);

                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Te has teletransportado a " + X + "," + Y + "," + Z);
                }
            }
        }

        /// <summary>
        /// Step command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="steps">Number of steps</param>
        [Command("paso", "~y~Uso: /paso [pasos]")]
        public void StepCmd(Client sender, int steps = 1)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Vector3 position = player.GetInFrontOf(steps);

                    player.SetPosition(position, player.user.Rotation);
                }
            }
        }

        /// <summary>
        /// Revive command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="hospital">If must respawn on the site or on the hospital</param>
        [Command("revivir", "~y~Uso: /revivir <id> [hospital? 1-0]")]
        public void ReviveCmd(Client sender, string target, int hospital = 1)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if(targetPlayer.character != null)
                        {
                            if(targetPlayer.character.dying == 1)
                            {
                                targetPlayer.Revive(hospital == 1);
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador te ha revivido.");
                                Notification(player.name + " ha revivido a " + targetPlayer.character.cleanName + ".");
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~El jugador no está muerto.");
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Set health command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="health">Health points</param>
        [Command("sethp", "~y~Uso: /sethp <id> [salud -1 a 100]")]
        public void SetHpCmd(Client sender, string target, int health = 100)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if (targetPlayer.character != null)
                        {
                            if (targetPlayer.character.dying == 0)
                            {
                                if(health >= -1)
                                {
                                    targetPlayer.SetHealth(health);
                                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador ha modificado la salud de tu personaje.");
                                    Notification(player.name + " ha modificado la salud de " + targetPlayer.character.cleanName + " en " + health + " puntos.");
                                }
                                else
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~La salud no puede ser negativa.");
                                }
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~El jugador está muerto, usa /revivir.");
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Set dimension command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="dimension">Dimension</param>
        [Command("setdim", "~y~Uso: /setdim <id> [dimension]")]
        public void SetDimCmd(Client sender, string target, uint dimension = 1)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if (targetPlayer.character != null)
                        {
                            if(dimension > 0)
                            {
                                targetPlayer.user.Dimension = dimension;
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador te ha cambiado a la dimensión " + dimension + ".");
                                Notification(player.name + " ha cambiado la dimension de " + targetPlayer.character.cleanName + " por " + dimension + ".");
                            }
                            else
                            {
                                player.DisplayNotification("~r~Dimensión no válida.");
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Slap command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="reason"></param>
        [Command("cachete", "~y~Uso: /cachete <id> [motivo]", GreedyArg = true)]
        public void SlapCmd(Client sender, string target, string reason = " ")
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if (targetPlayer.character != null)
                        {
                            if (targetPlayer.character.dying == 0)
                            {
                                Vector3 newPosition = new Vector3(targetPlayer.user.Position.X, targetPlayer.user.Position.Y, targetPlayer.user.Position.Z + 4.0);
                                targetPlayer.SetPosition(newPosition, targetPlayer.user.Rotation);
                                NAPI.Player.PlaySoundFrontEnd(targetPlayer.user, "Hit", "RESPAWN_SOUNDSET");
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador te ha pegado un cachete por: " + reason + ".");
                                Notification(player.name + " le ha pegado un cachete a " + targetPlayer.character.cleanName + " por: " + reason + ".");
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~El jugador está muerto.");
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Channel admin command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        [Command("caon", "~y~Uso: /caon <id>")]
        public void ChannelAdminCmd(Client sender, string target)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if(player.admin >= 1)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if(!targetPlayer.Equals(player))
                        {
                            if (targetPlayer.adminChannel == -1)
                            {
                                targetPlayer.adminChannel = player.id;
                                if (player.adminChannel == -1)
                                {
                                    player.adminChannel = player.id;
                                }
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Has invitado a " + targetPlayer.name + " a tu canal admin.");
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~" + player.name + " te ha invitado a su canal admin. Usa /ca para hablar por él.");
                            }
                            else
                            {
                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador siendo atendido por otro admin.");
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~No te puedes invitar a ti mismo.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Close channel admin command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("caoff", "~y~Uso: /caoff")]
        public void CloseChannelAdminCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    foreach (Player target in Player.Players)
                    {
                        if (target.adminChannel == player.id)
                        {
                            target.adminChannel = -1;
                            if (target.id != player.id)
                            {
                                NAPI.Chat.SendChatMessageToPlayer(target.user, "~y~" + player.name + " ha cerrado su canal admin.");
                            }
                        }
                    }

                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~y~Has cerrado tu canal admin y expulsados todos los usuarios invitados.");
                }
            }
        }

        /// <summary>
        /// Attend report command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="id">Report id</param>
        [Command("atender", "~y~Uso: /atender <id>")]
        public void AttendReportCmd(Client sender, int id)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    AdminReport report = AdminReport.GetById(id);

                    if(report != null)
                    {
                        if(report.admin == null)
                        {
                            report.admin = player;
                            Notification(player.name + " ha atendido el reporte de " + report.reporter.character.cleanName + " (" + report.reporter.id + ").");
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~b~Reporte: " + report.reportText);
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Este reporte ya está siendo atendido.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Este reporte ya no existe.");
                    }
                }
            }
        }

        /// <summary>
        /// Open the report list command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("reportes")]
        public void ReportsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 1)
                {
                    if(Global.AdminReports.Count > 0)
                    {
                        AdminReport.GenerateReportsMenu(player);
                    }
                    else
                    {
                        player.DisplayNotification("~g~No hay reportes pendientes.");
                    }
                }
            }
        }

        /// <summary>
        /// Cleans up the report list
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("eliminarreportes")]
        public void CleanUpReportsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 3)
                {
                    Global.AdminReports = new List<AdminReport>();
                    Notification(player.name + " ha limpiado la lista de reportes.");
                }
            }
        }

        /// <summary>
        /// Edit character
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="option">Edit option</param>
        /// <param name="value">New option value</param>
        [Command("setpj", "~y~Uso: /setpj <id> <opcion> <valor>\n" + 
                          "Opciones: edad, dinero")]
        public void SetCharacterCmd(Client sender, string target, string option, int value)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin >= 2)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if (targetPlayer.character != null)
                        {
                            option = option.ToLower();
                            switch(option)
                            {
                                //Player age
                                case "edad":
                                    {
                                        if(player.admin >= 3)
                                        {
                                            if(value >= 16 && value <= 90)
                                            {
                                                targetPlayer.character.age = value;
                                                Util.UpdateDBField("Characters", "age", value.ToString(), "id", targetPlayer.character.sqlid.ToString());
                                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador te ha modificado la edad a " + value + " años.");
                                                Notification(player.name + " ha cambiado la edad de " + targetPlayer.character.cleanName + " a " + value + " años.");
                                            }
                                            else
                                            {
                                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Edad debe estar entre 16 y 90 ambos incluidos.");
                                            }
                                        }
                                        else
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Nivel insuficiente.");
                                        }

                                        break;
                                    }
                                //Player pocket money
                                case "dinero":
                                    {
                                        if (player.admin >= 4)
                                        {
                                            if (value >= 0 && value <= Global.PocketMoneyLimit)
                                            {
                                                targetPlayer.SetMoney(value);
                                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Un administrador te ha modificado el dinero a " + value + " USD.");
                                                Notification(player.name + " ha cambiado el dinero de " + targetPlayer.character.cleanName + " a " + value + " USD.");
                                            }
                                            else
                                            {
                                                NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Dinero debe estar entre 0 y " + Global.PocketMoneyLimit + " ambos incluidos.");
                                            }
                                        }
                                        else
                                        {
                                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Nivel insuficiente.");
                                        }

                                        break;
                                    }
                                default:
                                    {
                                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Opción inexistente.");
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Sets the player admin level
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="level">Admin level</param>
        [Command("setadmin", "~y~Uso: /setadmin <id> <nivel>")]
        public void SetAdminCmd(Client sender, string target, int level)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin == 1337)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if(level == 0)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~r~Te han expulsado del equipo administrativo.");
                        }
                        else
                        {
                            if(targetPlayer.admin > level)
                            {
                                NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~r~Te han degradado en el equipo administrativo a nivel " + level + ".");
                            }
                            else
                            {
                                if(targetPlayer.admin == 0)
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~g~¡Felicidades! Te han invitado al equipo administrativo con nivel " + level + ".");
                                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~y~Puedes ver la lista de comandos disponibles escribiendo ~r~/ah~y~ en el chat.");
                                }
                                else
                                {
                                    NAPI.Chat.SendChatMessageToPlayer(targetPlayer.user, "~g~Te han promocionado en el equipo administrativo a nivel " + level + ".");
                                }
                            }
                        }

                        targetPlayer.admin = level;
                        Util.UpdateDBField("Players", "admin", level.ToString(), "id", targetPlayer.ToString());
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~g~Has editado el nivel admin de " + targetPlayer.name + " (" + targetPlayer.character.cleanName + ") a " + level + ".");
                    }
                }
            }
        }

        /// <summary>
        /// Give an item
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="target">Target name or id</param>
        /// <param name="itemId">Item id</param>
        /// <param name="hand">If must be given to the player's hand</param>
        /// <param name="quantity">Item quantity</param>
        [Command("daritem", "~y~Uso: /daritem <id> <itemid> [en mano? 1-0] [cantidad]")]
        public void GiveItemCmd(Client sender, string target, int itemId, int hand = 0, int quantity = -1)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    Player targetPlayer = Player.GetByIdOrName(target);

                    if (targetPlayer != null)
                    {
                        if(targetPlayer.character != null)
                        {
                            ItemData info = ItemData.GetById(itemId);
                            if (info != null)
                            {
                                if (quantity > info.quantity || quantity < 0)
                                {
                                    quantity = info.quantity;
                                    if (hand == 1)
                                    {
                                        string errMsg = Inventory.GetAddItemErrorMessage(targetPlayer.character.inventory.AddNewItem(itemId, quantity, 0));
                                        if(errMsg != "")
                                        {
                                            player.DisplayNotification("~r~" + errMsg);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~g~Item entregado correctamente.");
                                        }
                                    }
                                    else
                                    {
                                        string errMsg = Inventory.GetAddItemErrorMessage(targetPlayer.character.inventory.AddNewItem(itemId, quantity));
                                        if (errMsg != "")
                                        {
                                            player.DisplayNotification("~r~" + errMsg);
                                        }
                                        else
                                        {
                                            player.DisplayNotification("~g~Item entregado correctamente.");
                                        }
                                    }
                                }
                                else
                                {
                                    player.DisplayNotification("~r~Usos no válidos.");
                                }
                            }
                            else
                            {
                                player.DisplayNotification("~r~Item no válido.");
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~r~Jugador en selección de personaje.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Jugador no conectado.");
                    }
                }
            }
        }

        /// <summary>
        /// Create a world item
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="itemId">Item id</param>
        /// <param name="quantity"></param>
        [Command("crearitem", "~y~Uso: /crearitem <itemid> [cantidad]")]
        public void CreateItemCmd(Client sender, int itemId, int quantity = -1)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    ItemData info = ItemData.GetById(itemId);
                    if (info != null)
                    {
                        if (quantity > info.quantity || quantity < 0)
                        {
                            quantity = info.quantity;
                            Item newItem = Item.CreateItem(itemId, quantity);
                            WorldItem.Create(newItem, player.GetFootPos(), info.worldRotation, player.user.Dimension);

                            player.DisplayNotification("~g~Item creado correctamente.");
                        }
                        else
                        {
                            player.DisplayNotification("~r~Usos no válidos.");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Item no válido.");
                    }
                }
            }
        }

        /// <summary>
        /// Generate a debit card
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="pin">The debit card pin</param>
        /// <param name="accountNumber">The debit card account number (must be an actual account)</param>
        [Command("creartarjetadebito", "~y~Uso: /creartarjetadebito <pin> <numeroCuenta>", GreedyArg = true)]
        public void CreateDebitCardCmd(Client sender, string pin, string accountNumber)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    BankAccount account = BankAccount.GetAccountByNumber(accountNumber);
                    if(account != null)
                    {
                        Item newCard = Item.CreateDebitCard(account, pin);
                        if(newCard != null)
                        {
                            if(player.character.inventory.AddItem(newCard, 0) == 1)
                            {
                                player.DisplayNotification("~g~Tarjeta creada correctamente.");
                            }
                            else
                            {
                                player.DisplayNotification("~r~Mano derecha ocupada.");
                            }
                        }
                        else
                        {
                            player.DisplayNotification("~r~PIN no válido (debe tener 4 dígitos).");
                        }
                    }
                    else
                    {
                        player.DisplayNotification("~r~Cuenta bancaria inexistente.");
                    }
                }
            }
        }

        /// <summary>
        /// Clean up all world items
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("limpiaritems")]
        public void CleanUpWorldItemsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    WorldItem.DestroyAll();
                    player.DisplayNotification("~g~Items temporales limpiados. Sistema reseteado.");
                }
            }
        }

        /// <summary>
        /// Edit the nearest world item relative height
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="value">The height value</param>
        [Command("wialt", "~y~Uso: /wialt <valor>")]
        public void WorldItemHeightCmd(Client sender, double value)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    WorldItem closestItem = WorldItem.GetClosestItem(player.character.position, 2f, player.character.dimension);
                    if(closestItem != null)
                    {
                        Item item = closestItem.ToItem();
                        ItemData info = ItemData.GetById(item.id);
                        
                        Vector3 pos = closestItem.position;
                        info.worldZOffset = value;
                        NAPI.Entity.SetEntityPosition(closestItem.prop, new Vector3(pos.X, pos.Y, pos.Z - info.worldZOffset));
                        Util.UpdateDBField("Items", "worldZOffset", value.ToString(), "id", item.id.ToString());
                        player.DisplayNotification("~g~Item editado.");
                    }
                    else
                    {
                        player.DisplayNotification("~r~Ningun item cerca.");
                    }
                }
            }
        }

        /// <summary>
        /// Edit the nearest world item rotation
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="x">X rotation</param>
        /// <param name="y">Y rotation</param>
        /// <param name="z">Z rotation</param>
        [Command("wirot", "~y~Uso: /wirot <x> <y> <z>")]
        public void WorldItemRotationCmd(Client sender, float x, float y, float z)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    WorldItem closestItem = WorldItem.GetClosestItem(player.character.position, 2f, player.character.dimension);
                    if (closestItem != null)
                    {
                        Vector3 newRot = new Vector3(x, y, z);
                        Item item = closestItem.ToItem();
                        ItemData info = ItemData.GetById(item.id);
                        info.worldRotation = newRot;
                        closestItem.rotation = newRot;
                        NAPI.Entity.SetEntityRotation(closestItem.prop, closestItem.rotation);
                        Util.UpdateDBField("Items", "worldXRotation", x.ToString(), "id", item.id.ToString());
                        Util.UpdateDBField("Items", "worldYRotation", y.ToString(), "id", item.id.ToString());
                        Util.UpdateDBField("Items", "worldZRotation", z.ToString(), "id", item.id.ToString());
                        player.DisplayNotification("~g~Item editado.");
                    }
                    else
                    {
                        player.DisplayNotification("~r~Ningun item cerca.");
                    }
                }
            }
        }

        /// <summary>
        /// Create a new ATM command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="bank">Bank id</param>
        /// <param name="hasBeacon">If has map beacon or not</param>
        [Command("crearatm", "~y~Uso: /crearatm <banco -1 a 3> <tieneMarca? true o false>")]
        public void CreateATMCmd(Client sender, int bank, bool hasBeacon)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = ATM.Create(player, bank, hasBeacon);
                    if (result)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "CREADO CAJERO CORRECTAMENTE.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CREAR EL CAJERO.");
                    }
                }
            }
        }

        /// <summary>
        /// Create a new trash container command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("crearcontenedor")]
        public void CreateTrashContainerCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = Container.Create(player.user.Position, player.user.Dimension);
                    if (result)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "CREADO CONTENEDOR DE BASURA CORRECTAMENTE.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CREAR EL CONTENEDOR.");
                    }
                }
            }
        }

        /// <summary>
        /// Make trash containers visible command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("vercontenedores")]
        public void ShowTrashContainers(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if (Container.debugMode)
                    {
                        foreach(Container container in Global.Containers)
                        {
                            NAPI.Entity.DeleteEntity(container.debugMarker);
                            container.debugMarker = null;
                        }
                        Container.debugMode = false;
                        player.DisplayNotification("~g~Desactivados marcadores en contenedores de basura.");
                    }
                    else
                    {
                        foreach (Container container in Global.Containers)
                        {
                            container.debugMarker = NAPI.Marker.CreateMarker(28, container.position, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0.5f, 150, 255, 0, false);
                        }
                        Container.debugMode = true;
                        player.DisplayNotification("~g~Activados marcadores en contenedores de basura.");
                    }
                }
            }
        }

        /// <summary>
        /// Reload languages command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("cargaridiomas")]
        public void LoadLanguagesCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = Language.Load();
                    if (result)
                    {
                        Notification(player.name + " ha recargado los idiomas desde la BD.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CARGAR IDIOMAS.");
                    }
                }
            }
        }

        /// <summary>
        /// Reload animations command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("cargaranims")]
        public void LoadAnimationsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = Animation.Load();
                    result = result && AnimationCategory.Load();
                    if (result)
                    {
                        Notification(player.name + " ha recargado las animaciones desde la BD.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CARGAR ANIMACIONES.");
                    }
                }
            }
        }

        /// <summary>
        /// Reload ATMs command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("cargaratm")]
        public void LoadATMsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = ATM.Load();
                    if (result)
                    {
                        Notification(player.name + " ha recargado los cajeros desde la BD.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CARGAR CAJEROS.");
                    }
                }
            }
        }

        /// <summary>
        /// Reload admin teleports
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("cargartps")]
        public void LoadAdminTeleportsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = AdminTeleport.Load();
                    if (result)
                    {
                        Notification(player.name + " ha recargado los teleports admin desde la BD.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CARGAR TELEPORTS ADMIN.");
                    }
                }
            }
        }

        /// <summary>
        /// Reload item data command
        /// </summary>
        /// <param name="sender">Self client instance</param>
        [Command("cargaritems")]
        public void LoadItemsCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    bool result = ItemData.Load();
                    if (result)
                    {
                        Notification(player.name + " ha recargado los items desde la BD.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "HUBO UN ERROR AL CARGAR ITEMS.");
                    }
                }
            }
        }

        /// <summary>
        /// Edit item position (attached items)
        /// </summary>
        /// <param name="sender">Self client instance</param>
        /// <param name="part">Attach part</param>
        [Command("editaritem", "~y~Uso: /editaritem <parte 0-3>")]
        public void EditarItem(Client sender, int part)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    switch (part)
                    {
                        case 0:
                            {
                                if(player.character.inventory.rightHand != null && NAPI.Entity.DoesEntityExist(player.character.inventory.rightHandProp))
                                {
                                    ObjectEditor.StartEditor(player, (int)ObjectEditor.Mode.AttachedItem, (int)Global.InventoryBodypart.RightHand);
                                }
                                else
                                {
                                    player.DisplayNotification("~r~Item no editable.");
                                }
                                break;
                            }
                        case 1:
                            {
                                if (player.character.inventory.leftHand != null && NAPI.Entity.DoesEntityExist(player.character.inventory.leftHandProp))
                                {
                                    ObjectEditor.StartEditor(player, (int)ObjectEditor.Mode.AttachedItem, (int)Global.InventoryBodypart.LeftHand);
                                }
                                else
                                {
                                    player.DisplayNotification("~r~Item no editable.");
                                }
                                break;
                            }
                        case 2:
                            {
                                if (player.character.inventory.heavyWeapon != null && NAPI.Entity.DoesEntityExist(player.character.inventory.heavyWeaponProp))
                                {
                                    ObjectEditor.StartEditor(player, (int)ObjectEditor.Mode.AttachedItem, (int)Global.InventoryBodypart.HeavyWeapon);
                                }
                                else
                                {
                                    player.DisplayNotification("~r~Item no editable.");
                                }
                                break;
                            }
                        case 3:
                            {
                                if (player.character.inventory.meleeHeavyWeapon != null && NAPI.Entity.DoesEntityExist(player.character.inventory.meleeHeavyWeaponProp))
                                {
                                    ObjectEditor.StartEditor(player, (int)ObjectEditor.Mode.AttachedItem, (int)Global.InventoryBodypart.MeleeWeapon);
                                }
                                else
                                {
                                    player.DisplayNotification("~r~Item no editable.");
                                }
                                break;
                            }
                        default:
                            {
                                player.DisplayNotification("~r~Punto editable inexistente.");
                                break;
                            }
                    }
                }
            }
        }

        //-------------------------------TESTING COMMANDS (TEMPORARY)--------------------------------------


        //GET CURRENT POSITION COMMAND (Testing purposes)
        [Command("pos")]
        public void Pos(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "~g~" + player.user.Position.X + "," + player.user.Position.Y + "," + player.user.Position.Z + "," + player.user.Rotation.Z);
                    NAPI.Util.ConsoleOutput("[DEBUG] Current position: " + player.user.Position.X + "," + player.user.Position.Y + "," + player.user.Position.Z + "," + player.user.Rotation.Z);
                }
                else
                {
                    NAPI.Chat.SendChatMessageToPlayer(sender, "~r~No eres admin.");
                }
            }
        }

        //CHANGE PED MODEL (TEMPORARY)
        [Command("skin", "~y~USO: /skin (nombre modelo)\n" +
                            "~y~Consulta: https://wiki.gtanet.work/index.php?title=Peds")]
        public void Skin(Client sender, string pedName)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    NAPI.Player.SetPlayerSkin(player.user, NAPI.Util.PedNameToModel(pedName));
                    NAPI.Notification.SendNotificationToPlayer(player.user, "~g~Skin cambiado.");
                }
            }
        }

        //GODMODE
        [Command("god")]
        public void God(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if (NAPI.Entity.GetEntityInvincible(player.user))
                    {
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~r~Ya no eres dios...");
                        NAPI.Entity.SetEntityInvincible(player.user, false);
                    }
                    else
                    {
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~g~¡Ahora eres DIOS!");
                        NAPI.Entity.SetEntityInvincible(player.user, true);
                    }
                }
            }
        }

        //FULL HEAL
        [Command("curarse")]
        public void Curarse(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    NAPI.Notification.SendNotificationToPlayer(player.user, "~g~Curado.");
                    NAPI.Player.SetPlayerHealth(player.user, 100);
                }
            }
        }

        //FIX TESTING VEHICLE
        [Command("reparar")]
        public void RepararCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if (NAPI.Player.IsPlayerInAnyVehicle(player.user))
                    {
                        NetHandle vehicle = NAPI.Player.GetPlayerVehicle(player.user);
                        NAPI.Vehicle.RepairVehicle(vehicle);
                        NAPI.Vehicle.SetVehicleHealth(vehicle, 1000f);
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~g~Vehiculo reparado.");
                    }
                    else
                    {
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~r~No estás en un vehículo.");
                    }
                }
            }
        }

        //SPAWNS A TESTING VEHICLE
        [Command("vehiculo", "~y~USO: /veh (modelo)\n" +
                                "~y~Consulta: https://wiki.gtanet.work/index.php?title=Vehicle_Models", Alias = "veh")]
        public void VehiculoCmd(Client sender, VehicleHash model)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    var rot = NAPI.Entity.GetEntityRotation(player.user);
                    var veh = NAPI.Vehicle.CreateVehicle(model, player.user.Position, new Vector3(0, 0, rot.Z), 0, 0);

                    NAPI.Player.SetPlayerIntoVehicle(player.user, veh, -1);
                }
            }
        }

        //CHANGE A VEHICLE'S COLOR
        [Command("vehcolor", "~y~USO: /vehcolor (primario1) (primario2) (primario3) (secundario1) (secundario2) (secundario3)\n" +
                                "Cada color viene dado por los tres componentes rojo, verde y azul de 0 a 255")]
        public void CustomVehicleColorsCommand(Client sender, int primaryRed, int primaryGreen, int primaryBlue, int secondaryRed, int secondaryGreen, int secondaryBlue)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if (!player.user.Vehicle.IsNull)
                    {
                        NAPI.Vehicle.SetVehicleCustomPrimaryColor(player.user.Vehicle, primaryRed, primaryGreen, primaryBlue);
                        NAPI.Vehicle.SetVehicleCustomSecondaryColor(player.user.Vehicle, secondaryRed, secondaryGreen, secondaryBlue);
                        NAPI.Notification.SendNotificationToPlayer(player.user, "Colores cambiados.");
                    }
                    else
                    {
                        NAPI.Notification.SendNotificationToPlayer(player.user, "~r~No estás en un vehículo.");
                    }
                }
            }
        }

        //GIVE YOURSELF A TESTING WEAPON
        [Command("arma", "~y~USO: /arma (nombrearma) | EJ: pistol, assaultrifle, microsmg\n" +
                            "~y~Consulta: https://wiki.gtanet.work/index.php?title=Weapons_Models")]
        public void ArmaCmd(Client sender, WeaponHash weapon)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    NAPI.Player.GivePlayerWeapon(player.user, weapon, 9999);
                }
            }
        }

        //TEST TIME AND WEATHER DATA
        [Command("world")]
        public void WorldCmd(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "Hora sistema: " + World.hour + ":" + World.minute + ":" + World.second + " | Hora juego: " + NAPI.World.GetTime().ToString());
                    NAPI.Chat.SendChatMessageToPlayer(player.user, "Temp: " + World.temperature + " | Hum: " + World.humidity + " | Pres: " + World.pressure + " | Clima: " + World.currentWeather);
                }
            }
        }

        //RESPAWN
        [Command("respawn", "~y~USO: /respawn (hospital) | true = respawn en hospital, false = respawn en el lugar")]
        public void RespawnCmd(Client sender, bool hospital)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if(player.character.dying == 1)
                    {
                        player.Revive(hospital);
                    }
                }
            }
        }

        //TESTDIALOG
        [Command("testdialog", "~y~USO: /testdialog (titulo) (texto) (input) (botonaceptar) (botoncancelar)")]
        public void TestDialog(Client sender, string titulo, string texto, bool input, string acceptButton = "Aceptar", string cancelButton = "Cancelar")
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    GuiController.CreateDialog(player, new Dialog(titulo, texto, input, acceptButton, cancelButton, 
                    (Player acceptedBy, string tempInput) =>
                    {
                        NAPI.Chat.SendChatMessageToPlayer(acceptedBy.user, "Test dialog accepted: " + tempInput);
                    },
                    (Player canceledBy) =>
                    {
                        NAPI.Chat.SendChatMessageToPlayer(canceledBy.user, "Test dialog canceled.");
                    }));
                }
            }
        }

        //TESTANIM
        [Command("testanim", "~y~USO: /testanim (directorio) (animacion) [loop] [pararUltimoFrame] [noPiernas] [controlable] [cancelable]", Alias = "ta")]
        public void TestAnim(Client sender, string dir, string anim, int loop = 0, int stopLastFrame = 0, int onlyUpperBody = 0, int controllable = 0, int cancelable = 0)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    int flags = 0;

                    if (loop == 1)
                    {
                        flags = flags | (int)Animation.Flags.Loop;
                    }

                    if (stopLastFrame == 1)
                    {
                        flags = flags | (int)Animation.Flags.StopOnLastFrame;
                    }

                    if (onlyUpperBody == 1)
                    {
                        flags = flags | (int)Animation.Flags.OnlyAnimateUpperBody;
                    }

                    if (controllable == 1)
                    {
                        flags = flags | (int)Animation.Flags.AllowPlayerControl;
                    }

                    if (cancelable == 1)
                    {
                        flags = flags | (int)Animation.Flags.Cancellable;
                    }

                    NAPI.Player.PlayPlayerAnimation(player.user, flags, dir, anim);

                    NAPI.ClientEvent.TriggerClientEvent(player.user, "print_animation_duration", dir, anim);
                }
            }
        }

        //TESTCOMP
        [Command("testcomp", "~y~USO: /testcomp [indice] [variacion] [textura] | Dejalo vacio para resetear")]
        public void TestComp(Client sender, int index = -1, int variation = 0, int texture = 0)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if(index != -1)
                    {
                        NAPI.Player.SetPlayerClothes(player.user, index, variation, texture);
                    }
                    else
                    {
                        NAPI.Player.SetPlayerDefaultClothes(player.user);
                    }
                }
            }
        }

        //TESTPROP
        [Command("testprop", "~y~USO: /testprop [indice] [variacion] [textura] | Dejalo vacio para resetear")]
        public void TestProp(Client sender, int index = -1, int variation = 0, int texture = 0)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if (index != -1)
                    {
                        NAPI.Player.SetPlayerAccessory(player.user, index, variation, texture);
                    }
                    else
                    {
                        NAPI.Player.ClearPlayerAccessory(player.user, 0);
                        NAPI.Player.ClearPlayerAccessory(player.user, 1);
                        NAPI.Player.ClearPlayerAccessory(player.user, 2);
                        NAPI.Player.ClearPlayerAccessory(player.user, 6);
                        NAPI.Player.ClearPlayerAccessory(player.user, 7);
                    }
                }
            }
        }

        //TESTCLIP
        [Command("testclip", "~y~USO: /testclip [clipset] | Deja en blanco para resetearlo")]
        public void TestClip(Client sender, string clipset = "default")
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    player.SetMovementClipset(clipset);
                    player.DisplayNotification("~g~Clipset cambiado.");
                }
            }
        }

        //CREATE BANK ACCOUNT
        [Command("crearcuentabancaria", "~y~USO: /crearcuentabancaria (banco 0-3) (principal? 0-1) (tipo 0-2)")]
        public void CrearCuentaBancaria(Client sender, int bank, bool isMain, int type)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    BankAccount newAccount = BankAccount.Create(player.character.sqlid, 0, bank, type, isMain);
                    player.character.bankAccounts.Add(newAccount);

                    NAPI.Chat.SendChatMessageToPlayer(player.user, "Creada cuenta bancaria " + newAccount.accountNumber);
                }
            }
        }

        //LIST BANK ACCOUNT
        [Command("vercuentasbancarias")]
        public void VerCuentasBancarias(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    foreach(BankAccount account in player.character.bankAccounts)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "Cuenta " + account.accountNumber + ", tipo " + account.accountType + ", primaria? " + account.isPrimaryAccount);
                    }
                }
            }
        }



        //TEST ITEM
        /*
        [Command("testitem")]
        public void CrearTestItem(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    List<Item> contentList = new List<Item>();
                    contentList.Add(Item.createItem(1, -1));
                    contentList.Add(Item.createItem(1, -1));
                    Item testItem = Item.createItem(1, -1, )
                }
            }
        }*/

        //CREATE PLAYER INVENTORY
        [Command("crearinventario")]
        public void CrearInventario(Client sender)
        {
            Player player = Player.Exists(sender);
            if (player.character != null)
            {
                if (player.admin > 3)
                {
                    if(player.character.inventory == null)
                    {
                        Inventory newInventory = Inventory.Create(player.character.sqlid, (int)Global.InventoryType.Player, player);
                        player.character.inventory = newInventory;

                        NAPI.Chat.SendChatMessageToPlayer(player.user, "Creado inventario del personaje.");
                    }
                    else
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "Ya tienes un inventario asignado.");
                    }
                }
            }
        }


        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////HELPER FUNCTIONS//////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Send a notification to all connected and active duty admins
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="rank">The lower admin rank that should see the notification</param>
        public static void Notification(string message, int rank = 1)
        {
            foreach(Player player in Player.Players)
            {
                if(player.admin >= rank)
                {
                    if(player.adminDuty)
                    {
                        NAPI.Chat.SendChatMessageToPlayer(player.user, "~r~[A] ~y~" + message);
                        Log.AdminLog(message);
                    }
                }
            }
        }

        /// <summary>
        /// Shows the admin help menu
        /// </summary>
        /// <param name="player">The admin player</param>
        public static void ShowAdminHelp(Player player)
        {
            player.showingGui = (int)GuiController.GuiTypes.AdminHelpPage;
            player.showingGuiId = 0;
            NAPI.ClientEvent.TriggerClientEvent(player.user, "showAdminHelp");
        }

        /// <summary>
        /// Hide admin help menu
        /// </summary>
        /// <param name="player">The admin player</param>
        public static void HideAdminHelp(Player player)
        {
            player.showingGui = -1;
            player.showingGuiId = -1;
            NAPI.ClientEvent.TriggerClientEvent(player.user, "hideAdminHelp");
        }

    }

}
