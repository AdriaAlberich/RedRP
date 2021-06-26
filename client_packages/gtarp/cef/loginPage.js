var view = 0;

//Initial fade in
$(document).ready(function () {
	resourceCall('requestInitializationData');
});

//Enter key handling
$(document).keypress(function(e) {
  if(e.which == 13) {
  	if(view == 0) {
  		login();
  	}
  }
});

//Receives initialization data and proceed
function initialization(type, data) {
	view = type;
	$('#content').removeClass('invisible');
	if(type == 0) {
		showPlayerName(data);
	}else{
		initializeCharacterSelector(data);
	}
}

//Login checks
function login() {
	try {
		var nickOrEmail = $('#nick').val();
		var password = $('#password').val();
		if(password === '') {
			showError('Debes introducir la contraseña.');
		}else{
			resourceCall('sendLogin', nickOrEmail, password);
			$('#button').attr('disabled', 'disabled');
		}
	}
	catch(err) {
		showError("Error desconocido, contacta con un administrador.")
	}
}

//Show an error inside the login dialog
function showError(message) {
	if(view == 0){
		$('#loginErrorMessage').text(message);
		$('#loginErrorMessage').fadeIn().removeClass('invisible');
		$('#loginButton').removeAttr('disabled');
	}else{
		$('#characterErrorMessage').text(message);
		$('#characterErrorMessage').fadeIn().removeClass('invisible');
		$('#characterList button').removeAttr('disabled');
	}
	
}

//Sets the player name on the welcome message
function showPlayerName(name) {
	$('#playerName').text(name);
	$('#nick').val(name);
	$('#login').fadeIn().removeClass('invisible');
	$('#password').focus();
}

//Switched from login to character selector
function switchToCharacterSelector(data) {
	$('#login').fadeOut().addClass('invisible');
	view = 1;
	setTimeout(function() {
		initializeCharacterSelector(data);
	}, 1000);
	
}

//Initializes the character selector list
function initializeCharacterSelector(data) { 
	var processedData = JSON.parse(data);

	for(var i = 0; i<processedData.length; i++) {
		var characterData = processedData[i].split(',');
		$('#characterList tbody').append('<tr><td class=\"text-center\" style=\"max-width: 100px\"><strong>' + characterData[1] + '</strong></td><td class=\"text-center\" style=\"max-width: 50px\"><button type=\"button\" class=\"btnBlue center\" onClick="normalSpawn(' + characterData[0] + ')">Ultima posición</button></td><td class=\"text-center\" style=\"max-width: 50px\"><button type=\"button\" class=\"btnBlue center\" onClick="homeSpawn(' + characterData[0] + ')">Vivienda</button></td></tr>');
	}

	$('#characterSelector').fadeIn().removeClass('invisible');
}

//Spawn on last position callback
function normalSpawn(character) {
	$('#characterList button').attr('disabled', 'disabled');
	resourceCall('sendSpawnRequest', false, character);
}

//Spawn on home callback
function homeSpawn(character) {
	$('#characterList button').attr('disabled', 'disabled');
	resourceCall('sendSpawnRequest', true, character);
}
