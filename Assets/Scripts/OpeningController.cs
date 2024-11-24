using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float Speed = 1.0f;
    private WaitForSeconds wait;

    private void Awake()
    {
        wait = new WaitForSeconds(Speed/50f);
    }

    private void Start()
    {
        StartCoroutine(StartLoadScene());
    }

    private IEnumerator StartLoadScene()
    {
        slider.value = 0f;
        
        while(slider.value < 1f)//场景加载没有完成时
        {
            
            slider.value += 0.02f;

            if (slider.value > 0.9f)
            {
                slider.value = 1f;
                SceneManager.LoadScene(1);
            }
            yield return wait;
            
        }
    }
}
