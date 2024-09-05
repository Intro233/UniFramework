using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Manager : MonoBehaviour
{
    public static ResMgr Resource { get; private set; }
    public static UIMgr UI { get; private set; }
    public static MonoMgr MonoMgr { get; private set; }

    private void Awake()
    {
        UI = this.gameObject.GetComponent<UIMgr>();
        Resource = this.gameObject.AddComponent<ResMgr>();
        MonoMgr = this.gameObject.AddComponent<MonoMgr>();
    }
}