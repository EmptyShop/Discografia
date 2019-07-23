/*validador para fechas dd/mm/yyyy*/
$(function () {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || parseDateDMY(value);
    };
    /*$.validator.addMethod(
        "date",
        function (value, element) {
            return this.optional(element) || parseDateDMY(value);
        },
        "Fecha no válida."
    );*/

    function parseDateDMY(dateString) {
        // revisar el patrón
        if (!/^\d{1,2}(\-|\/)\d{1,2}(\-|\/)(\d{4}|\d{2})$/.test(dateString))
            return false;

        // convertir los números a enteros
        var parts = dateString.indexOf('/') != -1 ? dateString.split('/') : dateString.split('-');
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[2], 10);

        // convertir año de 2 dígitos a 4 dígitos
        year = year < 50 ? 2000 + year : (year < 100 ? 1900 + year : year);

        // Revisar los rangos de año y mes
        if ((year < 1000) || (year > 3000) || (month == 0) || (month > 12))
            return false;

        var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

        // Ajustar para los años bisiestos
        if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0))
            monthLength[1] = 29;

        // Revisar el rango del día
        return day > 0 && day <= monthLength[month - 1];
    }
});

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

$.fn.sortPosicion = function () {
    var indice = 1;
    $("option", this).each(function () {
        var p = $(this).val().indexOf('-');
        $(this).val($(this).val().substring(0, p) + '-' + indice++);
    });
}

function moverItem(event) {
    var origen = event.data.origen;
    var destino = event.data.destino;

    $(origen + " option:selected").remove().appendTo(destino);
    $(destino).sortSelect();
}

function moverTrack(event) {
    var origen = event.data.origen;
    var destino = event.data.destino;

    //asignación de canción: value="id-posición"
    if ($(destino).attr('id') == "listaCancionesSeleccionadas") {
        var tracksDestino = $(destino + " option").length + 1;
        $(origen + " option:selected").each(function () {
            $(this).val($(this).val() + "-" + (tracksDestino++));
        });
    }
        //des-asignación de canción: value="id"
    else {
        $(origen + " option:selected").each(function () {
            var p = $(this).val().indexOf('-');
            $(this).val($(this).val().substring(0, p));
        });
    }

    //movimiento del elemento option
    $(origen + " option:selected").remove().appendTo(destino);

    //en des-asignación se reordena la lista destino y se vuelve a asignar la posición de los elementos del tracklist
    if ($(destino).attr('id') == "listaCanciones") {
        $(destino).sortSelect();

        $("#listaCancionesSeleccionadas").sortPosicion();
    }
}

/*mueve los elementos marcados de la lista de no seleccionados a la lista de seleccionados*/
$("#asignar").on("click", {
    origen: "#listaArtistas",
    destino: "#listaArtistasSeleccionados"
}, moverItem);

/*mueve los elementos marcados de la lista de seleccionados a la lista de no seleccionados*/
$("#desasignar").on("click", {
    origen: "#listaArtistasSeleccionados",
    destino: "#listaArtistas"
}, moverItem);

/*mueve los elementos marcados de la lista de no seleccionados a la lista de seleccionados*/
$("#asignarCancion").on("click", {
    origen: "#listaCanciones",
    destino: "#listaCancionesSeleccionadas"
}, moverTrack);

/*mueve los elementos marcados de la lista de seleccionados a la lista de no seleccionados*/
$("#desasignarCancion").on("click", {
    origen: "#listaCancionesSeleccionadas",
    destino: "#listaCanciones"
}, moverTrack);

/*mueve una canción una posición arriba en el tracklist*/
function arriba() {
    //es útil sólo si hay más de una canción
    if ($("#listaCancionesSeleccionadas option:selected").length > 0) {
        //dejamos seleccionada sólo una canción
        $("#listaCancionesSeleccionadas option:selected").prop('selected', false).first().prop('selected', 'selected');

        //la posición de la canción seleccionada
        var p = $("#listaCancionesSeleccionadas").prop('selectedIndex');
        //movemos la canción sólo si no es la primera de la lista
        if (p > 0) {
            //eliminamos la canción del select pero retenemos la info.
            var seleccionado = $("#listaCancionesSeleccionadas option:selected").remove();

            //insertamos la canción una posición arriba de donde estaba originalmente
            $("#listaCancionesSeleccionadas option").eq(p - 1).before($("<option></option>")
                .val(seleccionado.val()).text(seleccionado.text()).attr("title", seleccionado.text()));
            $("#listaCancionesSeleccionadas option").eq(p - 1).prop('selected', 'selected');

            //reordenamos el valor de la posición de los elementos del tracklist
            $("#listaCancionesSeleccionadas").sortPosicion();
        }
    }
}

$("#arriba").on("click", arriba);   //mover track con mouse
$("body").on("keyup", function () { //mover track con hot key
    if (event.altKey && event.keyCode == 85) {
        arriba();
    }
})

/*mueve una canción una posición abajo en el tracklist*/
function abajo() {
    //es útil sólo si hay más de una canción
    if ($("#listaCancionesSeleccionadas option:selected").length > 0) {
        //dejamos seleccionada sólo una canción
        $("#listaCancionesSeleccionadas option:selected").prop('selected', false).first().prop('selected', 'selected');

        //la posición de la canción seleccionada
        var p = $("#listaCancionesSeleccionadas").prop('selectedIndex');

        //movemos la canción sólo si no es la última de la lista
        if (p < $("#listaCancionesSeleccionadas option").length - 1) {
            //eliminamos la canción del select pero retenemos la info.
            var seleccionado = $("#listaCancionesSeleccionadas option:selected").remove();

            //insertamos la canción una posición abajo de donde estaba originalmente
            $("#listaCancionesSeleccionadas option").eq(p).after($("<option></option>")
                .val(seleccionado.val()).text(seleccionado.text()).attr("title", seleccionado.text()));

            $("#listaCancionesSeleccionadas option").eq(p + 1).prop('selected', 'selected');

            //reordenamos el valor de la posición de los elementos del tracklist
            $("#listaCancionesSeleccionadas").sortPosicion();
        }
    }
}

$("#abajo").on("click", abajo); //mover track con mouse
$("body").on('keyup', function () { //mover track con hot key
    if (event.altKey && event.keyCode == 87) {
        abajo();
    }
});

/*des-seleciona los elementos de la lista de artistas no seleccionados y la lista de canciones no seleccionadas,
selecciona todos los elementos de la lista de artistas seleccionados y la lista de canciones seleccionadas*/
$(":submit").on("click", function (event) {
    $("#listaArtistas option").prop("selected", false);
    $("#listaArtistasSeleccionados option").prop("selected", true);
    $("#listaCanciones option").prop("selected", false);
    $("#listaCancionesSeleccionadas option").prop("selected", true);
})

/*asigna tooltip a cada item de las listas de canciones*/
$("#listaCanciones option").each(function () {
    $(this).attr("title", $(this).text());
});

$("#listaCancionesSeleccionadas option").each(function () {
    $(this).attr("title", $(this).text());
});