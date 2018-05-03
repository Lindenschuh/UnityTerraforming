using UnityEngine;
namespace Invector
{
    public class vEnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public vEnumFlagAttribute() { }

        public vEnumFlagAttribute(string name)
        {
            enumName = name;
        }
    }
}