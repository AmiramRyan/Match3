using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalPannel : MonoBehaviour
{
    public Image thisImg;
    public Sprite thisSpr;
    public Text thisText;
    public string thisStr;
    // Start is called before the first frame update
    void Start()
    {
        SetUp();
    }

    void SetUp()
    {
        thisImg.sprite = thisSpr;
        thisText.text = thisStr;
    }

}
