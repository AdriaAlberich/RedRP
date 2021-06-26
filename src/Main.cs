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
using System.Net;

namespace redrp
{
    /// <summary>
    /// Gamemode initialization
    /// </summary>
    public class Main : Script
    {

        /// <summary>
        /// Resource start event, handles gamemode initialization
        /// </summary>
        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            NAPI.Util.ConsoleOutput("[GTARP] Inicializando GM...");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // GM name and version
            NAPI.Server.SetGamemodeName(Global.ScriptName + " " + Global.ScriptVersion);

            // Logging system init
            NAPI.Util.ConsoleOutput("[GTARP] Inicializando logs...");
            Log.Initialize();

            // Database connection
            if (Util.CheckDBConnection())
            {
                NAPI.Util.ConsoleOutput("[GTARP] Conexión con la BD exitosa.");

                // Language data initialization
                if (Language.Load())
                {
                    NAPI.Util.ConsoleOutput("[GTARP] Leídos idiomas desde BD.");

                    // Animation data initialization
                    if (Animation.Load())
                    {
                        NAPI.Util.ConsoleOutput("[GTARP] Leídas animaciones desde BD.");

                        // Animation categories initialization
                        if (AnimationCategory.Load())
                        {
                            NAPI.Util.ConsoleOutput("[GTARP] Leídas categorias de animaciones desde BD.");

                            // ATM data initialization
                            if(ATM.Load())
                            {
                                NAPI.Util.ConsoleOutput("[GTARP] Leídos cajeros automáticos desde BD.");

                                // Admin Teleports initialization
                                if (AdminTeleport.Load())
                                {
                                    NAPI.Util.ConsoleOutput("[GTARP] Leídas localizaciones admin desde BD.");

                                    // Items initialization
                                    if (ItemData.Load())
                                    {
                                        NAPI.Util.ConsoleOutput("[GTARP] Leídos items desde BD.");

                                        // Garbage containers initialization
                                        if (Container.Load())
                                        {
                                            if (WorldItem.InitializeWorldItems())
                                            {
                                                NAPI.Util.ConsoleOutput("[GTARP] Leídos contenedores de basura desde BD.");

                                                // Weather system initialization
                                                World.SyncWeather();
                                                NAPI.Util.ConsoleOutput("[GTARP] Sistema de clima inicializado.");

                                                // Init time
                                                World.InitTime();
                                                NAPI.Util.ConsoleOutput("[GTARP] Ajustada hora del día: " + World.hour.ToString("D2") + ":" + World.minute.ToString("D2"));

                                                // Admin reports
                                                Global.AdminReports = new List<AdminReport>();

                                                // Bank accounts
                                                Global.BankAccounts = new List<BankAccount>();

                                                // Inventories
                                                Global.Inventories = new List<Inventory>();

                                                // Set command error message
                                                NAPI.Server.SetCommandErrorMessage(Global.CmdErrorMessage);

                                                // Disable respawn and automatic spawn
                                                NAPI.Server.SetAutoRespawnAfterDeath(false);
                                                NAPI.Server.SetAutoSpawnOnConnect(false);

                                                // Disable global server chat
                                                NAPI.Server.SetGlobalServerChat(false);

                                                // Set default spawn point
                                                NAPI.Server.SetDefaultSpawnLocation(new Vector3(-1037.7, -2737.8, 20.1), 324);

                                                NAPI.Util.ConsoleOutput("[GTARP] GM inicializado.");
                                                Global.ServerStatus = 1;
                                            }
                                            else
                                            {
                                                NAPI.Util.ConsoleOutput("[GTARP] Error al cargar los items globales.");
                                            }
                                        }
                                        else
                                        {
                                            NAPI.Util.ConsoleOutput("[GTARP] Error al leer contenedores de basura desde BD.");
                                        }
                                    }
                                    else
                                    {
                                        NAPI.Util.ConsoleOutput("[GTARP] Error al leer items desde BD.");
                                    }
                                }
                                else
                                {
                                    NAPI.Util.ConsoleOutput("[GTARP] Error al leer localizaciones admin desde BD.");
                                }
                            }
                            else
                            {
                                NAPI.Util.ConsoleOutput("[GTARP] Error al leer cajeros automáticos desde BD.");
                            }
                        }
                        else
                        {
                            NAPI.Util.ConsoleOutput("[GTARP] Error al leer categorias de animaciones desde BD.");
                        }
                    }
                    else
                    {
                        NAPI.Util.ConsoleOutput("[GTARP] Error al leer animaciones desde BD.");
                    }
                }
                else
                {
                    NAPI.Util.ConsoleOutput("[GTARP] Error al leer idiomas desde BD.");
                }
            }
            else
            {
                NAPI.Util.ConsoleOutput("[GTARP] No se puede conectar con la BD.");
            }
        }

        /// <summary>
        /// Closes gamemode
        /// </summary>
        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            NAPI.Util.ConsoleOutput("[GTARP] Parando GM.");
        }
    }
}
