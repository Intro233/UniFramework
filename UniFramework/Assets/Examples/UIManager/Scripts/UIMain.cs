using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : UIBase
{
    private void Start()
    {
        GetControl<Button>("Start")?.onClick.AddListener(() =>
        {
            Debug.Log("Click Start!");
            UIManager.Instance.ShowPanel<UITips>("UITipsPanel", (Panel) => { Panel.Init("这是提示！"); });
        });
    }
}