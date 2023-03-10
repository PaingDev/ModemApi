using GsmComm.GsmCommunication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModemApi
{
    class Program
    {
        

        static void Main(string[] args)
        {
            
            if (args.Length != 0)
            {
                string functionName = args[0];
                if (functionName.Equals("activePort"))
                {
                    Console.Write(ModemService.getActivePortList());
                }
                else if (functionName.Equals("sendMessage"))
                {
                    string phoneNo = args[1];
                    string portName = args[2];
                    string message = args[3];
                    message = message.Replace('&', '\n');
                    ModemService modemService = null;
                    try
                    {
                        modemService = ModemService.getInstance(portName);
                        modemService.SendMessage(phoneNo, message, false);
                       
                        Console.Write("Success");
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.Message);
                    }
                    finally
                    {
                        if(modemService != null)
                            modemService.close();
                    }
                   
                   
                }

            }
            
            //string portName = args[1];

        }
    }
}
