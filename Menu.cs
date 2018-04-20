using System;
using System.Text.RegularExpressions;

namespace MagicInventorySystem
{
    /// <summary>
    /// This class mainly responsible for display menu and handle UI
    /// </summary>
    public class Menu
    {
        public  void MainMenu()
        {
            Console.WriteLine("Welcome to Marvelous Magic\n" +
                                          "===========================\n" +
                                          "1.   Owner\n" +
                                          "2.   Franchise Holder\n" +
                                          "3.   Customer\n" +
                                          "4.   Quit\n");
        }

        public  void OwnerMenu()
        {
            Console.WriteLine("Welcome to Marvelous Magic (Owner) \n" +
                                           "================================\n" +
                                           "1.   Display All Stock Requests\n" +
                                           "2.   Display Owner Inventory\n" +
                                           "3.   Reset Inventory Item Stock\n" +
                                           "4.   Return To Main Menu\n");
        }

        public  void StoreMenu(string storeName)
        {
            Console.WriteLine("Welcome to Marvelous Magic (Franchise Holder - " + storeName + ")\n" +
                                          "================================\n" +
                                          "1.   Display Inventory\n" +
                                          "2.   Stock Request (Threshold) \n" +
                                          "3.   Add New Inventory Item\n" +
                                          "4.   Return To Main Menu\n");
        }

        public void CustomerMenu(string storeName)
        {
            Console.WriteLine("Welcome to Marvelous Magic (Retail - " + storeName + ")\n" +
                                          "================================\n" +
                                          "1.   Display Products\n" +
                                          "2.   Return To Main Menu\n");
        }

        /// <summary>
        /// Read the input by user 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string Selection(string msg)
        {
            Console.Write(msg);
            var option = Console.ReadLine();
            Console.WriteLine();
            return option;
        }

        /// <summary>
        /// make the warning message red
        /// </summary>
        /// <param name="msg">the warning message</param>
        public void warningMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// make the warning green
        /// </summary>
        /// <param name="msg">the success message</param>
        public void successMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.Black;
        }

        /// <summary>
        /// Using regular expresion to test if this string is a number
        /// </summary>
        /// <param name="text">the string need to be tested</param>
        /// <returns></returns>
        public bool isNumber(string text)
        {
            Regex reg = new Regex("^[0-9]+$");
            Match ma = reg.Match(text);
            if (ma.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
