/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  animationManager.js - Animation and ragdoll manager
 */

var ragdollLooped = false;
var ragdollLoopedType;
var localPlayer;

//Resource start
API.onResourceStart.connect(() => {

    API.every(100, "loop");

});

API.onServerEventTrigger.connect((name, args) => {
    
    if(name === 'play_ragdoll') {
        API.setPedToRagdoll(args[0], args[1]);

        if (args[0] === -1) {
            ragdollLooped = true;
            ragdollLoopedType = args[1];
        }
    }

    if(name === 'cancel_ragdoll') {
        API.cancelPedRagdoll();
        ragdollLooped = false;
        ragdollLoopedType = -1;
    }

    if (name === 'set_weapon_animation_mode') {
        localPlayer = API.getLocalPlayer();
        API.setPlayerWeaponAnimationOverride(localPlayer, args[0]);
        API.setEntitySyncedData(localPlayer, 'weapon_animation', args[0]);
    }

    if (name === 'set_face_animation') {
        localPlayer = API.getLocalPlayer();
        var animData = args[0].split(',');
        //API.callNative('SET_FACIAL_IDLE_ANIM_OVERRIDE', localPlayer, animData[1], animData[0]);
        API.setEntitySyncedData(localPlayer, 'face_animation_dict', animData[0]);
        API.setEntitySyncedData(localPlayer, 'face_animation', animData[1]);
    }

    if (name === 'set_movement_clipset') {
        localPlayer = API.getLocalPlayer();
        var clipset = args[0];
        if (clipset === 'default') {
            API.resetPlayerMovementClipset(localPlayer);
        } else {
            API.setPlayerMovementClipset(localPlayer, clipset, 1.0);
        }

        API.setEntitySyncedData(localPlayer, 'movement_clipset', clipset);
    }

    if (name === 'print_animation_duration') {
        var duration = API.getAnimTotalTime(args[0], args[1]);
        API.sendChatMessage("Duracion: " + Math.ceil(duration * 1000) + "ms");
    }
    
});

function loop() {

    if (ragdollLooped) {
        API.setPedToRagdoll(-1, ragdollLoopedType);
    }

}