{% raw %}
# Write-Up
> **title:** Simple Basique
>
> **category:** Cryptographie
>
> **difficulty:** Moyen
>
> **point:** 50
>
> **author:** Maestran
>
> **description:**
>
> [Vous n'avez pas les bases](https://www.youtube.com/watch?v=2bjk26RwjyU)

## Analyse du fichier

Tout d'abord, on se rend compte que le fichier contient beaucoup de 0. Le début ressemble a du binaire, et plus on avance, plus on trouve de 0 et de caractères alphabétique.

La description du challenge nous parle de bases. Essayons de passer ce qui ressemble à du binaire en base 10 et en char. On prend ce qui ressemble aux 8 premiers bits :

```
01000011 (2) => 67 (10) => C
```

C'est pas mal, ça nous donne un **`C`**, comme le début des flag **`CBYN`**.
Si l'on continue avec les 8 suivants : **`00010022`**, ça n'est plus du binaire mais on peut facilement imaginer que c'est de la **base 3** puisque nous n'avons que 3 caracètres différents, essayons :

```
00010022 (3) => 89 (10) => Y
```

De mieux en mieux, ça correspond au flag, on peut en déduire la logique suivante : on peut grouper par 8 caractères le contenu de notre fichier et à chaque fois on le décode de la base suivante en commençant à partir de la base 2 !

Petit script python pour faire ça :

```python
with open('./encoded.base') as f:
    msg = f.read()
groups = [msg[i:i+8] for i in range(0, len(msg), 8)]
# Base d'encodage actuelle
base = 2
flag = ''
for group in groups:
    # On décode la string en nombre selon la base actuelle
    i = int(group, base)

    # On transforme ce nombre en char selon l'ASCII et on ajoute le résultat au flag
    result += chr(i)

    # On passe à la base suivante
    base += 1
print(flag)
```

Mon côté oneliner me dit que l'on peut résumer le script en ça :

```python
with open('./encoded.base') as f:
    msg = f.read()
print(''.join([chr(int(a[i:i+8], (i//8)+2)) for i in range(0, len(a), 8)]))
```

```
Output: CYBN{f1nfr3r0t_tu_es_gr1ng3}
```

**Le flag : CYBN{f1nfr3r0t_tu_es_gr1ng3}**
{% endraw %}
