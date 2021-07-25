function initMap() {
    geocoder = new google.maps.Geocoder();
    var mapOptions = {
        zoom: 8,
        center: { lat: 32.08284, lng: 34.7855 },
        mapTypeControl: true,
        navigationControl: true,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    var map = new google.maps.Map(document.getElementById("map_element"), mapOptions);
    var address = $('#address').attr("data-address");
    if (geocoder) {
        geocoder.geocode({
            'address': address
        }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                if (status != google.maps.GeocoderStatus.ZERO_RESULTS) {
                    map.setCenter(results[0].geometry.location);

                    var infowindow = new google.maps.InfoWindow({
                        content: '<b>The order address: ' + address + '</b>',
                        size: new google.maps.Size(350, 100)
                    });

                    var marker = new google.maps.Marker({
                        position: results[0].geometry.location,
                        map: map,
                        title: address
                    });
                    google.maps.event.addListener(marker, 'click', function () {
                        infowindow.open(map, marker);
                    });

                } else {
                    //alert("No results found");
                }
            } else {
                // alert("Geocode was not successful for the following reason: " + status);
            }
        });
    }
}
