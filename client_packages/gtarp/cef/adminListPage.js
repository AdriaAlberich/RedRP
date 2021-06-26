
$(document).ready(function () {
	resourceCall('requestData');
});

function setAdminData(data) {
	//Account data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#adminList tbody').append('<tr><td class=\"text-center\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\"><strong>' + processedData[i][1] + '</strong></td><td class=\"text-center\"><span style=\"color: ' + processedData[i][3] + '\"><strong>' + processedData[i][2] + '</strong></span></td></tr>');
	}
}

function setVisible() {
	$('#content').removeClass('invisible');
}



