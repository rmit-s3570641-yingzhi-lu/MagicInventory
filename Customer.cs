using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace MagicInventorySystem
{
    /// <summary>
    /// Class Customer 
    /// Doing function purchase items
    /// </summary>
    public class Customer :User
    {
        Menu menu = new Menu();
        DatabaseOperation dataOperation = new DatabaseOperation();
        string name = ""; //the store name
        int storeID = 0; // the store ID
        ArrayList ID_Available = new ArrayList(); // To instore the available id for input validation
        const int pageSize = 3; // the number of item be showed in one page

        /// <summary>
        /// A public method for other class to call
        /// </summary>
        public void SelectAStore()
        {
            StoresSelect();
        }

        /// <summary>
        /// the loop menu function
        /// </summary>
        public override void LoopMenu()
        {
            string option = "";
            while (!option.Equals("2"))
            {
                menu.CustomerMenu(name);
                option = menu.Selection("Enter an option: ");
                switch (option)
                {
                    case "1":
                        DisplayProducts();
                        break;
                    case "2":
                        Console.WriteLine();
                        break;
                    default:
                            menu.warningMsg("INVALID INPUT!\n");
                        break;
                }
            }
        }

        /// <summary>
        /// Get the store inventory by store ID
        /// </summary>
        /// <returns></returns>
        private DataTable getInventory()
        {
            var sqlcommand = 
                @"select Product.ProductID,Product.Name, StoreInventory.StockLevel from 
                StoreInventory left join Product on Product.ProductID = StoreInventory.ProductID 
                Where StoreInventory.StoreID = @storeID";
            var table = dataOperation.getRequiredData(sqlcommand,new SqlParameter("storeID",storeID));
            return table;
        }

        /// <summary>
        /// Display the inventory in format 
        /// </summary>
        /// <param name="table">the datatable need to be displayed</param>
        private void DisplayInventory(DataTable table)
        {
            Console.WriteLine("Inventory\n");
            Console.WriteLine("{0,-5} {1,-30} {2,-10}", "ID", "Product", "Current Stock");
            foreach (var x in table.Select())
            {
                Console.WriteLine("{0,-5} {1,-30} {2,-10}", x["ProductID"], x["Name"], x["StockLevel"]);
            }
            //retrieve from the database to get inventory
            Console.WriteLine();
        }

        /// <summary>
        /// To display the product
        /// Let customer to purchase 
        /// Navigate between pages
        /// Return to menu
        /// </summary>
        private void DisplayProducts()
        {
            var dt = getInventory();
            int quantity = 0; // the quantity of item user want to buy
            int pageIndex = 1; // the current page index of paged table
            int parkingCount = dt.Rows.Count;// the total rows of current store inventory
            int pageCount = 0; // the total number of pages can be divided into of this table
            //count the total pages
            if (parkingCount % pageSize == 0)
            {
                pageCount = parkingCount / pageSize;
            }
            else
            {
                pageCount = parkingCount / pageSize + 1;
            }
            // do-while loop to show the inventory 
            do
            {
                dt = getInventory();
                nextPageInventory(dt, pageIndex);
                string input = menu.Selection("Enter Product ID to purchase or function: ");

                if (ID_Available.Contains(input) & !input.Equals("R") & !input.Equals("N"))//Input the available ID
                {
                    string inputNumber = menu.Selection("Enter quantity to purchase: ");// the number input by user
                    if (menu.isNumber(inputNumber))
                    {
                        quantity = Int32.Parse(inputNumber);
                    }
                    else if (inputNumber.Equals(""))
                    {
                        menu.warningMsg("Enter pressed, exit to menu \n");
                        return;
                    }else
                    {
                        menu.warningMsg("INVALID QUANTITY!\n");
                        continue;
                    }
                    var table = getInventorySelected(input);
                    int stockQuantity = Int32.Parse(table.Rows[0]["StockLevel"].ToString());
                    string pname = table.Rows[0]["name"].ToString();
                    if (quantity>stockQuantity)
                    {
                        menu.warningMsg(pname + " doesn't have enough stock to fulfill the purchase.\n");
                    }
                    else if(quantity <= stockQuantity)
                    {
                        updateInventoryStock(input, quantity);
                        menu.successMsg("Purchased " + quantity + " of " + pname + "\n");
                    }
                }
                else if ((!ID_Available.Contains(input)) & !input.Equals("R") & input.Equals("N"))// function next page
                {
                    pageIndex++;
                    if (pageIndex <= pageCount)
                    {
                        Console.WriteLine();
                    }
                    else
                    {
                        pageIndex = 1;
                    }
                }
                else if ((!ID_Available.Contains(input)) & input.Equals("R") & !input.Equals("N"))// function retrun to menu
                {
                    Console.WriteLine();
                    return;
                }
                else //other case, for example, enter pressed/invalid input
                {
                    if (input.Equals(""))
                    {
                        menu.warningMsg("Enter pressed, exit to menu \n");
                        return;
                    }
                    else if(!menu.isNumber(input))
                    {
                        menu.warningMsg("INVALID INPUT!\n");
                    }else if (menu.isNumber(input))
                    {
                        menu.warningMsg("NO SUCH ID! TRY NEXT PAGE!\n");
                    }                    
                }
            } while (pageIndex <= pageCount);
        }

        /// <summary>
        /// Update the store inventory after purchased
        /// </summary>
        /// <param name="PID">product ID customer selected</param>
        /// <param name="Q">the amount of item customer want to buy</param>
        private void updateInventoryStock(string PID, int Q)
        {
            //update the store inventory
            var updateSql = 
                @"update StoreInventory set StockLevel= StockLevel - @Q 
                WHERE ProductID = @PID AND StoreID = @storeID";
            dataOperation.UpdateOperation(updateSql,
                                          new SqlParameter("Q",Q),
                                          new SqlParameter("PID",PID),
                                          new SqlParameter("storeID",storeID));
        }

        /// <summary>
        /// Get the selected item details from the store inventory
        /// </summary>
        /// <param name="PID">product ID customer selected</param>
        /// <param name="Q">the amount of item customer want to buy</param>
        /// <returns>the selected product</returns>
        private DataTable getInventorySelected(string PID)
        {
            var Sql =
            @"select Product.ProductID,Product.Name, StoreInventory.StockLevel
            from StoreInventory left join Product on Product.ProductID = StoreInventory.ProductID
            Where StoreInventory.StoreID = @storeID AND StoreInventory.ProductID = @PID";
            return dataOperation.getRequiredData(Sql,
                                                 new SqlParameter("storeID",storeID),
                                                 new SqlParameter("PID",PID));
        }

        /// <summary>
        /// To get separated pages one by one
        /// </summary>
        /// <param name="dt">the original dataTable for paging</param>
        /// <param name="pageIndex">the current index of separated pages</param>
        private void nextPageInventory(DataTable dt, int pageIndex )
        {
            var newdt = GetPagedTable(dt, pageIndex); // get the first paged inventory
            DisplayInventory(newdt);// display the first 3 items
            AddAvailablePID(newdt);// add available product ID for validation
            Console.WriteLine("[Legend: 'N' Next Page |  'R' Return To Menu]\n");
        }

        /// <summary>
        /// Add Available Product ID by loop 
        /// </summary>
        /// <param name="dt">current datatable need to be operated</param>
        private void AddAvailablePID(DataTable dt)
        {
            ID_Available.Clear();
            foreach(var x in dt.Select())
            {
                ID_Available.Add(x["ProductID"].ToString());
            }
        }

        /// <summary>
        /// Handle the DataTable page operation
        /// </summary>
        /// <param name="dt">the DataTable need to be paged</param>
        /// <param name="PageIndex">current page</param>
        /// <returns>already paged Datatable</returns>
        
        private DataTable GetPagedTable(DataTable dt, int PageIndex)
        {
            if (PageIndex == 0)
                return dt;//0 represents current data, return instantly

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy the structure of dt 

            int rowbegin = (PageIndex - 1) * pageSize;
            int rowend = PageIndex * pageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;// if the total rows of current showed table, return newdt instantly

            if (rowend > dt.Rows.Count)// if the end of current table lager than the total rows of table
                rowend = dt.Rows.Count;// set the end row equals to the total rows of table

            for (int i = rowbegin; i <= rowend - 1; i++)// migerate the data and structure to new datatable
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }


        /// <summary>
        /// To select the store 
        /// </summary>
        private void StoresSelect()
        {
            var sqlcommand = "select * from Store";
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
                }else if (!ID_Available.Contains(input) & input.Equals("") & !menu.isNumber(input))
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
    }
}
