using System.IO;
using UnityEngine;

public class GameSaveManager : Singleton<GameSaveManager>
{
    public void SaveCurrentIndex(int _currentIndex)
    {
        PlayerPrefs.SetInt("currentIndex",_currentIndex);
    }
    public int LoadCurrentIndex()
    {
        if (PlayerPrefs.HasKey("currentIndex"))
        {
            // ֱ�Ӵӻ����ȡ
            return PlayerPrefs.GetInt("currentIndex");
        }
        else
        {
            return 0;
        }
    }
    public string LoadWordsFromJson()
    {
        Debug.Log("����WordJsonText���ز��ж�path�Ƿ����");
        string path = Application.persistentDataPath + "/cached_words.json";
        // �� Resources �ļ��м��� JSON
        if (File.Exists(path))
        {
            // ֱ�Ӵӻ����ļ���ȡ
            Debug.Log("�����д���Application.persistentDataPath + \"/cached_words.json");
            string cachedJson = File.ReadAllText(path);
            return cachedJson;
        }
        else
        {
            Debug.Log("����WordJsonText����path������,��ȡԭʼ JSON");
            // ��ȡԭʼ JSON
            TextAsset jsonFile = Resources.Load<TextAsset>("Data/WordJson/WordData");
            if (jsonFile != null)
            {
                string WordJsonText = jsonFile.text;
                // ���浽����
                File.WriteAllText(path, WordJsonText);
                return WordJsonText;

            }
            else
            {
                Debug.LogError("�Ҳ��� WordData.json �ļ�����ȷ�������� Resources Ŀ¼�£�");
            }
            return null;
        }
    }
}
