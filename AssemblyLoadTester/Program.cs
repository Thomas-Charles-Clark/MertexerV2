using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AssemblyLoadTester
{
    class Program
    {
        static void Main(string[] args)
        {
            var asm = Assembly.LoadFrom("C:\\MertexerV2DLLs\\MertexerV2.dll");
            var allocationType = asm.GetType("MertexerV2.Controllers.AllocationController");

            object[] ctorParams = new object[1];

            ctorParams[0] = "PLAY";

            var allocationController = Activator.CreateInstance(allocationType, ctorParams);

            var allocationCreationMethod = allocationType.GetMethod("CreatePreAllocation");

            var allocationRemovalMethod = allocationType.GetMethod("RemovePreAllocation");

            var expiryDate = DateTime.Now.AddDays(2);

            string result = "";

            result = (string)allocationCreationMethod.Invoke(allocationController, new object[]
            {
                "1234",
                "P619/05",
                "WH",
                "1.0",
                "ROL",
                null,
                null,
                expiryDate.ToString("dd/MM/yyyy"),
                expiryDate.ToString("HH:mm:ss"),
                true
            });

            Console.WriteLine($"RESULT {result}");

            result = (string)allocationRemovalMethod.Invoke(allocationController, new object[] 
            {
                "1234",
                true
            });

            Console.WriteLine($"RESULT {result}");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
