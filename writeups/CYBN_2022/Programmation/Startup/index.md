---
layout: default
type: writeup
title: Startup
category: Programmation
point: 10
files: []
---

{% raw %}
> **title:** Startup
>
> **category:** Programmation
>
> **difficulty:** Facile
>
> **point:** 10
>
> **author:** Maestran
>
> **description:**
>
> C'est le début, il faut juste se chauffer : 
>
> > nc 10.242.0.1 10001
>
> *Vous devez être connecté en VPN pour accéder à ce challenge.*
>
> 

## Solution

Un simple netcat suffit mais voici la solution en python :

```python
import socket

HOST = "10.242.0.1"
PORT = 10001

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

print(s.recv(1024).decode())
# C'était facile non ? Le flag est :
# CYBN{Welcome!}
```

**`FLAG : CYBN{Welcome!}`**

{% endraw %}
