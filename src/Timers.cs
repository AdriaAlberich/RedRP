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
using System.Threading;
using System.Net;

#pragma warning disable CS0168

namespace redrp
{
    /// <summary>
    /// Global timers
    /// </summary>
    public class Timers : Script
    {
        private Timer halfSecondTimer;
        private Timer oneSecondTimer;
        private Timer oneMinuteTimer;
        private Timer tenMinuteTimer;

        /// <summary>
        /// Timer initialization
        /// </summary>
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            halfSecondTimer = new Timer(HalfSecond, null, 500, 500);
            oneSecondTimer = new Timer(OneSecond, null, 1000, 1000);
            oneMinuteTimer = new Timer(OneMinute, null, 60000, 60000);
            tenMinuteTimer = new Timer(TenMinute, null, 600000, 600000);
        }

        /// <summary>
        /// Half second timer (500 ms)
        /// </summary>
        /// <param name="none"></param>
        private void HalfSecond(object none)
        {
            try
            {
                if (Global.ServerStatus != -1)
                {
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Sync the ingame time with real life time
                    World.SyncTime();
                    NAPI.World.SetTime(World.hour, World.minute, World.second);

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Only players loop
                    for (int i = 0; i < Player.Players.Count(); i++)
                    {
                        Player player = Player.Players[i];
                        player.HalfSecond();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// One second timer (1000 ms)
        /// </summary>
        /// <param name="none"></param>
        private void OneSecond(object none)
        {
            try
            {
                // Wait until gamemode is fully initialized
                if (Global.ServerStatus != -1)
                {
                    // Database connection check
                    if (!Util.CheckDBConnection())
                    {
                        if (Global.ServerStatus == 1)
                        {
                            Log.Debug("[BD] SE PERDIÓ LA CONEXIÓN CON LA BASE DE DATOS. CONGELANDO SERVIDOR.");
                            NAPI.Chat.SendChatMessageToAll("~r~[ERROR] SE HA PERDIDO LA CONEXIÓN CON LA BASE DE DATOS. DESCONECTANDO...");
                            foreach (Client player in NAPI.Pools.GetAllPlayers())
                            {
                                NAPI.Data.SetEntityData(player, "autokick", 5);
                            }

                            Global.ServerStatus = 0;
                        }
                    }
                    else
                    {
                        if (Global.ServerStatus == 0)
                        {
                            Log.Debug("[BD] SE RECUPERÓ LA CONEXIÓN CON LA BASE DE DATOS.");
                            Global.ServerStatus = 1;
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Admin reports checking
                    foreach (AdminReport report in Global.AdminReports)
                    {
                        report.timeElapsed++;
                        if (report.timeElapsed == Global.MaxAdminReportLifetime)
                        {
                            NAPI.Chat.SendChatMessageToPlayer(report.reporter.user, "~r~Tu reporte ha caducado, porfavor manda uno nuevo y disculpa las molestias.");
                            Global.AdminReports.Remove(report);
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // All clients loop
                    foreach (Client player in NAPI.Pools.GetAllPlayers())
                    {
                        // Autokick system (for login)
                        if (NAPI.Data.HasEntityData(player, "autokick"))
                        {
                            if (NAPI.Data.GetEntityData(player, "autokick") > 0)
                            {
                                NAPI.Data.SetEntityData(player, "autokick", NAPI.Data.GetEntityData(player, "autokick") - 1);
                                if (NAPI.Data.GetEntityData(player, "autokick") == 0)
                                {
                                    NAPI.Player.KickPlayer(player);
                                }
                            }
                        }
                    }

                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Only players loop
                    for (int i = 0; i < Player.Players.Count(); i++)
                    {
                        Player player = Player.Players[i];
                        player.OneSecond();
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Garbage containers
                    Container.GenerateGarbage();
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// One minute timer (60000 ms)
        /// </summary>
        /// <param name="none"></param>
        private void OneMinute(object none)
        {
            try
            {
                if (Global.ServerStatus != -1)
                {
                    // Only players loop
                    for (int i = 0; i < Player.Players.Count(); i++)
                    {
                        Player player = Player.Players[i];
                        player.OneMinute();
                    }
                }

                // Global inventory saving
                foreach(Inventory inventory in Global.Inventories)
                {
                    if(inventory.save)
                    {
                        inventory.Save();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }

        /// <summary>
        /// Ten minute timer (600000 ms)
        /// </summary>
        /// <param name="none"></param>
        private void TenMinute(object none)
        {
            try
            {
                if (Global.ServerStatus != -1)
                {
                    // Weather system
                    World.SyncWeather();
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // World items save
                    WorldItem.SaveWorldItems();
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
            }
        }
    }
}
