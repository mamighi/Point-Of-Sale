using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Point_Of_Sale
{
    public partial class EditAccount : Form
    {
        DataBaseManager db = new DataBaseManager();
        string username;
        public EditAccount(string un)
        {
            InitializeComponent();
            username = un;
            loadData();
        }
        public void loadData()
        {
            DataManager.user details = db.getUserDetails(username);
            textBox1.Text = details.userName;
            textBox2.Text = textBox3.Text = details.password;
            textBox4.Text = details.firstName;
            textBox5.Text = details.lastName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!infoValidation())
                return;
            DataManager.user userDetail = new DataManager.user();
            userDetail.userName = textBox1.Text;
            userDetail.password = textBox2.Text;
            userDetail.firstName = textBox4.Text;
            userDetail.lastName = textBox5.Text;
            db.updateAccount(userDetail);

            MessageBox.Show("Your Account Details Have Been Updated Successfully.");

        }
        public bool infoValidation()
        {
            if (textBox1.Text.TrimEnd().Length < 1 || textBox2.Text.TrimEnd().Length < 1 ||
                textBox3.Text.TrimEnd().Length < 1 || textBox4.Text.TrimEnd().Length < 1 ||
                textBox5.Text.TrimEnd().Length < 1)
            {
                label8.Text = "Please Fill All The Fields.";
                label8.Visible = true;
                return false;
            }
            if (!textBox2.Text.Equals(textBox3.Text))
            {
                label8.Text = "Passwords Do Not Match.";
                label8.Visible = true;
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
