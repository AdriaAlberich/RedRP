/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  keyManager.js - Keybinding controller and other methods related to game controls
 */

var disableWeaponWheel = false;

//Listening server events
API.onServerEventTrigger.connect(function (name, args) {

    //Disable or enable weapon wheel and other weapon interactions
    if (name === 'disable_weapon_wheel') {
        disableWeaponWheel = args[0];
    }

});

//Key up eventhandler (doesn't repeat)
API.onKeyUp.connect(function (sender, args) {
    switch (args.KeyCode) {
        //Change character keybind
        case Keys.P: {
            API.triggerServerEvent("key_change_character");
            break;
        }
        //Change voice type keybind
        case Keys.O: {
            API.triggerServerEvent("key_change_voice_type");
            break;
        }
        //Close all GUI elements
        case Keys.Apps: {
            API.triggerServerEvent("key_close_all_gui");
            break;
        }
        //Help menu
        case Keys.F1: {
            API.triggerServerEvent("key_show_help_menu");
            break;
        }
        //Player list
        case Keys.F2: {
            API.triggerServerEvent("key_show_player_list");
            break;
        }
        //Inventory keybind
        case Keys.I: {
            API.triggerServerEvent("key_toggle_inventory");
            break;
        }
        //Self interaction keybind
        case Keys.X: {
            API.triggerServerEvent("key_show_self_interaction_menu");
            break;
        }
        //External interaction keybind
        case Keys.E: {
            API.triggerServerEvent("key_show_external_interaction_menu");
            break;
        }
        //Crouch mode
        case Keys.Menu: {
            API.triggerServerEvent("key_crouch_mode");
            break;
        }
        //Cancel animations
        case Keys.Space: {
            API.triggerServerEvent("key_cancel_all_animations");
            break;
        }
    }
});

//Disabled controls
API.onUpdate.connect(function() {
    //Disable Weapon wheel and other controls
    
    API.disableControlThisFrame(56); //Drop weapon
    API.disableControlThisFrame(57); //Drop ammo



    if (disableWeaponWheel) {

        API.disableControlThisFrame(12); //Weapon Wheel
        API.disableControlThisFrame(13); //
        API.disableControlThisFrame(14); //
        API.disableControlThisFrame(15); //
        API.disableControlThisFrame(16); //
        API.disableControlThisFrame(17); //
        API.disableControlThisFrame(37); //
        API.disableControlThisFrame(53); //
        API.disableControlThisFrame(54); //
        API.disableControlThisFrame(99); //Vehicle weapon selection
        API.disableControlThisFrame(100); //
        API.disableControlThisFrame(115); //
        API.disableControlThisFrame(116); //
        API.disableControlThisFrame(157); //Fast weapon selection
        API.disableControlThisFrame(158); //
        API.disableControlThisFrame(159); //
        API.disableControlThisFrame(160); //
        API.disableControlThisFrame(161); //
        API.disableControlThisFrame(162); //
        API.disableControlThisFrame(163); //
        API.disableControlThisFrame(164); //
        API.disableControlThisFrame(165); //
        API.disableControlThisFrame(257); //
        API.disableControlThisFrame(261); //Other weapon selection controls
        API.disableControlThisFrame(262); //
        API.disableControlThisFrame(25); //
        API.disableControlThisFrame(24); //
        API.disableControlThisFrame(23); //
        API.disableControlThisFrame(21); //
        API.disableControlThisFrame(22); //
        API.disableControlThisFrame(45); //
        API.disableControlThisFrame(55); //
    }
});

