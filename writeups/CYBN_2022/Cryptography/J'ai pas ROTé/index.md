---
layout: default
type: writeup
title: J'ai pas ROTé
category: Cryptography
point: 25
files: []
---

{% raw %}
> **title:** J'ai pas ROTé
>
> **category:** Cryptography
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** Lmeaou
>
> **description:**
>
> votre collègue, adepte de blagues plus que de cryptographie, vous envoie la phrase suivante dans la conversation d'équipe:
>
> "j'vous promets les gars, cette fois, j'ai pas ROTé: 871 1157 858 1014 1599 1287 507 663 1508 1261 1365 1508 1235 1456 676 1495 1235 637 1235 1287 663 1495 1261 1482 1625"
>
> 

## Solution

On nous sait que le flag est sous format `CYBN{...}` et le début est `871 1157 858 ...`.

L'avantage qu'on a est que dans CYBN, le **C** et le **B** sont très proches et l'on voit que 871 (qui correspond à la position du C) et le 858 (le B) sont très proches également avec une différence de **13**.

Une autre façon de trouver et de diviser 871 par la valeur ASCII de C (67) on trouve également **13**.

Le schéma se déssine, la valeur ASCII chaque lettre a été multipliée par 13, en python ça donne :

```python
numbers = "871 1157 858 1014 1599 1287 507 663 1508 1261 1365 1508 1235 1456 676 1495 1235 637 1235 1287 663 1495 1261 1482 1625".split(" ")
print(''.join([chr(int(n) // 13) for n in numbers]))
# CYBN{c'3tait_p4s_1_c3sar}
```

**`FLAG : CYBN{c'3tait_p4s_1_c3sar}`** 

{% endraw %}
