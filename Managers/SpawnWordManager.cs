using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnWordManager : Singleton<SpawnWordManager>
{
    private List<string> wordList = new List<string>();  // 存储单词列表
    private Dictionary<string, string> wordDict;  // 存储单词和对应的意思
    private int currentIndex = 0;  // 记录当前显示的单词索引

    private Queue<GameObject> wordQueue=new Queue<GameObject>();
    private int MaxShowWordCount=17;

    private Vector2 weight = new Vector2(-300, 300);
    private Vector2 height = new Vector2(-230, 420);

    private const string audioClipFileName = "Data/WordAudio/{0}";
    private float IntervalTime = 0.8f;
    string prefabPath = "Prefabs/WordPrefab";

    private Vector2 touchStartPos;
    IEnumerator var_fade_word = null;
    //private Coroutine var_fade_word;  // 确保这是一个 Coroutine 类型的变量
    public void InitSpawnWord()
    {
        EventManager.Instance.AddListener(GameConstants.EventNames.SHOW_WORD, ShowCurrentWord);
        Debug.Log("已注册ShowCurrentWord事件");
        Debug.Log("进入WordJsonText加载");
        string tmp_WordJsonText = GameSaveManager.Instance.LoadWordsFromJson();
        Debug.Log("WordJsonText已经加载完毕");
        Debug.Log(tmp_WordJsonText);
        DeserializeWords(tmp_WordJsonText);
        currentIndex = GameSaveManager.Instance.LoadCurrentIndex();
        EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        Debug.Log("已经触发了GameConstants.EventNames.SHOW_WORD事件");
    }


    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float swipeThreshold = Screen.height * 0.1f; // 滑动阈值为屏幕高度的 10%

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float swipeDistance = touch.position.y - touchStartPos.y;

                if (swipeDistance > swipeThreshold)   // 上滑
                    NextWord();
                else if (swipeDistance < -swipeThreshold)  // 下滑
                    PreviousWord();
            }
        }
    }

    void DeserializeWords(string jsonText)
    {
        var tmp_wordData = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonText);//使用插件包using Newtonsoft.Json;
        wordDict = tmp_wordData;
        if (wordDict == null)
        {
            Debug.LogError("Failed to deserialize JSON!");
            return;
        }
        Debug.Log("Deserialization successful! Word count: " + wordDict.Count);
        wordList = new List<string>(wordDict.Keys);
        print(wordList.Count);
    }
    void ShowCurrentWord(object data)
    {
        // 方法 2：使用 as 或 is 检查类型（更安全）
        
        if (data is int intValueSafe)
        {
            Debug.Log("Safely parsed int value: " + intValueSafe);
            currentIndex = intValueSafe;
        }
        else
        {
            Debug.LogError("Data is not an integer!");
        }
        GameSaveManager.Instance.SaveCurrentIndex(currentIndex);
        EmptyWords();
        Debug.Log("进入方法ShowCurrentWord");
        if(var_fade_word!=null)
        {
            StopCoroutine(var_fade_word);
        }
        var_fade_word = SpawnWord();
        StartCoroutine(var_fade_word);
    }


    IEnumerator SpawnWord()
    {
        string currentWord = wordList[currentIndex];
        string currentWordMean = wordDict[currentWord];

        Debug.Log("开始进入协程SpawnWord的循环");
        for (int i = 0; i < MaxShowWordCount; i++)
        {

            GameObject tmp_WordPrefab = SpawnWordPosition();
            if (tmp_WordPrefab == null)
            {
                Debug.LogError("Failed to spawn prefab.");
                yield break;  // 提前退出协程
            }
            Debug.Log("找到预制体tmp_WordPrefab");
            // 检查子对象是否存在
            if (tmp_WordPrefab.transform.childCount < 2)
            {
                Debug.LogError($"Prefab '{tmp_WordPrefab.name}' does not have enough children! Expected 2, but found {tmp_WordPrefab.transform.childCount}.");
                yield break;
            }
            // 获取子对象的 Transform 组件
            Transform wordTransform = tmp_WordPrefab.transform.GetChild(0);
            Transform meanTransform = tmp_WordPrefab.transform.GetChild(1);
            // 获取 TextMeshPro 组件
            TextMeshProUGUI wordText = wordTransform.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI meanText = meanTransform.GetComponent<TextMeshProUGUI>();
            wordText.text = currentWord;
            meanText.text = currentWordMean;
            AudioManager.Instance.PlaySFX(string.Format(audioClipFileName, currentWord));
            if (i == MaxShowWordCount - 1)
            {
                Debug.Log("MaxShowWordCount reached, emptying words and moving to next.");
                EmptyWords();
                RectTransform rectTransform = tmp_WordPrefab.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero;
                }
                yield return new WaitForSeconds(1.0f);
            }

            // 处理队列逻辑
            if (wordQueue.Count > 4)
            {
                Debug.Log("Word queue exceeded 5, despawning oldest word.");
                PoolManager.Instance.Despawn(wordQueue.Dequeue());
            }

            wordQueue.Enqueue(tmp_WordPrefab);
            Debug.Log($"Playing audio for word: {currentWord}");

            yield return new WaitForSeconds(IntervalTime);
            Debug.Log($"已经循环到了第{i}次数");
        }
        NextWord();
        yield break; // 提前退出协程
    }
    private GameObject SpawnWordPosition()
    {
        // 定义网格尺寸
        int gridSizeX = 200;  // 水平方向间距
        int gridSizeY = 70;  // 垂直方向间距

        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

        // 记录已有单词的网格坐标
        foreach (GameObject existingWord in wordQueue)
        {
            if (existingWord == null) continue;

            Vector2Int occupiedCell = new Vector2Int(
                Mathf.RoundToInt(existingWord.transform.position.x / gridSizeX),
                Mathf.RoundToInt(existingWord.transform.position.y / gridSizeY)
            );
            occupiedCells.Add(occupiedCell);
        }

        // 尝试找到空闲网格
        Vector3 tmp_Position = Vector3.zero;
        bool foundValidPosition = false;

        for (int i = 0; i < 30; i++) // 最多尝试 10 次
        {
            int tmp_x_figure = Random.Range((int)weight.x, (int)weight.y);
            int tmp_y_figure = Random.Range((int)height.x, (int)height.y);

            Vector2Int newCell = new Vector2Int(tmp_x_figure / gridSizeX, tmp_y_figure / gridSizeY);

            if (!occupiedCells.Contains(newCell)) // 只选择未被占用的网格
            {
                tmp_Position = new Vector3(tmp_x_figure, tmp_y_figure, 0);
                foundValidPosition = true;
                occupiedCells.Add(newCell); // 记录这个单词的位置
                break;
            }
        }

        if (!foundValidPosition)
        {
            Debug.LogWarning("Failed to find a non-overlapping position after 10 attempts.");
        }

        GameObject tmp_WordPrefab = PoolManager.Instance.Spawn(prefabPath, tmp_Position);

        return tmp_WordPrefab;
    }
    private void EmptyWords()
    {
        while (wordQueue.Count > 0)
        {
            PoolManager.Instance.Despawn(wordQueue.Dequeue());
        }
    }
    // 向上滑动（下一单词）
    public void NextWord()
    {
        if (wordList != null && wordList.Count > 0)
        {
            currentIndex = (currentIndex + 1) % wordList.Count;
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        }
        else
        {
            Debug.LogError("wordList 为空，无法执行 NextWord()");
        }
    }

    // 向下滑动（上一单词）
    public void PreviousWord()
    {
        if (wordList != null && wordList.Count > 0)
        {
            currentIndex = (currentIndex - 1 + wordList.Count) % wordList.Count;
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        }
    }
}
