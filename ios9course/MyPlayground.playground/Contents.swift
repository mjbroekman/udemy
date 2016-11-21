//: Playground - noun: a place where people can play

import Cocoa

var num = 35
var isPrime = true

var factor = 2

if num == 1 {
    isPrime = false
}

while factor < num {
    print("In while loop")
    if num % factor == 0 {
        isPrime = false
        break
    }
    factor += 1
}

if !isPrime {
    print(num , "is NOT prime.")
} else {
    print(num , "is prime!")
}
