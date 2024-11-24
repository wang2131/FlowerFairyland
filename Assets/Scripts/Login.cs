using System;
using System.Collections;
using System.Collections.Generic;
using GlobalNamaspaces;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{

    private Login_InputField _inputField;
    private Text _resultText;

    [SerializeField]private Text _totalAddCredits;
    [SerializeField]private Text _totalReduceCredits;
    [SerializeField]private Text _totalCredits;

    private void Awake()
    {
        _inputField = GetComponentInChildren<Login_InputField>();
        _resultText = transform.Find("ResultText").GetComponent<Text>();
    }

    private void OnEnable()
    {
        _inputField.ResetValue();
        _resultText.gameObject.SetActive(false);
        _totalCredits.text = Save.gameData.bk.totalCredits.ToString();
        _totalAddCredits.text = Save.gameData.bk.totalAddCredits.ToString();
        _totalReduceCredits.text = Save.gameData.bk.totalReduceCredits.ToString();
        EventCenter.AddListener((EventType)EventName.hold1, () => {_inputField.ChangeIndex(-1);});
        EventCenter.AddListener((EventType)EventName.hold5, () => {_inputField.ChangeIndex(1);});
        EventCenter.AddListener((EventType)EventName.rollCredits, () => {_inputField.ChangeValue(1);});
        EventCenter.AddListener((EventType)EventName.guess, () => {_inputField.ChangeValue(-1);});
        
        EventCenter.AddListener((EventType)EventName.confirm, () =>
        {
            if (_inputField.VerifyPassword())
            {
                Main.main.SwitchIndex(typeof(Setting));
            }
            else
            {
                _resultText.gameObject.SetActive(true);
            }
        });
        EventCenter.AddListener((EventType)EventName.bet, () => {Main.main.SwitchIndex(typeof(Game));});
    }

    private void OnDisable()
    {
        EventCenter.RemoveAllListener((EventType)EventName.hold1);
        EventCenter.RemoveAllListener((EventType)EventName.hold5);
        EventCenter.RemoveAllListener((EventType)EventName.confirm);
        EventCenter.RemoveAllListener((EventType)EventName.rollCredits);
        EventCenter.RemoveAllListener((EventType)EventName.guess);
        EventCenter.RemoveAllListener((EventType)EventName.bet);
    }

    
}
