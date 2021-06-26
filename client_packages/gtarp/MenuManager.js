
var menu = null;
var visible = false;

var callbackId;
var title;
var subtitle;
var noExit;
var X;
var Y;
var anchor;
var hasBanner;
var widthOffset;

API.onServerEventTrigger.connect(function (name, args) {
    
    if (name === "menu_handler_create_menu") {

        //Get all arguments
        callbackId = args[0];
        title = args[1];
        subtitle = args[2];
        noExit = args[3];
        X = args[4];
        Y = args[5];
        anchor = args[6];
        hasBanner = args[7];
        widthOffset = args[8];

        //We initialize a new menu
        menu = API.createMenu(title, subtitle, X, Y, anchor, hasBanner);

        menu.RefreshIndex();
        menu.DisableInstructionalButtons(true);

        menu.SetMenuWidthOffset(widthOffset);

        //If banner is disabled we hide the rectangle
        if (!hasBanner) {
            API.setMenuBannerRectangle(menu, 0, 0, 0, 0);
        }

        //If is not cancellable
        if (noExit) {
            menu.ResetKey(menuControl.Back);
        }
        
        //We get the item data in JSON format
        var itemsList = JSON.parse(args[9]);

        //Now we process item descriptions and formats
        var itemDescriptions = null;
        var itemFormats = null;
        if(args[10] !== null) {
            itemDescriptions = JSON.parse(args[10]);
        }

        //We get the item format
        if(args[11] !== null) {
            itemFormats = JSON.parse(args[11]);
        }
        
        var count = 0;
        //Loop into item data and creation of the list.
        for (var i = 0; i < itemsList.length; i++) {
            var item;
            if(itemFormats !== null) {
                if(itemFormats[i] !== "") {
                    var itemFormat = itemFormats[i].split(",");
                    if(itemFormat[0] === "colour") {
                        if(itemDescriptions !== null) {
                            if(count <= 9) { 
                                item = API.createColoredItem("~u~" + count + " - " + itemsList[i], itemDescriptions[i], itemFormat[1], itemFormat[2]);
                            }else{
                                item = API.createColoredItem(itemsList[i], itemDescriptions[i], itemFormat[1], itemFormat[2]);
                            }
                        }else{
                            if(count <= 9) { 
                                item = API.createColoredItem("~u~" + count + " - " + itemsList[i], "", itemFormat[1], itemFormat[2]);
                            }else{
                                item = API.createColoredItem(itemsList[i], "", itemFormat[1], itemFormat[2]);
                            }
                        }
                    }
                }else{
                    if(itemDescriptions !== null) {
                        if(count <= 9) { 
                            item = API.createMenuItem("~r~" + count + "~s~ - " + itemsList[i], itemDescriptions[i]);
                        }else{
                            item = API.createMenuItem(itemsList[i], itemDescriptions[i]);
                        }
                    }else{
                        if(count <= 9) { 
                            item = API.createMenuItem("~r~" + count + "~s~ - " + itemsList[i], "");
                        }else{
                            item = API.createMenuItem(itemsList[i], "");
                        }
                    }
                }
            }else{
                if(itemDescriptions !== null) {
                    if(count <= 9) { 
                        item = API.createMenuItem("~r~" + count + "~s~ - " + itemsList[i], itemDescriptions[i]);
                    }else{
                        item = API.createMenuItem(itemsList[i], itemDescriptions[i]);
                    }
                }else{
                    if(count <= 9) { 
                        item = API.createMenuItem("~r~" + count + "~s~ - " + itemsList[i], "");
                    }else{
                        item = API.createMenuItem(itemsList[i], "");
                    }
                }
            }
            
            count++;
            menu.AddItem(item);
        }

        //We append the itemselect eventhandler with the server-side eventhandler
        menu.OnItemSelect.connect(function(sender, item, index) {
            API.triggerServerEvent("menu_handler_select_item", callbackId, index);
        });

        //Puts the menu visible
        menu.Visible = true;
        visible = true;
    }
    //If close, reset menu pool
    else if (name === "menu_handler_close_menu") {
        menu.Visible = false;
        visible = false;
    }
});

//Update menu
API.onUpdate.connect(function () {
    if (visible) {
        API.disableControlThisFrame(24);
        API.disableControlThisFrame(1);
        API.disableControlThisFrame(2);
    }
});

//Key up (menu hotkeys)
API.onKeyUp.connect(function (sender, args) {
    if (visible) {
        switch (args.KeyCode) {
            case Keys.NumPad0: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 0);
                break;
            }
            case Keys.NumPad1: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 1);
                break;
            }
            case Keys.NumPad2: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 2);
                break;
            }
            case Keys.NumPad3: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 3);
                break;
            }
            case Keys.NumPad4: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 4);
                break;
            }
            case Keys.NumPad5: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 5);
                break;
            }
            case Keys.NumPad6: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 6);
                break;
            }
            case Keys.NumPad7: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 7);
                break;
            }
            case Keys.NumPad8: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 8);
                break;
            }
            case Keys.NumPad9: {
                API.triggerServerEvent("menu_handler_select_item", callbackId, 9);
                break;
            }
        }
    }
});
