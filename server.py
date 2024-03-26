import socket
import threading
import colorama
from  colorama import Fore , Style
import os
from time import sleep
from scapy.all import *


#ESTA CAMADA SÓ VAI ENTRAR NO PACOTE QUANDO FOR UM PACOTE PARA A REDE
#OS PACOTES PARA O MESH LOCAL PERMANECERÃO USANDO MENSAGENS DE TEXTO COMUNS TCP
class Mesh(Packet):
    """
    Represents a mesh packet.

    Attributes:
        set_id (int): Flag for setting the ID for the Mesh.
        id (int): The ID for the packet.
        master_search (int): Flag for master search.
        master_search_response (int): Flag for master search response.
        coronation (int): Flag for coronation.
        coronation_response (int): Flag for coronation response.
        plea (int): Flag for plea.
        dbsync (int): Flag for dbsync.
        device_not_found (int): Flag for device not found.
        bestow (int): Flag for bestow.
        command_string (str): The command string for the packet.
    """
    
    name = "Mesh"
    fields_desc = [
  

      #### FLAGS MENSAGENS ####
      BitField("master_search", 0, 1),
      BitField("master_search_response", 0, 1),
      BitField("coronation", 0, 1),
      BitField("coronation_response", 0, 1),
      BitField("plea", 0, 1),
      BitField("dbsync", 0, 1),
      BitField("device_not_found", 0, 1),
      BitField("bestow", 0, 1),
      #### 1 BYTE ####
      StrField("command_string", ""),

    ]


class MeshProtocol:
    def __init__(self):
        #strings
        self.PLEA = "plea"
        self.CORONATION_RESPONSE = "coronation_response"
        self.CORONATION_PACKET = "coronation_packet"
        self.MASTER_SEARCH_RESPONSE = "master_search_response"
        self.SLAVE = "slave"
        self.MASTER_SEARCH = "master_search"
        self.MASTER = "master"

        #comandos internos disponíveis
        self.make_myself_master = "make_myself_master"
        self.make_myself_slave = "make_myself_slave"
        self.bestow = "bestow"
        self.shutdown = "shutdown"
        self.dbsync = "dbsync"
        self.device_not_found = "device_not_found"
        self.group_msg = "group_msg"
        
        #endereços e portas
        self.IHOST = "127.0.0.1"
        self.EHOST = "0.0.0.0"
        self.IPORT = 4444
        self.EPORT = 31415
        self.BPORT = 31416
        self.BADDR = ""
        
        #lista de mesh 
        self.mesh_running = set()
        self.mesh_ids = set()

        # ID e cargo
        self.master = False
        self.master_id = ""
        self.id = ""
        

        #LIMPA A TELA
        os.system('cls')

        colorama.init()

        

        #IMPEDE QUE PACOTES BROADCAST VERIFIQUEM SE CADA IP ESTÁ ATIVO ANTES DE ENVIAR PACOTE
        conf.checkIPaddr = False

    
    def getMeshId(self, ip):
        """
        Get the mesh ID from an IP address.

        Args:
            ip (str): The IP address.

        Returns:
            str: The mesh ID extracted from the IP address.
        """
        return ip.split('.')[-1]
    
    def setBroadcastAddress(self):
        """
        Get the broadcast address.

        Returns:
            str: The broadcast address.
        """
        
        # pega os 3 primeiros bytes do meu id e coloca '255' no final
        id_split = self.id.split('.')
        id_split[-1] = '255'
        self.BADDR = '.'.join(id_split)

    
    def setId(self):
        """
        Sets the ID of the object.

        Args:
            id (optional): The ID to be set. If not provided, the ID will be set to None.
            master (bool, optional): Whether the object is a master. Defaults to False.

        Returns:
            None
        """
        ips = socket.gethostbyname_ex(socket.gethostname())[2]
        
        if len(ips) > 1:
            print("""
MAIS DE UMA INTERFACE DE REDE DETECTADA. 
Seus IPs:
                  """)
            for i, ip in enumerate(ips):
                print(f"{i}. " + ip)

            option = input("SELECIONE O IP QUE DESEJA USAR: ")
            self.id = ips[int(option)]
        else:
            self.id = ips[0]

        #self.id = id if master == False else socket.gethostbyname(socket.gethostname())
        self.sendCommand("my_id," + self.getMeshId(self.id))
        self.mesh_running.add(self.id)




    def bindListeners(self):
        """
        Binds the listeners for the UDP and TCP connections.

        This function binds the UDP and TCP listeners to the specified IP addresses and ports.
        It also attempts to connect to the local mesh TCP.

        Parameters:
            None

        Returns:
            None
        """
        
        #self.id = socket.gethostbyname(socket.gethostname())
        listener_udp.bind((self.EHOST, self.BPORT))
        listener_tcp.bind((self.EHOST, self.EPORT))
        try:
            local_mesh_tcp.connect((self.IHOST, self.IPORT))
        except:
            print(Fore.RED + "O MESH ESTÁ DESATIVADO" + Style.RESET_ALL)
            exit()
        print(Fore.CYAN + "SERVIDOR INICIADO." + Style.RESET_ALL) 
        #self.sendCommand("my_id," + self.id.split(".")[-1])
    

    # PROCURA O REI (OU VERIFICA SE DETERMINADO HOST É O REI)
    def searchKing(self):   
        """
        Search for the master in the network.

        This function sends a packet to search for the master in the network.
        It creates a `Mesh` object with the `master_search` attribute set to 1.
        The packet is then sent using the `sender_udp` socket to the broadcast address
        '192.168.0.255' and the BPORT port.

        Args:
            None
        
        Returns:
            None
        
        """
        #PROCURA O REI NA REDE
        pkt = Mesh(master_search=1)
        sender_udp.sendto(raw(pkt), (self.BADDR, self.BPORT))
        

    #COMUNICA TODOS OS MESH NA REDE QUE EU SOU O REI AGORA
    def coronation(self):
        """
        coronation function is responsible for crowning the current instance as the master of the Mesh network. 
        It sets the 'master' attribute to True, assigns the 'id' attribute to the 'master_id' attribute, 
        and sends a command to the internal mesh to unlock the 'devices' session by using the 'sendCommand' method. 
        It then finds slaves, sends coronation packets, and sends the packets to all devices using the UDP socket.
        """
        self.master = True
        self.master_id = self.id
        self.sendCommand(f"role,{self.MASTER}") # ENVIA O CARGO PARA O MESH INTERNO (DESBLOQUEIA A SESSÃO DEVICES)
       
        #MANDANDO PACOTES DE COROAÇÃO
        print(Fore.CYAN + "ENCONTRANDO SLAVES..." + Style.RESET_ALL)
        
        #ENVIANDO PACOTES DE COROAÇÃO
        pkt = Mesh(coronation=1)
        sender_udp.sendto(raw(pkt), (self.BADDR, self.BPORT))
        
    def broadcastHandler(self, data, addr):
        """
        Handles the incoming broadcast packets.

        Args:
            data (str): The data payload of the packet.
            addr (tuple): The address of the sender.

        Returns:
            None
        """
        
        #SE O PACOTE TIVER UM PAYLOAD E NÃO FOR MEU
        if data != "" and addr[0] not in socket.gethostbyname_ex(socket.gethostname())[2]:
            mesh_pkt = Mesh(data)
            #SE FOR UM PACOTE DE BUSCA DE REI
            if mesh_pkt.master_search == 1 and self.master:
                #ENVIA PACOTE PARA O SLAVE DIZENDO QUE SOU O REI, JUNTO COM O ID DELE
                pkt = Mesh(master_search_response=1, command_string=addr[0])
                self.sendMessage(pkt, addr[0])
                self.mesh_running.add(addr[0])
                self.mesh_ids.add(self.getMeshId(addr[0]))
                print(Fore.GREEN + f"MESH ATIVO: {addr[0]}" + Style.RESET_ALL)
                self.sendCommand(f"active_mesh,{self.getMeshId(addr[0])}")

                
                return
            
            #SE FOR UM PACOTE DE COROAÇÃO
            if mesh_pkt.coronation == 1: 
                pkt = Mesh(coronation_response=1)
                print(Fore.GREEN + f"REI RECEBIDO: {addr[0]}" + Style.RESET_ALL)
                self.master_id = addr[0]
                self.master = False
                self.sendMessage(pkt, addr[0])
                self.sendCommand("make_myself_slave")
                return
        print((data,addr))
    
    # ESCUTA POR PACOTES E DECIDE COMO VAI RESPONDER
    def hear(self, packet, addr):
        """
        Handles incoming packets and performs various actions based on the packet type and content.

        Parameters:
            packet (bytes): The incoming packet.
            addr (str): The address of the sender.

        Returns:
            None
        """
        
        #DECODIFICA O PACOTE
        mesh_layer = Mesh(packet)
        command = mesh_layer.command_string
        
       
        

        # SE FOR UM PACOTE DE SUPLICA (plea,command,room_id)
        if mesh_layer.plea == 1 and self.master and addr in self.mesh_running:
            #ENVIA A SUPLICA PARA O MEU MESH
            self.sendCommand(f"{command.decode()},{addr}")

        #RESPOSTA DE COROAÇÃO 
        if mesh_layer.coronation_response == 1:
            print(Fore.GREEN + f"MESH ATIVO: {addr}" + Style.RESET_ALL)
            self.mesh_ids.add(self.getMeshId(addr))
            self.mesh_running.add(addr)
            self.sendCommand(f"active_mesh,{self.getMeshId(addr)}")
                 

        #SE FOR UM PACOTE DE RESPOSTA DE BUSCA
        if mesh_layer.master_search_response == 1:
            print(Fore.GREEN + f"REI ENCONTRADO: {addr}" + Style.RESET_ALL)
            self.master_id = addr
            self.sendCommand("role," + self.SLAVE) 
            self.master = False
            

        #SE FOR UM PACOTE DE DBSYNC
        if mesh_layer.dbsync == 1 and addr == self.master_id:
            #PASSO 4
            self.sendCommand(command.decode())
        
        if mesh_layer.device_not_found == 1 and addr == self.master_id:
            self.sendCommand("device_not_found")

        if mesh_layer.bestow == 1 and addr == self.master_id:
            #ENVIA O BESTOW COM O TTS PARA O LISTENER
            self.sendCommand("bestow," + command.decode())
            

    #COMANDOS QUE VÊM DO MEU MESH
    def commandSelector(self, command):
        """
        Selects and executes a command based on the given command by the internal mesh.

        Parameters:
        - command (str): The command containing the command.

        Returns:
        - None

        Raises:
        - None
        """
        #DEBUG PRINT
        command = command.split(",")
        print(Fore.GREEN + f"command: {command}" + Style.RESET_ALL)
        
        if command[0] == self.group_msg:
            ip = command[1]
            port = command[2]
            message = command[3]
            self.sendMessage(message, ip, False, int(port))
       
        #TORNA-SE REI
        if command[0] == self.make_myself_master:
            self.coronation()

        #TORNA-SE SLAVEO
        elif command[0] == self.make_myself_slave:
            self.sendCommand(f"role,{self.SLAVE}")
            self.master = False
            self.searchKing()

        #ENVIA O PEDIDO PARA O MESH REI
        elif command[0] == self.PLEA:
            #if self.master:
            #    self.sendCommand(command)
            #    return
            pkt = Mesh(plea = 1, command_string = f"{','.join(command)}")
            self.sendMessage(pkt, self.master_id)
        
        #MANDA O COMANDO PARA O DEVICE - PACOTE BESTOW: "bestow, ip, port, command, asker, ai_response"
        elif command[0] == self.bestow and self.master:
            if self.master:
                ip = command[1]
                port = command[2]
                command_server = command[3]
                asker = command[4]
                ai_response = command[5]
                
                #ENVIA UM BESTOW PARA O ASKER COM A RESPOSTA TTS
                pkt = Mesh(bestow = 1, command_string = ai_response)
                self.sendMessage(pkt, asker)

                #ENVIA UM PACOTE PARA O DISPOSITIVO
                self.sendMessage(command_server, ip, False, int(port))
                


        #DESLIGA O SERVIDOR (A THREAD É DEAMON, ELA DESLIGA JUNTO COM A THREAD PRINCIPAL)
        elif command[0] == self.shutdown:
            print(Fore.RED + "SHUTTING DOWN" + Style.RESET_ALL)
            exit()
            
        #SINCRONIZA O BANCO DE DADOS COM OS SLAVEOS DA REDE
        elif command[0] == self.dbsync and self.master:
            print(Fore.RED + f"{','.join(command)}" + Style.RESET_ALL)
    
            for mesh in self.mesh_running:
                if mesh == self.master_id:
                    continue
                pkt = Mesh(dbsync = 1, command_string = f"{','.join(command)}")
                self.sendMessage(pkt, mesh)

        #SE O DEVICE NÃO TIVER SIDO ENCONTRADO
        elif command[0] == self.device_not_found and self.master:
            pkt = Mesh(device_not_found = 1)
            self.sendMessage(pkt, command[1])

    # COMUNICAÇÃO COM O MESH INTERNO
    def sendCommand(self, command):
        """
        Sends a command to the TCP stream to the internal mesh.

        Parameters:
            command (str): The command to be sent to the TCP stream.

        Returns:
            None
        """
        try:
            print(Fore.YELLOW + command + Style.RESET_ALL)
            sleep(0.2) # IMPEDE QUE OS PACOTES CONCATENEM NO STREAM TCP
            local_mesh_tcp.sendto(command.encode(), (self.IHOST, self.IPORT))
        except Exception as e:
            print(e)

    
    #ENVIA OS BYTES DO PACOTE MESH AO MESH DESTINO
    def sendMessage(self, data, host, mesh_pkt=True, port=31415):
        """
        Sends a message to a specified host using TCP/IP protocol.

        Parameters:
            - data (str): The message to be sent.
            - host (str): The IP address or hostname of the destination.
            - mesh_pkt (bool): Specifies if the message should be sent as a mesh packet. Defaults to True.
            - port (int): The port number to use for the connection. Defaults to 31415.

        Returns:
            None
        """
        
        
        if not mesh_pkt:
            data = data.encode()
        try:
            sender_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            #print(host,port)
            sender_tcp.connect((host, port))
            sleep(0.2)# IMPEDE QUE OS PACOTES CONCATENEM NO STREAM TCP
            sender_tcp.send(raw(data))
        except TimeoutError:
            if mesh_pkt:
                if not self.master:
                    print(Fore.RED + "O REI CAIU, ASSUMINDO O TRONO..." + Style.RESET_ALL)
                    self.coronation()
                else:
                    print(Fore.RED + f"MESH {host} ESTÁ OFFLINE" + Style.RESET_ALL)    
            else:
                print(Fore.RED + "O DISPOSITIVO NÃO ESTÁ ON-LINE" + Style.RESET_ALL)
        except Exception as e:
            print(Fore.RED + "ERRO DESCONHECIDO:")
            print(e)
            print(Style.RESET_ALL)

        finally:
            try:
                sender_tcp.close()
            except:
                print(Fore.RED + "NÃO FOI POSSÍVEL ENCERRAR A CONEXÃO" + Style.RESET_ALL)
                return


   

def socket_listener():
    while True: 
        listener_tcp.listen(5)  # Permite uma conexão pendente
        conn, addr = listener_tcp.accept()  # Aceita a conexão
        #if addr not in mesh.mesh_running:    # PROTEGE O MESH DE RECEBER PACOTES FORA DA REDE
         #   conn.close()
         #  continue   
        data = conn.recv(1024)  
        if data:
            mesh.commandSelector(data.decode()) if addr[0] == mesh.IHOST else mesh.hear(data,addr[0])

#ESCUTA PACOTES ENVIADOS PARA O BROADCAST NA PORTA 31416
def broadcast_listener():       
    while True:
        data,addr = listener_udp.recvfrom(1024)
        mesh.broadcastHandler(data,addr)    


if __name__ == "__main__":

    try:
        #ESPERA O LISTENER INICIAR
        sleep(1)   

        #SOCKETS
        listener_udp = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        sender_udp = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        sender_udp.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1) #PARA BROADCAST
        listener_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        local_mesh_tcp = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    
        #INICIALIZA O MESH
        mesh = MeshProtocol()
        mesh.bindListeners()
        mesh.setId()
        mesh.setBroadcastAddress()

        #ESCUTA NAS PORTAS
        print(Fore.CYAN + "ESCUTANDO" + Style.RESET_ALL)
        mesh.sendCommand("hear")
            
            
        #INICIALIZA AS THREADS
        t2 = threading.Thread(target=broadcast_listener, daemon=True)
        t2.start()
        socket_listener()
    except Exception as e:
        input(f"\n\n\n{e}")