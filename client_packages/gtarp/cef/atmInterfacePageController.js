var atmBrowser = null;

var data = null;
var moreData = null;
var bank = null;
var nextPage = null;

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

    //Show the atm interface
    if (name == 'show_atm_interface') {
        debugMessage("Bank: " + args[0] + " Data: " + args[1]);
        bank = args[0];
        data = args[1];

        var res = API.getScreenResolution();
        atmBrowser = API.createCefBrowser(res.Width, res.Height, true);
        API.waitUntilCefBrowserInit(atmBrowser);
        API.setCefBrowserPosition(atmBrowser, 0, 0);

        API.loadPageCefBrowser(atmBrowser, 'client/cef/atmInterfacePage.html');
        API.setCanOpenChat(false);
        API.showCursor(true);

    }

    //Hide the atm interface
    if (name == 'hide_atm_interface') {
        API.destroyCefBrowser(atmBrowser);
        API.setCanOpenChat(true);
        API.showCursor(false);
        atmBrowser = null;
    }

    //Switches to an ATM page from another
    if (name == 'switch_to_page_atm_interface') {
        nextPage = args[0];
        data = args[1];
        moreData = args[2];
        atmBrowser.call('switchToPage', nextPage, data, moreData);
    }

    //Activates the buttons
    if (name == 'activate_buttons_atm_interface') {
        atmBrowser.call('activateButtons');
    }
});

//Sends initialization data when atm page is loaded
function requestInitializationData() {
    atmBrowser.call("atmInitialization", bank, data);
}

//Sends the card number to the server.
function sendSelectedCard(cardNumber) {
    API.triggerServerEvent('atm_send_card', cardNumber);
}

//Sends the PIN number to the server.
function sendPIN(pin) {
    API.triggerServerEvent('atm_send_pin', pin);
}

//Initializes a deposit operation
function openDeposit() {
    API.triggerServerEvent('atm_open_deposit');
}

//Initializes a withdraw operation
function openWithdraw() {
    API.triggerServerEvent('atm_open_withdraw');
}

//Initializes a transfer operation
function openTransfer() {
    API.triggerServerEvent('atm_open_transfer');
}

//Opens the historic
function openHistoric() {
    API.triggerServerEvent('atm_open_historic');
}

//Cancels current operation
function cancelOperation() {
    API.triggerServerEvent('atm_cancel_operation');
}

//Sends the deposit data to server
function makeDeposit(amount, concept) {
    API.triggerServerEvent('atm_make_deposit', amount, concept);
}

//Sends the withdraw data to server
function makeWithdraw(amount, concept) {
    API.triggerServerEvent('atm_make_withdraw', amount, concept);
}

//Confirms the transfer data to server
function makeTransfer(account, amount, concept) {
    API.triggerServerEvent('atm_make_transfer', account, amount, concept);
}

//Confirms a deposit operation
function confirmDeposit() {
    API.triggerServerEvent('atm_confirm_deposit');
}

//Confirms a withdraw operation
function confirmWithdraw() {
    API.triggerServerEvent('atm_confirm_withdraw');
}

//Confirms a transfer operation
function confirmTransfer() {
    API.triggerServerEvent('atm_confirm_transfer');
}

//Sends ATM exit signal to server
function closeATM() {
    API.triggerServerEvent('atm_close');
}

//Debug message
function debugMessage(message) {
    API.sendChatMessage(message);
}
