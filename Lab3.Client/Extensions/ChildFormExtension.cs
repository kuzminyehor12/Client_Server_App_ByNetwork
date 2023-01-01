using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PawnShop.Forms.Extensions
{
    public static class ChildFormExtension
    {
        public static void OpenChildForm(this Form parentForm, Form childForm, Control nestedControl)
        {
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            nestedControl.Controls.Add(childForm);
            childForm.ControlToFront();
            childForm.Show();
        }

        public static void ControlToFront(this Control control)
        {
            control.BringToFront();
            
            foreach (Control c in control.Controls)
            {
                c.BringToFront();
            }
        }
    }
}
