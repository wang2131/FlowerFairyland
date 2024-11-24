using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

public class USBCommunication
{
    
    


    public void ReceiveData(object obj)
    {

        

    }

    
    //usb发送的包
    [Serializable]
    public struct Package
    {
        public byte usage;//包的用途，普通调用为0
        public byte index;//表示分包序号从1开始，最高位表示是否为最后一帧，1为还没结束，0表示结束
        public byte effLength;//有效数据的长度
        public byte Id;//请求回复ID，每次递增，到0xff从0重新计数
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 60)]
        public byte[] data;//数据
    }
    
    //usb通讯有效数据结构
    [Serializable]
    public struct DataDesc
    {
        public ushort usType;//包类型，默认为0
        public ushort bVersionNo; //版本号。	提高兼容性
        public ushort usLength;  //长度:本结构的大小：包括定义的头信息和对应数据
        public ushort bCmd;//处理命令ID：用来识别每一个功能。收发用同一条命令ID。
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
        public byte[] arrData;  //字节数组：后面定义的需要发送或接收的	数据结构。
        
    }

    //奖项定义
    public const int ATT_BONUS_10_5K_WUZHANG = 10;      //5条
    public const int ATT_BONUS_9_RS_TONGHUADASHUN = 9;      //同花大顺
    public const int ATT_BONUS_8_SF_TONGHUASHUN = 8;    //同花顺
    public const int ATT_BONUS_7_4K_SITIAO = 7;     //四条
    public const int ATT_BONUS_6_FH_HULU = 6;       //葫芦
    public const int ATT_BONUS_5_FL_TONGHUA = 5;        //同花
    public const int ATT_BONUS_4_ST_SHUNZI = 4;     //顺子
    public const int ATT_BONUS_3_3K_SANZHANG = 3;       //3条
    public const int ATT_BONUS_2_2P_LIANGDUI = 2;       //2对
    public const int ATT_BONUS_1_1P_YIDUI = 1;  //1对
    public const int ATT_BONUS_NULL = 0;        //无奖

    //用一个字节表一张牌。 
    //高4bits花色。（分别为0，1，2，3，分别表示黑红梅方)。
    //低4bit为大小（1-13)
    //例如，黑桃10，则为0x0a;方块10为：0x3a;
    //0x3E为小鬼，0x3F为大鬼

    public const int CMD_GET_FIRST_CARD_RESULT = 10;
    
    [Serializable]
    public struct FirstCardResult//size = 11
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] arrCard;      //数组大小为5，对应的五张牌：参考1.1.1的定义
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] arrHoldCard; //数组大小为同上，对每一个出牌牌是否该留牌，有留牌为1，无则0
        public byte cCurAwardResult;
        
        //深拷贝
        // public void Copy(FirstCardResult obj)
        // {
        //     for (int i = 0; i < this.arrCard.Length; i++)
        //     {
        //         this.arrCard[i] = obj.arrCard[i];
        //         this.arrHoldCard[i] = obj.arrHoldCard[i];
        //         this.cCurAwardResult = obj.cCurAwardResult;
        //     }
        // }
    }

    public const int CMD_GET_SECOND_CARD_RESULT = 15;
    public struct RequestSecondOpenCard
    {
        public byte[] arrHoldCard;//数组大小为5，指示有哪些留牌，有留牌为1，无则0
    }

    //加密卡返回：
    [Serializable]
    public struct SecondCardResult//size = 6
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[] arrCard;  //数组大小为5，换牌之后的五张牌：参考1.1.1的定义。
        public byte bAwardItem;     //奖项ID。
        
        // //深拷贝
        // public void Copy(SecondCardResult obj)
        // {
        //     for (int i = 0; i < arrCard.Length; i++)
        //     {
        //         arrCard[i] = obj.arrCard[i];
        //     }
        //
        //     bAwardItem = obj.bAwardItem;
        // }
    }

    [Serializable]
    public struct DoubleUpResult
    {
        public byte bPlayerChoice;//玩家押的大小
        public byte bResult;//加密卡结果 
        public byte bCard;//返回的牌
    }

    [Serializable]
    public struct DoubleUpRequest
    {
        public byte bPhaseDoubleup; //第几次比倍
        public byte bPlayerChoice; //押大小
    }

    [Serializable]
    public struct SettingInformation
    {
        public byte bZongNandu; //总难度
        public byte b4KNandu; //四同难度
        public byte bxiaoshunNandu; //小顺难度
        public byte bDashunNandu; //大顺难度
        public byte b5KNandu; //五同难度
        public byte bBibeiNandu; //比倍难度
    }
    
    public object BytesToStuct(byte[] bytes, Type type)
    {

        int size = Marshal.SizeOf(type);

        if (size > bytes.Length)
        {

            return null;
        }

        IntPtr structPtr = Marshal.AllocHGlobal(size);

        Marshal.Copy(bytes, 0, structPtr, size);

        object obj = Marshal.PtrToStructure(structPtr, type);

        Marshal.FreeHGlobal(structPtr);

        return obj;
    }

    public static void CopyFirstCardResult(ref FirstCardResult orign, ref FirstCardResult target)
    {
        for (int i = 0; i < 5; i++)
        {
            target.arrCard[i] = orign.arrCard[i];
            target.arrHoldCard[i] = orign.arrHoldCard[i];
        }

        target.cCurAwardResult = orign.cCurAwardResult;
    }

    public static void CopySecondCardResult(ref SecondCardResult orign, ref SecondCardResult target)
    {
        for (int i = 0; i < 5; i++)
        {
            target.arrCard[i] = orign.arrCard[i];
        }

        target.bAwardItem = orign.bAwardItem;
    }
    
}
