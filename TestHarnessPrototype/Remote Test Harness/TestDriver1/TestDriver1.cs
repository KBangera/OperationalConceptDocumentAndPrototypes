﻿/////////////////////////////////////////////////////////////////////////////
//  TestDriver1.cs - Tests the code in CodeToTest1                         //
//  Application:  Remote Test Harness                                      //
//  Author:       Karthik Bangera                                          //
//  Version:      2.0                                                      //
/////////////////////////////////////////////////////////////////////////////
/*
*   Test driver needs to know the types and their interfaces
*   used by the code it will test.  It doesn't need to know
*   anything about the test harness.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDemo
{
    using TestExec;

    public class TestDriver1 : ITest
    {
        private CodeToTest1 code;  // will be compiled into separate DLL
        StringBuilder sb = new StringBuilder();
        //----< Testdriver constructor >---------------------------------
        /*
        *  For production code the test driver may need the tested code
        *  to provide a creational function.
        */
        public TestDriver1()
        {
            code = new CodeToTest1();
        }
        //----< factory function >---------------------------------------
        /*
        *   This can't be used by any code that doesn't know the name
        *   of this class.  That means the TestHarness will need to
        *   use reflection - ugh!
        *
        *   The language gives us this problem because it won't
        *   allow a static method in an interface or abstract class.
        */
        public static ITest create()
        {
            return new TestDriver1();
        }
        //----< test method is where all the testing gets done >---------

        public bool test()
        {
            bool check = true;
            try
            {

                int value = code.palindrome("Radar");
                if (value == 0)
                {
                    sb.AppendLine("\n Radar is not a palindrome");
                    sb.AppendLine("\n Test code tested succesfully");
                }
                else if (value == 1)
                {
                    sb.AppendLine("\n Radar is a palindrome");
                    sb.AppendLine("\n Test code tested succesfully");
                }
                return check;
            }
            catch (Exception ex)
            {
                sb.AppendLine("\n Exception thrown: " + ex.Message);
                check = false;
                return check;
            }

        }

        public string getLog()
        {
            return sb.ToString();
        }
        //----< test stub - not run in test harness >--------------------

        static void Main(string[] args)
        {
            Console.Write("\nLocal test:\n");

            ITest test = TestDriver1.create();

            if (test.test() == true)
                Console.Write("\nTest passed");
            else
                Console.Write("\nTest failed");
            Console.Write("\n");

        }
    }
}
