var view = 0;
var socialClubName = '';

// Initial fade in
$(document).ready(function () {
    mp.trigger('requestLoginData');
});

// Enter key handling
$(document).keypress(function(e) {
    if(e.which === 13) {
        if(view === 0) {
            login();
        }
    }
});

// Receives initialization data and proceed
function initialization(type, name, data) {
	view = type;
	socialClubName = name;
	
	switch (type) {
		case 0:
			$('#socialClubNameRegister').text(socialClubName);
			$('#register').fadeIn().removeClass('hidden');
			$('#username').focus();
			break;
		case 1:
			$('#socialClubNameLogin').text(socialClubName);
			$('#login').fadeIn().removeClass('hidden');
			$('#usernameOrEmail').focus();
			break;
		case 2:
			$('#recover').fadeIn().removeClass('hidden');
			$('#recoverEmail').focus();
			break;
		case 3:
			initializeCharacterSelector(data);
    }
}

// Login
function login() {
	try {
		var usernameOrEmail = $('#usernameOrEmail').val();
		var password = $('#password').val();
		if(password === '') {
			showError('You must provide a password.');
		}else{
			mp.trigger('sendLogin', usernameOrEmail, password);
			$('#button').attr('disabled', 'disabled');
		}
	}
	catch(err) {
        showError("Unknown error, contact with the administration.");
	}
}

// Register
function register() {
	try {
		var username = $('#username').val();
		var email = $('#email').val();
		var password = $('#registerPassword').val();
		var passwordConfirmation = $('repeatPassword').val();
		if (username === '') {
			showError('You must provide a valid username.');
		} else {
			if (email === '') {
				showError('You must provide a valid email.');
			} else {
				if (password === '') {
					showError('You must provide a password.');
				} else {
					if (password !== passwordConfirmation) {
						showError('The passwords are not equal.');
					} else {
						mp.trigger('sendRegistration', username, email, password);
						$('#button').attr('disabled', 'disabled');
					}
				}
            }
        }
	}
	catch (err) {
		showError("Unknown error, contact with the administration.");
	}
}

// Show an error inside the login dialog
function showError(message) {

	switch (type) {
		case 0:
			$('#registerErrorMessage').text(message);
			$('#registerErrorMessage').fadeIn().removeClass('hidden');
			$('#registerButton').removeAttr('disabled');
			break;
		case 1:
			$('#loginErrorMessage').text(message);
			$('#loginErrorMessage').fadeIn().removeClass('hidden');
			$('#loginButton').removeAttr('disabled');
			$('#recoverPassword').removeAttr('disabled');
			break;
		case 2:
			$('#recoverErrorMessage').text(message);
			$('#recoverErrorMessage').fadeIn().removeClass('hidden');
			$('#recoverButton').removeAttr('disabled');
			$('#goBack').removeAttr('disabled');
			break;
		case 3:
			$('#characterErrorMessage').text(message);
			$('#characterErrorMessage').fadeIn().removeClass('hidden');
			$('#characterList button').removeAttr('disabled');
			$('#createCharacterButton').removeAttr('disabled');
	}
	
}

// Switch to login from register
function switchToLoginFromRegister() {
	$('#register').fadeOut().addClass('hidden');
	view = 1;
	setTimeout(function () {
		$('#socialClubNameLogin').text(socialClubName);
		$('#login').fadeIn().removeClass('hidden');
		$('#usernameOrEmail').focus();
	}, 1000);
}

// Switch to login from recover
function switchToLoginFromRecover() {
	$('#recover').fadeOut().addClass('hidden');
	view = 1;
	setTimeout(function () {
		$('#socialClubNameLogin').text(socialClubName);
		$('#login').fadeIn().removeClass('hidden');
		$('#usernameOrEmail').focus();
	}, 1000);
}

// Switch to recover from login
function switchToRecoverFromLogin() {
	$('#login').fadeOut().addClass('hidden');
	view = 1;
	setTimeout(function () {
		$('#recover').fadeIn().removeClass('hidden');
		$('#recoverEmail').focus();
	}, 1000);
}

// Switch to character selector from login
function switchToCharacterSelector(data) {
	$('#login').fadeOut().addClass('hidden');
	view = 3;
	setTimeout(function() {
		initializeCharacterSelector(data);
	}, 1000);
}

// Initializes the character selector list
function initializeCharacterSelector(data) { 
	var processedData = JSON.parse(data);

	for(var i = 0; i<processedData.length; i++) {
		var characterData = processedData[i].split(',');
		$('#characterList').append('<button type=\"button\" class=\"btn btn-primary btn-selection\" onClick=\"selectCharacter(' + characterData[0] + ')\">' + characterData[1] + '</button>');
	}

	$('#characterSelector').fadeIn().removeClass('hidden');
}

// Selects a character and try to spawn it (on the future maybe we add the possibility to chose the spawn location if you pay a membership)
function selectCharacter(charId) {
	normalSpawn(charId);
}

// Spawn on last position callback (deprecated)
function normalSpawn(character) {
	$('#characterList button').attr('disabled', 'disabled');
	$('#createCharacterButton').attr('disabled', 'disabled');
	mp.trigger('sendSpawnRequest', false, character);
}

// Spawn on home callback (deprecated)
/*
function homeSpawn(character) {
	$('#characterList button').attr('disabled', 'disabled');
	mp.trigger('sendSpawnRequest', true, character);
}
*/