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
    /// Keybinds
    /// </summary>
    public class Keybinds : Events.Script
    {
        /// <summary>
        /// System static data
        /// </summary>
        private static readonly int KeyDetectionTime = 350000;
        private static Dictionary<int, long> pressedKeys;
        private static List<int> keybinds;

        /// <summary>
        /// Keybind initialization
        /// </summary>
        public Keybinds()
        {
            // Initialize the dictionary
            pressedKeys = new Dictionary<int, long>();

            // Initializes the supported keybinds
            InitializeKeybinds();
        }

        /// <summary>
        /// Checks if a key is pressed
        /// </summary>
        /// <param name="currentTicks">The current tick count</param>
        /// <returns></returns>
        public static int Check(long currentTicks)
        {
            // Check the first released key
            int releasedKey = -1;

            // Check if the keys are loaded
            if (keybinds == null) return releasedKey;

            foreach (int keybind in keybinds)
            {
                if (pressedKeys.TryGetValue(keybind, out long downTicks))
                {
                    // If there's already a key released we do nothing
                    if (releasedKey >= 0) continue;

                    // Check if the key is already up
                    if (!Input.IsDown(keybind) && (currentTicks - downTicks) > KeyDetectionTime)
                    {
                        releasedKey = keybind;
                        pressedKeys.Remove(releasedKey);
                    }
                }
                else if (Input.IsDown(keybind))
                {
                    // Store the key into the dictionary
                    pressedKeys.Add(keybind, currentTicks);
                }
            }

            return releasedKey;
        }

        /// <summary>
        /// Sends a key event to the server
        /// </summary>
        /// <param name="key">The key pressed</param>
        public static void SendKeyPressed(int key)
        {
            switch (key)
            {
                case (int)ConsoleKey.P:
                    // Change character
                    Events.CallRemote("changeCharacterKeyPressed");
                    break;
                case (int)ConsoleKey.O:
                    // Change voice type
                    Events.CallRemote("changeVoiceTypeKeyPressed");
                    break;
                case (int)ConsoleKey.Applications:
                    // Close all gui
                    Events.CallRemote("closeAllGuiKeyPressed");
                    break;
                case (int)ConsoleKey.F1:
                    // Help menu
                    Events.CallRemote("showHelpMenuKeyPressed");
                    break;
                case (int)ConsoleKey.F2:
                    // Show player list
                    Events.CallRemote("showPlayerListKeyPressed");
                    break;
                case (int)ConsoleKey.I:
                    // Open/close inventory
                    Events.CallRemote("toggleInventoryKeyPressed");
                    break;
                case (int)ConsoleKey.X:
                    // Self interaction menu
                    Events.CallRemote("showSelfInteractionMenuKeyPressed");
                    break;
                case (int)ConsoleKey.E:
                    // External interaction menu
                    Events.CallRemote("showExternalInteractionMenuKeyPressed");
                    break;
                case (int)ConsoleKey.RightWindows:
                    // Crouch mode
                    Events.CallRemote("crouchModeKeyPressed");
                    break;
                case (int)ConsoleKey.Spacebar:
                    // Cancel animations
                    Events.CallRemote("cancelAllAnimationsKeyPressed");
                    break;
                case (int)ConsoleKey.NumPad0:
                    // Menu shortcut key 0
                    Events.CallLocal("menuSelectionHotkeyPressed", 0);
                    break;
                case (int)ConsoleKey.NumPad1:
                    // Menu shortcut key 1
                    Events.CallLocal("menuSelectionHotkeyPressed", 1);
                    break;
                case (int)ConsoleKey.NumPad2:
                    // Menu shortcut key 2
                    Events.CallLocal("menuSelectionHotkeyPressed", 2);
                    break;
                case (int)ConsoleKey.NumPad3:
                    // Menu shortcut key 3
                    Events.CallLocal("menuSelectionHotkeyPressed", 3);
                    break;
                case (int)ConsoleKey.NumPad4:
                    // Menu shortcut key 4
                    Events.CallLocal("menuSelectionHotkeyPressed", 4);
                    break;
                case (int)ConsoleKey.NumPad5:
                    // Menu shortcut key 5
                    Events.CallLocal("menuSelectionHotkeyPressed", 5);
                    break;
                case (int)ConsoleKey.NumPad6:
                    // Menu shortcut key 6
                    Events.CallLocal("menuSelectionHotkeyPressed", 6);
                    break;
                case (int)ConsoleKey.NumPad7:
                    // Menu shortcut key 7
                    Events.CallLocal("menuSelectionHotkeyPressed", 7);
                    break;
                case (int)ConsoleKey.NumPad8:
                    // Menu shortcut key 8
                    Events.CallLocal("menuSelectionHotkeyPressed", 8);
                    break;
                case (int)ConsoleKey.NumPad9:
                    // Menu shortcut key 9
                    Events.CallLocal("menuSelectionHotkeyPressed", 9);
                    break;
                case (int)ConsoleKey.Enter:
                    // Enter key for object editor
                    if(ObjectEditor.visible)
                    {
                        Events.CallRemote("objectEditorEnterKeyPressed", ObjectEditor.offset, ObjectEditor.rotation, ObjectEditor.position);
                    }
                    break;
                case (int)ConsoleKey.Backspace:
                    // Backspace key for object editor
                    if (ObjectEditor.visible)
                    {
                        Events.CallRemote("objectEditorBackspaceKeyPressed");
                    }
                    break;
                case (int)ConsoleKey.UpArrow:
                    // Up key for object editor
                    if (ObjectEditor.visible)
                    {
                        ObjectEditor.SwitchEditingComponent();
                    }
                    break;
                case (int)ConsoleKey.DownArrow:
                    // Down key for object editor
                    if (ObjectEditor.visible)
                    {
                        ObjectEditor.ChangeEditingVelocity();
                    }
                    break;
                case (int)ConsoleKey.LeftArrow:
                    // Left key for object editor
                    if (ObjectEditor.visible)
                    {
                        ObjectEditor.DecreaseComponent();
                    }
                    break;
                case (int)ConsoleKey.RightArrow:
                    // Right key for object editor
                    if (ObjectEditor.visible)
                    {
                        ObjectEditor.IncreaseComponent();
                    }
                    break;
            }
        }

        /// <summary>
        /// Initializes the list of supported keybinds
        /// </summary>
        private void InitializeKeybinds()
        {
            // Initialize the keybinds
            keybinds = new List<int>()
            {
                (int)ConsoleKey.P,
                (int)ConsoleKey.O,
                (int)ConsoleKey.Applications,
                (int)ConsoleKey.F1,
                (int)ConsoleKey.F2,
                (int)ConsoleKey.I,
                (int)ConsoleKey.X,
                (int)ConsoleKey.E,
                (int)ConsoleKey.RightWindows,
                (int)ConsoleKey.Spacebar,
                (int)ConsoleKey.NumPad0,
                (int)ConsoleKey.NumPad1,
                (int)ConsoleKey.NumPad2,
                (int)ConsoleKey.NumPad3,
                (int)ConsoleKey.NumPad4,
                (int)ConsoleKey.NumPad5,
                (int)ConsoleKey.NumPad6,
                (int)ConsoleKey.NumPad7,
                (int)ConsoleKey.NumPad8,
                (int)ConsoleKey.NumPad9,
                (int)ConsoleKey.Enter,
                (int)ConsoleKey.Backspace,
                (int)ConsoleKey.UpArrow,
                (int)ConsoleKey.DownArrow,
                (int)ConsoleKey.LeftArrow,
                (int)ConsoleKey.RightArrow
            };

        }

    }
}
