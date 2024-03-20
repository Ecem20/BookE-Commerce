//SignalR bağlantısı oluştırma
var connection = new signalR.HubConnectionBuilder().withUrl("/session").build();

//SignalR bağlantısını başlatma
connection.start()
    .then(() => {
        console.log("Hub connection established.");
        //tarayıcı bilgisi

        //Tarayıcıdaki JWT tokenini alır 
        var Token = document.cookie.split("JWTToken=")[1];
        var browser = navigator.userAgentData.brands[2].brand;
        //kullanıcının tarayıcı bilgilerini ve JWT tokenini sunucuya gönderir
        //ConnectionCheck metodu, sunucunun, kullanıcının tarayıcı bilgileri ve JWT tokeni ile oturumu izlemesini sağlar
        connection.invoke("ConnectionCheck", Token, browser)
            .then(() => {
                console.log("Connection check invoked successfully.");
            })
            .catch((error) => {
                console.error("Error invoking ConnectionCheck:", error);
            });
    })
    .catch((error) => {
        console.error("Error establishing hub connection:", error);
    });
//Sunucudan gelen "SignOut" mesajını dinler
connection.on("SignOut", (hasExistSession) => {
    if (hasExistSession) {
        //kullanıcıyı oturum sonlandırma sayfasına yönlendirir
        window.location.href = "https://localhost:44384/auth/logout";
    }
});