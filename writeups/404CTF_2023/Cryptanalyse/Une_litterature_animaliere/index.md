---
layout: default
type: writeup
title: Une littérature animalière
category: Cryptanalyse
point: 1000
files: ['chiffre.wav']
---

{% raw %}
> **title:** Une littérature animalière
>
> **category:** Cryptanalyse
>
> **difficulty:** Facile
>
> **point:** 1000
>
> **author:** NainCapable#2614
>
> **description:**
> Au café littéraire, vous bavardez avec Voltaire.
> 
> Il se trouve que ce petit sacripan s'est infiltré dans la maison de Rousseau pour trouver des informations compromettantes sur son nouveau livre "Le Gorfou ou de l'éducation".
> 
> La seule chose qu'il a récupéré est un étrange fichier, qu'il a mis dans son manteau avant de prendre la poudre d'escampette.
> 
> Il espère pouvoir obtenir votre aide pour lire ce fichier décidément bien étrange...
> 
> Bonne chance
> 
> ***

## Solution

En faisant la commande **`file`** on voit que **`chiffre.wav`** est en fait un fichier **ZIP** :

```
┌──(kali㉿kali)-[~]
└─$ file chiffre.wav 
chiffre.wav: Zip archive data, at least v2.0 to extract, compression method=deflate
```

A l'intérieur se trouve le fichier **`chiffre.png`** qui est en fait un fichier texte qui ne contient que des **`0`** et des **`1`**.

Au total il y a **`29 740 656`** chiffres dans ce fichier. Ce nombre étant divisible par **`8`**, on peut les interpréter en octets pour former un fichier :

```python
data = open('chiffre.png').read()
data = [int(data[i:i+8], 2) for i in range(0, len(data), 8)]
```

Cependant même avec cela, le fichier ne semble correspondre à rien.

Sachant que d'après l'extension c'est un PNG et que nous sommes en Cryptanalyse, on peut se dire que le fichier est chiffré. On va donc tester le chiffrement le plus basique : le XOR.

On prend le header des PNG : **`89 50 4E 47 0D 0A 1A 0A`** et on le XOR avec le début du fichier :

```python
data = open('chiffre.png').read()
data = [int(data[i:i+8], 2) for i in range(0, len(data), 8)]

header = bytes.fromhex("89 50 4E 47 0D 0A 1A 0A")
print([header[i] ^ data[i] for i in range(len(header))])
# [100, 111, 196, 144, 40, 198, 200, 223]
```

En regardant en base 10, rien d'apparant. Mais si on passe le tout en binaire :

```python
data = open('chiffre.png').read()
data = [int(data[i:i+8], 2) for i in range(0, len(data), 8)]

header = bytes.fromhex("89 50 4E 47 0D 0A 1A 0A")
print(''.join([f"{header[i] ^ data[i]:0>8b}" for i in range(len(header))]))
# 0110010001101111110001001001000000101000110001101100100011011111
```

Là on voit que tous les 47 bits, on a un répétition :

`01100100011011111100010010010000001010001100011 01100100011011111`

On va donc XOR ces bits en les répétant autant de fois que nécessaire avec les bits de **`chiffre.png`** :

```python
data = open('chiffre.png').read()
data = [int(data[i:i+8], 2) for i in range(0, len(data), 8)]

header = bytes.fromhex("89 50 4E 47 0D 0A 1A 0A")

key = "01100100011011111100010010010000001010001100011"
key = key * (len(data) * 8 // len(key) + 1)
key = [int(key[i:i+8], 2) for i in range(0, len(key), 8)]

with open('decrypted.png', 'wb') as f:
	f.write(bytearray([key[i] ^ data[i] for i in range(len(data))]))
```

<br>

![Image déchiffrée](./images/decrypted.png)

<br>

<span class="flag">FLAG : 404CTF{1g0rfu4v3rt1Env4ut2}</span>

{% endraw %}