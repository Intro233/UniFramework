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
    /// ��ý������������ı�
    /// </summary>
    /// <returns></returns>
    public string GetDesc()
    {
        return desc;
    }
}