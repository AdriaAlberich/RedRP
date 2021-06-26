
$(document).ready(function () {
	resourceCall('requestData');
});

function setPlayerData(data) {
	//Account data initialization
	var processedData = JSON.parse(data);
	for(var i = 0; i<processedData.length; i++) {
		$('#playerList tbody').append('<tr><td class=\"text-center\"><strong>' + processedData[i][0] + '</strong></td><td class=\"text-center\"><strong>' + processedData[i][1] + '</strong></td><td class=\"text-center\"><strong>' + processedData[i][2] + '</strong></td></tr>');
	}
}

function setVisible() {
	$('#content').removeClass('invisible');
}



