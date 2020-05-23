using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    public partial class Form1 : Form
    {
        public AsyncCallback pfnWorkerCallBack;
        public Socket m_socListener;
        public Socket m_socWorker;
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdListen_Click(object sender, EventArgs e)
        {
            try
            {
                //create the listening socket...
                m_socListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipLocal = new IPEndPoint(IPAddress.Any, 8221);
                //bind to local IP Address...
                m_socListener.Bind(ipLocal);
                //start listening...
                m_socListener.Listen(4);
                // create the call back for any client connections...
                m_socListener.BeginAccept(new AsyncCallback(OnClientConnect), null);
                cmdListen.Enabled = false;

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
        public void OnClientConnect(IAsyncResult asyn)
        {
            try
            {
                m_socWorker = m_socListener.EndAccept(asyn);

                WaitForData(m_socWorker);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\n OnClientConnection: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }

        }
        public class CSocketPacket
        {
            public System.Net.Sockets.Socket thisSocket;
            public byte[] dataBuffer = new byte[1];
        }

        public void WaitForData(System.Net.Sockets.Socket soc)
        {
            try
            {
                if (pfnWorkerCallBack == null)
                {
                    pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
                }
                CSocketPacket theSocPkt = new CSocketPacket();
                theSocPkt.thisSocket = soc;
            
                soc.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }

        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSockId = (CSocketPacket)asyn.AsyncState;
                //end receive...
                int iRx = 0;
                iRx = theSockId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSockId.dataBuffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);

                txtDataRx.Invoke(new Action(() => { txtDataRx.Text = txtDataRx.Text + szData; }));

                WaitForData(m_socWorker);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "\nOnDataReceived: Socket has been closed\n");
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Object objData = txtDataTx.Text;
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                m_socWorker.Send(byData);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }
    }
}


