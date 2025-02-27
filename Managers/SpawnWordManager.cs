using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnWordManager : Singleton<SpawnWordManager>
{
    private List<string> wordList = new List<string>();  // �洢�����б�
    private Dictionary<string, string> wordDict;  // �洢���ʺͶ�Ӧ����˼
    private int currentIndex = 0;  // ��¼��ǰ��ʾ�ĵ�������

    private Queue<GameObject> wordQueue=new Queue<GameObject>();
    private int MaxShowWordCount=17;

    private Vector2 weight = new Vector2(-300, 300);
    private Vector2 height = new Vector2(-230, 420);

    private const string audioClipFileName = "Data/WordAudio/{0}";
    private float IntervalTime = 0.8f;
    string prefabPath = "Prefabs/WordPrefab";

    private Vector2 touchStartPos;
    IEnumerator var_fade_word = null;
    //private Coroutine var_fade_word;  // ȷ������һ�� Coroutine ���͵ı���
    public void InitSpawnWord()
    {
        EventManager.Instance.AddListener(GameConstants.EventNames.SHOW_WORD, ShowCurrentWord);
        Debug.Log("��ע��ShowCurrentWord�¼�");
        Debug.Log("����WordJsonText����");
        string tmp_WordJsonText = GameSaveManager.Instance.LoadWordsFromJson();
        Debug.Log("WordJsonText�Ѿ��������");
        Debug.Log(tmp_WordJsonText);
        DeserializeWords(tmp_WordJsonText);
        currentIndex = GameSaveManager.Instance.LoadCurrentIndex();
        EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        Debug.Log("�Ѿ�������GameConstants.EventNames.SHOW_WORD�¼�");
    }


    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float swipeThreshold = Screen.height * 0.1f; // ������ֵΪ��Ļ�߶ȵ� 10%

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float swipeDistance = touch.position.y - touchStartPos.y;

                if (swipeDistance > swipeThreshold)   // �ϻ�
                    NextWord();
                else if (swipeDistance < -swipeThreshold)  // �»�
                    PreviousWord();
            }
        }
    }

    void DeserializeWords(string jsonText)
    {
        var tmp_wordData = JsonConvert.DeserializeObject<Dictionary<string,string>>(jsonText);//ʹ�ò����using Newtonsoft.Json;
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
        // ���� 2��ʹ�� as �� is ������ͣ�����ȫ��
        
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
        Debug.Log("���뷽��ShowCurrentWord");
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

        Debug.Log("��ʼ����Э��SpawnWord��ѭ��");
        for (int i = 0; i < MaxShowWordCount; i++)
        {

            GameObject tmp_WordPrefab = SpawnWordPosition();
            if (tmp_WordPrefab == null)
            {
                Debug.LogError("Failed to spawn prefab.");
                yield break;  // ��ǰ�˳�Э��
            }
            Debug.Log("�ҵ�Ԥ����tmp_WordPrefab");
            // ����Ӷ����Ƿ����
            if (tmp_WordPrefab.transform.childCount < 2)
            {
                Debug.LogError($"Prefab '{tmp_WordPrefab.name}' does not have enough children! Expected 2, but found {tmp_WordPrefab.transform.childCount}.");
                yield break;
            }
            // ��ȡ�Ӷ���� Transform ���
            Transform wordTransform = tmp_WordPrefab.transform.GetChild(0);
            Transform meanTransform = tmp_WordPrefab.transform.GetChild(1);
            // ��ȡ TextMeshPro ���
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

            // ��������߼�
            if (wordQueue.Count > 4)
            {
                Debug.Log("Word queue exceeded 5, despawning oldest word.");
                PoolManager.Instance.Despawn(wordQueue.Dequeue());
            }

            wordQueue.Enqueue(tmp_WordPrefab);
            Debug.Log($"Playing audio for word: {currentWord}");

            yield return new WaitForSeconds(IntervalTime);
            Debug.Log($"�Ѿ�ѭ�����˵�{i}����");
        }
        NextWord();
        yield break; // ��ǰ�˳�Э��
    }
    private GameObject SpawnWordPosition()
    {
        // ��������ߴ�
        int gridSizeX = 200;  // ˮƽ������
        int gridSizeY = 70;  // ��ֱ������

        HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();

        // ��¼���е��ʵ���������
        foreach (GameObject existingWord in wordQueue)
        {
            if (existingWord == null) continue;

            Vector2Int occupiedCell = new Vector2Int(
                Mathf.RoundToInt(existingWord.transform.position.x / gridSizeX),
                Mathf.RoundToInt(existingWord.transform.position.y / gridSizeY)
            );
            occupiedCells.Add(occupiedCell);
        }

        // �����ҵ���������
        Vector3 tmp_Position = Vector3.zero;
        bool foundValidPosition = false;

        for (int i = 0; i < 30; i++) // ��ೢ�� 10 ��
        {
            int tmp_x_figure = Random.Range((int)weight.x, (int)weight.y);
            int tmp_y_figure = Random.Range((int)height.x, (int)height.y);

            Vector2Int newCell = new Vector2Int(tmp_x_figure / gridSizeX, tmp_y_figure / gridSizeY);

            if (!occupiedCells.Contains(newCell)) // ֻѡ��δ��ռ�õ�����
            {
                tmp_Position = new Vector3(tmp_x_figure, tmp_y_figure, 0);
                foundValidPosition = true;
                occupiedCells.Add(newCell); // ��¼������ʵ�λ��
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
    // ���ϻ�������һ���ʣ�
    public void NextWord()
    {
        if (wordList != null && wordList.Count > 0)
        {
            currentIndex = (currentIndex + 1) % wordList.Count;
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        }
        else
        {
            Debug.LogError("wordList Ϊ�գ��޷�ִ�� NextWord()");
        }
    }

    // ���»�������һ���ʣ�
    public void PreviousWord()
    {
        if (wordList != null && wordList.Count > 0)
        {
            currentIndex = (currentIndex - 1 + wordList.Count) % wordList.Count;
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, currentIndex);
        }
    }
}
