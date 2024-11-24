using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_MainSetting_Buttons1 : MonoBehaviour
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
        EventCenter.AddListener((EventType)EventName.hold1, () => {setting.SwitchIndex(typeof(Setting_ChangePwd));});
        EventCenter.AddListener((EventType)EventName.hold2, () => {mainSetting.changeIndex(2);});
        EventCenter.AddListener((EventType)EventName.hold3, () => {setting.SwitchIndex(typeof(Setting_MachineSetting));});
        EventCenter.AddListener((EventType)EventName.hold4, () => {setting.SwitchIndex(typeof(Setting_DataClear));});
        EventCenter.AddListener((EventType)EventName.hold5, () => {setting.SwitchIndex(typeof(Setting_CheckUpdate));});
        EventCenter.AddListener((EventType)EventName.bet, (() => {Main.main.SwitchIndex(typeof(Game));}));
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold2);
        EventCenter.RemoveAllListener((EventType)EventName.hold3);
        EventCenter.RemoveAllListener((EventType)EventName.hold4);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }
}
