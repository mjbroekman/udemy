//
//  ViewController.swift
//  Is It Prime
//
//  Created by Maarten Broekman on 4/18/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    @IBOutlet var numEntry: UITextField!
    
    @IBAction func isItPrime(sender: AnyObject) {
        numResult.text = "\n"
        
        if let num = Int(numEntry.text!) {

            var isPrime = true
            var factor = 2
            var factorList = [1]
            if num == 1 {
                isPrime = false
            }
            
            while factor < num {
                numResult.text = "Finding factors...\n"
                if num % factor == 0 {
                    isPrime = false
                    factorList.append(factor)
                }
                factor += 1
            }
            
            if num != 1 {
                factorList.append(num)
            }
            
            if !isPrime && num != 1 {
                numResult.text = "\(num) is\nNOT Prime.\nFactors: \(factorList)"
            } else if num == 1 {
                numResult.text = "You entered 1.\n1 is Prime."
            } else {
                numResult.text = "\(num) IS\nPRIME!"
            }
        } else {
            numResult.text = "Not a Number"
        }

    }
        
    @IBOutlet var numResult: UILabel!

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

