$(document).ready(function () {

    
	$("#filter_submit_btn").click(function () {
		executeFilter();
	});

	$("#product_name_input").keypress(function (e) {
		if (e.which == 13) {
			executeFilter();
        }
	});


    // Slider
	$("#min_price,#max_price").on('change', function () {

		var min_price_range = parseInt($("#min_price").val());

		var max_price_range = parseInt($("#max_price").val());

		if (min_price_range > max_price_range) {
			$('#max_price').val(min_price_range);
		}

		$("#slider-range").slider({
			values: [min_price_range, max_price_range]
		});

	});


	$("#min_price,#max_price").on("paste keyup", function () {

		var min_price_range = parseInt($("#min_price").val());

		var max_price_range = parseInt($("#max_price").val());

		if (min_price_range == max_price_range) {

			max_price_range = min_price_range + 100;

			$("#min_price").val(min_price_range);
			$("#max_price").val(max_price_range);
		}

		$("#slider-range").slider({
			values: [min_price_range, max_price_range]
		});

	});


	$(function () {
		$("#slider-range").slider({
			range: true,
			orientation: "horizontal",
			min: 0,
			max: 10000,
			values: [0, 10000],
			step: 100,

			slide: function (event, ui) {
				if (ui.values[0] == ui.values[1]) {
					return false;
				}

				$("#min_price").val(ui.values[0]);
				$("#max_price").val(ui.values[1]);
			}
		});

		$("#min_price").val($("#slider-range").slider("values", 0));
		$("#max_price").val($("#slider-range").slider("values", 1));
	});


    // Products Loader
    $('#loader').hide();
    $(document).ajaxStart(function () {
        $('#products-container').empty();
        $('#loader').show();
    }).ajaxStop(function () {
        $('#loader').hide();
    });
});

function executeFilter() {
	// Name filter
	var name = $('#product_name_input').val();

	// Categories Filter
	var categories = [];
	$("input:checkbox[name=category]:checked").each(function () {
		categories.push($(this).val());
	});

	// Price Filter
	var min_price = $('#min_price').val();
	var max_price = $('#max_price').val();


	$.ajax({
		url: "/Products/Filter",
		dataType: "json",
		type: "POST",
		data: {
			name: name,
			categories: categories,
			minPrice: min_price || 0,
			maxPrice: max_price || Number.MAX_VALUE
		},
		success: function (data) {
			$('#filter-results').tmpl(data).appendTo('#products-container');
		},

	});
}
