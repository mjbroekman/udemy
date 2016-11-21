//
//  SecondViewController.swift
//  Task List
//
//  Created by Maarten Broekman on 8/20/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class SecondViewController: UIViewController {

    @IBOutlet var addLabel: UILabel!
    @IBOutlet var taskName: UITextField!

    @IBAction func endTaskEdit(sender: AnyObject) {
        addLabel.text = "Task added!"
        taskList.append(taskName.text!)
        taskName.text = ""
        self.view.endEditing(true)
    }
    @IBAction func saveItem(sender: AnyObject) {
        addLabel.text = "Task added!"
        taskList.append(taskName.text!)
        taskName.text = ""
        self.view.endEditing(true)
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
        if taskName.text != nil {
            taskName.text = ""
        }
        if addLabel.text != nil {
            addLabel.text = ""
        }
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    override func touchesBegan(touches: Set<UITouch>, withEvent event: UIEvent?) {
        self.view.endEditing(true)
    }
    
//    func buttonPress(sender: AnyObject) {
//        addLabel.text = "Task added!"
//        taskList.append(taskName.text!)
//        taskName.text = ""
//        self.view.endEditing(true)
//    }

    func textFieldShouldReturn(textField: UITextField!) -> Bool {
        taskName.resignFirstResponder()
//        buttonPress(self)
        return true
    }

    override func viewDidAppear(animated: Bool) {
        if taskName.text != nil {
            taskName.text = ""
        }
        if addLabel.text != nil {
            addLabel.text = ""
        }
    }
}

