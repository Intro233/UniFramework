

using UnityEngine;

public class UIMainPanel : UniFramework.UI.UIBase
{
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.LogError("UI Awake");
    }

    public override void OnShow()
    {
        base.OnShow();
        Debug.LogError("UI OnShow");
    }

    public override void OnHide()
    {
        base.OnHide();
        Debug.LogError("UI Hide");
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Debug.LogError("UI Destroy");
    }
}