using System.Collections;
using System.Collections.Generic;
using CodeBind;
using UnityEngine;
using UnityEngine.UI;

public partial class UIWindow : UniFramework.UI.UIBase
{
    private bool isOpen = true;
    public override void OnAwake()
    {
        base.OnAwake();
        InitBind(mBindMono);
        AddButtonClickListener(ShowButton, () =>
        {
            if (isOpen)
            {
                Manager.UIManager.HidePanel<UIMainPanel>();
            }
            else
            {
                Manager.UIManager.ShowPanel<UIMainPanel>();
            }

            isOpen = !isOpen;
        });
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Debug.LogError("di er Tan chuang ");
    }
}