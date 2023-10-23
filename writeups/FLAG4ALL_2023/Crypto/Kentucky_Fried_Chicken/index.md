---
layout: default
type: writeup
title: Kentucky Fried Chicken
category: Crypto
point: 500
files: ["fichier_de_recettes.7z"]
---

{% raw %}
> **title:** Kentucky Fried Chicken
>
> **category:** Crypto
>
> **difficulty:** -
>
> **point:** 500
>
> **author:** -
>
> **description:**
> 
> <img src="https://cdn.iconscout.com/icon/free/png-256/free-france-flag-country-nation-empire-36011.png?f=webp" width="20" height="20"/>
>
> KFC une grande chaîne de restauration rapide spécialisée dans le poulet, nous a récemment contactés pour signaler la perte de son mot de passe nécessaire pour accéder à son livre de recettes.
>
> Ils souhaitent récupérer la recette de leur poulet frit afin d'y ajouter une nouvelle épice!
>
> KFC nous a remis son fichier de recettes confidentielles accompagné d'une clé primaire, de deux clés combinées, et de la formule pour générer le mot de passe.
>
> Votre mission consiste à découvrir les autres clés primaires et à finalier la formule pour déverrouiller le livre de recettes et obtenir la fameuse recette tant convoitée.
>
> Le flag est caché dans le fichier de recettes.
>
> ```
>
> Key1 = 1039380a3d3c0d0028465f0b3b016d704c1333193e7a12205a2d0812
>
> Key2 = 796a6d440c6a583705213558577159231276103c074e715469665a3c
>
> Key3 = 29011f095c24234c5654580723410665231874417a1e38121928237d
>
> Password^Key1^key2^Key3 = 086744430f47467f12625875283534244866180b040a4e013176744e
>
> Recettes confidentielles : « fichier_de_recettes.7z »
>
> ```
>
> Auteur : Cybersecurity Alliance [Joey,Arcade,Titouann] (ESDAcademy - ENI Rennes - RESD05)

## Solution

L'une des méthode est de faire un bruteforce entre toutes les clés. Il y en a 4 donc 2^4 = 16 possibilités.

En Python :

```python
from itertools import combinations
from functools import reduce
from operator import xor
from Crypto.Util.number import long_to_bytes

k1 = 0x1039380a3d3c0d0028465f0b3b016d704c1333193e7a12205a2d0812
k2 = 0x796a6d440c6a583705213558577159231276103c074e715469665a3c
k3 = 0x29011f095c24234c5654580723410665231874417a1e38121928237d
k4 = 0x086744430f47467f12625875283534244866180b040a4e013176744e
keys = [k1, k2, k3, k4]

for r in range(2, len(keys) + 1):
	for combination in combinations(range(len(keys)), r):
		result = long_to_bytes(reduce(xor, [keys[i] for i in combination]))
		print(combination, result)
```
```
(0, 1) b'iSUN1VU7-gjSlp4S^e#%94ct3KR.'
(0, 2) b"98'\x03a\x18.L~\x12\x07\x0c\x18@k\x15o\x0bGXDd*2C\x05+o"
(0, 3) b'\x18^|I2{K\x7f:$\x07~\x134YT\x04u+\x12:p\\!k[|\\'
(1, 2) b'PkrMPN{{Sum_t0_F1nd}}PIFpNyA'
(1, 3) b'q\r)\x07\x03-\x1eH\x17Cm-\x7fDm\x07Z\x10\x087\x03D?UX\x10.r'
(2, 3) b'!f[JSce3D6\x00r\x0bt2Ak~lJ~\x14v\x13(^W3'
(0, 1, 2) b'@RJGmrv{{32TO126}}WdC*[f*cqS'
(0, 1, 3) b'a4\x11\r>\x11\x13H?\x052&DE\x00w\x16\x03;.=>-u\x02=&`'
(0, 2, 3) b"1_c@n_h3lp_y0u_1'm_S@nd3rs_!"
(1, 2, 3) b'X\x0c6\x0e_\t=\x04A\x175*\\\x05kby\x08|vyZ\x07GA8\r\x0f'
(0, 1, 2, 3) b'H5\x0e\x04b50\x04iQj!g\x04\x06\x125\x1bOoG \x15g\x1b\x15\x05\x1d'
```

On trouve le mot de passe pour la combinaison (0, 2, 3), soit avec : Key1 ^ Key3 ^ Key4. Le mot de passe qui sert à ouvrir le ZIP est donc : **`1_c@n_h3lp_y0u_1'm_S@nd3rs_!`**.

Le flag est dans le ZIP.

**`FLAG : ESD{GG_n1ce_m1nds3t}`**

{% endraw %}