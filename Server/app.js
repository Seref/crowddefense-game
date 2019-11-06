const WebSocket = require('ws')

// create new websocket server
const wss = new WebSocket.Server({ port: 8000 })

// empty object to store all players
var serverData = ""

// on new client connect
wss.on('connection', function connection(client, req) {
    // on new message recieved
    console.log("IP connected:" + req.connection.remoteAddress);
    client.on('message', function incoming(data) {
        // get data from string
        // store data to players object        
        serverData = data
        // save player udid to the client
        client.udid = req.connection.remoteAddress
    })
})

wss.on('error', () => console.log('error'));

function broadcastUpdate() {
    // broadcast messages to all clients
    wss.clients.forEach(function each(client) {
        // filter disconnected clients
        if (client.readyState !== WebSocket.OPEN) return        
        client.send(serverData)
    })
}

// call broadcastUpdate every 0.1s
setInterval(broadcastUpdate, 20)
