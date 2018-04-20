using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace MagicInventorySystem
{
    /// <summary>
    /// Franchise user operation function
    /// Check their inventory by store ID
    /// Submit a request by input a threshold
    /// Submit a add new item request
    /// </summary>
    public class Franchise :User
    {
        Menu menu = new Menu();
        DatabaseOperation dataOperation = new DatabaseOperation();
        string name = ""; //the store name
        int storeID = 0;
        ArrayList ID_Available = new ArrayList();

        /// <summary>
        /// public method for calling
        /// </summary>
        public void SelectAStore()
        {
            StoresSelect();
        }

        /// <summary>
        /// The menu for looping
        /// </summary>
        public override void LoopMenu()
        {
            string option = "";
            while (!option.Equals("4"))
            {
                menu.StoreMenu(name);
                option = menu.Selection("Enter an option:");
                switch (option)
                {
                    case "1":
                        DisplayInventory();
                        break;
                    case "2":
                        StockRequest_Threshold();
                        break;
                    case "3":
                        AddItem();
                        break;
                    case "4":
                        Console.WriteLine();
                        break;
                    default:
                        menu.warningMsg("INVALID SELECT!\n");
                        break;
                }
            }
        }

        /// <summary>
        /// To select the store 
        /// </summary>
        private void StoresSelect()
        {
            string sqlcommand = "select * from Store";
            var table = dataOperation.getRequiredData(sqlcommand);
            foreach (var x in table.Select())
            {
                ID_Available.Add(x["StoreID"].ToString());
            }

            while(true)
            {
                Console.WriteLine("Stores\n");
                Console.WriteLine("{0,-10} {1,-20} ", "StoreID", "Name");
                foreach (var x in table.Select())
                {
                    Console.WriteLine("{0,-10} {1,-20} ", x["StoreID"], x["Name"]);
                }
                Console.WriteLine();
                var input = menu.Selection("Enter the store to use: ");

                if (!ID_Available.Contains(input) & !input.Equals("") & menu.isNumber(input))
                {
                    menu.warningMsg("NO SUCH STORE!\n");
                }
                else if (ID_Available.Contains(input) & menu.isNumber(input))
                {
                    storeID = Int32.Parse(input.ToString());
                    break;
                }
                else if (!ID_Available.Contains(input) & input.Equals("") & !menu.isNumber(input))
                {
                    menu.warningMsg("Enter pressed, exit to menu \n");
                    return;
                }
                else if (!menu.isNumber(input))
                {
                    menu.warningMsg("INVALID INPUT!\n");
                }
            }

            foreach (var x in table.Select())
            {
                if (x["StoreID"].Equals(storeID))
                {
                    name = x["Name"].ToString();
                }
            }

            Console.WriteLine();
            ID_Available.Clear();
            LoopMenu();
        }

        /// <summary>
        /// Display the whole inventory by global store ID
        /// </summary>
        private void DisplayInventory()
        {
            var sqlcommand = 
                @"select Product.ProductID,Product.Name, StoreInventory.StockLevel 
                from StoreInventory left join Product on Product.ProductID = StoreInventory.ProductID 
                Where StoreInventory.StoreID = @storeID";
            var table = dataOperation.getRequiredData(sqlcommand,new SqlParameter("storeID",storeID));

            Console.WriteLine("Inventory\n");
            Console.WriteLine("{0,-5} {1,-30} {2,-10}", "ID", "Product", "Current Stock");
            foreach(var x in table.Select())
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-10}", x["ProductID"], x["Name"], x["StockLevel"]);
            }
            //retrieve from the database to get inventory
            Console.WriteLine();
        }

        /// <summary>
        /// Create a StockRequest by input a threshold
        /// </summary>
        private void StockRequest_Threshold()
        {
            while(true)
            {
                var threshold = 0;
                var input = menu.Selection("Enter threshold for re-stocking:");
                if (menu.isNumber(input))
                {
                    threshold = Int32.Parse(input);

                    var sqlcommand =
                        @"select Product.ProductID,Product.Name, StoreInventory.StockLevel
                        from StoreInventory left join Product on Product.ProductID = StoreInventory.ProductID
                        Where StoreInventory.StockLevel < @threshold AND StoreInventory.StoreID = @storeID";

                    var table = dataOperation.getRequiredData(sqlcommand,
                        new SqlParameter("threshold", threshold),
                        new SqlParameter("storeID", storeID));

                    if (table.Rows.Count != 0)
                    {
                        foreach (var x in table.Select())
                        {
                            ID_Available.Add(Int32.Parse(x["ProductID"].ToString()));
                        }

                        if (table.Equals(null)) //all stock is above the threshold
                        {
                            menu.warningMsg("All Inventory stock levels are equal to or above " + threshold + "!");
                        }
                        else
                        {
                            Console.WriteLine("Inventory\n");
                            Console.WriteLine("{0,-5} {1,-30} {2,-10}", "ID", "Product", "Current Stock");
                            foreach (var x in table.Select())
                            {
                                Console.WriteLine("{0,-5} {1,-30} {2,-10}", x["ProductID"], x["Name"], x["StockLevel"]);
                            }
                            //retrieve from the database to get inventory
                            Console.WriteLine();
                            requectSelect(threshold);
                        }
                    }
                    else
                    {
                        menu.warningMsg("No Item Under Threshold " + threshold + "!\n");
                        break;
                    }
                }
                else if (!menu.isNumber(input) & !input.Equals(""))
                {
                    menu.warningMsg("INVALID INPUT!\n");
                }
                else if (input.Equals(""))
                {
                    menu.warningMsg("Enter pressed, exit to menu \n");
                    break;
                }
            }
        }

        /// <summary>
        /// Handle the request selection
        /// </summary>
        /// <param name="threshold"></param>
        private void requectSelect(int threshold)
        {
            int PID = 0; // the product ID user selected
            while (true)
            {
                var input = menu.Selection("Enter request to process: ");
                if (menu.isNumber(input))
                {
                    PID = Int32.Parse(input);
                    if (ID_Available.Contains(PID))
                    {
                        AddRequest(PID, threshold);
                        menu.successMsg("Stock Request Created! \n");
                        menu.successMsg("If you want to re-stock another, input the threshold! Otherwise press enter to exit to menu \n");
                        ID_Available.Clear();
                        return;
                    }
                    else
                    {
                        menu.warningMsg("NO SUCH ID, TRY AGAIN!\n");
                    }
                }
                else if (!menu.isNumber(input) & !input.Equals(""))
                {
                    menu.warningMsg("INVALID INPUT!\n");
                }
                else if (input.Equals(""))
                {
                    menu.warningMsg("Enter pressed, back to re-stocking \n");
                    return;
                }
            }
        }

        /// <summary>
        /// To add a new item request operation
        /// </summary>
        private void AddItem()
        {
            var sql =
            @"SELECT OwnerInventory.ProductID,Product.Name,OwnerInventory.StockLevel
            from OwnerInventory left join Product on OwnerInventory.ProductID = Product.ProductID
            where OwnerInventory.ProductID not in (SELECT ProductID from StoreInventory where StoreID = @storeID)";

            var table = dataOperation.getRequiredData(sql,new SqlParameter("storeID",storeID));

            if (table.Rows.Count == 0)
            {
                menu.warningMsg("You already have all items stocked!\n");
            }
            else
            {
                Console.WriteLine("Inventory\n");
                Console.WriteLine("{0,-5} {1,-30} {2,-10}", "ID", "Product", "Current Stock");

                foreach (var x in table.Select())
                {
                    ID_Available.Add(Int32.Parse(x["ProductID"].ToString()));
                    Console.WriteLine("{0,-5} {1,-30} {2,-10}", x["ProductID"], x["Name"], x["StockLevel"]);
                }
                //retrieve from the database to get inventory
                Console.WriteLine();
                int PID = 0; // the product ID user selected
                while(true)
                {
                    var input = menu.Selection("Enter product to add: ");
                    if (menu.isNumber(input))
                    {
                        PID = Int32.Parse(input);
                        if (ID_Available.Contains(PID))
                        {
                            updateItem(PID);
                            menu.successMsg("Add New Item Stock Request Created!\n");
                            ID_Available.Clear();
                            break;
                        }
                        else
                        {
                            menu.warningMsg("NO SUCH ID, TRYAGAIN!\n");
                        }
                    }
                    else if (!menu.isNumber(input) & !input.Equals(""))
                    {
                        menu.warningMsg("INVALID INPUT!\n");
                    }
                    else if (input.Equals(""))
                    {
                        menu.warningMsg("Enter pressed, exit to menu \n");
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Insert a new request to stockrequest table
        /// </summary>
        /// <param name="ID">product ID</param>
        /// <param name="threshold">The threshold,quantity of item</param>
        private void AddRequest(int ID, int threshold)
        {
            var sqlcommand = "select * from StockRequest";
            var table = dataOperation.getRequiredData(sqlcommand);
            int nextID = table.Rows.Count +1;// the lenght of StockRequest table

            var sql = 
                @"SET IDENTITY_INSERT StockRequest ON INSERT INTO 
                StockRequest (StockRequestID,StoreID,ProductID,Quantity) 
                values(@nextID,@storeID,@ID,@threshold)";
            dataOperation.UpdateOperation(sql,
                                          new SqlParameter("nextID",nextID),
                                          new SqlParameter("storeID",storeID),
                                          new SqlParameter("ID",ID),
                                          new SqlParameter("threshold",threshold));
        }

        /// <summary>
        /// Insert a new request to stockrequest table when the user want to add a new item
        /// </summary>
        /// <param name="PID"> the product ID</param>
        private void updateItem(int PID)
        {
            var sqlcommand = "select * from StockRequest";
            var table = dataOperation.getRequiredData(sqlcommand);
            int nextID = table.Rows.Count + 1;// the lenght of StockRequest table

            var sql = 
                @"SET IDENTITY_INSERT StockRequest ON INSERT INTO 
                StockRequest (StockRequestID,StoreID,ProductID,Quantity)
                values(@nextID,@storeID,@PID,1)";
            dataOperation.UpdateOperation(sql,
                                          new SqlParameter("nextID", nextID),
                                          new SqlParameter("storeID", storeID),
                                          new SqlParameter("PID", PID));
        }
    }
}
