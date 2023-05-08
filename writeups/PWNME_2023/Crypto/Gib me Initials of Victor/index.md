---
layout: default
type: writeup
title: Gib me Initials of Victor
category: Cryptography
point: 50
files: ["encrypt.py", "Gib me Initials of Victor.txt"]
---

{% raw %}
> **title:** Gib me Initials of Victor
>
> **category:** Cryptography
>
> **difficulty:** Easy
>
> **point:** 50
>
> **author:** Ectario#7001
>
> **description:**
>
> Imagine making an encryption system. That's what someone did...
> 
> Find a way to recover the flag.
> 

## Solution

Dans le fichier `encrypt.py` on voit que le chiffrement est fait de cette façon :

```python
from Crypto.Cipher import AES
from Crypto.Util.Padding import pad, unpad
import os

FLAG = "PWNME{redacted}"

# Choix d'une clé aléatoire
KEY = os.urandom(16)

def encrypt_flag():
    # Choix d'un IV aléatoire
    iv = os.urandom(16)

    # Utilisation d'AES CBC avec la clé et l'IV
    cipher = AES.new(KEY, AES.MODE_CBC, iv)

    # Chiffrement après padding par bloc de 16
    encrypted = cipher.encrypt(pad(FLAG.encode(), 16))

    # Création d'une signature grâce à un XOR entre l'iv et la clé inversée (en partant de la fin vers le début)
    signature = [hex(a ^ b)[2:].zfill(2) for a, b in zip(iv, KEY[::-1])]
    signature = "".join(signature).

    # Le résultat est une partie de l'iv, suivi du chiffrement puis de la signature 
    ciphertext = iv.hex()[4:] + encrypted.hex() + signature
    return {"ciphertext": ciphertext}
```

Comme le XOR est réversible et que l'on dispose de la signature et d'une partie de l'IV, on peut refaire l'opération pour retrouver la clé de base.

Cependant il manque les 2 premiers octets de l'iv dans notre ciphertext, il faut donc les **bruteforcer**, soit 256 * 256 = 65536 possibilités.

Pour filtrer les résultats de ces possibilités, on sait que le message commence par **`PWNME{`**.

```python
from itertools import product
from Crypto.Cipher import AES
from Crypto.Util.Padding import unpad

with open('./Gib me Initials of Victor.txt') as f: ciphertext = bytes.fromhex(eval(f.read())['ciphertext'])

# Récupération des 14 octets de l'IV
iv = ciphertext[:14]

# Récupération du chiffré
encrypted = ciphertext[14:-16]

# Récupération de la signature
signature = ciphertext[-16:]

print(f"IV: ????{iv.hex()}")
print(f"SI: {signature.hex()}")
print(f"C:  {encrypted.hex()}")

# Bruteforce des 2 octets
for b0, b1 in product(range(256), range(256)):
    # On complète l'iv avec les 2 octets manquants
    complete_iv = bytearray([b0, b1, *iv])

    # On déduit la clé en refaisant un XOR entre l'IV et la signature, donnant l'inverse de clé, on réinverse alors ensuite
    KEY = bytes.fromhex(''.join([hex(a ^ b)[2:].zfill(2) for a, b in zip(complete_iv, signature)][::-1]))

    # On tente de déchiffrer avec ces paramètres
    cipher = AES.new(KEY, AES.MODE_CBC, complete_iv)
    try:
        decrypted = unpad(cipher.decrypt(encrypted), 16).decode()
        # Si le déchiffrement réussi et commence par "PWNME{", alors on affiche
        if decrypted.startswith("PWNME{"):
            print(f"M:  {decrypted}")
    except:
        continue

# IV: ????ec99a438e52de135ad277039ce23
# SI: 0f9c361b0e2a7331b40cdf3df7bb2d4c
# C:  2c148aedd7c3f9a0688a3c95b6de4b4c35acca54edced84032f70c8ea88a1338d361b0fec7861c2eb26c244b99de45e6
# M:  PWNME{0mg_Cr34t1ng_4n_4lg0_l1ke_th4t_wtf_br0}
```

**`FLAG : PWNME{0mg_Cr34t1ng_4n_4lg0_l1ke_th4t_wtf_br0}`**

{% endraw %}
