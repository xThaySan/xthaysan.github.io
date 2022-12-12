{% raw %}
# Write-Up
> **title:** Smiley😃
>
> **category:** Cryptographie
>
> **difficulty:** Moyen
>
> **point:** 50
>
> **author:** Langley
>
> **description:**
>
> 🤣 🤣 🤣 🤣 et tu brute 🤣 🤣 🤣 🤣

## Analyse du fichier

Le fichier contient ça :

```
🐷👍🐶👂👯𾨢👓🐨👊🐧👓👩👢🐥👗🐤👘🐧👓𾷢👱
```

La première chose à remarquer, c'est que **certains émojis se répètent**, comme **`👓`**.

Sachant qu'il existe un grand nombre d'emoji, la probabilité que des emojis pris aléatoirement se répètent 3 fois est très faible, il s'agit donc de **substitution**.

Avant de faire compliqué, faisons simple avec un **chiffrement de César**, qui est la substitution basique. En imaginant le flag sous la forme **`CYBN{...}`**, on voit que les crochets **`{}`** seraient aussi substitués, ce qui nous ferait appliquer un décalage sur **leur valeur utf-8** (*oui oui, ce sont des emojis donc c'est encodé en utf-8*) et non sur les lettres uniquement.

Sinon, en cherchant **`et tu brute`** sur google, on voit que c'est une phrase attribuée à Jules César, l'hypothèse est encore plus vraisemblable.

Si l'on tente en python :

```python
print(ord('🐷') - ord('C'))
print(ord('👍') - ord('Y'))
print(ord('🐶') - ord('B'))
print(ord('👂') - ord('N'))
```
```
Output:
127988
127988
127988
127988
```

**On a un décalage constant**. La clé serait alors 127988.

## Déchiffrement du fichier

Il suffit simplement de lire les caractères 1 à 1 puis de leur **retirer la valeur 127988** pour obtenir le caractère déchiffré du flag.

En python ça nous donne :

```python
with open('./flag.crypt', encoding='utf-8') as f:
    data = f.read()

with open('./flag.txt', 'wb') as f:
    f.write(bytearray(''.join([chr(ord(i)-127988) for i in data]), encoding="utf-8"))
```

**Le flag : CYBN{😮_4V3_un1c0d3_🧮}**
{% endraw %}