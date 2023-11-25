---
layout: default
type: writeup
title: AES IV
category: Crypto
point: 486
files: ["output.txt", "source.py"]
---

{% raw %}
> **title:** AES IV
>
> **category:** Crypto
>
> **difficulty:** -
>
> **point:** 486
>
> **author:** -
>
> **description:**
> 
> <img src="https://icons.iconarchive.com/icons/twitter/twemoji-flags/256/United-Kingdom-Flag-icon.png" width="20" height="20"/>
>
> I lost my IV, can you help me to recover it?
>
> Auteur : Raccoon (BZHack Friends)

## Solution

Il s'agit d'AES en mode CBC. Les données chiffrées / déchiffrées sont coupées en blocs de 16 octets.

Dans le code source, on déchiffre 32 octets de valeur NULL (00). Voici des schémas pour comprendre un peu ce qu'il se passe, en vert ce sont les données que nous connaissons et en rouge les inconnues :

![Explication de l'AES en mode CBC](./images/diagramme_0.png)

```
Pour déchiffrer en mode CBC, pour chaque blocs :
1. On déchiffre avec la clé
2. On xor avec le bloc chiffré précédent
   Pour le premier bloc, le xor se fait avec l'IV
```

![Explication de l'AES en mode CBC](./images/diagramme_1.png)

```
Ici on sait que l'on déchiffre 32 octets, soit 2 blocs. Ces 2 blocs sont uniquement fait de valeurs 0.

On connaît également le résultat du déchiffrement, mais pas ceux de l'étape juste avant le XOR.

Or pour le second bloc, on sait que le résultat est issu du XOR avec le premier bloc chiffré, donc NULL.

Comme X XOR 0 = X, on connaît maintenant le résultat avant le XOR, qui est le même pour les 2 blocs.
```

![Explication de l'AES en mode CBC](./images/diagramme_2.png)

```
Maintenant que l'on connaît le résultat du bloc 1 avant le XOR avec l'IV puis après, il suffit de faire un XOR sur les 2 pour retrouver l'IV.

Autrement dit, il faut faire un XOR entre les 16 premiers octets du résultat et les 16 suivants.
```

![Explication de l'AES en mode CBC](./images/diagramme_3.png)

Voici le code Python :

```python
ciphertext = bytes.fromhex(open('output.txt').read())

bloc_1 = ciphertext[00:16]
bloc_2 = ciphertext[16:32]
iv = bytes([bloc_1[i] ^ bloc_2[i] for i in range(16)])
flag = "FLAG{" + iv.hex() + "}"
print(flag)
```

**`FLAG : FLAG{dcf54172d25449f8bc67f7672af1c0e6}`**

{% endraw %}