using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static USBCommunication;

namespace GlobalNamaspaces
{
	[Serializable]
    public class Save
	{
		public static string git_version = "v1.10";
		//保存数据
		public static GameData gameData;
		private static string strFile = Application.persistentDataPath + "/game.bin";


		//整个游戏的全局变量
		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct GameData
		{
			public StationData sd;//前台数据，开机从后台加载
			public Bookkeeping bk;	//游戏帐目
			public GameSetting gs; //游戏设置
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
			public GameRecord[] gr; //游戏记录

			public short Lasttuibishu;
			public short Lasttoubishu;
		}


		//前台界面显示的所有变量
		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct StationData
		{
			public int cloudEnergy;//云能量
			public int energyCondition;//能量条件
			public int bet;//能量
			public int win; //获得成果
			public int credits;//总成果


			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			//public byte[] arrFirstCard;      //数组大小为5，对应的五张牌
			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			//public byte[] arrHoldCard;      //数组大小为同上，对每一个出牌牌是否该留牌，有留牌为1，无则0
			public FirstCardResult firstCardResult;


			//[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			//public byte[] arrSecondCard;  //数组大小为5，换牌之后的五张牌
			//public byte bAwardItem;     //奖项ID  0-10,0为无奖，10为5条
			public SecondCardResult secondCardResult;


			public int prize1;
			public int prize2;
			public int prize3;
			public int prize4;
			public int prize5;
			public int prize6;
			public int prize7;
			public int prize8;
			public int prize9;
			public int prize10;
		}

		//游戏帐目
		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct Bookkeeping
		{
			public int totalAddCredits;//总上能量
			public int totalReduceCredits;//总下能量
			public int totalWin;//总得能量
			public int totalBet; //总押能量
			public int totalCredits; //总成果
			public int totalGuessBet; //天气总押
			public int totalGuessWin; //天气成果
			public int totalBonus; //额外奖励
			public int totalTickets;//退彩票

			public int Caijin_7;
			public int Caijin_8;
			public int Caijin_9;
			public int Caijin_10;
			public int totalCaijin;
			
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
			public byte[] guessCards;
			public int prize10Count;
			public int prize9Count;
			public int prize8Count;
			public int prize7Count; 
			
		}

		//游戏记录
		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct GameRecord
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			public byte[] arrFirstCard;      //数组大小为5，对应的五张牌
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			public byte[] arrHoldCard;      //数组大小为同上，对每一个出牌牌是否该留牌，有留牌为1，无则0
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
			public byte[] arrSecondCard;  //数组大小为5，换牌之后的五张牌
			public byte bAwardItem;     //奖项ID  0-10,0为无奖，10为5条
			public int bet;
			public int win;
			
		}
		
		

		//游戏设置
		[Serializable]
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct GameSetting
		{
			public string pwd; //后台密码
			public string deviceId;//设备唯一id序列号
			public string sourceCleanCode;//清零码报帐码
			public int creditsPerCoin;  //1币几分
			public int coinsPerTicket;  //1票几币
			public int minEnergyCondition;              //最小能量条件
			public int maxEnergyCondition;              //最大能量条件
			public int maxWin;          //设备电频
			public int difficult;				//设备音量
			public int minBet;              //最小押能量
			public int maxBet;              //最大押能量
			public int maxCoinIn;           //最大投币数
			public int gameMode;        //娱乐模式
			public int stationId;           //机台号
			public int volumn;              //音量
			public int openCloudBonus; //开启云能量=1为开启
		}

		
#region _table

		//按顺序往下添加，根据视频
		public static int[] creditsPerCoin_tbl = new int[]
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90,
			100, 200, 300, 400, 500, 600, 700, 800, 900, 1000
		};//10

		public static int[] coinsPerTicket_tbl = new int[]
			{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };//1

		public static int[] minEnergyCondition_tbl = new int[]
		{
			100, 200,300, 400, 500, 600, 700, 800, 900, 1000, 2000, 3000,
			4000, 5000, 6000, 7000, 8000, 9000, 10000, 20000, 30000, 40000,
			50000, 60000, 70000, 80000, 90000, 100000, 110000, 120000, 130000,
			140000, 150000, 160000, 170000, 180000, 190000, 20000, 210000, 230000,
			240000, 250000, 260000, 270000, 280000, 290000, 300000, 310000, 320000,
			330000, 340000, 350000, 360000, 370000, 380000, 390000, 400000, 410000,
			420000, 430000, 440000, 450000, 460000, 470000, 480000, 490000, 500000
		};//6000

		public static int[] maxEnergyCondition_tbl = new int[]
		{
			10000, 20000, 30000, 40000, 50000, 60000, 70000, 80000, 90000,
			100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000, 900000,1000000, 1500000, 2000000, 2500000, 3000000
		};

		public static int[] maxWin_tbl = new int[]
		{
			300000, 400000, 500000, 600000, 700000,
			800000, 900000, 1000000, 1100000, 1200000
		};

		public static int[] difficult_tbl = new int[]
		{
			1, 2, 3, 4, 5
		};

		public static int[] minBet_tbl = new int[]
		{
			1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 150, 200, 250, 300, 350, 400, 450, 500,
			600, 700, 800, 900, 1000
		};

		public static int[] maxBet_tbl = new[]
		{
			100, 150,200,250, 300,350, 400,450, 500, 600, 700, 800, 900, 1000, 1100,
			1200, 1300, 1400, 1500, 1600, 1700, 1800, 1900, 2000,
			2100, 2200, 2300, 2400, 2500, 2600, 2700, 2800, 2900,
			3000, 3100, 3200, 3300, 3400, 3500, 3600, 3700, 3800,
			3900, 4000, 4100, 4200, 4300, 4400, 4500, 4600, 4700,
			4800, 4900, 5000
		};

		public static int[] maxCoinIn_tbl = new int[]
		{
			100000, 200000, 300000, 400000, 500000, 600000, 700000, 800000, 900000,
			1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 1600000, 1700000,
			1800000, 1900000, 2000000
		};

		public static char[] gameMode_tbl = new char[]
		{
			'A', 'B'
		};

		public static int[] stationId_tbl = new int[]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24
		};

		public static int[] volumn_tbl = new int[]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
		};

		public static char[] openCloudBonus_tbl = new char[]
		{
			'否', '是'
		};

		public static readonly string[] error_strings = new string[]
		{
			string.Empty,
			"水银电损坏",
			"水银电数据错误",
			string.Empty,
			string.Empty,
			string.Empty,
			"主机连接错误"//16
		};
#endregion

		public static void InitData()
		{
			gameData = new GameData();
			gameData.sd = new StationData();
			gameData.sd.firstCardResult = new FirstCardResult();
            gameData.sd.firstCardResult.arrCard = new byte[5]{0,0,0,0,0};
            gameData.sd.firstCardResult.arrHoldCard = new byte[5]{0,0,0,0,0};

			gameData.sd.secondCardResult = new SecondCardResult();
			gameData.sd.secondCardResult.arrCard = new byte[5]{0,0,0,0,0};
			gameData.sd.secondCardResult.bAwardItem = 0;

			gameData.bk = new Bookkeeping();
			gameData.bk.guessCards = new byte[6]{0x0,0x0,0x0,0x0,0x0,0x0};
			gameData.gs = new GameSetting();
			gameData.gr = new GameRecord[20];
            for (int i = 0; i < gameData.gr.Length; i++)
            {
				gameData.gr[i].arrFirstCard = new byte[5]{0,0,0,0,0};
				gameData.gr[i].arrHoldCard = new byte[5]{0,0,0,0,0};
				gameData.gr[i].arrSecondCard = new byte[5]{0,0,0,0,0};
            }

			GetGameData();
		}

		public static void GetGameData()
		{
			UnityEngine.Debug.Log("数据路径：" + strFile);

			FileStream readstream;
			if (!File.Exists(strFile))
			{
				UnityEngine.Debug.Log("文件不存在,准备创建");
				readstream = new FileStream(strFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
				readstream.Close();
				//reset to default
				Reset2DefaultBookkeeping();
				Reset2DefaultGameSetting(true);
				Reset2DefaultStationData();
				gameData.Lasttuibishu = -1;
				gameData.Lasttoubishu = -1;
				SetGameData(gameData);
			}
			else
			{
				UnityEngine.Debug.Log("文件存在,开始读取");
				readstream = new FileStream(strFile, FileMode.Open, FileAccess.Read, FileShare.None);
				BinaryFormatter formatter = new BinaryFormatter();
				Save.gameData = (GameData)formatter.Deserialize(readstream);
				Debug.Log("读取： " + Save.gameData.bk.totalCredits);
			}
			readstream.Close(); 
		}

		public static void Reset2DefaultBookkeeping()
        {
			gameData.bk.totalAddCredits = 0;
			gameData.bk.totalReduceCredits = 0;
			gameData.bk.totalWin = 0;
			gameData.bk.totalBet = 0;
			gameData.bk.totalCredits = 0;
			gameData.bk.totalGuessBet = 0;
			gameData.bk.totalGuessWin = 0;
			gameData.bk.totalTickets = 0;
			gameData.bk.prize10Count = 0;
			gameData.bk.prize9Count = 0;
			gameData.bk.prize8Count = 0;
			gameData.bk.prize7Count = 0;
			gameData.bk.Caijin_10 = 5000;
			gameData.bk.Caijin_9 = 2000;
			gameData.bk.Caijin_8 = 500;
			gameData.bk.Caijin_7 = 200;
			gameData.bk.totalCaijin = 0;
			Debug.LogFormat(" Reset2DefaultBookkeeping-->totalCredits={0}", Save.gameData.bk.totalCredits);

		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="isPwdNeedClear">后台密码是否需要重置</param>
		public static void Reset2DefaultGameSetting(bool isPwdNeedClear)
		{
			if (isPwdNeedClear)
			{
				gameData.gs.pwd = "00000000";
			}

			gameData.gs.stationId = 1;
			gameData.gs.sourceCleanCode = "0000000";
			gameData.gs.coinsPerTicket = 0;
			gameData.gs.creditsPerCoin = 0;
			gameData.gs.minEnergyCondition = 0;
			gameData.gs.maxEnergyCondition = 0;
			gameData.gs.maxWin = 0;
			gameData.gs.difficult = 0;
			gameData.gs.minBet = 9;
			gameData.gs.maxBet = 0;
			gameData.gs.maxCoinIn = 0;
			gameData.gs.gameMode = 0;
			gameData.gs.volumn = 8;
			gameData.gs.openCloudBonus = 0;
			
		}

		public static void Reset2DefaultStationData()
		{
			gameData.sd.bet = 0;
			gameData.sd.energyCondition = minEnergyCondition_tbl[0];
			gameData.sd.credits = gameData.bk.totalCredits;
			gameData.sd.prize1 = gameData.gs.minBet;
			gameData.sd.prize2 = gameData.gs.minBet * 2;
			gameData.sd.prize3 = gameData.gs.minBet * 3;
			gameData.sd.prize4 = gameData.gs.minBet * 5;
			gameData.sd.prize5 = gameData.gs.minBet * 7;
			gameData.sd.prize6 = gameData.gs.minBet * 10;
			gameData.sd.prize7 = gameData.gs.minBet * 60;
			gameData.sd.prize8 = gameData.gs.minBet * 120;
			gameData.sd.prize9 = gameData.gs.minBet * 250;
			gameData.sd.prize10 = gameData.gs.minBet * 750;
		}

		/// <summary>
		/// 重置所有游戏记录
		/// </summary>
		public static void ResetGameRecord()
		{
			for (int i = 0; i < gameData.gr.Length; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					gameData.gr[i].arrFirstCard[j] = 0x0;
					gameData.gr[i].arrHoldCard[j] = 0x0;
					gameData.gr[i].arrSecondCard[j] = 0x0;
				}
				gameData.gr[i].bAwardItem = 0x0;
				gameData.gr[i].bet = 0x0;
				gameData.gr[i].win = 0x0;
			}
		}

		public static void SaveGameRecord(byte[] arrFirstCard, byte[] arrHoldCard, byte[] arrSecondCard, byte bAwardItem, int Bet, int Win)
		{
			for (int i = 18; i >= 0; i--)
			{
				for (int j = 0; j < 5; j++)
				{
					gameData.gr[i + 1].arrFirstCard[j] = gameData.gr[i].arrFirstCard[j];
					gameData.gr[i + 1].arrHoldCard[j] = gameData.gr[i].arrHoldCard[j];
					gameData.gr[i + 1].arrSecondCard[j] = gameData.gr[i].arrSecondCard[j];
				}

				gameData.gr[i + 1].bAwardItem = gameData.gr[i].bAwardItem;
				gameData.gr[i + 1].bet = gameData.gr[i].bet;
				gameData.gr[i + 1].win = gameData.gr[i].win;
			}

			for (int j = 0; j < 5; j++)
			{
				gameData.gr[0].arrFirstCard[j] = arrFirstCard[j];
				gameData.gr[0].arrHoldCard[j] = arrHoldCard[j];
				gameData.gr[0].arrSecondCard[j] = arrSecondCard[j];
			}

			gameData.gr[0].bAwardItem = bAwardItem;
			gameData.gr[0].bet = Bet;
			gameData.gr[0].win = Win;
		}

		public static void SetGuessCardRecord(byte value)
		{
			for (int i = Save.gameData.bk.guessCards.Length - 1; i >= 1; i--)
			{
				Save.gameData.bk.guessCards[i] = Save.gameData.bk.guessCards[i - 1];
			}
			
			Save.gameData.bk.guessCards[0] = value;
			Save.SetGameData(Save.gameData);
		}

		public static byte[] CalcPasswordHash(string password)
		{
			if (string.IsNullOrEmpty(password))
			{
				return null;
			}
			SHA256Managed sha256Managed = new SHA256Managed();
			byte[] bytes = Encoding.Default.GetBytes(password);
			sha256Managed.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			byte[] array = new byte[] { 74, 190, 45, 36, 203, 106, 32, 231, 66, 197, 7, 177, 153, 76, 253, 241 };
			sha256Managed.TransformFinalBlock(array, 0, array.Length);
			byte[] hash = sha256Managed.Hash;
			sha256Managed.Initialize();
			sha256Managed.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
			sha256Managed.TransformBlock(hash, 8, 10, hash, 8);
			sha256Managed.TransformFinalBlock(hash, 0, 16);
			hash = sha256Managed.Hash;
			sha256Managed.Initialize();
			sha256Managed.TransformBlock(array, 0, array.Length, array, 0);
			sha256Managed.TransformBlock(hash, 8, 8, array, 0);
			sha256Managed.TransformFinalBlock(array, 0, array.Length);
			Array.Copy(sha256Managed.Hash, 0, hash, 20, 8);
			//dumpbyte(hash, hash.Length, string.Format("PW {0} hash end", password));
			return hash;
		}

		public static void SetGameData(GameData gd)
		{
			FileStream stream = new FileStream(strFile, FileMode.Open, FileAccess.Write, FileShare.None);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, gd);
			stream.Flush(true);
			stream.Close();
			//Debug.Log("数据保存成功");
		}

		public static void ResetDataInSetting()
		{
			Reset2DefaultBookkeeping();
			Reset2DefaultGameSetting(false);
			Reset2DefaultStationData();
		}
		//序列化
		public static byte[] RawSerialize(object obj)
		{
			int rawsize = Marshal.SizeOf(obj);
			IntPtr buffer = Marshal.AllocHGlobal(rawsize);
			Marshal.StructureToPtr(obj, buffer, false);
			byte[] rawdatas = new byte[rawsize];
			Marshal.Copy(buffer, rawdatas, 0, rawsize);
			Marshal.FreeHGlobal(buffer);
			return rawdatas;
		}
		//反序列化
		public static object RawDeserialize(byte[] rawdatas, Type anytype)
		{
			int rawsize = Marshal.SizeOf(anytype);
			if (rawsize > rawdatas.Length) return null;
			IntPtr buffer = Marshal.AllocHGlobal(rawsize);
			Marshal.Copy(rawdatas, 0, buffer, rawsize);
			object retobj = Marshal.PtrToStructure(buffer, anytype);
			Marshal.FreeHGlobal(buffer);
			return retobj;
		}
		
	}
}

