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


    // ��GameManager�������
    private GameObject tmp_Word_List_Button = null;
    private GameObject tmp_Bgm_Option_Button = null;
    private Dictionary<char, List<string>> categorizedWords = new Dictionary<char, List<string>>();
    private Dictionary<char, GameObject> letterItems = new Dictionary<char, GameObject>();
    private string currentDisplayWord; // ��ǰ��ʾ�ĵ���
    //protected override void Awake()
    //{
    //    base.Awake();
    //    InitializeGame();
    //}

    public void InitializeGame()
    {
        //RecorderManager.Instance.Initialized();
        // ��ʼ�����й�����
        UIManager.Instance.InitUIRoot();
        AudioManager.Instance.InitAudioSources();
        PoolManager.Instance.InitPoolManager();
        AudioManager.Instance.PlayBGM(GameSaveManager.Instance.LoadCurrentBgm());
        AudioManager.Instance.SetBGMVolume(GameSaveManager.Instance.LoadCurrentBgmValue());
        AudioManager.Instance.SetSFXVolume(GameSaveManager.Instance.LoadCurrentReadValue());
        // ע��ȫ���¼�
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
        // ������ͣ�߼�
    }

    private void OnGameResumed(object data)
    {
        SpawnWordManager.Instance.isTouchValid = false; // ȷ����ͣ��ָ�ʱ�����󴥷�
        var_IsGamePaused = false;
        Time.timeScale = 1;
        // ����ָ��߼�
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

            // ��ȡ AudioPaths ��Ĺ�����̬�ֶ�
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
            // ���������ֶ�
            foreach (FieldInfo field in fields)
            {
                // ����ֶ��Ƿ�Ϊ string ����
                if (field.FieldType == typeof(string))
                {
                    // ��ȡ�ֶ�ֵ
                    string tmp_BgmPath = (string)field.GetValue(null);
                    Console.WriteLine($"�ֶ���: {field.Name}, ֵ: {tmp_BgmPath}");
                    GameObject BgmCategoryItem = Instantiate(Resources.Load<GameObject>("Prefabs/BgmButtonPrefab"), content);
                    // ������ĸ����
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

            // ��ȡ��ǰ��ʾ�ĵ��ʲ�����
            currentDisplayWord = GetCurrentDisplayWord();
            if (isFristLoadWordListPanel)
            {

                ScrollToWord(currentDisplayWord);
                return;
            }
            // ��ʼ��Ŀ¼
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
        // ����ĸ����
        foreach (var word in SpawnWordManager.Instance.wordList)
        {
            if (string.IsNullOrEmpty(word)) continue;
            char firstLetter = char.ToUpper(word[0]);

            if (!categorizedWords.ContainsKey(firstLetter))
                categorizedWords[firstLetter] = new List<string>();

            categorizedWords[firstLetter].Add(word);
        }
        int globalIndex = 0; // ȫ������
        // ����ĸ˳������UI
        foreach (var category in categorizedWords.OrderBy(k => k.Key))
        {
            char letter = category.Key;
            List<string> words = category.Value;
            // ʵ������ĸ����Ԥ����
            //GameObject categoryItem = UIManager.Instance.OpenPanel("Prefabs/LetterCategoryPrefab");
            //categoryItem.transform.SetParent(contentParent, false);
            GameObject categoryItem = Instantiate(
                Resources.Load<GameObject>("Prefabs/LetterCategoryPrefab"),
                contentParent);

            // ������ĸ����
            TextMeshProUGUI headerText = categoryItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            headerText.text = letter.ToString();
            Transform wordsContainer = categoryItem.transform.GetChild(1);
            // ��ӵ���¼�
            Button headerButton = categoryItem.GetComponent<Button>();
            headerButton.onClick.AddListener(() =>
                ToggleCategory(wordsContainer.gameObject));

            // ���ɵ�������
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

            // Ĭ���۵�
            wordsContainer.gameObject.SetActive(false);
            letterItems.Add(letter, categoryItem);
        }
        // ǿ��ˢ�³�ʼ����
        //LayoutRebuilder.ForceRebuildLayoutImmediate(contentParent.GetComponent<RectTransform>());
        //Canvas.ForceUpdateCanvases();
    }
    // �޸ĺ�� ToggleCategory ����
    private void ToggleCategory(GameObject container)
    {
        bool isActive = !container.activeSelf;
        container.SetActive(isActive);

        // ǿ��ˢ�������Ĳ���
        LayoutRebuilder.ForceRebuildLayoutImmediate(container.GetComponent<RectTransform>());

        // ˢ������ Content �Ĳ���
        Transform content = tmp_Word_List_Button.transform.Find("ScrollView/Viewport/Content");
        if (content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
        }

        // ���� Canvas
        Canvas.ForceUpdateCanvases();
    }

    // �Ż���Ĺ���Э��
    IEnumerator ScrollToPositionCoroutine(GameObject target)
    {
        // �ȴ���֡ȷ���������
        yield return null;
        yield return null;

        ScrollRect scrollRect = tmp_Word_List_Button.GetComponentInChildren<ScrollRect>();
        if (scrollRect == null) yield break;

        RectTransform content = scrollRect.content;
        RectTransform targetRect = target.GetComponent<RectTransform>();

        // �ٴ�ǿ��ˢ�²���
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        Canvas.ForceUpdateCanvases();

        // ����Ŀ��λ�ã�����content�¸߶ȣ�
        Vector3 targetPosition = content.InverseTransformPoint(targetRect.position);
        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        // ����ȷ�Ĺ���λ�ü���
        float normalizePosition = 1 - Mathf.Clamp01(
            (targetPosition.y - viewportHeight * 0.5f) /
            (contentHeight - viewportHeight)
        );
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalizePosition);
        // ʹ��ƽ������
        //LeanTween.value(scrollRect.gameObject, scrollRect.verticalNormalizedPosition, normalizePosition, 0.3f)
        //    .setOnUpdate((float val) => {
        //        scrollRect.verticalNormalizedPosition = val;
        //    });
    }

    // �޸ĺ��ScrollToWord����
    public void ScrollToWord(string word)
    {
        if (string.IsNullOrEmpty(word)) return;

        char targetLetter = char.ToUpper(word[0]);
        if (!letterItems.ContainsKey(targetLetter)) return;

        GameObject categoryItem = letterItems[targetLetter];
        Transform wordsContainer = categoryItem.transform.GetChild(1);

        // ��չ������
        if (!wordsContainer.gameObject.activeSelf)
        {
            ToggleCategory(wordsContainer.gameObject);
        }

        // �ӳ���������
        StartCoroutine(ScrollToPositionAfterExpand(categoryItem, word));
    }

    private IEnumerator ScrollToPositionAfterExpand(GameObject categoryItem, string word)
    {
        // �ȴ��������
        yield return new WaitForEndOfFrame();

        // �ҵ����嵥����
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
    // �� GameManager ����������´���
    private void OnWordSelected(string _word,int _index)
    {
        Debug.Log($"{_word}��������{_index}");
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
