//SignalR bağlantısı oluştırma
var connection = new signalR.HubConnectionBuilder().withUrl("/session").build();

//SignalR bağlantısını başlatma
connection.start()
    .then(() => {
        console.log("Hub connection established.");
        var Token = document.cookie.split("JWTToken=")[1];
        var browser = navigator.userAgentData.brands[2].brand;
        connection.invoke("ConnectionCheck", Token, browser)
            .then(() => {console.log("Connection check invoked successfully.");})
            .catch((error) => {console.error("Error invoking ConnectionCheck:", error);});
    })
    .catch((error) => {console.error("Error establishing hub connection:", error);});

connection.on("SignOut", (hasExistSession) => {
    if (hasExistSession) {
        window.location.href = "https://localhost:44384/auth/logout";
    }
});