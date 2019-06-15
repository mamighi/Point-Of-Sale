using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Point_Of_Sale
{
    public partial class AddAccount : Form
    {
        DataBaseManager db = new DataBaseManager();

        public AddAccount()
        {
            InitializeComponent();
        }
        public void freshForm()
        {
            label8.Visible = false;
            textBox1.Text = textBox2.Text = textBox3.Text =
                textBox4.Text = textBox5.Text = string.Empty;
            comboBox1.SelectedText = "Admin";

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            if (!infoValidation())
                return;
            db.addNewAccount(textBox1.Text, textBox2.Text, textBox4.Text, 
                textBox5.Text, comboBox1.SelectedItem.ToString());
            MessageBox.Show("The New Account Has Been Added Successfully.");
            freshForm();
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
            if (db.isUserNameExist(textBox1.Text))
            {
                label8.Text = "The Username Is Already Exist.";
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
