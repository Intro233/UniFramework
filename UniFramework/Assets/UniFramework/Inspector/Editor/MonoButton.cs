using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UniFramework.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class MonoButton : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mono = target as MonoBehaviour;
            var type = mono.GetType();
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var method in methods)
            {
                var button = method.GetCustomAttribute<ButtonAttribute>(true);
                Debug.Log(1);
                if (button != null)
                {
                    if (button.name == null)
                    {
                        if (GUILayout.Button(method.Name))
                        {
                            method.Invoke(target, null);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button(button.name))
                        {
                            method.Invoke(target, null);
                        }
                    }
                }
            }
        }
    }
}