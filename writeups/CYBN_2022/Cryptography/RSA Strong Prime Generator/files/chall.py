from Crypto.Util.number import bytes_to_long, long_to_bytes, getPrime, isPrime
import math
import random

# Importing flag : 
message = b"XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"   # This is not that easy !

def getcustomprime(x): # The orginal get prime from Crypto.Util is not enough random I think so here is my own implementation
    temp = pow(2,x) + 1
    while(True):
        temp += 2 # No need to check 1 to 1 cuz an even number will never be prime ! eeh
        if(isPrime(temp)):
            if(random.randint(0,x/4) == 1):
                return temp
                
def getQfromP(p,x): # Why should i bother to take another big prime nubmer when i can just search from my first one ???
    q = p + pow(x,4)
    if(q % 2 == 0):
        q += 1
    while(True):
        q += 2 # No need to check 1 to 1 cuz an even number will never be prime ! eeh
        if(isPrime(q)):
            return q


p  = getcustomprime(2048)  
q  = getQfromP(p,2048)      # Generate a random large prime (should ALWAYS be kept PRIVATE)


N = p*q
totient = (p-1)*(q-1)
e = 65537
d = pow(e, -1, totient) 


message = bytes_to_long(message) # Transform bytes to a number (long) so that we can apply RSA

# Encryption
ciphertext = pow(message, e, N) 

print("M : " + str(ciphertext))
print("N : " + str(N))
print("E : " + str(e))
