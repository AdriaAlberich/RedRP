var pageLeft = 0;
var maxPageLeft = 0;
var pageRight = 0;
var maxPageRight = 0;

var leftLocked = false;
var leftTotalItems = null;
var leftCurrentCapacity = 0;
var leftCurrentTotalCapacity = 0;

var rightTotalItems = null;
var rightCurrentCapacity = 0;
var rightCurrentTotalCapacity = 0;
var isRightContainer = false;

var doubleClick = 0;

//Initial fade in
$(document).ready(function () {
    resourceCall('requestInitializationData');
});

//Inventory initialization function
function inventoryInitialization(leftTitle, leftCapacity, leftCapacityMax, leftItems, leftLocked, rightTitle, rightCapacity, rightCapacityMax, rightItems) {

    resourceCall('debugMessage', 'START');
    //Left grid initialization
    pageLeft = 1;
    setLeftTitle(leftTitle);
    leftCurrentCapacity = leftCapacity;
    leftCurrentTotalCapacity = leftCapacityMax;
    updateLeftCapacityIndicator();
    leftTotalItems = JSON.parse(leftItems);
    loadLeftItems(0);
    leftLocked = leftLocked;

    resourceCall('debugMessage', 'LEFT GRID');

    //Handles the left grid left pagination button click event
    $('#leftPaginateLeft').click(function () {
        if (pageLeft > 1) {
            pageLeft -= 1;
            var startItem = pageLeft * 30 - 30;
            clearLeftItems();
            resourceCall('debugMessage', 'CLEAR ITEMS');
            loadLeftItems(startItem);
            resourceCall('debugMessage', 'LOAD ITEMS START AT: ' + startItem);
            updateLeftPagination();
        }
    });

    //Handles the left grid right pagination button click event
    $('#leftPaginateRight').click(function () {
        if (pageLeft < maxPageLeft) {
            pageLeft += 1;
            var startItem = pageLeft * 30 - 30;
            clearLeftItems();
            resourceCall('debugMessage', 'CLEAR ITEMS');
            loadLeftItems(startItem);
            resourceCall('debugMessage', 'LOAD ITEMS START AT: ' + startItem);
            updateLeftPagination();
        }
    });

    //Handles the left grid item simple click event
    $('.itemLeft').click(function () {
        var hasTitle = $(this).attr('title');
        if (typeof hasTitle !== typeof undefined && hasTitle !== false) {
            var itemId = parseInt($(this).attr('id')) - 1;
            var startItem = pageLeft * 30 - 30;
            itemId = itemId + startItem;
            doubleClick++;
            if (doubleClick === 2) {
                resourceCall('sendItemDoubleClick', false, itemId);
                doubleClick = 0;
            } else {
                setTimeout(function () {
                    if (doubleClick === 1) {
                        resourceCall('sendItemClick', false, itemId);
                        doubleClick = 0;
                    }
                }, 300);
            }
        }
    });

    //Handles the left grid item right click event
    $('.itemLeft').contextmenu(function (e) {
        var hasTitle = $(this).attr('title');
        if (typeof hasTitle !== typeof undefined && hasTitle !== false) {
            e.preventDefault();
            var itemId = parseInt($(this).attr('id')) - 1;
            var startItem = pageLeft * 30 - 30;
            itemId = itemId + startItem;
            resourceCall('sendItemRightClick', false, itemId);
        }
    });

    //Handles the left grid return button click event
    $('#returnLeft').click(function () {
        resourceCall('sendReturnClick', false);
    });

    resourceCall('debugMessage', 'LEFT GRID EVENTS');

    //Right grid initialization
    pageRight = 1;
    setRightTitle(rightTitle);
    rightCurrentCapacity = rightCapacity;
    rightCurrentTotalCapacity = rightCapacityMax;
    updateRightCapacityIndicator();
    rightTotalItems = JSON.parse(rightItems);
    //updateRightPagination(rightTotalItems.lenght);
    loadRightItems();

    resourceCall('debugMessage', 'RIGHT GRID');

    //Handles the right grid item simple click event
    $('.itemRight').click(function () {
        var hasTitle = $(this).attr('title');
        if (typeof hasTitle !== typeof undefined && hasTitle !== false) {
            var itemId = parseInt($(this).attr('id')) - 1;
            var startItem = pageRight * 30 - 30;
            itemId = itemId + startItem;
            doubleClick++;
            if (doubleClick === 2) {
                resourceCall('sendItemDoubleClick', true, itemId);
                doubleClick = 0;
            } else {
                setTimeout(function () {
                    if (doubleClick === 1) {
                        resourceCall('sendItemClick', true, itemId);
                        doubleClick = 0;
                    }
                }, 300);
            }
        }
    });

    //Handles the right grid item right click event
    $('.itemRight').contextmenu(function (e) {
        var hasTitle = $(this).attr('title');
        if (typeof hasTitle !== typeof undefined && hasTitle !== false) {
            e.preventDefault();
            var itemId = parseInt($(this).attr('id')) - 1;
            var startItem = pageRight * 30 - 30;
            itemId = itemId + startItem;
            resourceCall('sendItemRightClick', true, itemId);
        }
    });

    //Handles the right grid return button click event
    $('#returnRight').click(function () {
        resourceCall('sendReturnClick', true);
    });

    resourceCall('debugMessage', 'RIGHT GRID EVENTS');

    $('.inventoryItem').hover(function () {
        var hasTitle = $(this).attr('title');
        if (typeof hasTitle !== typeof undefined && hasTitle !== false) {
            $('#showInfo').text(hasTitle);
        }
    }, function () {
        $('#showInfo').text('');
    });

    $('#content').removeClass('invisible');

}

//Sets the left grid title
function setLeftTitle(title) {
    $('#titleLeft').text(title);
}

//Sets the left grid capacity indicator
function updateLeftCapacityIndicator() {
    $('#capacityLeft').text(leftCurrentCapacity + '/' + leftCurrentTotalCapacity);
}

//Loads all items on the left grid
function loadLeftItems(startItem) {
    var pageItemCount = 0;
    if (leftTotalItems.length - startItem > 30) {
        pageItemCount = startItem + 30;
    } else {
        pageItemCount = leftTotalItems.length;
    }

    var count = 1;
    for (var i = startItem; i < pageItemCount; i++) {
        if (leftTotalItems[i] !== 'null') {
            var itemData = leftTotalItems[i].split(';');
            var itemName = itemData[0];
            var itemImage = itemData[1];
            var itemUses = itemData[2];

            setLeftItem(count, itemName, itemImage, itemUses);

            count++;
        }
    }

    updateLeftPagination();
}

//Clears to the default state all left grid item slots
function clearLeftItems() {
    for (var i = 1; i <= 30; i++) {
        clearLeftItem(i);
    }
}

//Reloads left items
function reloadLeftItems() {
    var startItem = pageLeft * 30 - 30;
    clearLeftItems();
    loadLeftItems(startItem);
    updateLeftCapacityIndicator();
}

//Sets an item slot occupied in the left grid
function setLeftItem(itemId, itemName, itemImage, itemUses) {
    $('#contentLeft > #' + itemId).attr('title', itemName);
    $('#contentLeft > #' + itemId).css('background', 'url(assets/img/items/' + itemImage + ')');
    $('#contentLeft > #' + itemId + ' > div').text(itemUses);
}

//Clears an item slot in the left grid
function clearLeftItem(itemId) {
    $('#contentLeft > #' + itemId).removeAttr('title');
    $('#contentLeft > #' + itemId).css('background', '');
    $('#contentLeft > #' + itemId + ' > div').text('');
}

//Remove left item at position
function removeLeftItem(itemId, capacity) {
    leftTotalItems.splice(itemId, 1);
    leftCurrentCapacity -= capacity;
    reloadLeftItems();
}

//Adds left item at the end
function addLeftItem(itemName, itemImage, itemUses, capacity) {
    leftTotalItems.push(itemName + ';' + itemImage + ';' + itemUses);
    leftCurrentCapacity += capacity;
    reloadLeftItems();
}

//Updates the pagination data on the left grid
function updateLeftPagination() {
    if (leftTotalItems.length > 30) {
        maxPageLeft = Math.ceil(leftTotalItems.length / 30);
    } else {
        maxPageLeft = 1;
    }

    resourceCall('debugMessage', 'Max page left: ' + maxPageLeft + ' ' + leftTotalItems.length);

    $('#paginationCounterLeft').text(pageLeft + '/' + maxPageLeft);
}

//Open a container at the left grid
function openLeftContainer(title, occupiedSlots, maxCapacity, items) {
    setLeftTitle(title);
    leftCurrentCapacity = occupiedSlots;
    leftCurrentTotalCapacity = maxCapacity;
    updateLeftCapacityIndicator();
    leftTotalItems = JSON.parse(items);
    $('#returnLeft').removeClass('invisible');

    reloadLeftItems();
}

//Close a container at the left grid
function closeLeftContainer(title, occupiedSlots, maxCapacity, items) {
    setLeftTitle(title);
    leftCurrentCapacity = occupiedSlots;
    leftCurrentTotalCapacity = maxCapacity;
    updateLeftCapacityIndicator();
    leftTotalItems = JSON.parse(items);
    $('#returnLeft').addClass('invisible');

    reloadLeftItems();
}

//Sets the right grid title
function setRightTitle(title) {
    $('#titleRight').text(title);
}

//Sets the right grid capacity indicator
function updateRightCapacityIndicator() {
    $('#capacityRight').text(rightCurrentCapacity + '/' + rightCurrentTotalCapacity);
}

//Loads all items on the right grid
function loadRightItems() {
    for (var i = 0; i < rightTotalItems.length; i++) {
        if (rightTotalItems[i] !== 'null') {

            var itemData = rightTotalItems[i].split(';');
            var itemName = itemData[0];
            var itemImage = itemData[1];
            var itemUses = itemData[2];

            setRightItem(i + 1, itemName, itemImage, itemUses);

        } else {
            setSpecialItemDefault(i + 1);
        }
    }
}

//Clears to the default state all right grid item slots
function clearRightItems() {
    for (var i = 1; i <= 30; i++) {
        clearRightItem(i);
    }
}

//Reloads right items
function reloadRightItems() {
    clearRightItems();
    loadRightItems();
    //updateRightPagination();
    updateRightCapacityIndicator();
}

//Sets an item slot occupied in the right grid
function setRightItem(itemId, itemName, itemImage, itemUses) {
    $('#contentRight > #' + itemId).attr('title', itemName);
    $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/' + itemImage + ')');
    $('#contentRight > #' + itemId + ' > div').text(itemUses);
}

//Clears an item slot in the right grid
function clearRightItem(itemId) {
    if (itemId > 15 || isRightContainer) {
        $('#contentRight > #' + itemId).removeAttr('title');
        $('#contentRight > #' + itemId).css('background', '');
        $('#contentRight > #' + itemId + ' > div').text('');
    } else {
        setSpecialItemDefault(itemId);
    }
}

//Remove right item at position
function removeRightItem(itemId, capacity) {
    rightTotalItems.splice(itemId, 1);
    rightCurrentCapacity -= capacity;
    reloadRightItems();
}

//Adds right item at the end
function addRightItem(itemName, itemImage, itemUses, capacity) {
    rightTotalItems.push(itemName + ';' + itemImage + ';' + itemUses);
    rightCurrentCapacity += capacity;
    reloadRightItems();
}

//Put special item
function putSpecialItem(index, itemName, itemImage, itemUses, capacity) {
    rightTotalItems[index] = itemName + ';' + itemImage + ';' + itemUses;
    rightCurrentCapacity += capacity;
    reloadRightItems();
}

//Remove special item
function removeSpecialItem(index, capacity) {
    rightTotalItems[index] = 'null';
    rightCurrentCapacity -= capacity;
    reloadRightItems();
}

function setSpecialItemDefault(itemId) {
    $('#contentRight > #' + itemId).removeAttr('title');
    switch (itemId) {
        case 1:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/righthandslot.png)');
            break;
        case 2:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/lefthandslot.png)');
            break;
        case 3:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/bodyarmorslot.png)');
            break;
        case 4:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/backpackslot.png)');
            break;
        case 5:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/glovesslot.png)');
            break;
        case 6:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/hatslot.png)');
            break;
        case 7:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/glassesslot.png)');
            break;
        case 8:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/maskslot.png)');
            break;
        case 9:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/accessoryslot.png)');
            break;
        case 10:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/earslot.png)');
            break;
        case 11:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/torsoslot.png)');
            break;
        case 12:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/legsslot.png)');
            break;
        case 13:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/feetslot.png)');
            break;
        case 14:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/watchslot.png)');
            break;
        case 15:
            $('#contentRight > #' + itemId).css('background', 'url(assets/img/items/special/braceletslot.png)');
            break;
    }

    $('#contentRight > #' + itemId + ' > div').text('');
}

//Open a container at the right grid
function openRightContainer(title, occupiedSlots, maxCapacity, items) {
    setRightTitle(title);
    rightCurrentCapacity = occupiedSlots;
    rightCurrentTotalCapacity = maxCapacity;
    updateRightCapacityIndicator();
    rightTotalItems = JSON.parse(items);
    $('#returnRight').removeClass('invisible');
    isRightContainer = true;
    reloadRightItems();
}

//Close a container at the right grid
function closeRightContainer(title, occupiedSlots, maxCapacity, items) {
    setRightTitle(title);
    rightCurrentCapacity = occupiedSlots;
    rightCurrentTotalCapacity = maxCapacity;
    updateRightCapacityIndicator();
    rightTotalItems = JSON.parse(items);
    $('#returnRight').addClass('invisible');
    isRightContainer = false;
    reloadRightItems();
}

//Updates the pagination data on the right grid
function updateRightPagination(itemCount) {
    /*
    if (itemCount > 30) {
        maxPageRight = itemCount / 30;
    } else {
        maxPageRight = 1;
    }

    $('#paginationCounterRight').text(pageRight + '/' + maxPageRight);
    */
}

//Handles the right grid left pagination button click event
/*
$('#rightPaginateLeft').click(function () {
    if (pageRight > 1) {
        pageRight -= 1;
        var startItem = (pageRight * 30) - 30;
        clearRightItems();
        loadRightItems(rightItems, startItem);
        updateRightPagination();
    }
});

//Handles the right grid right pagination button click event
$('#rightPaginateRight').click(function () {
    if (pageRight < maxPageRight) {
        pageRight += 1;
        var startItem = (pageRight * 30) - 30;
        clearRightItems();
        loadRightItems(rightItems, startItem);
        updateRightPagination();
    }
});*/

