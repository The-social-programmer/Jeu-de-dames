'''from random import randrange

main = []

couleur = ["trèfle", "carreau", "pique", "coeur"]
valeur = ["6", "7", "8", "9", "10", "valet", "dame", "roi", "as"]

cr = randrange(0, 4)
vr = randrange(0, 9)

carte = valeur[vr] + " de " + couleur[cr]

main.append(carte)

condition = 0

while(condition != 9):
    cr = randrange(0, 4)
    vr = randrange(0, 9)

    carte = valeur[vr] + " de " + couleur[cr]
    
    if carte not in main:
        main.append(carte)
        condition += 1

print(main)
'''
"""from random import *

tab = []
for i in range(0, 20):
    n = randrange(1, 101)
    tab.append(n)

print(tab)

n = len(tab)

for i in range(0, n):
    min = i
    for j in range(i + 1, n):
        if tab[j] < tab[min]:
            min = j
    inter = tab[i]
    tab[i] = tab[min]
    tab[min] = inter


print(min, tab[min])
print(tab)

def findMin():
    """
"""
a = 0
b = 10
m = 5

def f(x):
    return x**5 - 3*(x**3) + 2*(x**2)- 7

while abs(f(m)) > 0.001:
    m = (a + b)/2
    if f(m)*f(a) < 0:
        b = m
    else:
        a = m

print((a + b)/2)
"""
"""
ne marche pas mdr
from math import *
def prem(x):
    compte = 0
    for j in range(1, x + 1):
        for i in range(1, int(sqrt(j)) + 1):
            if x % i == 0:
                compte += 1
        if compte == 2:
            return True
        else:
            return False
    

print(prem(7))

"""
from math import *
p = [7, 89]
vecteur = [-1, -1]
distance = -1;



def f():
    mini = 1000000
    for i in range(0, 40):
        x = i
        y = x**2
        vecteur[0] = p[0] - x
        vecteur[1] = p[1] - y
        distance = sqrt(vecteur[0]**2 + vecteur[1]**2)
        print(distance)
        if(distance < mini):
            mini = distance
    print("la plus petite distance est : ", mini)

f()
    
    




















