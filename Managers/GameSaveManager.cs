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
    public void SaveCurrentBgm(string _currentBgmPath)
    {
        PlayerPrefs.SetString("currentBgm", _currentBgmPath);
    }

    public float LoadCurrentBgmValue()
    {
        if (PlayerPrefs.HasKey("currentBgmValue"))
        {
            // ֱ�Ӵӻ����ȡ
            return PlayerPrefs.GetFloat("currentBgmValue");
        }
        else
        {
            return 0.4f;
        }
    }
    public void SaveCurrentBgmValue(float _currentBgmValue)
    {
        PlayerPrefs.SetFloat("currentBgmValue", _currentBgmValue);
    }
    public float LoadCurrentReadValue()
    {
        if (PlayerPrefs.HasKey("currentReadValue"))
        {
            // ֱ�Ӵӻ����ȡ
            return PlayerPrefs.GetFloat("currentReadValue");
        }
        else
        {
            return 1.0f;
        }
    }
    public void SaveCurrentReadValue(float _currentReadValue)
    {
        PlayerPrefs.SetFloat("currentReadValue", _currentReadValue);
    }
    public string LoadCurrentBgm()
    {
        if (PlayerPrefs.HasKey("currentBgm"))
        {
            // ֱ�Ӵӻ����ȡ
            return PlayerPrefs.GetString("currentBgm");
        }
        else
        {
            return GameConstants.AudioPaths.BGM1;
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
