using System;
using System.Collections;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MagicInventorySystem
{
    
    /// <summary>
    /// Owner operation class
    /// display and handle stock request
    /// Display owner Inventory
    /// Reset Owner Product
    /// </summary>
    public class Owner :User
    {
        Menu menu = new Menu();
        DatabaseOperation dataOperation = new DatabaseOperation();
        ArrayList ID_Available = new ArrayList(); // to store available ID
        ArrayList ID_Unavailable = new ArrayList(); // to store unavailable ID

        /// <summary>
        /// The main Loop menu function
        /// </summary>
        public override void LoopMenu()
        {
            string option = "";
            while (!option.Equals("4"))
            {
                menu.OwnerMenu();
                option = menu.Selection("Enter an option: ");
                switch (option)
                {
                    case "1":
                        StockRequest();
                        break;
                    case "2":
                        OwnerInventory();
                        break;
                    case "3":
                        ResetStock();
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
        /// Retreive all related request data from database and then display
        /// </summary>
        private void StockRequest()
        {
            var sql =
                @"select StockRequestID,Store.Name as Store,Product.Name as Product, StockRequest.Quantity, OwnerInventory.StockLevel
                from ((StockRequest left join Store on Store.StoreID = StockRequest.StoreID)
                left join Product on Product.ProductID = StockRequest.ProductID)
                left join OwnerInventory on OwnerInventory.ProductID = StockRequest.ProductID";

            Console.WriteLine("Stock Requests:\n");
            Console.WriteLine("{0,-5} {1,-15} {2,-25} {3,-10} {4,-15} {5,-20}", "ID", "Store", "Product","Quantity", "Current Stock", "Stock Availability");
            var table = dataOperation.getRequiredData(sql);

            //print out the request table
            foreach (var x in table.Select())
            {
                int quantity = Int32.Parse(x["Quantity"].ToString());
                int stockLevel = Int32.Parse(x["StockLevel"].ToString());
                int requestID = Int32.Parse(x["StockRequestID"].ToString());
                Console.WriteLine("{0,-5} {1,-15} {2,-25} {3,-10} {4,-15} {5,-20}", x["StockRequestID"],x["Store"],x["Product"],x["Quantity"],x["StockLevel"], quantity>stockLevel? "False":"True" );
                if (quantity <= stockLevel)
                {
                    ID_Available.Add(requestID);
                }
                else
                {
                    ID_Unavailable.Add(requestID);
                }
            }
            Console.WriteLine();
            RequestHandle();
        }

        /// <summary>
        /// To handle the request , choose the requestID and deal with them
        /// </summary>
        private void RequestHandle()
        {
            string input = "";
            int ID = 0; // the Request ID user selected
            do
            {
                input = menu.Selection("Enter request to process: ");
                if (menu.isNumber(input))
                {
                    ID = Int32.Parse(input);
                    if (ID_Available.Contains(ID) || ID_Unavailable.Contains(ID))
                    {
                        int Q = 0; int SID = 0; int PID = 0;
                        // Loop to get the Request Details
                        var sqlCommand = "select * from StockRequest";
                        var table = dataOperation.getRequiredData(sqlCommand);
                        foreach (var x in table.Select())
                        {
                            if (x["StockRequestID"].Equals(ID))
                            {
                                Q = Int32.Parse(x["Quantity"].ToString());
                                SID = Int32.Parse(x["StoreID"].ToString());
                                PID = Int32.Parse(x["ProductID"].ToString());
                            }
                        }

                        // check the input availability
                        if (ID_Available.Contains(ID) & (!ID_Unavailable.Contains(ID)) & Q != 1)
                        {
                            updateReStock(ID,SID,PID,Q);
                        }
                        else if (ID_Available.Contains(ID) & (!ID_Unavailable.Contains(ID)) & Q == 1)
                        {
                            updateAddNewItem(ID,SID,PID,Q);
                        }
                        else if (ID_Unavailable.Contains(ID) & (!ID_Available.Contains(ID)))
                        {
                            menu.warningMsg("Current Stock is not enough! Choose others!\n");
                        }
                        ID_Available.Clear();
                        ID_Unavailable.Clear();
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
            } while (!input.Equals(""));
        }

        /// <summary>
        /// update the store inventory when dealing with a re-stock request
        /// </summary>
        /// <param name="ID">the stock request ID</param>
        /// <param name="SID">the store ID</param>
        /// <param name="PID">the product ID</param>
        /// <param name="Q">the Quantity to restock</param>
        private void  updateReStock(int ID, int SID, int PID, int Q)
        {
            //update the storeInventory 
            var updateSql =
                @"update StoreInventory set StockLevel= StockLevel+ @Q 
                                WHERE StoreID = @SID AND ProductID = @PID";
            //update the store inventory
            dataOperation.UpdateOperation(updateSql,
                                          new SqlParameter("Q", Q),
                                          new SqlParameter("SID", SID),
                                          new SqlParameter("PID", PID));

            updateAfterRequestHandle(ID, Q, PID, "Request Completed\n");
        }

        /// <summary>
        /// update the store inventory when dealing with add new item request
        /// </summary>
        /// <param name="ID">the stock request ID</param>
        /// <param name="SID">the store ID</param>
        /// <param name="PID">the product ID</param>
        /// <param name="Q">the Quantity to restock</param>
        private void updateAddNewItem(int ID, int SID, int PID, int Q)
        {
            // insert a new item in storeInventory
            string updateSql =
                @"insert into StoreInventory (StoreID,ProductID,StockLevel)
                                values(@SID,@PID,@Q)";
            //update the store inventory
            dataOperation.UpdateOperation(updateSql,
                                          new SqlParameter("SID", SID),
                                          new SqlParameter("PID", PID),
                                          new SqlParameter("Q", Q));
            updateAfterRequestHandle(ID, Q, PID, "Add New Item Request Completed\n");
        }
        /// <summary>
        /// delete the stock request after successfully approve the request
        /// update the owner inventoty ( delete the quantity)
        /// update the store inventory by passing in update sql 
        /// </summary>
        /// <param name="ID">the stock request ID</param>
        /// <param name="Q"> the quantity</param>
        /// <param name="PID">the product ID</param>
        /// <param name="msg">the message showed after excuted </param>
        private void updateAfterRequestHandle(int ID, int Q, int PID, string msg)
        {
            //delete from request table
            var updateSql = "Delete from StockRequest where StockRequestID = @ID";
            dataOperation.UpdateOperation(updateSql,new SqlParameter("ID",ID));
            //update the owner inventory
            updateSql = "update OwnerInventory set StockLevel= StockLevel - @Q WHERE ProductID = @PID";
            dataOperation.UpdateOperation(updateSql,new SqlParameter("Q",Q),new SqlParameter("PID",PID));
            menu.successMsg(msg);
        }
        
        /// <summary>
        /// Display the owner inventory
        /// </summary>
        /// <returns></returns>
        private DataTable OwnerInventory()
        {
            var sqlCommand = 
                @"SELECT Product.ProductID,Product.Name,OwnerInventory.StockLevel from OwnerInventory 
                left join Product on OwnerInventory.ProductID = Product.ProductID";
            var table = dataOperation.getRequiredData(sqlCommand);
            Console.WriteLine("Owner Inventory\n");
            Console.WriteLine("{0,-5} {1,-30} {2,-10}", "ID", "Product","Current Stock");
            foreach (var x in table.Select())
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-10}", x["ProductID"], x["Name"], x["StockLevel"]);
            }
            Console.WriteLine();
            return table;
        }

        /// <summary>
        /// Reset Stock operation
        /// </summary>
        private void ResetStock()
        {
            string name = ""; // the product name which is selected to be reset
            int stock = 0;
            Console.WriteLine("Reset Stock");
            Console.WriteLine("Product stock will be reset to 20\n");

            var table = OwnerInventory();
            //get the available ID list
            ArrayList ID_Available = new ArrayList();
            foreach (var x in table.Select())
            {
                ID_Available.Add(Int32.Parse(x["ProductID"].ToString()));
            }

            int PID = 0; // the product ID user selected
            while(true)
            {
                var input = menu.Selection("Enter product ID to reset: ");
                if (menu.isNumber(input))
                {
                    PID = Int32.Parse(input);
                    if (ID_Available.Contains(PID))
                    {
                        //get the product name by ID selected
                        foreach (var x in table.Select())
                        {
                            if (x["ProductID"].Equals(PID))
                            {
                                name = x["Name"].ToString();
                                stock = Int32.Parse(x["StockLevel"].ToString());
                            }
                        }
                        if (stock >= 20)
                        {
                            menu.warningMsg(name + " already has enough stock\n");
                            break;
                        }
                        else
                        {
                            var sqlCommand = "UPDATE OwnerInventory set stockLevel = 20 where ProductID = @PID";
                            dataOperation.UpdateOperation(sqlCommand,new SqlParameter("PID",PID));
                            menu.successMsg(name + " stock level has been reset to 20.\n");
                            break;
                        }
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
}
