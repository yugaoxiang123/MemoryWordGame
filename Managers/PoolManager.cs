using UnityEngine;
using System.Collections.Generic;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();
    private GameObject var_WordParent = null;
    private GameObject var_Pool_root;
    public void InitPoolManager()
    {
        var_WordParent = GameObject.Find("UI_Root");
        if (var_WordParent != null)
        {
            Debug.Log("找到物体：" + var_WordParent.name);
        }
        else
        {
            Debug.Log("未找到该物体 var_WordParent.name");
        }
    }
    public GameObject Spawn(string prefabPath, Vector2 anchoredPosition)
    {
        if (!pools.ContainsKey(prefabPath))
        {
            CreatePool(prefabPath);
        }

        return pools[prefabPath].Spawn(anchoredPosition, Quaternion.identity);
    }

    public void Despawn(GameObject obj)
    {
        PoolItem poolItem = obj.GetComponent<PoolItem>();
        if (poolItem != null && pools.ContainsKey(poolItem.PrefabPath))
        {
            pools[poolItem.PrefabPath].Despawn(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    private void CreatePool(string prefabPath)
    {
        GameObject prefab = ResourceManager.Instance.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"找不到预制体: {prefabPath}");
            return;
        }

        if(var_Pool_root == null)
        {
            var_Pool_root = new GameObject($"Pool_{prefab.name}");

        }

        if(var_WordParent!=null) 
        {
            var_Pool_root.transform.SetParent(var_WordParent.transform);
            RectTransform tmp_Pool_Root = var_Pool_root.AddComponent<RectTransform>();
            tmp_Pool_Root.anchoredPosition = Vector2.zero;

        }
        ObjectPool pool = new ObjectPool(prefab, prefabPath, var_Pool_root.transform);
        pools.Add(prefabPath, pool);
    }
}
