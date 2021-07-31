$(document).ready(function () {

	$("#product_search").on('keyup', function () {
		var product_name = $(this).val().toLowerCase();
		executeFilter($(this), product_name);
	});
	$("#account_search").on('keyup', function () {
		var account_name = $(this).val().toLowerCase();
		executeFilter($(this), account_name);
	});
	$("#category_search").on('keyup', function () {
		var category_name = $(this).val().toLowerCase();
		executeFilter($(this), category_name);
	});
	$("#order_search").on('keyup', function () {
		var account_name = $(this).val().toLowerCase();
		executeFilter($(this), account_name);
	});
});

function executeFilter(element, name) {
	var searchCol = element.closest('.container').find("#table-container tr");
	searchCol.filter(function() {
		$(this).toggle($(this).find('#search-column').text().toLowerCase().indexOf(name) > -1)
	});
	
}

