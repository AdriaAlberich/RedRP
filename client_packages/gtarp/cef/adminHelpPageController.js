var adminHelpBrowser = null;

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_admin_help_page') {
		var res = API.getScreenResolution();
        adminHelpBrowser = API.createCefBrowser(res.Width, res.Height, true);
        API.waitUntilCefBrowserInit(adminHelpBrowser);
        API.setCefBrowserPosition(adminHelpBrowser, 0, 0);

        API.loadPageCefBrowser(adminHelpBrowser, 'client/cef/adminHelpPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);
	}

	//Hide the dialog page
	if(name == 'hide_admin_help_page') {
        API.destroyCefBrowser(adminHelpBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
        adminHelpBrowser = null;
	}

});

