---
layout: default
type: writeup
title: Recette
category: Cryptanalyse
point: 100
files: []
---

{% raw %}
> **title:** Recette
>
> **category:** Cryptanalyse
>
> **difficulty:** Introduction
>
> **point:** 100
>
> **author:** Neptales
>
> **description:**
> Le Commissaire Maigret, café à la main, vous raconte une de ses dernières enquêtes. Il vous explique que sur une scène de crime il a retrouvé un papier faisant office de message codé. Il le sort de sa poche pour vous le montrer :
> 
> _Convertir depuis l'hexadécimal_  
> 
> _Développer de sorte à ne plus voir de chiffres_  
> 
> _Décoder le DeadFish_  
> 
> _Convertir depuis la Base 85_  
> 
> Suivi de la séquence :
> 
> **`32 69 31 73 34 69 31 73 31 35 64 31 6f 34 39 69 31 6f 34 64 31 6f 33 69 31 6f 31 35 64 31 6f 32 32 64 31 6f 32 30 64 31 6f 31 39 69 31 6f 37 64 31 6f 35 64 31 6f 32 69 31 6f 35 35 69 31 6f 31 64 31 6f 31 39 64 31 6f 31 37 64 31 6f 31 38 64 31 6f 32 39 69 31 6f 31 32 69 31 6f 32 36 69 31 6f 38 64 31 6f 35 39 64 31 6f 32 37 69 31 6f 36 64 31 6f 31 37 69 31 6f 31 32 64 31 6f 37 64 31 6f 35 69 31 6f 31 64 31 6f 32 64 31 6f 31 32 69 31 6f 39 64 31 6f 32 36 64 31 6f`**
> 
> ***
> 
> Décodez ce message.
> 
> > *Contact en cas de problème : `Racoon#8487`*

## Solution

On suit les étapes :

```python
from re import findall
from base64 import a85decode

# Étape 0
text = "32 69 31 73 34 69 31 73 31 35 64 31 6f 34 39 69 31 6f 34 64 31 6f 33 69 31 6f 31 35 64 31 6f 32 32 64 31 6f 32 30 64 31 6f 31 39 69 31 6f 37 64 31 6f 35 64 31 6f 32 69 31 6f 35 35 69 31 6f 31 64 31 6f 31 39 64 31 6f 31 37 64 31 6f 31 38 64 31 6f 32 39 69 31 6f 31 32 69 31 6f 32 36 69 31 6f 38 64 31 6f 35 39 64 31 6f 32 37 69 31 6f 36 64 31 6f 31 37 69 31 6f 31 32 64 31 6f 37 64 31 6f 35 69 31 6f 31 64 31 6f 32 64 31 6f 31 32 69 31 6f 39 64 31 6f 32 36 64 31 6f"

# Étape 1
# On convertit chaque hexa en caratère selon la table ASCII
text = ''.join([chr(int(s, 16)) for s in text.split(' ')])
# text = 2i1s4i1s15d1o49i1o4d1o3i1o15d1o22d1o20d1o19i1o7d1o5d1o2i1o55i1o1d1o19d1o17d1o18d1o29i1o12i1o26i1o8d1o59d1o27i1o6d1o17i1o12d1o7d1o5i1o1d1o2d1o12i1o9d1o26d1o

# Étape 2
# On récupère tous les couples nombre n suivis d'un caractère c
# Puis on transforme chaque couple en n fois le caractère c 
text = ''.join([int(n) * c for n, c in findall('(\d+)(\w)', text)])
# text = iisiiiisdddddddddddddddoiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiioddddoiiiodddddddddddddddoddddddddddddddddddddddoddddddddddddddddddddoiiiiiiiiiiiiiiiiiiiodddddddodddddoiioiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiododddddddddddddddddddodddddddddddddddddoddddddddddddddddddoiiiiiiiiiiiiiiiiiiiiiiiiiiiiioiiiiiiiiiiiioiiiiiiiiiiiiiiiiiiiiiiiiiioddddddddodddddddddddddddddddddddddddddddddddddddddddddddddddddddddddoiiiiiiiiiiiiiiiiiiiiiiiiiiioddddddoiiiiiiiiiiiiiiiiioddddddddddddodddddddoiiiiiododdoiiiiiiiiiiiiodddddddddoddddddddddddddddddddddddddo

# Étape 3
# On interprète en ASCII le deadfish (ici j'ai utilisé https://www.dcode.fr/deadfish-language)
text = "1b^aR<(;4/1hgTC1NZtl1LFWKDIHFRI/"

# Étape 4
# On décode depuis la base85 en version standard
print(a85decode(text).decode())
```

```
404CTF{M4igr3t_D3_c4naRd}
```

<br>

<span class="flag">FLAG : 404CTF{M4igr3t_D3_c4naRd}</span>

{% endraw %}