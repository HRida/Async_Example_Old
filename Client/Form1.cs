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

namespace Client
{
    public partial class Form1 : Form
    {
        Socket m_socClient;
        public delegate void Action();
        public Form1()
        {
            InitializeComponent();
        }

        private void cmdConnect_Click(object sender, System.EventArgs e)
        {
            try
            {
                //create a new client socket ...
                m_socClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                String szIPSelected = txtIPAddress.Text;
                String szPort = txtPort.Text;
                int alPort = System.Convert.ToInt16(szPort, 10);

                System.Net.IPAddress remoteIPAddress = System.Net.IPAddress.Parse(szIPSelected);
                System.Net.IPEndPoint remoteEndPoint = new System.Net.IPEndPoint(remoteIPAddress, alPort);
                m_socClient.Connect(remoteEndPoint);
                String szData = "Hello There";
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(szData);
                m_socClient.Send(byData);

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }


        private void cmdSendData_Click(object sender, System.EventArgs e)
        {
            try
            {
                Object objData = txtData.Text;
                byte[] byData = System.Text.Encoding.ASCII.GetBytes(objData.ToString());
                m_socClient.Send(byData);
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void cmdReceiveData_Click(object sender, System.EventArgs e)
        {
            try
            {
                byte[] buffer = new byte[1024];
                int iRx = m_socClient.Receive(buffer);
                char[] chars = new char[iRx];

                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(buffer, 0, iRx, chars, 0);
                System.String szData = new System.String(chars);
                txtDataRx.Invoke(new Action(() => { txtDataRx.Text = szData; }));
            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void cmdClose_Click(object sender, System.EventArgs e)
        {
            m_socClient.Close();
        }
    }
}