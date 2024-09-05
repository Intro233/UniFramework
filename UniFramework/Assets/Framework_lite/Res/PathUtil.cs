using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil 
{
    //根目录
    public static readonly string AseetsPath = Application.dataPath;
    
    //只读目录
    public static readonly string ReadPath = Application.streamingAssetsPath;

    //可读写目录
    public static readonly string ReadWritePath = Application.persistentDataPath;
    
    //记录UI面板资源目录路径
    public static readonly string Panel_Url = "UI/";
    
    /// <summary>
    /// 属性路径
    /// </summary>
    public static readonly string Attribute_Url = "Sprite/Attributle/";
    
    public static readonly string Common_Url = "Sprite/Common/";
    
    public static readonly string Shop_Url = "Sprite/Shop/";

    /// <summary>
    /// 获取Unity的相对路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Substring(path.IndexOf("Assets"));
    }

    /// <summary>
    /// 获取标准路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;
        return path.Trim().Replace("\\", "/");
    }
}
