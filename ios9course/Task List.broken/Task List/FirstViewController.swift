//
//  FirstViewController.swift
//  Task List
//
//  Created by Maarten Broekman on 8/20/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

var taskList = [String]()

class FirstViewController: UIViewController, UITableViewDelegate {


    @IBOutlet var taskTable: UITableView!
 //   var userItems = NSUserDefaults.standardUserDefaults().objectForKey("tasks")! as! NSArray
    
    func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return taskList.count
    }
    
    func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
        let task = UITableViewCell(style: UITableViewCellStyle.Default, reuseIdentifier: "Task")
        task.textLabel?.text = taskList[indexPath.row]
        return task
    }

    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


    override func viewDidAppear(animated: Bool) {
        taskTable.reloadData()
    }
    
}

