using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingConsoleApp
{
    internal class TestClass1 : ITestClass
    {
        public void HelloWorld()
        {
            Console.WriteLine("I refuse to say hello world!");
        }
    }
}
