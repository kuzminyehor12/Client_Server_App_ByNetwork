using Lab3.Business.Models;
using Lab3.Client.Networking;
using Lab3.Server.Enums;
using Newtonsoft.Json.Linq;
using PawnShop.Forms.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lab3.Client.Forms
{
    public partial class SecondaryEmployeeForm : Form
    {
        public SecondaryEmployeeForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            EmployeeForm employeeForm = new EmployeeForm();
            this.OpenChildForm(employeeForm, this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Specialization specialization = null;
                Group group = null;
                var response = string.Empty;

                if (listBox1.SelectedIndex != -1)
                {
                    TCPClient.Send($"GetSpecializationByName {listBox1.Items[listBox1.SelectedIndex]}", out response);

                    if (response == "Server error")
                    {
                        MessageBox.Show("Server has been stopped!");
                        return;
                    }

                    var jsonSpecialization = JObject.Parse(response);
                    specialization = jsonSpecialization.ToObject<Specialization>();
                }

                if (listBox2.SelectedIndex != -1)
                {
                    TCPClient.Send($"GetGroupByName {listBox2.Items[listBox2.SelectedIndex]}", out response);

                    if (response == "Server error")
                    {
                        MessageBox.Show("Server has been stopped!");
                        return;
                    }

                    var jsonGroup = JObject.Parse(response);
                    group = jsonGroup.ToObject<Group>();
                }

                var employee = new Employee
                {
                    FirstName = textBox1.Text,
                    LastName = textBox2.Text,
                    DateOfBirth = dateTimePicker1.Value,
                    Address = textBox3.Text,
                    SpecializationId = specialization?.Id,
                    GroupId = group?.Id
                };

                TCPClient.Send($"AddEmployee {JObject.FromObject(employee)}", out response);

                if (response == "Server error")
                {
                    MessageBox.Show("Server has been stopped!");
                    return;
                }

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Employee added successfully!");
                    EmployeeForm employeeForm = new EmployeeForm();
                    this.OpenChildForm(employeeForm, this);
                }
                else if(response == ResponseResult.Failure.ToString())
                {
                    MessageBox.Show("There was a mistake adding employee.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SecondaryEmployeeForm_Load(object sender, EventArgs e)
        {
            FillSpecialization();
            FillGroups();
        }

        private void FillSpecialization()
        {
            TCPClient.Send("GetSpecializations", out var response);
            var jArray = JArray.Parse(response);
            var specializations = jArray.ToObject<IEnumerable<Specialization>>();

            foreach (var specialization in specializations)
            {
                listBox1.Items.Add(specialization.Name);
            }
        }

        private void FillGroups()
        {
            TCPClient.Send("GetGroups", out var response);
            var jArray = JArray.Parse(response);
            var groups = jArray.ToObject<IEnumerable<Group>>();

            foreach (var group in groups)
            {
                listBox2.Items.Add(group.Name);
            }
        }
    }
}
