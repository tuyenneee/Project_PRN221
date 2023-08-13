
var connection = new signalR.HubConnectionBuilder().withUrl("/MessageHub").build();

connection.on("ReloadDocuments", function () {
    location.reload();
});

connection.start().then(
).catch(
    function (err) {
        return console.error(err.toString());
    }
);



