---
layout: default
type: writeup
title: Repeat
category: Programmation
point: 25
files: []
---

{% raw %}
> **title:** Repeat
>
> **category:** Programmation
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** Maestran
>
> **description:**
>
> Timon : Il te faut peut-être une autre méthode, répète après moi : Hakuna Matata.
>
> Simba : Quoi ?!
>
> Pumba: Ha-ku-na Ma-ta-ta ! ça veut dire: pas de soucis !!
>
> > nc 10.242.0.1 10002
>
> *Vous devez être connecté en VPN pour accéder à ce challenge.*
>
> 

## Solution

En python :

```python
import socket

HOST = "10.242.0.1"
PORT = 10002

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

for i in range(101):
	n = s.recv(1024).split(b"\n")[-2]
	s.send(n + b"\n")
print(s.recv(1024).decode())
# Bravo ! :
# CYBN{S0m3t1m3s_1t's_34s13r_t0_4ut0m4t3_n0?}
```

**`FLAG : CYBN{S0m3t1m3s_1t's_34s13r_t0_4ut0m4t3_n0?}`**

{% endraw %}
