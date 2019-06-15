//#define USE_CALLBACK_FOR_CARD_READER
//#define USE_DEMO_SMART_CARD_SIMULATOR_DLL
#define USE_UNITED_HOST_ANSWER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Data;
using System.Data.OleDb;

namespace EMVExampleDotNet
{
    public class emv_APP_INFO
    {
        public byte btIssuerCodeTable; //Shall be 1-10, if zero => common

        public byte btAppLabelLength;
        public byte[] btIsoAppLabel = new byte[16];

        public byte btAppAidLength;
        public byte[] btAppAid = new byte[16];

        public byte btTerParamEntryID;
    }
    public class emv_TRANS_DATA
    {
        public byte[] _9C = new byte[1]; // ISO 8583:1993(E), A.9 Processing codes:
        public byte[] _5F36 = new byte[1]; // Transaction Currency Exponent
        // Indicates the implied position of the decimal
        // point from the right of the transaction amount
        // represented according to ISO 4217
        public byte[] _5F2A = new byte[2]; // Transaction Currency Code Example USD => _5F2A[2]={0x08, 0x40};
        public byte[] _9F02 = new byte[6]; // Amount, Authorized (Numeric) Example:12.99(USD) => _9F12[6]={0x00, 0x00, 0x00, 0x00, 0x12, 0x99};
        public byte[] _9F03 = new byte[6]; // Amount, Other (Binary)
        public byte status_pan; // 0 => "OK", 1 => "PAN in the exception fie"
    }
    
    public unsafe delegate byte     FUNC_PTR_emv_fSelLang       (IntPtr lpHandle, byte* lpbtLang, byte btMaxSize);
    public unsafe delegate byte     FUNC_PTR_emv_fSelApp        (IntPtr lpHandle, byte btRetry, byte* lpbtLang, byte btLangSize, IntPtr lpAppList, byte btAppListSize);
    public unsafe delegate void     FUNC_PTR_emv_fAppSelected   (IntPtr lpHandle, byte[] lpbtLang, byte btLangSize, IntPtr lpSelApp);
    public unsafe delegate byte     FUNC_PTR_emv_fAttGetTrData  (IntPtr lpHandle, byte* lpbtLang, byte btLangSize, IntPtr lpSelApp, IntPtr lpTrData);
    public unsafe delegate byte     FUNC_PTR_emv_fGetTrData     (IntPtr lpHandle, IntPtr lpTrData);
    public unsafe delegate byte     FUNC_PTR_emv_fGetCryptPin   (IntPtr lpHandle, byte* lpbtPinblock, byte* lpbtSize, byte btMaxSize);
    public unsafe delegate byte     FUNC_PTR_emv_fGetPin        (IntPtr lpHandle, byte btTryCounter, byte btRetry, byte* lpbtPinblock, byte* lpbtSize, byte btMaxSize);
    public unsafe delegate byte     FUNC_PTR_emv_fHostSwop      (IntPtr lpHandle, byte* btAC1, uint dwAC1Size, byte* lpbtHostCr, uint* lpdwHostCrSize, uint dwMaxSize, byte* lpbtTag71, uint* lpdwTag71Size, uint dwTag71Size, byte* lpbtTag72, uint* lpdwTag72Size, uint dwTag72Size);
    public unsafe delegate byte     FUNC_PTR_emv_fIssuerReferral(IntPtr lpHandle, byte* lpbtPan, byte btSize);
    public unsafe delegate byte     FUNC_PTR_emv_fSelectAccount (IntPtr lpHandle, byte* lpbtLang, byte btLangSize, IntPtr lpSelApp);
#if USE_CALLBACK_FOR_CARD_READER
    public unsafe delegate ushort   FUNC_PTR_emv_ApduIn         (byte* cmd, byte* data, ushort* data_len, byte* answer, ushort answer_max_len);
    public unsafe delegate ushort   FUNC_PTR_emv_ApduOut        (byte* cmd, ushort* stat, byte* answer, ushort answer_max_len);
    public unsafe delegate byte     FUNC_PTR_emv_Atr            (byte* dest_atr, uint* max_len, byte* protocol);
    public unsafe delegate byte     FUNC_PTR_emv_CheckConnect   ();
#endif
    class EmvKernelDll
    {
        public static bool isBOkPressed = false;
        static TextBox OutTextBox;
        public EmvKernelDll(TextBox textBox)
        {
            OutTextBox = textBox;
        }
        public const string emv_ker = "emv_ker.dll";

        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fSelLang          (IntPtr lpHandle, FUNC_PTR_emv_fSelLang fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fSelApp           (IntPtr lpHandle, FUNC_PTR_emv_fSelApp fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fAppSelected      (IntPtr lpHandle, FUNC_PTR_emv_fAppSelected fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fAttGetTrData     (IntPtr lpHandle, FUNC_PTR_emv_fAttGetTrData fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fGetTrData        (IntPtr lpHandle, FUNC_PTR_emv_fGetTrData fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fGetCryptPin      (IntPtr lpHandle, FUNC_PTR_emv_fGetCryptPin fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fGetPin           (IntPtr lpHandle, FUNC_PTR_emv_fGetPin fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fHostSwop         (IntPtr lpHandle, FUNC_PTR_emv_fHostSwop fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fIssuerReferral   (IntPtr lpHandle, FUNC_PTR_emv_fIssuerReferral fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_fSelectAccount    (IntPtr lpHandle, FUNC_PTR_emv_fSelectAccount fPointer);

#if USE_CALLBACK_FOR_CARD_READER
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_ApduIn(IntPtr lpHandle, FUNC_PTR_emv_ApduIn fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_ApduOut(IntPtr lpHandle, FUNC_PTR_emv_ApduOut fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_Atr(IntPtr lpHandle, FUNC_PTR_emv_Atr fPointer);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernel_registry_CheckConnect(IntPtr lpHandle, FUNC_PTR_emv_CheckConnect fPointer);
#endif
        [DllImport(emv_ker, SetLastError = true, EntryPoint = "emv_kernelInit")]
        static extern unsafe IntPtr emv_kernelInit(string fname);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe void emv_kernelUnInit(IntPtr lpHandle);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe UInt32 emv_kernel_run(IntPtr lpHandle, IntPtr lpCallbackHandle, byte btTrStatus, byte* lpbtCr, uint* lpdwCrSize, uint dwMaxSize);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe UInt32 emv_kernelGetCardData(ushort dwTag, byte* lpbtData, ushort* lpdwSize, uint dwMaxSize);

        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe UInt32 emv_kernelSetCardData(ushort dwTag, byte[] lpbtData, UInt32 dwSize);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernelSetBerTlvData(IntPtr lpHandle, ref byte lpbtBuffer, UInt32 dwBufferSize);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernelGetCamData(IntPtr lpHandle, ref byte lpbtByte1, ref byte lpbtByte2);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernelGetErrData(IntPtr lpHandle, ref byte lpbtByte1, ref byte lpbtByte2);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernelClearData(IntPtr lpHandle);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_kernelGetDataByTag(IntPtr lpHandle, ref byte lpbtTag, UInt32 dwTagSize, ref byte lpbtData, ushort* lpdwSize, UInt32 dwMaxSize);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_GetTLVBufByDOL(IntPtr lpHandle, byte[] dol, UInt32 dol_size, byte[] outt, out UInt32 out_len, UInt32 out_max);
        [DllImport(emv_ker, SetLastError = true)]
        static extern unsafe byte emv_GetTLVBufByCDOL(IntPtr lpHandle, byte flag_cdol, ref byte lpbtBuffer, UInt32 dwBufferSize, ref byte outt, UInt32* out_len, UInt32 out_max);

#if USE_CALLBACK_FOR_CARD_READER
        //
#else
        public const string smartCard = "SmartCard.dll";

        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte OpenReader(string rname, string comport);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte Initialize();
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte UnInit();

        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte TakeCard(UInt32 time_out);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte GetMagData(byte* data, uint * data_len);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte EjectCard();

        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte CheckConnect();
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte WaitConnect(UInt32 time_out);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte Atr(byte* atr, UInt32* max_len, byte* protocol);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte SendApduIn(byte* cmd, byte* data, UInt16* data_len, byte* answer, UInt16 answer_max_len);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte SendApduOut(byte* cmd, UInt16* stat, byte* answer, UInt16 answer_max_len);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte CloseReader();
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte WaitDisConnect();    
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte SelectReader(byte[] rname, int size);
        [DllImport(smartCard, SetLastError = true)]
        static extern unsafe byte GetReaderName(byte[] rname, int size, int index);
#endif

        static emv_APP_INFO InSelApp = new emv_APP_INFO();
        static emv_TRANS_DATA InTrData = new emv_TRANS_DATA();
        static byte[] InbtLang = new byte[3];
        static Thread hInTrDataThread = null;

        public static unsafe byte emv_fSelLang(IntPtr lpHandle, byte* lpbtLang, byte btMaxSize)
        {
            SetText("emv_fSelLang\r\n");
            lpbtLang[0] = 0x45;//'E'
            lpbtLang[1] = 0x4E;//'N'
            return 2;
        }
        public static unsafe byte emv_fSelectAccount(IntPtr lpHandle, byte* lpbtLang, byte btLangSize, IntPtr lpSelApp)
        {
            SetText("emv_fSelAccount\r\n");
            return 0;
        }

        public static unsafe byte emv_fSelApp(IntPtr lpHandle, byte btRetry, byte* lpbtLang, byte btLangSize, IntPtr lpAppList, byte btAppListSize)
        {
            SetText("emv_fSelApp\r\n");
            return 0;
        }
        public static unsafe void emv_fAppSelected(IntPtr lpHandle, byte[] lpbtLang, byte btLangSize, IntPtr lpSelApp)
        {
            SetText("emv_fAppSelected starts in new thread" + "\r\n");
            if (btLangSize <= InbtLang.Length)
            {
                InbtLang = lpbtLang;
            }//Получаем язык

            //InSelApp = lpSelApp;
            if (hInTrDataThread != null)
                if (!hInTrDataThread.IsAlive)
                    hInTrDataThread = null;

            if (hInTrDataThread == null)
            {
                hInTrDataThread = new Thread(new ThreadStart(InGetTrData));
            }
            hInTrDataThread.Start();
        }

        private static void InGetTrData()
        {
            //ushort tag_len = 0;
            byte[] pan = new byte[10];
            uint rc;
            rc = fGetTag(0x5A, pan, (uint)pan.Length);//Получение ПАНа.
            byte[] span = new byte[20];
            OtherFunc.BinToHex(pan, span, (int)rc);
            SetText("PAN = " + Encoding.ASCII.GetString(span));
            SetText("\r\n");
            if (rc != 0)
            {
                //Поиск ПАНа в стоп листе.
            }

            InTrData._9C[0] = 0x00;

            InTrData._9F02[0] = 0x00;
            InTrData._9F02[1] = 0x00;
            InTrData._9F02[2] = 0x00;
            InTrData._9F02[3] = 0x01;
            InTrData._9F02[4] = 0x00;
            InTrData._9F02[5] = 0x00;

            InTrData._5F2A[0] = 0x06;
            InTrData._5F2A[1] = 0x43;
            //MessageBox.Show("Inputing of amount, currency and type of transaction", "Inputing transacThread");
            SetText("emv_fAppSelected thread end" + "\r\n");
        }

        public static unsafe byte emv_fAttGetTrData(IntPtr lpHandle, byte* lpbtLang, byte btLangSize, IntPtr lpSelApp, IntPtr lpTrData)
        {
            SetText("emv_fAttGetTrData\r\n");
            return 0;
        }
        public static unsafe byte emv_fGetTrData(IntPtr lpHandle, IntPtr lpTrData)
        {
            SetText("emv_fGetTrData\r\n");
            hInTrDataThread.Join();
            emv_kernelSetCardData(0x5F36, InTrData._5F36, (uint)InTrData._5F36.Length);
            emv_kernelSetCardData(0x5F2A, InTrData._5F2A, (uint)InTrData._5F2A.Length);
            emv_kernelSetCardData(0x9F02, InTrData._9F02, (uint)InTrData._9F02.Length);
            emv_kernelSetCardData(0x9F03, InTrData._9F03, (uint)InTrData._9F03.Length);
            emv_kernelSetCardData(0x9C, InTrData._9C, (uint)InTrData._9C.Length);
            return 0;
        }
        public static unsafe byte emv_fGetCryptPin(IntPtr lpHandle, byte* lpbtPinblock, byte* lpbtSize, byte btMaxSize)
        {
            SetText("emv_fGetCryptPin\r\n");
            byte[] PinBlock = new byte[40];
            // ...
            lpbtPinblock[0] = PinBlock[0];
            lpbtPinblock[1] = PinBlock[1];
            lpbtPinblock[2] = PinBlock[2];
            lpbtPinblock[3] = PinBlock[3];
            lpbtPinblock[4] = PinBlock[4];
            lpbtPinblock[5] = PinBlock[5];
            lpbtPinblock[6] = PinBlock[6];
            lpbtPinblock[7] = PinBlock[7];
            *lpbtSize = 8;
            return 0;
        }
        public static unsafe byte emv_fGetPin(IntPtr lpHandle, byte btTryCounter, byte btRetry, byte* lpbtPinblock, byte* lpbtSize, byte btMaxSize)
        {
            SetText("emv_fGetPin\r\n");
            
            byte[] pin_data = new byte[13];
            int pin_data_len = 0;

            pin_data[0] = 0x31; pin_data_len++;
            pin_data[1] = 0x32; pin_data_len++;
            pin_data[2] = 0x33; pin_data_len++;
            pin_data[3] = 0x34; pin_data_len++;

            *lpbtSize = (byte)pin_data_len;
            Marshal.Copy(pin_data, 0, (IntPtr)lpbtPinblock, pin_data_len);

            string message;
            message = "You have " + btTryCounter + " attempts for input of PIN";
            string caption;
            if (btRetry == 1)
            {
                caption = "Try again. Do you want to continue?";
            }
            else
            {
                caption = "Do you want to continue?";
            }
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(message, caption, buttons);

            if (result == DialogResult.Yes)
            {
                return 0;
            } else {
                return 1;//err
            }
        }
        public static unsafe ushort fGetTag(ushort tag, byte[] data_out, uint max_len)
        {
            uint rc;
            ushort len;
            fixed (byte* p = data_out)
            {
                rc = emv_kernelGetCardData(tag, p, &len, max_len);
            }
            if (rc == 0)
            {//ok
                return len;
            }
            else
            {
                return 0;
            }
        }
        public static unsafe string fGetTagInString(ushort tag)
        {
            string s;
            //ushort tag_len = 0;
            byte[] tag_val = new byte[1000];
            uint rc;
            s = "";

            rc = fGetTag(tag, tag_val, (uint)tag_val.Length); 
            if (rc > 0)
            {
                byte[] hex = new byte[rc * 4];
                OtherFunc.BinToHex( tag_val, hex, (int) rc);

                s = s 
                    + tag.ToString("X") 
                    + rc.ToString("X2")
                    + Encoding.ASCII.GetString(hex);
                SetText("\r\n" + s );
            }
            else
            {
                SetText("\r\n Error tag " + tag.ToString("X"));
                s = "";
            }
            return s;
        }
        private static void CreateField_55()
        {
            byte[] _55 = new byte[1000];
            ushort _55_len;
            //ushort tag_len = 0;
            byte[] tag_val = new byte[1000];
            uint rc;
            _55.Initialize();
            _55_len = 0;

            rc = fGetTag(0x9F26, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F27, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F10, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F37, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F36, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x95,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9A,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9C,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F02, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x5F2A, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x82,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F1A, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F03, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F33, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x4F,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F08, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F34, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F35, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F1E, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F53, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x84,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F09, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0x9F41, tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            rc = fGetTag(0xCB,   tag_val, (uint)tag_val.Length); if (rc > 0) { Array.Copy(tag_val, 0, _55, _55_len, (int)rc); _55_len += (ushort)rc; }
            if (_55_len > 0)
            {
                byte[] hex = new byte[_55_len*4];
                OtherFunc.BinToHex(_55, hex, (int)_55_len);
                SetText("\r\n55 = " + Encoding.ASCII.GetString(hex));
                SetText("\r\n");
            }
            
            string[] s = new string[100];
            int i = 0;

            s[i++] =fGetTagInString(0x9F26);
            s[i++] =fGetTagInString(0x9F27);
            s[i++] =fGetTagInString(0x9F10);
            s[i++] =fGetTagInString(0x9F37);
            s[i++] =fGetTagInString(0x9F36);
            s[i++] =fGetTagInString(0x95  );
            s[i++] =fGetTagInString(0x9A  );
            s[i++] =fGetTagInString(0x9C  );
            s[i++] =fGetTagInString(0x9F02);
            s[i++] =fGetTagInString(0x5F2A);
            s[i++] =fGetTagInString(0x82  );
            s[i++] =fGetTagInString(0x9F1A);
            s[i++] =fGetTagInString(0x9F03);
            s[i++] =fGetTagInString(0x9F33);
            s[i++] =fGetTagInString(0x4F  );
            s[i++] =fGetTagInString(0x9F08);
            s[i++] =fGetTagInString(0x9F34);
            s[i++] =fGetTagInString(0x9F35);
            s[i++] =fGetTagInString(0x9F1E);
            s[i++] =fGetTagInString(0x9F53);
            s[i++] =fGetTagInString(0x84  );
            s[i++] =fGetTagInString(0x9F09);
            s[i++] =fGetTagInString(0x9F41);
            s[i++] =fGetTagInString(0xCB  );

            SetText("\r\n55 = ");
            for (int ii = 0; ii < i; ii++ )
            {
                SetText(s[ii]);
            }
            SetText("\r\n");
        }

        static OtherFunctions OtherFunc = new OtherFunctions();

        public static unsafe byte emv_fHostSwop(IntPtr lpHandle, 
            //EMV data after first GAC ( Generate Application Cryptogram )
            byte* btAC1, 
            uint dwAC1Size, 
            //Pointer to buffer for host answer
            byte* lpbtHostCr, 
            uint* lpdwHostCrSize, 
            uint dwMaxSize, 
            //Pointer to buffer for script which should send before GAC2
            byte* lpbtTag71, 
            uint* lpdwTag71Size, 
            uint dwTag71Size, 
            //Pointer to buffer for script which should send after GAC2
            byte* lpbtTag72, 
            uint* lpdwTag72Size,
            uint dwTag72Size)
        {
            SetText("emv_fHostSwop" + "\r\n");

            CreateField_55();

            byte[] bin = new byte[dwAC1Size];
            byte[] hex = new byte[dwAC1Size * 4 + 4];

            Marshal.Copy((IntPtr)btAC1, bin, 0, (int)dwAC1Size);

            OtherFunc.BinToHex(bin, hex, (int)dwAC1Size);
            SetText("btAC1 = " + Encoding.ASCII.GetString(hex));
            SetText("\r\n");

            byte[] host_answer = new byte[dwMaxSize]; 
            ASCIIEncoding _Encoding = new ASCIIEncoding();
            byte[] buffer = new byte[dwMaxSize];

            string sHostAnswer = 
                "910A993680ED37FC095330308A023030"
#if USE_UNITED_HOST_ANSWER
                //+
                //"7180"
                //+
                //"717E9F180411223344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD860D8416000008AABB1122CCDD3344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD"
                //+
                //"7280"
                //+
                //"727E9F180411223344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD860D8416000008AABB1122CCDD3344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD"
#endif
                ;
            _Encoding.GetBytes(sHostAnswer, 0, sHostAnswer.Length, buffer, 0);
            OtherFunc.HexToBin(buffer, host_answer, sHostAnswer.Length / 2);
            if ( (sHostAnswer.Length / 2) < dwMaxSize)
            {
                *lpdwHostCrSize = (uint)(sHostAnswer.Length / 2);
                Marshal.Copy(host_answer, 0, (IntPtr)lpbtHostCr, (int)*lpdwHostCrSize);
            }

#if USE_UNITED_HOST_ANSWER
            //
#else
            /*
            string tag_71 = "717E9F180411223344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD860D8416000008AABB1122CCDD3344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD";
            _Encoding.GetBytes(tag_71, 0, tag_71.Length, buffer, 0);
            OtherFunc.HexToBin(buffer, host_answer, tag_71.Length / 2);
            if ((tag_71.Length / 2) < dwTag71Size)
            {
                *lpdwTag71Size = (uint)(tag_71.Length / 2);
                Marshal.Copy(host_answer, 0, (IntPtr)lpbtTag71, (int)*lpdwTag71Size);
            }

            string tag_72 = "727E9F180411223344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD860D8416000008AABB1122CCDD3344860D8416000008AABBCCDD11223344860D8424000008AA11223344BBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD86098424000004AABBCCDD";
            _Encoding.GetBytes(tag_72, 0, tag_72.Length, buffer, 0);
            OtherFunc.HexToBin(buffer, host_answer, tag_72.Length / 2);
            if ((tag_72.Length / 2) < dwTag72Size)
            {
                *lpdwTag72Size = (uint)(tag_72.Length / 2);
                Marshal.Copy(host_answer, 0, (IntPtr)lpbtTag72, (int)*lpdwTag71Size);
            }
            */
#endif
            return 0;
        }

        static byte Lrc(byte[] p, int ind, int len)
        {
            byte lrc = 0;
            int i = ind;
            while (len != 0)
            {
                lrc ^= p[i];
                i++;
                len--;
            }

            return lrc;
        }
        public static unsafe byte emv_fIssuerReferral(IntPtr lpHandle, byte* lpbtPan, byte btSize)
        {
            SetText("emv_fIssuerReferral\r\n");
            return 0;
        }

#if USE_CALLBACK_FOR_CARD_READER

        private static ushort apdu_data_len;
        private static byte[] apdu_cmd = new byte[5];
        private static byte[] apdu_data = new byte[356];

        private static ushort apdu_sw;
        private static byte[] apdu_answer = new byte[356];
        private static ushort apdu_answer_len;

        static void CardSimulator_1()
        {
            //ASCIIEncoding Encoding = new ASCIIEncoding();

            byte[] buffer_cmd = new byte[256];
            byte[] buffer_data = new byte[256];

            Array.Clear(buffer_cmd, 0, buffer_cmd.Length);
            OtherFunc.BinToHex(apdu_cmd, buffer_cmd, 5);
            string sCmd = Encoding.ASCII.GetString(buffer_cmd,0,10);
            SetText("cmd == " + sCmd);

            if(apdu_data_len > 0)
            {
                Array.Clear(buffer_data, 0, buffer_data.Length);
                OtherFunc.BinToHex(apdu_data, buffer_data, apdu_data_len);
                string sData = Encoding.ASCII.GetString(buffer_data,0,apdu_data_len*2);
                SetText(" data = " + sData);

                if (sCmd.Equals("00A404000E") && sData.Equals("315041592E5359532E4444463031"))
                {
                    apdu_sw = 0x6A82;
                } else
                if (sCmd.Equals("00A4040007") && sData.Equals("A0000000031010"))
                {
                    apdu_sw = 0x9000;
                    string str = "6F318407A0000000031010A526500B56495341204352454449549F120F4352454449544F20444520564953418701019F110101";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[256];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("80A8000002") && sData.Equals("8300"))
                {
                    apdu_sw = 0x9000;
                    string str = "800E5C00080101001001030018010201";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[256];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("80AE80001D"))// && sData.Equals("00000001000000000000000008404000048000064313083100E9C4B090"))
                {
                    apdu_sw = 0x9000;
                    string str = "80128000018B5911CDEBC190F506010A03900000";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[256];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("008200000A"))// && sData.Equals("993680ED37FC09533030"))
                {
                    apdu_sw = 0x9000;
                }
                else
                    if (sCmd.Equals("80AE40001F"))// && sData.Equals("3030000000010000000000000000084040000480000643130831006D4B0149"))
                {
                    apdu_sw = 0x9000;
                    string str = "8012400001492136F956F8B83006010A03600000";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[256];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Contains("00A404000"))
                {
                    apdu_sw = 0x6A82;
                } else
                {
                    SetText("\r\nNo Equals");    
                }
                 
            } else {
                if (sCmd.Equals("00B2010C00"))
                {
                    apdu_sw = 0x9000;
                    string str = "70435F201A564953412041435155495245522054455354204341524420303157114761739001010010D101220111438780899F1F1031313433383030373830303030303030";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2011400"))
                {
                    apdu_sw = 0x9000;
                    string str = "7081939081908B3901F6253048A8B2CB08974A4245D90E1F0C4A2A69BCA469615A71DB21EE7B3AA94200CFAEDCD6F0A7D9AD0BF79213B6A418D7A49D234E5C9715C9140D87940F2E04D6971F4A204C927A455D4F8FC0D6402A79A1CE05AA3A526867329853F5AC2FEB3C6F59FF6C453A7245E39D73451461725795ED73097099963B82EBF7203C1F78A529140C182DBBE6B42AE00C02";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2021400"))
                {
                    apdu_sw = 0x9000;
                    string str = "702D8F01959F320103922433F5E4447D4A32E5936E5A1339329BB4E8DD8BF0044CE4428E24D0866FAEFD2348809D71";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2031400"))
                {
                    apdu_sw = 0x9000;
                    string str = "708193938190919D6C210B3981D1C99B3AD55EDF36A138FFAD54D838FA40622AB97046E05EA6E6230AB89D5BE871114EB5431B97403B8C3D2D4CA9BB625AC13FD8C6B825433656CB56557AAC396D945F6D4014FB6E71E8DBEA74B285E9CF3FCEABDFA61D5A4BE16DAAA433F7F2644B178A7DD93DA98BB9D10E84298BDB6B6AE02D04E6E5558C77E79F82C9E046DF821DD0277AB9D00C";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2011C00"))
                {
                    apdu_sw = 0x9000;
                    string str = "700E5A0847617390010100105F340101";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2021C00"))
                {
                    apdu_sw = 0x9000;
                    string str = "707D9F420208405F25039507015F24031012319F0802008C5F280208405F300202019F0702FF008E0E00000000000000001E0302031F008C159F02069F03069F1A0295055F2A029A039C019F37048D178A029F02069F03069F1A0295055F2A029A039C019F37049F0D05F0400088009F0E0500100000009F0F05F040009800";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                {
                    SetText("\r\nNo Equals");    
                }
            }
        }

        static void CardSimulator_2()
        {
            //ASCIIEncoding Encoding = new ASCIIEncoding();

            byte[] buffer_cmd = new byte[256];
            byte[] buffer_data = new byte[256];

            Array.Clear(buffer_cmd, 0, buffer_cmd.Length);
            OtherFunc.BinToHex(apdu_cmd, buffer_cmd, 5);
            string sCmd = Encoding.ASCII.GetString(buffer_cmd, 0, 10);
            SetText("cmd == " + sCmd);

            if (apdu_data_len > 0)
            {
                Array.Clear(buffer_data, 0, buffer_data.Length);
                OtherFunc.BinToHex(apdu_data, buffer_data, apdu_data_len);
                string sData = Encoding.ASCII.GetString(buffer_data, 0, apdu_data_len * 2);
                SetText(" data = " + sData);

                if (
                    sCmd.Equals("8416000008") ||
                    sCmd.Equals("8424000008") ||
                    sCmd.Equals("8416000008") ||
                    sCmd.Equals("8416000008") ||
                    sCmd.Equals("8424000008") ||
                    sCmd.Equals("8424000004") ||
                    sCmd.Equals("8424000004") ||
                    sCmd.Equals("8424000004") ||
                    sCmd.Equals("8424000004")
                    )
                {
                    apdu_sw = 0x9000;
                }
                else
                    if (sCmd.Equals("00A404000E") && sData.Equals("315041592E5359532E4444463031"))
                {
                    apdu_sw = 0x9000;
                    string str = "6F15840E315041592E5359532E4444463031A503880101";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[256];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                    if (sCmd.Equals("00A4040007") && sData.Equals("A0000000041010"))
                    {
                        apdu_sw = 0x9000;
                        string str = "6F0E8407A0000000041010A503870101";
                        ASCIIEncoding _Encoding = new ASCIIEncoding();
                        byte[] buffer = new byte[256];
                        _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                        apdu_answer_len = (ushort)(str.Length / 2);
                        OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                    }
                    else
                        if (sCmd.Equals("80A8000002") && sData.Equals("8300"))
                        {
                            apdu_sw = 0x9000;
                            string str = "770E8202580094081001040018010201";
                            ASCIIEncoding _Encoding = new ASCIIEncoding();
                            byte[] buffer = new byte[256];
                            _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                            apdu_answer_len = (ushort)(str.Length / 2);
                            OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                        }
                        else
                            if (
                                sCmd.Equals("80AE000020")
                                ||
                                sCmd.Equals("80AE400020")
                                ||
                                sCmd.Equals("80AE800020")
                            )
                            {
                                apdu_sw = 0x9000;
                                string str = "771F9F2701809F360200959F2608D3C06556C7B7E0E99F10080101038000009294";
                                ASCIIEncoding _Encoding = new ASCIIEncoding();
                                byte[] buffer = new byte[256];
                                _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                                apdu_answer_len = (ushort)(str.Length / 2);
                                OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                            }
                            else
                            {
                                SetText("\r\nNo Equals");
                            }

            }
            else
            {
                if (sCmd.Equals("00B2010C00"))
                {
                    apdu_sw = 0x9000;
                    string str = "701A61184F07A0000000041010500A4D617374657243617264870101";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                if (sCmd.Equals("00B2020C00"))
                {
                    apdu_sw = 0x6A83;
                }
                else
                    if (sCmd.Equals("00B2011400"))
                {
                    apdu_sw = 0x9000;
                    string str = "702D8F01479224463571D2D815C68EA388BEF5D84EE75D3935462908C0ABC53CDAF35301F556EB694FEFCF9F320103";
                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                    byte[] buffer = new byte[556];
                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                    apdu_answer_len = (ushort)(str.Length / 2);
                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                }
                else
                    if (sCmd.Equals("00B2021400"))
                    {
                        apdu_sw = 0x9000;
                        string str = "7081939081903F16C67A0600781819A0B98065168D4AC8C5D9B4620D1BC04F25BC3F6C835781634F00DB7CE1F8EC1A7E36F812BAA1AC6C54E686524C77DB27E5821967C8348334871BBF34991B359E8C2D7F9C01B996C3D684DE2CB214F111569D80CE78061AE17A8635C453676BA4BFE94965057EE8431B0D326671730C0A2BC2D6EBE2DAB3882D43428BEAD5400FCC869A89814739";
                        ASCIIEncoding _Encoding = new ASCIIEncoding();
                        byte[] buffer = new byte[556];
                        _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                        apdu_answer_len = (ushort)(str.Length / 2);
                        OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                    }
                    else
                        if (sCmd.Equals("00B2031400"))
                        {
                            apdu_sw = 0x9000;
                            string str = "70258C1B9F02069F03069F1A0295055F2A029A039C019F37049F35019F45028D06910A8A029505";
                            ASCIIEncoding _Encoding = new ASCIIEncoding();
                            byte[] buffer = new byte[556];
                            _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                            apdu_answer_len = (ushort)(str.Length / 2);
                            OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                        }
                        else
                            if (sCmd.Equals("00B2041400"))
                            {
                                apdu_sw = 0x9000;
                                string str = "708193938190A8146FE50946399F5892F547E38C3C260798DACBDB0791B4BA6748AD868F0CC0CD94C6F5C9FDCA9D852D522F30C500DE214D83341D586D4A200BC5955AEE53766C888A32ADE153D3BECE6129A0CDE1A79B080A4D76F7ADD780063EC8E026975FE475C99949FCE1FE0AC0EBD66648DB0D4365A5835D789E6F3DEBD3A86893D0E52831B7A0822465333DFA3C92ACD35948";
                                ASCIIEncoding _Encoding = new ASCIIEncoding();
                                byte[] buffer = new byte[556];
                                _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                                apdu_answer_len = (ushort)(str.Length / 2);
                                OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                            }
                            else
                                if (sCmd.Equals("00B2011C00"))
                                {
                                    apdu_sw = 0x9000;
                                    string str = "704E5F24034412315A0810000312000000505F3401049F0702FF005F280206438E120000000000000000410342035E0341031F039F4A01829F0D05F050A400009F0E0500000000009F0F05F070A49800";
                                    ASCIIEncoding _Encoding = new ASCIIEncoding();
                                    byte[] buffer = new byte[556];
                                    _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                                    apdu_answer_len = (ushort)(str.Length / 2);
                                    OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                                }
                                else
                                    if (sCmd.Equals("00B2021C00"))
                                    {
                                        apdu_sw = 0x9000;
                                        string str = "705C5F201A43415244352F454D56202020202020202020202020202020202057131000031200000050D44122011346700000639F9F1F183133343637303030303036333930303633393030303030309F080200029F420206435F30020201";
                                        ASCIIEncoding _Encoding = new ASCIIEncoding();
                                        byte[] buffer = new byte[556];
                                        _Encoding.GetBytes(str, 0, str.Length, buffer, 0);
                                        apdu_answer_len = (ushort)(str.Length / 2);
                                        OtherFunc.HexToBin(buffer, apdu_answer, (int)apdu_answer_len);
                                    }
                                    else
                                    {
                                        SetText("\r\nNo Equals");
                                    }
            }
        }

        /**
        **********************************************************************************************
        *	@fn WORD WINAPI emv_ApduIn(BYTE *Cmd,BYTE * Data,WORD *Data_len,BYTE *answer,WORD answer_max_len)
        *  @b Назначение:							\n
        *	@brief Посылает APDU команду с данными и получает ответ. Используется EMV ядром.	\n
        *  @author  
        **********************************************************************************************
        *  @b Выполняет:
        *	@code
        *  1.) Посылает APDU команду и получает ответ.
        *  2.) Возвращает данные в буфер.
        *	@endcode
        **********************************************************************************************
        *  @param Вход: \n
        *  @b BYTE * Cmd;	– Буфер, содержащий APDU команду.
        *  @b BYTE * Data;	– Буфер, содержащий данные.
        *  @b WORD *Data_len;	- Длина даннных.
        *  @b WORD answer_max_len;	- Размер буфера для ответа.
        *
        *  @param Выход: \n
        *  @b BYTE * answer;	– Буфер для ответа от карты на APDU команду.
        *  @b WORD *Data_len;	- Длина полученых от карты даннных.
        **********************************************************************************************
        *  @return 
        *  @b WORD;	- Статус, полученный от карты на APDU команду.
        **********************************************************************************************
        */
        public static unsafe ushort emv_ApduIn(byte* cmd, byte* data, ushort* data_len, byte* answer, ushort answer_max_len)
        {
            SetText("\r\nemv_ApduIn()...");

            Marshal.Copy((IntPtr)cmd, apdu_cmd, 0, 5);
            apdu_data_len = *data_len;
            Marshal.Copy((IntPtr)data, apdu_data, 0, apdu_data_len);

            apdu_sw = 0;
            apdu_answer_len = 0;
            Array.Clear(apdu_answer, 0, apdu_answer.Length);

            CardSimulator_1();

            if (apdu_answer_len > 0 && answer_max_len > apdu_answer_len)
            {
                Marshal.Copy(apdu_answer, 0, (IntPtr)answer, (int)apdu_answer_len);
            }
            *data_len = apdu_answer_len;
            return apdu_sw;
        }


        /**
         **********************************************************************************************
         *	@fn WORD emv_ApduOut(BYTE *Cmd,WORD *Stat,BYTE *answer,WORD answer_max_len)
         *  @b Назначение:						\n
         *	@brief Посылает APDU команду и получает ответ. Используется EMV ядром.	\n
         *  @author  
         **********************************************************************************************
         *  @b Выполняет:
         *	@code
         *  1.) Посылает APDU команду и получает ответ.
         *  2.) Возвращает данные в буфер.
         *	@endcode
         **********************************************************************************************
         *  @param Вход: \n
         *  @b BYTE * Cmd;	– Буфер, содержащий APDU команду.
         *  @b WORD answer_max_len;	- Размер буфера для ответа.
         *
         *  @param Выход: \n
         *  @b WORD * Stat;	– Возвращается статус, полученный от карты на APDU команду.
         *  @b BYTE * answer;	– Буфер для ответа от карты на APDU команду.
         **********************************************************************************************
         *  @return 
         *  @b WORD;	- Длинна полученного ответа от карты.
         **********************************************************************************************
        */
        public static unsafe ushort emv_ApduOut(byte* cmd, ushort* stat, byte* answer, ushort answer_max_len)
        {
            SetText("\r\nemv_ApduOut()...");

            Marshal.Copy((IntPtr)cmd, apdu_cmd, 0, 5);
            apdu_data_len = 0;

            apdu_sw = 0;
            apdu_answer_len = 0;
            Array.Clear(apdu_answer, 0, apdu_answer.Length);

            CardSimulator_1();

            if (apdu_answer_len > 0 && answer_max_len > apdu_answer_len)
            {
                Marshal.Copy(apdu_answer, 0, (IntPtr)answer, (int)apdu_answer_len);
            }
            *stat = apdu_sw;

            return apdu_answer_len;
        }
        /**
         **********************************************************************************************
         *	@fn BYTE emv_Atr(BYTE *atr, DWORD * max_len, BYTE * protocol) 
         *  @b Назначение:							\n
         *	@brief Получает ATR, используется EMV ядром.	\n
         *  @author  Ilya Yalin
         **********************************************************************************************
         *  @b Выполняет:
         *	@code
         *  1.) Посылает ридеру команду подать питание на карточку.
         *  2.) Получает от карточки ATR.
         *	@endcode
         **********************************************************************************************
         *  @param Вход: \n
         *  @b DWORD * max_len;	- Максимальный размер буфера для приёма данных ATR.
         *
         *  @param Выход: \n
         *  @b BYTE *atr	- Указатель на буфер для данных ATR.
         *  @b DWORD * max_len;	- Размер полученных данных.
         *  @b BYTE * protocol;	- Адрес для получения протокола. Для моторизованных ридеров всегда 0.
         **********************************************************************************************
         *  @return 
         *  @b BYTE;	- Статус выполнения операций.
         *	@b 1 – операция завершилась успешно.
         *	@b 0 – операция завершилась не удачей.
         **********************************************************************************************
        */
        public static unsafe byte emv_Atr(byte* dest_atr, uint* max_len, byte* protocol)
        {
            SetText("\r\nemv_Atr()...");
            string sAtr ="3B620000811F";
            uint atr_len = (uint)(sAtr.Length / 2);

            if (atr_len > 0 && atr_len < * max_len)
            {
                byte[] buffer = new byte[256];
                byte[] bAtr = new byte[256];
                ASCIIEncoding Encoding = new ASCIIEncoding();
                Encoding.GetBytes(sAtr, 0, sAtr.Length, buffer, 0);

                OtherFunc.HexToBin(buffer, bAtr, (int)atr_len);

                Marshal.Copy(bAtr, 0, (IntPtr)dest_atr, (int)atr_len);
                *max_len = (uint)atr_len;
                *protocol = 0;
                return 1;//ok
            }

            return 0;//err
        }

        /**
        **********************************************************************************************
        *	@fn BYTE emv_CheckConnect()
        *  @b Назначение:							\n
        *	@brief Проверяет наличие карты в ридере.	\n
        *  @author  
        **********************************************************************************************
        *  @b Выполняет:
        *	@code
        *  1.) Проверяет флаг наличия карты в ридере. Флаг выставляется после выполнения TakeCard(), если была подача карты.
        *	@endcode
        **********************************************************************************************
        *  @param Вход: \n
        *  @b None
        *
        *  @param Выход: \n
        *  @b None
        **********************************************************************************************
        *  @return 
        *  @b BYTE;	- Результат выполнения.
        *  @b 1	- Карта присутствует.
        *  @b 0	- Карта отсуттствует.
        **********************************************************************************************
        */
        private static byte emv_CheckConnect()
        {
            SetText("\r\nemv_CheckConnect()...\r\n");
            return 1; //TRUE
            //return 0;//FALSE
        }
#endif

        unsafe FUNC_PTR_emv_fSelLang        _emv_fSelLang           = new FUNC_PTR_emv_fSelLang         (emv_fSelLang);
        unsafe FUNC_PTR_emv_fSelApp         _emv_fSelApp            = new FUNC_PTR_emv_fSelApp          (emv_fSelApp);
        unsafe FUNC_PTR_emv_fAppSelected    _emv_fAppSelected       = new FUNC_PTR_emv_fAppSelected     (emv_fAppSelected);
        unsafe FUNC_PTR_emv_fAttGetTrData   _emv_fAttGetTrData      = new FUNC_PTR_emv_fAttGetTrData    (emv_fAttGetTrData);
        unsafe FUNC_PTR_emv_fGetTrData      _emv_fGetTrData         = new FUNC_PTR_emv_fGetTrData       (emv_fGetTrData);
        unsafe FUNC_PTR_emv_fGetCryptPin    _emv_fGetCryptPin       = new FUNC_PTR_emv_fGetCryptPin     (emv_fGetCryptPin);
        unsafe FUNC_PTR_emv_fGetPin         _emv_fGetPin            = new FUNC_PTR_emv_fGetPin          (emv_fGetPin);
        unsafe FUNC_PTR_emv_fHostSwop       _emv_fHostSwop          = new FUNC_PTR_emv_fHostSwop        (emv_fHostSwop);
        unsafe FUNC_PTR_emv_fIssuerReferral _emv_fIssuerReferral    = new FUNC_PTR_emv_fIssuerReferral  (emv_fIssuerReferral);
        unsafe FUNC_PTR_emv_fSelectAccount  _emv_fSelectAccount     = new FUNC_PTR_emv_fSelectAccount   (emv_fSelectAccount);
#if USE_CALLBACK_FOR_CARD_READER
        unsafe FUNC_PTR_emv_ApduIn          _emv_ApduIn             = new FUNC_PTR_emv_ApduIn           (emv_ApduIn);
        unsafe FUNC_PTR_emv_ApduOut         _emv_ApduOut            = new FUNC_PTR_emv_ApduOut          (emv_ApduOut);
        unsafe FUNC_PTR_emv_Atr             _emv_Atr                = new FUNC_PTR_emv_Atr              (emv_Atr);
        unsafe FUNC_PTR_emv_CheckConnect    _emv_CheckConnect       = new FUNC_PTR_emv_CheckConnect     (emv_CheckConnect);
#endif
        static IntPtr hEmvInit = IntPtr.Zero;
        static IntPtr lpCallbackHandle = IntPtr.Zero;
        static byte btTrStatus = 0;
        static byte[] data = new byte[512];
        static uint data_len;
        public unsafe string fEmvKernelInit()
        {
            hEmvInit = emv_kernelInit(null);
            if (hEmvInit != (IntPtr)null)
            {
                if (0 != emv_kernel_registry_fSelLang       (hEmvInit, _emv_fSelLang))          return "Err registry_emv_fSelLang         ";//(NULL, NAME_FUNC_emv_APP_SELECTED, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fSelApp        (hEmvInit, _emv_fSelApp))           return "Err registry_emv_fSelApp          ";//(NULL, NAME_FUNC_emv_SELECT_LANG, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fAppSelected   (hEmvInit, _emv_fAppSelected))      return "Err registry_emv_fAppSelected     ";//(NULL, NAME_FUNC_emv_SELECT_APPL, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fGetTrData     (hEmvInit, _emv_fGetTrData))        return "Err registry_emv_fGetTrData       ";//(NULL, NAME_FUNC_emv_GET_TR_DATA, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fGetCryptPin   (hEmvInit, _emv_fGetCryptPin))      return "Err registry_emv_fGetCryptPin     ";//(NULL, NAME_FUNC_emv_GET_CRYPT_PIN, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fGetPin        (hEmvInit, _emv_fGetPin))           return "Err registry_emv_fGetPin          ";//(NULL, NAME_FUNC_emv_GET_PIN, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fHostSwop      (hEmvInit, _emv_fHostSwop))         return "Err registry_emv_fHostSwop        ";//(NULL, NAME_FUNC_emv_HOST_SWOP, "EmvWeb err", MB_OK);            
                if (0 != emv_kernel_registry_fIssuerReferral(hEmvInit, _emv_fIssuerReferral))   return "Err registry_emv_fIssuerReferral  ";//(NULL, NAME_FUNC_emv_ISSUER_REFERRAL, "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_fSelectAccount (hEmvInit, _emv_fSelectAccount))    return "Err registry_emv_fSelectAccount   ";//(NULL, NAME_FUNC_emv_SELECT_ACCOUNT, "EmvWeb err", MB_OK);
#if USE_CALLBACK_FOR_CARD_READER
#if USE_DEMO_SMART_CARD_SIMULATOR_DLL
                //
#else
                if (0 != emv_kernel_registry_ApduIn         (hEmvInit, _emv_ApduIn))            return "Err registry_emv_ApduIn           ";//(NULL, "WebSendApduIn", "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_ApduOut        (hEmvInit, _emv_ApduOut))           return "Err registry_emv_ApduOut          ";//(NULL, "WebSendApduOut", "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_Atr            (hEmvInit, _emv_Atr))               return "Err registry_emv_Atr              ";//(NULL, "Atr", "EmvWeb err", MB_OK);
                if (0 != emv_kernel_registry_CheckConnect   (hEmvInit, _emv_CheckConnect))      return "Err registry_emv_CheckConnect     ";//(NULL, "CheckConnect", "EmvWeb err", MB_OK);
#endif
#endif
                return "emv_kernelInit() == OK!";
            }
            else
            {
                return "emv_kernelInit() == null!";
            }
        }
#if USE_CALLBACK_FOR_CARD_READER
        //
#else
        byte[] readerName = new byte[256];
        public unsafe string Get_USB_ReaderName(int index)
        {
            try
            {
                if (GetReaderName(readerName, (int)readerName.Length, index) == 1)//TRUE
                {
                    return Encoding.ASCII.GetString(readerName);
                }
                else
                {
                    return null;
                }
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nGetReaderName() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("GetReaderName() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
                return null;
            }
        }
        public unsafe bool Open_USB_CardReader(string readerName)
        {
            if (readerName != null )
            {
                SetText("\r\nOpening of USB reader: "
                    + readerName
                    + "\r\n");

                try
                {
                    if ((byte)OpenReader(readerName, "") != 0)
                    {
                        SetText("\r\nUSB reader open ERROR!\r\n");
                        return false;
                    }
                    else
                    {
                        SetText("\r\nUSB reader open OK!\r\n");
                        return true;
                    }
                }
                catch //(EntryPointNotFoundException ex)
                {
                    SetText("\r\nOpenReader() ERROR!\r\n");
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    result = MessageBox.Show("OpenReader() from SmartCard.dll ERROR!", "Exit or continue?", buttons);
                    return false;
                }
            }
            else
            {
                SetText("\r\nUSB reader open ERROR!\r\n");
                SetText("\r\nPlease select USB Reader!\r\n");
                return false;
            }
        }
        public unsafe bool Open_COM_CardReader(string readerName, string com_port)
        {
            //string readerName = "Sankyo ICT3K5";
            //string com_port = "COM4";
            if(readerName != null && com_port != null)
            {
                SetText("\r\nOpening of reader: "
                    + readerName
                    + " "
                    + com_port
                    + "\r\n");

                if ((byte)OpenReader(readerName, com_port) != 0)
                {
                    SetText("reader open ERROR!\r\n");
                    return false;
                }
                else
                {
                    SetText("reader open OK!\r\n");
                    return true;
                }
            } else {
                SetText("reader open ERROR!\r\n");
                SetText("Please select COM Port And Reader!\r\n");
                return false;
            }
        }
        public unsafe bool Wait_USB_CardIn()
        {
            SetText("Please insert USB card\r\n");

            try
            {
                while ((byte)CheckConnect() == 0)
                {
                    SetText(".");
                    Thread.Sleep(1000);
                }
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nCheckConnect() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("CheckConnect() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
                return false;
            }

            SetText("\r\nTakeCard() OK!\r\n");
            return true;
        }
        public unsafe bool WaitCardIn()
        {
            UInt32 time_out = 5000;
            SetText("Please insert card\r\n");
            byte rc;

            try
            {
                rc = TakeCard(time_out);
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nTakeCard() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("TakeCard() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
                return false;
            }

            SetText("\r\nTakeCard(" + time_out.ToString() + ") = " + rc.ToString());
            if (rc == 0)
            {                
                {
                    byte res = 0;
                    byte[] data = new byte[2560];
                    fixed (byte* p = data)
                    {
                        uint[] pdata_len = new uint[1];
                        fixed (uint* d = pdata_len)
                        {
                            try
                            {
                                res = GetMagData(p, d);
                            }
                            catch //(EntryPointNotFoundException ex)
                            {
                                SetText("\r\nGetMagData() from SmartCard.dll ERROR!\r\n");

                                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                                DialogResult result;
                                result = MessageBox.Show("GetMagData() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                                //if (result == DialogResult.Yes)
                                //{
                                //    return 0;
                                //}
                                //else
                                //{
                                //    return 1;//err
                                //}
                                return false;
                            }


                            if (res == 0 || res == 0x0A)
                            {
                                byte[] hex = new byte[pdata_len[0] * 2 + 2];
                                OtherFunc.BinToHex(data, hex, (int)pdata_len[0]);
                                
                                ASCIIEncoding Encoding = new ASCIIEncoding();
                                string content = Encoding.GetString(hex, 0, (int)pdata_len[0] * 2);
                                SetText("\r\nMagnit Data in HEX = " + content);

                                content = Encoding.GetString(data, 0, (int)pdata_len[0]);
                                SetText("\r\nMagnit Data in string = " + content.Replace('\0',' '));

                                SetText("\r\nGetMagData() OK");
                            }
                            else
                            {
                                SetText("\r\nGetMagData() err...");
                            }
                        }
                    }
                }                

                SetText("\r\nTakeCard() OK!\r\n");
                return true;
            }
            else
            {
                SetText("\r\nTakeCard() ERROR!\r\n");
                return false;
            }
        }
        public void WaitCardOut()
        {
            SetText("\r\nPlease remove card\r\n");
            while ((byte)CheckConnect() == 1)
            {
                SetText(".");
                Thread.Sleep(1000);
            }
            SetText("\r\n");
        }
        public void CloseCardReader()
        {
            SetText("\r\nClose Reader...\r\n");
            try
            {
                CloseReader();
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nCloseReader() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("CloseReader() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
            }
        }
        public void InitReader()
        {
            try
            {
                Initialize();
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nInitialize() from SmartCard.dll ERROR!\r\n");
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("Initialize() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
            }
        }
        public void UninitReader()
        {
            try
            {
                UnInit();
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nUnInit() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("UnInit() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
            }
        }
#endif
        public unsafe string EmvRun(string str_data)
        {
            string retVal = "";
            Array.Clear(data, 0, data.Length);
            Encoding.ASCII.GetBytes(str_data, 0, str_data.Length, data, 0);
            byte rc_dword = 0;

            SetText("\r\nStart EMV Transaction...\r\n");
            fixed (byte* p = data)
            {
                fixed (uint* d = &data_len)
                {
                    rc_dword = (byte)emv_kernel_run(
                    hEmvInit,
                    lpCallbackHandle,
                    btTrStatus,
                    p,
                    d,
                    (uint)data.Length);
                }
            }

            //emv_kernelGetCardData(ushort dwTag, byte* lpbtData, ushort* lpdwSize, uint dwMaxSize);
            InGetTrData();
            byte *tag=null;
            byte *lpdata=null;
            uint* si=null;
            //byte retVa_l=(byte)emv_kernelGetCardData(0,lpdata,si,2048);
            emv_kernelGetDataByTag(hEmvInit, ref tag, 2048, ref lpdata,ref si, 2048);

            SetText("\r\nEMV result = " + "0x"+rc_dword.ToString("X"));

            SetText("\r\nEMV result can be:");
            SetText("\r\n	AC1:                       ");
            SetText("\r\n	  0x00 - approved          ");
            SetText("\r\n	  0x01 - force accept      ");
            SetText("\r\n	  0x02 - decline           ");
            SetText("\r\n	  0x03 - abort             ");
            SetText("\r\n	  0x04 - capture           ");
            SetText("\r\n	  0x05 - decline & advice  ");
            SetText("\r\n	AC2:                       ");
            SetText("\r\n	  0x10 - approved          ");
            SetText("\r\n	  0x11 - force accept      ");
            SetText("\r\n	  0x12 - decline           ");
            SetText("\r\n	  0x13 - reversal          ");
            SetText("\r\n	  0x14 - abort + reversal  ");
            SetText("\r\n	  0x15 - capture           ");
            SetText("\r\n	  0x16 - capture & reversal");
            SetText("\r\n	  0x17 - approved     + advice");
            SetText("\r\n	  0x18 - force accept + advice");
            SetText("\r\n	  0x19 - decline      + advice");
            SetText("\r\n	Other:                     ");
            SetText("\r\n	  0x55 - NotAccepted       ");
            SetText("\r\n	  0x66 - Card Error.       ");

            SetText("\r\nEMV result = " + "0x" + rc_dword.ToString("X"));
            SetText("\r\nPlease take card...");
#if USE_CALLBACK_FOR_CARD_READER
            //
#else
            try
            {
                EjectCard();
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nEjectCard() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("EjectCard() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
            }
#endif
            return retVal;
        }
        public void EmvUnInit()
        {
#if USE_CALLBACK_FOR_CARD_READER
            //
#else
            try
            {
                CloseReader();
            }
            catch //(EntryPointNotFoundException ex)
            {
                SetText("\r\nCloseReader() from SmartCard.dll ERROR!\r\n");

                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show("2 CloseReader() from SmartCard.dll ERROR!", "Exit or continue?", buttons);

                //if (result == DialogResult.Yes)
                //{
                //    return 0;
                //}
                //else
                //{
                //    return 1;//err
                //}
            }
#endif
            emv_kernelUnInit(hEmvInit);
        }
        delegate void SetTextCallback(string text);
        static void SetText(string text)
        {
            if (OutTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                OutTextBox.Invoke(d, new object[] { text });
            }
            else
            {
                OutTextBox.Text += text;
            }
        }
    }
}
