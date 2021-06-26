/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  hudManager.js - HUD events and rendering
 */

var hudActive = false;
var money = '';
var language = '';
var voice = '';
var adminduty = '';

API.onServerEventTrigger.connect(function (name, args) {

	//Hide GTA hud
    if(name == 'hud_hide') {
    	API.setHudVisible(false);
        hudActive = false;
    }

    //Show GTA hud
    if(name == 'hud_show') {
    	API.setHudVisible(true);
        hudActive = true;
    }

    //Money indicator
    if(name == 'hud_update_money') {
        money = '$' + args[0].toString();
    }

    //Language indicator
    if(name == 'hud_set_language') {
        language = args[0].toString();
    }

    //Voice indicator
    if(name == 'hud_set_voice_type') {
        voice = args[0].toString();
    }

    //Admin duty indicator
    if (name == 'hud_set_admin_duty') {
        adminduty = args[0].toString();
    }

    //Display a subtitle
    if(name == 'hud_display_subtitle') {
        API.displaySubtitle(args[0], args[1]);
    }

    //Show loading prompt
    if(name == 'hud_show_loading_prompt') {
        API.showLoadingPrompt(args[0], args[1]);
    }

    //Hide loading prompt
    if(name == 'hud_hide_loading_prompt') {
        if(API.isLoadingPromptActive())
            API.hideLoadingPrompt();
    }

    //Show a shard
    if(name == 'hud_show_shard') {
        API.displaySubtitle(args[0], args[1]);
    }

    //Set waypoint
    if(name == 'hud_set_waypoint') {
        API.setWaypoint(args[0], args[1]);
    }

    //Remove waypoint
    if(name == 'hud_remove_waypoint') {
        API.removeWaypoint();
    }
});

//HUD Renderer
API.onUpdate.connect(function () {
    if (hudActive) {

        //Money indicator
        if(money !== '') {
            API.drawText(money, 320.0, 870.0, 0.7, 11, 97, 11, 255, 7, 0, false, true, 0);
        }

        //Language indicator
        if (language !== '') {
            API.drawText(language, 320.0, 920.0, 0.6, 31, 185, 232, 255, 1, 0, false, true, 0);
        }

        //Voice indicator
        if (voice !== '') {
            switch(voice) {
                case 'Susurrar':
                    API.drawText(voice, 320.0, 960.0, 0.6, 235, 242, 107, 255, 1, 0, false, true, 0);
                    break;
                case 'Hablar':
                    API.drawText(voice, 320.0, 960.0, 0.6, 245, 194, 100, 255, 1, 0, false, true, 0);
                    break;
                case 'Gritar':
                    API.drawText(voice, 320.0, 960.0, 0.6, 245, 131, 100, 255, 1, 0, false, true, 0);
                    break;
            }
        }

        //Situational Awareness indicator
        var localPlayerPos = API.getEntityPosition(API.getLocalPlayer());
        //var localPlayerRot = API.getEntityRotation(API.getLocalPlayer());
        //var localPlayerAngle = localPlayerRot.Z;
        var localPlayerGameplayCameraRot = API.getGameplayCamRot();
        var localPlayerAngle = localPlayerGameplayCameraRot.Z;
        var direction = 'N';

        if (localPlayerAngle < 10 && localPlayerAngle > -10) {
            direction = 'N';
        }
        else if (localPlayerAngle > 10 && localPlayerAngle < 80) {
            direction = 'NO';
        }
        else if (localPlayerAngle > 80 && localPlayerAngle < 100) {
            direction = 'O';
        }
        else if (localPlayerAngle > 100 && localPlayerAngle < 170) {
            direction = 'SO';
        }
        else if ((localPlayerAngle > 170 && localPlayerAngle < 180) || (localPlayerAngle < -170 && localPlayerAngle > -180)) {
            direction = 'S';
        }
        else if (localPlayerAngle > -170 && localPlayerAngle < -100) {
            direction = 'SE';
        }
        else if (localPlayerAngle > -100 && localPlayerAngle < -80) {
            direction = 'E';
        }
        else if (localPlayerAngle > -80 && localPlayerAngle < -10) {
            direction = 'NE';
        }

        var situationalAwareness = API.getZoneNameLabel(localPlayerPos) + ' | ' + API.getStreetName(localPlayerPos) + ' | ' + direction;

        API.drawText(situationalAwareness, 320.0, 1000.0, 0.6, 255, 255, 255, 255, 1, 0, false, true, 0);

        //Admin duty indicator
        if (adminduty !== '') {
            API.drawText(adminduty, 320.0, 1040.0, 0.6, 255, 0, 0, 255, 1, 0, false, true, 0);
        }
    }
});

