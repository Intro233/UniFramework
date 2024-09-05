using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIExtends
{
    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
        if (transform is RectTransform rectTransform)
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }

    public static void ResetScale(this Transform transform)
    {
        transform.localScale = Vector3.one;
    }
}