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
    public partial class AddProduct : Form
    {
        DataBaseManager db = new DataBaseManager();
        int id=0;
        private Byte[] imageByte=null;
       
        public AddProduct()
        {
            InitializeComponent();
            freshForm();
        }
        public void freshForm()
        {
            id = db.getProdId();
            label2.Text = id.ToString();
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;
            textBox3.Text = string.Empty;
            label8.Visible = false;

            pictureBox1.Image = Point_Of_Sale.Properties.Resources.no_image_icon_6;

            ImageConverter converter = new ImageConverter();
            imageByte= (byte[])converter.ConvertTo(pictureBox1.Image, typeof(byte[]));
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Images|*.jpg;*.png;";
            openFileDialog1.ShowDialog();
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (!validateInfo())
                return;
            db.addNewProduct(id, textBox1.Text, textBox2.Text,
                float.Parse(textBox3.Text), imageByte);
            MessageBox.Show("New Product Has Been Added Successfully.");
            freshForm();
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
            float price=0;
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
    }
}
