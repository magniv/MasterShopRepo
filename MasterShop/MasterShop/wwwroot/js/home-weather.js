$(document).ready(function () {
    // Show Weather Loader
    $('#loader').show();

    $.ajax({
        url: "https://api.weatherapi.com/v1/current.json?key=b421e5f0b3394f4b88472532212407&q=Tel%20Aviv",
        dataType: "json",
        type: "GET",
        success: function (data) {
            $('#loader').hide();
            var weatherHtml = "";
            weatherHtml += `<img src="` + data?.current?.condition?.icon + `" />`;
            weatherHtml += `<div>The weather in Tel Aviv is ` + data?.current?.condition?.text + `</div>`;
            weatherHtml += `<div>The temprature is ` + data?.current?.temp_c + ` °C</div>`;
            $('#weather').html(weatherHtml);
        },
        error: function () {
            $('#loader').hide();
        }
    });
});