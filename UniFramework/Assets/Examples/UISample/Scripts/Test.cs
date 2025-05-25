using UnityEngine;

public class Test : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    private void Init()
    {
        Manager.UIManager.ShowPanel<UIMainPanel>();
        Manager.UIManager.ShowPanel<UIWindow>();
    }
}