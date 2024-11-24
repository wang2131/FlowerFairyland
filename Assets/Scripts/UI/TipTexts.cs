using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;

public class TipTexts : MonoBehaviour
{
    [SerializeField] private GameObject waitForStart;
    [SerializeField] private GameObject cardStart;
    [SerializeField] private GameObject cardOver;
    [SerializeField] private GameObject guess;
    [SerializeField] private GameObject MainTheme;

    WaitForEndOfFrame wait = new WaitForEndOfFrame();
    

    private int currentIndex;

    private void OnEnable()
    {
        
    }

    private void Start()
    {
        waitForStart.SetActive(false);
        cardStart.SetActive(false);
        cardOver.SetActive(false);
        guess.SetActive(false);
        MainTheme.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        
    }

    public void Tip(int index)
    {

        GameObject temp;
        StopAllCoroutines();
        waitForStart.SetActive(false);
        cardStart.SetActive(false);
        cardOver.SetActive(false);
        guess.SetActive(false);
        MainTheme.SetActive(false);

        switch (index)
        {
            case 1:
                temp = waitForStart;
                
                break;
            case 2:
                temp = cardStart;
                break;
            case 3:
                temp = cardOver;
                break;
            case 4:
                temp = guess;
                break;
            default:
                temp = waitForStart;
                break;
        }

        StartCoroutine(TipSparkle(temp));
    }

    public void MainSparkle()
    {
        StartCoroutine(MainThemeSparkle());
    }

    IEnumerator TipSparkle(GameObject temp)
    {
        while (true)
        {
            bool a = (int)Time.fixedTime % 2 == 1;
            temp.SetActive(a);
            yield return wait;
        }
    }

    IEnumerator MainThemeSparkle()
    {
        MainTheme.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        while (true)
        {
            if (MainTheme.activeSelf)
            {
                MainTheme.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }

            if (!MainTheme.activeSelf)
            {
                MainTheme.SetActive(true);
                yield return new WaitForSeconds(3f);
            }
        }
    }

    public void Stop()
    {
        StopAllCoroutines();
        waitForStart.SetActive(false);
        cardStart.SetActive(false);
        cardOver.SetActive(false);
        guess.SetActive(false);
        MainTheme.SetActive(false);
    }
}
