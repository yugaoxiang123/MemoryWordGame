using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Dictionary<string, GameObject> uiPanels = new Dictionary<string, GameObject>();
    private Transform uiRoot;

    public void InitUIRoot()
    {
        GameObject root = new GameObject("UI_Root");
        Canvas canvas = root.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; // 设置缩放模式
        scaler.referenceResolution = new Vector2(1080, 1920); // 示例分辨率
        root.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        uiRoot = root.transform;
        DontDestroyOnLoad(root);
        Debug.Log("UI_Root 初始化完成");
    }

    public GameObject OpenPanel(string panelPath)
    {
        if (uiPanels.ContainsKey(panelPath))
        {
            uiPanels[panelPath].SetActive(true);
            return uiPanels[panelPath];
        }

        GameObject panelPrefab = ResourceManager.Instance.Load<GameObject>(panelPath);
        if (panelPrefab != null)
        {
            GameObject panel = Instantiate(panelPrefab, uiRoot);
            uiPanels.Add(panelPath, panel);
            return panel;
        }

        Debug.LogError($"找不到UI面板: {panelPath}");
        return null;
    }

    public void ClosePanel(string panelPath)
    {
        if (uiPanels.ContainsKey(panelPath))
        {
            uiPanels[panelPath].SetActive(false);
        }
    }
}