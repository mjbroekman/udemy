//
//  FirstViewController.swift
//  To Do List
//
//  Created by Maarten Broekman on 9/28/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

var ToDoListArray = [String]()

class FirstViewController: UIViewController, UITableViewDelegate {

    @IBOutlet var ToDoListTable: UITableView!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }

    @nonobjc public func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return ToDoListArray.count
    }
    
    @nonobjc public func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = UITableViewCell(style: UITableViewCellStyle.default, reuseIdentifier: "toDoCell")
        cell.textLabel?.text = ToDoListArray[indexPath.row]
        return cell
    }

//    override func viewDidAppear(_ animated: Bool) {
//        ToDoListTable.reloadData()
//    }
    
}
