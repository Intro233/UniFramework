using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Manager.UIManager.ShowPanel<UIMainPanel>();
        Manager.UIManager.ShowPanel<UIWindow>();
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
