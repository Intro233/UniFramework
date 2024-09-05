using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Events;
using Object = UnityEngine.Object;

/// <summary>
/// 资源加载模块
/// 1.异步加载
/// 2.委托和 lambda表达式
/// 3.协程
/// 4.泛型
/// </summary>
public class ResMgr : MonoBehaviour
{
    //同步加载资源
    public T Load<T>(string name) where T : Object
    {
        T res = Resources.Load<T>(name);
        //如果对象是一个GameObject类型的 我把他实例化后 再返回出去 外部 直接使用即可
        if (res == null)
        {
            Debug.Log(name + "找不到");
            return null;
        }
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else
            return res;
    }

    //异步加载本地资源
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        //开启异步加载的协程
        Manager.MonoMgr.StartCoroutine(ReallyLoadAsync(name, callback));
    }
    //真正的协同程序函数  用于 开启异步加载对应的资源
    private IEnumerator ReallyLoadAsync<T>(string assetName, UnityAction<T> callback = null) where T : Object
    {
        ResourceRequest res = Resources.LoadAsync<T>(assetName);
        if (res == null)
            Debug.Log(assetName + "找不到");
        yield return res;

        if (res.asset is GameObject)
        {
            var go = Instantiate(res.asset) as GameObject;
            go.name = res.asset.name;
            callback(Instantiate(go) as T);
        }
        else
            callback(res.asset as T);
    }
}
