using System.Collections;
using System.Collections.Generic;
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
            if (property == null)
            {
                Debug.Log($"Enumerable property of '{gameObject.name}':'{propertyName}' is NULL.");
                return false;
            }

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

        // ------------
        // LIST RELATED
        public static void CopyListTo(ref List<string> source, ref List<string> target)
        {
            foreach (var item in source)
                target.Add(item);
        }

        public static void CopyListTo(ref List<Doorway> source, ref List<Doorway> target)
        {
            foreach (var item in source)
                target.Add(new Doorway(item));
        }

        // ------------
        // MATH RELATED
        public static bool IsInBounds(int min1, int max1, int min2, int max2)
        {
            return Mathf.Max(min1, min2) <= Mathf.Min(max1, max2);
        }
    }
}
