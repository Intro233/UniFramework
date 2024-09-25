using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class GameObjectPool : SingletonBase<GameObjectPool>
{
    private Dictionary<string, SimpleObjectPool<GameObject>> mGOPools = new();


    /// <summary>
    /// 从池中取得一个对象
    /// </summary>
    /// <param name="path">相对Resouces目录路径</param>
    /// <returns>物体</returns>
    public GameObject Allocate(string path)
    {
        if (mGOPools.TryGetValue(path, out var pool))
        {
            var aseet = pool.Allocate();
            aseet.SetActive(true);
            Debug.LogWarning($"已取出对象,对象池{path}剩余对象：{pool}.");
            return aseet;
        }
        else
        {
            SimpleObjectPool<GameObject> goPool = new SimpleObjectPool<GameObject>(() =>
                {
                    var go = Resources.Load<GameObject>(path);
                    if (!go)
                        return null;
                    var asset = Object.Instantiate(go);
                    asset.name = go.name;
                    return asset;
                },
                go => { go.SetActive(false); });
            mGOPools.Add(path, goPool);
            return goPool.Allocate();
        }
    }

    /// <summary>
    /// 回收某个路径下的该对象
    /// </summary>
    /// <param name="path">分配对象时的路径</param>
    /// <param name="go">回收的对象</param>
    public void Recycle(string path, GameObject go)
    {
        if (mGOPools.TryGetValue(path, out var pool))
        {
            pool.Recycle(go);
            Debug.LogWarning($"已回收对象,对象池{path}剩余对象：{pool}.");
        }
        else
        {
            Debug.LogError($"对象池{path}不存在，无法回收对象.");
        }
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void Clear()
    {
        foreach (var item in mGOPools)
        {
            item.Value.Clear((go) => { Object.Destroy(go); });
        }

        mGOPools.Clear();
    }
}