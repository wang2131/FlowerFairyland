using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrizeNotes : MonoBehaviour
{
    public Text[] values;
    public Image[] highLights;
    private void Awake()
    {
        values = new Text[10];
        highLights = new Image[10];
        for (int i = 0; i < values.Length; i++)
        {
            highLights[i] = transform.Find((i + 1).ToString()).GetComponent<Image>();
            values[i] = highLights[i].transform.Find("value").GetComponent<Text>();
        }
    }

    public void ChangeValue(int value)
    {
        values[0].text = (value * 1).ToString();
        values[1].text = (value * 2).ToString();
        values[2].text = (value * 3).ToString();
        values[3].text = (value * 5).ToString();
        values[4].text = (value * 7).ToString();
        values[5].text = (value * 10).ToString();
        values[6].text = (value * 60).ToString();
        values[7].text = (value * 120).ToString();
        values[8].text = (value * 250).ToString();
        values[9].text = (value * 750).ToString();
    }

    /// <summary>
    /// 第二组牌高亮奖励
    /// </summary>
    /// <param name="index"></param>
    public void HighLight(int index)
    {
        ClearHighLights();
        int realIndex = index - 1;
        if (realIndex >= 0)
        {
            highLights[realIndex].enabled = true;
        }

    }

    public void ClearHighLights()
    {
        for (int i = 0; i < highLights.Length; i++)
        {
            highLights[i].enabled = false;
        }
    }

    /// <summary>
    /// 第一组牌闪烁奖励
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IEnumerator StartHighlight(int index)
    {
        ClearHighLights();
        int realIndex = index - 1;
        if (realIndex >= 0 && realIndex <= highLights.Length)
        {
            while (true)
            {
                highLights[realIndex].enabled = !highLights[realIndex].enabled;
                yield return new WaitForSeconds(0.5f);
            }            
        }

        
    }
}
