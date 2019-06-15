using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Point_Of_Sale
{
    public partial class EditProduct : Form
    {
        DataBaseManager db = new DataBaseManager();

        Byte[] imageByte = null;
        public EditProduct()
        {
            InitializeComponent();
        }
        public void freshForm()
        {
            loadIds();
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            textBox3.Text = string.Empty;
            label8.Visible = false;
            pictureBox1.Image = Point_Of_Sale.Properties.Resources.no_image_icon_6;
        }
        public void loadIds()
        {
            List<int> IDs = db.allProductsId();
            comboBox1.Items.Clear();
            foreach (int tempId in IDs)
                comboBox1.Items.Add(tempId);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = Int32.Parse(comboBox1.SelectedItem.ToString());
            DataManager.product details = db.getProductById(id);
            textBox1.Text = details.name;
            textBox2.Text = details.des;
            textBox3.Text = details.price.ToString();
            imageByte = details.image;
            MemoryStream imageBlob = new MemoryStream(imageByte);
            pictureBox1.Image = Image.FromStream(imageBlob);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

        }
        public bool validateInfo()
        {
            if (textBox1.Text.TrimEnd().Length < 1 || textBox2.Text.TrimEnd().Length < 1 ||
                textBox3.Text.TrimEnd().Length < 1)
            {
                label8.Text = "Please Fill All Field.";
                label8.Visible = true;
                return false;
            }
            float price = 0;
            if (!float.TryParse(textBox3.Text, out price))
            {
                label8.Text = "Inserted Price Is Not Valid.";
                label8.Visible = true;
                return false;
            }
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            if ( comboBox1.SelectedItem==null || comboBox1.SelectedItem.ToString().TrimEnd().Length < 1)
            {
                label8.Text = "Please Select Product Id.";
                label8.Visible = true;
                return;
            }
            DialogResult res= MessageBox.Show("Are You Sure?", "Remove Product", MessageBoxButtons.YesNo);
            if (res == DialogResult.No)
                return;
            db.deleteProduct(Int32.Parse(comboBox1.SelectedItem.ToString()));
            MessageBox.Show("Product Has Been Removed Successfully.");
            freshForm();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label8.Visible = false;
            if (comboBox1.SelectedItem == null || comboBox1.SelectedItem.ToString().TrimEnd().Length < 1)
            {
                label8.Text = "Please Select Product Id.";
                label8.Visible = true;
                return;
            }
            if (!validateInfo())
                return;
            db.editProduct(Int32.Parse(comboBox1.SelectedItem.ToString()), textBox1.Text, textBox2.Text,
                float.Parse(textBox3.Text), imageByte);
            MessageBox.Show("New Product Has Been Updated Successfully.");
            freshForm();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            FileStream image = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            imageByte = new Byte[image.Length];
            image.Read(imageByte, 0, imageByte.Length);
            image.Close();
            MemoryStream imageBlob = new MemoryStream(imageByte);
            pictureBox1.Image = Image.FromStream(imageBlob);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images|*.jpg;*.png;";
            openFileDialog1.ShowDialog();
        }
    }
}
