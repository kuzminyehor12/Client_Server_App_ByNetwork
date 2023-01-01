using Lab3.Business.Models;
using Lab3.Client.Networking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lab3.Client.Forms
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        private void LogForm_Load(object sender, EventArgs e)
        {
            FillLogs();
        }

        private void FillLogs()
        {
            dataGridView1.Rows.Clear();
            TCPClient.Send("GetLogs", out var response);

            if (response == "Server error")
            {
                MessageBox.Show("Server has been stopped!");
                return;
            }

            if (response == "Locked")
            {
                MessageBox.Show("Operation is not available because database is updating.");
                return;
            }

            var jArray = JArray.Parse(response);
            var logs = jArray.ToObject<IEnumerable<Log>>();

            foreach (var log in logs)
            {
                dataGridView1.Rows.Add(log.CommandText, log.CommandResult, log.CommandDate);
            }
        }
    }
}
