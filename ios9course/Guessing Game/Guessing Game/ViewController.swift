//
//  ViewController.swift
//  Guessing Game
//
//  Created by Maarten Broekman on 2/21/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    @IBOutlet weak var answerLabel: UILabel!
    @IBOutlet weak var answerImage: UIImageView!
    @IBOutlet weak var userGuess: UITextField!
    
    var guessNum = 0

    @IBAction func submitGuess(sender: AnyObject) {
        guessNum++

        let randValue = Int(arc4random_uniform(6))

        
        switch userGuess.text! {
        case "0","1","2","3","4","5": break // okay
        default: answerLabel.text = "Please enter a number\nbetween 0 and 5"
        }

        if String(randValue) == userGuess.text {
            answerLabel.text = "Correct!\nIt took \(guessNum) tries."
            guessNum = 0
        } else {
            answerLabel.text = "Incorrect\nPlease try again.\nAttempt #\(guessNum)"
        }
        
        userGuess.text = ""
    }
    
    @IBAction func resetCount(sender: AnyObject) {
        guessNum = 0
        answerLabel.text = ""
        userGuess.text = ""
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

