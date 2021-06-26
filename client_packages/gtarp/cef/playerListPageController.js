var playerListBrowser = null;

var playerData = "";

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_player_list_page') {

		var res = API.getScreenResolution();
		playerListBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(playerListBrowser);
		API.setCefBrowserPosition(playerListBrowser, 0, 0);

		API.loadPageCefBrowser(playerListBrowser, 'client/cef/playerListPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);

		playerData = args[0];
	}

	//Hide the dialog page
	if(name == 'hide_player_list_page') {
		API.destroyCefBrowser(playerListBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		playerListBrowser = null;
	}

});

function requestData() {
	playerListBrowser.call('setPlayerData', playerData);
	playerListBrowser.call('setVisible');
}


