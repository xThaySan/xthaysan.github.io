---
layout: default
type: writeup
title: Scream Like Viking
category: Cryptography
point: 221
files: ["Scream Like Viking.py"]
---

{% raw %}
> **title:** Scream Like Viking
>
> **category:** Cryptography
>
> **difficulty:** Medium
>
> **point:** 221
>
> **author:** Ectario#7001
>
> **description:**
>
> Our protagonist John is in a room, he hears some kind of noise, like something resonating.
> 
> But he doesn't understand it...
> 
> Perhaps he could play with his own echoes to guess what the meaning of this famous resonance could be...
> 
> `nc 51.68.95.78 32773`
> 

## Solution

Ce que l'on sait en regardant le fichier de chiffrement :

- Il s'agit de **`RSA`**
- **`e = 17`**, c'est une valeur assez faible
- **`p`** et **`q`** sont inconnus et des premiers aléatoires de 512 bits, peu de chance de les casser
- **`N`** est également inconnu, c'est assez rare dans un challenge

En voyant un **`e`** si petit, on pense tout de suite au théorème des restes chinois pour casser le message. Le problème étant que nous ne connaissons pas N, il faudrait trouver une solution pour le déduire.


### Retrouver N

Pour ce faire, on va réaliser une opération assez simple. Mais définissons dans un premier temps les termes que je vais utiliser ensuite :

- **`e`** : l'exposant
- **`N`** : le modulo
- **`M`** : le message
- **`C`** : le chiffré, *c'est à dire le résultat de M<sup>e</sup> % N, c'est ce que le serveur nous renvoie*
- **`T`** : le résultat de M<sup>e</sup> (*donc sans appliquer le modulo*).

Ce que l'on peut noter c'est que **`T - C`** est forcèment un multiple de notre inconnue **`N`**.

> Pour illustrer ceci, prenons les valeurs suivantes :
> 
> - **`e = 2`**
> - **`N = 10`**
> - **`M = 5`**
> 
> Commençons par calculer **`T`** : <br>
> `T = M ^ e` <br>
> `T = 5 ^ 2` <br>
> `T = 25`
> 
> Calculer maintenant **`C`** : <br>
> `C = (M ^ e) % N` <br>
> `C = (5 ^ 2) % 10` <br>
> `C = 25 % 10` <br>
> `C = 5`
> 
> On a bien alors **`T - C`** congru à **`N`**, plus simplement dit : **`T - C`** est un multiple de **`N`**.


Revenons à notre problème, étant donné que l'on connaît **`e`** et que nous choisissons **`M`**, on peut calculer la puissance de notre côté pour obtenir **`T`** et le serveur lui nous fournira **`C`**.

Et que se passe-t-il avec suffisament de multiples d'un nombre inconnu ?

**En regardant leur PGCD on retrouvera ce nombre inconnu.**

Partons du principe maintenant que nous connaissons **`e`** et **`N`**.


### Théorème des restes chinois

Je ne vais pas faire l'affront aux mathématiciens d'essayer d'expliquer le théroème en profondeur. Ce qu'il suffit de retenir pour nous :

Si un **même** message est chiffré autant de fois que la valeur de **`e`**, et qu'à chaque fois on utilise la **même** valeur **`e`** mais avec des **`N`** différents, on peut retrouver le message initial.

> Pour faire plus simple : si mon message **`M`** est chiffré **3** fois avec les clés suivantes : **`(e, N1)`**, **`(e, N2)`** et **`(e, N3)`** où **`e = 3`**, alors je peux casser le chiffrement.
> 
> Cela fonctionne bien évidemment aussi si mon message est chiffré avec **17** clés dont le **`e = 17`**.


### Construction du script

```python
from math import gcd
import libnum
import socket
from Crypto.Util.Padding import pad
from Crypto.Util.number import bytes_to_long, long_to_bytes

# Le e que nous connaissons
e = 17

# Pour appliquer le théorème des restes chinois, on doit se souvenir des clés et des chiffrés (N et C).
modulus = []
ciphers = []

# On doit trouver 17 clés (car e = 17) 
for j in range(e):
    # Connection au serveur
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.connect(('51.68.95.78', 32773))

    # Ensemble des multiples de N
    multiples_N = []
    s.recv(2048)
    
    # On va calculer 10 multiples
    for m in range(10):
        # Envoie d'une valeur à chiffrer
        s.send(b'Encrypt\n')
        s.recv(2048)
        s.send(str(m).encode() + b'\n')

        # Récupération du chiffré
        C = int(s.recv(2048).decode().split('\n\n')[0])

        # Calcul de la puissance sans le modulo
        T = pow(bytes_to_long(pad(long_to_bytes(m), 50)), e)
        
        # Ajout du multiple à notre liste
        multiples_N.append(T - C)
    
    # Une fois 10 multiples récupérés, calcul de leur PGCD pour en déduire N
    N = gcd(*multiples_N)

    # Demande du flag chiffré avec cette clé (e, N)
    s.send(b'Flag\n')
    cipher = int(s.recv(2048).decode().split('Flag cipher for you: ')[1].strip())

    # Ajout du modulo N déduit
    modulus.append(N)

    # Ajout du flag chiffré
    ciphers.append(cipher)

# Application du théorème des restes chinois
res = libnum.solve_crt(ciphers, modulus)
val = libnum.nroot(res, e)

# Conversion en bytes
flag = long_to_bytes(val)

# Affichage du flag
print(flag.strip().decode())

# PWNME{3e851a6cc5525581446cad5694185b99}
```

**`FLAG : PWNME{3e851a6cc5525581446cad5694185b99}`**

{% endraw %}
