using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pool<T> : IPool<T>
{
    /// <summary>
    /// 缓存池
    /// </summary>
    protected readonly Stack<T> cacheStack = new();

    protected IFactory<T> createFactory;

    /// <summary>
    /// 缓存池数量
    /// </summary>
    public int Count => cacheStack.Count;
    
    public virtual T Allocate()
    {
        return cacheStack.Count == 0 ? createFactory.Create() : cacheStack.Pop();
    }

    public abstract bool Recycle(T obj);

    /// <summary>
    /// 清空缓存池
    /// </summary>
    /// <param name="onClear"></param>
    public void Clear(Action<T> onClear = null)
    {
        if (onClear != null)
        {
            foreach (var item in cacheStack)
            {
                onClear?.Invoke(item);
            }
        }
        cacheStack.Clear();
    }
}