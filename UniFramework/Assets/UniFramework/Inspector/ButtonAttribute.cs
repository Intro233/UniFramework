using System;

namespace UniFramework.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : Attribute
    {
        public string name;

        public ButtonAttribute()
        {
        }

        public ButtonAttribute(string name)
        {
            this.name = name;
        }
    }
}