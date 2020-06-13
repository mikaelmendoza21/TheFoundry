// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



// Searching
$('#searchCardByName').click(function () {
    var cardNameStartingWith = $('#cardName').val();

    $.ajax({
        type: "GET",
        dataType: "jsonp",
        url: "/api/metacard/byNameStart?substring=" + cardNameStartingWith,
        success: function (data) {
            $("#cardSearchResults").text(data);
        },
        error: function (xhr, status) {
            console.log(status);
            console.log(xhr.responseText);
        }
    });
});