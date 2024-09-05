using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// UI层级
/// </summary>
public enum E_UI_Layer
{
    BotDown,
    Bot,
    Mid,
    Top,
    TopUp,
    System,
}
/// <summary>
/// UI状态
/// </summary>
public enum E_UI_State
{
    Common,
    Bag,
    FriendBag,
    RongLian,
    Qianghua,
    HunShiWear,
    HunShiLvUp,
    EquipUpStage,
    EquipUpRongZhu,
}
/// <summary>
/// UI管理器
/// 1.管理所有显示的面板
/// 2.提供给外部 显示和隐藏等等接口
/// </summary>
public class UIMgr : MonoBehaviour
{
    //游戏内常驻界面
    private Dictionary<string, UIBase> foreverpanelDic = new Dictionary<string, UIBase>();
    private Dictionary<string, UIBase> panelDic = new Dictionary<string, UIBase>();
    public List<string> topPanelDic = new();
    private Transform botdown;
    private Transform bot;
    private Transform mid;
    private Transform top;
    public Transform topup { get; private set; }
    public Transform system { get; private set; }
    
    //记录我们UI的Canvas父对象 方便以后外部可能会使用它
    public RectTransform canvas;


    private void Awake()
    {
        //找到各层
        botdown = canvas.Find("BotDown");
        bot = canvas.Find("Bot");
        mid = canvas.Find("Mid");
        top = canvas.Find("Top");
        topup = canvas.Find("TopUp");
        system = canvas.Find("System");
    }

    /// <summary>
    /// 通过层级枚举 得到对应层级的父对象
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public Transform GetLayerFather(E_UI_Layer layer)
    {
        switch(layer)
        {
            case E_UI_Layer.BotDown:
                return this.botdown;
            case E_UI_Layer.Bot:
                return this.bot;
            case E_UI_Layer.Mid:
                return this.mid;
            case E_UI_Layer.Top:
                return this.top;
            case E_UI_Layer.TopUp:
                return this.topup;
            case E_UI_Layer.System:
                return this.system;
        }
        return null;
    }
   
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <typeparam name="T">面板脚本类型</typeparam>
    /// <param name="panelName">面板名</param>
    /// <param name="layer">显示在哪一层</param>
    /// <param name="callBack">当面板预设体创建成功后 你想做的事</param>
    public void ShowPanel<T>(string panelName, UnityAction<T> callBack = null, E_UI_Layer layer = E_UI_Layer.Top) where T:UIBase
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

        Transform father = bot;
        switch (layer)
        {
            case E_UI_Layer.BotDown:
                father = botdown;
                break;
            case E_UI_Layer.Bot:
                father = bot;
                break;
            case E_UI_Layer.Mid:
                father = mid;
                break;
            case E_UI_Layer.Top:
                father = top;
                break;
            case E_UI_Layer.TopUp:
                father = topup;
                break;
            case E_UI_Layer.System:
                father = system;
                break;
        }

        var go = GameObjectPool.Instance.Allocate(PathUtil.Panel_Url + panelName);
        go.transform.SetParent(father);
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
        #region 无对象池版本

        //
        // Manager.Resource.LoadAsync<GameObject>(PathUtil.Panel_Url + panelName, (obj) =>
        // {
        //     //把他作为 Canvas的子对象
        //     //并且 要设置它的相对位置
        //     //找到父对象 你到底显示在哪一层
        //     Transform father = bot;
        //     switch (layer)
        //     {
        //         case E_UI_Layer.BotDown:
        //             father = botdown;
        //             break;
        //         case E_UI_Layer.Bot:
        //             father = bot;
        //             break;
        //         case E_UI_Layer.Mid:
        //             father = mid;
        //             break;
        //         case E_UI_Layer.Top:
        //             father = top;
        //             break;
        //         case E_UI_Layer.TopUp:
        //             father = topup;
        //             break;
        //         case E_UI_Layer.System:
        //             father = system;
        //             break;
        //     }
        //     //GameObject obj = GameObject.Instantiate(res) as GameObject;
        //     //设置父对象  设置相对位置和大小
        //     obj.transform.SetParent(father);
        //     obj.transform.localPosition = Vector3.zero;
        //     obj.transform.localScale = Vector3.one;
        //     (obj.transform as RectTransform).offsetMax = Vector2.zero;
        //     (obj.transform as RectTransform).offsetMin = Vector2.zero;
        //
        //     //得到预设体身上的面板脚本
        //     T panel = obj.GetComponent<T>();
        //     // 处理面板创建完成后的逻辑
        //     callBack?.Invoke(panel);
        //     panel.ShowMe();
        //     //把面板存起来
        //     if (panelDic.ContainsKey(panelName))
        //     {
        //         GameObject.Destroy(panelDic[panelName].gameObject);
        //         panelDic.Remove(panelName);
        //         panelDic.Add(panelName, panel);
        //     }
        //     else
        //         panelDic.Add(panelName, panel);
        // });

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
                GameObjectPool.Instance.Recycle(PathUtil.Panel_Url + panelName,panelDic[panelName].gameObject);
                panelDic.Remove(panelName);
            }
        }
    }
    /// <summary>
    /// 隐藏除常驻面板之外的所有面板
    /// </summary>
    /// <param name="panelName"></param>
    public void HideOtherPanel()
    {
        List<string> mapname = new List<string>();
        foreach(string key in panelDic.Keys)
        {
            if (!foreverpanelDic.ContainsKey(key))
                mapname.Add(key);
        }
        for(int i=0;i< mapname.Count;++i)
        {
            if (panelDic.ContainsKey(mapname[i]))
            {
                panelDic[mapname[i]].HideMe();
                GameObject.Destroy(panelDic[mapname[i]].gameObject);
                panelDic.Remove(mapname[i]);
            }
        }
        // foreverpanelDic["MainPanel"].GetComponent<MainPanel>()?.ResetSceneState();
    }
    /// <summary>
    /// 当前是否有除了常驻界面之外的界面显示
    /// </summary>
    /// <param name="panelName"></param>
    public bool IsHasOtherPanel()
    {
        return foreverpanelDic.Count == panelDic.Count ? false :true ;
    }
    /// <summary>
    /// 得到某一个已经显示的面板 方便外部使用
    /// </summary>
    public T GetPanel<T>(string name) where T:UIBase
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
            return topPanelDic[topPanelDic.Count-1];
        return null;
    }
    public void AddForeverPanel(string name,UIBase gameObject) 
    {
        if(!foreverpanelDic.ContainsKey(name))
            foreverpanelDic.Add(name, gameObject);
    }
    /// <summary>
    /// 给控件添加自定义事件监听
    /// </summary>
    /// <param name="control">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callBack)
    {
        EventTrigger trigger = control.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = control.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callBack);

        trigger.triggers.Add(entry);
    }
}
