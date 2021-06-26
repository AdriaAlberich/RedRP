var adminListBrowser = null;

var adminData = "";

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_admin_list_page') {

		var res = API.getScreenResolution();
		adminListBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(adminListBrowser);
		API.setCefBrowserPosition(adminListBrowser, 0, 0);

		API.loadPageCefBrowser(adminListBrowser, 'client/cef/adminListPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);

		adminData = args[0];
	}

	//Hide the dialog page
	if(name == 'hide_admin_list_page') {
		API.destroyCefBrowser(adminListBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		adminListBrowser = null;
	}

});

function requestData() {
	adminListBrowser.call('setAdminData', adminData);
	adminListBrowser.call('setVisible');
}


