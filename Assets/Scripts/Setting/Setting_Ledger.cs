using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;


public class Setting_Ledger : MonoBehaviour
{
    private Setting setting;
    public Text[] values;

    private void Awake()
    {
        values = transform.Find("values").GetComponentsInChildren<Text>();
        setting = GetComponentInParent<Setting>();
    }

    public void ShowValues()
    {
        values[0].text = Save.gameData.bk.totalAddCredits.ToString();
        values[1].text = Save.gameData.bk.totalReduceCredits.ToString();
        values[2].text = Save.gameData.bk.totalWin.ToString();
        values[3].text = Save.gameData.bk.totalBet.ToString();
        values[4].text = Save.gameData.bk.totalCredits.ToString();
        values[5].text = Save.gameData.bk.totalGuessBet.ToString();
        values[6].text = Save.gameData.bk.totalGuessWin.ToString();
        values[7].text = Save.gameData.bk.totalBonus.ToString();
        values[8].text = Save.gameData.bk.totalTickets.ToString();
        values[9].text = Save.gameData.bk.prize7Count.ToString();
        values[10].text = Save.gameData.bk.prize8Count.ToString();
        values[11].text = Save.gameData.bk.prize9Count.ToString();
        values[12].text = Save.gameData.bk.prize10Count.ToString();
    }

    private void OnEnable()
    {
        ShowValues();
        
        EventCenter.AddListener((EventType)EventName.bet, (() => { setting.SwitchIndex(typeof(Setting_MainSetting)); }));
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }
}
