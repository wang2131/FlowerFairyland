using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryMyself : MonoBehaviour
{
    public float time=2.0f;


    void Start()
    {
        Invoke("DestroyMyself", time);
    }

     void DestroyMyself()
    {
        DestroyImmediate(gameObject.transform.gameObject);
    }
}
