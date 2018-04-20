using System;

namespace MagicInventorySystem
{
    /// <summary>
    /// The  class Drive ( has main method)
    /// </summary>
    class Driver
    {
        /// <summary>
        /// Main Method (The Outside LOOP)
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("Author: Yingzhi Lu, Bei'er Nie           Time: 23/03/2018");
            Console.WriteLine("Instruction: Press ENTER at any time will bring you back to menu :)\n");
            Menu menu = new Menu();

            string input = "";
            while (!input.Equals("4"))
            {
                Driver driver = new Driver();
                menu.MainMenu();
                input = menu.Selection("Enter an option:");
                switch (input)
                {
                    case "1":
                        User owner = new Owner();
                        owner.LoopMenu();
                        break;
                    case "2":
                        Franchise franchise = new Franchise();
                        franchise.SelectAStore();
                        break;
                    case "3":
                        Customer customer = new Customer();
                        customer.SelectAStore();
                        break;
                    case "4":
                        menu.successMsg("Thanks for coming Marvelous Magic ! Have a nice day\n");
                        Environment.Exit(0);
                        break;
                    default:
                        menu.warningMsg("INVALID SELECT!\n");
                        break;
                }
            }
        }
    }
}
