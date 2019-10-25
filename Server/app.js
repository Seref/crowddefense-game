const WebSocket = require('ws')

// create new websocket server
const wss = new WebSocket.Server({port: 8000})

// empty object to store all players
var players = {}

// on new client connect
wss.on('connection', function connection (client, req) {
  // on new message recieved
  console.log("IP connected:"+ req.connection.remoteAddress);
  client.on('message', function incoming (data) {	
    // get data from string
    var [udid, playerData] = data.toString().split('\t')	
    // store data to players object
    players[udid] = JSON.parse(playerData)
    players[udid].serverTimeStamp = Date.now()   	
    // save player udid to the client
    client.udid = udid
  })
})

function broadcastUpdate () {
  // broadcast messages to all clients
  wss.clients.forEach(function each (client) {
    // filter disconnected clients
    if (client.readyState !== WebSocket.OPEN) return
    // filter out current player by client.udid
    var otherPlayers = Object.keys(players).filter(udid => udid !== client.udid)
    // create array from the rest
    var otherPlayersPositions = otherPlayers.map(udid => players[udid])
    // send it
    client.send(JSON.stringify({players: otherPlayersPositions}))
  })
}

// call broadcastUpdate every 0.1s
setInterval(broadcastUpdate, 20)
