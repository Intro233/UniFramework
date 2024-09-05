using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于定义所有触发事件的名字，方便统一管理
/// </summary>
public enum EventDefine
{
    PanelClose,

    /// <summary>
    /// 矿物修改事件
    /// </summary>
    OnOreChanged,

    /// <summary>
    /// 修改属性升级数据
    /// </summary>
    UpdateAttributeData,

    /// <summary>
    /// 增加矿石(局外道具)
    /// </summary>
    AddOre,
    
    /// <summary>
    /// 使用矿石
    /// </summary>
    UseOre,
}