$(document).ready(function () {

    
	$("#order_filter_btn").click(function () {
		executeSearch();
	});
});

function executeSearch() {
	var name = $('#account_name_input').val();
	var price = $('#price_input').val();
	var date = $('#date_input').val();

	$.ajax({
		url: "/Orders/Filter",
		dataType: "json",
		type: "POST",
		data: {
			accountName: name,
			price: price,
			date: date
		},
		success: function (data) {
			$('#search-results').tmpl(data).appendTo('#table-container');
		},

	});
}
