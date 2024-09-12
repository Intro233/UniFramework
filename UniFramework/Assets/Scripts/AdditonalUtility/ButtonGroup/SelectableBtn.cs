using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SelectableBtn : MonoBehaviour
{
    [SerializeField] private GameObject selectedObj;

    private Button mButton;

    public Button Button
    {
        get
        {
            if (mButton)
            {
                return mButton;
            }
            else
            {
                mButton = GetComponent<Button>();
                return mButton;
            }
        }
    }

    public Action onSelect;

    public Action onDeselect;

    public virtual void OnSelect()
    {
        selectedObj?.SetActive(true);
        onSelect?.Invoke();
    }

    public virtual void OnDeSelect()
    {
        selectedObj?.SetActive(false);
        onDeselect?.Invoke();
    }
}