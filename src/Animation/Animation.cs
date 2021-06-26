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
using System.Threading;
using GTANetworkAPI;
using MySql.Data.MySqlClient;

namespace redrp
{
    /// <summary>
    /// Animation system
    /// </summary>
    public class Animation : Script
    {
        // STATIC DATA

        /// <summary>
        /// Debug constant
        /// </summary>
        public const bool debug = true;

        // ANIMATION ATTRIBUTES

        /// <summary>
        /// Animation database id
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Animation name
        /// </summary>
        public string animationName { get; set; }

        /// <summary>
        /// Scenario name (GTA special animation)
        /// </summary>
        public string scenario { get; set; }

        /// <summary>
        /// First male animation
        /// </summary>
        public string entryMale { get; set; }

        /// <summary>
        /// Delay to male main animation
        /// </summary>
        public int delayToMainMale { get; set; }

        /// <summary>
        /// Main male animation
        /// </summary>
        public string mainMale { get; set; }

        /// <summary>
        /// Delay to male exit animation
        /// </summary>
        public int delayToEndMale { get; set; }

        /// <summary>
        /// Ending male animation
        /// </summary>
        public string endMale { get; set; }

        /// <summary>
        /// Complementary male animation
        /// </summary>
        public string complementaryMale { get; set; }

        /// <summary>
        /// Complementary male animation delay
        /// </summary>
        public int delayComplementaryMale { get; set; }

        /// <summary>
        /// Male animation dictionary
        /// </summary>
        public string maleDictionary { get; set; }

        /// <summary>
        /// First female animation
        /// </summary>
        public string entryFemale { get; set; }

        /// <summary>
        /// Delay to female main animation
        /// </summary>
        public int delayToMainFemale { get; set; }

        /// <summary>
        /// Main female animation
        /// </summary>
        public string mainFemale { get; set; }

        /// <summary>
        /// Delay to female exit animation
        /// </summary>
        public int delayToEndFemale { get; set; }

        /// <summary>
        /// Ending female animation
        /// </summary>
        public string endFemale { get; set; }

        /// <summary>
        /// Complementary female animation
        /// </summary>
        public string complementaryFemale { get; set; }

        /// <summary>
        /// Complementary female animation delay
        /// </summary>
        public int delayComplementaryFemale { get; set; }

        /// <summary>
        /// Female animation dictionary
        /// </summary>
        public string femaleDictionary { get; set; }

        /// <summary>
        /// Chat command
        /// </summary>
        public string command { get; set; }

        /// <summary>
        /// Category id
        /// </summary>
        public int categoryId { get; set; }

        /// <summary>
        /// If is a looped animation
        /// </summary>
        public bool isLooped { get; set; }

        /// <summary>
        /// If animation must freeze on last frame
        /// </summary>
        public bool isStopped { get; set; }

        /// <summary>
        /// If animation only affects upper body
        /// </summary>
        public bool isUpperBody { get; set; }

        /// <summary>
        /// If animation does not freeze player
        /// </summary>
        public bool isControllable { get; set; }

        /// <summary>
        /// If animation is cancellable via normal input
        /// </summary>
        public bool isCancellable { get; set; }

        /// <summary>
        /// Animation flags
        /// </summary>
        [Flags]
        public enum Flags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        /// <summary>
        /// Load animation data from database
        /// </summary>
        /// <returns>True if success, false if failure</returns>
        public static Boolean Load()
        {
            try
            {
                // New animation list
                Global.Animations = new List<Animation>();

                // Instantiate a new disposable connection
                using (MySqlConnection tempConnection = new MySqlConnection())
                {
                    // Try to connect to database
                    tempConnection.ConnectionString = Global.ConnectionString;
                    tempConnection.Open();
                    // If success, instantiate a new disposable command
                    using (MySqlCommand tempCmd = tempConnection.CreateCommand())
                    {
                        // Command query
                        tempCmd.CommandText = "SELECT * FROM Animations";
                        // Reader
                        using (MySqlDataReader tempReader = tempCmd.ExecuteReader())
                        {
                            while (tempReader.HasRows)
                            {
                                tempReader.Read();
                                Animation newAnimation = new Animation();
                                newAnimation.sqlid = tempReader.GetInt32("id");
                                newAnimation.animationName = tempReader.GetString("animationName");
                                newAnimation.scenario = tempReader.GetString("scenario");
                                newAnimation.entryMale = tempReader.GetString("entryMale");
                                newAnimation.delayToMainMale = tempReader.GetInt32("delayToMainMale");
                                newAnimation.mainMale = tempReader.GetString("mainMale");
                                newAnimation.delayToEndMale = tempReader.GetInt32("delayToEndMale");
                                newAnimation.endMale = tempReader.GetString("endMale");
                                newAnimation.complementaryMale = tempReader.GetString("complementaryMale");
                                newAnimation.delayComplementaryMale = tempReader.GetInt32("delayComplementaryMale");
                                newAnimation.entryFemale = tempReader.GetString("entryFemale");
                                newAnimation.delayToMainFemale = tempReader.GetInt32("delayToMainFemale");
                                newAnimation.mainFemale = tempReader.GetString("mainFemale");
                                newAnimation.delayToEndFemale = tempReader.GetInt32("delayToEndFemale");
                                newAnimation.endFemale = tempReader.GetString("endFemale");
                                newAnimation.complementaryFemale = tempReader.GetString("complementaryFemale");
                                newAnimation.delayComplementaryFemale = tempReader.GetInt32("delayComplementaryFemale");
                                newAnimation.command = tempReader.GetString("command");
                                newAnimation.isLooped = tempReader.GetBoolean("isLooped");
                                newAnimation.isStopped = tempReader.GetBoolean("isStopped");
                                newAnimation.isUpperBody = tempReader.GetBoolean("isUpperBody");
                                newAnimation.isControllable = tempReader.GetBoolean("isControllable");
                                newAnimation.isCancellable = tempReader.GetBoolean("isCancellable");

                                // Add new animation to the global list
                                Global.Animations.Add(newAnimation);
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Debug(e.Message);
                Log.Debug(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Calculates the animation flags
        /// </summary>
        /// <returns>The flags value</returns>
        public int GetFlags()
        {
            int flags = 0;

            if (isLooped)
            {
                flags = flags | (int)Flags.Loop;
            }

            if (isStopped)
            {
                flags = flags | (int)Flags.StopOnLastFrame;
            }

            if (isUpperBody)
            {
                flags = flags | (int)Flags.OnlyAnimateUpperBody;
            }

            if (isControllable)
            {
                flags = flags | (int)Flags.AllowPlayerControl;
            }

            if (isCancellable)
            {
                flags = flags | (int)Flags.Cancellable;
            }

            return flags;
        }

        /// <summary>
        /// Play an animation on a player ped
        /// </summary>
        /// <param name="player">The player instance</param>
        /// <param name="phase">The animation phase</param>
        /// <param name="applyFlags">If must apply flags</param>
        /// <param name="ignoreSex">Play the animation ignoring the character sex</param>
        public void Play(Player player,  int phase = 1, bool applyFlags = true, bool ignoreSex = false)
        {
            // Calculate the flags if necessary
            int flags = 0;
            if (applyFlags)
            {
                flags = GetFlags();
            }

            // If is male or ignore sex is true
            if (player.character.sex == 0 || ignoreSex)
            {
                switch (phase)
                {
                    // Enter phase
                    case 0:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, maleDictionary, entryMale);
                            break;
                        }
                    // Main phase
                    case 1:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, maleDictionary, mainMale);
                            break;
                        }
                    // End phase
                    case 2:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, maleDictionary, endMale);
                            break;
                        }
                    // Complementary animation
                    case 3:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, maleDictionary, complementaryMale);
                            break;
                        }
                    default:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, maleDictionary, mainMale);
                            break;
                        }
                }
            }
            // If its female
            else
            {
                switch (phase)
                {
                    // Enter phase
                    case 0:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, femaleDictionary, entryFemale);
                            break;
                        }
                    // Main phase
                    case 1:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, femaleDictionary, mainFemale);
                            break;
                        }
                    // End phase
                    case 2:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, femaleDictionary, endFemale);
                            break;
                        }
                    //Complementary animation
                    case 3:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, femaleDictionary, complementaryFemale);
                            break;
                        }
                    default:
                        {
                            NAPI.Player.PlayPlayerAnimation(player.user, flags, femaleDictionary, mainFemale);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Starts the animation sequence
        /// </summary>
        /// <param name="player">Player instance</param>
        public void start(Player player)
        {
            if(scenario != "")
            {
                if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Enter scenario");
                NAPI.Player.PlayPlayerScenario(player.user, scenario);
            }
            else
            {
                // If there is a female specific animation
                if (mainFemale != "")
                {
                    // If there is an entering animation
                    if (entryMale != "" && entryFemale != "")
                    {
                        if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Enter animation");
                        // Play entering animation
                        Play(player, 0, false, false);
                        // Triggers a delay to the main animation
                        DelayTo(player, 1);
                    }
                    // If not
                    else
                    {
                        // Play the main animation directly
                        Play(player);
                        // Triggers a delay to the exit animation
                        if (endMale != "" && endFemale != "")
                        {
                            DelayTo(player, 2);
                        }
                    }
                }
                // If there is no female specific animation then we play the male animation
                else
                {
                    // If there is an entering animation
                    if (entryMale != "")
                    {
                        if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Enter animation");
                        // Play entering animation
                        Play(player, 0, false, true);
                        //Triggers a delay to the main animation
                        DelayTo(player, 1);
                    }
                    else
                    {
                        // Play the main animation directly
                        Play(player, 1, true, true);
                        // Triggers a delay to the exit animation
                        if (endMale != "")
                        {
                            DelayTo(player, 2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Applies a delay and executes the next animation
        /// </summary>
        /// <param name="player">Player instance</param>
        /// <param name="phase">Animation phase</param>
        private void DelayTo(Player player, int phase)
        {
            if(player.animationTimer != null)
            {
                player.animationTimer.Dispose();
                player.animationTimer = null;
            }

            // Male
            if (player.character.sex == 0)
            {
                switch (phase)
                {
                    // To main
                    case 1:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay to main MALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToMain, player, delayToMainMale, Timeout.Infinite);
                            break;
                        }
                    // To end
                    case 2:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay to ending MALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToEnd, player, delayToEndMale, Timeout.Infinite);
                            break;
                        }
                    // From complementary to main
                    case 3:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay complementary MALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToMain, player, delayComplementaryMale, Timeout.Infinite);
                            break;
                        }
                }
            }
            // Female
            else
            {
                switch (phase)
                {
                    // To main
                    case 1:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay to main FEMALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToMain, player, delayToMainFemale, Timeout.Infinite);
                            break;
                        }
                    // To end
                    case 2:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay to ending FEMALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToEnd, player, delayToEndFemale, Timeout.Infinite);
                            break;
                        }
                    // From complementary to main
                    case 3:
                        {
                            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Delay complementary FEMALE");
                            player.currentAnimation = this;
                            player.animationTimer = new Timer(ContinueToMain, player, delayComplementaryFemale, Timeout.Infinite);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Executes the main animation
        /// </summary>
        /// <param name="target">The player instance</param>
        private void ContinueToMain(object target)
        {
            Player player = (Player)target;
            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Main animation");
            // If there is a female specific animation
            if (mainFemale != "")
            {
                // Play it directly
                player.currentAnimation.Play(player);
                // Triggers a delay to the exit animation
                if (endMale != "" && endFemale != "")
                {
                    player.currentAnimation.DelayTo(player, 2);
                }
            }
            // If not
            else
            {
                // Play it directly
                Play(player, 1, true, true);
                if (endMale != "")
                {
                    // Triggers a delay to the exit animation
                    player.currentAnimation.DelayTo(player, 2);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void ContinueToEnd(object target)
        {
            Player player = (Player)target;
            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Ending animation");
            player.currentAnimation.Play(player, 2, false, false);
            player.currentAnimation = null;
            player.animationTimer.Dispose();
            player.animationTimer = null;
        }

        /**
        * executeComplementary()
        * Plays the complementary animation and triggers a delay to return to the main one
        */
        public void ExecuteComplementary(Player player)
        {
            if (debug) NAPI.Chat.SendChatMessageToPlayer(player.user, "Complementary animation");
            player.currentAnimation.Play(player, 3, false);
            player.currentAnimation.DelayTo(player, 3);
        }
    }

}
