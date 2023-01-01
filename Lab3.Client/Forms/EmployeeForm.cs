using Checkers.Server.Networking;
using Lab3.Business.Models;
using Lab3.Client.Models;
using Lab3.Client.Networking;
using Lab3.Server.Enums;
using Newtonsoft.Json.Linq;
using PawnShop.Forms.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Lab3.Client.Forms
{
    public partial class EmployeeForm : Form
    {
        public EmployeeForm()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            var addingForm = new SecondaryEmployeeForm();
            this.OpenChildForm(addingForm, this);
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            FillSpecalizations();
            FillGroups();
        }

        private void FillSpecalizations()
        {
            TCPClient.Send("GetSpecializations", out var response);

            if (response == "Server error")
            {
                MessageBox.Show("Server has been stopped!");
                return;
            }

            var jArray = JArray.Parse(response);
            var specializations = jArray.ToObject<IEnumerable<Specialization>>();
            var comboItems = new List<ComboItem>();
            comboItems.Add(new ComboItem { Text = string.Empty, Value = string.Empty });

            foreach (var specialization in specializations)
            {
                comboItems.Add(new ComboItem { Text = specialization.Name, Value = specialization.Id.ToString() });
            }

            comboBox1.DataSource = comboItems;
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
        }

        private void FillGroups()
        {
            TCPClient.Send("GetGroups", out var response);

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
            var groups = jArray.ToObject<IEnumerable<Group>>();
            var comboItems = new List<ComboItem>();
            comboItems.Add(new ComboItem { Text = string.Empty, Value = string.Empty });

            foreach (var group in groups)
            {
                comboItems.Add(new ComboItem { Text = group.Name, Value = group.Id.ToString() });
            }

            comboBox2.DataSource = comboItems;
            comboBox2.DisplayMember = "Text";
            comboBox2.ValueMember = "Value";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            TCPClient.Send("GetEmployeesWithDetails", out var response);

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
            var employees = jArray.ToObject<IEnumerable<Employee>>();

            foreach (var employee in employees)
            {
                var employeeAge = DateTime.Now.Year - employee.DateOfBirth.Year;
                dataGridView1.Rows.Add(employee.Id, employee.FirstName, employee.LastName, employeeAge,
                    employee.Address, employee.SpecalizationName, employee.GroupName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var response = string.Empty;

                if (dataGridView1.SelectedRows == null)
                {
                    MessageBox.Show("There is nothing to delete.", "Delete Message", MessageBoxButtons.OK);
                    return;
                }

                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    TCPClient.Send($"GetEmployeeById {row.Cells["Id"].EditedFormattedValue}", out response);

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

                    var jObject = JObject.Parse(response);
                    var existingEmployee = jObject.ToObject<Employee>();
                    TCPClient.Send($"DeleteEmployee {JObject.FromObject(existingEmployee)}", out response);

                    if (response == "Locked")
                    {
                        MessageBox.Show("Operation is not available because database is updating.");
                        return;
                    }
                }

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Employee has been deleted successfully!", "Delete", MessageBoxButtons.OK);
                }
                else if (response == ResponseResult.Failure.ToString())
                {
                    MessageBox.Show("There was a mistake in deleting");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            TCPClient.Send("GetEmployeesWithDetails", out var response);

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
            var employees = jArray.ToObject<IEnumerable<Employee>>();

            if(!string.IsNullOrEmpty(comboBox1.Text))
            {
                employees = employees.Where(e => e.SpecalizationName == comboBox1.Text);
            }

            if (!string.IsNullOrEmpty(comboBox2.Text))
            {
                employees = employees.Where(e => e.GroupName == comboBox2.Text);
            }

            if(dateTimePicker1.Value != DateTime.Now)
            {
                employees = employees.Where(e => e.DateOfBirth <= dateTimePicker1.Value);
            }

            foreach (var employee in employees)
            {
                var employeeAge = DateTime.Now.Year - employee.DateOfBirth.Year;
                dataGridView1.Rows.Add(employee.Id, employee.FirstName, employee.LastName, employeeAge,
                    employee.Address, employee.SpecalizationName, employee.GroupName);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            var dataRow = dataGridView1.Rows[e.RowIndex];

            if (!IsUpdateValid(dataRow))
            {
                MessageBox.Show("Data was input incorrectly", "Error", MessageBoxButtons.OK);
                return;
            }

            dataGridView1.Update();

            try
            {
                TCPClient.Send($"GetEmployeeById {dataRow.Cells["Id"].EditedFormattedValue}", out var response);

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

                var jObject = JObject.Parse(response);
                var existingEmployee = jObject.ToObject<Employee>();

                var updatingEmployee = new Employee
                {
                    Id = existingEmployee.Id,
                    FirstName = dataRow.Cells["FirstName"].EditedFormattedValue.ToString(),
                    LastName = dataRow.Cells["LastName"].EditedFormattedValue.ToString(),
                    Address = dataRow.Cells["Address"].EditedFormattedValue.ToString(),
                    DateOfBirth = existingEmployee.DateOfBirth,
                    SpecializationId = existingEmployee.SpecializationId,
                    GroupId = existingEmployee.GroupId
                };

                TCPClient.Send($"UpdateEmployee {JObject.FromObject(updatingEmployee)}", out response);

                if (response == "Locked")
                {
                    MessageBox.Show("Operation is not available because database is updating.");
                    return;
                }

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Employee has been updated successfully!");
                }
                else if(response == ResponseResult.Failure.ToString())
                {
                    MessageBox.Show("There was a mistake in updating");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool IsUpdateValid(DataGridViewRow dataRow)
        {
            return !string.IsNullOrEmpty(dataRow.Cells["FirstName"].EditedFormattedValue.ToString())
                && !string.IsNullOrEmpty(dataRow.Cells["LastName"].EditedFormattedValue.ToString())
                && !string.IsNullOrEmpty(dataRow.Cells["Address"].EditedFormattedValue.ToString());
        }
    }
}
