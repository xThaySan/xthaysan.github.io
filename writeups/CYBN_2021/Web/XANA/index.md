{% raw %}
# Write-Up
> **title:** XANA
>
> **category:** Web
>
> **difficulty:** Moyen
>
> **point:** 100
>
> **author:** MrSheepSheep
>
> **description:**
>
> XANA a commencé son attaque, et a pris le contrôle de notre serveur.
> > ??? : Ah, ce XANA. Il barricade la porte d'entrée, mais il laisse les fenêtres grandes ouvertes.
> >
> > Jérémie : Mais enfin... Mais c'est ma place là... Qu'est-ce que vous faites là?
> >
> > ??? : Ce que je fais? Et bien, je désactive les tours.
> >
> > Jérémie : Comment ça? D'ici, sans aller sur Lyoko?
> >
> > ??? : C'est plus simple comme ça, non?
> >
> > Aelita : Sûrement, oui. Mais vous êtes qui, au juste?
> >
> > ??? : Comment ça? Vous ne l'avez pas encore compris? Mais je suis Franz Hopper, bien sûr. C'est moi qui ai donné le jour à Lyoko. Et j'ai aussi créé XANA. XANA qui a échappé à mon contrôle. Au tout début, il s'agissait d'un programme que j'avais conçu, basé sur un système multi-agent, pour contrer un projet militaire.
>
> Objectif : réalisez un retour vers le passé pour contrer XANA
>
> Indice gratuit : https://xana.cybernight-c.tf/.git/
>
> Et oui, c'est une 404 !


## Dump du .git

Même si la phrase "réalisez un retour vers le passé" aurait dû nous mettre la puce à l'oreille, il a fallu attendre un hint des admmins pour se rappeler un dossier .git pouvait être oublié sur certains serveurs.

Bref, on skip la phase analyse pour le découvrir, on sait qu'il est là, on va donc le dumper. Perso j'utilise [git-dumper](https://github.com/arthaud/git-dumper), c'est en python, c'est pratique et rapide.

On commence par installer le tool : `pip install git-dumper`.

On oublie pas d'ajouter le dossier des libs python dans le PATH pour ceux qui sont sous Windows pour pouvoir les utiliser dans la cmd (sous Kali je crois que c'est fait de base).

```cmd
> pip install git-dumper
> git-dumper https://xana.cybernight-c.tf/.git/ D:/Desktop/XANA/
```

Vous devriez récupérer tout un tas de fichiers (ceux à jour) :

![Dossier récupéré](images/folder.png)


## Analyse des fichiers actuels

On commence par ouvrir **index.php** pour voir ce qui se passe sur le serveur (le fichier est dans /files).

Une chose intéressante :

```php
<?php
  $this->xana = getenv('FLAG');
?>
```
On voit que le flag est sauvegardé dans l'attribut **xana**.

```php
<?php if (!$Lyoko->isHacked): ?>
	<head>
		<title>Welcome to Lyoko</title>
		<style>
			body {
				background-color: darkblue;
				color: white;
				font-family: 'Courier New', sans-serif;
			}
		</style>
	</head>
	<body>
		Retour vers le passé ! XANA a été repoussé, et le monde est sauvé... pour l'instant.<br>
		<?= $Lyoko->xana; ?>
```

Dans la partie HTML, on peut y voir qu'il faut que Lioko.isHacked soit faux pour récupérer notre flag.

```php
<?php
  $this->world_shutdown = new DateTime(date("Y-m-d H:i:s", strtotime("3 December 2021 8:00")));
  $this->world_init = clone $this->world_shutdown;
  $this->world_init->modify('-24 hours');
  $this->identity = $this->get_identity();
  $this->isHacked = $this->identity["iat"] > $this->world_init->getTimestamp();
?>
```

Et pour que isHacked soit faux, il faut que **iat** de **identity** soit inférieur au **2 Décembre 2021 à 8h**. Mais du coup regardons **get_identity**.

```php
<?php
  public function get_identity() {
    try {
      # Si pas de JWT dans les cookie, en créé un
      if (!isset($_COOKIE["lyokoToken"])) {
        $payload = $this->generate_identity();
      }
      # Sinon le récupère
      else {
        $payload = $_COOKIE["lyokoToken"];
      }
      # Cast les infos dans le corps du JWT en vérifiant la signature.
      $jwt = (array) JWT::decode($payload, new Key($this->key, "HS256"));
    } catch (Exception $e) {
      echo $e;
      die('XANA CANNOT BE HACKED');
    }
  return $jwt;
  }
?>
```

On voit donc que pour modifier le **iat** de notre JWT, il faut soit contourner la vérification de signature, soit connaître la clé pour forger des JWT valides.

Intéressons nous à la deuxième solution (la première ne fonctionne pas, croyez moi).


## Analyse des commits

Ce qui nous intéresse : **les précédents commits**, on va essayé de **recover** les anciens en espérant tomber sur quelque chose de croustillant (**genre une clé**). On se place alors dans le dossier principal (*XANA selon ma cmd plutôt*) et l'on regarde les **logs**.

```
> git log
commit 504226cce9c480519606d1e99ac3413ea8b72450 (HEAD -> main)
Author: XANA <XANA@lyoko.cybn>
Date:   Fri Dec 3 15:59:26 2021 +0100

    RUN XANA_PATCH_0x90d266

commit 45f042fd921100a8faff50da1b5a5db0d296e074
Author: XANA <XANA@lyoko.cybn>
Date:   Fri Dec 3 15:48:39 2021 +0100

    RUN XANA_PATCH_0x28e33f1

commit c3e2f8ac6f04dfadb357cc247266d83b6f6cb6eb
Author: XANA <XANA@lyoko.cybn>
Date:   Fri Dec 3 14:19:52 2021 +0100

    RUN XANA_PATCH_0x28371

commit ca49a198627260eefb73e0a88cfde962d2604717
Author: XANA <XANA@lyoko.cybn>
Date:   Fri Dec 3 14:18:24 2021 +0100

    INITIALIZE XANA PROCESS

commit 99a6fc9aaa80598f040329195e0c85cc545a9e94
Author: Jeremy <jeremy@lyoko.cybn>
Date:   Fri Dec 3 11:49:22 2021 +0100

    Initial commit
```

On peut commencer à regarder si on trouve des choses intéressantes, en commençant par **INITIALIZE XANA PROCESS** puisque qu'on voit 3 patch ensuite, on se doute que le robot à fait des dingueries qu'il a dû corriger.


## Analyse du commit INITIALIZE

```
> git show ca49a198627260eefb73e0a88cfde962d2604717

[...]

class Lyoko {
-       public $key;
+       public $key = "tBDNlSUPfkiIOBLS5J8yVTprzZbQXVZm";
        public $xana;
        public $world_init;

        public function __construct() {
+               $this->xana = getenv('FLAG');
+               $this->world_shutdown = new DateTime(date("Y-m-d H:i:s", strtotime("3 December 2021 8:00")));
+               $this->world_init = clone $this->world_shutdown;
+               $this->world_init->modify('-24 hours');
        }
```

On voit que la $key servant à forger les JWT token est présente en clair, on peut tenter de l'utiliser pour forger nos propres tokens en espérant qu'elle n'est pas changée entre temps.


## Création d'un JWT

Un JWT, c'est :
- un body = base64(header).base64(payload)
- une signature = HMACSHA256(body, key)

On connait une clé, on connaît le body, plus qu'à modifier tout ça pour que ça fonctionne.

Première étape : récupérer un token créé par le site, pour ça on va dessus et on copie colle notre cookie, ici j'ai celui là : `eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpYXQiOjE2MzkyNDE3MTR9.rgiHY0E6vUgTFg9OEaNBuv0NO0v9CruEVDcxxQG7E60`

Ensuite même pas besoin de coder :
1. on va sur **[jwt.io](https://jwt.io)**
2. on colle notre **jeton** dans la partie de gauche.
3. on colle notre secret **tBDNlSUPfkiIOBLS5J8yVTprzZbQXVZm** dans la partie signature.
4. on change le iat pour un nombre **inférieur à 1638432001**
5. on modifie notre cookie et l'on refresh la page

![Création du JWT](images/jwt.png)

![Récupération du flag](images/web.png)

**Flag : CYBN{R3t0ur_veR5_l3_P4S5e_!}** 
{% endraw %}
