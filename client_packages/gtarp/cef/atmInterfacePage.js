var page = 0;

//Initial fade in
$(document).ready(function () {
    resourceCall('requestInitializationData');
});


//Switches from a page to another
function switchToPage(nextPage, data, moreData) {

    //First we hide the current page
    switch (page) {
        case 0: {
            closeCardList();
            break;
        }
        case 1: {
            closePinPrompt();
            break;
        }
        case 2: {
            closeMainMenu();
            break;
        }
        case 3: {
            closeDepositPrompt();
            break;
        }
        case 4: {
            closeWithdrawPrompt();
            break;
        }
        case 5: {
            closeTransferPrompt();
            break;
        }
        case 6: {
            closeDepositConf();
            break;
        }
        case 7: {
            closeWithdrawConf();
            break;
        }
        case 8: {
            closeTransferConf();
            break;
        }
        case 9: {
            closeHistoricPage();
            break;
        }
    }

    //Then switch to the new one passing the generic data
    setTimeout(function () {
        switch (nextPage) {
            case 0: {
                openCardList(data);
                break;
            }
            case 1: {
                openPinPrompt();
                break;
            }
            case 2: {
                openMainMenu(data, moreData);
                break;
            }
            case 3: {
                openDepositPrompt();
                break;
            }
            case 4: {
                openWithdrawPrompt(data);
                break;
            }
            case 5: {
                openTransferPrompt(data);
                break;
            }
            case 6: {
                openDepositConf(data);
                break;
            }
            case 7: {
                openWithdrawConf(data);
                break;
            }
            case 8: {
                openTransferConf(data);
                break;
            }
            case 9: {
                openHistoricPage(data);
                break;
            }
        }
    }, 1000);
}

//Activates the page's buttons
function activateButtons() {
    switch (page) {
        case 0: {
            $('#cardList button').removeAttr('disabled');
            break;
        }
        case 1: {
            $('#pinButton').removeAttr('disabled');
            break;
        }
        case 2: {
            break;
        }
        case 3: {
            $('#makeDepositButton').removeAttr('disabled');
            break;
        }
        case 4: {
            $('#makeWithdrawButton').removeAttr('disabled');
            break;
        }
        case 5: {
            $('#makeTransferButton').removeAttr('disabled');
            break;
        }
        case 6: {
            break;
        }
        case 7: {
            break;
        }
        case 8: {
            break;
        }
        case 9: {
            break;
        }
    }
}

//Receives initialization data and proceed
function atmInitialization(bank, cards) {
    switch (bank) {
        case -1: {
            $('#atmLogo').html('<img src=\"assets/img/banks/free.png\"/>');
            break;
        }
		case 0: {
			$('#atmLogo').html('<img src=\"assets/img/banks/fleeca.png\"/>');
			break;
		}
		case 1: {
			$('#atmLogo').html('<img src=\"assets/img/banks/mazebank.png\"/>');
			break;
		}
		case 2: {
			$('#atmLogo').html('<img src=\"assets/img/banks/pacificstandard.png\"/>');
			break;
		}
		case 3: {
			$('#atmLogo').html('<img src=\"assets/img/banks/lombank.jpg\"/>');
			break;
		}
	}

	$('#content').removeClass('invisible');

    openCardList(cards);

}

//Opens the cardList page
function openCardList(cards) {
    var cardData = JSON.parse(cards);

	for(var i = 0; i<cardData.length; i++) {
        $('#cardList tbody').append('<tr><td class=\"text-center\" style=\"max-width: 100px\"><strong>' + cardData[i] + '</strong></td><td class=\"text-center\" style=\"max-width: 50px\"><button type=\"button\" class=\"btnBlue center\" onClick="selectCard(\'' + cardData[i] + '\')">Introducir</button></td></tr>');
	}

	page = 0;

	//$('#cardList button').removeAttr('disabled');

    $('#cardSelector').fadeIn().removeClass('invisible');
}

//Closes the cardList page
function closeCardList() {
	$('#cardSelector').fadeOut().addClass('invisible');
	$('#cardList tbody').html('');
}

//Send the selected card to server
function selectCard(cardNumber) {
	$('#cardList button').attr('disabled', 'disabled');
    resourceCall('sendSelectedCard', cardNumber);
}

//Opens the PIN prompt
function openPinPrompt() {
	page = 1;
    $('#pinInput').val('');
    $('#pinButton').removeAttr('disabled');
    $('#pin').fadeIn().removeClass('invisible');
    $('#pinInput').focus();
}

//Send the card PIN to server
function sendPIN() {
    var pin = $('#pinInput').val();
    if (pin != '') {
        resourceCall('sendPIN', pin);
        $('#pinButton').attr('disabled', 'disabled');
    }
}

//Closes the PIN prompt
function closePinPrompt() {
	$('#pinInput').val('');
	$('#pin').fadeOut().addClass('invisible');
}

//Open the main menu
function openMainMenu(data, hasDebt) {
    resourceCall('debugMessage', "OPEN MAIN MENU: " + data + " " + hasDebt);
	if(data != 'no') {
		var accountData = JSON.parse(data);

		$('#infoAccountNumber').text(accountData[0]);
		$('#infoAccountType').text(accountData[1]);
		$('#infoAccountOwner').text(accountData[2]);
		$('#infoMoney').text(accountData[3]);
		$('#infoDebt').text(accountData[4]);
		$('#infoLastMovement').text(accountData[5]);
		
	}

	if(hasDebt) {
		$('#withdrawButton').attr('disabled', 'disabled');
		$('#transferButton').attr('disabled', 'disabled');
	}else{
		$('#withdrawButton').removeAttr('disabled');
		$('#transferButton').removeAttr('disabled');
	}

	page = 2;
	$('#main').fadeIn().removeClass('invisible');
}

//Closes the main menu
function closeMainMenu() {
	$('#main').fadeOut().addClass('invisible');
}

//Main menu deposit button
function openDeposit() {
    resourceCall('openDeposit');
}

//Main menu withdraw button
function openWithdraw() {
    resourceCall('openWithdraw');
}

//Main menu transfer button
function openTransfer() {
    resourceCall('openTransfer');
}

//Main menu historic button
function openHistoric() {
    resourceCall('openHistoric');
}

//Cancel operation button (cancels current operation and returns to main menu)
function cancelOperation() {
    resourceCall('cancelOperation');
}

//Make deposit (prepare)
function makeDeposit() {
    var amount = $('#depositAmount').val();
    var concept = $('#depositConcept').val();

    if (amount != '') {
        if (concept == '') {
            concept = " "
        }

        resourceCall('makeDeposit', amount, concept);
        $('#makeDepositButton').attr('disabled', 'disabled');
    }
}

//Make withdraw (prepare)
function makeWithdraw() {
    var amount = $('#withdrawAmount').val();
    var concept = $('#withdrawConcept').val();

    if (amount != '') {
        if (concept == '') {
            concept = " "
        }

        resourceCall('makeWithdraw', amount, concept);
        $('#makeWithdrawButton').attr('disabled', 'disabled');
    }
}

//Make transfer (prepare)
function makeTransfer() {
    var account = $('#transferAccount').val();
    var amount = $('#transferAmount').val();
    var concept = $('#transferConcept').val();

    if (account != '' && amount != '') {
        if (concept == '') {
            concept = " "
        }

        resourceCall('makeTransfer', account, amount, concept);
        $('#makeTransferButton').attr('disabled', 'disabled');
    }
}

//Main menu close ATM button
function closeATM() {
    resourceCall('closeATM');
}

//Open the deposit prompt
function openDepositPrompt() {
	$('#depositAmount').val('');
	$('#depositConcept').val('');
	page = 3;
    $('#depositPrompt').fadeIn().removeClass('invisible');
    $('#depositAmount').focus();
}

//Send the deposit request to server
function sendDepositRequest() {
	var depositAmount = $('#depositAmount').val();
	var depositConcept = $('#despositConcept').val();

	resourceCall('sendDepositRequest', depositAmount, depositConcept);
	$('#makeDepositButton').attr('disabled', 'disabled');
}

//Closes the deposit prompt
function closeDepositPrompt() {
	$('#depositAmount').val('');
	$('#depositConcept').val('');
	$('#depositPrompt').fadeOut().addClass('invisible');
}

//Open the withdraw prompt
function openWithdrawPrompt(availableMoney) {
	$('#withdrawActualMoney').text(availableMoney);
	$('#withdrawAmount').val('');
	$('#withdrawConcept').val('');
	page = 4;
    $('#withdrawPrompt').fadeIn().removeClass('invisible');
    $('#withdrawAmount').focus();
}

//Send the withdraw request to server
function sendWithdrawRequest() {
	var withdrawAmount = $('#withdrawAmount').val();
	var withdrawConcept = $('#withdrawConcept').val();

	resourceCall('sendWithdrawRequest', withdrawAmount, withdrawConcept);
	$('#makeWithdrawButton').attr('disabled', 'disabled');
}

//Closes the withdraw prompt
function closeWithdrawPrompt() {
	$('#withdrawActualMoney h4').text('');
	$('#withdrawAmount').val('');
	$('#withdrawConcept').val('');
	$('#withdrawPrompt').fadeOut().addClass('invisible');
}

//Open the transfer prompt
function openTransferPrompt(availableMoney) {
	$('#transferActualMoney').text(availableMoney);
	$('#transferAccount').val('');
	$('#transferAmount').val('');
	$('#transferConcept').val('');
	page = 5;
    $('#transferPrompt').fadeIn().removeClass('invisible');
    $('#transferAccount').focus();
}

//Send the transfer request to server
function sendTransferRequest() {
	var transferAccount = $('#transferAccount').val();
	var transferAmount = $('#transferAmount').val();
	var transferConcept = $('#transferConcept').val();
	resourceCall('sendTransferRequest', transferAccount, transferAmount, transferConcept);
	$('#makeTransferButton').attr('disabled', 'disabled');
}

//Closes the transfer prompt
function closeTransferPrompt() {
	$('#transferActualMoney').text('');
	$('#transferAccount').val('');
	$('#transferAmount').val('');
	$('#transferConcept').val('');
	$('#transferPrompt').fadeOut().addClass('invisible');
}

//Open the deposit conf page
function openDepositConf(data) {
	var operationData = JSON.parse(data);

	$('#depositConfAccount').text(operationData[0]);
	$('#depositConfActualMoney').text(operationData[1]);
	$('#depositConfDebt').text(operationData[2]);
	$('#depositConfDepositMoney').text(operationData[3]);
	$('#depositConfFinalMoney').text(operationData[4]);
	$('#depositConfConcept').text(operationData[5]);

	page = 6;
	$('#depositConf').fadeIn().removeClass('invisible');
}

//Closes the deposit conf page
function closeDepositConf() {
	$('#depositConf').fadeOut().addClass('invisible');
}

//Open the withdraw conf page
function openWithdrawConf(data) {
	var operationData = JSON.parse(data);

	$('#withdrawConfAccount').text(operationData[0]);
	$('#withdrawConfAccountType').text(operationData[1]);
	$('#withdrawConfActualMoney').text(operationData[2]);
	$('#withdrawConfComissionType').text(operationData[3]);
	$('#withdrawConfComissionAmount').text(operationData[4]);
	$('#withdrawConfMoney').text(operationData[5]);
	$('#withdrawConfFinalMoney').text(operationData[6]);
	$('#withdrawConfConcept').text(operationData[7]);

	page = 7;
	$('#withdrawConf').fadeIn().removeClass('invisible');
}

//Closes the withdraw conf page
function closeWithdrawConf() {
	$('#withdrawConf').fadeOut().addClass('invisible');
}

//Open the transfer conf page
function openTransferConf(data) {
	var operationData = JSON.parse(data);

	$('#transferConfAccount').text(operationData[0]);
	$('#transferConfBeneficiary').text(operationData[1]);
	$('#transferConfActualMoney').text(operationData[2]);
	$('#transferConfComissionType').text(operationData[3]);
	$('#transferConfComissionAmount').text(operationData[4]);
	$('#transferConfMoney').text(operationData[5]);
	$('#transferConfFinalMoney').text(operationData[6]);
	$('#transferConfConcept').text(operationData[7]);

	page = 8;
	$('#transferConf').fadeIn().removeClass('invisible');
}

//Confirms the deposit operation
function confirmDeposit() {
    resourceCall('confirmDeposit');
}

//Confirms the withdraw operation
function confirmWithdraw() {
    resourceCall('confirmWithdraw');
}

//Confirms the transfer operation
function confirmTransfer() {
    resourceCall('confirmTransfer');
}

//Closes the transfer conf page
function closeTransferConf() {
	$('#transferConf').fadeOut().addClass('invisible');
}

//Open the historic page
function openHistoricPage(data) {
	var historicData = JSON.parse(data);

	for(var i = 0; i<historicData.length; i++) {
		$('#historicList tbody').append('<tr><td style=\"width:15%\"><small>' + historicData[i][0] + '</small></td><td style=\"width:15%\"><small>' + historicData[i][1] + '</small></td><td style=\"width:15%\"><small>' + historicData[i][2] + '</small></td><td style=\"width:55%\"><small>' + historicData[i][3] + '</small></td></tr>');
	}

	page = 9;
	$('#historic').fadeIn().removeClass('invisible');
}

//Closes the historic page
function closeHistoricPage() {
	$('#historic').fadeOut().addClass('invisible');
	$('#historicList tbody').html('');
}
