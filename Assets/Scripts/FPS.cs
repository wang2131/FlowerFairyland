 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    [SerializeField]
    public Text showFPSText;
    private float fpsByDeltatime = 1.5f;
    private float passedTime = 0.0f;
    private int frameCount = 0;
    private float realtimeFPS = 0.0f;
 

    void Start()
    {
      //  inst = GameManager.Inst;
        SetFPS();
    }
    void Update()
    {
        GetFPS();
    }
    private void SetFPS()
    {
        //如果QualitySettings.vSyncCount属性设置，这个值将被忽略。
        //设置应用平台目标帧率为 60
        //Application.targetFrameRate = 60;
    }
    private void GetFPS()
    {
        if (showFPSText == null) return;

        //第一种方式获取FPS
        if (showFPSText != null)
        {
            float fps = 1.0f / Time.smoothDeltaTime;
            showFPSText.text = "FPS:  " + fps.ToString("0");
        }

        //第二种方式
        frameCount++;
        passedTime += Time.deltaTime;
        if (passedTime >= fpsByDeltatime)
        {
            realtimeFPS = frameCount / passedTime;
            if (showFPSText != null)
                showFPSText.text = "FPS:  " + realtimeFPS.ToString("f1");
            passedTime = 0.0f;
            frameCount = 0;
        }
    }
}
