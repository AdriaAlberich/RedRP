/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using RAGE;
using RAGE.Elements;

namespace redrp
{

    /// <summary>
    /// Streamer
    /// </summary>
    public class Streamer : Events.Script
    {
        private Timer syncDataTimer;

        /// <summary>
        /// Streamer initialization
        /// </summary>
        public Streamer()
        {
            Events.OnEntityStreamIn += StreamIn;
            Events.OnEntityStreamOut += StreamOut;

            syncDataTimer = new Timer(SyncData, null, 1000, 1000);
        }

        /// <summary>
        /// Triggered when an entity enters the streamer
        /// </summary>
        /// <param name="entity">The entity object</param>
        private void StreamIn(Entity entity)
        {
            // Entity is a player
            if (entity.Type == RAGE.Elements.Type.Player)
            {
                SyncPlayer(entity);
            }
        }

        /// <summary>
        /// Triggered when an entity exits the streamer
        /// </summary>
        /// <param name="entity">The entity object</param>
        private void StreamOut(Entity entity)
        {

        }

        /// <summary>
        /// Sync entity data every tick
        /// </summary>
        /// <param name="none"></param>
        private void SyncData(object none)
        {
            SyncPlayer(Player.LocalPlayer);
            foreach (Entity player in Entities.Players.Streamed)
            {
                SyncPlayer(player);
            }
        }

        /// <summary>
        /// Player syncronization
        /// </summary>
        /// <param name="ped">The ped object</param>
        private void SyncPlayer(Entity ped)
        {
            // Check if sex is shared
            if (ped.GetSharedData("playerSex") != null)
            {
                // Get player sex
                int sex = int.Parse(ped.GetSharedData("playerSex").ToString());

                // Check if mood is shared
                if (ped.GetSharedData("playerMood") != null)
                {
                    // Get player mood
                    int mood = int.Parse(ped.GetSharedData("playerMood").ToString());
                    // Get the correct anim dict depending on player sex
                    string sexDict = sex == 0 ? "gen_male" : "gen_female";
                    // Update player mood
                    switch (mood)
                    {
                        case 0:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@normal", "mood_normal_1");
                                break;
                            }
                        case 1:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@aiming", "mood_aiming_1");
                                break;
                            }
                        case 2:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@angry", "mood_angry_1");
                                break;
                            }
                        case 3:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@drunk", "mood_drunk_1");
                                break;
                            }
                        case 4:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@happy", "mood_happy_1");
                                break;
                            }
                        case 5:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@injured", "mood_injured_1");
                                break;
                            }
                        case 6:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@stressed", "mood_stressed_1");
                                break;
                            }
                        case 7:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@sulk", "mood_sulk_1");
                                break;
                            }
                        default:
                            {
                                RAGE.Game.Ped.SetFacialIdleAnimOverride(ped.Id, "facials@" + sexDict + "@variations@normal", "mood_normal_1");
                                break;
                            }
                    }
                }

                // Check if movement clipset is shared
                if (ped.GetSharedData("playerMovementClipset") != null)
                {
                    // Get movement clipset data
                    string movementClipset = ped.GetSharedData("playerMovementClipset").ToString();
                    // If is not default
                    if (movementClipset != "default")
                    {
                        // Update movement clipset
                        RAGE.Game.Ped.SetPedMovementClipset(ped.Id, movementClipset, 0.1f);
                    }
                    else
                    {
                        // Reset movement clipset
                        RAGE.Game.Ped.ResetPedMovementClipset(ped.Id, 0.0f);
                    }
                }
            }
        }
    }
}
