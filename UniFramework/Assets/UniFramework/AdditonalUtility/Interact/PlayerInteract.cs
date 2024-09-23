using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float checkRadius = 2f;
    private List<InteractBase> mInteractables = new();
    private Collider[] mColliders = new Collider[20];

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GetInteractableObj()?.Interact();
        }
    }

    /// <summary>
    /// 得到最近的可交互物体
    /// </summary>
    /// <returns></returns>
    public InteractBase GetInteractableObj()
    {
        InteractBase closerObj = null;
        mInteractables.Clear();
        int length = Physics.OverlapSphereNonAlloc(transform.position, checkRadius, mColliders);
        if (length > mColliders.Length)
        {
            length = mColliders.Length;
        }

        Debug.Log($"InteractObjs Count:{mColliders.Length}");
        if (mColliders != null && length > 0)
        {
            for (int i = 0; i < length; i++)
            {
                if (mColliders[i].TryGetComponent(out InteractBase interactable))
                {
                    mInteractables.Add(interactable);
                }
            }
        }

        //计算最小距离
        if (mInteractables.Count > 0)
        {
            foreach (var interactable in mInteractables)
            {
                if (closerObj == null)
                {
                    closerObj = interactable;
                }
                else
                {
                    if (Vector3.Distance(closerObj.GetTansform().position, transform.position) >
                        Vector3.Distance(interactable.GetTansform().position, transform.position)
                       )
                    {
                        closerObj = interactable;
                    }
                }
            }
        }

        return closerObj;
    }
}