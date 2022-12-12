import base64

a = b'Vly{4Gjd-vbvI~VZ8UjeGX'
wlcm = b'PitX$AaidZbZBKDW@&PBbRc1MbZBL6bRc(Ob8aVeAZKrHWG*00VR>R@AY*7@Zf9w3XCQQFWgu)}ZfA92XJsIFX>4pDXk~10E&'
prmt = b'MQ(Iuav*eQWgu{2b8~lZa%4In'
print(base64.b85decode(wlcm.decode()).decode())
b = input(base64.b85decode(prmt.decode()).decode())
b = base64.b85encode(str.encode(b))
if a == b:
    print("Can you stop breaking my authentification FOR 5 MINUTES ?! Im trying to learn !\n(tu peux soumettre ce password comme flag)")
else:
    print("Well seems stronger this time, try again !")