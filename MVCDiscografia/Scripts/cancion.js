/*ordena alfabéticamente campos de tipo select, basándose en la propiedad text*/
$.fn.sortSelect = function () {
    var opciones = $('#' + this.attr('id') + ' option');

    opciones.sort(function (a, b) {
        if (a.text > b.text) return 1;
        else if (a.text < b.text) return -1;
        else return 0;
    });

    this.empty().append(opciones);

    $('#' + this.attr('id') + ' option').attr('selected', false);
}

function moverItem(event) {
    var origen = event.data.origen;
    var destino = event.data.destino;
    
    $(origen + " option:selected").remove().appendTo(destino);
    $(destino).sortSelect();
}
/*mueve los elementos marcados de la lista de no seleccionados a la lista de seleccionados*/
$("#asignar").on("click", { origen: "#ListaArtistas", destino: "#ArtistasSeleccionados" }, moverItem);

/*mueve los elementos marcados de la lista de seleccionados a la lista de no seleccionados*/
$("#desasignar").on("click", { origen: "#ArtistasSeleccionados", destino: "#ListaArtistas" }, moverItem);

/*des-seleciona los elementos de la lista de artistas no seleccionados,
selecciona todos los elementos de la lista de artistas seleccionados,
da formato al campo duración*/
$(":submit").on("click", function (event) {
    $("#ListaArtistas option").prop("selected", false);
    $("#ArtistasSeleccionados option").prop("selected", true);
    if ($("#Cancion_Duracion").val().length != 0 && $("#Cancion_Duracion").val().length <= 5) {
        $("#Cancion_Duracion").val("00:" + $("#Cancion_Duracion").val());
    }
})