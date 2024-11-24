using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    //main
    public Setting_MainSetting mainSetting;
    //1
    public Setting_ChangePwd changePwd;
    //2
    public Setting_FunctionSetting functionSetting;
    //3
    public Setting_MachineSetting machineSetting;
    //4
    public Setting_DataClear dataClear;
    //5
    public Setting_CheckUpdate checkUpdate;

    public Setting_GameRecord gameRecord;

    public Setting_Ledger ledger;
    
    public Setting_HardSetting hardSetting;

    public Setting_KeyTest keyTest;

    private MonoBehaviour[] settings;

    [SerializeField]
    private Text version_txt;
    private void Awake()
    {
        settings = new MonoBehaviour[10];
        settings[0] = mainSetting;
        settings[1] = functionSetting;
        settings[2] = machineSetting;
        settings[3] = changePwd;
        settings[4] = dataClear;
        settings[5] = checkUpdate;
        settings[6] = gameRecord;
        settings[7] = ledger;
        settings[8] = hardSetting;
        settings[9] = keyTest;

        version_txt.text = "version:" + Save.git_version;
    }

    public void SwitchIndex(System.Type type)
    {
        MonoBehaviour temp = null;
        foreach (var setting in settings)
        {
            if (setting.GetType() == type)
            {
                temp = setting;
            }
            else
            {
                setting.gameObject.SetActive(false);
            }
        }

        if (temp != null)
        {
           temp.gameObject.SetActive(true); 
        }
        
    }

    private void OnEnable()
    {
        SwitchIndex(typeof(Setting_MainSetting));
    }

    private void OnDisable()
    {
        
    }
}
