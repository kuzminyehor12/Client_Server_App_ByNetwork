using Lab3.Business.Models;
using Lab3.Client.Networking;
using Lab3.Server.Enums;
using Newtonsoft.Json.Linq;
using PawnShop.Forms.Extensions;
using System;
using System.Windows.Forms;

namespace Lab3.Client.Forms
{
    public partial class SecondaryGroupForm : Form
    {
        public SecondaryGroupForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var group = new Group
                {
                    Name = textBox1.Text,
                    Description = richTextBox1.Text,
                    CreationDate = dateTimePicker1.Value
                };

                TCPClient.Send($"AddGroup {JObject.FromObject(group)}", out var response);

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

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Group added successfully!");
                    GroupForm groupForm = new GroupForm();
                    this.OpenChildForm(groupForm, this);
                }
                else if (response == ResponseResult.Failure.ToString())
                {
                    MessageBox.Show("There was a mistake adding group.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            GroupForm groupForm = new GroupForm();
            this.OpenChildForm(groupForm, this);
        }
    }
}
