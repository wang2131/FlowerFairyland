﻿//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO.Ports;
//using System.Threading;
//using GlobalNamaspaces;
//using UnityEngine;
//using UnityEngine.UI;
////using Random = UnityEngine.Random;

////挂在某个GameObject上，修改接收到事件后，
////发送给ObjectListener即可在内部定义接收事件
//public class UnitySerialPort1 : MonoBehaviour
//{
//    public string portName = "COM6"; //串口名
//    public int baudRate = 115200; //波特率
//    public Parity parity = Parity.None; //效验位
//    public int dataBits = 8; //数据位
//    public StopBits stopBits = StopBits.One; //停止位
//    private static SerialPort sp = null;
//    private Thread dataReceiveThread;
//    private Thread dataRequestThread;
//    private int maxUnreadMessages = 4096; //最大队列值
//    private Queue inputQueue;
//    private bool isWorking = false;
//    private bool isReceivedData = false;
//    private int lastTimeoutReceivedTime = 0;
//    private int lastReceivedTime = 0;
//    private bool isFirstRecv = true;
//    private bool[] isKeyDown = new bool[15];
//    private bool[] isKeyUp = new bool[15];
//    private List<byte> lastRevBuffList = new List<byte>();
//    private bool isHoppering = false;

//    private ushort TuibiCount = 0;
//    private ushort ToubiCount = 0;

//    //Tim add for Debug
//    private string recvMsg = "";
//    private string sendMsg = "";
//    private Text recvText;
//    private Text sendText;
//    private Text errText;
//    private float debugTime = 0;
//    private byte hopperPressCount = 0;
//    public Game game;


//    private int remainHopperCount = 0;

//    private float startHopperTime;

//    //投退币信息
//    public ushort toubijishu; //投币计数
//    public ushort tuibijishu; //退币计数

//    void Awake()
//    {


//        Debug.Log("protName:" + portName);

//        if (OpenPort())
//        {
//            //inputQueue = Queue.Synchronized(new Queue());
//            inputQueue = new Queue();
//            isWorking = true;
//            dataReceiveThread = new Thread(new ThreadStart(DataReceiveFunction));
//            dataReceiveThread.Start();
//            debugTime = Time.time;
//            lastTimeoutReceivedTime = (int)(Time.time * 1000);
//            DontDestroyOnLoad(gameObject);
//        }
//    }

//    public bool OpenPort()
//    {
//        sp = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
//        //sp.ReadTimeout = 2000;
//        try
//        {
//            sp.Open();
//            Debug.Log("串口打开成功！" + portName);
//            return true;
//        }
//        catch (Exception ex)
//        {
//            Debug.Log("串口打开失败！" + portName + "  " + ex.ToString());
//            return false;
//        }
//    }

//    public void ClosePort()
//    {
//        try
//        {
//            sp.Close();
//            isWorking = false;
//            dataReceiveThread.Abort();
//            sp = null;
//            dataReceiveThread = null;
//        }
//        catch (Exception ex)
//        {
//            Debug.Log("ClosePort: " + ex.Message);
//        }
//    }

//    void Update()
//    {
//        HandleQuene();
//        if (isHoppering)
//        {
//            if (remainHopperCount <= 0) ;
//            isHoppering = false;
//        }

//        if (isHoppering && Time.time - startHopperTime > 5f)
//        {
//            Debug.Log("退票错误");
//            isHoppering = false;
//        }

//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            SendHopper();
//        }
//    }



//    void HandleQuene()
//    {
//        if (inputQueue.Count == 0)
//            return;

//        List<byte> buffer = (List<byte>)inputQueue.Dequeue();

//        //校验
//        if (!CheckCRC(buffer))
//        {
//            Debug.Log("校验没通过...");
//            return;
//        }


//        byte key1 = buffer[4];
//        byte key2 = buffer[5];
//        byte key3 = buffer[6];
//        byte TouTuiInit = buffer[7]; //投退币计数初始化值，此值少于3主机要重新初始化投退币计数变量
//        byte TouBiNumHigh = buffer[8]; //投币计数，每投一个币此值加1
//        byte TouBiNumLow = buffer[9];
//        byte TuiBiNumHigh = buffer[10]; //退币计数，每退一个币此值加1
//        byte TuiBiNumLow = buffer[11];

//        toubijishu = BitConverter.ToUInt16(new byte[] { TouBiNumLow, TouBiNumHigh }, 0);
//        tuibijishu = BitConverter.ToUInt16(new byte[] { TuiBiNumLow, TuiBiNumHigh }, 0);
//        if (tuibijishu > TuibiCount && isHoppering)
//        {
//            //******临时测试********
//            int tuibi = tuibijishu - TuibiCount;
//            Debug.Log("退币：" + tuibi);
//            //**********************

//            startHopperTime = Time.time;
//            remainHopperCount -= (tuibijishu - TuibiCount);

//            TuibiCount = tuibijishu;
//            Debug.Log("TouTuiInit : " + TouTuiInit);
//            Debug.Log("投币计数：" + toubijishu);
//            Debug.Log("退币计数：" + tuibijishu);
//            //Debug.Log("总分：" + Save.gameData.bk.totalCredits);
//        }

//        if (toubijishu > ToubiCount && isHoppering)
//        {
//            int toubi = toubijishu - ToubiCount;
//            Debug.Log("投币：" + toubi);

//            ToubiCount = toubijishu;
//        }

//        //key message
//        for (int i = 0; i < 8; i++)
//        {
//            if ((key1 & 0x1) == 1)
//            {
//                isKeyDown[i] = true;

//            }
//            else
//            {
//                isKeyUp[i] = false;
//                //真正的事件
//                if (isKeyDown[i])
//                {
//                    isKeyDown[i] = false;
//                    isKeyUp[i] = true;
//                }
//            }

//            key1 = (byte)(key1 >> 1);
//        }

//        for (int i = 0; i < 7; i++)
//        {
//            if ((key2 & 0x1) == 1)
//            {
//                isKeyDown[i + 8] = true;
//            }
//            else
//            {
//                isKeyUp[i + 8] = false;
//                if (isKeyDown[i + 8] == true)
//                {
//                    isKeyDown[i + 8] = false;
//                    isKeyUp[i + 8] = true;
//                }
//            }

//            key2 = (byte)(key2 >> 1);
//        }

//        for (int i = 0; i < 15; i++)
//        {
//            if (isKeyUp[i] && !isHoppering)
//            {
//                //     switch (i)
//                //     {
//                //         case 0:
//                //             EventCenter.Broadcast((EventType)EventName.hold1);
//                //             MyDebugger.Log("hold1");
//                //             break;
//                //         case 1:
//                //             EventCenter.Broadcast((EventType)EventName.hold2);
//                //             MyDebugger.Log("hold2");
//                //             break;
//                //         case 2:
//                //             EventCenter.Broadcast((EventType)EventName.hold3);
//                //             MyDebugger.Log("hold3");
//                //             break;
//                //         case 3:
//                //             EventCenter.Broadcast((EventType)EventName.hold4);
//                //             MyDebugger.Log("hold4");
//                //             break;
//                //         case 4:
//                //             EventCenter.Broadcast((EventType)EventName.hold5);
//                //             MyDebugger.Log("hold5");
//                //             break;
//                //         case 5:
//                //             EventCenter.Broadcast((EventType)EventName.bet);
//                //             MyDebugger.Log("bet");
//                //             break;
//                //         case 6:
//                //             EventCenter.Broadcast((EventType)EventName.hopperOutKey);
//                //             Debug.Log("hopperOutKey");
//                //             MyDebugger.Log("hopperOutKey");
//                //             break;
//                //         case 7:
//                //             EventCenter.Broadcast((EventType)EventName.guess);
//                //             MyDebugger.Log("guess");
//                //             break;
//                //         case 8:
//                //             EventCenter.Broadcast((EventType)EventName.big);
//                //             MyDebugger.Log("big");
//                //             break;
//                //         case 9:
//                //             EventCenter.Broadcast((EventType)EventName.small);
//                //             MyDebugger.Log("small");
//                //             break;
//                //         case 10:
//                //             EventCenter.Broadcast((EventType)EventName.rollCredits);
//                //             MyDebugger.Log("rollCredits");
//                //             break;
//                //         case 11:
//                //             EventCenter.Broadcast((EventType)EventName.confirm);
//                //             MyDebugger.Log("confirm");
//                //             break;
//                //         case 12:
//                //             EventCenter.Broadcast((EventType)EventName.addCredits);
//                //             MyDebugger.Log("addCredits");
//                //             break;
//                //         case 13:
//                //             EventCenter.Broadcast((EventType)EventName.reduceCredits);
//                //             MyDebugger.Log("reduceCredits");
//                //             break;
//                //         case 14:
//                //             EventCenter.Broadcast((EventType)EventName.gameSetting);
//                //             MyDebugger.Log("gameSetting");
//                //             break;
//                //     }
//                // }
//            }


//        }
//    }

//    void DataReceiveFunction()
//        {
//            int bytes = 0;
//            int i = 0;
//            int dataLen = 0;
//            int count = 0;

//            while (isWorking)
//            {
//                Thread.Sleep(2);

//                if (sp != null && sp.IsOpen)
//                {
//                    try
//                    {
//                        if (sp.BytesToRead > 0)
//                        {
//                            byte[] readBuffer = new byte[12];
//                            List<byte> revBuffList = new List<byte>();
//                            //##read package header
//                            bytes = sp.Read(readBuffer, 0, 1);
//                            if (readBuffer[0] != 0xbb)
//                                continue;
//                            bytes = sp.Read(readBuffer, 0, 1);
//                            if (readBuffer[0] != 0xaa)
//                                continue;
//                            //##read datalen
//                            dataLen = 9;
//                            revBuffList.Add(0xbb);
//                            revBuffList.Add(0xaa);
//                            //start to recv all data
//                            count = 0;
//                            do
//                            {
//                                bytes = sp.Read(readBuffer, 0, dataLen + 1);
//                                count += bytes;
//                                for (i = 0; i < bytes; i++)
//                                {
//                                    revBuffList.Add(readBuffer[i]);
//                                }
//                            } while (count < dataLen + 1);

//                            bool isSame = true;
//                            isReceivedData = true;
//                            if (inputQueue.Count < maxUnreadMessages)
//                            {
//                                for (i = 0; i < lastRevBuffList.Count; i++)
//                                {
//                                    if (lastRevBuffList[i] != revBuffList[i])
//                                    {
//                                        //Debug.Log("数据不同");
//                                        isSame = false;
//                                        break;
//                                    }
//                                }

//                                if (!isSame || isFirstRecv)
//                                {
//                                    isFirstRecv = false;
//                                    inputQueue.Enqueue(revBuffList);

//                                    recvMsg = "";
//                                    lastRevBuffList.Clear();
//                                    for (i = 0; i < revBuffList.Count; i++)
//                                    {
//                                        recvMsg += revBuffList[i].ToString("X2");
//                                        lastRevBuffList.Add(revBuffList[i]);
//                                    }

//                                    //Debug.Log("数据不同，进队列");
//                                }

//                                //Debug.Log("###recvMsg: " + recvMsg);
//                            }
//                            else
//                            {
//                                Debug.Log("Queue满了...");
//                            }
//                        }

//                        //查询发送
//                        if (!isReceivedData)
//                        {
//                            //Debug.LogFormat("(int)(Time.time * 1000): {0},{1},{2}" , (int)(Time.time * 1000), lastTimeoutReceivedTime, (int)(Time.time * 1000)- lastTimeoutReceivedTime);
//                            if ((int)(Time.time * 1000) - lastTimeoutReceivedTime >= 500)
//                            {
//                                lastTimeoutReceivedTime = (int)(Time.time * 1000);
//                                MainBoardSend(false, 0);
//                                Debug.Log("超过200ms没收到，重新发送。。。。");
//                            }
//                        }
//                        else
//                        {
//                            if ((int)(Time.time * 1000) - lastReceivedTime >= 50)
//                            {
//                                if (!isHoppering)
//                                {
//                                    lastReceivedTime = (int)(Time.time * 1000);
//                                    MainBoardSend(false, 0);
//                                    isReceivedData = false;
//                                    lastTimeoutReceivedTime = (int)(Time.time * 1000);
//                                    //Debug.Log("收到发送。。。。");
//                                }

//                            }
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        if (ex.GetType() != typeof(ThreadAbortException))
//                        {
//                            Debug.Log("Unity串口线程出错了: " + ex.ToString());
//                        }

//                        Debug.Log("Unity串口线程出错了2: " + ex.ToString());
//                    }
//                }
//            }

//            Debug.Log("退出串口接收线程");
//        }

//        public void WriteData(string dataStr)
//        {
//            if (sp.IsOpen)
//            {
//                sp.Write(dataStr);
//            }
//        }

//        public static void WriteData(byte[] databyte)
//        {
//            if (sp.IsOpen)
//            {
//                sp.Write(databyte, 0, databyte.Length);
//            }
//        }

//        void OnApplicationQuit()
//        {
//            Debug.Log("Close Port");
//            ClosePort();
//        }

//        bool CheckCRC(List<byte> RecvBuf)
//        {
//            byte checksum = 0;
//            byte temp = 0;

//            for (temp = 0 + 2; temp < 9 + 2; temp++)
//            {
//                checksum += RecvBuf[temp];
//            }

//            checksum ^= 0x20;
//            checksum += 0x24;
//            if (checksum == RecvBuf[9 + 2])
//            {
//                for (temp = 1 + 2; temp < 9 + 2; temp++)
//                {
//                    RecvBuf[temp] ^= RecvBuf[0 + 2];
//                }

//                //Debug.LogFormat("checksum = {0:X},  RecvBuf={1}", checksum, BitConverter.ToString(RecvBuf));
//                return true;
//            }

//            Debug.Log("校验没通过:checksum=" + checksum + "   收到的=" + RecvBuf[11]);
//            return false;
//        }

//        public static void SendCheckInfo()
//        {
//            byte bcc = 0x34;
//            byte[] va = new byte[]
//            {
//                0x87, 0x08, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
//                0xff, 0x34
//            };
//            WriteData(va);
//        }

//        //停止退币
//        public static void SendStopHopper()
//        {
//            byte bcc = 0x34;
//            byte[] va = new byte[]
//            {
//                0x87, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
//                0x00, 0x34
//            };

//            va[2] = 0;
//            WriteData(va);
//        }

//        int SendMaxCnt = 0;
//        byte[] SendBuf = new byte[12];

//        public void MainBoardSend(bool isLanchHopperout, short totalHopperCredits)
//        {
//            SendMaxCnt = 0;
//            SendBuf[SendMaxCnt++] = 0xAA;
//            SendBuf[SendMaxCnt++] = 0xBB;
//            SendBuf[SendMaxCnt++] = 0x1e;
//            SendBuf[SendMaxCnt++] = 0x00;
//            SendBuf[SendMaxCnt++] = 0x00;
//            SendBuf[SendMaxCnt++] = 0x00;
//            if (isLanchHopperout)
//            {
//                SendBuf[SendMaxCnt++] = 0x01;
//                remainHopperCount = totalHopperCredits;
//                isHoppering = true;
//                hopperPressCount++;
//            }
//            ////开启退币的条件：Flag的0位为1，TuiBiCnt值发生变化，（(TuiBiNumHigh<<8)+TuiBiNumLow）的值大于0
//            else
//                SendBuf[SendMaxCnt++] = 0x00;


//            SendBuf[SendMaxCnt++] = hopperPressCount;
//            SendBuf[SendMaxCnt++] = (byte)(totalHopperCredits >> 8);
//            SendBuf[SendMaxCnt++] = (byte)(totalHopperCredits);
//            SendBuf[SendMaxCnt++] = 0x00;
//            //SendBuf[SendMaxCnt++] = 0x00;
//            if (isLanchHopperout)
//            {
//                Debug.Log("cnt:" + SendBuf[7]);
//                Debug.Log("flag" + SendBuf[6]);
//                Debug.Log("ticket:" + totalHopperCredits);
//            }

//            int temp = 0;
//            for (temp = 3; temp < SendMaxCnt; temp++)
//            {
//                SendBuf[temp] ^= SendBuf[2];
//            }

//            byte checksum = 0;
//            for (temp = 2; temp < SendMaxCnt; temp++)
//            {
//                checksum += SendBuf[temp];
//            }

//            checksum ^= 0x20;
//            checksum += 0x24;

//            SendBuf[SendMaxCnt++] = checksum;
//            WriteData(SendBuf);
//            if (isLanchHopperout)
//            {
//                startHopperTime = Time.time;
//            }
//            //Debug.LogFormat("checksum = {0:X}, SendMaxCntLen={1}, Send={2}", checksum, SendMaxCnt, BitConverter.ToString(SendBuf));
//        }

//        void McuRecvied(byte[] RecvBuf)
//        {
//            byte checksum = 0;
//            byte temp = 0;

//            for (temp = 0 + 2; temp < 9 + 2; temp++)
//            {
//                checksum += RecvBuf[temp];
//            }

//            checksum ^= 0x20;
//            checksum += 0x24;
//            if (checksum == RecvBuf[9 + 2])
//            {
//                for (temp = 1 + 2; temp < 9 + 2; temp++)
//                {
//                    RecvBuf[temp] ^= RecvBuf[0 + 2];
//                }
//            }

//            Debug.LogFormat("checksum = {0:X},  RecvBuf={1}", checksum, BitConverter.ToString(RecvBuf));
//        }

//        public void SendHopper()
//        {
//            MainBoardSend(true, 20);
//        }
//    }