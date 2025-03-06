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
            Debug.LogError("û���ҵ�SliderTransform");
            return;
        }
        var_Slider = SliderTransform.GetComponent<Slider>();
    }

    // ���� SendMessage ���õķ���
    public void UpdateProgress(float progress)
    {
        Debug.Log("��ǰ����: " + progress);
        if (var_Slider != null)
        {
            var_Slider.value = progress; // ���½�����
        }
    }
}
