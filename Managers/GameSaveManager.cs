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
            // 直接从缓存读取
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
            // 直接从缓存读取
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
            // 直接从缓存读取
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
            // 直接从缓存读取
            return PlayerPrefs.GetString("currentBgm");
        }
        else
        {
            return GameConstants.AudioPaths.BGM1;
        }
    }
    public string LoadWordsFromJson()
    {
        Debug.Log("进入WordJsonText加载并判断path是否存在");
        string path = Application.persistentDataPath + "/cached_words.json";
        // 从 Resources 文件夹加载 JSON
        if (File.Exists(path))
        {
            // 直接从缓存文件读取
            Debug.Log("缓存中存在Application.persistentDataPath + \"/cached_words.json");
            string cachedJson = File.ReadAllText(path);
            return cachedJson;
        }
        else
        {
            Debug.Log("进入WordJsonText加载path不存在,读取原始 JSON");
            // 读取原始 JSON
            TextAsset jsonFile = Resources.Load<TextAsset>("Data/WordJson/WordData");
            if (jsonFile != null)
            {
                string WordJsonText = jsonFile.text;
                // 保存到本地
                File.WriteAllText(path, WordJsonText);
                return WordJsonText;

            }
            else
            {
                Debug.LogError("找不到 WordData.json 文件，请确保它放在 Resources 目录下！");
            }
            return null;
        }
    }
}
