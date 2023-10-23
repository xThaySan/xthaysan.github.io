---
layout: default
type: writeup
title: Crypt My Loop
category: Crypto
point: 25
files: ["flag_maraboute.txt"]
---

{% raw %}
> **title:** Crypt My Loop
>
> **category:** Crypto
>
> **difficulty:** -
>
> **point:** 25
>
> **author:** -
>
> **description:**
> 
> <img src="https://cdn.iconscout.com/icon/free/png-256/free-france-flag-country-nation-empire-36011.png?f=webp" width="20" height="20"/>
>
> Un flag a été marabouté ! Retrouvez le.
>
> <img src="https://icons.iconarchive.com/icons/twitter/twemoji-flags/256/United-Kingdom-Flag-icon.png" width="20" height="20"/>
>
> Some magic on the flag ! Fint it.
>
> Auteur : K_lfa (BZHack / ESDAcademy)

## Solution

En cherchant quelques lignes du fichier sur internet, on comprend qu'il contient une liste de hash MD5.

Les banques de données de hash en ligne montrent qu'il s'agit de hash d'un seul caractère à chaque fois.

Il suffit alors de se construire un dictionnaire avec les hash de tous les caractères possibles puis de remplacer ceux du fichier par leur correspondance.

```python
from Crypto.Hash import MD5
from string import printable

# Récupération des hash dans le fichier
data = open('./flag_maraboute.txt').read().split('\n')

# Création du dictionnaire de hash
hashes = {MD5.new(c.encode()).hexdigest(): c for c in printable}

# Remplacement des hash par la lettre correspondante
flag = ''.join([hashes[line] for line in data])

print(flag)
```

```
iletaitunefoisunflagFLAG{md5_really_sucks}
```

**`FLAG : FLAG{md5_really_sucks}`**

{% endraw %}