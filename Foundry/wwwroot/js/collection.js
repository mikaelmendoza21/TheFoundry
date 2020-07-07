// Searching collection

$('#searchCardInCollection').on('click touchend submit', function () {
    searchMetacardInCollection('searchMetacardInCollectionResults');
});

function searchMetacardInCollection(resultsContainerId) {
    var cardNameStartingWith = $('#cardNameInCollection').val();
    var spinner = "<span id='search-spinner' class='spinner-border spinner-border-sm search-spinner' role='status' aria-hidden='true'></span>" +
        "<span class='sr-only search-spinner'>Loading...</span>";
    $('#searchCardInCollection').append(spinner);

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/api/metacard/byNameStart?substring=" + encodeURIComponent(cardNameStartingWith),
        success: function (data) {
            $('.search-spinner').remove();
            if (data != null && data.length > 0) {
                var html = "";
                $.each(data, function (key, value) {
                    var cardName = encodeURIComponent(value.name);
                    html += "<li class='spaced'>" +
                        "<div>" +
                        "<a class='text-md' href=\"/collection/copiesInCollection?metacardId=" + value.id + "\">" + value.name + "</a>" +
                        "</div></li>";
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
        fetchCopiesInCollection();
    }
});

function fetchCopiesInCollection() {
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

                    var deckHtml = (value.deckId != null && value.deckId != "") ? "<span class='spaced'>Deck:" + value.deckId + "</span>" : "";
                    var notesHtml = (value.notes != null && value.notes != "") ? "<span class='spaced'><em>Notes:</em> " + value.notes + "</span>" : "";
                    var isFoil = (value.isFoil != null && value.isFoil) ? true : false;
                    var isFoilHtml = "";
                    if (isFoil) {
                        isFoilHtml = "<div class='form-check spaced'>" +
                            "<input type='checkbox' id='is-foil-" + value.id + "' onchange='toggleIsFoil(\"" + value.id + "\")' data-card-construct-id='" + value.id + "'  type='checkbox' class='form-check-input is-foil-check' checked='checked'/>" +
                            "<label class='form-check-label' for='is-foil-" + value.id + "' >Is Foil</label>" +
                            "</div>";
                    }
                    else {
                        isFoilHtml = "<div class='form-check spaced'>" +
                            "<input type='checkbox' id='is-foil-" + value.id + "' onchange='toggleIsFoil(\"" + value.id + "\")' data-card-construct-id='" + value.id + "'  type='checkbox' class='form-check-input is-foil-check' />" +
                            "<label class='form-check-label' for='is-foil-" + value.id + "' >Is Foil</label>" +
                            "</div>";
                    }

                    $currentCardGroup.find(".copiesContainer")
                        .append("<li class='row spaced form' data-construct-id='" + value.id + "'>" +
                            "<div class='form-group'>" +
                            "<p><em>Card</em></p>" +
                            deckHtml +
                            notesHtml +
                            isFoilHtml +
                            "<input type='button' class='btn btn-secondary spaced form-control' onclick='deleteCardConstruct(\"" + value.id + "\")' data-cardConstruct-id='" + value.id + "' value='Delete' />" +
                            "</div>" +
                            "</li><hr />");
                    currentMtgCardCount++;

                    if (i == (data.length - 1) || data[i + 1].mtgCardId != currentMtgCardId) {
                        // Finishing up card group (if last card or next card a different MtgCard)

                        // Show image
                        var imageUrl = $currentCardGroup.attr("data-image-url");
                        $currentCardGroup.find(".mtgImage").html("<img class='spaced' src='" + imageUrl + "' />");
                        if (currentMtgCardCount == 1)
                            $currentCardGroup.find(".mtgCardCount").html("<h4 class='spaced cardCopiesCount'><i>" + currentMtgCardCount + " copy from Set</i></h4>");
                        else
                            $currentCardGroup.find(".mtgCardCount").html("<h4 class='spaced cardCopiesCount'><i>" + currentMtgCardCount + " copies from Set</i></h4>");

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

function toggleIsFoil(cardConstructId) {
    var $checkbox = $(".is-foil-check[data-card-construct-id='" + cardConstructId + "']");
    var isFoil = $checkbox.is(":checked");
    if (isFoil) {
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "/api/collection/markAsFoil?constructId=" + encodeURIComponent(cardConstructId),
            error: function () {
                alert("An error occured.");
            }
        });
    }
    else {
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "/api/collection/unmarkAsFoil?constructId=" + encodeURIComponent(cardConstructId),
            error: function () {
                alert("An error occured.");
            }
        });
    }
}