
$(document).ready(function () {
    mp.trigger('requestAccountInfoData');
});

function setAccountName(name) {
	//Account name
	$('#accountName').text(name);
}

function setAccountData(data) {
	//Account data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#account tbody').append('<tr><td class=\"text-left\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\">' + processedData[i][1] + '</td></tr>');
	}
}

function setCharacterName(name) {
	//Character name
	$('#characterName').text(name);
}

function setCharacterGeneralData(data) {
	//Character general data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#characterGeneral tbody').append('<tr><td class=\"text-left\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\">' + processedData[i][1] + '</td></tr>');
	}
}

function setCharacterProperties(data) {
	//Character properties data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#characterProperties tbody').append('<tr><td class=\"text-left\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\">' + processedData[i][1] + '</td></tr>');
	}
}

function setCharacterExperience(data) {
	//Character experience data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#characterExperience tbody').append('<tr><td class=\"text-left\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\"><span>' + processedData[i][1] + ' / 100</span><br/><progress max=\"100\" value=\"' + processedData[i][1] + '\"></progress></td></tr>');
	}
}

function setVisible() {
	$('#content').removeClass('invisible');
}



