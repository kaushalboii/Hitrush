using UnityEngine;
using System.Collections.Generic;

namespace Hitrush.World
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int initialSize = 20;
        [SerializeField] private int maxSize = 100;
        [SerializeField] private bool expandable = true;
        
        private Queue<GameObject> pool = new Queue<GameObject>();
        private List<GameObject> activeObjects = new List<GameObject>();
        
        private void Awake()
        {
            Prewarm();
        }
        
        private void Prewarm()
        {
            for (int i = 0; i < initialSize; i++)
            {
                CreateObject();
            }
        }
        
        private GameObject CreateObject()
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
            return obj;
        }
        
        public GameObject Get()
        {
            GameObject obj;
            
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else if (expandable && activeObjects.Count < maxSize)
            {
                obj = CreateObject();
            }
            else
            {
                // Pool exhausted, return oldest active object (recycle)
                if (activeObjects.Count > 0)
                {
                    obj = activeObjects[0];
                    activeObjects.RemoveAt(0);
                    obj.SetActive(false);
                }
                else
                {
                    return null;
                }
            }
            
            obj.SetActive(true);
            activeObjects.Add(obj);
            return obj;
        }
        
        public void Return(GameObject obj)
        {
            if (obj == null) return;
            
            if (activeObjects.Contains(obj))
            {
                activeObjects.Remove(obj);
            }
            
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
        
        public void ReturnAll()
        {
            foreach (var obj in activeObjects)
            {
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                pool.Enqueue(obj);
            }
            activeObjects.Clear();
        }
        
        public int ActiveCount => activeObjects.Count;
        public int PooledCount => pool.Count;
    }
}
