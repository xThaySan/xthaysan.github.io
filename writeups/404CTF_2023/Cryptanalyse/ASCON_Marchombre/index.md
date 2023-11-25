---
layout: default
type: writeup
title: ASCON Marchombre
category: Cryptanalyse
point: 878
files: []
---

{% raw %}
> **title:** ASCON Marchombre
>
> **category:** Cryptanalyse
>
> **difficulty:** Facile
>
> **point:** 878
>
> **author:** Phengar#8046
>
> **description:**
> Cela fait maintenant quelques semaines que vous voyagez avec Salim, mais ce que vous attendez le plus chaque jour ce ne sont plus les palpitantes aventures mais plutôt la poésie marchombre que partage avec vous Salim.
> 
> Vous avez en effet pris goût à écouter les courts poèmes propres à cette guilde qui vous rappellent les haïkus de votre monde.
> 
> Ce soir cependant Salim vous met au défi de déchiffrer le code marchombre qui permet de dissimuler les messages qu'il échange avec Ellana et il vous semble alors reconnaitre un chiffrement pas tout-à-fait inconnu ...
> 
> clef : 00456c6c616e61206427416c2d466172
> 
> nonce : 0
> 
> message chiffré :
> 
> `ac6679386ffcc3f82d6fec9556202a1be26b8af8eecab98783d08235bfca263793b61997244e785f5cf96e419a23f9b29137d820aab766ce986092180f1f5a690dc7767ef1df76e13315a5c8b04fb782`
> 
> Données associées : 80400c0600000000
> 
> ***

## Solution

Le titre nous donne le nom du chiffrement : **Ascon**.

En python on utilise la lib **`ascon`** (*`pip install ascon`*)

On traduit les données qui nous sont données dans l'énoncé et on déchiffre :

```python
import ascon

key = bytes.fromhex("00456c6c616e61206427416c2d466172")
nonce = (0).to_bytes(length=16, byteorder='big')
ciphertext = bytes.fromhex("ac6679386ffcc3f82d6fec9556202a1be26b8af8eecab98783d08235bfca263793b61997244e785f5cf96e419a23f9b29137d820aab766ce986092180f1f5a690dc7767ef1df76e13315a5c8b04fb782")
associateddata = bytes.fromhex("80400c0600000000")
print(ascon.decrypt(key, nonce, associateddata, ciphertext).decode('latin-1'))
# La voie de l'ombre
# Et du silence
# 404CTF{V3r5_l4_lum1èr3.}
# Ellana
```

<span class="flag">FLAG : 404CTF{V3r5_l4_lum1èr3.}</span>

{% endraw %}