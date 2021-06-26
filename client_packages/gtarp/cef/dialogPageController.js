var dialogBrowser = null;
var callbackId = null;

var title = "";
var text = "";
var hasInput = false;
var acceptButtonText = "";
var cancelButtonText = "";

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

	//Show the dialog
	if(name == 'show_dialog_page') {

		callbackId = args[0];
		var res = API.getScreenResolution();
		dialogBrowser = API.createCefBrowser(res.Width, res.Height, true);
		API.waitUntilCefBrowserInit(dialogBrowser);
		API.setCefBrowserPosition(dialogBrowser, 0, 0);

		API.loadPageCefBrowser(dialogBrowser, 'client/cef/dialogPage.html');
		API.setCanOpenChat(false);
		API.showCursor(true);
		title = args[1];
		text = args[2];
		hasInput = args[3];
		acceptButtonText = args[4];
		cancelButtonText = args[5];
	}

	//Hide the dialog page
	if(name == 'hide_dialog_page') {
		API.destroyCefBrowser(dialogBrowser);
		API.setCanOpenChat(true);
		API.showCursor(false);
		dialogBrowser = null;
	}

});

//Sends the dialog data
function requestDialogData() {
	dialogBrowser.call("dialogInit", title, text, hasInput, acceptButtonText, cancelButtonText);	
}

//Sends the dialog input to the server.
function sendDialogAccept(dialogInput) {
	API.triggerServerEvent('dialog_accept', callbackId, dialogInput);
}

//Sends the dialog cancel request
function sendDialogCancel() {
	API.triggerServerEvent('dialog_cancel');
}
