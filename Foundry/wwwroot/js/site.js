// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Searching
$('#searchCardByName').on('click touchend submit', searchMetacard);

function searchMetacard(resultsContainerId) {
    var cardNameStartingWith = encodeURIComponent($('#cardName').val());

    var spinner = "<span id='search-spinner' class='spinner-border spinner-border-sm search-spinner' role='status' aria-hidden='true'></span>" +
        "<span class='sr-only search-spinner'>Loading...</span>";
    $('#searchCardByName').append(spinner);

    $.ajax({
        type: "GET",
        dataType: "json",
        url: encodeURI("/api/metacard/byNameStart?substring=" + cardNameStartingWith),
        success: function (data) {
            $('.search-spinner').remove();
            if (data != null && data.length > 0) {
                var html = "";
                $.each(data, function (key, value) {
                    var cardName = encodeURIComponent(value.name);
                    html += "<li class='spaced'><div><a class='text-md' href=\"/selectSet?metacardId=" + value.id + "&cardName=" + cardName + "\">" + value.name + "</a></div></li>";
                });
                $("#cardSearchResults").html("<div class='spaced'>" + html + "</div>");
            }
            else {
                $("#cardSearchResults").text("No results found");
            }
        },
        error: function (xhr, status) {
            $('.search-spinner').remove();
            console.log(status);
        }
    });
}

$('#addCardToCollection').click(createCardCopies);

// Creating Card Constructs
function createCardCopies() {
    var mtgCardId = $('#mtgCardId').val();
    var numberOfCopies = $('#numberOfCopies').val();

    window.location.replace("/addCopies?mtgCardId=" + mtgCardId + "&numberOfCopies=" + numberOfCopies);
}