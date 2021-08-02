$(document).ready(function () {
	$("#order_filter_btn").click(function () {
		executeSearch();
	});
});

$('#loader').hide();
$(document).ajaxStart(function () {
	$('#table-container').empty();
	$('#loader').show();
}).ajaxStop(function () {
	$('#loader').hide();
});


function executeSearch() {
	var address = $('#address_input').val();
	var price = $('#price_input').val();
	var date = $('#date_input').val();

	$.ajax({
		url: "/Orders/Filter",
		type: "POST",
		data: {
			address: address,
			price: price,
			date: date
		},
		success: function (data) {
			for (let i = 0; i < data.length; ++i) {
				var date = new Date(data[i].orderTime);
				data[i]["orderTime"] = date.toLocaleString("en-US");
			}
			$('#search-results').tmpl(data).appendTo('#table-container');
		},

	});
}
