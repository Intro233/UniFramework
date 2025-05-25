using UniFramework.Editor;
using UnityEngine;

public class TestInspector : MonoBehaviour
{
   [Button("1")]
    public void TestButton()
    {
        Debug.Log("Hello Button");
    }
}