---
layout: default
type: writeup
title: Le Jour de l'espace
category: Cryptanalyse
point: 874
files: []
---

{% raw %}
> **title:** Le Jour de l'espace
>
> **category:** Cryptanalyse
>
> **difficulty:** Facile
>
> **point:** 874
>
> **author:** NainCapable#2614
>
> **description:**
> Rimbaud vous propose une séance initiatique au Oui-ja dans l'aile mystique du café littéraire (oui, oui, ça existe), vous avez une vision ésotérique : 
> 
> Alors que vous voyez le texte suivant `ueomaspblbppadgidtfn`, 
> 
> Rimbaud vous décrit voir un étrange cadre de 50cm de côté, avec des petits carrés de 10cm de côtés, numérotés de 0 à 24 et jetés pêle-mêle sur le sol.
> 
> Rimbaud n'y comprends rien, mais vous restez obsédé par cette idée, et décidez de résoudre l'énigme.
> 
> ***
> 
> Toutes les informations nécéssaires à la résolution de ce challenge sont présentes dans l'énoncé ci-dessus.
> 
> > **Format** : 404CTF{cequevousalleztrouver}
> 
> ```
> nc challenges.404ctf.fr 31451
> ```

## Solution

L'objectif ici est de déchiffrer **`ueomaspblbppadgidtfn`**. Pour cela un oracle de chiffrement est à notre disposition.

On va donc devoir trouver la "logique" du chiffrement (*les équations*) derrière.

<br>

### Création d'une classe Python

Commençons par créer en **Python** une classe qui permettra de communiquer facilement avec l'oracle :

```python
import socket

class Challenge:
	def __init__(self):
		self.host = 'challenges.404ctf.fr'
		self.port = 31451
		self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.client.connect((self.host, self.port))
		self.cipher = self.client.recv(1024).decode().splitlines()[0].split(' : ')[1]

	def encrypt(self, msg: str):
		self.client.send(msg.encode() + b"\n")
		data = self.client.recv(2048).decode()
		return data.split(' : ')[1].splitlines()[0]
```

<br>

### Fonctionnement par blocs indépendants

Maintenant nous pouvons réaliser quelques essais :

```python
challenge = Challenge()
print(challenge.encrypt("a"))
# aaaaa
print(challenge.encrypt("aa"))
# aaaaa
print(challenge.encrypt("aaaaaa"))
# aaaaaaaaaa
```

Rien qu'avec ces trois messages, on en déduit que le chiffrement **se fait par bloc de 5 caractères**.

En plus de cela, les blocs n'ont pas l'air d'intérragir entre eux, **ils sont indépendants**.

<br>

### Définition du charset

Si l'on essaie toutes les minuscules une par une :

```python
challenge = Challenge()
for c in 'abcdefghijklmnopqrstuvwxyz':
	print(challenge.encrypt(c))
```

On obtient une erreur avec le **`z`**. Le charset utilisé va donc de **`a`** à **`y`**, soit 25 caractères.

Regardons alors les lettres comme un nombre allant de 0 à 24 inclus. On va ajouter la méthode statique **`to_numbers`** à notre classe **`Challenge`** :

```python
import socket

class Challenge:
	CHARSET = 'abcdefghijklmnopqrstuvwxy'

	def __init__(self):
		self.host = 'challenges.404ctf.fr'
		self.port = 31451
		self.client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
		self.client.connect((self.host, self.port))
		self.cipher = self.client.recv(1024).decode().splitlines()[0].split(' : ')[1]

	def encrypt(self, msg: str):
		self.client.send(msg.encode() + b"\n")
		data = self.client.recv(2048).decode()
		return data.split(' : ')[1].splitlines()[0]

	@staticmethod
	def to_numbers(msg):
		return ' '.join([f"{Challenge.CHARSET.index(c):0>2}" for c in msg])
```

<br>

### Réaction à un changement

Essayons maintenant de modifier 1 seul caractère :

```python
challenge = Challenge()
print(Challenge.to_numbers(challenge.encrypt("aaaaa")))
# 00 00 00 00 00
print(Challenge.to_numbers(challenge.encrypt("aaaab")))
# 08 03 12 17 24
print(Challenge.to_numbers(challenge.encrypt("aaaac")))
# 16 06 24 09 23
```

On observe plusieurs chose du fait d'avoir ajouté **`1`** au dernier caractère de notre message (*c'est à dire en transformant **`a`** en **`b`***):
- **`8`** a été ajouté au caractère n°**`1`** du chiffré
- **`3`** a été ajouté au caractère n°**`2`** du chiffré
- **`12`** a été ajouté au caractère n°**`3`** du chiffré
- **`17`** a été ajouté au caractère n°**`4`** du chiffré
- **`24`** a été ajouté au caractère n°**`5`** du chiffré

En ajoutant **`2`** (*c'est à dire en transformant **`a`** en **`c`***), ces valeurs ajoutées au chiffré ont été doublées. Pour **`17`** et **`24`**, un modulo 25 a été appliqué : 
- 17 * 2 = 34 et 34 % 25 = 9 
- 24 * 2 = 48 et 48 % 25 = 23

Ce modulo ne sort pas de nul part, c'est simplement pour rester dans le charset de 25 caractères.

On émet alors l'hypothèse que le chiffrement se fait simplement en découpant le message par bloc de 5 et que pour chaque bloc du message, on créé un bloc du chiffré comme suit :

- **`bloc_chiffré[0] = ( 8 * bloc_message[4]) % 25`**
- **`bloc_chiffré[1] = ( 3 * bloc_message[4]) % 25`**
- **`bloc_chiffré[2] = (12 * bloc_message[4]) % 25`**
- **`bloc_chiffré[3] = (17 * bloc_message[4]) % 25`**
- **`bloc_chiffré[4] = (24 * bloc_message[4]) % 25`**

Il s'agit simplement d'un produit entre un facteur fixe et la lettre à la position n.

<br>

### Trouver tous les autres facteurs

Pour ça on va ajouter lettre par lettre la valeur **`1`** à notre message :

```python
challenge = Challenge()
print(Challenge.to_numbers(challenge.encrypt("baaaa")))
# 09 11 05 13 19
print(Challenge.to_numbers(challenge.encrypt("abaaa")))
# 04 00 06 14 21
print(Challenge.to_numbers(challenge.encrypt("aabaa")))
# 18 02 07 15 22
print(Challenge.to_numbers(challenge.encrypt("aaaba")))
# 20 01 10 16 23
print(Challenge.to_numbers(challenge.encrypt("aaaab")))
# 08 03 12 17 24
```

De là on peut émettre les équations suivantes :

```python
bloc_chiffré[0] = (  9 * bloc_message[0] +  4 * bloc_message[1] + 18 * bloc_message[2] + 20 * bloc_message[3] +  8 * bloc_message[4]) % 25
bloc_chiffré[1] = ( 11 * bloc_message[0] +  0 * bloc_message[1] +  2 * bloc_message[2] +  1 * bloc_message[3] +  3 * bloc_message[4]) % 25
bloc_chiffré[2] = (  5 * bloc_message[0] +  6 * bloc_message[1] +  7 * bloc_message[2] + 10 * bloc_message[3] + 12 * bloc_message[4]) % 25
bloc_chiffré[3] = ( 13 * bloc_message[0] + 14 * bloc_message[1] + 15 * bloc_message[2] + 16 * bloc_message[3] + 17 * bloc_message[4]) % 25
bloc_chiffré[4] = ( 19 * bloc_message[0] + 21 * bloc_message[1] + 22 * bloc_message[2] + 23 * bloc_message[3] + 24 * bloc_message[4]) % 25
```
<br>

### Vérification du chiffrement

On va recréer la fonction de chiffrement de notre côté pour vérifier notre hypothèse avec quelques comparaisons entre le chiffrement de l'oracle et le notre :

```python
def encrypt_local(msg: str):
	bloc_size = 5
	cipher = []
	for i in range(0, len(msg), bloc_size):
		bloc_message = [Challenge.CHARSET.index(c) for c in msg[i:i+bloc_size]]
		# On fait un padding pour compléter le bloc de 5 avec la lettre 'a' puisqu'elle vaut 0
		while len(bloc_message) < bloc_size:
			bloc_message.append(0)
		bloc_chiffre = [0] * 5
		bloc_chiffre[0] = ( 9 * bloc_message[0] +  4 * bloc_message[1] + 18 * bloc_message[2] + 20 * bloc_message[3] +  8 * bloc_message[4]) % 25
		bloc_chiffre[1] = (11 * bloc_message[0] +  0 * bloc_message[1] +  2 * bloc_message[2] +  1 * bloc_message[3] +  3 * bloc_message[4]) % 25
		bloc_chiffre[2] = ( 5 * bloc_message[0] +  6 * bloc_message[1] +  7 * bloc_message[2] + 10 * bloc_message[3] + 12 * bloc_message[4]) % 25
		bloc_chiffre[3] = (13 * bloc_message[0] + 14 * bloc_message[1] + 15 * bloc_message[2] + 16 * bloc_message[3] + 17 * bloc_message[4]) % 25
		bloc_chiffre[4] = (19 * bloc_message[0] + 21 * bloc_message[1] + 22 * bloc_message[2] + 23 * bloc_message[3] + 24 * bloc_message[4]) % 25
		cipher.extend(bloc_chiffre)
	return ''.join([Challenge.CHARSET[n] for n in cipher])
```

```python
challenge = Challenge()

msg = "a"
print(challenge.encrypt(msg), challenge.encrypt_local(msg))
# aaaaa aaaaa

msg = "aaaaa"
print(challenge.encrypt(msg), challenge.encrypt_local(msg))
# aaaaa aaaaa

msg = "aaaaabbbbb"
print(challenge.encrypt(msg), challenge.encrypt_local(msg))
# aaaaajrpaj aaaaajrpaj

msg = "mqpehnvjskirq"
print(challenge.encrypt(msg), challenge.encrypt_local(msg))
# dmknedjegpduehl dmknedjegpduehl
```

Ca paraît plutôt bien. Maintenant que l'on connaît les équations derrière il ne reste plus qu'à les résoudre.

<br>

### Décrypter le cipher

Ici nous avons à faire avec un problème de type SAT, donc résoluble avec z3 par exemple.

En utilisant Z3 j'ai eu quelques problèmes de performance avec Python 3.9 sur Windows, problèmes que je n'ai pas eu en 3.10 sur Windows ou en 3.9 sur Kali Linux.

```python
def solve_bloc(bloc: str):
	# Convertir les lettres en nombres
	bloc_numbers = [Challenge.CHARSET.index(c) for c in bloc]

	# Création du solver
	solver = z3.Solver()

	# Créations des variables
	variables = []
	for i in range(len(bloc)):
		variables.append(z3.Int(f"c{i}"))
		# Toutes comprises dans [0, 24]
		solver.add(variables[-1] >= 0, variables[-1] < 25)

	# Ajout des contraintes (les équations de chiffrement)
	solver.add(bloc_numbers[0] == ( 9 * variables[0] +  4 * variables[1] + 18 * variables[2] + 20 * variables[3] +  8 * variables[4]) % 25)
	solver.add(bloc_numbers[1] == (11 * variables[0] +  0 * variables[1] +  2 * variables[2] +  1 * variables[3] +  3 * variables[4]) % 25)
	solver.add(bloc_numbers[2] == ( 5 * variables[0] +  6 * variables[1] +  7 * variables[2] + 10 * variables[3] + 12 * variables[4]) % 25)
	solver.add(bloc_numbers[3] == (13 * variables[0] + 14 * variables[1] + 15 * variables[2] + 16 * variables[3] + 17 * variables[4]) % 25)
	solver.add(bloc_numbers[4] == (19 * variables[0] + 21 * variables[1] + 22 * variables[2] + 23 * variables[3] + 24 * variables[4]) % 25)

	# Vérification qu'il s'agit bien d'un problème SAT
	assert solver.check() == z3.sat

	# Récupération d'un modèle valide qui répond aux contraintes
	model = solver.model()
	
	# Transformation de ce modèle en lettres
	message = ''.join([Challenge.CHARSET[model[variable].as_long()] for variable in variables])
	return message
```

Il ne reste plus qu'à découper notre cipher en blocs de 5 et tous les passer dans notre solver :

```python
bloc_size = 5
print([solve_bloc(challenge.cipher[i:i+bloc_size]) for i in range(0, len(challenge.cipher), bloc_size)])
# ['barja', 'velma', 'assas', 'sinea']
```

Comme la lettre **`a`** sert de padding et que ça n'a pas de sens dans notre phrase ici, on la retire.

On trouve donc : **`barjavelmaassassine`** (*Barjavel m'a assassiné*)

<br>

**`FLAG : 404CTF{barjavelmaassassine}`**

{% endraw %}