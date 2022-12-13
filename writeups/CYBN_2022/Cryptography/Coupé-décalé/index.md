---
layout: default
type: writeup
title: Coupé-décalé
category: Cryptography
point: 25
files: ['coupe_decale.png']
---

{% raw %}
> **title:** Coupé-décalé
>
> **category:** Cryptography
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** Marie-Jeanne
>
> **description:**
>
> J'espère que vous avez une bonne vue.
>
> *Le flag que vous allez trouver n'est pas au format CYBN*
>
> *Vous pouvez l'ajouter entre CYBN{}*


## Solution

Le titre est assez explicit :
- **coupé** : on voit plusieurs lignes, on imagine qu'il faut les assembler
- **décalé** : dans un challenge de cryptographie, c'est clairement du **César**


Il suffit de faire un César sur chaque ligne, la clé de chiffrement utilisée est notée à droite de la ligne, on prend donc son inverse dans 26 :
```
( 1) C 			=> (25) B
( 2) TC 		=> (24) RA
( 3) YRO 		=> (23) VOL
( 4) IJPE 		=> (22) EFLA
( 5) LJXYS 		=> (21) GESTN
( 6) UCEUAY 		=> (20) OWYOUS
( 7) LLTLPSM 		=> (19) EEMEILF
( 8) ICBIRWCB 		=> (18) AUTAJOUT
( 9) NAMNBCRA 		=> (17) ERDESTIR
(10) ODCOXDBO 		=> (16) ETSENTRE
(11) NLSBFPXZE 		=> (15) CAHQUEMOT
```

En python :

```python
from string import ascii_uppercase as charset
lines = ["C", "TC", "YRO", "IJPE", "LJXYS", "UCEUAY", "LLTLPSM", "ICBIRWCB", "NAMNBCRA", "ODCOXDBO", "NLSBFPXZE"]
print(''.join([charset[(charset.index(c) + 26-i-1) % len(charset)] for i in range(len(lines)) for c in lines[i]]))
# BRAVOLEFLAGESTNOWYOUSEEMEILFAUTAJOUTERDESTIRETSENTRECAHQUEMOT
```


**`FLAG : CYBN{NOW-YOU-SEE-ME}`** 

{% endraw %}
