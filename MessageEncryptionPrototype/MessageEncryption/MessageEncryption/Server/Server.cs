/////////////////////////////////////////////////////////////////////
// Server.cs - Demonstrate application use of channel              //
// Ver 1.0                                                         //
// Author: Karthik Bangera                                         //
// Source: Jim Fawcett, CSE681 - Software Modeling and Analysis    //
/////////////////////////////////////////////////////////////////////
/*
 * Package Operations:
 * -------------------
 * The Server package defines one class, Server, that uses the Comm<Server>
 * class to receive messages from a remote endpoint.
 * 
 * Required Files:
 * ---------------
 * - Server.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Utilities;
using System.Security.Cryptography;
using System.IO;

namespace CommChannelDemo
{
  class Server
  {
    public Comm<Server> comm { get; set; } = new Comm<Server>();

    public string endPoint { get; } = Comm<Server>.makeEndPoint("http://localhost", 8088);

    private Thread rcvThread = null;

    public Server()
    {
      comm.rcvr.CreateRecvChannel(endPoint);
      rcvThread = comm.rcvr.start(rcvThreadProc);
    }

    public void wait()
    {
      rcvThread.Join();
    }
    public Message makeMessage(string author, string fromEndPoint, string toEndPoint)
    {
      Message msg = new Message();
      msg.author = author;
      msg.from = fromEndPoint;
      msg.to = toEndPoint;
      return msg;
    }
        

        public static string DecryptMessage(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            string password = "P@@Sw0rd";
            string saltkey = "S@LT&KEY";
            string vikey = "@1B2c3D4e5F6g7H8";
            byte[] keyBytes = new Rfc2898DeriveBytes(password, Encoding.ASCII.GetBytes(saltkey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(vikey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }
        void rcvThreadProc()
    {
      while (true)
      {
        Message msg = comm.rcvr.GetMessage();
        msg.time = DateTime.Now;
        Console.Write("\n  {0} received encrypted message:", comm.name);
        msg.showMsg();
        if (msg.body == "quit")
                {
                    break;
                }
        else
                {
                    // Console.WriteLine("\n Decrypting the message author:\n");
                    //string author = msg.author;
                    //string decryptAuthor = DecryptMessage(author);
                    //Console.Write(decryptAuthor);
                    //Console.WriteLine("\n Decrypting the message from:\n");
                    //string from = msg.from;
                    //string decryptFrom = DecryptMessage(from);
                    //Console.Write(decryptFrom);
                    //Console.WriteLine("\n Decrypting the message to:\n");
                    //string to = msg.to;
                    //string decryptTo = DecryptMessage(to);
                    //Console.Write(decryptTo);
                    Console.WriteLine("\n Decrypting the message body:\n");
                    string body = msg.body;
                    string decryptBody = DecryptMessage(body);
                    Console.Write(decryptBody);

                }
        }
    }
    static void Main(string[] args)
    {
      Console.Write("\n  Server");
      Console.Write("\n =====================\n");

      Server Server = new Server();

      Message msg = Server.makeMessage("Karthik", Server.endPoint, Server.endPoint);
      Console.Write("\n  press key to exit: ");
      Console.ReadKey();
      msg.to = Server.endPoint;
      msg.body = "quit";
      Server.comm.sndr.PostMessage(msg);
      Server.wait();
      Console.Write("\n\n");
    }
  }
}
