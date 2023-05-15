using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBClient
{
    static public class OperationsHelper
    {
        static public int GetId()
        {
            Console.WriteLine("Enter an id to search for an entry.");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Wrong id entered, try again.");
            }
            return id;
        }

        static public string GetName()
        {
            Console.WriteLine("Enter the document name.");
            string name = Console.ReadLine();
            while (name.Length == 0)
            {
                Console.WriteLine("The name must not be empty, try again.");
                name = Console.ReadLine();
            }
            return name;
        }

        static public int GetNumber()
        {
            Console.WriteLine("Enter the document number");
            int number;
            while (!int.TryParse(Console.ReadLine(), out number))
            {
                Console.WriteLine("Wrong number entered, try again.");
            }
            return number;
        }
    }
}
