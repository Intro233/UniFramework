using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YooAsset;

public class Test : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        while (!AppFacade.PacageInited)
        {
            yield return null;
        }

        Manager.UIManager.ShowPanel<UIMainPanel>();
        Manager.UIManager.ShowPanel<UIWindow>();
    }
}