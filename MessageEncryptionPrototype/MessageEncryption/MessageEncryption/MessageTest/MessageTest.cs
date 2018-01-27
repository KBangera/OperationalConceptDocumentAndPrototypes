/////////////////////////////////////////////////////////////////////
// MessageTest.cs - defines specialized communication messages     //
// ver 1.0                                                         //
// Jim Fawcett, CSE681 - Software Modeling and Analysis, Fall 2016 //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * Messages provides helper code for building and parsing XML messages.
 *
 * Required files:
 * ---------------
 * - Messages.cs
 * 
 * Maintanence History:
 * --------------------
 * ver 1.0 : 16 Oct 2016
 * - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace CommChannelDemo
{
  public class TestElement
  {
    public string testName { get; set; }
    public string testDriver { get; set; }
    public List<string> testCodes { get; set; } = new List<string>();

    public TestElement() { }
    public TestElement(string name)
    {
      testName = name;
    }
    public void addDriver(string name)
    {
      testDriver = name;
    }
    public void addCode(string name)
    {
      testCodes.Add(name);
    }
    public override string ToString()
    {
      string te = "\ntestName:\t" + testName;
      te += "\ntestDriver:\t" + testDriver;
      foreach (string code in testCodes)
      {
        te += "\ntestCode:\t" + code;
      }
      return te += "\n";
    }
  }
  public class TestRequest
  {
    public TestRequest() { }
    public string author { get; set; }
    public List<TestElement> tests { get; set; } = new List<TestElement>();

    public override string ToString()
    {
      string tr = "\nAuthor:\t" + author + "\n";
      foreach (TestElement te in tests)
      {
        tr += te.ToString();
      }
      return tr;
    }
  }

  public class MessageTest
  {
    public static string makeTestRequest()
    {
      TestElement te1 = new TestElement("test1");
      te1.addDriver("td1.dll");
      te1.addCode("t1.dll");
      te1.addCode("t2.dll");
      TestElement te2 = new TestElement("test2");
      te2.addDriver("td2.dll");
      te2.addCode("tc3.dll");
      te2.addCode("tc4.dll");
      TestRequest tr = new TestRequest();
      tr.author = "Karthik Bangera";
      tr.tests.Add(te1);
      tr.tests.Add(te2);
      return tr.ToXml();
    }
#if (TEST_MESSAGETEST)
    static void Main(string[] args)
    {
      Message msg = new Message();
      msg.to = "http://localhost:8080/ICommunicator";
      msg.from = "http://localhost:8081/ICommunicator";
      msg.author = "Karthik";
      msg.type = "TestRequest";

      Console.Write("\n  Testing Message with Serialized TestRequest");
      Console.Write("\n ---------------------------------------------\n");
      TestElement te1 = new TestElement("test1");
      te1.addDriver("td1.dll");
      te1.addCode("tc1.dll");
      te1.addCode("tc2.dll");
      TestElement te2 = new TestElement("test2");
      te2.addDriver("td2.dll");
      te2.addCode("tc3.dll");
      te2.addCode("tc4.dll");
      TestRequest tr = new TestRequest();
      tr.author = "Karthik Bangera";
      tr.tests.Add(te1);
      tr.tests.Add(te2);
      msg.body = tr.ToXml();

      Console.Write("\n  Serialized TestRequest:");
      Console.Write("\n -------------------------\n");
      Console.Write(msg.body.shift());

      Console.Write("\n  TestRequest Message:");
      Console.Write("\n ----------------------");
      msg.showMsg();

      Console.Write("\n  Testing Deserialized TestRequest");
      Console.Write("\n ----------------------------------\n");
      TestRequest trDS = msg.body.FromXml<TestRequest>();
      Console.Write(trDS.showThis());
    }
#endif
  }
}
