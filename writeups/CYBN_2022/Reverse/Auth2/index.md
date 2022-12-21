---
layout: default
type: writeup
title: Auth2
category: Reverse
point: 25
files: ['auth2.py']
---

{% raw %}
> **title:** Auth2
>
> **category:** Reverse
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** m00n
>
> **description:**
>
> Bon en vrai au début j'étais naze, je retente !  
>
> ``Le flag n'est pas au format cybn{}``
>
> 

## Solution

Il faut partir de la fin et remonter, on voit que **`a`** est comparé à **`b`** :

```python
if a == b:
    print("Can you stop breaking my authentification FOR 5 MINUTES ?! Im trying to learn !\n(tu peux soumettre ce password comme flag)")
else:
    print("Well seems stronger this time, try again !")
```

**`a`** n'est jamais modifié avant la comparaison, sa valeur **`Vly{4Gjd-vbvI~VZ8UjeGX`** ne bouge jamais. Maintenant il faut que notre **`b`** soit égale à ça et on voit à la ligne 8 qu'il se fait encoder en base 85 pour "devenir a" :

```python
b = base64.b85encode(str.encode(b))
```

Il faudra donc simplement décoder **`a`** de la base 85.

```python
from base64 import b85decode
print(b85decode("Vly{4Gjd-vbvI~VZ8UjeGX"))

# b'b3773r_4u7h_m4yb3'
```

**`FLAG : b3773r_4u7h_m4yb3`**

{% endraw %}
