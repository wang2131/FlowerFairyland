using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyDebugger
{
    private static MyDebugger debugger;

    public static void Init()
    {
        if (debugger == null)
        {
            debugger = new MyDebugger();
        }
        else
        {
            debugger = null;
            debugger = new MyDebugger();
        }
    }

    public static void Log(object message)
    {
#if UNITY_EDITOR
        if(debugger!=null)
            Debug.Log(message);
#endif
    }

    public static void Log(object message, Object context)
    {
#if UNITY_EDITOR
        if(debugger!=null)
            Debug.Log(message, context);
#endif
    }

}
