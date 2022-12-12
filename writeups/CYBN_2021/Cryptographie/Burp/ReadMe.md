# Write-Up
> **title:** Burp
>
> **category:** Cryptographie
>
> **difficulty:** Facile
>
> **point:** 25
>
> **author:** Znrfgena
>
> **description:**
>
> Ovrairahr à yn ploreavtug !
>
> Oba pbhentr à ibhf !
>
> Yr synt rfg PLOA{I3el_3nfl_Pelcg0_Pu4yy}


## Analyse du challenge

On voit dans la description **`PLOA{I3el_3nfl_Pelcg0_Pu4yy}`**, ce qui ressemble étrangement au flag.

De plus, les caractères spéciaux comme **`{}_`** n'ont pas bougés. On pense directement à du chiffrement César.

On peut utiliser un décodeur automatique en ligne comme **[dcode](https://www.dcode.fr/chiffre-cesar)**.

![Décode en ligne](images/dcode.png)

Il s'agissait de **ROT13**.

**Le flag : CYBN{V3ry_3asy_Crypt0_Ch4ll}**
