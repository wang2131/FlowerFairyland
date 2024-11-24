using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_MachineSetting : MonoBehaviour, ISetting
{
    private Setting_MachineSetting_gamesettings _gamesettings;
    private Setting setting;

    private void Awake()
    {
        _gamesettings = GetComponentInChildren<Setting_MachineSetting_gamesettings>();
        setting = GetComponentInParent<Setting>();
    }
    
    private void OnEnable()
    {
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{_gamesettings.chooseIndex(-1);});
        EventCenter.AddListener((EventType)EventName.hold5, ()=>{_gamesettings.chooseIndex(1);});
        EventCenter.AddListener((EventType)EventName.rollCredits, ()=>{_gamesettings.changeValue();});
        EventCenter.AddListener((EventType)EventName.confirm, ()=>{_gamesettings.OnSave();setting.SwitchIndex(typeof(Setting_MainSetting));});
        EventCenter.AddListener((EventType)EventName.bet, ()=>{setting.SwitchIndex(typeof(Setting_MainSetting));});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }
}
