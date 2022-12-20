---
layout: default
type: writeup
title: Roombaverse Simulator - Roomba tricheur
category: Misc
point: 50
files: []
---

{% raw %}
> **title:** Roombaverse Simulator - Roomba tricheur
>
> **category:** Misc
>
> **difficulty:** Moyen
>
> **point:** 50
>
> **author:** MrSheepSheep
>
> **description:**
>
> Terminez Roomba Simulator.
>
> 
>
> https://roombaverse.cybernight-c.tf/roombaverse-simulator

## Solution

Plusieurs façons de faire comme trouver un glitch dans le jeu ou modifier son score via un logiciel comme CheatEngine. Voici deux solutions :

### Glitch

La gravité est intentionnellement mal codée, ce qui permet en montant sur le pouf au sol de voler jusqu'au canapé pour récupérer la dernière boule jaune. 

![GIF montrant comment glitcher](images/glitch.gif)

### CheatEngine

Première étape on démarre le jeu et CheatEngine. Ensuite on attache le processus du jeu dans CheatEngine.

![Attacher le processus](images/process_attach.png)

![Liste des processus](images/process.png)

On fait une première recherche de notre score actuel (**0**) dans CheatEngine. Comme c'est une valeur entière, on peut parier sur le fait que c'est codé en tant que Integer, donc  4 bytes.

![Scan de la valeur 0](images/scan_0.png)

Maintenant on ramasse une boule jaune dans le jeu pour faire augmenter notre score à 1, puis on relance une recherche avec **Next Scan** sur la valeur 1.

![Scan de la valeur 1](images/scan_1.png)

6666 adresses compatibles, c'est trop donc on répète l'opération. On augmente notre score à 2 et l'on recherche la valeur, de mon côté je trouve une soixantaine d'adresses, donc je répète avec mon score à 3 et je trouve finalement 4 adresses :

![Scan de la valeur 3](images/scan_3.png)

Maintenant on ajoute ces adresses à notre liste :

![Ajout des adresses à notr liste](images/add_selected.png)

On modifie la valeur des 4 adresses de notre liste en pour les mettre à 5 :

![Modification des valeurs](images/value.png)

Enfin, on ramasse une boule jaune, ce qui incrémentera notre score à 6 :

![Flag](images/flag.png)

**`FLAG : CYBN{FLYING_R0OMB4}`**

{% endraw %}
