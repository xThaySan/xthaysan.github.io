from socket import socket
from multiprocessing import Process
from Crypto.Cipher import AES
from os import urandom
from hashlib import md5
from flag import flag


troll = b"""
::::::::::-------=----------*---:----------       
:::::::::::-:----=--------:--::::::::::::--       
.::::::::::::-------:::::::-::::::::::::-=-       
.::::::::::::::----:::::::::::::::::--=====       
.::::::::::::::----:-------+------------==-       
++=:::::::::-===+=:::------%-:.:-+-------=-       
=++#*+::::==+*+-::-..::::-:@===+++-------=:       
:::::=+**++@@%%%*::-:.::-+=#:--------=-==-:       
:::::::=*+#%@@%%##+--:::#@%*....::--====--:       
*+=---=*#*#####*++++----=#%*:-:---========-       
:------%######*+++---------:-::==-====+=+==       
=======%######+=------------::=:-::=--:::.+       
+++=++=+=*#%##*=----------------:=-::::-==-       
+++++++*++*#*+====-----------------=-==---:       
+++****##+***#+====----====-------==+===:::       
+++*++*#%*++***++===========---=-----==:=-:       
+++***#%%#++**++*++++++========+*+=----=:::       
****###%%****#*+*##*+++====+=+***#*+==++-=.       
***##%#%%*********###*+++++=**#########***=       
####%%%%%***********#%#******%@%%%%%%%#%####*     
###%%%%%%**#*#*#####+++%#***#%@%%%%%%%###******   
#%%@%%%%#**########**++=%%*##%@@@@@%%%%###******% 
%%%@%%%%%###%##%*++++++==+%#*#%@@@%%%%%%#******** 
                             #*%%%%%@@%%%#*****+# 
                                +%%#%%%@@@#****   
                                    ###%%%#**     
"""


def menu(c):
	c.send(b"\n")
	options = ["Encrypt", "Verify", "Flag"]
	for i, option in enumerate(options):
		c.send((f"{i+1} - {option}").encode()+b'\n')
	c.send(b"> ")

def rec(c):
	return c.recv(1024).decode().strip()

def encrypt(c, key, plaintext):
	iv = md5(plaintext).digest()
	associated_data = urandom(16)
	aes = AES.new(key, AES.MODE_GCM, iv)
	aes.update(associated_data)
	ciphertext, tag = aes.encrypt_and_digest(plaintext)
	return {
		"ciphertext": ciphertext.hex(),
		"associated_data": associated_data.hex(),
		"iv": iv.hex(),
		"tag": tag.hex()
	}

def verify(c, key, ciphertext, associated_data, iv, tag):
	try:
		aes = AES.new(key, AES.MODE_GCM, iv)
		aes.update(associated_data)
		aes.decrypt_and_verify(ciphertext, tag)
		return flag
	except ValueError as e:
		return str(e)

def challenge(c):
	c.send(b"Welcome to my AES-GCM service !\nAll values must be sent in hexadecimal.\n")
	key = urandom(16)
	while True:
		menu(c)
		choice = rec(c)
		if choice == "1":
			c.send(b"plaintext : ")
			plaintext = bytes.fromhex(rec(c))
			if b"raccoon" in plaintext:
				return b"you are not authenticated"
			output = str(encrypt(c, key, plaintext)).encode()
			c.send(output+b'\n')
		elif choice == "2":
			ciphertext = urandom(16)
			associated_data = urandom(16)
			c.send(str({"ciphertext": ciphertext.hex(), "associated_data": associated_data.hex()}).encode()+b'\n')
			c.send(b"iv : ")
			iv = bytes.fromhex(rec(c))
			c.send(b"tag : ")
			tag = bytes.fromhex(rec(c))
			output = str(verify(c, key, ciphertext, associated_data, iv, tag)).encode()
			c.send(output+b'\n')
		elif choice == "3":
			c.send(troll)
		else:
			return


host = "0.0.0.0"
port = 12345
server = socket()
server.bind((host, port))
server.listen(5)

while True:
	conn, addr = server.accept()
	print(addr)
	p = Process(target=challenge, args=(conn,))
	p.daemon = True
	p.start()
	conn.close()
