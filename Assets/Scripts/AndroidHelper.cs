using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidHelper : MonoBehaviour
{
    static AndroidJavaObject appController;

    //call 后面int 是返回值，max*value是参数,注意Call和CallStatic区别，以API为准
    //appController.Call<int>("setSystemVolumeIndex", max* value);
    //appController.CallStatic<int>("setSystemVolumeIndex", max* value);

    public static void Init()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            //通过UnityPlayer类，获取Activity
            var currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");   

            Debug.Log("Activity类：" + currentActivity!=null);
            AndroidJavaClass launcher = new AndroidJavaClass("ZtlApi.ZtlManager");      //获取java类
            appController = launcher.CallStatic<AndroidJavaObject>("GetInstance");
            Debug.Log("appController：" + appController!=null);

            //初始化java类，把activity传进去，很多android方法调用都要用到，callback是一个回调，就是java调用Unity要用到的
            appController.Call("setContext", currentActivity);
        }
    }

    public static string GetJarVersion()
    {
        string ver = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            ver = appController.Call<string>("getJARVersion");
        }
        return ver;
    }

    // 0<=hour<24, 0<=min<60 
    public static void SetSchedulePowerOn(int hour, int min, bool enableSchedulePoweron = true)
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            appController.Call("setSchedulePowerOn", hour, min, enableSchedulePoweron);
        }
    }

    public static void SetSchedulePowerOff(int hour, int min, bool enableSchedulePoweroff = true)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appController.Call("setSchedulePowerOff", hour, min, enableSchedulePoweroff);
        }
    }

    public static void SetAutoBoot()
    {
        //if (Application.platform == RuntimePlatform.Android)
        {
            //appController.Call("setBootPackageActivity", "com.demo.test1", "com.unity3d.player.UnityPlayerActivity");
            appController.Call("setBootPackageActivity", "com.demo.test1", "com.demo.test.MainActivity");
        }
    }

    public static void RebootAndroid()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appController.Call("rebootSystem");
            appController.Call("reboot",1);
        }
    }

    public static string GetIPv4(string nettype)
    {
        string ip = "";
        if(Application.platform == RuntimePlatform.Android)
        {
            ip = appController.Call<string>("getIPv4", nettype);
        }
        return ip;
    }

    public static void SetEthIP(string ip, string mask, string gate, string dns1)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appController.Call("setEthIP", true, ip, mask, gate, dns1, dns1);
        }
    }

    public static void SetWiFiIP(string ssid, string pwd, string ip, string mask, string gate, string dns1)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            appController.Call("setWifiIP", ssid, pwd, true, ip, mask, gate, dns1, dns1);
        }
    }

    public static void SetSystemVolum(float value)
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            int max = appController.Call<int>("getSystemMaxVolume");

            //call 后面int 是返回值，max*value是参数
            int ret = appController.Call<int>("setSystemVolumeIndex", (int)(max * value));
            Debug.Log("设置音量，返回： " + ret);
        }
    }

    public void OpenSetting()
    {
        appController.Call<int>("startSettings");//这里是调用的jar的api打开系统设置
    }

    private void Start()
    {
        Init();
        
        SetAutoBoot();
        //appController.Call("setDesktop", "com.demo.test1");
        appController.Call("openSystemBar", true);
        string flag1 = appController.Call<string>("getBootPackageName");
        string flag2 = appController.Call<string>("getBootPackageActivity");
        bool flag3 = appController.Call<bool>("isAppExist", "com.demo.test1");
        string flag4 = appController.Call<string>("getDesktop");
        Debug.Log("获取自启动包名：" + flag1);
        Debug.Log("获取自启动类名：" + flag2);
        Debug.Log("com.demo.test1包对应app是否存在：" + flag3);
        Debug.Log("桌面的包名：" + flag4);
        

    }

    void OnGUI()
    {
        //方法一 
        //try
        //{
        //    var javaClass = new AndroidJavaObject("com.ztl.u3dztlapi.MainActivity");
        //    string value1 = javaClass.Call<string>("test");
        //    GUI.Label(new Rect(10, 10, 500, 500), value1);
        //}
        //catch (System.Exception ex)
        //{
        //    GUI.Label(new Rect(10, 10, 500, 500), ex.ToString());
        //}
        //方法二
        //try
        //{
        //    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        //    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
        //    string value2 = jo.Call<string>("test");
        //    GUI.Label(new Rect(20, 20, 500, 500), value2);
        //}
        //catch (System.Exception ex)
        //{
        //    GUI.Label(new Rect(20, 20, 500, 500), ex.ToString());
        //}

    }

    public void OnClickQuit()
    {
        Application.Quit(0);
    }
}