// Searching collection

$('#searchCardInCollection').click(function () {
    searchMetacardInCollection('searchMetacardInCollectionResults');
});

function searchMetacardInCollection(resultsContainerId) {
    var cardNameStartingWith = $('#cardNameInCollection').val();

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/api/metacard/byNameStart?substring=" + cardNameStartingWith,
        success: function (data) {
            if (data != null && data.length > 0) {
                var html = "";
                $.each(data, function (key, value) {
                    var cardName = encodeURIComponent(value.name);
                    html += "<div><a href=\"/collection/copiesInCollection?metacardId=" + value.id + "\">" + value.name + "</a></div>";
                });
                $("#searchMetacardInCollectionResults").html("<div>" + html + "</div>");
            }
            else {
                $("#searchMetacardInCollectionResults").text("No results found");
            }
        },
        error: function (xhr, status) {
            console.log(status);
        }
    });
}

$(document).ready(function () {
    if (currentPage != null && currentPage == 'GetCardCopiesInCollection') {
        var metacardId = $('#metacardId').val();
        $.ajax({
            type: "GET",
            dataType: "json",
            url: "/api/collection/getCardConstructsByMetacardId?metacardId=" + metacardId,
            success: function (data) {
                if (data != null && data.length > 0) {
                    var html = "<div><h4>Copies found: " + data.length + "</h4></div>";
                    $.each(data, function (key, value) {
                        var cardName = encodeURIComponent(value.name);
                        // TODO: Expand here to group card by set and individually edit/delete
                        html += "";
                    });
                    $("#cardCopiesInCollection").html("<div>" + html + "</div>");
                }
                else {
                    $("#cardCopiesInCollection").html("<h4>Card not found in your collection</h4>");
                }
            },
            error: function (xhr, status) {
                console.log(status);
            }
        });
    }
});