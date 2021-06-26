/**
 *  Grand Theft Auto Roleplay GameMode - GTARPES
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Grand Theft Auto: Roleplay [gta-rp.es]
 *  
 *  main.js - Main controller for client
 */


//Resource start
API.onResourceStart.connect(() => {

    //Disable alternative main menu key (P)
    API.disableAlternativeMainMenuKey(true);

    //Disable "wasted" death message
    API.setShowWastedScreenOnDeath(false);

});

//Listening server events
API.onServerEventTrigger.connect(function (name, args) {
    

});