using Checkers.Server.Networking;
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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3.Client.Forms
{
    public partial class GroupForm : Form
    {
        public GroupForm()
        {
            InitializeComponent();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            var addingForm = new SecondaryGroupForm();
            this.OpenChildForm(addingForm, this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            TCPClient.Send("GetGroupsWithDetails", out var response);

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

            foreach (var group in groups)
            {
                dataGridView1.Rows.Add(group.Id, group.Name, group.Description, group.CreationDate, group.ParticipantsCount);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();

            TCPClient.Send("GetGroupsWithDetails", out var response);

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

            groups = groups.Where(g => g.ParticipantsCount >= numericUpDown1.Value);

            if (dateTimePicker1.Value != DateTime.Now)
            {
                groups = groups.Where(g => g.CreationDate <= dateTimePicker1.Value);
            }

            foreach (var group in groups)
            {
                dataGridView1.Rows.Add(group.Id, group.Name, group.Description, group.CreationDate, group.ParticipantsCount);
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
                    TCPClient.Send($"GetGroupById {row.Cells["Id"].EditedFormattedValue}", out response);

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
                    var existingGroup = jObject.ToObject<Group>();
                    TCPClient.Send($"DeleteGroup {JObject.FromObject(existingGroup)}", out response);

                    if (response == "Locked")
                    {
                        MessageBox.Show("Operation is not available because database is updating.");
                        return;
                    }
                }

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Group has been deleted successfully!", "Delete", MessageBoxButtons.OK);
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
                TCPClient.Send($"GetGroupById {dataRow.Cells["Id"].EditedFormattedValue}", out var response);

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
                var existingGroup = jObject.ToObject<Group>();

                var updatingGroup = new Group
                {
                    Id = existingGroup.Id,
                    Name = dataRow.Cells["GroupName"].EditedFormattedValue.ToString(),
                    Description = dataRow.Cells["Description"].EditedFormattedValue.ToString(),
                    CreationDate = DateTime.Parse(dataRow.Cells["CreationDate"].EditedFormattedValue.ToString())
                };

                TCPClient.Send($"UpdateGroup {JObject.FromObject(updatingGroup)}", out response);

                if (response == ResponseResult.Success.ToString())
                {
                    MessageBox.Show("Group has been updated successfully!");
                }
                else if (response == ResponseResult.Failure.ToString())
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
            return !string.IsNullOrEmpty(dataRow.Cells["GroupName"].EditedFormattedValue.ToString())
                && !string.IsNullOrEmpty(dataRow.Cells["Description"].EditedFormattedValue.ToString())
                && !string.IsNullOrEmpty(dataRow.Cells["CreationDate"].EditedFormattedValue.ToString())
                && DateTime.TryParse(dataRow.Cells["CreationDate"].EditedFormattedValue.ToString(), out var result);
        }

        private void GroupForm_Load(object sender, EventArgs e)
        {
           
        }
    }
}
