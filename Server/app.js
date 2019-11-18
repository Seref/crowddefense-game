const WebSocket = require('ws');

// create new websocket server
const wss = new WebSocket.Server({ port: 8000 });


var lobbies = {};

var currLobby = null; // Debug

//right now only with Lobby 0
// on new client connect
wss.on('connection', function connection(client, req) {
    console.log("IP connected:" + req.connection.remoteAddress);
    client.on('message', function incoming(data) {
        //parse json to an object
        var request = JSON.parse(data);
        if (request === undefined) {
            console.log("Error got: " + data + "\n");
            return;
        }
        if (request.lobby !== undefined) {
            //SERVER_COMMAND means a server request (create lobby, delete lobby, etc..)
            if (request.lobby === "SERVER_COMMAND") {
                switch (request.request) {
                    case 0: // Create Lobby                        
                        console.log("Creating Lobby");
                        lobbies[request.optional] =
                            {
                                HOST: {
                                    Client: client,
                                    Data: null
                                },
                                CLIENTS: {}
                            };
                        currLobby = lobbies[request.optional];
                        client.send("ok");
                        break;
                    case 1: // Join Lobby                         
                        console.log("Joining Lobby");

                        if (lobbies[request.optional] !== undefined && client.ID === undefined) {
                            if (lobbies[request.optional].CLIENTS[client.ID] === undefined) {
                                client.ID = Object.keys(lobbies[request.optional].CLIENTS).length + 1;
                                console.log("Joined as Player " + client.ID);
                                lobbies[request.optional].CLIENTS[client.ID] = { Client: client, client_data: null };
                            }
                            client.send("ok");
                        }
                        else {
                            client.send("error");
                        }
                        break;
                    default:
                        break;
                }
            }
            else if (lobbies[request.lobby] !== undefined) {
                var currentLobby = lobbies[request.lobby];
                //HOST Action
                if (currentLobby.HOST.Client === client) {
                    currentLobby.HOST.Data = data;
                    broadcastUpdateClients(currentLobby, data);
                }
                else {                    
                    if (client.ID !== undefined && currentLobby.CLIENTS[client.ID] !== undefined) {                        
                        var jsonData = JSON.parse(data);
                        jsonData.clientID = client.ID;                        
                        sendUpdateServer(currentLobby, JSON.stringify(jsonData));
                    }
                }
            }
        }
    })
})

wss.on('error', () => console.log('error'));

function broadcastUpdateClients(currentLobby, data) {
    if (currentLobby.CLIENTS !== []) {
        Object.values(currentLobby.CLIENTS).forEach(
            function each(client) {
                if (client.Client.readyState == WebSocket.OPEN) {
                    client.Client.send(data);
                }
            });
    }
}

function sendUpdateServer(currentLobby, data) {
    if (currentLobby.HOST.Client !== undefined && currentLobby.HOST.Client.readyState === WebSocket.OPEN) {        
        currentLobby.HOST.Client.send(data);
    }
}

function broadcastUpdateServer(currentLobby, data) {
    if (currentLobby.HOST.Client !== undefined && currentLobby.HOST.readyState === WebSocket.OPEN) {
        console.log("sending Data");
        currentLobby.HOST.Client.send(data);
    }
}

function printSize() {
    if (currLobby !== null && currLobby.HOST.Data !== null)
        console.log(currLobby.HOST.Data.length);
}
// call broadcastUpdate every 0.1s
//setInterval(printSize, 100)