var accountInfoBrowser = null;

var accountName = "";
var accountData = "";

var characterName = "";
var characterGeneralData = "";
var characterPropertyData = "";
var characterExperienceData = "";

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_account_info_page') {

		var res = API.getScreenResolution();
		accountInfoBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(accountInfoBrowser);
		API.setCefBrowserPosition(accountInfoBrowser, 0, 0);

		API.loadPageCefBrowser(accountInfoBrowser, 'client/cef/accountInfoPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);

		accountName = args[0];
		accountData = args[1];
		characterName = args[2];
		characterGeneralData = args[3];
		characterPropertyData = args[4];
		characterExperienceData = args[5];
	}

	//Hide the dialog page
	if(name == 'hide_account_info_page') {
		API.destroyCefBrowser(accountInfoBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		accountInfoBrowser = null;
	}

});

function requestData() {
	accountInfoBrowser.call('setAccountName', accountName);
	accountInfoBrowser.call('setAccountData', accountData);
	accountInfoBrowser.call('setCharacterName', characterName);
	accountInfoBrowser.call('setCharacterGeneralData', characterGeneralData);
	accountInfoBrowser.call('setCharacterProperties', characterPropertyData);
	accountInfoBrowser.call('setCharacterExperience', characterExperienceData);
	accountInfoBrowser.call('setVisible');
}


