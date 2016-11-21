//
//  ViewController.swift
//  TableViews
//
//  Created by Maarten Broekman on 8/19/16.
//  Copyright © 2016 Maarten Broekman. All rights reserved.
//

import UIKit

class ViewController: UIViewController, UITableViewDelegate {


    @IBOutlet var sliderValue: UISlider!

    @IBOutlet var tableData: UITableView!
    
    var sizeOfTable = 10
    
    // Row display. Implementers should *always* try to reuse cells by setting each cell's reuseIdentifier and querying for available reusable cells with dequeueReusableCellWithIdentifier:
    // Cell gets various attributes set automatically based on table (separators) and data source (accessory views, editing controls)
    
    
    func tableView(tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        return 20
    }

    func tableView(tableView: UITableView, cellForRowAtIndexPath indexPath: NSIndexPath) -> UITableViewCell {
        let cell = UITableViewCell(style: UITableViewCellStyle.Default, reuseIdentifier: "Cell")
        cell.textLabel?.text = "\(indexPath.row + 1) * \(sizeOfTable) = \( (indexPath.row + 1) * sizeOfTable)"
        return cell
    }

    @IBAction func changeTableSize(sender: AnyObject) {
        sizeOfTable = Int(sliderValue.value)
        tableData.reloadData()
        print( "New value \(sizeOfTable)" )
    }
    
    override func viewDidLoad() {
        super.viewDidLoad()
        tableData.reloadData()
        // Do any additional setup after loading the view, typically from a nib.
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
        // Dispose of any resources that can be recreated.
    }


}

