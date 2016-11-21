//
//  ViewController.swift
//  KeyboardControl
//
//  Created by Maarten Broekman on 8/20/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController, UITextFieldDelegate {

    @IBOutlet var textInput: UITextField!
    @IBOutlet var labelText: UILabel!
    @IBAction func buttonPress(sender: AnyObject) {
        labelText.text = textInput.text!
        self.view.endEditing(true)

    }
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        self.textInput.delegate = self
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    override func touchesBegan(touches: Set<UITouch>, withEvent event: UIEvent?) {
        self.view.endEditing(true)
    }
    
    func textFieldShouldReturn(textField: UITextField) -> Bool {
        textField.resignFirstResponder()
        buttonPress(self)
        return true
    }
}

