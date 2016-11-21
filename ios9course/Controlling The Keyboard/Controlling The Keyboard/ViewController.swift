//
//  ViewController.swift
//  Controlling The Keyboard
//
//  Created by Rob Percival on 16/01/2015.
//  Copyright (c) 2015 Appfish. All rights reserved.
//

import UIKit

class ViewController: UIViewController, UITextFieldDelegate {

    @IBOutlet var text: UITextField!
    
    
    @IBOutlet var label: UILabel!
    
    @IBAction func buttonPressed(sender: AnyObject) {
        
        label.text = text.text
        
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        
        self.text.delegate = self
        
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    override func touchesBegan(touches: Set<UITouch>, withEvent event: UIEvent?) {
        
        self.view.endEditing(true)
        
    }
    
    func textFieldShouldReturn(textField: UITextField!) -> Bool {
        
        textField.resignFirstResponder()
        
        return true
        
    }

}

