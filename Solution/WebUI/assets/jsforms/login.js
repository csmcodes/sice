$(document).ready(function () {
    ClearStorage();
     LoadStatics();
    $("#btnlogin").on("click", Login);


    $('.live-tile').each(function () {
        $(this).liveTile("destroy", true); /* To get new size if resize event */
        tile_height = $(this).data("height") ? $(this).data("height") : $(this).find('.panel-body').height() + 52;
        $(this).height(tile_height);
        $(this).liveTile({
            speed: $(this).data("speed") ? $(this).data("speed") : 500, // Start after load or not
            mode: $(this).data("animation-easing") ? $(this).data("animation-easing") : 'carousel', // Animation type: carousel, slide, fade, flip, none
            playOnHover: $(this).data("play-hover") ? $(this).data("play-hover") : false, // Play live tile on hover
            repeatCount: $(this).data("repeat-count") ? $(this).data("repeat-count") : -1, // Repeat or not (-1 is infinite
            delay: $(this).data("delay") ? $(this).data("delay") : 0, // Time between two animations
            startNow: $(this).data("start-now") ? $(this).data("start-now") : true, //Start after load or not
        });
    });


   

});




function Login() {

    var obj = {};
    obj["email"] = $(".user").val();
    obj["password"] = $(".password").val();
    var jsonText = JSON.stringify({ objeto: obj });
    CallServerMethods(webservice + "Login", jsonText, 0);
}

function ServerResult(data, retorno) {
    if (data != "") {
        if (retorno == 0) {
            if (data.d == "OK") {
                SetOnlineUser($(".user").val(), GetBooleanCheckValue($("#chkmantener")));
                window.location.href = indexpage;
            }
            else if (data.d == "DESHABILITADA")
            {
                var mensaje = "Su cuenta ha sido deshabilitada, debe confirmar el pago de su juego a cristhian_sm@hotmail.com <br> " +
                            "<b>Datos depósito</b><br> " +
                            "<ul><li>VALOR: $10</li> " +
                            "<li>CTA AHORROS #2201144273</li> " +
                            "<li>BANCO DEL PICHINCHA</li> " +
                            "<li>Cristhian Sanmartín</li> " +
                            "<li>C.I.:0103567665</li> " +
                            "</ul> " +
                            "<br>" +
                            "Una vez confirmado el pago podrá seguir participando";
                
                MessageFull("error", mensaje, "center", "top", false, 5000);
            }
            else
                Message("error", data.d, "center", "top", false);
            
        }
    }
}