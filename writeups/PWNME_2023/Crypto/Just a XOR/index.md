---
layout: default
type: writeup
title: Just a XOR
category: Cryptography
point: 50
files: ["encrypt.py", "intercepted-original-mesage.txt", "message-encrypted.txt"]
---

{% raw %}
> **title:** Just a XOR
>
> **category:** Cryptography
>
> **difficulty:** Introduction
>
> **point:** 50
>
> **author:** Eteck#3426
>
> **description:**
>
> You are in possession of an encrypted message as well as the algorithm that encrypted it. Find the original message PS: According to a reliable source who intercepted a part of the message before encryption, you have in your possession some characters of the orginal message (The * are the unknown parts of the message)
>
> 

## Solution

Le fichier `message-encrypted.txt` contient une liste d'entiers correspondant au message en clair xoré avec une clé secrète.

Le fichier `intercepted-original-mesage.txt` contient quelques caractères du message en clair. Les caractères qui n'ont pas pu être interceptés eux apparaissent par une **`*`**.

On voit dans le fichier `encrypt.py` que l'opération de chiffrement est un simple XOR. C'est une opération réversible :

- `a ^ b = c`
- `a ^ c = b`
- `b ^ c = a`

On sait également grâce à ce fichier que la clé utilisée à une longueur de 16.

On peut donc prendre un caractère intercepté en clair et faire un xor avec le caractère chiffré à la même position dans l'autre fichier pour retrouver avec quelle valeur il a été xoré originellement.

La clé peut alors être reconstruite en le faisant sur tous les caractères interceptés.

```python
from itertools import cycle

with open('./intercepted-original-mesage.txt') as f: partial = f.read()
with open('./message-encrypted.txt') as f: encrypted = [int(n) for n in f.read().split(',')]

print(f"Parial size:    {len(partial)}")
print(f"Encrypted size: {len(encrypted)}")

SECRET_SIZE = 16
SECRET = [-1] * SECRET_SIZE

for i, c in enumerate(partial):
    if c != '*':
        SECRET[i % SECRET_SIZE] = encrypted[i] ^ ord(c)
print(f"SECRET:         {SECRET}")

decrypted = ''.join([chr(a ^ b) for a, b in zip(encrypted, cycle(SECRET))])
print(f"Decrypted:      {decrypted}")

# Parial size:    144
# Encrypted size: 144
# SECRET:         [3727, 6069, 4606, 8615, 4486, 3034, 6721, 9619, 3923, 7302, 4826, 8004, 8110, 3120, 4630, 2341]
# Decrypted:      So, I can see you know how XOR works.. Congratulation :) Here is your flag: PWNME{1t_W4s_r34aLy_Ju3s7_A_x0R} ! Good luck for the next challenges
```

**`FLAG : PWNME{1t_W4s_r34aLy_Ju3s7_A_x0R}`**

{% endraw %}
