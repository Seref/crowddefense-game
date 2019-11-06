from websocket_server import WebsocketServer
import json, time, threading


global serverData_lock, serverData
def new_client(client, server):    
    print("New client connected and was given id %d" % client['id'])    
    #server.send_message_to_all("Hey all, a new client has joined us")


def client_left(client, server):
    print("Client(%d) disconnected" % client['id'])    



def message_received(client, server, message):    
    try:                
        serverData_lock.acquire()        
        print(serverData)
    except:
        print("Error while handling message: "+message)
    finally:
        serverData_lock.release()        


PORT=8000

serverData_lock = threading.Lock()
serverData = "data"

server = WebsocketServer(PORT)
server.set_fn_new_client(new_client)
server.set_fn_client_left(client_left)
server.set_fn_message_received(message_received)

def update():
    while True:        
        try:
            serverData_lock.acquire()    
            server.send_message_to_all(serverData)        
        finally:
            serverData_lock.release()        
        
threading.Thread(target=update).start()

server.run_forever()


