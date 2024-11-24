using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_MainSetting : MonoBehaviour, ISetting
{
    private Setting setting;
    private GameObject Buttons1;
    private GameObject Buttons2;
    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
        Buttons1 = transform.Find("Buttons1").gameObject;
        Buttons2 = transform.Find("Buttons2").gameObject;
    }

    private void OnEnable()
    {
        Buttons2.SetActive(false);
        Buttons1.SetActive(true);
        
        // EventCenter.AddListener((EventType)EventName.hold1, () => {setting.SwitchIndex(typeof(Setting_ChangePwd));});
        // EventCenter.AddListener((EventType)EventName.hold2, () => {setting.SwitchIndex(typeof(Setting_FunctionSetting));});
        // EventCenter.AddListener((EventType)EventName.hold3, () => {setting.SwitchIndex(typeof(Setting_MachineSetting));});
        // EventCenter.AddListener((EventType)EventName.hold4, () => {setting.SwitchIndex(typeof(Setting_DataClear));});
        // EventCenter.AddListener((EventType)EventName.hold5, () => {setting.SwitchIndex(typeof(Setting_CheckUpdate));});
        // EventCenter.AddListener((EventType)EventName.bet, (() => {Main.main.SwitchIndex(typeof(Game));}));
    }

    private void OnDisable()
    {
        // EventCenter.RemoveListener((EventType)EventName.hold1, () => {setting.SwitchIndex(typeof(Setting_ChangePwd));});
        // EventCenter.RemoveListener((EventType)EventName.hold2, () => {setting.SwitchIndex(typeof(Setting_FunctionSetting));});
        // EventCenter.RemoveListener((EventType)EventName.hold3, () => {setting.SwitchIndex(typeof(Setting_MachineSetting));});
        // EventCenter.RemoveListener((EventType)EventName.hold4, () => {setting.SwitchIndex(typeof(Setting_DataClear));});
        // EventCenter.RemoveListener((EventType)EventName.hold5, () => {setting.SwitchIndex(typeof(Setting_CheckUpdate));});
        // EventCenter.RemoveListener((EventType)EventName.bet, (() => {Main.main.SwitchIndex(typeof(Game));}));
    }

    public void changeIndex(int index)
    {
        switch (index)
        {
            case 1:
                Buttons2.SetActive(false);
                Buttons1.SetActive(true);
                break;
            case 2:
                Buttons1.SetActive(false);
                Buttons2.SetActive(true);
                break;
        }
    }
}
