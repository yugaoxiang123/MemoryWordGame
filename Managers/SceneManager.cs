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

        // ��ʾ���ؽ���
        GameObject loadingPanel = UIManager.Instance.OpenPanel(GameConstants.UIPaths.LOADING_PANEL);
        if (loadingPanel == null )
        {
            Debug.Log("loadingPanelû���ҵ�");
            yield break;
        }
        // �첽���س���
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        float minLoadTime = 1f;
        float elapsedTime = 0f;
        while (!asyncLoad.isDone || elapsedTime < minLoadTime)
        {
            elapsedTime += Time.deltaTime;
            float realProgress = Mathf.Clamp01(asyncLoad.progress / 1.0f);
            float displayProgress = Mathf.Min(realProgress, elapsedTime / minLoadTime);
            Debug.Log($"���ؽ���: {realProgress} (��ʾ����: {displayProgress})");
            if (loadingPanel != null)
            {
                loadingPanel.SendMessage("UpdateProgress", displayProgress, SendMessageOptions.DontRequireReceiver);
            }
            yield return null;
        }
    
        // �ȴ�һ֡��ȷ���������ݶ���׼������
        //yield return new WaitForSeconds(1.0f);
        //asyncLoad.allowSceneActivation = true;

        // �رռ��ؽ���
        UIManager.Instance.ClosePanel(GameConstants.UIPaths.LOADING_PANEL);

        isLoading = false;
        onComplete?.Invoke();
    }
}