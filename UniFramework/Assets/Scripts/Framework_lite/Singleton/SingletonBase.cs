using UnityEngine;

public class SingletonBase<T> where T : new()
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}

public class SingletonMonoBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var t = FindObjectOfType<T>();
                if (t)
                {
                    instance = t;
                }
                else
                {
                    var obj = new GameObject();
                    obj.name = typeof(T).ToString();
                    DontDestroyOnLoad(obj);
                    instance = obj.AddComponent<T>();
                }
            }

            return instance;
        }
    }
}