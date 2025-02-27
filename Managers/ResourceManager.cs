using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : Singleton<ResourceManager>
{
    private Dictionary<string, Object> resourceCache = new Dictionary<string, Object>();

    public T Load<T>(string path) where T : Object
    {
        if (resourceCache.ContainsKey(path))
        {
            return resourceCache[path] as T;
        }

        T resource = Resources.Load<T>(path);
        if (resource != null)
        {
            resourceCache[path] = resource;
        }
        return resource;
    }

    public void ClearCache()
    {
        resourceCache.Clear();
        Resources.UnloadUnusedAssets();
    }
}