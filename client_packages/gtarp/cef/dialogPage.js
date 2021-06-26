var input = true;

//Initial fade in
$(document).ready(function () {
	resourceCall('requestDialogData');
});

//Key handling
$(document).keypress(function(e) {
	switch(e.which) {
		case 13: {
			dialogAccept(); 
			break;
		}
	}
});

//Dialog initialization
function dialogInit(title, text, hasInput, acceptButtonText, cancelButtonText) {
	$('#dialogTitle').text(title);
	$('#dialogText').text(text);
	if(!hasInput) {
		input = false;
		$('#dialogZoneInput').remove();
	}
	$('#dialogAccept').text(acceptButtonText);
	$('#dialogCancel').text(cancelButtonText);
	$('#content').removeClass('invisible');
	$('#dialogInput').focus();
}

//Dialog accept button
function dialogAccept() {

	var inputText = "";

	if(input) {
		inputText = $('#dialogInput').val();
	}

	resourceCall('sendDialogAccept', inputText);
}

//Dialog cancel button
function dialogCancel() {
	resourceCall('sendDialogCancel');
}




