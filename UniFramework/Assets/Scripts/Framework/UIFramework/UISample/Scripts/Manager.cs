using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static UniFramework.UI.UIManager UIManager = UniFramework.UI.UIManager.Instance;
    private void Awake()
    {
        UIManager.Initialize();
    }
}
