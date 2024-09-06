using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// UI层级
/// </summary>
public enum EUILayer
{
    Bottom,
    Mid,
    Top,
}

public class UIManager : SingletonMonoBase<UIManager>
{
    //Canvas
    [SerializeField] private RectTransform canvas;
    
    //UI层级
    private Transform mBottom;
    private Transform mMid;
    private Transform mTop;
    
    //游戏内常驻界面
    private Dictionary<string, UIBase> panelDic = new Dictionary<string, UIBase>();
    public List<string> topPanelDic = new();

    private void Awake()
    {
        mBottom = canvas.Find("Bottom");
        mMid = canvas.Find("Mid");
        mTop = canvas.Find("Top");
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// 通过层级枚举 得到对应层级的父对象
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerFather(EUILayer layer)
    {
        return layer switch
        {
            EUILayer.Bottom => mBottom,
            EUILayer.Mid => mMid,
            EUILayer.Top => mTop,
            _ => throw new ArgumentOutOfRangeException(nameof(layer), layer, null)
        };
    }

    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">当面板预设体创建成功后 你想做的事</param>
    public void ShowPanel<T>(string panelName, UnityAction<T> callBack = null) where T : UIBase
    {
        if (panelDic.ContainsKey(panelName))
        {
            if (panelDic[panelName] != null)
            {
                // 处理面板创建完成后的逻辑
                callBack?.Invoke(panelDic[panelName] as T);
                panelDic[panelName]?.ShowMe();
                if (!topPanelDic.Contains(panelName))
                    topPanelDic.Add(panelName);
                //避免面板重复加载 如果存在该面板 即直接显示 调用回调函数后  直接return 不再处理后面的异步加载逻辑
                return;
            }
        }

        #region 使用对象池

        // Transform father = mBottom;


        var go = GameObjectPool.Instance.Allocate(PathUtil.Panel_Url + panelName);
        // go.transform.SetParent(father);
        go.transform.Reset();

        //得到预设体身上的面板脚本
        T panel = go.GetComponent<T>();
        // 处理面板创建完成后的逻辑
        callBack?.Invoke(panel);
        panel.ShowMe();
        //把面板存起来
        if (panelDic.ContainsKey(panelName))
        {
            Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
            panelDic.Add(panelName, panel);
        }
        else
            panelDic.Add(panelName, panel);

        #endregion
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HidePanel(string panelName)
    {
        if (topPanelDic.Contains(panelName))
            topPanelDic.Remove(panelName);
        if (panelDic.ContainsKey(panelName))
        {
            if (panelDic[panelName] != null)
            {
                //隐藏子面板
                panelDic[panelName].HideMe();
                //隐藏自己
                // Destroy(panelDic[panelName].gameObject);
                GameObjectPool.Instance.Recycle(PathUtil.Panel_Url + panelName, panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }
    }

    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>(string name) where T : UIBase
    {
        if (panelDic.ContainsKey(name))
            return panelDic[name] as T;
        return null;
    }

    /// <summary>
    /// 获取当前活跃的面板名字
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetActivityPanel()
    {
        if (topPanelDic.Count > 0)
            return topPanelDic[topPanelDic.Count - 1];
        return null;
    }
}