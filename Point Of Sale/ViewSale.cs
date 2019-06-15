using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Point_Of_Sale
{
    public partial class ViewSale : Form
    {
        DataBaseManager db = new DataBaseManager();
        List<DataManager.product> products = new List<DataManager.product>();
        ImageList images = new ImageList();
        List<List<int>> mapImages = new List<List<int>>();
        List<DataManager.sale> sales = new List<DataManager.sale>();
        public ViewSale()
        {
            InitializeComponent();
        }
        public void freshForm()
        {
            //LOAD PRODCUTS AND IMAGES
            products = db.getAllProducts();
            int index = 0;
            images.ImageSize = new Size(30, 30);
            mapImages.Clear();
            images.Images.Clear();
            foreach (DataManager.product temp in products)
            {
                //good luck
                MemoryStream imageBlob = new MemoryStream(temp.image);
                System.Drawing.Image img = System.Drawing.Image.FromStream(imageBlob);
                images.Images.Add(img);
                List<int> insideMap = new List<int>();
                insideMap.Add(temp.id);
                insideMap.Add(index);
                mapImages.Add(insideMap);
                index++;
            }
            listView2.SmallImageList = images;
            listView2.SmallImageList.ImageSize = new Size(40, 40);


            //LOAD SALES
            sales = db.getAllSales();
            listView1.Items.Clear();
            foreach (DataManager.sale tempSale in sales)
            {
                string[] items= new string[6];
                items[0] = tempSale.id.ToString();
                items[1] = tempSale.date.ToString();
                items[2] = tempSale.total.ToString();
                items[3] = tempSale.discount.ToString();
                items[4] = tempSale.gtotal.ToString();
                items[5] = tempSale.payment.ToUpper();
                ListViewItem lvi = new ListViewItem(items);
                listView1.Items.Add(lvi);
                
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            textBox2.Text = textBox1.Text = textBox4.Text = "0";
            ListViewItem lvi;
            try
            {
                lvi = listView1.SelectedItems[0];
            }
            catch (Exception)
            {
                return;
            }
            int saleId = Int32.Parse(lvi.SubItems[0].Text);
            textBox1.Text = lvi.SubItems[3].Text;
            textBox2.Text = lvi.SubItems[2].Text;
            textBox4.Text = lvi.SubItems[4].Text;
            List<DataManager.saleDetails> saleDetail = new List<DataManager.saleDetails>();
            foreach (DataManager.sale tempSale in sales)
            {
                if (tempSale.id == saleId)
                {
                    saleDetail = tempSale.det;
                    break;
                }
            }
            foreach (DataManager.saleDetails tempSaleDetail in saleDetail)
            {
                DataManager.product selected = new DataManager.product();
                foreach (DataManager.product temp in products)
                {
                    
                    if (temp.id==tempSaleDetail.itemId)
                    {
                        selected = temp;
                        break;
                    }
                }
                float totalPrice = tempSaleDetail.qun * selected.price;
                int imageIndex = 0;
                foreach (List<int> tempIndex in mapImages)
                {
                    if (tempIndex[0] == selected.id)
                        imageIndex = tempIndex[1];
                }
                string[] items = new string[5];
                items[0] = selected.id.ToString();
                items[1] = selected.name;
                items[2] = selected.price.ToString();
                items[3] = tempSaleDetail.qun.ToString();
                items[4] = totalPrice.ToString();

                ListViewItem lvi2 = new ListViewItem(items);
                lvi2.ImageIndex = imageIndex;

                listView2.Items.Add(lvi2);


            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
