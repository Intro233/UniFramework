using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow : UniFramework.UI.UIBase
{
    private bool isOpen = true;

    public override void OnAwake()
    {
        base.OnAwake();
        transform.Find("Panel/Button")?.GetComponent<Button>()?.onClick.AddListener(
            () =>
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
            }
        );
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Debug.LogError("di er Tan chuang ");
    }
}