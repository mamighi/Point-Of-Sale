//#define USE_CALLBACK_FOR_CARD_READER

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EMVExampleDotNet
{
    public partial class EMVExample : Form
    {
        delegate void SetTextCallback(string text);
        delegate void EnableRunEmvCallback();
        EmvKernelDll EmvKernel;
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
        string com_port_name;
        string reader_name;
        string usb_reader_name;
#endif
        public EMVExample()
        {
            InitializeComponent();
            EmvKernel = new EmvKernelDll(textBox1);
            textBox1.Text += EmvKernel.fEmvKernelInit() + "\r\n";
#if USE_CALLBACK_FOR_CARD_READER
            this.Com_Port_comboBox.Enabled = false;
            this.button_CloseReader.Enabled = false;
            this.button_Open_Reader.Enabled = false;
            this.Open_USB_button.Enabled = false;
            this.USB_Readers_comboBox.Enabled = false;
            this.Readers_comboBox.Enabled = false;
            
#else
            this.BEMVRun.Enabled = false;
            Add_USB_To_List();
#endif
        }
        ~EMVExample()
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            EmvKernel.CloseCardReader();
            EmvKernel.UninitReader();
#endif
        }

        void SetText(string text)
        {
            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.textBox1.Text += text;
            }
        }
        void EnableRunEmv()
        {
            if (this.BEMVRun.InvokeRequired)
            {
                EnableRunEmvCallback d = new EnableRunEmvCallback(EnableRunEmv);
                this.Invoke(d, null);
            }
            else
            {
                this.BEMVRun.Enabled = true;
            }
        }
        public const string emv_param_tt_14 = "test_ttype14.cfg";
        public const string emv_param_tt_25 = "test_ttype25.cfg";
        void RunTrans()
        {
#if USE_CALLBACK_FOR_CARD_READER
            EmvKernel.EmvRun(emv_param_tt_14);
            SetText("END\r\n");
            BEMVRun.Enabled = true;
#else
            if (reader_name != null && com_port_name != null)
            {
                if (EmvKernel.WaitCardIn() == true)
                {
                    EmvKernel.EmvRun(emv_param_tt_25);
                    EmvKernel.WaitCardOut();
                    SetText("END\r\n");
                    EnableRunEmv();
                }
                else
                {
                    EnableRunEmv();
                }
            }
            else
            {
                if (usb_reader_name != null)
                {
                    if (EmvKernel.Wait_USB_CardIn() == true)
                    {
                        EmvKernel.EmvRun(emv_param_tt_25);
                        EmvKernel.WaitCardOut();
                        SetText("END\r\n");
                        EnableRunEmv();
                    }
                    else
                    {
                        EnableRunEmv();
                    }
                }
            }
#endif
        } 
        private void BEMVRun_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            BEMVRun.Enabled = false;
            Thread RunThread = new Thread(new ThreadStart(RunTrans));
            RunThread.Start();
        }
        private void BClear_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
        private void EMVExample_FormClosing(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            EmvKernel.CloseCardReader();
            EmvKernel.EmvUnInit();
#endif
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.Select(textBox1.Text.Length, 0);
            textBox1.ScrollToCaret();
        }
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
        void tf_OpenCardReader()
        {
            EmvKernel.InitReader();
            if (EmvKernel.Open_COM_CardReader(reader_name, com_port_name) == true)
            {
                this.BEMVRun.Enabled = true;
            }
        }
        void tf_CloseCardReader()
        {
            EmvKernel.CloseCardReader();
            EmvKernel.UninitReader();
        }
        public void Add_USB_To_List()
        {
            USB_Readers_comboBox.BeginUpdate();
            for (int i = 0; i < 10 ;i++)
            {
                string r_name = EmvKernel.Get_USB_ReaderName(i);
                if (r_name != null)
                {
                    USB_Readers_comboBox.Items.Add("" + r_name);
                }
                else
                {
                    break;
                }
            }
            USB_Readers_comboBox.EndUpdate();
        }
        void tf_OpenCard_USB_Reader()
        {
            EmvKernel.InitReader();
            if (EmvKernel.Open_USB_CardReader(usb_reader_name) == true)
            {
                this.BEMVRun.Enabled = true;
            }
        }
#endif
        private void button_Open_Reader_Click(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            Thread RunThread = new Thread(new ThreadStart(tf_OpenCardReader));
            RunThread.Start();
#endif
        }
        private void button_CloseReader_Click(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            Thread RunThread = new Thread(new ThreadStart(tf_CloseCardReader));
            RunThread.Start();
            this.BEMVRun.Enabled = false;
#endif
        }
        private void EMVExample_Load(object sender, EventArgs e)
        {

        }
        private void Com_Port_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            Object Com_Port_selectedItem = Com_Port_comboBox.SelectedItem;
            com_port_name = Com_Port_selectedItem.ToString();
#endif
        }
        private void Readers_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            Object Readers_selectedItem = Readers_comboBox.SelectedItem;
            reader_name = Readers_selectedItem.ToString();
#endif
        }
        private void USB_Readers_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            Object Readers_selectedItem = USB_Readers_comboBox.SelectedItem;
            usb_reader_name = Readers_selectedItem.ToString();
#endif
        }
        private void Open_USB_button_Click(object sender, EventArgs e)
        {
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
            //usb_reader_name = EmvKernel.Select_USB_Reader();
            if(usb_reader_name != null)
            {
                reader_name = null;
                com_port_name = null;

                Thread RunThread = new Thread(new ThreadStart(tf_OpenCard_USB_Reader));
                RunThread.Start();
            }
#endif
        }
    }
}
