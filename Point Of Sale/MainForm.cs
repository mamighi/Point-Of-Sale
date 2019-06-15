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
    public partial class MainForm : Form
    {
        //Objects
        DataBaseManager db = new DataBaseManager();
        AddAccount addAccount = new AddAccount();
        AddProduct addProduct = new AddProduct();
        EditProduct editProduct = new EditProduct();
        Sale sale = new Sale();
        ViewSale viewSale = new ViewSale();
        EditAccount editAccount;
        public string userName = string.Empty;
        public string password = string.Empty;
        public MainForm()
        {
            InitializeComponent();
            showLoginPanel();
            db.creatTable();


        }
        public void showLoginPanel()
        {
            label2.Visible = false;
            MainMenu.Visible = false;
            textBox1.Text = textBox2.Text = string.Empty;
            LoginPanel.Visible = true;
            LoginPanel.BringToFront();
            LoginPanel.Dock = DockStyle.Fill;
        }
        public void showMainMenu()
        {
            LoginPanel.Visible = false;
            MainMenu.Visible = true;
            MainMenu.BringToFront();
            MainMenu.Dock = DockStyle.Fill;
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            sale.FormBorderStyle = FormBorderStyle.None;
            sale.TopLevel = false;
            sale.freshForm();
            this.Controls.Add(sale);
            sale.Visible = true;
            sale.Dock = DockStyle.Fill;
            sale.BringToFront();
            sale.Show();
        }

        private void label9_Click(object sender, EventArgs e)
        {
            sale.FormBorderStyle = FormBorderStyle.None;
            sale.TopLevel = false;
            sale.freshForm();
            this.Controls.Add(sale);
            sale.Visible = true;
            sale.Dock = DockStyle.Fill;
            sale.BringToFront();
            sale.Show();
        }

        private void MainMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

            addProduct.FormBorderStyle = FormBorderStyle.None;
            addProduct.TopLevel = false;
            addProduct.freshForm();
            this.Controls.Add(addProduct);
            addProduct.Visible = true;
            addProduct.Dock = DockStyle.Fill;
            addProduct.BringToFront();
            addProduct.Show();
        }

        private void label8_Click(object sender, EventArgs e)
        {

            editProduct.FormBorderStyle = FormBorderStyle.None;
            editProduct.TopLevel = false;
            editProduct.freshForm();
            this.Controls.Add(editProduct);
            editProduct.Visible = true;
            editProduct.Dock = DockStyle.Fill;
            editProduct.BringToFront();
            editProduct.Show();
        }

        private void label10_Click(object sender, EventArgs e)
        {

            viewSale.FormBorderStyle = FormBorderStyle.None;
            viewSale.TopLevel = false;
            viewSale.freshForm();
            this.Controls.Add(viewSale);
            viewSale.Visible = true;
            viewSale.Dock = DockStyle.Fill;
            viewSale.BringToFront();
            viewSale.Show();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            editAccount = new EditAccount(userName);
            editAccount.FormBorderStyle = FormBorderStyle.None;
            editAccount.TopLevel = false;
            this.Controls.Add(editAccount);
            editAccount.Visible = true;
            editAccount.Dock = DockStyle.Fill;
            editAccount.BringToFront();
            editAccount.Show();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            addAccount.FormBorderStyle = FormBorderStyle.None;
            addAccount.TopLevel = false;
            addAccount.freshForm();
            this.Controls.Add(addAccount);
            addAccount.Visible = true;
            addAccount.Dock = DockStyle.Fill;
            addAccount.BringToFront();
            addAccount.Show();
        }

        private void label12_Click(object sender, EventArgs e)
        {
            showLoginPanel();
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            editAccount = new EditAccount(userName);
            editAccount.FormBorderStyle = FormBorderStyle.None;
            editAccount.TopLevel = false;
            this.Controls.Add(editAccount);
            editAccount.Visible = true;
            editAccount.Dock = DockStyle.Fill;
            editAccount.BringToFront();
            editAccount.Show();
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            showLoginPanel();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            viewSale.FormBorderStyle = FormBorderStyle.None;
            viewSale.TopLevel = false;
            viewSale.freshForm();
            this.Controls.Add(viewSale);
            viewSale.Visible = true;
            viewSale.Dock = DockStyle.Fill;
            viewSale.BringToFront();
            viewSale.Show();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            editProduct.FormBorderStyle = FormBorderStyle.None;
            editProduct.TopLevel = false;
            editProduct.freshForm();
            this.Controls.Add(editProduct);
            editProduct.Visible = true;
            editProduct.Dock = DockStyle.Fill;
            editProduct.BringToFront();
            editProduct.Show();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            addProduct.FormBorderStyle = FormBorderStyle.None;
            addProduct.TopLevel = false;
            this.Controls.Add(addProduct);
            addProduct.freshForm();
            addProduct.Visible = true;
            addProduct.Dock = DockStyle.Fill;
            addProduct.BringToFront();
            addProduct.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            addAccount.FormBorderStyle = FormBorderStyle.None;
            addAccount.TopLevel = false;
            addAccount.freshForm();
            this.Controls.Add(addAccount);
            addAccount.Visible = true;
            addAccount.Dock = DockStyle.Fill;
            addAccount.BringToFront();
            addAccount.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Login
            label2.Visible = false;
            userName = textBox1.Text;
            password = textBox2.Text;

            if(db.login(userName,password).ToLower().Equals("admin"))
            {
                pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible =
                    label6.Visible = label7.Visible = label8.Visible = true;
                DataManager.user det= db.getUserDetails(userName);
                label5.Text = "WELCOME " + det.firstName.ToUpper();

                showMainMenu();
            }
            else if (db.login(userName, password).ToLower().Equals("clerk"))
            {
                pictureBox2.Visible = pictureBox3.Visible = pictureBox4.Visible =
                    label6.Visible = label7.Visible = label8.Visible = false;
                DataManager.user det = db.getUserDetails(userName);
                label5.Text = "WELCOME " + det.firstName.ToUpper();
                showMainMenu();
            }
            else
                label2.Visible=true;
            label5.Left=(MainMenu.Width-label5.Width)/2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
