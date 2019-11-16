const WebSocket = require('ws')

// create new websocket server
const wss = new WebSocket.Server({ port: 8000 })

// empty object to store all players
var hostData = ""
var host = null
var clientData = []

var players = {}
// on new client connect
wss.on('connection', function connection(client, req) {
    // on new message recieved
    console.log("IP connected:" + req.connection.remoteAddress);
    client.on('message', function incoming(data) {
        // get data from string
        // store data to players object        
        var udid = (JSON.parse(data)).sender;
        
        players[udid] = data        
        // save player udid to the client        
        client.udid = udid

        if (udid == 0) {
            host = client
            broadcastUpdateClients()
        }
        else
            broadcastUpdateServer()
    })
})

wss.on('error', () => console.log('error'));

function broadcastUpdateClients() {
    // broadcast messages to all clients
    wss.clients.forEach(function each(client) {
        // filter disconnected clients
        if (client.readyState !== WebSocket.OPEN) return        
        if (client.udid != 0) {
            client.send(players[0])
        }            
    })
}

function broadcastUpdateServer() {    
    wss.clients.forEach(function each(client) {        
        if (client.readyState !== WebSocket.OPEN) return              
        if (client.udid != 0 && host != null) {            
            host.send(players[client.udid])            
        }        
    })
}

// call broadcastUpdate every 0.1s
//setInterval(broadcastUpdateClients, 17)
