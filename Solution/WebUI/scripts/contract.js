var userstorageid = "usuariosice";
var companystorageid = "companysice";
var loginpage = "login.html";
var signuppage = "signup.html";
var indexpage = "listado.html";
var forgotpage = "olvido.html";
var resultadospage = "resultados.html";
var onlineuser;

IsOnline();

function IsOnline() {
    var pathname = window.location.pathname;
    if (pathname.indexOf(loginpage) < 0 && pathname.indexOf(signuppage) < 0 && pathname.indexOf(forgotpage) < 0 ) {
        var usr = GetOnlineUser();
        $(".username").html(usr);   
        if (usr == null)
            window.location.href = loginpage;
    }

}

function EndContract() {
    ClearStorage();
    window.location.href = indexpage;
}


///////////////////////////SIGNED OBJECTS/////////////////////////////////////////

function SetOnlineCompany(obj, persistent) {
    SetStorage(obj, companystorageid, persistent);
}


function GetOnlineCompany() {
    return GetStorage(companystorageid);
}


function SetOnlineUser(obj, persistent) {
    SetStorage(obj, userstorageid, persistent);
}


function GetOnlineUser() {
    return GetStorage(userstorageid);
}







