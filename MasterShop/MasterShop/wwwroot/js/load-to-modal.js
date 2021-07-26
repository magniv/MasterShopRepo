$(document).ready(function () {
    $(".show-product").click(function () {
        var itemName = $(this).data("name");
        var itemImage = $(this).data("image");
        $('#productName').html(itemName);
        $("#productImage").attr("src", itemImage);
        $("#exampleModalToggle").modal();
    });
});