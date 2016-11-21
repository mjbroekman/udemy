//
//  ViewController.swift
//  NavBar
//
//  Created by Maarten Broekman on 4/22/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    var timerClicks = 0
    
    var timer = NSTimer()
    
    @IBOutlet var timerText: UILabel!
    
    @IBAction func timerReset(sender: AnyObject) {
        timerClicks = 0
        timerText.text = "00:00.00"
    }
    
    @IBAction func timerStart(sender: AnyObject) {
        if !timer.valid {
            timer = NSTimer.scheduledTimerWithTimeInterval(0.01, target: self, selector: #selector(ViewController.timerClick), userInfo: nil, repeats: true)
        } else {
            timerPause(self)
        }
    }
    
    @IBAction func timerStop(sender: AnyObject) {
        timerClicks = 0
        timer.invalidate()
    }
    
    @IBAction func timerPause(sender: AnyObject) {
        timer.invalidate()
    }
    
    func timerClick() {
        timerClicks += 1
        let hundrds: Int = timerClicks % 100
        let seconds: Int = ( timerClicks / 100 ) % 60
        let minutes: Int = ( timerClicks / 100 ) / 60
        
        if minutes < 10 && seconds < 10 && hundrds < 10 {
            timerText.text = "0\(minutes):0\(seconds).0\(hundrds)"
        } else if minutes < 10 && seconds < 10 {
                timerText.text = "0\(minutes):0\(seconds).\(hundrds)"
        } else if seconds < 10 && hundrds < 10 {
            timerText.text = "\(minutes):0\(seconds).0\(hundrds)"
        } else if seconds < 10 {
            timerText.text = "\(minutes):0\(seconds).\(hundrds)"
        } else if minutes < 10 && hundrds < 10 {
            timerText.text = "0\(minutes):\(seconds).0\(hundrds)"
        } else {
            timerText.text = "\(minutes):\(seconds).\(hundrds)"
        }
        // print(seconds,"time has passed")
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        
        timer.invalidate()
        timerText.text = "00:00.00"

        // Do any additional setup after loading the view, typically from a nib.

    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }
}

