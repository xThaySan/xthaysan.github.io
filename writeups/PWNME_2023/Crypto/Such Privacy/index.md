---
layout: default
type: writeup
title: Such Privacy
category: Cryptography
point: 297
files: ["hint.json", "main.js", "package.json"]
---

{% raw %}
> **title:** Such Privacy
>
> **category:** Cryptography
>
> **difficulty:** Medium
>
> **point:** 297
>
> **author:** BrutiNicolas
>
> **description:**
>
> Is everything cryptosecure ?
> 
> flag format: PWNME{uppercase of the md5 of flag.json}
> 
> You can use md5sum flag.json to recover the md5 hash of flag.json
> 

## Solution

On dispose de 3 fichiers et nous travaillons sur du NodeJS.

L'un des fichiers, nommé `hint.json` contient une liste de 5 valeurs.

Dans le `main.js`, on voit que la première opération faite est de créé ce `hint.json` :

```js
writeFileSync('hint.json', JSON.stringify(new Array(5).fill(true).map(Math.random), null, 2), {encoding: 'utf-8'})
```

Pour ce faire, 5 valeur aléatoires son générées. Avec un peu de recherche sur les PRNG, on tombe sur des outils de prédictions concernant le moteur V8 de Google (*c'est ce qui est utilisé par NodeJS*).

**[https://github.com/PwnFunction/v8-randomness-predictor](https://github.com/PwnFunction/v8-randomness-predictor)**

Grâce à seulement quelques valeurs, il est possible d'en déduire l'état du générateur et ainsi en préduire la suite.

En reprenant le script fournit sur le Github et en remplaçant par nos valeurs, on peut vérifier que cela fonctionne bien :

```python
# Valeurs du hint.json
sequence = [
  0.018576535281384476,
  0.4830061962355321,
  0.5884432689111756,
  0.10435533137397934,
  # 0.636142394871601 <- valeur attendue 
]

# [...]

print(next_sequence)

# 0.636142394871601 <- valeur prédite 
```

Le script fonctionne bien, merci à son auteur.

Générons alors le 6ème aléatoire :

```python
# Valeurs du hint.json
sequence = [
  0.018576535281384476,
  0.4830061962355321,
  0.5884432689111756,
  0.10435533137397934,
  0.636142394871601
]

# [...]

print(next_sequence)

# 0.5440681218850456 <- valeur prédite 
```

On connaît donc la valeur utilisée lors de l'exécution du `main.js` à la ligne 8 :

```js
 const seedKey = arrayify(BigNumber.from(keccak256(Math.floor(Math.random() * Number.MAX_SAFE_INTEGER))).add(BigNumber.from('0x260026002600260026002600').mul(0x2600n)).mod(CURVE.n));
```

Devient :

```js
 const seedKey = arrayify(BigNumber.from(keccak256(Math.floor(0.5440681218850456 * Number.MAX_SAFE_INTEGER))).add(BigNumber.from('0x260026002600260026002600').mul(0x2600n)).mod(CURVE.n));
```

Il ne reste plus qu'à lancer le `main.js` de notre côté pour générer le même `flag.json` :

```
> node main.js
```

Ensuite on calcule le md5sum `flag.json` (ici en PowerShell) :

```
> Get-FileHash ./flag.json -Algorithm MD5 | Format-List

Algorithm : MD5
Hash      : 09AF217F399CDE1D07096D9D4317A593
Path      : *********\Such Privacy\flag.json
```


**`FLAG : PWNME{09AF217F399CDE1D07096D9D4317A593}`**

{% endraw %}
