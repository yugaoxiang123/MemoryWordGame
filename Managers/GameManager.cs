using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public bool var_IsGamePaused {  get; private set; }
    private bool var_IsPopWordPanel = false;
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        InitializeGame();
    }

    public void InitializeGame()
    {
        // 初始化所有管理器
        UIManager.Instance.InitUIRoot();
        AudioManager.Instance.InitAudioSources();
        PoolManager.Instance.InitPoolManager();

        // 注册全局事件
        EventManager.Instance.AddListener(GameConstants.EventNames.GAME_PAUSED, OnGamePaused);
        EventManager.Instance.AddListener(GameConstants.EventNames.GAME_RESUMED, OnGameResumed);
        EventManager.Instance.AddListener(GameConstants.EventNames.SHOW_WORD_LIST_PANEL, LoadWordListPanel);
        SpawnWordManager.Instance.InitSpawnWord();
        LoadWordListButton();
    }
    private void OnGamePaused(object data)
    {
        var_IsGamePaused = true;
        Time.timeScale = 0;
        // 处理暂停逻辑
    }

    private void OnGameResumed(object data)
    {
        var_IsGamePaused = false;
        Time.timeScale = 1;
        // 处理恢复逻辑
    }

    private void LoadWordListButton()
    {
        GameObject tmp_Word_List = UIManager.Instance.OpenPanel(GameConstants.UIPaths.WORD_LIST_BUTTON);
        Button tmp_Word_List_Button = tmp_Word_List.GetComponent<Button>();
        tmp_Word_List_Button.onClick.AddListener(
            () => { EventManager.Instance.TriggerEvent(GameConstants.EventNames.SHOW_WORD_LIST_PANEL);
            });
    }

    private void LoadWordListPanel(object data)
    {
        if(!var_IsPopWordPanel)
        {
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_PAUSED);
            GameObject tmp_Word_List = UIManager.Instance.OpenPanel(GameConstants.UIPaths.WORD_LIST_PANEL);
            var_IsPopWordPanel = true;
        }
        else
        {
            EventManager.Instance.TriggerEvent(GameConstants.EventNames.GAME_RESUMED);
            UIManager.Instance.ClosePanel(GameConstants.UIPaths.WORD_LIST_PANEL);
            var_IsPopWordPanel = false;
        }
    }
    private void OnDestroy()
    {
        // 清理事件
        //EventManager.Instance.RemoveListener(GameConstants.EventNames.GAME_PAUSED, OnGamePaused);
        //EventManager.Instance.RemoveListener(GameConstants.EventNames.GAME_RESUMED, OnGameResumed);
    }
}
