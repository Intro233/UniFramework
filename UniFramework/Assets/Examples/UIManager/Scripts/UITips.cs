using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITips : UIBase
{
    public void Init(string msg)
    {
        GetControl<Text>("msg").text = msg;
    }
}
