var generalHelpBrowser = null;

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_general_help_page') {
		var res = API.getScreenResolution();
		generalHelpBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(generalHelpBrowser);
		API.setCefBrowserPosition(generalHelpBrowser, 0, 0);

		API.loadPageCefBrowser(generalHelpBrowser, 'client/cef/generalHelpPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);
	}

	//Hide the dialog page
	if(name == 'hide_general_help_page') {
		API.destroyCefBrowser(generalHelpBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		generalHelpBrowser = null;
	}

});

