using GsmComm.GsmCommunication;
using GsmComm.PduConverter;
using GsmComm.PduConverter.SmartMessaging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace ModemApi
{
    class ModemService
    {
        static ModemService instance;
        GsmCommMain comm;

        private ModemService(string portName)
        {
            comm = new GsmCommMain(portName, 9600, 600);
            try
            {
                comm.Open();
                
               
            }
            
            catch (Exception ex)
            {
                throw new Exception("Cannot Open Port", ex);
            }
        }

        public static ModemService getInstance(string portName)
        {
            if (instance == null)
            {
                instance = new ModemService(portName);
            }
            return instance;
        }

        public static String getPortList()
        {
            string jsonPortList = "[data]";
            string[] ports = SerialPort.GetPortNames();
            for(int i=0; i<ports.Length ;i++){
                ports[i] = "\""+ports[i]+"\"";
            }
            string rawData = String.Join(",", ports);
            jsonPortList = jsonPortList.Replace("data", rawData);
            return jsonPortList;
        }

        public static String getActivePortList()
        {
            string jsonPortList = "[data]";
            string[] ports = SerialPort.GetPortNames();

            List<string> listActivePort = new List<string>();
            for (int i = 0; i < ports.Length; i++)
            {
                ModemService modemService = ModemService.getInstance(ports[i]);
                if (modemService.checkPort())
                {
                    //ports[i] = "\"" + ports[i] + "\"";
                    listActivePort.Add("\"" + ports[i] + "\"");
                }
                modemService.close();
            }
            string rawData = String.Join(",", listActivePort);
            jsonPortList = jsonPortList.Replace("data", rawData);
            return jsonPortList;
        }

        private bool checkPort()
        {
            return comm.IsConnected();
        }

        public void close()
        {
            if (comm.IsOpen())
            {
                comm.Close();
            }
        }

        public void SendMessage(string phoneNo, string ms, bool unicode)
        {
            SmsSubmitPdu[] pdus = CreateConcatMessage(ms, phoneNo,
                unicode, false);
            if (pdus != null)
                SendMultiple(pdus);
        }

        private void SendMultiple(SmsSubmitPdu[] pdus)
        {
            int num = pdus.Length;
            try
            {
                foreach (OutgoingSmsPdu pdu in pdus)
                {
                    comm.SendMessage(pdu, true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private SmsSubmitPdu[] CreateConcatMessage(string message, string number, bool unicode, bool showParts)
        {
            SmsSubmitPdu[] pdus = null;
            try
            {
                if (!unicode)
                {
                    pdus = SmartMessageFactory.CreateConcatTextMessage(message, number);

                }
                else
                {
                    pdus = SmartMessageFactory.CreateConcatTextMessage(message, true, number);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return null;
            }

            if (pdus.Length == 0)
            {
                return null;
            }

            return pdus;
        }


    }
}
