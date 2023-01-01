using Checkers.Server.Networking;
using Lab3.Client.Forms;
using Lab3.Client.Networking;
using PawnShop.Forms.Extensions;
using PawnShop.Forms.Forms.BaseForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Lab3.Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }

        private void ClientForm_Load(object sender, EventArgs e)
        {
            AuthorizationForm authForm = new AuthorizationForm();
            authForm.VisibleChanged += AuthForm_VisibleChanged;
            authForm.FormClosed += AuthForm_FormClosed;
            authForm.Show();
        }

        private void AuthForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        private void AuthForm_VisibleChanged(object sender, EventArgs e)
        {
            this.Enabled = !this.Enabled;

            if (Enabled)
            {
                try
                {
                    TCPClient.Connect();
                }
                catch (SocketException)
                {
                    MessageBox.Show("Server is not started");
                }
            }
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            TCPClient.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new EmployeeForm(), splitContainer1.Panel2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new GroupForm(), splitContainer1.Panel2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(new LogForm(), splitContainer1.Panel2);
        }
    }
}
