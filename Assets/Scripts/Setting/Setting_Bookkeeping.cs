using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_Bookkeeping : MonoBehaviour, ISetting
{
    private Setting setting;

    private void Awake()
    {
        setting = GetComponentInParent<Setting>();
    }

    private void OnEnable()
    {
        EventCenter.AddListener((EventType)EventName.bet, ()=>{setting.SwitchIndex(typeof(Setting_MainSetting));});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }
}
