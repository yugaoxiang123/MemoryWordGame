using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;

public class GameManager : Singleton<GameManager>
{
    public bool var_IsGamePaused { get; private set; }
    private bool var_IsPopWordPanel = false;
    private bool var_IsPopBgmPanel = false;
    private bool isFristLoadWordListPanel = false;
    private bool isFristLoadAudioListPanel = false;


    // 在GameManager类中添加
    private GameObject tmp_Word_List_Button = null;
    private GameObject tmp_Bgm_Option_Button = null;
    private Dictionary<char, List<string>> categorizedWords = new Dictionary<char, List<string>>();
    private Dictionary<char, GameObject> letterItems = new Dictionary<char, GameObject>();
    private string currentDisplayWord; // 当前显示的单词
    //protected override void Awake()
    //{
    //    base.Awake();
    //    InitializeGame();
    //}

    public void InitializeGame()
    {
        //RecorderManager.Instance.Initialized();
        // 初始化所有管理器
        UIManager.Instance.InitUIRoot();
        AudioManager.Instance.InitAudioSources();
        PoolManager.Instance.InitPoolManager();
        AudioManager.Instance.PlayBGM(GameSaveManager.Instance.LoadCurrentBgm());
        AudioManager.Instance.SetBGMVolume(GameSaveManager.Instance.LoadCurrentBgmValue());
        AudioManager.Instance.SetSFXVolume(GameSaveManager.Instance.LoadCurrentReadValue());
        // 注册全局事件
        EventManager.Instance.AddListener(GameConstants.EventNames.GAME_PAUSED, OnGamePaused);
        EventManager.Instance.AddListener(GameConstants.EventNames.GAME_RESUMED, OnGameResumed);
        EventManager.Instance.AddListener(GameConstants.EventNames.SHOW_WORD_LIST_PANEL, LoadWordListPanel);
        EventManager.Instance.AddListener(GameConstants.EventNames.SHOW_BGM_PANEL, LoadBgmListPanel);
        SpawnWordManager.Instance.InitSpawnWord();
        LoadWordListButton();
        LoadBgmOptionButton();
        LoadFrameUI();
    }

    private void OnGamePaused(object data)
    {
        var_IsGamePaused = true;
        Time.timeScale = 0;
        // 处理暂停逻辑
    }

    private void OnGameResumed(object data)
    {
        SpawnWordManager.Instance.isTouchValid = false; // 确保暂停后恢复时不会误触发
        var_IsGamePaused = false;
        Time.timeScale = 1;
        // 处理恢复逻辑
    }
    private void LoadFrameUI()
    {
        UIManager.Instance.OpenPanel(GameConstants.UIPaths.FRAME);
    }

    private void LoadBgmOptionButton()
    {
        tmp_Bgm_Option_Button = UIManager.Instance.OpenPanel(GameConstants.UIPaths.BGM_OPTION_BUTTON);
        Button tmp_Word_Bgm_Button = tmp_Bgm_Option_Button.GetComponent<Button>();
        tmp_Word_Bgm_Button.onClick.AddListener(
            () => {
                EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_BGM_PANEL);
            });
    }
    private void LoadBgmListPanel(object obj)
    {
        if(var_IsPopWordPanel)
        {
           ResumedWordPanel();
        }
        if(!var_IsPopBgmPanel)
        {
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_PAUSED);
            var_IsPopBgmPanel = true;
            tmp_Bgm_Option_Button = UIManager.Instance.OpenPanel(GameConstants.UIPaths.BGM_LIST_PANEL);
            if(isFristLoadAudioListPanel)
            {
                return;
            }

            // 获取 AudioPaths 类的公共静态字段
            FieldInfo[] fields = typeof(AudioPaths).GetFields(BindingFlags.Public | BindingFlags.Static);
            Transform content = tmp_Bgm_Option_Button.transform.Find("ScrollView/Viewport/Content");

            GameObject BgmValueSliderUI = Instantiate(Resources.Load<GameObject>("Prefabs/BgmValueSlider"), content);
            GameObject WordReadAloudSliderUI = Instantiate(Resources.Load<GameObject>("Prefabs/WordReadAloudSlider"), content);

            Slider tmp_BamSlider = BgmValueSliderUI.GetComponent<Slider>();
            Slider tmp_WordSlider = WordReadAloudSliderUI.GetComponent<Slider>();

            tmp_BamSlider.value = GameSaveManager.Instance.LoadCurrentBgmValue();
            tmp_WordSlider.value = (GameSaveManager.Instance.LoadCurrentReadValue() / 2);

            tmp_BamSlider.onValueChanged.AddListener(ChangeBgmValue);
            tmp_WordSlider.onValueChanged.AddListener(ChangeReadValue);


            int tmp_BgmIndex = 1;
            // 遍历所有字段
            foreach (FieldInfo field in fields)
            {
                // 检查字段是否为 string 类型
                if (field.FieldType == typeof(string))
                {
                    // 获取字段值
                    string tmp_BgmPath = (string)field.GetValue(null);
                    Console.WriteLine($"字段名: {field.Name}, 值: {tmp_BgmPath}");
                    GameObject BgmCategoryItem = Instantiate(Resources.Load<GameObject>("Prefabs/BgmButtonPrefab"), content);
                    // 设置字母标题
                    TextMeshProUGUI headerText = BgmCategoryItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    headerText.text = $"BGM----{tmp_BgmIndex}";
                    Button BgmOptionButton = BgmCategoryItem.GetComponent<Button>();
                    BgmOptionButton.onClick.AddListener(() => {
                        AudioManager.Instance.PlayBGM(tmp_BgmPath);
                        GameSaveManager.Instance.SaveCurrentBgm(tmp_BgmPath);
                        ResumedBgmPanel();
                    }
                    ); ;
                }
                tmp_BgmIndex++;
            }
            isFristLoadAudioListPanel = true;
        }
        else
        {
            ResumedBgmPanel();
        }
    }

    private void ChangeBgmValue(float _bgmValue)
    {
        AudioManager.Instance.SetBGMVolume(_bgmValue);
        GameSaveManager.Instance.SaveCurrentBgmValue( _bgmValue );
    }
    private void ChangeReadValue(float _readValue)
    {
        _readValue *= 2;
        AudioManager.Instance.SetSFXVolume(_readValue);
        GameSaveManager.Instance.SaveCurrentReadValue(_readValue);
    }
    private void LoadWordListButton()
    {
        GameObject tmp_Word_List = UIManager.Instance.OpenPanel(GameConstants.UIPaths.WORD_LIST_BUTTON);
        Button tmp_Word_List_Button = tmp_Word_List.GetComponent<Button>();
        tmp_Word_List_Button.onClick.AddListener(
            () => {
                EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD_LIST_PANEL);
            });
    }

    private void LoadWordListPanel(object data)
    {
        if (var_IsPopBgmPanel)
        {
            ResumedBgmPanel();
        }
        if (!var_IsPopWordPanel)
        {
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_PAUSED);
            var_IsPopWordPanel = true;
            tmp_Word_List_Button = UIManager.Instance.OpenPanel(GameConstants.UIPaths.WORD_LIST_PANEL);

            // 获取当前显示的单词并滚动
            currentDisplayWord = GetCurrentDisplayWord();
            if (isFristLoadWordListPanel)
            {

                ScrollToWord(currentDisplayWord);
                return;
            }
            // 初始化目录
            Transform content = tmp_Word_List_Button.transform.Find("ScrollView/Viewport/Content");
            InitializeWordCategories(content);

            ScrollToWord(currentDisplayWord);
            isFristLoadWordListPanel = true;
        }
        else
        {
            ResumedWordPanel();
        }
    }

    private string GetCurrentDisplayWord()
    {
        return SpawnWordManager.Instance.wordList[SpawnWordManager.Instance.currentIndex];
    }

    public void InitializeWordCategories(Transform contentParent)
    {
        // 按字母分类
        foreach (var word in SpawnWordManager.Instance.wordList)
        {
            if (string.IsNullOrEmpty(word)) continue;
            char firstLetter = char.ToUpper(word[0]);

            if (!categorizedWords.ContainsKey(firstLetter))
                categorizedWords[firstLetter] = new List<string>();

            categorizedWords[firstLetter].Add(word);
        }
        int globalIndex = 0; // 全局索引
        // 按字母顺序生成UI
        foreach (var category in categorizedWords.OrderBy(k => k.Key))
        {
            char letter = category.Key;
            List<string> words = category.Value;
            // 实例化字母分类预制体
            //GameObject categoryItem = UIManager.Instance.OpenPanel("Prefabs/LetterCategoryPrefab");
            //categoryItem.transform.SetParent(contentParent, false);
            GameObject categoryItem = Instantiate(
                Resources.Load<GameObject>("Prefabs/LetterCategoryPrefab"),
                contentParent);

            // 设置字母标题
            TextMeshProUGUI headerText = categoryItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            headerText.text = letter.ToString();
            Transform wordsContainer = categoryItem.transform.GetChild(1);
            // 添加点击事件
            Button headerButton = categoryItem.GetComponent<Button>();
            headerButton.onClick.AddListener(() =>
                ToggleCategory(wordsContainer.gameObject));

            // 生成单词子项
            foreach (string word in words)
            {
                GameObject wordItem = Instantiate(
                    Resources.Load<GameObject>("Prefabs/WordItemPrefab"),
                    wordsContainer);
                int curIndex = globalIndex;
                wordItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = word;
                wordItem.GetComponent<Button>().onClick.AddListener(() =>
                    OnWordSelected(word, curIndex));
                globalIndex++;
            }

            // 默认折叠
            wordsContainer.gameObject.SetActive(false);
            letterItems.Add(letter, categoryItem);
        }
        // 强制刷新初始布局
        //LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        //Canvas.ForceUpdateCanvases();
    }
    // 修改后的 ToggleCategory 方法
    private void ToggleCategory(GameObject container)
    {
        bool isActive = !container.activeSelf;
        container.SetActive(isActive);

        // 强制刷新容器的布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        // 刷新整个 Content 的布局
        Transform content = tmp_Word_List_Button.transform.Find("ScrollView/Viewport/Content");
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }

        // 更新 Canvas
        Canvas.ForceUpdateCanvases();
    }

    // 优化后的滚动协程
    IEnumerator ScrollToPositionCoroutine(GameObject target)
    {
        // 等待两帧确保布局完成
        yield return null;
        yield return null;

        ScrollRect scrollRect = tmp_Word_List_Button.GetComponentInChildren<ScrollRect>();
        if (scrollRect == null) yield break;

        RectTransform content = scrollRect.content;
        RectTransform targetRect = target.GetComponent<RectTransform>();

        // 再次强制刷新布局
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();

        // 计算目标位置（考虑content新高度）
        Vector3 targetPosition = content.InverseTransformPoint(targetRect.position);
        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        // 更精确的滚动位置计算
        float normalizePosition = 1 - Mathf.Clamp01(
            (targetPosition.y - viewportHeight * 0.5f) /
            (contentHeight - viewportHeight)
        );
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizePosition);
        // 使用平滑滚动
        //LeanTween.value(scrollRect.gameObject, scrollRect.verticalNormalizedPosition, normalizePosition, 0.3f)
        //    .setOnUpdate((float val) => {
        //        scrollRect.verticalNormalizedPosition = val;
        //    });
    }

    // 修改后的ScrollToWord方法
    public void ScrollToWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return;

        char targetLetter = char.ToUpper(word[0]);
        if (!letterItems.ContainsKey(targetLetter)) return;

        GameObject categoryItem = letterItems[targetLetter];
        Transform wordsContainer = categoryItem.transform.GetChild(1);

        // 先展开分类
        if (!wordsContainer.gameObject.activeSelf)
        {
            ToggleCategory(wordsContainer.gameObject);
        }

        // 延迟启动滚动
        StartCoroutine(ScrollToPositionAfterExpand(categoryItem, word));
    }

    private IEnumerator ScrollToPositionAfterExpand(GameObject categoryItem, string word)
    {
        // 等待布局完成
        yield return new WaitForEndOfFrame();

        // 找到具体单词项
        Transform wordsContainer = categoryItem.transform.GetChild(1);
        foreach (Transform child in wordsContainer)
        {
            if (child.GetChild(0).GetComponent<TextMeshProUGUI>().text == word)
            {
                StartCoroutine(ScrollToPositionCoroutine(child.gameObject));
                yield break;
            }
        }
    }
    // 在 GameManager 类中添加以下代码
    private void OnWordSelected(string _word,int _index)
    {
        Debug.Log($"{_word}的索引是{_index}");
        EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD, _index);
        ResumedWordPanel();
    }

    private void ResumedWordPanel()
    {
        EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_RESUMED);
        UIManager.Instance.ClosePanel(GameConstants.UIPaths.WORD_LIST_PANEL);
        foreach (GameObject Item in letterItems.Values)
        {
            Item.transform.GetChild(1).gameObject.SetActive(false);
        }
        var_IsPopWordPanel = false;
    }
    private void ResumedBgmPanel()
    {
        EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_RESUMED);
        UIManager.Instance.ClosePanel(GameConstants.UIPaths.BGM_LIST_PANEL);
        var_IsPopBgmPanel = false;
    }
}
