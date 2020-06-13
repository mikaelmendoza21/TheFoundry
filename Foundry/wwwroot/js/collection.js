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
                    html += "<li><a href=\"/collection/copiesInCollection?metacardId=" + value.id + "\">" + value.name + "</a></li>";
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
    if (typeof(currentPage) != "undefined" && currentPage != null && currentPage == 'GetCardCopiesInCollection') {
        var metacardId = $('#metacardId').val();
        $.ajax({
            type: "GET",
            dataType: "json",
            url: "/api/collection/getCardConstructsByMetacardId?metacardId=" + metacardId,
            success: function (data) {
                if (data != null && data.length > 0) {
                    // Assumes cardCopies come sorted by MtgCardId
                    // Add MtgCards with found copies
                    $("#cardCopiesInCollection").prepend("<div class='row'><h3>" + data.length + " total copies found</h3></div>");

                    var currentMtgCardId = data[0].mtgCardId;
                    var currentMtgCardCount = 0;
                    var $currentCardGroup = $(".mtgCardGroup[data-mtgcard-id='" + currentMtgCardId + "']");
                    $currentCardGroup.prepend("<hr />");

                    for (var i = 0; i < data.length; i++) {
                        var value = data[i];
                        if (currentMtgCardId != value.mtgCardId) {
                            currentMtgCardId = data[i].mtgCardId;
                            currentMtgCardCount = 0;
                            $currentCardGroup = $(".mtgCardGroup[data-mtgcard-id='" + currentMtgCardId + "']");
                        }
                        $currentCardGroup.find(".copiesContainer").append("<li>" + value.id + "</li>");
                        currentMtgCardCount++;

                        if (i == (data.length - 1) || data[i + 1].mtgCardId != currentMtgCardId) {
                            // Finishing up card group (if last card or next card a different MtgCard)

                            // Show image
                            var imageUrl = $currentCardGroup.attr("data-image-url");
                            $currentCardGroup.find(".mtgImage").html("<img src='" + imageUrl + "' />");
                            $currentCardGroup.find(".mtgCardCount").html("<h4><i>" + currentMtgCardCount + " copies </i></h4>");

                            // Temp
                            $currentCardGroup.find(".copiesContainer").addClass("hidden");
                            //

                            $currentCardGroup.removeClass("hidden");
                        }
                    }
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