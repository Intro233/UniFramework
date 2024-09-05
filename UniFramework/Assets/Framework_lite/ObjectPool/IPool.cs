using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPool<T>
{
    /// <summary>
    /// 分配对象
    /// </summary>
    /// <returns></returns>
    T Allocate();

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    bool Recycle(T obj);
}