using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 面板基类 
/// 帮助我门通过代码快速的找到所有的子控件
/// 方便我们在子类中处理逻辑 
/// 节约找控件的工作量
/// </summary>
public class UIBase : MonoBehaviour
{
    //一个面板同时只会存在一个子节点，当该面板关闭时，子界面也会关闭
    [HideInInspector] public string sonPanelName;

    //一个面板同时只会存在一个父节点
    [HideInInspector] public string parentPanelName;

    //面板的层级
    public EUILayer layer = EUILayer.Top;

    private bool mIsPanel = false;

    //通过里式转换原则 来存储所有的控件
    private Dictionary<string, List<UIBehaviour>> mControlDic = new();

    // Use this for initialization
    protected virtual void Awake()
    {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();
        FindChildrenControl<TMP_Text>();
        FindChildrenControl<TMP_InputField>();
        FindChildrenControl<TMP_Dropdown>();
    }

    public virtual void UpdateInfo()
    {
        if (mIsPanel)
            UIManager.Instance.GetPanel<UIBase>(parentPanelName)?.UpdateInfo();
    }

    public virtual void UpdateInfo(int id)
    {
        if (mIsPanel)
            UIManager.Instance.GetPanel<UIBase>(parentPanelName)?.UpdateInfo(id);
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe()
    {
        mIsPanel = true;
        this.transform.SetParent(UIManager.Instance.GetLayerFather(layer),false);
        if (UIManager.Instance.GetActivityPanel() != null)
            parentPanelName =UIManager.Instance.GetActivityPanel();

        if (layer == EUILayer.Top)
        {
            if (!UIManager.Instance.topPanelDic.Contains(this.transform.name.Replace("(Clone)", "")))
                UIManager.Instance.topPanelDic.Add(this.transform.name.Replace("(Clone)", ""));
        }

        sonPanelName = "";
    }

    /// <summary>
    /// 显示自己
    /// </summary>
    public virtual void ShowMe<T>(T info)
    {
    }

    /// <summary>
    /// 隐藏自己
    /// </summary>
    public virtual void HideMe()
    {
        if (sonPanelName != "")
            UIManager.Instance.HidePanel(sonPanelName);
    }

    /// <summary>
    /// 根据新的子界面名字来判断是否关闭原来的子界面
    /// </summary>
    public void HideSonPanel(string nextpanelname = "")
    {
        if (sonPanelName == "")
        {
            sonPanelName = nextpanelname;
            return;
        }

        if (sonPanelName == nextpanelname)
            return;
        if (sonPanelName != "")
        {
            UIManager.Instance.HidePanel(sonPanelName);
            sonPanelName = nextpanelname;
        }
    }

    protected virtual void OnClick(string btnName)
    {
    }

    protected virtual void OnValueChanged(string toggleName, bool value)
    {
    }

    public virtual void InitInfo(int num)
    {
    }


    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if (mControlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < mControlDic[controlName].Count; ++i)
            {
                if (mControlDic[controlName][i] is T)
                    return mControlDic[controlName][i] as T;
            }
        }

        return null;
    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected List<T> GetControls<T>(string controlName) where T : UIBehaviour
    {
        List<T> controls = new List<T>();
        if (mControlDic.ContainsKey(controlName))
        {
            for (int i = 0; i < mControlDic[controlName].Count; ++i)
            {
                if (mControlDic[controlName][i] is T)
                    controls.Add(mControlDic[controlName][i] as T);
            }
        }

        return controls;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T : UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].gameObject.name;
            if (mControlDic.ContainsKey(objName))
                mControlDic[objName].Add(controls[i]);
            else
                mControlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
            //如果是按钮控件
            if (controls[i] is Button)
            {
                (controls[i] as Button).onClick.AddListener(() => { OnClick(objName); });
            }
            //如果是单选框或者多选框
            else if (controls[i] is Toggle)
            {
                (controls[i] as Toggle).onValueChanged.AddListener((value) => { OnValueChanged(objName, value); });
            }
        }
    }
}