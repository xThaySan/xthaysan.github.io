---
layout: default
type: writeup
title: Kentucky Fried Chicken
category: Crypto
point: 500
files: ["fichier_de_recettes.7z"]
---

{% raw %}
> **title:** Kentucky Fried Chicken
>
> **category:** Crypto
>
> **difficulty:** -
>
> **point:** 500
>
> **author:** -
>
> **description:**
> 
> <img src="https://cdn.iconscout.com/icon/free/png-256/free-france-flag-country-nation-empire-36011.png?f=webp" width="20" height="20"/>
>
> KFC une grande chaîne de restauration rapide spécialisée dans le poulet, nous a récemment contactés pour signaler la perte de son mot de passe nécessaire pour accéder à son livre de recettes.
>
> Ils souhaitent récupérer la recette de leur poulet frit afin d'y ajouter une nouvelle épice!
>
> KFC nous a remis son fichier de recettes confidentielles accompagné d'une clé primaire, de deux clés combinées, et de la formule pour générer le mot de passe.
>
> Votre mission consiste à découvrir les autres clés primaires et à finalier la formule pour déverrouiller le livre de recettes et obtenir la fameuse recette tant convoitée.
>
> Le flag est caché dans le fichier de recettes.
>
> ```
>
> Key1 = 1039380a3d3c0d0028465f0b3b016d704c1333193e7a12205a2d0812
>
> Key2 = 796a6d440c6a583705213558577159231276103c074e715469665a3c
>
> Key3 = 29011f095c24234c5654580723410665231874417a1e38121928237d
>
> Password^Key1^key2^Key3 = 086744430f47467f12625875283534244866180b040a4e013176744e
>
> Recettes confidentielles : « fichier_de_recettes.7z »
>
> ```
>
> Auteur : Cybersecurity Alliance [Joey,Arcade,Titouann] (ESDAcademy - ENI Rennes - RESD05)

## Solution

**`FLAG : 404CTF{}`**

{% endraw %}