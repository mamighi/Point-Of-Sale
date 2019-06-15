using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using EMVExampleDotNet;
using System.Drawing.Printing;
namespace Point_Of_Sale
{
    public partial class Sale : Form
    {

        EmvKernelDll EmvKernel;
        delegate void SetTextCallback(string text);
        delegate void EnableRunEmvCallback();

        DataBaseManager db = new DataBaseManager();
        List<DataManager.product> products = new List<DataManager.product>();
        ImageList images = new ImageList();
        List<List<int>> mapImages = new List<List<int>>();
        TextBox dllDetails = new TextBox();

        string strdllDetails = string.Empty;
        public Sale()
        {
            InitializeComponent();
            EmvKernel = new EmvKernelDll(dllDetails);
            
            freshForm();



        }
        public void freshForm()
        {
            listView1.Items.Clear();
            textBox1.Text = textBox2.Text = 
                textBox4.Text = textBox5.Text = textBox6.Text =
                textBox7.Text = "0";
            numericUpDown1.Value = 0;
            label3.Visible = false;

            products = db.getAllProducts();
            comboBox1.Items.Clear();
            int index = 0;
            images.ImageSize = new Size(30, 30);
            mapImages.Clear();
            images.Images.Clear();
            foreach (DataManager.product temp in products)
            {
                comboBox1.Items.Add(temp.name);

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
            listView1.SmallImageList = images;


            listView1.SmallImageList.ImageSize = new Size(40, 40);
            panel2.Visible = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            label3.Visible = false;
            if (comboBox1.SelectedItem == null) return;
            int qun = 0;
            if (!Int32.TryParse(textBox1.Text, out qun) || qun==0)
            {
                label3.Visible = true;
                return;
            }
            DataManager.product selected = new DataManager.product();
            foreach (DataManager.product temp in products)
            {
                if (temp.name.Equals(comboBox1.SelectedItem.ToString()))
                {
                    selected = temp;
                    break;
                }
            }
            float totalPrice = qun * selected.price;
            int imageIndex = 0;
            foreach(List<int> tempIndex in mapImages)
            {
                if(tempIndex[0]==selected.id)
                    imageIndex=tempIndex[1];
            }
            
            string[] items = new string[5];
            items[0] = selected.id.ToString();
            items[1] = selected.name;
            items[2]=selected.price.ToString();
            items[3] = textBox1.Text;
            items[4] = totalPrice.ToString();
            
            ListViewItem lvi = new ListViewItem(items);
            lvi.ImageIndex = imageIndex;
            
            listView1.Items.Add(lvi);
            calculatePayment();
        }
        public void calculatePayment()
        {
            float totalPrice = 0;
            foreach (ListViewItem lvi in listView1.Items)
                totalPrice += float.Parse(lvi.SubItems[4].Text.ToString());

            float grandTotal = totalPrice - (float.Parse(numericUpDown1.Value.ToString()) * totalPrice / 100);
            float balance = grandTotal - float.Parse(textBox5.Text);
            float change = -balance;

            textBox2.Text = totalPrice.ToString();
            textBox4.Text = grandTotal.ToString();
            if (balance > 0) textBox6.Text = balance.ToString();
            else textBox6.Text = "0";
            if (change > 0) textBox7.Text = change.ToString();
            else textBox7.Text = "0";
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            calculatePayment();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                listView1.Items.RemoveAt(listView1.SelectedIndices[0]);
                calculatePayment();
            }
            catch (Exception) { }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            calculatePayment();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float balance = float.Parse(textBox6.Text);
            float change = float.Parse(textBox7.Text);
            if (balance > 0)
            {
                MessageBox.Show("Please Settle Balance Amount For New Sale");
                return;
            }
            if (change > 0)
            {
                MessageBox.Show("Please Settle Change Amount For New Sale");
                return;
            }
            if (!textBox2.Text.Equals("0"))
                finilizeSale();

            freshForm();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            float balance = float.Parse(textBox6.Text);
            float change = float.Parse(textBox7.Text);
            if (balance > 0)
            {
                MessageBox.Show("Please Settle Balance Amount.");
                return;
            }
            if (change > 0)
            {
                MessageBox.Show("Please Settle Change Amount.");
                return;
            }
            if (!textBox2.Text.Equals("0"))
                finilizeSale();

            this.Visible = false;
        }
        public void finilizeSale()
        {
            int saleId = db.getSaleId();
            DataManager.sale saleData = new DataManager.sale();
            saleData.total = float.Parse(textBox2.Text);
            saleData.discount = Int32.Parse(numericUpDown1.Value.ToString());
            saleData.gtotal = float.Parse(textBox4.Text);
            saleData.id = saleId;
            saleData.date = DateTime.Now;
            if (radioButton1.Checked) saleData.payment = "cash";
            else saleData.payment = "card";

            List<DataManager.saleDetails> details = new List<DataManager.saleDetails>();
            foreach (ListViewItem lvi in listView1.Items)
            {
                int tempId = Int32.Parse(lvi.SubItems[0].Text.ToString());
                int tempQu = Int32.Parse(lvi.SubItems[3].Text.ToString());
                DataManager.saleDetails tempdet = new DataManager.saleDetails();
                tempdet.itemId = tempId;
                tempdet.qun = tempQu;
                details.Add(tempdet);
            }
            saleData.det = details;
            db.addNewSale(saleData);


        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            float change = float.Parse(textBox7.Text);
            if (change > 0)
                button7.Enabled = true;
            else
                button7.Enabled=false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            label14.Visible = false;
            float payAmount = 0;
            if(!float.TryParse(textBox3.Text,out payAmount))
            {
                label14.Visible=true;
                return;
            }
            float paid=float.Parse(textBox5.Text);
            paid += payAmount;
            textBox5.Text = paid.ToString();
            panel2.Visible = false;
            calculatePayment();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                panel2.Visible = true;
            else
            {
                Thread RunThread = new Thread(new ThreadStart(tf_OpenCard_USB_Reader));
                RunThread.Start();
            }

        }
        void tf_OpenCard_USB_Reader()
        {
            EmvKernel.InitReader();
            if (EmvKernel.Open_USB_CardReader("CASTLES EZ100PU 0") == true)
            {
                //this.Invoke((MethodInvoker)(() => this.BEMVRun.Enabled = true));
                this.Invoke((MethodInvoker)(() => panel3.Visible = true));
                this.Invoke((MethodInvoker)(() => label16.Text = "Please Insert The card"));
            }
            else
                MessageBox.Show("Faild to open card reader.");
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            float balance = float.Parse(textBox6.Text);
            if (balance == 0) button4.Enabled = false;
            else button4.Enabled = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox7.Text = "0";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Thread RunThread = new Thread(new ThreadStart(RunTrans));
            RunThread.Start();
        }
        void RunTrans()
        {
            #if USE_CALLBACK_FOR_CARD_READER
                        EmvKernel.EmvRun(emv_param_tt_14);
                        SetText("END\r\n");
                        BEMVRun.Enabled = true;
            #else
            this.Invoke((MethodInvoker)(() => label16.ForeColor=System.Drawing.Color.Red));
                if (EmvKernel.Wait_USB_CardIn() == true)
                {
                    strdllDetails=EmvKernel.EmvRun("test_ttype25.cfg");
                    strdllDetails = dllDetails.Text;
                    this.Invoke((MethodInvoker)(() => label16.Text="The Payment Approved.\nPlease Take The Card."));
                    EmvKernel.WaitCardOut();
                    EnableRunEmv();
                    this.Invoke((MethodInvoker)(() => label16.Text = "The Payment Approved.\nPlease Take The Card."));
                    this.Invoke((MethodInvoker)(() => textBox5.Text=textBox6.Text));
                    this.Invoke((MethodInvoker)(() => panel3.Visible=false));
                    this.Invoke((MethodInvoker)(() => calculatePayment()));
                }
                else
                {
                     //EnableRunEmv();
                }
            #endif
        }
        void SetText(string text)
        {

            if (this.dllDetails.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.dllDetails.Text += text;
            }
        }
        void EnableRunEmv()
        {

                EnableRunEmvCallback d = new EnableRunEmvCallback(EnableRunEmv);
                this.Invoke(d, null);
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                // If you want a preview use the PrintPreviewDialog
                PrintPreviewDialog preview = new PrintPreviewDialog();

                PrintDocument document = new PrintDocument();
                document.PrintPage += new PrintPageEventHandler(document_PrintPage);

                preview.Document = document;
                // Then show the dialog window.
                preview.Show();

                // Otherwise, just call the document.Print();

            }
            catch
            {
                throw;

            }
        }
        protected void document_PrintPage(object sender, PrintPageEventArgs ev)
        {
            Font printFont = new Font("Arial", 14);
            string printDetails = string.Empty;
            printDetails +="NAME"+  "         " + "UNIT PRICE" + "         " + "QUN" +  "         " +
                   "TOTAL PRICE" + " \n";
            foreach (ListViewItem lvi in listView1.Items)
            {
                printDetails += lvi.SubItems[1].Text;
                for (int i = 0; i < 13 - lvi.SubItems[1].Text.Length; i++)
                    printDetails += "  ";
                printDetails += lvi.SubItems[2].Text;
                for (int i = 0; i < 22 - lvi.SubItems[2].Text.Length; i++)
                    printDetails += " ";
                printDetails += lvi.SubItems[3].Text;
                for (int i = 0; i < 14 - lvi.SubItems[3].Text.Length; i++)
                    printDetails += " ";
                printDetails += lvi.SubItems[4].Text+"\n";

  
            }
            printDetails+="\n";
            printDetails += "\n";
            printDetails += "\n";
            printDetails += "Total= " + textBox2.Text + "\n";
            printDetails += "Discount= " + numericUpDown1.Value.ToString() + "\n";
            printDetails+="Grand Total= "+textBox4.Text+"\n";
            ev.Graphics.DrawString(printDetails, printFont, Brushes.Black, ev.MarginBounds.Left, ev.MarginBounds.Top, new StringFormat());
        }

    }
}
