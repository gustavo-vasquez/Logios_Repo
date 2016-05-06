﻿var solutionEditor;

window.onload = function () {
    solutionEditor = com.wiris.jsEditor.JsEditor.newInstance({ 'language': 'es' });    
    solutionEditor.insertInto(document.getElementById('solutionContainer'));
}

function ViewDevelopment(UserId, ExerciseId) {
	if ($('#DevelopmentField').is(':hidden'))
	{
		$('#DevelopmentField').slideDown(500);
	}
	else
    {
	    $('#DevelopmentField').slideUp(500);
	}
	console.log(UserId);
	console.log(ExerciseId);
		
}

function copyAnswer() {
    $('#answer').val(solutionEditor.getMathML());
    var isValid = ($('#answer').val() != "<math xmlns=\"http://www.w3.org/1998/Math/MathML\"/>");
    if (isValid)
        return true;    
    else
        alert("ERROR: Debe escribir una respuesta.");

    return false;
}

MathJax.Hub.Config({        
    jax: ["input/MathML", "output/SVG"],
    SVG: {
        scale: 100,
        font: "Gyre-Pagella",
        undefinedFamily: "STIXGeneral, 'Arial Unicode MS', serif",
        matchFontHeight: true,
        useFontCache: true
    }
});

