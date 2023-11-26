---
layout: default
type: writeup
title: "Invest Now !"
category: osint
point: 100
files: []
---

<!-- {% raw %} -->

<div class="info">
<p class="title">
	<span class="name">titre:</span>
	Invest Now !
</p>
<p class="category">
	<span class="name">categorie:</span>
	OSINT
</p>
<p class="difficulty">
	<span class="name">difficulté:</span>
	Moyen
</p>
<p class="points">
	<span class="name">points:</span>
	100
</p>
<div class="description">
	<span class="name">description:</span>
	<p>
		InvestNow! est une application de gestion de wallet Bitcoin : elle permet d'acheter et d'échanger des Bitcoins à tout moment pour spéculer sur la montée des cours. De nombreuses personnes sont devenues riches grâce à ce site!
	</p>
	<p>
		Vous avez réussi à développer un script pour détecter si une adresse mail est inscrite sur ce site. Après quelques heures de tests avec des bases de données trouvées sur le darknet, une première adresse mail est ressortie : emily.clement.hevre@malice.fr.
	</p>
	<p>
		Volez la clé privée du compte de Emily Clément Hevré, il y a peut-être des milliers d'euros à la clé !
	</p>
	<p>
		Le flag sera à soumettre sous la forme MALICE{clé}
	</p>
</div> 
<p class="connection">
	<span class="name">connection:</span>
	<a href="http://investnow3.chall.malicecyber.com/">http://investnow3.chall.malicecyber.com/</a>
</p> 
</div>

## Solution

Direction **[Yandex](https://yandex.com/search/?text=emily.clement.hevre%40malice.fr)** pour chercher cet email :

![Résultat de la recherche sur Yandex](images/yandex.png)

On tombe sur un compte **[Twitter](https://twitter.com/emily_clement_h)**, sur lequel on va trouver sa chanson préférée : **All Night** de Parov Stellar

![Compte twitter et chanson préférée](images/twitter.png)

Sur le compte **[Facebook](https://www.facebook.com/emily.clementhevre/about_overview)** sorti également par Yandex, on trouve son lycée, qui est surement situé dans sa ville de naissance :

![Compte Facebook et ville de naissance](images/facebook.png)

Il ne nous manque plus qu'un **[Instagram](https://www.instagram.com/emilyclementhevre/)** pour obtenir le trio magique, et effectivement en cherchant son nom complet dessus on trouve son compte ainsi qu'un animal totem :

![Compte Instagram et animal préféré](images/instagram.png)

On peut maintenant répondre aux questions de sécurité pour récupérer son compte :

![Questions de sécurité](images/recover.png)

On a maintenant accès à son wallet crypto et donc au flag :

![Questions de sécurité](images/flag.png)

<span class="flag">`FLAG : MALICE{5JWVBcrWBS2F98nRai8BjDKt8EBQ8Jm9CtMEQD1xkpGNwqFFGgX}`</span>

<!-- {% endraw %} -->
