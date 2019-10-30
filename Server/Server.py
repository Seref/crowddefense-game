from websocket_server import WebsocketServer
import json, time, threading


global players_lock, bullets_lock, enemies_lock, enemies, players, bullets
def new_client(client, server):    
    print("New client connected and was given id %d" % client['id'])    
    #server.send_message_to_all("Hey all, a new client has joined us")


def client_left(client, server):
    print("Client(%d) disconnected" % client['id'])    



def message_received(client, server, message):
    global players_lock, bullets_lock, enemies_lock, enemies, players, bullets
    try:        
        mess = json.loads(message)
        
        uh = json.loads(mess["data"]) #temporary
        mess_id = uh["id"]
        
        if mess["type"] == 0:            
            try:                
                enemies_lock.acquire() 
                enemies[str(mess_id)] = ['{"type":0,'+mess["data"][1:]]                
            finally:
                enemies_lock.release()
        elif mess["type"] == 1:
            try:
                players_lock.acquire()
                players[str(mess_id)] = ['{"type":1,'+mess["data"][1:]]
            finally:
                players_lock.release()
        elif mess["type"] == 2:
            try:
                bullets_lock.acquire()
                bullets[str(mess_id)]= ['{"type":2,'+mess["data"][1:]]
            finally:
                bullets_lock.release()
                
    except:
        print("Error while handling message: "+message)    


PORT=8000

players_lock = threading.Lock()
bullets_lock = threading.Lock()
enemies_lock = threading.Lock()

players = {}
bullets = {}
enemies = {}

server = WebsocketServer(PORT)
server.set_fn_new_client(new_client)
server.set_fn_client_left(client_left)
server.set_fn_message_received(message_received)

def update():
    while True:        
        players_lock.acquire()        
        for i in list(players.values()):            
            server.send_message_to_all(i[0])
        players_lock.release()
        
        bullets_lock.acquire()        
        for i in list(bullets.values()):
            server.send_message_to_all(i[0])
        bullets_lock.release()

        enemies_lock.acquire()        
        for i in list(enemies.values()):            
            server.send_message_to_all(i[0])
        enemies_lock.release()
        
        time.sleep(0.20)
        
threading.Thread(target=update).start()

server.run_forever()


