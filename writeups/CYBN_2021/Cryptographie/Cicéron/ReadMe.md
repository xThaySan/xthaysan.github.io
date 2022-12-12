# Write-Up
> **title:** Cicéron
>
> **category:** Cryptographie
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** Maestran
>
> **description:**
>
> Cicéron c'est pas carré.
>
> En décodant ce challenge, vous allez trouver une chaine : cybnxxxxxxxxxxxxxxxx
>
> Le flag est alors CYBN{XXXXXXXXX}

## Analyse du fichier et recherches

Une chose saute aux yeux en ouvrant le fichier : **ce ne sont que des nombre à 2 chiffres**.

La description nous disant **`Cicéron c'est pas carré`**, on peut rechercher sur google **`carré cryptographie`**. On tombe alors sur le **[Carré de Polybe](https://fr.wikipedia.org/wiki/Carré_de_Polybe)**. Il s'agit simplement d'une méthode de chiffrement par substitution qui s'appuie sur un matrice 5x5 avec l'alphabet à l'intérieur. Cette mattrice ressemble à cela :

<table>
  <tr>
    <th></th>
    <th>1</th>
    <th>2</th>
    <th>3</th>
    <th>4</th>
    <th>5</th>
  </tr>
  <tr>
    <th>1</th>
    <td>A</td>
    <td>B</td>
    <td>C</td>
    <td>D</td>
    <td>E</td>
  </tr>
  <tr>
    <th>2</th>
    <td>F</td>
    <td>G</td>
    <td>H</td>
    <td>I/J</td>
    <td>K</td>
  </tr>
  <tr>
    <th>3</th>
    <td>L</td>
    <td>M</td>
    <td>N</td>
    <td>O</td>
    <td>P</td>
  </tr>
  <tr>
    <th>4</th>
    <td>Q</td>
    <td>R</td>
    <td>S</td>
    <td>T</td>
    <td>U</td>
  </tr>
  <tr>
    <th>5</th>
    <td>V</td>
    <td>W</td>
    <td>X</td>
    <td>Y</td>
    <td>Z</td>
  </tr>
</table>

Le texte chiffré à l'aide de cette méthode ressemble étrangement à notre texte, partons là dessus.

## Déchiffrement

Petit script python pour déchiffrer tout ça :

```python
with open('./ciceron.txt') as f:
  # On récupère les nombres et on les parse en tuple de 2 chiffres
  msg = [(int(i[0]), int(i[1])) for i in f.read().split(' ')]

# La matrice de substitution
matrix = ['ABCDE', 'FGHIK', 'LMNOP', 'QRSTU', 'VWXYZ']

# Récupère les lettres dans la matrice et les assemble
print(''.join([matrix[code[0]-1][code[1]-1] for code in msg]))
```

```
Output: HLLUHSUEEHTALORSDYAPPROEONDHRSONAFONHSANTERANFHNPOURVOHRSACCOMPLHRSONEOREAHTQUHPARSURCROHTPROEHTAAUPAYSPUHSQUUNMOHSPLUSTARDTOUTUNCGACUNSACCORDAHTPOURNANTHRLHNTRHFANTELUXQUHSOURDAHTDUPUHTSDUNEORTPOUVOHRCURATHESURTOUTANTHCATARRGALMAHSSAPPLHQUANTAUSSHALALBUFOALANCGHLOPSAUXBUBONSAUXCALCULSAUXCGALAZHONSAUTRHSMUSAUPHTYRHASHSAUMALBLANCAUPRURHFOAUMALCADUCAUFLOSSANTGRAXELAFCYBNBRAVOTOTOLOTODODO
```

On trouve tout à la fin : **`CYBNBRAVOTOTOLOTODODO`**

**Le flag : CYBN{BRAVOTOTOLOTODODO}**
