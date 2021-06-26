var activeCamera = null;

API.onServerEventTrigger.connect(function (name, args) {

	//Login camera
    if (name == "login_camera") {
        activeCamera = API.createCamera(new Vector3(-1094.3, -3411.6, 200.9), new Vector3(0,0,0));
        API.setActiveCamera(activeCamera);
    }
    
    //Gameplay camera
    if(name == "gameplay_camera") {
        API.setGameplayCameraActive();
    }

});

