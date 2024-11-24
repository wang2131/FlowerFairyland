using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.UI;

namespace Notification
{
    public class JavaToData : MonoBehaviour
    {
        List<byte> dataList;
        List<byte> dataList1;
        List<byte> dataList2;
        string strMsg, strMsg1, strMsg2 = "";
        byte[] binary_data = new byte[8] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88 };
        public AndroidJavaObject jo;
        List<byte> StringToByteList = new List<byte>();
        List<byte> lenList = new List<byte>() { 0x00, 0x00, 0x00 };
        List<byte> bytesList = new List<byte>() { 0xAA, 0x55, 0x55 };
        private USBCommunication UsbCommunication = new USBCommunication();
        private int ExclusiveOrNum;
        public static bool canError = false;
        
        [SerializeField]
        private Text text;

        void Awake()
        {
            dataList = new List<byte>();
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            canError = false;
        }

        private void Start()
        {
            //vid, pid
            jo.Call("UsbInit", 0xC352, 0x2C02);
            UsbOpen();
            jo.Call("registerReceiver");
            //EnumerateDevice();
            Usb_Senddata(binary_data);
        }

        public void RestartApp()
        {
            jo.CallStatic("ReStartApp", 201);
        }

        public void UsbOpen()
        {
            jo.Call("Open");
        }

        public void EnumerateDevice()
        {
            jo.Call("enumerateDevice");
        }

        public void UsbClose()
        {
            Debug.Log("usb关闭");
            jo.Call("OnClose");
        }

        public void Usb_Senddata(byte[] data)
        {
            jo.Call("SendData", data);
            Debug.Log("Usb_Senddata" + BitConverter.ToString(data));
        }

        public void OnDestroy()
        {
            UsbClose();
        }

        public void ReceiveData(string msg)
        {
            
            //Debug.Log("ReceiveData: " + msg);
            dataList.Clear();
            dataList = StringToByteArray(msg);
            if (dataList[0] == 0xFD) return;
            ReceiveMessage(dataList);
            //java_data(dataList, 0);
        }

        public void USBError(string msg)
        {
            //NotificationCenter.DefaultCenter().PostNotification("OnError", msg);
            Debug.LogError("on received usb error:...." + msg);
            if (canError)
            {
                text.text = msg;
                //Invoke("setErrorNull", 3.0f);
            }

        }

        private void setErrorNull()
        {
            text.text = "";
        }

        List<byte> dataByteList = new List<byte>();

        /// <summary>
        /// ????????
        /// </summary>
        /// <param name="obj"></param>
        public void SendData(object obj)
        {
            dataByteList = StructToBytes(obj);
            byte[] bytes = new byte[dataByteList.Count];
            dataByteList.CopyTo(bytes);
            jo.CallStatic("unity_messageByte", bytes, bytes.Length);
        }

        List<byte> dataByteList1 = new List<byte>();

        /// <summary>
        /// ????????
        /// </summary>
        /// <param name="obj"></param>
        public void SendData1(object obj)
        {
            dataByteList1 = StructToBytes(obj);
            byte[] bytes = new byte[dataByteList1.Count];
            dataByteList1.CopyTo(bytes);
            jo.CallStatic("SendData1", bytes);
        }

        List<byte> dataByteList2 = new List<byte>();

        /// <summary>
        /// ????????
        /// </summary>
        /// <param name="obj"></param>
        public void SendData2(object obj)
        {
            dataByteList2 = StructToBytes(obj);
            byte[] bytes = new byte[dataByteList2.Count];
            dataByteList2.CopyTo(bytes);
            jo.CallStatic("SendData2", bytes);
        }

        //????????λ,Index??0???
        public static int GetbitValue(byte input, int index)
        {
            return input >> index & 1;
        }

        public static byte SetBitOne(byte b, int index)
        {
            return (byte)(b | (1 << index));
        }

        public static byte SetBitZero(byte b, int index)
        {
            return (byte)(b & (byte.MaxValue - (1 << index)));
        }

        /// <summary>
        ///??16?????byte????????
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;

            if (bytes != null)
            {

                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {

                    strB.Append(bytes[i].ToString("X2"));

                }

                hexString = strB.ToString();

            }

            return hexString;
        }

        /// <summary>
        /// ??16??????????????byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public List<byte> StringToByteArray(string str)
        {
            StringToByteList.Clear();
            if (str.Length % 2 == 1)
                return null;
            for (int i = 0; i < str.Length; i += 2)
            {
                StringToByteList.Add(Convert.ToByte(str.Substring(i, 2), 16));
            }

            return StringToByteList;
        }

        public object BytesToStuct(byte[] bytes, Type type)
        {

            int size = Marshal.SizeOf(type);

            if (size > bytes.Length)
            {

                Debug.Log("type: "+ size + "," + "bytes: " + bytes.Length +",   "+ sizeof(short));
                return null;
            }

            IntPtr structPtr = Marshal.AllocHGlobal(size);

            Marshal.Copy(bytes, 0, structPtr, size);

            object obj = Marshal.PtrToStructure(structPtr, type);

            Marshal.FreeHGlobal(structPtr);

            return obj;
        }

        /// <summary>
        /// ?????byte????
        /// </summary>
        /// <param name="structObj">?????????</param>
        /// <returns>??????byte????</returns>
        public List<byte> StructToBytes(object structObj)
        {
            int size = Marshal.SizeOf(structObj);
            byte[] bytes = new byte[size];
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(structObj, structPtr, false);
            Marshal.Copy(structPtr, bytes, 0, size);
            Marshal.FreeHGlobal(structPtr);
            lenList.RemoveRange(3, lenList.Count - 3);
            byte[] len = intToBytes2(size + 2);
            lenList.AddRange(len);
            byte checksum = ExclusiveOr(lenList, lenList.Count);
            bytesList.RemoveRange(3, bytesList.Count - 3);
            bytesList.Add(checksum);
            bytesList.AddRange(bytes);
            checksum = ExclusiveOr(bytesList, bytesList.Count);
            bytesList.Add(checksum);

            return bytesList;
        }

        /// <summary>
        /// int ?byte[]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] intToBytes2(int value)
        {
            byte[] src = new byte[4];
            src[0] = (byte)((value >> 24) & 0xFF);
            src[1] = (byte)((value >> 16) & 0xFF);
            src[2] = (byte)((value >> 8) & 0xFF);
            src[3] = (byte)(value & 0xFF);
            return src;
        }

        /// <summary>
        /// byte[]?int
        /// </summary>
        /// <returns></returns>
        public static int bytesToInt(byte[] b)
        {
            return (int)(b[0] | b[1] << 8 | b[2] << 16 | b[3] << 24);
        }

        byte ExclusiveOr(List<byte> list, int size)
        {
            byte checksum = 0;
            for (int i = 3; i < size; i++)
            {
                checksum ^= list[i];
            }

            return checksum;
        }

        void ReceiveMessage(List<byte> dataList)
        {
            byte[] temp = dataList.ToArray();
            // Debug.Log(BitConverter.ToString(temp));
            if (dataList[0] == 0xFD) return;

            int length = dataList[2];
            
            dataList.RemoveRange(0,4);
            byte[] data = dataList.GetRange(0, length).ToArray();
            byte[] finalData = EncodingCenter.Decode(data);
            Debug.Log(BitConverter.ToString(finalData));
            //Loom.QueueOnMainThread((() => text.text = BitConverter.ToString(temp)));
            List<byte> finalDataList = new List<byte>();
            finalDataList.AddRange(finalData);
            ushort usType = BitConverter.ToUInt16(finalData, 0);
            ushort usVersion = BitConverter.ToUInt16(finalData, 2);
            ushort usLength = BitConverter.ToUInt16(finalData, 4);
            ushort usCmd = BitConverter.ToUInt16(finalData, 6);
            finalDataList.RemoveRange(0, 8);
            USBCommunication.DataDesc dataDesc = new USBCommunication.DataDesc()
            {
                usType = usType,
                bVersionNo = usVersion,
                usLength = usLength,
                bCmd = usCmd,
                arrData = finalDataList.ToArray()
            };
            //Debug.Log("Type:" + dataDesc.usType + ", Version:" + dataDesc.bVersionNo + 
            //          ", Length:" + dataDesc.usLength + ", Cmd:" + dataDesc.bCmd + ", arrData:" + 
            //       BitConverter.ToString(dataDesc.arrData));
            java_data(dataDesc.arrData, dataDesc.bCmd);
            

        }
        void java_data(byte[] dataBytes, ushort cmd)
        {
            Debug.Log("java_data: " + BitConverter.ToString(dataBytes) + ", cmd: " + cmd);
            switch (cmd)
            {
                case (ushort)0X0401:
                    var firstcard = (USBCommunication.FirstCardResult)BytesToStuct(dataBytes, typeof(USBCommunication.FirstCardResult));
                    UsbEventCenter.Broadcast((EventType)EventName.getCardFirst, firstcard);
                    break;
                case (ushort)0x0402:
                    var secondcard = (USBCommunication.SecondCardResult)BytesToStuct(dataBytes, typeof(USBCommunication.SecondCardResult));
                    UsbEventCenter.Broadcast((EventType)EventName.getCardSecond, secondcard);
                    break;
                case (ushort)0x0405:
                    Debug.Log("USBCommunication : Guess");
                    var guessResult = (USBCommunication.DoubleUpResult)BytesToStuct(dataBytes, typeof(USBCommunication.DoubleUpResult));                    
                    UsbEventCenter.Broadcast((EventType)EventName.guess, guessResult);
                    break;
                case (ushort)0x0201:
                    Debug.Log("USBCommunication : SettingInformation");
                    var getCardResult = (USBCommunication.SettingInformation)BytesToStuct(dataBytes, typeof(USBCommunication.SettingInformation));
                    UsbEventCenter.Broadcast((EventType)EventName.getHardSetting, getCardResult);
                    break;
                case (ushort)0x0202:
                    Debug.Log("USBCommunication : ReveiveSettingInformation");
                    break;
            }

        }


    }
}
