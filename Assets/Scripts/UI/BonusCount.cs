using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCount : MonoBehaviour
{
    [SerializeField] private SpriteRenderer ge;
    [SerializeField] private SpriteRenderer shi;

    [SerializeField] private Sprite[] characters;
    
    public void changeValue(int value)
    {
        if (value > 99)
        {
            ge.sprite = characters[9];
            shi.sprite = characters[9];

        }
        else
        {
            int temp = value;
            ge.sprite = characters[temp % 10];
            temp /= 10;
            shi.sprite = characters[temp % 10];
            temp /= 10;


        }
    }
}
