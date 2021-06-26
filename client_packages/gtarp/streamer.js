/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  streamer.js - Streamer controller and sync stuff
 */

/**
*  Event handlers
**/
/////////////////////////////////////////////////////////////////////////

//Resource start
API.onResourceStart.connect(() => {

    //API.every(5000, "sync");

});

//Main syncronization handler
/*
function sync() {
    var allPlayers = API.getAllPlayers();
    API.sendChatMessage("SYNC " + allPlayers.Length);
    for (var i = 0; i < allPlayers.Length; i++) {
        //If weapon animation is set then we apply it
        if (API.hasEntitySyncedData(allPlayers[i], 'weapon_animation')) {
            API.setPlayerWeaponAnimationOverride(allPlayers[i], API.getEntitySyncedData(allPlayers[i], 'weapon_animation'));
        }
    }
}*/

//Stream IN
API.onEntityStreamIn.connect((ent, entType) => {
    
    //Entity is a player or ped
    if (entType === 6 || entType === 8) {
        
        //If weapon animation is set then we apply it
        if (API.hasEntitySyncedData(ent, 'weapon_animation')) {
            API.setPlayerWeaponAnimationOverride(ent, API.getEntitySyncedData(ent, 'weapon_animation'));
        }

        //If face animation is set then we apply it
        if (API.hasEntitySyncedData(ent, 'face_animation')) {
            API.sendChatMessage("SINCRONIZANDO ANIMACION FACIAL");
            //API.callNative('SET_FACIAL_IDLE_ANIM_OVERRIDE', ent, API.getEntitySyncedData(ent, 'face_animation'), API.getEntitySyncedData(ent, 'face_animation_dict'));
        }
        
        //If movement clipset is set then we apply it
        if (API.hasEntitySyncedData(ent, 'movement_clipset')) {
            API.sendChatMessage("SINCRONIZANDO CLIPSET");
            var clipset = API.getEntitySyncedData(ent, 'movement_clipset');
            if (clipset !== 'default') {
                API.setPlayerMovementClipset(ent, clipset, 1.0);
            }
        }

    }
    
});

//Stream OUT
API.onEntityStreamOut.connect((ent, entType) => {


});

//Entity data change
API.onEntityDataChange.connect((ent, key, oldValue) => {
    
    //If weapon animation is set then we apply it
    if (key === 'weapon_animation') {
        API.setPlayerWeaponAnimationOverride(ent, API.getEntitySyncedData(ent, 'weapon_animation'));
    }

    //If face animation is set then we apply it
    if (key === 'face_animation') {
        API.sendChatMessage("SINCRONIZANDO ANIMACION FACIAL");
        //API.callNative('SET_FACIAL_IDLE_ANIM_OVERRIDE', ent, API.getEntitySyncedData(ent, 'face_animation'), API.getEntitySyncedData(ent, 'face_animation_dict'));
    }

    //If movement clipset is set then we apply it
    if (key === 'movement_clipset') {
        API.sendChatMessage("SINCRONIZANDO CLIPSET");
        var clipset = API.getEntitySyncedData(ent, 'movement_clipset');
        if(clipset !== 'default') {
            API.setPlayerMovementClipset(ent, clipset, 1.0);
        }else{
            API.resetPlayerMovementClipset(ent);
        }
    }

});

/////////////////////////////////////////////////////////////////////////