//
//  ViewController.swift
//  Storage-Test
//
//  Created by Maarten Broekman on 8/20/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        // NSUserDefaults.standardUserDefaults().setObject("Maarten", forKey: "name")
        
        let userName = NSUserDefaults.standardUserDefaults().objectForKey("name")! as! String
        
        print("Hello \(userName)")
        
        let arr = [ 1, 2, 3, 4 ]
        
        NSUserDefaults.standardUserDefaults().setObject(arr, forKey: "array")
        
        let userArray = NSUserDefaults.standardUserDefaults().objectForKey("array")! as! NSArray
        
        for x in userArray {
            print("Retrieved \(x) from userArray")
        }
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

