/////////////////////////////////////////////////////////////////////
// Client.cs - Demonstrate application use of channel              //
// Ver 1.0                                                         //
// Author: Karthik Bangera                                         //
// Source: Jim Fawcett, CSE681                                     //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * The Client package defines one class, Client, that uses the Comm<Client>
 * class to pass messages to a remote endpoint.
 * 
 * Required Files:
 * ---------------
 * - Client.cs
 * - ICommunicator.cs, CommServices.cs
 * - Messages.cs, MessageTest, Serialization
 *
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 10 Nov 2016
 * - first release 
 *  
 */
using System;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace CommChannelDemo
{
    ///////////////////////////////////////////////////////////////////
    // Client class demonstrates how an application uses Comm
    //
    public class Client
  {
    public Comm<Client> comm { get; set; } = new Comm<Client>();

    public string endPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8087);

    private Thread rcvThread = null;

    //----< initialize receiver >------------------------------------

    public Client()
    {
      comm.rcvr.CreateRecvChannel(endPoint);
      rcvThread = comm.rcvr.start(rcvThreadProc);
    }
    //----< join receive thread >------------------------------------

    public void wait()
    {
      rcvThread.Join();
    }
    //----< construct a basic message >------------------------------

    public Message makeMessage(string author, string fromEndPoint, string toEndPoint)
    {
      Message msg = new Message();
      msg.author = author;
      msg.from = fromEndPoint;
      msg.to = toEndPoint;
      return msg;
    }
    //----< use private service method to receive a message >--------
     
    void rcvThreadProc()
    {
      while (true)
      {
        Message msg = comm.rcvr.GetMessage();
        msg.time = DateTime.Now;
        Console.Write("\n  {0} received message:", comm.name);
        msg.showMsg();
        if (msg.body == "quit")
          break;
      }
    }
        

        public static string EncryptMessage(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            string password = "P@@Sw0rd";
            string saltkey = "S@LT&KEY";
            string vikey = "@1B2c3D4e5F6g7H8";
            byte[] keyBytes = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(saltkey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(vikey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        

        //----< run client demo >----------------------------------------

        static void Main(string[] args)
    {
      Console.Write("\n  Client ");
      Console.Write("\n =====================\n");

      Client client = new Client();

      Message msg = client.makeMessage("Karthik", client.endPoint, client.endPoint);

      msg = client.makeMessage("Karthik", client.endPoint, client.endPoint);
      msg.body = MessageTest.makeTestRequest();
      string remoteEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8088);
      msg = msg.copy();
      msg.to = remoteEndPoint;
      string author = EncryptMessage(msg.author);
      string body = EncryptMessage(msg.body);
      msg.body = body;
      msg.author = author;
      client.comm.sndr.PostMessage(msg);
      Console.Write("\n  press key to exit: ");
      Console.ReadKey();
      msg.to = client.endPoint;
      msg.body = "quit";
     // client.comm.sndr.PostMessage(msg);
      client.wait();
      Console.Write("\n\n");
    }
  }
}
