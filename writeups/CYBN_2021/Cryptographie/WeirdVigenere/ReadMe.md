# Write-Up
> **title:** WeirdVigenere
>
> **category:** Cryptographie
>
> **difficulty:** Difficile
>
> **point:** 100
>
> **author:** Langley
>
> **description:**
>
> Je ne pense pas que vous arriverez à trouver un outil en ligne qui le déchiffre pour vous ...

## Trouver la longueur de la clé

D'après le titre, il s'agit d'un chiffrement de Vigenère, une méthode de substitution assez basique et simple à casser de nos jours.

La première étape est de trouver la longueur potentielle de la clé utilisée. Pour ça, l'objectif est de trouver des **n-grammes qui se répètent** et en regardant la distance qui les sépare, d'essayer de deviner la longueur de la clé. Si vous ne comprenez pas par quelle magie noire cela fonctionne, pour des explications plus détaillées je vous renvoie vers cette page **[Wikipédia](https://fr.wikipedia.org/wiki/Cryptanalyse_du_chiffre_de_Vigenère)**.

En python cela donne ça :

```python
with open('./flag.crypt', encoding='utf-8') as f:
    text = f.read()

def distance(data, size):
    distances = []
    range_max = len(data)-(size-1)
    for pointer in range(0, range_max):
        for compare in range(pointer+1, range_max):
            if data[pointer:pointer+size] == data[compare:compare+size]:
                distances.append(compare - pointer)
    return set(distances)
```

Commençons en essayant de trouver les 10-grammes qui se répètent, on obtient cette liste de distances :

```python
print(distance(text, 10))
```

```
Output: {768, 1440, 1122, 294, 108, 2064, 276, 1014, 762, 1662}
```

On pourrait tester avec des n-grammes plus grands pour rendre le résultat plus probable d'un point de vu statistique ou encore mettre en place une marge d'erreur pour d'éventuelles répétitions par simple coïncidence, mais cela suffira ici.

On cherche maintenant le **PGCD** entre toutes ces distances :

```python
from math import gcd
print(gcd(*list(distance(text, 10))))
```

```
Output: 6
```

Il y a de grandes chances que **la clé utilisée soit de longueur 6**.


## Bruteforce de la clé

Maintenant que nous connaissons la longueur, faire un bruteforce bête et méchant serait impossible, il y a **256^6 possibilités** (*environ 281 mille milliairds*). On peut cependant récupérer les caractères 6 à 6 et appliquer un césar par bruteforce dessus pour connaître avec quelles valeurs nous obtenons quelque chose de lisible.

Pour être plus précis, avec notre clés de 6, nous allons construire 6 groupes. Exemple avec notre premier groupe, **appelons le groupe 0** : on va commencer au caractère 0 et de 6 en 6 ajouter les caractères, c'est à dire le 0, 6, 12, 18...

Pour le second groupe, **noté groupe 1**, on commence au caractère 1 et toujours de 6 en 6 on ajoute, c'est à dire le 1, 7, 13, 19...

Une fois qu'on a nos groupes, on va tester sur chacun les 256 César possibles. On prend alors le résultat du César et l'on regarde si le texte déchiffré ressemble à quelque chose de plausible.

On fait ça pour nous 6 groupes, en python ça nous donne ça :

```python
from string import printable
key_length = 6


# Retourne vrai si le caractères est considéré comme valide
def is_valid_char(c):
    return c.isalnum() or c in printable or 32 <= ord(c) <= 126


# Retourne vrai si le groupe décodé parait plausible (il faut que + de 99% de ses caractères soit valides
def is_valid_decoded(decoded):
    count_valid = len([True for c in decoded if is_valid_char(c)])
    return count_valid / len(decoded) >= .99


# Génère les groupes en fonction de la taille de la clé
groups = ['' for i in range(key_length)]
for i in range(len(text)):
    groups[i % key_length] += data[i]


# Applique un César en fonction d'une clé
def decode_group(group, key):
    return ''.join([chr((ord(c) - key) % 256) for c in group])


# Teste les 256 clés sur un groupe et retourne les clés potetielles
def bruteforce(group):
    possibilities = []
    for i in range(256):
        decoded = decode_group(group, i)
        possible = True
        # Si tous les caractères décodés sont des lettre, chiffres ou ponctuations
        # On ajoute la clé comme possibilité
        if is_valid_decoded(decoded):
            possibilities.append(i)
    return possibilities

# Pour chaque groupe, détermine les clés plausibles et affiche le texte décodé associé pour une vérification humaine
for i in range(len(groups)):
    print(f"Groupe {i}")
    possibilities = bruteforce(groups[i])
    print(i, possibilities)
    for key in possibilities:
        print(f"{key} :", decode_group(groups[i], key).encode())
    print(f"\n")
```

```
Output:
Groupe 0
0 [193, 194, 195, 196, 197, 198, 199, 200, 201]
193 : b'Kmqmp(o(v\xc3\xb0(z(}}xxqim(mm(((vxw{q\xc3\xb2m\xc3\xb1(m(z4zm}|mn|(jmmp({}|(lwwvm\xc3\xb1(|{/{nvk(v|\xc3\xb1(mnvvpyKi(z^z|k(r}(zi((\xc3\xb1\xc3\xb1(>xmx(wx}{|tK;no\\=v\xc2\x85muv({(nmq|`qJ(o4tzv(kmxwm{|(p{(@(mir(lkmi}{wzl~|({}=mn|wtq(]\xc3\xb1zmzv{w/|/p(xznwm\xc3\xa8}i(}wmzt(m}(qvl|{ti}~(m((k\xc3\xb1niy\xc2\x81(\xc3\xb1({iv(z|zi}\xc3\xb1znm{lzT}zlvm}\xc3\xa8zmivx(uzz{ljkpvumq(qv(u(wmt(v{lkxily{k(w\xc2\x80\xc3\xb1v(|v(mumz|tt}tni(|(i}mtKtymvzkm(i\xc3\xb1{qp{jv(\xc3\xb1(i}mzilkt|i4m|\xc3\xb1q(q|i(onK{im|\xc3\xb1pumvvi/{z6~\xc3\xb1z{kzqm{k\xc3\xb1(|lm(i(\xc3\xb1(4({l(6m(}um({(m|{t(}ilnvz(zx|t(~iw(smqvxqmm|mp{qml}vlvkk|(q(zvxnmzmkm}(B\x7flo7z^Km'
194 : b"Jlplo'n'u\xc3\xaf'y'||wwphl'll'''uwvzp\xc3\xb1l\xc3\xb0'l'y3yl|{lm{'illo'z|{'kvvul\xc3\xb0'{z.zmuj'u{\xc3\xb0'lmuuoxJh'y]y{j'q|'yh''\xc3\xb0\xc3\xb0'=wlw'vw|z{sJ:mn[<u\xc2\x84ltu'z'mlp{_pI'n3syu'jlwvlz{'oz'?'lhq'kjlh|zvyk}{'z|<lm{vsp'\\\xc3\xb0ylyuzv.{.o'wymvl\xc3\xa7|h'|vlys'l|'puk{zsh|}'l''j\xc3\xb0mhx\xc2\x80'\xc3\xb0'zhu'y{yh|\xc3\xb0ymlzkyS|ykul|\xc3\xa7ylhuw'tyyzkijoutlp'pu't'vls'uzkjwhkxzj'v\x7f\xc3\xb0u'{u'ltly{ss|smh'{'h|lsJsxluyjl'h\xc3\xb0zpoziu'\xc3\xb0'h|lyhkjs{h3l{\xc3\xb0p'p{h'nmJzhl{\xc3\xb0otluuh.zy5}\xc3\xb0yzjyplzj\xc3\xb0'{kl'h'\xc3\xb0'3'zk'5l'|tl'z'l{zs'|hkmuy'yw{s'}hv'rlpuwpll{lozplk|ukujj{'p'yuwmlyljl|'A~kn6y]Jl"
195 : b'Ikokn&m&t\xc3\xae&x&{{vvogk&kk&&&tvuyo\xc3\xb0k\xc3\xaf&k&x2xk{zklz&hkkn&y{z&juutk\xc3\xaf&zy-ylti&tz\xc3\xaf&klttnwIg&x\\xzi&p{&xg&&\xc3\xaf\xc3\xaf&<vkv&uv{yzrI9lmZ;t\xc2\x83kst&y&lkoz^oH&m2rxt&ikvukyz&ny&>&kgp&jikg{yuxj|z&y{;klzuro&[\xc3\xafxkxtyu-z-n&vxluk\xc3\xa6{g&{ukxr&k{&otjzyrg{|&k&&i\xc3\xaflgw\x7f&\xc3\xaf&ygt&xzxg{\xc3\xafxlkyjxR{xjtk{\xc3\xa6xkgtv&sxxyjhintsko&ot&s&ukr&tyjivgjwyi&u~\xc3\xaft&zt&kskxzrr{rlg&z&g{krIrwktxik&g\xc3\xafyonyht&\xc3\xaf&g{kxgjirzg2kz\xc3\xafo&ozg&mlIygkz\xc3\xafnskttg-yx4|\xc3\xafxyixokyi\xc3\xaf&zjk&g&\xc3\xaf&2&yj&4k&{sk&y&kzyr&{gjltx&xvzr&|gu&qkotvokkzknyokj{tjtiiz&o&xtvlkxkik{&@}jm5x\\Ik'
196 : b'Hjnjm%l%s\xc3\xad%w%zzuunfj%jj%%%sutxn\xc3\xafj\xc3\xae%j%w1wjzyjky%gjjm%xzy%ittsj\xc3\xae%yx,xksh%sy\xc3\xae%jkssmvHf%w[wyh%oz%wf%%\xc3\xae\xc3\xae%;uju%tuzxyqH8klY:s\xc2\x82jrs%x%kjny]nG%l1qws%hjutjxy%mx%=%jfo%ihjfzxtwi{y%xz:jkytqn%Z\xc3\xaewjwsxt,y,m%uwktj\xc3\xa5zf%ztjwq%jz%nsiyxqfz{%j%%h\xc3\xaekfv~%\xc3\xae%xfs%wywfz\xc3\xaewkjxiwQzwisjz\xc3\xa5wjfsu%rwwxighmsrjn%ns%r%tjq%sxihufivxh%t}\xc3\xaes%ys%jrjwyqqzqkf%y%fzjqHqvjswhj%f\xc3\xaexnmxgs%\xc3\xae%fzjwfihqyf1jy\xc3\xaen%nyf%lkHxfjy\xc3\xaemrjssf,xw3{\xc3\xaewxhwnjxh\xc3\xae%yij%f%\xc3\xae%1%xi%3j%zrj%x%jyxq%zfiksw%wuyq%{ft%pjnsunjjyjmxnjizsishhy%n%wsukjwjhjz%?|il4w[Hj'
197 : b'Gimil$k$r\xc3\xac$v$yyttmei$ii$$$rtswm\xc3\xaei\xc3\xad$i$v0viyxijx$fiil$wyx$hssri\xc3\xad$xw+wjrg$rx\xc3\xad$ijrrluGe$vZvxg$ny$ve$$\xc3\xad\xc3\xad$:tit$stywxpG7jkX9r\xc2\x81iqr$w$jimx\\mF$k0pvr$gitsiwx$lw$<$ien$hgieywsvhzx$wy9ijxspm$Y\xc3\xadvivrws+x+l$tvjsi\xc3\xa4ye$ysivp$iy$mrhxwpeyz$i$$g\xc3\xadjeu}$\xc3\xad$wer$vxvey\xc3\xadvjiwhvPyvhriy\xc3\xa4viert$qvvwhfglrqim$mr$q$sip$rwhgtehuwg$s|\xc3\xadr$xr$iqivxppypje$x$eyipGpuirvgi$e\xc3\xadwmlwfr$\xc3\xad$eyivehgpxe0ix\xc3\xadm$mxe$kjGweix\xc3\xadlqirre+wv2z\xc3\xadvwgvmiwg\xc3\xad$xhi$e$\xc3\xad$0$wh$2i$yqi$w$ixwp$yehjrv$vtxp$zes$oimrtmiixilwmihyrhrggx$m$vrtjivigiy$>{hk3vZGi'
198 : b'Fhlhk#j#q\xc3\xab#u#xxssldh#hh###qsrvl\xc3\xadh\xc3\xac#h#u/uhxwhiw#ehhk#vxw#grrqh\xc3\xac#wv*viqf#qw\xc3\xac#hiqqktFd#uYuwf#mx#ud##\xc3\xac\xc3\xac#9shs#rsxvwoF6ijW8q\xc2\x80hpq#v#ihlw[lE#j/ouq#fhsrhvw#kv#;#hdm#gfhdxvrugyw#vx8hiwrol#X\xc3\xacuhuqvr*w*k#suirh\xc3\xa3xd#xrhuo#hx#lqgwvodxy#h##f\xc3\xacidt|#\xc3\xac#vdq#uwudx\xc3\xacuihvguOxugqhx\xc3\xa3uhdqs#puuvgefkqphl#lq#p#rho#qvgfsdgtvf#r{\xc3\xacq#wq#hphuwooxoid#w#dxhoFothqufh#d\xc3\xacvlkveq#\xc3\xac#dxhudgfowd/hw\xc3\xacl#lwd#jiFvdhw\xc3\xackphqqd*vu1y\xc3\xacuvfulhvf\xc3\xac#wgh#d#\xc3\xac#/#vg#1h#xph#v#hwvo#xdgiqu#uswo#ydr#nhlqslhhwhkvlhgxqgqffw#l#uqsihuhfhx#=zgj2uYFh'
199 : b'Egkgj"i"p\xc3\xaa"t"wwrrkcg"gg"""prquk\xc3\xacg\xc3\xab"g"t.tgwvghv"dggj"uwv"fqqpg\xc3\xab"vu)uhpe"pv\xc3\xab"ghppjsEc"tXtve"lw"tc""\xc3\xab\xc3\xab"8rgr"qrwuvnE5hiV7p\x7fgop"u"hgkvZkD"i.ntp"egrqguv"ju":"gcl"fegcwuqtfxv"uw7ghvqnk"W\xc3\xabtgtpuq)v)j"rthqg\xc3\xa2wc"wqgtn"gw"kpfvuncwx"g""e\xc3\xabhcs{"\xc3\xab"ucp"tvtcw\xc3\xabthguftNwtfpgw\xc3\xa2tgcpr"ottufdejpogk"kp"o"qgn"pufercfsue"qz\xc3\xabp"vp"gogtvnnwnhc"v"cwgnEnsgpteg"c\xc3\xabukjudp"\xc3\xab"cwgtcfenvc.gv\xc3\xabk"kvc"ihEucgv\xc3\xabjogppc)ut0x\xc3\xabtuetkgue\xc3\xab"vfg"c"\xc3\xab"."uf"0g"wog"u"gvun"wcfhpt"trvn"xcq"mgkprkggvgjukgfwpfpeev"k"tprhgtgegw"<yfi1tXEg'
200 : b'Dfjfi!h!o\xc3\xa9!s!vvqqjbf!ff!!!oqptj\xc3\xabf\xc3\xaa!f!s-sfvufgu!cffi!tvu!eppof\xc3\xaa!ut(tgod!ou\xc3\xaa!fgooirDb!sWsud!kv!sb!!\xc3\xaa\xc3\xaa!7qfq!pqvtumD4ghU6o~fno!t!gfjuYjC!h-mso!dfqpftu!it!9!fbk!edfbvtpsewu!tv6fgupmj!V\xc3\xaasfsotp(u(i!qsgpf\xc3\xa1vb!vpfsm!fv!joeutmbvw!f!!d\xc3\xaagbrz!\xc3\xaa!tbo!susbv\xc3\xaasgftesMvseofv\xc3\xa1sfboq!nsstecdionfj!jo!n!pfm!otedqbertd!py\xc3\xaao!uo!fnfsummvmgb!u!bvfmDmrfosdf!b\xc3\xaatjitco!\xc3\xaa!bvfsbedmub-fu\xc3\xaaj!jub!hgDtbfu\xc3\xaainfoob(ts/w\xc3\xaastdsjftd\xc3\xaa!uef!b!\xc3\xaa!-!te!/f!vnf!t!futm!vbegos!squm!wbp!lfjoqjffufitjfevoeoddu!j!soqgfsfdfv!;xeh0sWDf'
201 : b"Ceieh g n\xc3\xa8 r uuppiae ee   nposi\xc3\xaae\xc3\xa9 e r,reuteft beeh sut doone\xc3\xa9 ts'sfnc nt\xc3\xa9 efnnhqCa rVrtc ju ra  \xc3\xa9\xc3\xa9 6pep opustlC3fgT5n}emn s feitXiB g,lrn cepoest hs 8 eaj dceausordvt su5eftoli U\xc3\xa9rernso't'h prfoe\xc3\xa0ua uoerl eu indtslauv e  c\xc3\xa9faqy \xc3\xa9 san rtrau\xc3\xa9rfesdrLurdneu\xc3\xa0reanp mrrsdbchnmei in m oel nsdcpadqsc ox\xc3\xa9n tn emertllulfa t auelClqenrce a\xc3\xa9sihsbn \xc3\xa9 aueradclta,et\xc3\xa9i ita gfCsaet\xc3\xa9hmenna'sr.v\xc3\xa9rscriesc\xc3\xa9 tde a \xc3\xa9 , sd .e ume s etsl uadfnr rptl vao keinpieetehsiedundncct i rnpfereceu :wdg/rVCe"


Groupe 1
1 [93, 94, 95, 96, 97, 98, 99]
93 : b'n&m\x10ojkk&sikvhzunwo&r&yivyzut&3zskjzjk&gtt\xc3\xae&x&g\xc3\xaf&&ojg-oigsszzzxkogkxikk&giyylzug{ktikok\xc3\xaf\xc3\xafruyLoywv&zk9{zuolri\xc3\xaf\xc3\xafg_ex99ze\x10ysy^ok\xc3\xaf&vk\\\xc3\xaerjk&koz\xc3\xa6nsrs/&xjo&k<z&o\xc3\xa6sknstk&{gkgzHu&;&x&jguit&\xc3\xaf&gz&x{&{xVu&xz~&kitzt&kgvlkyz4k2&\xc3\xaf&kgk{zyn4g|{&{xu&mzjkzkozy&xy&ykk~kg-ty&&{o&rjkk&{kuuk&ky&izujhws&\xc3\xafjzok\xc3\xafgrk{&nj{zz\xc3\xafyozryk&&kkuxg&tykw&k&k--{t{&nssrz4k&qr&{zkixx&okrk&y&v&vuitotskln&my\xc3\xaf&ok&\xc3\xaekog&g&gi\xc3\xaf&gzkygnyrnktO&v&y&uvkt&&vk\xc3\xaf&{\xc3\xafg~k&kjzz{lzznok&&jx&jQotr&gy&&kyt&yy\xc3\xafxjkikuk^\xc3\xaeykirly&xn4xn5oo5Iko9\x10'
94 : b'm%l\x0fnijj%rhjugytmvn%q%xhuxyts%2yrjiyij%fss\xc3\xad%w%f\xc3\xae%%nif,nhfrryyywjnfjwhjj%fhxxkytfzjshjnj\xc3\xae\xc3\xaeqtxKnxvu%yj8zytnkqh\xc3\xae\xc3\xaef^dw88yd\x0fxrx]nj\xc3\xae%uj[\xc3\xadqij%jny\xc3\xa5mrqr.%win%j;y%n\xc3\xa5rjmrsj%zfjfyGt%:%w%ifths%\xc3\xae%fy%wz%zwUt%wy}%jhsys%jfukjxy3j1%\xc3\xae%jfjzyxm3f{z%zwt%lyijyjnyx%wx%xjj}jf,sx%%zn%qijj%zjttj%jx%hytigvr%\xc3\xaeiynj\xc3\xaefqjz%mizyy\xc3\xaexnyqxj%%jjtwf%sxjv%j%j,,zsz%mrrqy3j%pq%zyjhww%njqj%x%u%uthsnsrjkm%lx\xc3\xae%nj%\xc3\xadjnf%f%fh\xc3\xae%fyjxfmxqmjsN%u%x%tujs%%uj\xc3\xae%z\xc3\xaef}j%jiyyzkyymnj%%iw%iPnsq%fx%%jxs%xx\xc3\xaewijhjtj]\xc3\xadxjhqkx%wm3wm4nn4Hjn8\x0f'
95 : b'l$k\x0emhii$qgitfxslum$p$wgtwxsr$1xqihxhi$err\xc3\xac$v$e\xc3\xad$$mhe+mgeqqxxxvimeivgii$egwwjxseyirgimi\xc3\xad\xc3\xadpswJmwut$xi7yxsmjpg\xc3\xad\xc3\xade]cv77xc\x0ewqw\\mi\xc3\xad$tiZ\xc3\xacphi$imx\xc3\xa4lqpq-$vhm$i:x$m\xc3\xa4qilqri$yeiexFs$9$v$hesgr$\xc3\xad$ex$vy$yvTs$vx|$igrxr$ietjiwx2i0$\xc3\xad$ieiyxwl2ezy$yvs$kxhiximxw$vw$wii|ie+rw$$ym$phii$yissi$iw$gxshfuq$\xc3\xadhxmi\xc3\xadepiy$lhyxx\xc3\xadwmxpwi$$iisve$rwiu$i$i++yry$lqqpx2i$op$yxigvv$mipi$w$t$tsgrmrqijl$kw\xc3\xad$mi$\xc3\xacime$e$eg\xc3\xad$exiwelwplirM$t$w$stir$$ti\xc3\xad$y\xc3\xade|i$ihxxyjxxlmi$$hv$hOmrp$ew$$iwr$ww\xc3\xadvhigisi\\\xc3\xacwigpjw$vl2vl3mm3Gim7\x0e'
96 : b'k#j\rlghh#pfhsewrktl#o#vfsvwrq#0wphgwgh#dqq\xc3\xab#u#d\xc3\xac##lgd*lfdppwwwuhldhufhh#dfvviwrdxhqfhlh\xc3\xac\xc3\xacorvIlvts#wh6xwrliof\xc3\xac\xc3\xacd\\bu66wb\rvpv[lh\xc3\xac#shY\xc3\xabogh#hlw\xc3\xa3kpop,#ugl#h9w#l\xc3\xa3phkpqh#xdhdwEr#8#u#gdrfq#\xc3\xac#dw#ux#xuSr#uw{#hfqwq#hdsihvw1h/#\xc3\xac#hdhxwvk1dyx#xur#jwghwhlwv#uv#vhh{hd*qv##xl#oghh#xhrrh#hv#fwrgetp#\xc3\xacgwlh\xc3\xacdohx#kgxww\xc3\xacvlwovh##hhrud#qvht#h#h**xqx#kppow1h#no#xwhfuu#lhoh#v#s#srfqlqphik#jv\xc3\xac#lh#\xc3\xabhld#d#df\xc3\xac#dwhvdkvokhqL#s#v#rshq##sh\xc3\xac#x\xc3\xacd{h#hgwwxiwwklh##gu#gNlqo#dv##hvq#vv\xc3\xacughfhrh[\xc3\xabvhfoiv#uk1uk2ll2Fhl6\r'
97 : b'j"i\x0ckfgg"oegrdvqjsk"n"ueruvqp"/vogfvfg"cpp\xc3\xaa"t"c\xc3\xab""kfc)kecoovvvtgkcgtegg"ceuuhvqcwgpegkg\xc3\xab\xc3\xabnquHkusr"vg5wvqkhne\xc3\xab\xc3\xabc[at55va\x0cuouZkg\xc3\xab"rgX\xc3\xaanfg"gkv\xc3\xa2jono+"tfk"g8v"k\xc3\xa2ogjopg"wcgcvDq"7"t"fcqep"\xc3\xab"cv"tw"wtRq"tvz"gepvp"gcrhguv0g."\xc3\xab"gcgwvuj0cxw"wtq"ivfgvgkvu"tu"uggzgc)pu""wk"nfgg"wgqqg"gu"evqfdso"\xc3\xabfvkg\xc3\xabcngw"jfwvv\xc3\xabukvnug""ggqtc"pugs"g"g))wpw"joonv0g"mn"wvgett"kgng"u"r"rqepkpoghj"iu\xc3\xab"kg"\xc3\xaagkc"c"ce\xc3\xab"cvgucjunjgpK"r"u"qrgp""rg\xc3\xab"w\xc3\xabczg"gfvvwhvvjkg""ft"fMkpn"cu""gup"uu\xc3\xabtfgegqgZ\xc3\xaaugenhu"tj0tj1kk1Egk5\x0c'
98 : b'i!h\x0bjeff!ndfqcupirj!m!tdqtupo!.unfeuef!boo\xc3\xa9!s!b\xc3\xaa!!jeb(jdbnnuuusfjbfsdff!bdttgupbvfodfjf\xc3\xaa\xc3\xaamptGjtrq!uf4vupjgmd\xc3\xaa\xc3\xaabZ`s44u`\x0btntYjf\xc3\xaa!qfW\xc3\xa9mef!fju\xc3\xa1inmn*!sej!f7u!j\xc3\xa1nfinof!vbfbuCp!6!s!ebpdo!\xc3\xaa!bu!sv!vsQp!suy!fdouo!fbqgftu/f-!\xc3\xaa!fbfvuti/bwv!vsp!huefufjut!st!tffyfb(ot!!vj!meff!vfppf!ft!dupecrn!\xc3\xaaeujf\xc3\xaabmfv!ievuu\xc3\xaatjumtf!!ffpsb!otfr!f!f((vov!innmu/f!lm!vufdss!jfmf!t!q!qpdojonfgi!ht\xc3\xaa!jf!\xc3\xa9fjb!b!bd\xc3\xaa!buftbitmifoJ!q!t!pqfo!!qf\xc3\xaa!v\xc3\xaabyf!feuuvguuijf!!es!eLjom!bt!!fto!tt\xc3\xaasefdfpfY\xc3\xa9tfdmgt!si/si0jj0Dfj4\x0b'
99 : b"h g\nidee mcepbtohqi l scpston -tmedtde ann\xc3\xa8 r a\xc3\xa9  ida'icammtttreiaercee acssftoauenceie\xc3\xa9\xc3\xa9losFisqp te3utoiflc\xc3\xa9\xc3\xa9aY_r33t_\nsmsXie\xc3\xa9 peV\xc3\xa8lde eit\xc3\xa0hmlm) rdi e6t i\xc3\xa0mehmne uaeatBo 5 r daocn \xc3\xa9 at ru urPo rtx ecntn eapfest.e, \xc3\xa9 eaeutsh.avu uro gtdeteits rs seexea'ns  ui ldee ueooe es ctodbqm \xc3\xa9dtie\xc3\xa9aleu hdutt\xc3\xa9sitlse  eeora nseq e e''unu hmmlt.e kl utecrr iele s p pocninmefh gs\xc3\xa9 ie \xc3\xa8eia a ac\xc3\xa9 atesahslhenI p s open  pe\xc3\xa9 u\xc3\xa9axe edttuftthie  dr dKinl as  esn ss\xc3\xa9rdeceoeX\xc3\xa8seclfs rh.rh/ii/Cei3\n"


Groupe 2
2 [77, 78, 79, 80, 81, 82, 83]
77 : b'ojkRlktyyknsgyorg{yskjyrk{&y&iixv&kzotioz&siksrzirlkxorktsv/zn\xc3\xaf&&t&\xc3\xafk&ygmo{&xy&hkvzn&m&&&kxyxio{{ynt4ozwrl{{i4mHI9t~e7\x10z\xc3\xafoO\xc3\xaetxgr&Oigktw&z\xc3\xaf&ok{v&ygklvt4xkz&\xc3\xaf&okg&{xo&tok&k9iko{&trkyymr&rstutgu{ikxzi&zuoyr&&ulx{{&s&r&r&xz~kko&{u-gto\xc3\xbfvko-y\xc3\xafyko&ik&m&zy&ytgz&gkxtkg{&&yx&tstjy4ru&sku{vr2k&h&jx\x7f&kygkykky\xc3\xafu&k&yi\xc3\xa6x&t&&.zu~{r{rlkgk&krokuvo\x10jQoo7tnlk&srr&k&jok\xc3\xafr\xc3\xaftkylyk&xgHkzxglt\\x&ttg|Oon&jyox&mo&gu&\xc3\xaerx{\xc3\xa6{stu&uUygrzoz2&vxlykoo&x4goktwg\xc3\xafoskg2&&gyyz\xc3\xaf4&oyz&k&o&o2{y^i-tu{o&i&o&iz5qg}nem+'
78 : b'nijQkjsxxjmrfxnqfzxrjixqjz%x%hhwu%jynshny%rhjrqyhqkjwnqjsru.ym\xc3\xae%%s%\xc3\xaej%xflnz%wx%gjuym%l%%%jwxwhnzzxms3nyvqkzzh3lGH8s}d6\x0fy\xc3\xaenN\xc3\xadswfq%Nhfjsv%y\xc3\xae%njzu%xfjkus3wjy%\xc3\xae%njf%zwn%snj%j8hjnz%sqjxxlq%qrstsftzhjwyh%ytnxq%%tkwzz%r%q%q%wy}jjn%zt,fsn\xc3\xbeujn,x\xc3\xaexjn%hj%l%yx%xsfy%fjwsjfz%%xw%srsix3qt%rjtzuq1j%g%iw~%jxfjxjjx\xc3\xaet%j%xh\xc3\xa5w%s%%-yt}zqzqkjfj%jqnjtun\x0fiPnn6smkj%rqq%j%inj\xc3\xaeq\xc3\xaesjxkxj%wfGjywfks[w%ssf{Nnm%ixnw%ln%ft%\xc3\xadqwz\xc3\xa5zrst%tTxfqyny1%uwkxjnn%w3fnjsvf\xc3\xaenrjf1%%fxxy\xc3\xae3%nxy%j%n%n1zx]h,stzn%h%n%hy4pf|mdl*'
79 : b'mhiPjirwwilqewmpeywqihwpiy$w$ggvt$ixmrgmx$qgiqpxgpjivmpirqt-xl\xc3\xad$$r$\xc3\xadi$wekmy$vw$fitxl$k$$$ivwvgmyywlr2mxupjyyg2kFG7r|c5\x0ex\xc3\xadmM\xc3\xacrvep$Mgeiru$x\xc3\xad$miyt$weijtr2vix$\xc3\xad$mie$yvm$rmi$i7gimy$rpiwwkp$pqrsresygivxg$xsmwp$$sjvyy$q$p$p$vx|iim$ys+erm\xc3\xbdtim+w\xc3\xadwim$gi$k$xw$wrex$eivriey$$wv$rqrhw2ps$qisytp0i$f$hv}$iweiwiiw\xc3\xads$i$wg\xc3\xa4v$r$$,xs|ypypjiei$ipmistm\x0ehOmm5rlji$qpp$i$hmi\xc3\xadp\xc3\xadriwjwi$veFixvejrZv$rrezMml$hwmv$km$es$\xc3\xacpvy\xc3\xa4yqrs$sSwepxmx0$tvjwimm$v2emirue\xc3\xadmqie0$$ewwx\xc3\xad2$mwx$i$m$m0yw\\g+rsym$g$m$gx3oe{lck)'
80 : b'lghOihqvvhkpdvlodxvphgvohx#v#ffus#hwlqflw#pfhpowfoihulohqps,wk\xc3\xac##q#\xc3\xach#vdjlx#uv#ehswk#j###huvuflxxvkq1lwtoixxf1jEF6q{b4\rw\xc3\xaclL\xc3\xabqudo#Lfdhqt#w\xc3\xac#lhxs#vdhisq1uhw#\xc3\xac#lhd#xul#qlh#h6fhlx#qohvvjo#opqrqdrxfhuwf#wrlvo##riuxx#p#o#o#uw{hhl#xr*dql\xc3\xbcshl*v\xc3\xacvhl#fh#j#wv#vqdw#dhuqhdx##vu#qpqgv1or#phrxso/h#e#gu|#hvdhvhhv\xc3\xacr#h#vf\xc3\xa3u#q##+wr{xoxoihdh#holhrsl\rgNll4qkih#poo#h#glh\xc3\xaco\xc3\xacqhvivh#udEhwudiqYu#qqdyLlk#gvlu#jl#dr#\xc3\xaboux\xc3\xa3xpqr#rRvdowlw/#suivhll#u1dlhqtd\xc3\xaclphd/##dvvw\xc3\xac1#lvw#h#l#l/xv[f*qrxl#f#l#fw2ndzkbj('
81 : b'kfgNhgpuugjocukncwuogfungw"u"eetr"gvkpekv"oegonvenhgtkngpor+vj\xc3\xab""p"\xc3\xabg"ucikw"tu"dgrvj"i"""gtutekwwujp0kvsnhwwe0iDE5pza3\x0cv\xc3\xabkK\xc3\xaaptcn"Kecgps"v\xc3\xab"kgwr"ucghrp0tgv"\xc3\xab"kgc"wtk"pkg"g5egkw"pnguuin"nopqpcqwegtve"vqkun""qhtww"o"n"n"tvzggk"wq)cpk\xc3\xbbrgk)u\xc3\xabugk"eg"i"vu"upcv"cgtpgcw""ut"popfu0nq"ogqwrn.g"d"ft{"gucguggu\xc3\xabq"g"ue\xc3\xa2t"p""*vqzwnwnhgcg"gnkgqrk\x0cfMkk3pjhg"onn"g"fkg\xc3\xabn\xc3\xabpguhug"tcDgvtchpXt"ppcxKkj"fukt"ik"cq"\xc3\xaantw\xc3\xa2wopq"qQucnvkv."rthugkk"t0ckgpsc\xc3\xabkogc.""cuuv\xc3\xab0"kuv"g"k"k.wuZe)pqwk"e"k"ev1mcyjai\''
82 : b'jefMgfottfinbtjmbvtnfetmfv!t!ddsq!fujodju!ndfnmudmgfsjmfonq*ui\xc3\xaa!!o!\xc3\xaaf!tbhjv!st!cfqui!h!!!fstsdjvvtio/jurmgvvd/hCD4oy`2\x0bu\xc3\xaajJ\xc3\xa9osbm!Jdbfor!u\xc3\xaa!jfvq!tbfgqo/sfu!\xc3\xaa!jfb!vsj!ojf!f4dfjv!omftthm!mnopobpvdfsud!upjtm!!pgsvv!n!m!m!suyffj!vp(boj\xc3\xbaqfj(t\xc3\xaatfj!df!h!ut!tobu!bfsofbv!!ts!onoet/mp!nfpvqm-f!c!esz!ftbftfft\xc3\xaap!f!td\xc3\xa1s!o!!)upyvmvmgfbf!fmjfpqj\x0beLjj2oigf!nmm!f!ejf\xc3\xaam\xc3\xaaoftgtf!sbCfusbgoWs!oobwJji!etjs!hj!bp!\xc3\xa9msv\xc3\xa1vnop!pPtbmuju-!qsgtfjj!s/bjforb\xc3\xaajnfb-!!bttu\xc3\xaa/!jtu!f!j!j-vtYd(opvj!d!j!du0lbxi`h&'
83 : b"ideLfenssehmasilausmedsleu s ccrp etincit mcemltclferilenmp)th\xc3\xa9  n \xc3\xa9e sagiu rs bepth g   ersrciuushn.itqlfuuc.gBC3nx_1\nt\xc3\xa9iI\xc3\xa8nral Icaenq t\xc3\xa9 ieup saefpn.ret \xc3\xa9 iea uri nie e3ceiu nlessgl lmnonaoucertc toisl  ofruu m l l rtxeei uo'ani\xc3\xb9pei's\xc3\xa9sei ce g ts snat aerneau  sr nmnds.lo meoupl,e b dry esaesees\xc3\xa9o e sc\xc3\xa0r n  (toxululfeae elieopi\ndKii1nhfe mll e die\xc3\xa9l\xc3\xa9nesfse raBetrafnVr nnavIih dsir gi ao \xc3\xa8lru\xc3\xa0umno oOsaltit, prfseii r.aienqa\xc3\xa9imea,  asst\xc3\xa9. ist e i i,usXc'noui c i ct/kawh_g%"


Groupe 3
3 [112, 114, 115, 116, 117]
112 : b'kjsjk%\xc3\xady~%njwyt~gj%\xc3\xafyzffznxnijnjquxwkytw%xjmrtuntjk%%qnuyjt3jtxf\xc3\xa5fivxvy{jxwhj%f\xc3\xaexj%nijfuu%%nnmxngft%%xjz%wxsz\x0f%Smd\xc3\xady\xc2\xb5=N%%%]h%jztijqn%\xc3\xadzi%lzksxqitnxkf%%ts%zyiksqisyyL%xqus3mrsns%\xc3\xae%jj\xc3\xaejxfj%zjxz{mwjjmh\xc3\xadzq%jihzj%gy\xc3\x8erufxtjn%%}wkNynn%jt%fxjX%w%squmwuwxx%huy{%v{sxx}n%qu,jqsuxjx%,sqg%qjtf%{uquj%xkszhx2xw%qsiyixq%{qlihf%zyjtwf.xy%h%jkssmv\x0fwf%j=jtk%infqqkyjxswfy%w%x%xh\xc3\xaewf%%jzkynjijszfqynij%hxrjku%i[w%ng%of%xxysfxqmqn%uq%fx%qthj%nxsxz%hw\xc3\xae%x%vsn%\xc3\xaewh\x0fyvynkxqhhi%{%jqfywxhufqk\x0fjykn3nnijF'
114 : b'ihqhi#\xc3\xabw|#lhuwr|eh#\xc3\xadwxddxlvlghlhosvuiwru#vhkprslrhi##olswhr1hrvd\xc3\xa3dgtvtwyhvufh#d\xc3\xacvh#lghdss##llkvledr##vhx#uvqx\r#Qkb\xc3\xabw\xc2\xb3;L###[f#hxrghol#\xc3\xabxg#jxiqvogrlvid##rq#xwgiqogqwwJ#vosq1kpqlq#\xc3\xac#hh\xc3\xachvdh#xhvxykuhhkf\xc3\xabxo#hgfxh#ew\xc3\x8cpsdvrhl##{uiLwll#hr#dvhV#u#qoskusuvv#fswy#tyqvv{l#os*hoqsvhv#*qoe#ohrd#ysosh#viqxfv0vu#oqgwgvo#yojgfd#xwhrud,vw#f#hiqqkt\rud#h;hri#gldooiwhvqudw#u#v#vf\xc3\xacud##hxiwlhghqxdowlgh#fvphis#gYu#le#md#vvwqdvokol#so#dv#orfh#lvqvx#fu\xc3\xac#v#tql#\xc3\xacuf\rwtwlivoffg#y#hodwuvfsdoi\rhwil1llghD'
115 : b'hgpgh"\xc3\xaav{"kgtvq{dg"\xc3\xacvwccwkukfgkgnruthvqt"ugjoqrkqgh""nkrvgq0gquc\xc3\xa2cfsusvxguteg"c\xc3\xabug"kfgcrr""kkjukdcq""ugw"tupw\x0c"Pja\xc3\xaav\xc2\xb2:K"""Ze"gwqfgnk"\xc3\xaawf"iwhpunfqkuhc""qp"wvfhpnfpvvI"unrp0jopkp"\xc3\xab"gg\xc3\xabgucg"wguwxjtggje\xc3\xaawn"gfewg"dv\xc3\x8borcuqgk""zthKvkk"gq"cugU"t"pnrjtrtuu"ervx"sxpuuzk"nr)gnprugu")pnd"ngqc"xrnrg"uhpweu/ut"npfvfun"xnifec"wvgqtc+uv"e"ghppjs\x0ctc"g:gqh"fkcnnhvguptcv"t"u"ue\xc3\xabtc""gwhvkgfgpwcnvkfg"euoghr"fXt"kd"lc"uuvpcunjnk"rn"cu"nqeg"kupuw"et\xc3\xab"u"spk"\xc3\xabte\x0cvsvkhuneef"x"gncvtuercnh\x0cgvhk0kkfgC'
116 : b'gfofg!\xc3\xa9uz!jfsupzcf!\xc3\xabuvbbvjtjefjfmqtsgups!tfinpqjpfg!!mjqufp/fptb\xc3\xa1bertruwftsdf!b\xc3\xaatf!jefbqq!!jjitjcbp!!tfv!stov\x0b!Oi`\xc3\xa9u\xc2\xb19J!!!Yd!fvpefmj!\xc3\xa9ve!hvgotmepjtgb!!po!vuegomeouuH!tmqo/inojo!\xc3\xaa!ff\xc3\xaaftbf!vftvwisffid\xc3\xa9vm!fedvf!cu\xc3\x8anqbtpfj!!ysgJujj!fp!btfT!s!omqisqstt!dquw!rwottyj!mq(fmoqtft!(omc!mfpb!wqmqf!tgovdt.ts!moeuetm!wmhedb!vufpsb*tu!d!fgooir\x0bsb!f9fpg!ejbmmguftosbu!s!t!td\xc3\xaasb!!fvgujfefovbmujef!dtnfgq!eWs!jc!kb!ttuobtmimj!qm!bt!mpdf!jtotv!ds\xc3\xaa!t!roj!\xc3\xaasd\x0burujgtmdde!w!fmbustdqbmg\x0bfugj/jjefB'
117 : b"fenef \xc3\xa8ty iertoybe \xc3\xaatuaauisideielpsrftor sehmopioef  lipteo.eosa\xc3\xa0adqsqtvesrce a\xc3\xa9se ideapp  iihsibao  seu rsnu\n Nh_\xc3\xa8t\xc2\xb08I   Xc euodeli \xc3\xa8ud gufnsldoisfa  on utdfnldnttG slpn.hmnin \xc3\xa9 ee\xc3\xa9esae uesuvhreehc\xc3\xa8ul edcue bt\xc3\x89mpasoei  xrfItii eo aseS r nlphrprss cptv qvnssxi lp'elnpses 'nlb leoa vplpe sfnucs-sr lndtdsl vlgdca uteora)st c efnnhq\nra e8eof diallftesnrat r s sc\xc3\xa9ra  euftiedenualtide csmefp dVr ib ja sstnaslhli pl as loce isnsu cr\xc3\xa9 s qni \xc3\xa9rc\ntqtifslccd v elatrscpalf\netfi.iideA"


Groupe 4
4 [111, 112, 113, 114]
111 : b'i#\xc3\xab#uYu#vgiq#lqd\xc3\xac#xpw#jlwydwdo/#dd#hihqh\xc3\xa3|#lhqktp#uF+#vh##v##gll#ohx/x#d#l#kppow1qoihq#hdpshh#n#o#g4G##hqh#hu\r=~4Yubb9oqddhouq#px#hvYul\xc3\xac+uqiw#hdqw#uu4Rx#gqkhiwrd##\xc3\xaclEwod#\rlhwwrg1f#qqpr##p##hurl##/dduvlxwhoufovlyho#hqw\xc3\xacpowdio#uoh#ggv#uxod\xc3\xacwlrl#odh1grrvdsxr###huplrdudhulvd\rrqhuvh#u#llrhuudhufux#w#phhqhwxddlddxhoxgvh#q##1wwervviwrdxIlvs#9#gls\xc3\xacq#hd/h#n#d#lgwp#ovk1oevlv#u#j#l#\xc3\xacsq##idvsx#hvidphlhqhofhlghhh#l#hr#olxrglholqkpFqwvh*sr#wglex*wo#du\rhxdtr#*hrhghd#hy#h#drvhiV#susrnihq;'
112 : b'h"\xc3\xaa"tXt"ufhp"kpc\xc3\xab"wov"ikvxcvcn."cc"ghgpg\xc3\xa2{"kgpjso"tE*"ug""u""fkk"ngw.w"c"k"joonv0pnhgp"gcorgg"m"n"f3F""gpg"gt\x0c<}3Xtaa8npccgntp"ow"guXtk\xc3\xab*tphv"gcpv"tt3Qw"fpjghvqc""\xc3\xabkDvnc"\x0ckgvvqf0e"ppoq""o""gtqk"".cctukwvgntenukxgn"gpv\xc3\xabonvchn"tng"ffu"twnc\xc3\xabvkqk"ncg0fqqucrwq"""gtokqctcgtkuc\x0cqpgtug"t"kkqgttcgtetw"v"oggpgvwcckccwgnwfug"p""0vvdquuhvqcwHkur"8"fkr\xc3\xabp"gc.g"m"c"kfvo"nuj0nduku"t"i"k"\xc3\xabrp""hcurw"guhcogkgpgnegkfggg"k"gq"nkwqfkgnkpjoEpvug)rq"vfkdw)vn"ct\x0cgwcsq")gqgfgc"gx"g"cqughU"rtrqmhgp:'
113 : b'g!\xc3\xa9!sWs!tego!job\xc3\xaa!vnu!hjuwbubm-!bb!fgfof\xc3\xa1z!jfoirn!sD)!tf!!t!!ejj!mfv-v!b!j!innmu/omgfo!fbnqff!l!m!e2E!!fof!fs\x0b;|2Ws``7mobbfmso!nv!ftWsj\xc3\xaa)sogu!fbou!ss2Pv!eoifgupb!!\xc3\xaajCumb!\x0bjfuupe/d!oonp!!n!!fspj!!-bbstjvufmsdmtjwfm!fou\xc3\xaanmubgm!smf!eet!svmb\xc3\xaaujpj!mbf/epptbqvp!!!fsnjpbsbfsjtb\x0bpofstf!s!jjpfssbfsdsv!u!nffofuvbbjbbvfmvetf!o!!/uucpttgupbvGjtq!7!ejq\xc3\xaao!fb-f!l!b!jeun!mti/mctjt!s!h!j!\xc3\xaaqo!!gbtqv!ftgbnfjfofmdfjefff!j!fp!mjvpejfmjoinDoutf(qp!uejcv(um!bs\x0bfvbrp!(fpfefb!fw!f!bptfgT!qsqplgfo9'
114 : b"f \xc3\xa8 rVr sdfn ina\xc3\xa9 umt gitvatal, aa efene\xc3\xa0y ienhqm rC( se  s  dii leu,u a i hmmlt.nlfen eampee k l d1D  ene er\n:{1Vr__6lnaaelrn mu esVri\xc3\xa9(rnft eant rr1Ou dnheftoa  \xc3\xa9iBtla \niettod.c nnmo  m  eroi  ,aarsiutelrclsivel ent\xc3\xa9mltafl rle dds rula\xc3\xa9tioi lae.doosapuo   ermioaraerisa\nonerse r iioerraercru t meenetuaaiaaueludse n  .ttbossftoauFisp 6 dip\xc3\xa9n ea,e k a idtm lsh.lbsis r g i \xc3\xa9pn  faspu esfameienelceideee i eo liuodielinhmCntse'po tdibu'tl ar\neuaqo 'eoedea ev e aosefS prpokfen8"


Groupe 5
5 [227, 228, 229, 230, 231, 232, 233]
227 : b'x\\xikok{zklzyz&rzstkxskx2g&ot{&xixry\xc3\xafyzs&yjltug{sik\xc3\xafw{ktiigIskytr\x7f&k&o{tjlrokuvo&jkl&\xc3\xae\xc3\xafxxgxtjQogosk>ki\xc3\xaf2\x1e&g&oL&Rl79999&uo{&k\xc3\xafijg&y&kok&io\xc3\xaf&x&i~t&\xc3\xafik{;t|l\xc3\xafku&x&mtiz&ugggx7Iltx&zk&rvz\xc3\xafk{ljujv4&oltz&wxk&ytz&\xc3\xaf&zgzuot{ixm&kokk&x&y&&{vkkyky|ooz&y{lr{ti&kxt&or-ors{sk\xc3\xb0|{y&&&\xc3\xafu&mYtg&k\x7fyizirky&u&t&\xc3\xafk&tykjotigyx&motr&k&kzk3yrmji&&got{&xy3hkxio{k9skiuzkz&&&yQoxtxzkgujkgo&kh-tyikjk{\xc2\x80jkgzgjlt&grjy&xx\xc3\xaf&m4-tokzyo&yytzw&jgor&okhy-y&okkyu&to{{rnkyo-k&gjioJikz{tyo&\xc3\xb5t\xc3\xafx{y2\xc3\xaek&ki{y&xu@y4kxole+x'
228 : b'w[whjnjzyjkyxy%qyrsjwrjw1f%nsz%whwqx\xc3\xaexyr%xikstfzrhj\xc3\xaevzjshhfHrjxsq~%j%nzsikqnjtun%ijk%\xc3\xad\xc3\xaewwfwsiPnfnrj=jh\xc3\xae1\x1d%f%nK%Qk68888%tnz%j\xc3\xaehif%x%jnj%hn\xc3\xae%w%h}s%\xc3\xaehjz:s{k\xc3\xaejt%w%lshy%tfffw6Hksw%yj%quy\xc3\xaejzkitiu3%nksy%vwj%xsy%\xc3\xae%yfytnszhwl%jnjj%w%x%%zujjxjx{nny%xzkqzsh%jws%nq,nqrzrj\xc3\xaf{zx%%%\xc3\xaet%lXsf%j~xhyhqjx%t%s%\xc3\xaej%sxjinshfxw%lnsq%j%jyj2xqlih%%fnsz%wx2gjwhnzj8rjhtyjy%%%xPnwswyjftijfn%jg,sxhjijz\x7fijfyfiks%fqix%ww\xc3\xae%l3,snjyxn%xxsyv%ifnq%njgx,x%njjxt%snzzqmjxn,j%fihnIhjyzsxn%\xc3\xb4s\xc3\xaewzx1\xc3\xadj%jhzx%wt?x3jwnkd*w'
229 : b'vZvgimiyxijxwx$pxqrivqiv0e$mry$vgvpw\xc3\xadwxq$whjrseyqgi\xc3\xaduyirggeGqiwrp}$i$myrhjpmistm$hij$\xc3\xac\xc3\xadvvevrhOmemqi<ig\xc3\xad0\x1c$e$mJ$Pj57777$smy$i\xc3\xadghe$w$imi$gm\xc3\xad$v$g|r$\xc3\xadgiy9rzj\xc3\xadis$v$krgx$seeev5Gjrv$xi$ptx\xc3\xadiyjhsht2$mjrx$uvi$wrx$\xc3\xad$xexsmrygvk$imii$v$w$$ytiiwiwzmmx$wyjpyrg$ivr$mp+mpqyqi\xc3\xaezyw$$$\xc3\xads$kWre$i}wgxgpiw$s$r$\xc3\xadi$rwihmrgewv$kmrp$i$ixi1wpkhg$$emry$vw1fivgmyi7qigsxix$$$wOmvrvxieshiem$if+rwgihiy~hiexehjr$ephw$vv\xc3\xad$k2+rmixwm$wwrxu$hemp$mifw+w$miiws$rmyypliwm+i$ehgmHgixyrwm$\xc3\xb3r\xc3\xadvyw0\xc3\xaci$igyw$vs>w2ivmjc)v'
230 : b'uYufhlhxwhiwvw#owpqhuphu/d#lqx#ufuov\xc3\xacvwp#vgiqrdxpfh\xc3\xactxhqffdFphvqo|#h#lxqgiolhrsl#ghi#\xc3\xab\xc3\xacuuduqgNldlph;hf\xc3\xac/\x1b#d#lI#Oi46666#rlx#h\xc3\xacfgd#v#hlh#fl\xc3\xac#u#f{q#\xc3\xacfhx8qyi\xc3\xachr#u#jqfw#rdddu4Fiqu#wh#osw\xc3\xachxigrgs1#liqw#tuh#vqw#\xc3\xac#wdwrlqxfuj#hlhh#u#v##xshhvhvyllw#vxioxqf#huq#lo*lopxph\xc3\xadyxv###\xc3\xacr#jVqd#h|vfwfohv#r#q#\xc3\xach#qvhglqfdvu#jlqo#h#hwh0vojgf##dlqx#uv0ehuflxh6phfrwhw###vNluquwhdrghdl#he*qvfhghx}ghdwdgiq#dogv#uu\xc3\xac#j1*qlhwvl#vvqwt#gdlo#lhev*v#lhhvr#qlxxokhvl*h#dgflGfhwxqvl#\xc3\xb2q\xc3\xacuxv/\xc3\xabh#hfxv#ur=v1hulib(u'
231 : b'tXtegkgwvghvuv"nvopgtogt.c"kpw"tetnu\xc3\xabuvo"ufhpqcwoeg\xc3\xabswgpeecEogupn{"g"kwpfhnkgqrk"fgh"\xc3\xaa\xc3\xabttctpfMkckog:ge\xc3\xab.\x1a"c"kH"Nh35555"qkw"g\xc3\xabefc"u"gkg"ek\xc3\xab"t"ezp"\xc3\xabegw7pxh\xc3\xabgq"t"ipev"qccct3Ehpt"vg"nrv\xc3\xabgwhfqfr0"khpv"stg"upv"\xc3\xab"vcvqkpweti"gkgg"t"u""wrgguguxkkv"uwhnwpe"gtp"kn)knowog\xc3\xacxwu"""\xc3\xabq"iUpc"g{uevengu"q"p"\xc3\xabg"pugfkpecut"ikpn"g"gvg/unife""ckpw"tu/dgtekwg5ogeqvgv"""uMktptvgcqfgck"gd)puegfgw|fgcvcfhp"cnfu"tt\xc3\xab"i0)pkgvuk"uupvs"fckn"kgdu)u"kgguq"pkwwnjguk)g"cfekFegvwpuk"\xc3\xb1p\xc3\xabtwu.\xc3\xaag"gewu"tq<u0gtkha\'t'
232 : b'sWsdfjfvufgutu!munofsnfs-b!jov!sdsmt\xc3\xaatun!tegopbvndf\xc3\xaarvfoddbDnftomz!f!jvoegmjfpqj!efg!\xc3\xa9\xc3\xaassbsoeLjbjnf9fd\xc3\xaa-\x19!b!jG!Mg24444!pjv!f\xc3\xaadeb!t!fjf!dj\xc3\xaa!s!dyo!\xc3\xaadfv6owg\xc3\xaafp!s!hodu!pbbbs2Dgos!uf!mqu\xc3\xaafvgepeq/!jgou!rsf!tou!\xc3\xaa!ubupjovdsh!fjff!s!t!!vqfftftwjju!tvgmvod!fso!jm(jmnvnf\xc3\xabwvt!!!\xc3\xaap!hTob!fztdudmft!p!o!\xc3\xaaf!otfejodbts!hjom!f!fuf.tmhed!!bjov!st.cfsdjvf4nfdpufu!!!tLjsosufbpefbj!fc(otdfefv{efbubego!bmet!ss\xc3\xaa!h/(ojfutj!ttour!ebjm!jfct(t!jfftp!ojvvmiftj(f!bedjEdfuvotj!\xc3\xb0o\xc3\xaasvt-\xc3\xa9f!fdvt!sp;t/fsjg`&s'
233 : b"rVrceieuteftst ltmnermer,a inu rcrls\xc3\xa9stm sdfnoaumce\xc3\xa9quenccaCmesnly e iundflieopi def \xc3\xa8\xc3\xa9rrarndKiaime8ec\xc3\xa9,\x18 a iF Lf13333 oiu e\xc3\xa9cda s eie ci\xc3\xa9 r cxn \xc3\xa9ceu5nvf\xc3\xa9eo r gnct oaaar1Cfnr te lpt\xc3\xa9eufdodp. ifnt qre snt \xc3\xa9 tatoinucrg eiee r s  upeesesviit suflunc ern il'ilmume\xc3\xaavus   \xc3\xa9o gSna eysctcles o n \xc3\xa9e nsedincasr ginl e ete-slgdc  ainu rs-berciue3mecotet   sKirnrteaodeai eb'nscedeuzdeatadfn alds rr\xc3\xa9 g.'nietsi ssntq dail iebs's ieeso niuulhesi'e adciDcetunsi \xc3\xafn\xc3\xa9rus,\xc3\xa8e ecus ro:s.erif_%r"

```

Pour le **groupe 0**, on voit une liste de 9 clés plausibles selon notre marge d'erreur : **193, 194, 195, 196, 197, 198, 199, 200, 201**.

Si l'on regarde les déchiffrements associés, on voit très clairement qu'avec 200, le texte contient trop de points d'exclamation, contrairement à **201 qui semble très probable**. On déduit donc que **la première valeur du vigenère est 201**.

En appliquant la même logique pour le reste, obtient les clés : **`201 99 83 117 114 233`**

Essayons maintenant un Vigenère avec la clé **`201 99 83 117 114 233`** :

```python
key = [201, 99, 83, 117, 114, 233]
decoded = ''
for i in range(0, len(text)):
    decoded += chr(ord(text[i]) - key[i % len(key)])
print(decoded)
```

```
Output:
Chũffre Ťe Vigťnère
Ōe chiŦfre dť VigeŮère eųt un ųystèmť de cŨiffreŭent pšr subųtitutũon poŬyalphšbétiqŵe maių une ŭême lťttre Ťu mesųage cŬair pťut, sŵivantĠsa poųitionĠdans ţelui-ţi, êtŲe remŰlacéeĠpar dťs letŴres dũfféreŮtes, ţontraũremenŴ à unĠsystèŭe de ţhiffrťment ŭono aŬphabéŴique ţomme Ŭe chiŦfre dť CésaŲ (qu'ũl utiŬise cťpendaŮt comŭe comŰosantĩ. CetŴe métŨode rǩsisteĠainsiĠà l'aŮalyseĠde frǩquencťs, ceĠqui eųt un švantaŧe décũsif sŵr lesĠchiffŲementų monoĠalphaŢétiquťs. CeŰendanŴ le cŨiffreĠde ViŧenèreĠa étéĠpercéĠpar lť majoŲ prusųien FŲiedriţh Kasũski qŵi a pŵblié ųa métŨode eŮ 1863Į Depuũs cetŴe époűue, iŬ n‘ofŦre plŵs aucŵne séţuritéĮ

Flaŧ : CYłN{L3_Ńh1ffrĳ_V1g3Ůèr3T3Ÿt_35tş⚰_3n_ı863}
ĊIl esŴ nommǩ ainsũ au XŉXe siǨcle eŮ réféŲence šu dipŬomateĠdu XVŉe sièţle Blšise dť VigeŮère, űui leĠdécriŴ (intǩgré àĠun chũffremťnt plŵs comŰlexe)Ġdans ųon tršité dťs chiŦfres Űaru eŮ 1586Į On tŲouve ťn faiŴ déjàĠune mǩthodeĠde chũffremťnt anšlogueĠdans ŵn couŲt traũté deĠGiovaŮ Battũsta BťllasoĠparu ťn 155ĳ.
Ce ţhiffrťment ũntrodŵit laĠnotioŮ de cŬé. Unť clé ųe préųente ŧénéraŬementĠsous Ŭa forŭe d'uŮ mot ůu d'uŮe phršse. Půur poŵvoir ţhiffrťr notŲe texŴe, à ţhaqueĠcaracŴère nůus utũlisonų une ŬettreĠde laĠclé půur efŦectueŲ la sŵbstitŵtion.ĠÉvideŭment,Ġplus Ŭa cléĠsera ŬongueĠet vaŲiée eŴ mieuŸ le tťxte sťra chũffré.ĠIl faŵt savůir quħil y š eu uŮe pérũode oǹ des Űassagťs entũers dħœuvreų littǩraireų étaiťnt utũlisésĠpour ţhiffrťr lesĠplus ŧrandsĠsecreŴs. Leų deuxĠcorreųpondaŮts n'švaienŴ plusĠqu'à švoir ťn leuŲs maiŮs un ťxemplšire dŵ mêmeĠlivreĠpour ų'assuŲer deĠla boŮne coŭpréheŮsion Ťes meųsagesĮ
Si Ŭ'on cůnnaitĠle noŭbre dť symbůles qŵe comŰorte Ŭa cléĬ il dťvientĠpossiŢle deĠprocéŤer paŲ analŹse deĠfréquťnces ųur chšcun dťs souų-textťs détťrminéų en sǩlectiůnnantĠdes lťttresĠdu meųsage ţlair Ǡ inteŲvalleĠla loŮgueurĠde laĠclef ĨautanŴ de sůus-teŸtes qŵe la Ŭongueŵr de Ŭa cleŦ). C'ťst l'šttaquť bienĠconnuť sur Ŭes chũffremťnts můno-alŰhabétũques.Ċ
FrieŤrich ŋasiskũ publũe en ı863 uŮe métŨode eŦficacť pourĠdéterŭiner Ŭa taiŬle deĠla clťf, leĠtest Ťe Kasũski, ťn repǩrant Ŭa répǩtitioŮ de cťrtainų motiŦs danų le mťssageĠchiffŲé. Chšrles łabbagť s'esŴ intéŲessé šu chiŦfremeŮt de ŖigenèŲe uneĠdizaiŮe d'aŮnées šuparaŶant. ŉl avaũt décŨiffréĠdans Ťes caų partũculieŲs desĠmessaŧes chũffrésĠpar lš méthůde deĠVigenǨre. IŬ n'a Ųien pŵblié Ǡ ce sŵjet, ŭais oŮ dispůse deĠses nůtes. ŏn ne ųait pšs queŬle méŴhode ũl a uŴiliséĬ il aĠpu exŰloiteŲ des Ŧaibleųses dť l'utũlisatũon duĠchiffŲementĮ Certšins hũstoriťns peŮsent űu'il š pu dǩcouvrũr la ŭéthodť de KšsiskiĬ bienĠqu'ilĠn'en šit paų laisųé de Ŵrace ǩcriteĮ

DesĠtechnũques ųtatisŴiquesĠfondéťs surĠl'indũce deĠcoïncũdenceĬ décoŵverteų au XŘe sièţle, sħavèreŮt encůre plŵs effũcacesĠpour ţasserĠle chũffre.Ġ
Sourţe : hŴtps:/įfr.wiūipediš.org/ŷiki/CŨiffreşde_Viŧen%C3ĥA8re
```

On voit qu'on y presque, seul problème est que tous les 6 caractères en commençant au 3ème, ce n'est pas bon. Nous sommes parti du principe que c'était codé sur 1 octet, donc jusque 256, mais n'oublions pas que nous somme en UTF-8, essayons donc d'ajouter 256 à 83 autant de fois qu'il faut pour obtenir quelque chose de lisible. **`83 + 256 = 339`**

```python
key = [201, 99, 339, 117, 114, 233]
decoded = ''
for i in range(0, len(text)):
    decoded += chr(ord(text[i]) - key[i % len(key)])
print(decoded)
```

```
Output:
Chiffre de Vigenère
Le chiffre de Vigenère est un système de chiffrement par substitution polyalphabétique mais une même lettre du message clair peut, suivant sa position dans celui-ci, être remplacée par des lettres différentes, contrairement à un système de chiffrement mono alphabétique comme le chiffre de César (qu'il utilise cependant comme composant). Cette méthode résiste ainsi à l'analyse de fréquences, ce qui est un avantage décisif sur les chiffrements mono alphabétiques. Cependant le chiffre de Vigenère a été percé par le major prussien Friedrich Kasiski qui a publié sa méthode en 1863. Depuis cette époque, il n‘offre plus aucune sécurité.

Flag : CYBN{L3_Ch1ffr3_V1g3nèr3T3xt_35t_⚰_3n_1863}

Il est nommé ainsi au XIXe siècle en référence au diplomate du XVIe siècle Blaise de Vigenère, qui le décrit (intégré à un chiffrement plus complexe) dans son traité des chiffres paru en 1586. On trouve en fait déjà une méthode de chiffrement analogue dans un court traité de Giovan Battista Bellaso paru en 1553.
Ce chiffrement introduit la notion de clé. Une clé se présente généralement sous la forme d'un mot ou d'une phrase. Pour pouvoir chiffrer notre texte, à chaque caractère nous utilisons une lettre de la clé pour effectuer la substitution. Évidemment, plus la clé sera longue et variée et mieux le texte sera chiffré. Il faut savoir qu'il y a eu une période où des passages entiers d'œuvres littéraires étaient utilisés pour chiffrer les plus grands secrets. Les deux correspondants n'avaient plus qu'à avoir en leurs mains un exemplaire du même livre pour s'assurer de la bonne compréhension des messages.
Si l'on connait le nombre de symboles que comporte la clé, il devient possible de procéder par analyse de fréquences sur chacun des sous-textes déterminés en sélectionnant des lettres du message clair à intervalle la longueur de la clef (autant de sous-textes que la longueur de la clef). C'est l'attaque bien connue sur les chiffrements mono-alphabétiques.

Friedrich Kasiski publie en 1863 une méthode efficace pour déterminer la taille de la clef, le test de Kasiski, en repérant la répétition de certains motifs dans le message chiffré. Charles Babbage s'est intéressé au chiffrement de Vigenère une dizaine d'années auparavant. Il avait déchiffré dans des cas particuliers des messages chiffrés par la méthode de Vigenère. Il n'a rien publié à ce sujet, mais on dispose de ses notes. On ne sait pas quelle méthode il a utilisé, il a pu exploiter des faiblesses de l'utilisation du chiffrement. Certains historiens pensent qu'il a pu découvrir la méthode de Kasiski, bien qu'il n'en ait pas laissé de trace écrite.

Des techniques statistiques fondées sur l'indice de coïncidence, découvertes au XXe siècle, s'avèrent encore plus efficaces pour casser le chiffre.
Source : https://fr.wikipedia.org/wiki/Chiffre_de_Vigenère
```

**Le flag : CYBN{L3_Ch1ffr3_V1g3nèr3T3xt_35t_⚰_3n_1863}**
