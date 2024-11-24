using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GlobalNamaspaces;
public class Setting_ChangePwd_InputField : MonoBehaviour
{
    [SerializeField] private Color _selectedColor;
    [SerializeField] private Color _unselectedColor = Color.white;
    private Image[] _images;
    private Text[] _texts;
    private string password;
    private string tempPassword = "";

    public bool IsRewrite;

    private int currentIndex = 0;
    private void Awake()
    {
        _images = new Image[8];
        _texts = new Text[8];
        password = "00000000";
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i] = transform.Find((i + 1).ToString()).GetComponent<Image>();
            _texts[i] = _images[i].GetComponentInChildren<Text>();
        }
    }

    private void ChangeIndexFromInt(int index)
    {
        if (index >= 0 && index < 8)
        {
            currentIndex = index;
        }
        for (int i = 0; i < _images.Length; i++)
        {
            if (i == currentIndex) _images[i].color = _selectedColor;
            else _images[i].color = _unselectedColor;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction">1是向后，-1向前</param>
    public void ChangeIndex(int direction)
    {
        if (direction == 1)
        {
            ChangeIndexFromInt((currentIndex+1)%8);
        }
        else if (direction == -1)
        {   
            ChangeIndexFromInt((currentIndex+8-1)%8);
        }
        
        
    }
    
    /// <summary>
    /// 改变值
    /// </summary>
    /// <param name="direction">1为增加，-1为减少</param>
    public void ChangeValue(int direction)
    {
        if (direction == 1)
        {
            int value = int.Parse(password[currentIndex].ToString());
            int changedValue = (value + 1) % 10;
            password = password.Remove(currentIndex, 1);
            password = password.Insert(currentIndex, changedValue.ToString());
        }
        else if (direction == -1)
        {
            int value = int.Parse(password[currentIndex].ToString());
            int changedValue = (value + 10 - 1) % 10;
            password = password.Remove(currentIndex, 1);
            password = password.Insert(currentIndex, changedValue.ToString());
            _texts[currentIndex].text = (changedValue).ToString();
        }

        StartCoroutine(ShowPassword(currentIndex));
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    /// <returns></returns>
    public int VerifyPassword()
    {
        if (!IsRewrite)
        {
            tempPassword = password;
            Reset();
            IsRewrite = true;
            return 0;
        }
        else
        {
            if (tempPassword == password)
            {
                Save.gameData.gs.pwd = password;
                Reset();
                tempPassword = "";
                IsRewrite = false;
                return 1; 
            }
            else
            {
                Reset();
                tempPassword = "";
                IsRewrite = false;
                return -1;
            }

        }
        
        
    }

    public void Reset()
    {
        ChangeIndexFromInt(0);
        
        password = "00000000";
        foreach (var VARIABLE in _texts)
        {
            VARIABLE.text = "*";
        }

        for (int i = 0; i < _texts.Length; i++)
        {
            StartCoroutine(ShowPassword(i));
        }
    }

    public IEnumerator ShowPassword(int index)
    {
        _texts[index].text = password[index].ToString();
        yield return new WaitForSeconds(0.5f);
        _texts[index].text = "*";
    }
}
