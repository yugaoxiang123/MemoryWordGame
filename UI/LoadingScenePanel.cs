using UnityEngine;
using UnityEngine.UI;

public class LoadingScenePanel : MonoBehaviour
{
    // Start is called before the first frame update
    private Slider var_Slider = null;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Transform SliderTransform = transform.GetChild(1);
        if (SliderTransform == null )
        {
            Debug.LogError("没有找到SliderTransform");
            return;
        }
        var_Slider = SliderTransform.GetComponent<Slider>();
    }

    // 接收 SendMessage 调用的方法
    public void UpdateProgress(float progress)
    {
        Debug.Log("当前进度: " + progress);
        if (var_Slider != null)
        {
            var_Slider.value = progress; // 更新进度条
        }
    }
}
