var invBrowser = null;

var leftTitle = null;
var leftCapacity = null;
var leftCapacityMax = null;
var leftItems = null;
var leftLocked = null;
var rightTitle = null;
var rightCapacity = null;
var rightCapacityMax = null;
var rightItems = null;

//Here we handle events triggered from server
API.onServerEventTrigger.connect(function (name, args) {

    //Show the inventory interface
    if (name == 'show_inv_interface') {

        var res = API.getScreenResolution();
        invBrowser = API.createCefBrowser(res.Width, res.Height, true);
        API.waitUntilCefBrowserInit(invBrowser);
        API.setCefBrowserPosition(invBrowser, 0, 0);

        API.loadPageCefBrowser(invBrowser, 'client/cef/inventoryInterfacePage.html');
        API.setCanOpenChat(false);
        API.showCursor(true);

        leftTitle = args[0];
        leftCapacity = args[1];
        leftCapacityMax = args[2];
        leftItems = args[3];
        leftLocked = args[4];
        rightTitle = args[5];
        rightCapacity = args[6];
        rightCapacityMax = args[7];
        rightItems = args[8];
    }

    //Hide the inventory interface
    if (name == 'hide_inv_interface') {
        API.destroyCefBrowser(invBrowser);
        API.setCanOpenChat(true);
        API.showCursor(false);
        invBrowser = null;
    }

    //Adds an item to the inventory interface
    if (name == 'add_item_inv_interface') {
        var isRight = args[0];
        var itemName = args[1];
        var itemImage = args[2];
        var itemUses = args[3];
        var capacity = args[4];

        if (isRight) {
            invBrowser.call('addRightItem', itemName, itemImage, itemUses, capacity);
        } else {
            invBrowser.call('addLeftItem', itemName, itemImage, itemUses, capacity);
        }

        debugMessage('addItem: ' + isRight + ' ' + itemName);
    }

    //Removes an item to the inventory interface
    if (name == 'remove_item_inv_interface') {
        var isRight = args[0];
        var itemId = args[1];
        var capacity = args[2];

        if (isRight) {
            invBrowser.call('removeRightItem', itemId, capacity);
        } else {
            invBrowser.call('removeLeftItem', itemId, capacity);
        }

        debugMessage('removeItem: ' + isRight + ' ' + itemId);
    }

    //Adds an special item to the inventory interface
    if (name == 'add_special_item_inv_interface') {
        var index = args[0];
        var itemName = args[1];
        var itemImage = args[2];
        var itemUses = args[3];
        var capacity = args[4];


        invBrowser.call('putSpecialItem', index, itemName, itemImage, itemUses, capacity);

        debugMessage('addSpecialItem: ' + index + ' ' + itemName);
    }

    //Removes an special item from the inventory interface
    if (name == 'remove_special_item_inv_interface') {
        var index = args[0];
        var capacity = args[1];

        invBrowser.call('removeSpecialItem', index, capacity);

        debugMessage('removeSpecialItem: ' + index);
    }

    //Open an item container
    if (name == 'open_container_inv_interface') {
        var isRight = args[0];
        var title = args[1];
        var occupiedSlots = args[2];
        var capacity = args[3];
        var items = args[4];

        if (isRight) {
            invBrowser.call('openRightContainer', title, occupiedSlots, capacity, items);
        } else {
            invBrowser.call('openLeftContainer', title, occupiedSlots, capacity, items);
        }

        debugMessage('openItemContainer: ' + isRight + ' ' + title);
    }

    //Close an item container
    if (name == 'close_container_inv_interface') {
        var isRight = args[0];
        var title = args[1];
        var occupiedSlots = args[2];
        var capacity = args[3];
        var items = args[4];

        if (isRight) {
            invBrowser.call('closeRightContainer', title, occupiedSlots, capacity, items);
        } else {
            invBrowser.call('closeLeftContainer', title, occupiedSlots, capacity, items);
        }

        debugMessage('closeItemContainer: ' + isRight + ' ' + title);
    }
});

//Sends initialization data when inventory page is loaded
function requestInitializationData() {
    invBrowser.call("inventoryInitialization", leftTitle, leftCapacity, leftCapacityMax, leftItems, leftLocked, rightTitle, rightCapacity, rightCapacityMax, rightItems);
}

//Sends inventory exit signal to server
function closeInventory() {
    API.triggerServerEvent('inv_close');
}

//Sends item simple click event to the server
function sendItemClick(isRight, itemId) {
    if (isRight) {
        API.triggerServerEvent('inv_item_click', 1, itemId);
    } else {
        API.triggerServerEvent('inv_item_click', 0, itemId);
    }

    debugMessage('sendItemClick: ' + isRight + ' ' + itemId);
}

//Sends item double click event to the server
function sendItemDoubleClick(isRight, itemId) {
    if (isRight) {
        API.triggerServerEvent('inv_item_double_click', 1, itemId);
    } else {
        API.triggerServerEvent('inv_item_double_click', 0, itemId);
    }

    debugMessage('sendItemDoubleClick: ' + isRight + ' ' + itemId);
}

//Sends item right click event to the server
function sendItemRightClick(isRight, itemId) {
    if (isRight) {
        API.triggerServerEvent('inv_item_right_click', 1, itemId);
    } else {
        API.triggerServerEvent('inv_item_right_click', 0, itemId);
    }

    debugMessage('sendItemRightClick: ' + isRight + ' ' + itemId);
}

//Sends return button click to the server
function sendReturnClick(isRight) {
    if (isRight) {
        API.triggerServerEvent('inv_return_click', 1);
    } else {
        API.triggerServerEvent('inv_return_click', 0);
    }

    debugMessage('sendReturnClick: ' + isRight);
}

//Debug message
function debugMessage(message) {
    API.sendChatMessage(message);
}
