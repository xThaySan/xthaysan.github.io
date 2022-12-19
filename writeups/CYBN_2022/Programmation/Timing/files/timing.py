#!/usr/bin/python3.10

import time
import random
import os

random.seed(int(time.time()))

print("Guess the number !")
try:
        input_num_1 = int(input())
except:
        print("Leaving")
        exit()

true_num_1 = random.randint(0, 50)

if(input_num_1 != true_num_1):
        print("Error, it was " + str(true_num_1))
        exit()

print("What ?! Guess the next one if you are that strong !")

try:
        input_num_2 = int(input())
except:
        print("Leaving")
        exit()

true_num_2 = random.randint(0, 50000000000)

if(input_num_2 != true_num_2):
        print("Error, it was " + str(true_num_2))
        exit()

print("CHEATER !!! But you won't be able to guess the next one i'm sure !")

try:
        input_num_3 = float(input())
except:
        print("Leaving")
        exit()

true_num_3 = random.random()

if(input_num_3 != true_num_3):
        print("Error, it was " + str(true_num_3))
        exit()

print("GG")
print(os.environ['FLAG'])
