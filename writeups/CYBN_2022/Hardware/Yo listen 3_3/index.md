---
layout: default
type: writeup
title: Yo listen 3/3
category: Hardware
point: 50
files: []
---

{% raw %}
> **title:** Yo listen 3/3
>
> **category:** Hardware
>
> **difficulty:** Moyen
>
> **point:** 50
>
> **author:** Maestran
>
> **description:**
>
> Bon super, on a trouvé son matos. Laissons le tourner pour l'instant.
>
> Il serait intéressant de voir ce que ce truc diffuse en boucle sur les postes qui se connectent.
>
> Le code du wifi est "toto1234@".
>
> **Challenge à effectuer en physique uniquement**
>
> 

## Solutions

Malheureusement je n'ai aucune capture d'écran du challenge mais voici la logique :

En se connectant au routeur en wifi et en écoutant le traffic dessus via Wireshark, on pouvait constater des paquets sur le canal broadcast tapant sur différents ports.

Les ports tapés par les requêtes correspondaient simplement à la valeur ASCII des lettres du flag.

**`FLAG : ?`**

{% endraw %}
