//
//  SecondViewController.swift
//  To Do List
//
//  Created by Maarten Broekman on 9/28/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class SecondViewController: UIViewController {

    
    @IBOutlet var ToDoItem: UITextField!
        
    @IBAction func ToDoAction(_ sender: AnyObject) {
        ToDoListArray.append(ToDoItem.text!)
        ToDoItem.text = ""
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    override func touchesBegan(_ touches: Set<UITouch>, with event: UIEvent?) {
        self.view.endEditing(true)
    }
    
    func textFieldShouldReturn(textField: UITextField!) -> Bool {
        ToDoItem.resignFirstResponder()
        return true
    }
}

