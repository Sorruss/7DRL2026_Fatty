using System;
using System.Collections.Generic;
using UnityEngine;

namespace FG
{
    [System.Serializable]
    public struct Pool
    {
        public int size;
        public GameObject prefab;
        public string componentName;
    }

    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        [Header("Pools")]
        public Pool[] pools;
        private Dictionary<int, Queue<Component>> poolDict = new();  // KEY - PREFAB ID, VALUE - COMPONENT QUEUE

        // ------------
        // UNITY EVENTS
        private void Start()
        {
            foreach (var pool in pools)
                CreatePool(pool.size, pool.prefab, pool.componentName);
        }

        // ------------
        // MAIN METHODS
        private void CreatePool(int size, GameObject prefab, string componentName)
        {
            int prefabID = prefab.GetInstanceID();
            if (poolDict.ContainsKey(prefabID))
                return;

            // CREATING ANCHOR POINT FOR POOL'S PREFABS
            GameObject anchorObject = new GameObject($"{prefab.name} Anchor");
            anchorObject.transform.SetParent(transform);

            // CREATE & ADD PREFAB'S COMPONENT TO THE POOL
            poolDict.Add(prefabID, new Queue<Component>());
            for (int i = 0; i < size; ++i)
            {
                GameObject prefabInstantiated = Instantiate(prefab, anchorObject.transform);
                prefabInstantiated.SetActive(false);
                poolDict[prefabID].Enqueue(prefabInstantiated.GetComponent(Type.GetType(componentName)));
            }
        }

        public Component ReuseObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            int prefabID = prefab.GetInstanceID();
            if (!poolDict.ContainsKey(prefabID))
                return null;

            // 1. GET COMPONENT
            Component component = GetComponentInPool(prefabID);

            // 2. RESET IT
            component.transform.position = position;
            component.transform.rotation = rotation;
            component.transform.localScale = prefab.transform.localScale;

            // 3. RETURN IT
            return component;
        }

        private Component GetComponentInPool(int prefabID)
        {
            if (!poolDict.ContainsKey(prefabID))
                return null;

            Component component = poolDict[prefabID].Dequeue();
            poolDict[prefabID].Enqueue(component);

            if (component.gameObject.activeSelf)
                component.gameObject.SetActive(false);

            return component;
        }
    }
}
