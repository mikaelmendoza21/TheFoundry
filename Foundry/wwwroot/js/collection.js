// Searching collection

$('#searchCardInCollection').click(function () {
    searchMetacardInCollection('searchMetacardInCollectionResults');
});

function searchMetacardInCollection(resultsContainerId) {
    var cardNameStartingWith = encodeURIComponent($('#cardNameInCollection').val());

    $.ajax({
        type: "GET",
        dataType: "json",
        url: encodeURI("/api/metacard/byNameStart?substring=" + cardNameStartingWith),
        success: function (data) {
            if (data != null && data.length > 0) {
                var html = "";
                $.each(data, function (key, value) {
                    var cardName = encodeURIComponent(value.name);
                    html += "<li class='spaced'><div><a class='text-md' href=\"/collection/copiesInCollection?metacardId=" + value.id + "\">" + value.name + "</a></div></li>";
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
                    $("#cardCopiesInCollection").prepend("<div class='row spaced'><h3>" + data.length + " total copies found</h3></div>");

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
                        $currentCardGroup.find(".copiesContainer").append("<li class='row spaced'><div class='pad-right'>" + value.id + " - Deck: " + value.deckId + "   </div><input type='button' class='btn btn-secondary pad-left' data-cardConstruct-id='" + value.id +"' value='Delete'></input></li>");
                        currentMtgCardCount++;

                        if (i == (data.length - 1) || data[i + 1].mtgCardId != currentMtgCardId) {
                            // Finishing up card group (if last card or next card a different MtgCard)

                            // Show image
                            var imageUrl = $currentCardGroup.attr("data-image-url");
                            $currentCardGroup.find(".mtgImage").html("<img class='spaced' src='" + imageUrl + "' />");
                            $currentCardGroup.find(".mtgCardCount").html("<h4 class='spaced'><i>" + currentMtgCardCount + " copies </i></h4>");

                            // Temp
                            $currentCardGroup.find(".copiesContainer").addClass("hidden");
                            //

                            $currentCardGroup.removeClass("hidden");
                        }
                    }
                }
                else {
                    $("#cardCopiesInCollection").prepend("<div class='spaced'><p class='text-md'>Card not found in your collection</p></div>");
                }
            },
            error: function (xhr, status) {
                console.log(status);
            }
        });
    }
});