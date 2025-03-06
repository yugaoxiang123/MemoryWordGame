using UnityEngine;
using System.Collections;
using System;

public class SceneManager : Singleton<SceneManager>
{
    private bool isLoading = false;

    protected override void Awake()
    {
        base.Awake();
        LoadScene("WordScene",() => { GameManager.Instance.InitializeGame(); });
    }
    public void LoadScene(string sceneName, Action onComplete = null)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadSceneAsync(sceneName, onComplete));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, Action onComplete)
    {
        isLoading = true;

        // 显示加载界面
        GameObject loadingPanel = UIManager.Instance.OpenPanel(GameConstants.UIPaths.LOADING_PANEL);
        if (loadingPanel == null )
        {
            Debug.Log("loadingPanel没有找到");
            yield break;
        }
        // 异步加载场景
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        float minLoadTime = 1f;
        float elapsedTime = 0f;
        while (!asyncLoad.isDone || elapsedTime < minLoadTime)
        {
            elapsedTime += Time.deltaTime;
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 1.0f);
            float displayProgress = Mathf.Min(realProgress, elapsedTime / minLoadTime);
            Debug.Log($"加载进度: {realProgress} (显示进度: {displayProgress})");
            if (loadingPanel != null)
            {
                loadingPanel.SendMessage("UpdateProgress", displayProgress, SendMessageOptions.DontRequireReceiver);
            }
            yield return null;
        }
    
        // 等待一帧，确保所有内容都已准备就绪
        //yield return new WaitForSeconds(1.0f);
        //asyncLoad.allowSceneActivation = true;

        // 关闭加载界面
        UIManager.Instance.ClosePanel(GameConstants.UIPaths.LOADING_PANEL);

        isLoading = false;
        onComplete?.Invoke();
    }
}