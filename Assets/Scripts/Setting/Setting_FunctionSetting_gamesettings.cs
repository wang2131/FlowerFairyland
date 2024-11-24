using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Setting_FunctionSetting_gamesettings : MonoBehaviour
{
    public Color selectedColor;
    public Color unselectedColor;
    
    
    private Image[] options;
    private Text[] values;
    
    private int currentIndex = 0;
    
    
    private int creditsPerCoin;  //1币几分
    private int coinsPerTicket;  //1票几币
    private int minEnergyCondition;              //最小能量条件
    private int maxEnergyCondition;              //最大能量条件
    private int maxWin;          //设备电频
    private int difficult;				//设备音量
    private int minBet;              //最小押能量
    private int maxBet;              //最大押能量
    private int maxCoinIn;           //最大投币数
    private int gameMode;        //娱乐模式

    private void Awake()
    {
        options = GetComponentsInChildren<Image>();
        values = new Text[options.Length];
        for (int i = 0; i < options.Length; i++)
        {
            values[i] = options[i].gameObject.transform.Find("value").GetComponent<Text>();
        }
    }

    /// <summary>
    /// 切换选项
    /// </summary>
    /// <param name="value">1向下，-1向上</param>
    public void chooseIndex(int value)
    {
        if (value == 1)
        {
            if (currentIndex < options.Length - 1)
            {
                pri_chooseIndex(currentIndex + 1);
            }
        }
        else if (value == -1)
        {
            if (currentIndex > 0)
            {
                pri_chooseIndex(currentIndex - 1);
            }
        }

    }

    private void pri_chooseIndex(int value)
    {
        options[currentIndex].color = unselectedColor;
        currentIndex = value;
        options[currentIndex].color = selectedColor;
    }

    /// <summary>
    /// 切换当前选项的值
    /// </summary>
    public void changeValue()
    {
        switch (currentIndex)
        {
            case 0:
                creditsPerCoin = (creditsPerCoin + 1) % Save.creditsPerCoin_tbl.Length;
                values[0].text = Save.creditsPerCoin_tbl[creditsPerCoin].ToString();
                break;
            case 1:
                coinsPerTicket = (coinsPerTicket + 1) % Save.coinsPerTicket_tbl.Length;
                values[1].text = Save.coinsPerTicket_tbl[coinsPerTicket].ToString();
                break;
            case 2:
                minEnergyCondition = (minEnergyCondition + 1) % Save.minEnergyCondition_tbl.Length;
                values[2].text = Save.minEnergyCondition_tbl[minEnergyCondition].ToString();
                break;
            case 3:
                maxEnergyCondition = (maxEnergyCondition + 1) % Save.maxEnergyCondition_tbl.Length;
                values[3].text = Save.maxEnergyCondition_tbl[maxEnergyCondition].ToString();
                break;
            case 4:
                maxWin = (maxWin + 1) % Save.maxWin_tbl.Length;
                values[4].text = Save.maxWin_tbl[maxWin].ToString();
                break;
            case 5:
                difficult = (difficult + 1) % Save.difficult_tbl.Length;
                values[5].text = Save.difficult_tbl[difficult].ToString();
                break;
            case 6:
                minBet = (minBet + 1) % Save.minBet_tbl.Length;
                values[6].text = Save.minBet_tbl[minBet].ToString();
                break;
            case 7:
                maxBet = (maxBet + 1) % Save.maxBet_tbl.Length;
                values[7].text = Save.maxBet_tbl[maxBet].ToString();
                break;
            case 8:
                maxCoinIn = (maxCoinIn + 1) % Save.maxCoinIn_tbl.Length;
                values[8].text = Save.maxCoinIn_tbl[maxCoinIn].ToString();
                break;
            case 9:
                gameMode = (gameMode + 1) % Save.gameMode_tbl.Length;
                values[9].text = Save.gameMode_tbl[gameMode].ToString();
                break;
        }
    }

    public void OnSave()
    {
        Save.gameData.gs.creditsPerCoin = creditsPerCoin;
        Save.gameData.gs.coinsPerTicket = coinsPerTicket;
        Save.gameData.gs.minEnergyCondition = minEnergyCondition;
        Save.gameData.gs.maxEnergyCondition = maxEnergyCondition;
        Save.gameData.gs.maxWin = maxWin;
        Save.gameData.gs.difficult = difficult;
        Save.gameData.gs.minBet = minBet;
        Save.gameData.gs.maxBet = maxBet;
        Save.gameData.gs.maxCoinIn = maxCoinIn;
        Save.gameData.gs.gameMode = gameMode;
        Save.SetGameData(Save.gameData);
    }

    private void OnEnable()
    {
        creditsPerCoin = Save.gameData.gs.creditsPerCoin;
        values[0].text = Save.creditsPerCoin_tbl[creditsPerCoin].ToString();
        coinsPerTicket = Save.gameData.gs.coinsPerTicket;
        values[1].text = Save.coinsPerTicket_tbl[coinsPerTicket].ToString();
        minEnergyCondition = Save.gameData.gs.minEnergyCondition;
        values[2].text = Save.minEnergyCondition_tbl[minEnergyCondition].ToString();
        maxEnergyCondition = Save.gameData.gs.maxEnergyCondition;
        values[3].text = Save.maxEnergyCondition_tbl[maxEnergyCondition].ToString();
        maxWin = Save.gameData.gs.maxWin;
        values[4].text = Save.maxWin_tbl[maxWin].ToString();
        difficult = Save.gameData.gs.difficult;
        values[5].text = Save.difficult_tbl[difficult].ToString();
        minBet = Save.gameData.gs.minBet;
        values[6].text = Save.minBet_tbl[minBet].ToString();
        maxBet = Save.gameData.gs.maxBet;
        values[7].text = Save.maxBet_tbl[maxBet].ToString();
        maxCoinIn = Save.gameData.gs.maxCoinIn;
        values[8].text = Save.maxCoinIn_tbl[maxCoinIn].ToString();
        gameMode = Save.gameData.gs.gameMode;
        values[9].text = Save.gameMode_tbl[gameMode].ToString();
        
        pri_chooseIndex(0);
    }

    private void OnDisable()
    {
        
    }
}
