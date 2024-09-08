using UnityEngine;

public abstract class InteractBase : MonoBehaviour, IInteractable
{
    [SerializeField] private string desc;
    public abstract void Interact();

    
    public Transform GetTansform()
    {
        return transform;
    }

    /// <summary>
    /// 获得交互面板的描述文本
    /// </summary>
    /// <returns></returns>
    public string GetDesc()
    {
        return desc;
    }
}