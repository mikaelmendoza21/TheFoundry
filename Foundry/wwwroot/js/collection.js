// Searching collection

$('#searchCardInCollection').on('click touchend submit', function () {
    searchMetacardInCollection('searchMetacardInCollectionResults');
});

function searchMetacardInCollection(resultsContainerId) {
    var cardNameStartingWith = encodeURIComponent($('#cardNameInCollection').val());
    var spinner = "<span id='search-spinner' class='spinner-border spinner-border-sm search-spinner' role='status' aria-hidden='true'></span>" +
        "<span class='sr-only search-spinner'>Loading...</span>";
    $('#searchCardInCollection').append(spinner);

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
                    html += "<li class='spaced'><div><a class='text-md' href=\"/collection/copiesInCollection?metacardId=" + value.id + "\">" + value.name + "</a></div></li>";
                });
                $("#searchMetacardInCollectionResults").html("<div>" + html + "</div>");
            }
            else {
                $("#searchMetacardInCollectionResults").text("No results found");
            }
        },
        error: function (xhr, status) {
            $('.search-spinner').remove();
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
                    if (data.length == 1) {
                        $("#cardCopiesInCollection").prepend("<div class='row spaced cardCopiesCount'><h3>1 copy found</h3></div>");
                    }
                    else {
                        $("#cardCopiesInCollection").prepend("<div class='row spaced cardCopiesCount'><h3>" + data.length + " total copies found</h3></div>");
                    }

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

                        var deckId = (value.deckId != "") ? value.deckId : "None";
                        $currentCardGroup.find(".copiesContainer")
                            .append("<li class='row spaced' data-construct-id='" + value.id + "'>" +
                                    "<span class='pad-right'>" +
                                        "<p>Card copy - Deck: " + deckId + "</p>" +
                                    "</span>" +
                                    "<span>" +
                                        "<input type='button' class='btn btn-secondary pad-left' onclick='deleteCardConstruct(\"" + value.id + "\")' data-cardConstruct-id='" + value.id + "' value='Delete' />" +
                                    "</span>" +
                                    "</li><hr />");
                        currentMtgCardCount++;

                        if (i == (data.length - 1) || data[i + 1].mtgCardId != currentMtgCardId) {
                            // Finishing up card group (if last card or next card a different MtgCard)

                            // Show image
                            var imageUrl = $currentCardGroup.attr("data-image-url");
                            $currentCardGroup.find(".mtgImage").html("<img class='spaced' src='" + imageUrl + "' />");
                            $currentCardGroup.find(".mtgCardCount").html("<h4 class='spaced cardCopiesCount'><i>" + currentMtgCardCount + " copies </i></h4>");

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

function deleteCardConstruct(id) {
    var $modal = $('#main-modal');
    $modal.find('.modal-title').text('Delete card?');
    $modal.find('.modal-body').text('This will remove this copy from your collection, including any Decks.');
    $modal.find('#main-modal-submit').on('click submit touchend', function () {
        $.ajax({
            type: "DELETE",
            dataType: "json",
            url: "/api/collection/deleteCardCopy?constructId=" + encodeURIComponent(id),
            success: function (data) {
                if (data != null && data.success == true) {
                    $('.cardCopiesCount').hide();
                    $("li[data-construct-id='" + id + "']").remove();
                    $('#main-modal').modal('hide');;
                }
            },
            error: function () {
                alert("An error occured.");
            }
        });
    });

    $('#main-modal').modal('show');
}