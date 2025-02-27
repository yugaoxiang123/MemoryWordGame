
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private GameObject prefab;
    private string prefabPath;
    private Transform root;
    private Stack<GameObject> inactiveObjects = new Stack<GameObject>();

    public ObjectPool(GameObject prefab, string path, Transform root)
    {
        this.prefab = prefab;
        this.prefabPath = path;
        this.root = root;
    }

    public GameObject Spawn(Vector2 anchoredPosition, Quaternion rotation)
    {
        GameObject obj;
        if (inactiveObjects.Count > 0)
        {
            obj = inactiveObjects.Pop();
            //obj.transform.position = Vector3.zero;
            // **处理 UI 物体的 anchoredPosition**
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = anchoredPosition;
            }
            obj.transform.rotation = rotation;
        }
        else
        {
            obj = GameObject.Instantiate(prefab, Vector3.zero, rotation,root);
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = anchoredPosition;
            }
            obj.AddComponent<PoolItem>().PrefabPath = prefabPath;
        }
        obj.SetActive(true);
        return obj;
    }

    public void Despawn(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(root);
        inactiveObjects.Push(obj);
        Debug.Log("inactiveObjects的数量是"+inactiveObjects.Count);
    }
}

