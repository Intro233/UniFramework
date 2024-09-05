using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;
    public Dictionary<string,T> dataset= new Dictionary<string, T>();
    public Dictionary<string, UnityAction<T>> funset = new Dictionary<string, UnityAction<T>>();
    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public UnityAction actions;
    public Dictionary<string, UnityAction> funset = new Dictionary<string, UnityAction>();
    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}

public class EventInfo<T1,T2> : IEventInfo
{
    public UnityAction<T1,T2> actions;
    public EventInfo(UnityAction<T1,T2> action)
    {
        actions += action;
    }
}

/// <summary>
/// 事件中心 单例模式对象
/// 1.Dictionary
/// 2.委托
/// 3.观察者设计模式
/// 4.泛型
/// </summary>
public class EventManager : SingletonBase<EventManager>
{
    //key —— 事件的名字（比如：怪物死亡，玩家死亡，通关 等等）
    //value —— 对应的是 监听这个事件 对应的委托函数们
    private Dictionary<EventDefine, IEventInfo> eventDic = new Dictionary<EventDefine, IEventInfo>();
    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件 的委托函数</param>
    public void AddEventListener<T>(EventDefine name, UnityAction<T> action)
    {
        //有没有对应的事件监听
        //有的情况
        if( eventDic.ContainsKey(name) )
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        //没有的情况
        else
        {
            eventDic.Add(name, new EventInfo<T>( action ));
        }
    }
    /// <summary>
    /// 监听不需要参数传递的事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(EventDefine name, UnityAction action)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        //没有的情况
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    public void AddEventListener<T1,T2>(EventDefine name, UnityAction<T1,T2> action)
    {
        //有没有对应的事件监听
        //有的情况
        if( eventDic.ContainsKey(name) )
        {
            (eventDic[name] as EventInfo<T1,T2>).actions += action;
        }
        //没有的情况
        else
        {
            eventDic.Add(name, new EventInfo<T1,T2>( action ));
        }
    }
    
    /// <summary>
    /// 移除对应的事件监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">对应之前添加的委托函数</param>
    public void RemoveEventListener<T>(EventDefine name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T>).actions -= action;
    }
    /// <summary>
    /// 移除不需要参数的事件
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveEventListener(EventDefine name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo).actions -= action;
    }
    
    public void RemoveEventListener<T1,T2>(EventDefine name, UnityAction<T1,T2> action)
    {
        if (eventDic.ContainsKey(name))
            (eventDic[name] as EventInfo<T1,T2>).actions -= action;
    }

    /// <summary>
    /// 事件触发,T：触发参数的类型，触发事件，触发参数
    /// </summary>
    /// <param name="name">哪一个名字的事件触发了</param>
    public void TriggerEvent<T>(EventDefine name, T info)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T>).actions != null)
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
        }
    }
    /// <summary>
    /// 事件触发（不需要参数的）
    /// </summary>
    /// <param name="name"></param>
    public void TriggerEvent(EventDefine name)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();
        }
    }
    
    public void TriggerEvent<T1,T2>(EventDefine name, T1 info1,T2 info2)
    {
        //有没有对应的事件监听
        //有的情况
        if (eventDic.ContainsKey(name))
        {
            if ((eventDic[name] as EventInfo<T1,T2>).actions != null)
                (eventDic[name] as EventInfo<T1,T2>).actions.Invoke(info1,info2);
        }
    }
    
    
    /// <summary>
    /// 清空事件中心
    /// 主要用在 场景切换时
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
