﻿var parsedTopics = [];

$(function () {
    //////////// SEARCH SECTION ////////////////////////////
    $.ajax({
        url: '/Home/GetTopics',
        method: 'GET',
        success: configureAutocomplete
    });

    function configureAutocomplete(rawTopics) {
        parsedTopics = $.map(rawTopics, function (topic, index) {
                            return {
                                label: topic.Description,
                                id: topic.TopicId
                            };
                        });

        $('#searchInput').autocomplete({
            source: parsedTopics,
            autoFocus: true,
            select: function (event, ui) {
                var selectedTopic = ui.item;
                $('input[name="topicId"]').val(selectedTopic.id);
            }
        });
    }

    generateSearchTags();
    // SEARCH SECTION ////////////////////////////
});

function searchExercise(userId) {
    // Guardarme los dos campos del formulario
    var topicId = parseInt($('input[name="topicId"]').val());
    var lastTopicIdInput = $('input#lastTopicId');
    var lastTopicId = parseInt($('input#lastTopicId').val());
    var topicDescription = $('input#searchInput').val();

    // Sacar el registro de busquedas del usuario de local storage
    var searches = storage.get(userId);

    // Si quere buscar lo mismo dos veces seguidas, no hago nada
    if (lastTopicId === topicId) {
        return;
    }

    // Si llegó hasta acá es porque tengo que mandar el fomulario
    var exerciseSearchForm = $('#exerciseSearchForm');

    // Actualizar el valor de la ultima busqueda realizada
    lastTopicIdInput.val(topicId);

    // Sumarle uno a la lista de busquedas realizadas por el usuario, validar si es la primera vez que busca eso
    addOneToSearchCount(searches, topicDescription, topicId);

    // Ordernar el array y actualizar el local storage
    searches.sort(function (search1, search2) {
        return search1.searchCount < search2.searchCount
    });
    storage.set(userId, searches);

    // Mandar el formulario
    exerciseSearchForm.submit();
}

function generateSearchTags() {
    var topSearches = getTopThreeMostSearchedTopics();
    var labelStart = '<span class="label label-default search-tag" ';
    var labelEnd = '</span>';

    topSearches.forEach(function (search) {
        var action = 'onclick=searchFromTags(\"' + search.description + '\")>';
        var newLabel = labelStart + action + search.description + labelEnd;
        $('#search-tags').append(newLabel);
    });
}

function getTopThreeMostSearchedTopics() {
    var searches = storage.get(userIdentity);
    
    // Devolver solo con los 3 que mas busquedas tienen
    return searches.slice(0, 3);
}

function searchFromTags(topicDescription) {
    var topic = getTopicByDescription(topicDescription);

    $('input[name="topicId"]').val(topic.id);
    $('input#searchInput').val(topic.description);

    searchExercise(userIdentity);
}

function getTopicByDescription(topicDescription) {
    var searches = storage.get(userIdentity);

    var theSearch = searches.find(function (search) {
        return search.description === topicDescription;
    });

    return theSearch;
}

function addOneToSearchCount(searches, topicDescription, topicId) {
    var found = false;

    for (var i = 0; i < searches.length; i++) {
        if (searches[i].description === topicDescription) {
            searches[i].searchCount++;
            found = true;
            break;
        }
    }

    if (found === false) {
        searches.push({
            description: topicDescription,
            id: topicId,
            searchCount: 1
        });
    }
}

function animateResults() {
    // Animar la seccion de resultados
    $('#exerciseSearchResult').hide();
    $('#exerciseSearchResult').slideDown('slow');

    // Borrar las tags y volver a generarlas por si una pasa a tener mas busquedas
    $('#search-tags').empty();
    generateSearchTags();
}