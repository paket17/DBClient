using System;
using System.Data;
using System.Net.Http.Json;
using DBClient;

class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            await DBOperations.TestConnection();
            while (true)
            {
                int operation;
                Console.WriteLine($"\n\nSelect the database operation with a number and enter it:\n" +
                    $"1 - Output the whole table\n" +
                    $"2 - Output all data by record id\n" +
                    $"3 - Add an entry to the table\n" +
                    $"4 - To change a table entry by id\n" +
                    $"5 - To delete a record in a table by identifier\n" +
                    $"6 - To exit the program");
                while (!int.TryParse(Console.ReadLine(), out operation) || operation < 1 || operation > 6)
                {
                    Console.WriteLine("Wrong number entered, try again.");
                }
                switch (operation)
                {
                    case 1:
                        {
                            await DBOperations.Print();
                            break;
                        }
                    case 2:
                        {
                            int id = OperationsHelper.GetId();
                            await DBOperations.PrintId(id);
                            break;
                        }
                    case 3:
                        {
                            string name = OperationsHelper.GetName();
                            int number = OperationsHelper.GetNumber();
                            await DBOperations.Add(new Document(name, number));
                            break;
                        }
                    case 4:
                        {
                            int id = OperationsHelper.GetId();
                            if (DBOperations.Get(id).Result != null)
                            {
                                Console.WriteLine("The document now");
                                await DBOperations.PrintId(id);
                                string name = OperationsHelper.GetName();
                                int number = OperationsHelper.GetNumber();
                                await DBOperations.Edit(id, new Document(name, number));
                            }
                            break;
                        }
                    case 5:
                        {
                            int id = OperationsHelper.GetId();
                            await DBOperations.Delete(id);
                            break;
                        }
                }
                if (operation == 6)
                {
                    Console.WriteLine("Goodbye");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadLine();
                    break;
                }
            }
        }
        catch
        {
            Console.WriteLine("Server is not available");
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
        }
    }
}