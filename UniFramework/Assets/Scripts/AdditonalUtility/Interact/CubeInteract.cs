using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInteract : InteractBase
{
    public override void Interact()
    {
        Debug.Log("��ð�������Cube");
        Destroy(gameObject);
    }
}