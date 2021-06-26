var loginBrowser = null;

var data = null;

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the login page
	if(name == 'show_login_page') {
		var res = API.getScreenResolution();
		loginBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(loginBrowser);
		API.setCefBrowserPosition(loginBrowser, 0, 0);

		API.loadPageCefBrowser(loginBrowser, 'client/cef/loginPage.html');
		API.setCanOpenChat(false);
		API.setHudVisible(false);
		API.showCursor(true);

		data = args[0];
	}

	//Show error popup (incorrect password for example)
	if(name == 'show_error_login_page') {
		loginBrowser.call('showError', args[0]);	
	}

	//Switches from login page to character selector page
	if(name == 'switch_character_selector_login_page') {
		loginBrowser.call('switchToCharacterSelector', args[0]);	
	}

	//Hide the login page
	if(name == 'hide_login_page') {
		API.destroyCefBrowser(loginBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		loginBrowser = null;
	}

});

//Sends the login attempt to the server.
function sendLogin(nickOrEmail, password) {
	API.triggerServerEvent('login_attempt', nickOrEmail, password, API.getUniqueHardwareId());
}

function sendSpawnRequest(type, sqlid) {
	API.triggerServerEvent('character_spawn_attempt', type, sqlid);
}

//Sends initialization data when login page is loaded
function requestInitializationData() {
	if(data === 'login') {
		loginBrowser.call("initialization", 0, API.getPlayerName(API.getLocalPlayer()));
	}else{
		loginBrowser.call("initialization", 1, data);
	}
}

