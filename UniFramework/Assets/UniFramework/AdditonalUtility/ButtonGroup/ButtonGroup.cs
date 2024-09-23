using System;
using UnityEngine;

public class ButtonGroup : MonoBehaviour
{
    [SerializeField] private SelectableBtn[] selectableBtns;

    private SelectableBtn mCurrentSelectBtn;

    public SelectableBtn[] SelectableBtns => selectableBtns;

    private void OnEnable()
    {
        SelectDefalutBtn();
    }

    private void Start()
    {
        if (selectableBtns == null)
        {
            selectableBtns = GetComponentsInChildren<SelectableBtn>();
            BindEvent();
            SelectDefalutBtn();
        }
        else
        {
            Debug.Log("已初始化按钮组，无需再初始化");
        }
    }

    public void Init()
    {
        if (selectableBtns == null || selectableBtns?.Length == 0)
        {
            selectableBtns = GetComponentsInChildren<SelectableBtn>();
            BindEvent();
            SelectDefalutBtn();
        }
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void BindEvent()
    {
        foreach (var selectableBtn in selectableBtns)
        {
            selectableBtn.Button.onClick.AddListener(() =>
            {
                if (mCurrentSelectBtn == selectableBtn)
                    return;
                if (mCurrentSelectBtn != null)
                {
                    mCurrentSelectBtn.OnDeSelect();
                    mCurrentSelectBtn = selectableBtn;
                    mCurrentSelectBtn.OnSelect();
                }
                else
                {
                    mCurrentSelectBtn = selectableBtn;
                    mCurrentSelectBtn.OnSelect();
                }
            });
        }
    }

    private void RemoveEvent()
    {
        foreach (var selectableBtn in selectableBtns)
        {
            selectableBtn.Button.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 默认选中第一个按钮
    /// </summary>
    private void SelectDefalutBtn()
    {
        if (selectableBtns != null && selectableBtns.Length > 0)
        {
            selectableBtns[0].Button.onClick.Invoke();
        }
    }
}