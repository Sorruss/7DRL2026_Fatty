using System.Collections;
using UnityEngine;

namespace FG
{
    public static class Helpers
    {
        // --------------------------
        // EDITOR PROPERTY VALIDATORS
        public static bool ValidateStringProperty(Object gameObject, string propertyName, string property)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                Debug.Log($"String property of '{gameObject.name}':'{propertyName}' has no value. It should.");
                return false;
            }

            return true;
        }

        public static bool ValidateEnumerableProperty(Object gameObject, string propertyName, IEnumerable property)
        {
            foreach (var item in property)
            {
                if (item == null)
                {
                    Debug.Log($"Enumerable property of '{gameObject.name}':'{propertyName}' has NULL in it. It shouldn't.");
                    return false;
                }
            }

            return true;
        }
    }
}
