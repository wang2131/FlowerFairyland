using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_MainSetting_Buttons2 : MonoBehaviour
{
    private Setting setting;
    private Setting_MainSetting mainSetting;

    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
        mainSetting = GetComponentInParent<Setting_MainSetting>();
    }

    private void OnEnable()
    {
        EventCenter.AddListener((EventType)EventName.hold1, ()=>{setting.SwitchIndex(typeof(Setting_GameRecord));});
        EventCenter.AddListener((EventType)EventName.hold2, ()=>{setting.SwitchIndex(typeof(Setting_Ledger));});
        EventCenter.AddListener((EventType)EventName.hold3, ()=>{setting.SwitchIndex(typeof(Setting_FunctionSetting));});
        EventCenter.AddListener((EventType)EventName.hold4, ()=>{setting.SwitchIndex(typeof(Setting_KeyTest));});
        EventCenter.AddListener((EventType)EventName.bet, ()=>{mainSetting.changeIndex(1);});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold2);
        EventCenter.RemoveAllListener((EventType)EventName.hold3);
        EventCenter.RemoveAllListener((EventType)EventName.hold4);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }
}
