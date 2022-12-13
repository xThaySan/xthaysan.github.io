---
layout: default
type: writeup
title: Timing
category: Programmation
point: 50
files: ['timing.py']
---

{% raw %}
> **title:** Timing
>
> **category:** Programmation
>
> **difficulty:** Moyen
>
> **point:** 50
>
> **author:** Maestran
>
> **description:**
>
> "Dans la vie tout est une question de timing".
>
> > nc 10.242.0.1 10003
>
> *Vous devez être connecté en VPN pour accéder à ce challenge.*
>
> 

## Solution

Pour ce challenge il fallait faire pas mal de guessing si vous vouliez le réaliser avant que le code source ne soit donné.

La première chose est de remarquer que **le nombre attendu ne change pas à chaque connexion mais uniquement toutes les secondes**, on peut s'en rendre compte en lançant plusieurs connexions à la suite :

```python
import socket
import time

HOST = "10.242.0.1"
PORT = 10003

for i in range(100):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((HOST, PORT))
	start_at = time.time()
	s.recv(1024)
	s.send(b'0\n')
	d = s.recv(1024)
	if d.startswith(b'Error'):
		n = int(d.split(b'was')[1].strip())
		print(start_at, "=>", n)

# 1670952928.1638553 => 21
# ...
# 1670952928.6083567 => 21
# 1670952928.7483597 => 3
# ...
# 1670952929.2978578 => 3
# 1670952929.3879302 => 40
```

Il y a un décalage entre le timestamp côté client et serveur dû aux configurations internes et au ping entre les deux machines mais on devine que time.time() doit être utilisé pour générer les aléatoires mais dans sa version entière puisque le changement n'a lieu que toutes les secondes, donc **int(time.time())**.


On peut également estimer **les valeurs min et max** à trouver en lancant plusieurs connexions :

```python
import socket

HOST = "10.242.0.1"
PORT = 10003

numbers = []
for i in range(100):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((HOST, PORT))
	s.recv(1024)
	s.send(b'0\n')
	d = s.recv(1024)
	if d.startswith(b'Error'):
		n = int(d.split(b'was')[1].strip())
		numbers.append(n)
print(max(numbers))
# 6 48
```

Ici on trouve **6 et 48**, il est possible que ce ne soit pas les vraies valeurs min et max mais pas c'est pas critique, une approximation suffit amplement mais ici on peut facilement deviner que c'est **0 à 50**.

Maintenant il faut vérifier nos hypothèse et par la même occasion estimer le décalage temporaire avec le serveur, pour ça on demande une valeur aléatoire et l'on cherche la bonne seed de notre côté pour générer la même :

```python
import socket
import time
import random

HOST = "10.242.0.1"
PORT = 10003

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
now = int(time.time())
s.recv(1024)
s.send(b"0\n")
d = s.recv(1024)
if d.startswith(b'Error'):
	n = int(d.split(b'was')[1].strip())
	print(f"Valeur attendue : {n}")
	for i in range(-1, 2):
		random.seed(now + i)
		n = random.randint(0, 50)
		print(f"{i} {n}")
# Valeur attendue : 36
# -1 8
# 0 45
# 1 36
```

Ici mon décalage varie entre **0 et 1 seconde** (en même temps si c'était plus j'appelerai mon FAI pour gueuler) donc **je vais ajouter une moyenne de 0.3 à time.time()** avant de le transformer en int pour la seed car j'ai plus souvent 0 que 1. Le script ne fonctionnera peut-être pas à chaque fois, il faudra juste le relancer en croisant les doigts.

On peut maintenant passer à la deuxième étape, on va essayer de trouver une approximation de les valeurs min et max :

```python
import socket
import time
import random

HOST = "10.242.0.1"
PORT = 10003

numbers = []
for i in range(10):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((HOST, PORT))
	random.seed(int(time.time() + 0.3))
	s.recv(1024)
	s.send(f"{random.randint(0, 50)}\n".encode())
	s.recv(1024)
	s.send(f"0\n".encode())
	d = s.recv(1024)
	print(d)
	n = int(d.split(b'was')[1].strip())
	numbers.append(n)
	time.sleep(1)

print(numbers)
print(min(numbers), max(numbers))

# 3753165013 48747953963
```

On arrive sur des nombres maximum à 11 chiffres donc on peut estimer que le max est entre 50 000 000 000 et 100 000 000 000.
En testant avec 100 milliards ça ne fonctionne pas, et en descendant on progressivement à 50-60 milliards ça fonctionne.

On continue :

```python
import socket
import time
import random

HOST = "10.242.0.1"
PORT = 10003

for i in range(3):
	s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
	s.connect((HOST, PORT))
	random.seed(int(time.time() + 0.3))
	s.recv(1024)
	s.send(f"{random.randint(0, 50)}\n".encode())
	s.recv(1024)
	s.send(f"{random.randint(0, 50000000000)}\n".encode())
	s.recv(1024)
	s.send(f"0\n".encode())
	d = s.recv(1024)
	print(d)
	n = float(d.split(b'was')[1].strip())
	time.sleep(1)
# b'Error, it was 0.6774753589433444\n'
# b'Error, it was 0.8810140854709425\n'
# b'Error, it was 0.3964003186630989\n'
```

On voit clairement que c'est un float entre 0 et 1, donc surement la fonction **random.random()**.

```python
import socket
import time
import random

HOST = "10.242.0.1"
PORT = 10003

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
random.seed(int(time.time() + 0.3))
s.recv(1024)
s.send(f"{random.randint(0, 50)}\n".encode())
s.recv(1024)
s.send(f"{random.randint(0, 50000000000)}\n".encode())
s.recv(1024)
s.send(f"{random.random()}\n".encode())
print(s.recv(1024).decode())

# GG
# CYBN{T1m3_1s_r34lly_tr1cky_1n_r4nd0m}
```

**`FLAG : CYBN{T1m3_1s_r34lly_tr1cky_1n_r4nd0m}`**

{% endraw %}
