using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager>
{
    private Dictionary<string, Action<object>> var_EventDictionary = new Dictionary<string, Action<object>>();

    public void AddListener(string _eventName, Action<object> _listener)
    {
        if(!var_EventDictionary.ContainsKey(_eventName))
        {
            var_EventDictionary[_eventName] = null;
        }
        var_EventDictionary[_eventName] += _listener;
    }
    public void RemoveListener(string _eventName,Action<object> _listener)
    {
        {
            if(var_EventDictionary.ContainsKey(_eventName))
                var_EventDictionary[_eventName] -= _listener;
        }
    }

    // 触发事件
    public void TriggerEvent(string _eventName, object _data = null)
    {
        if (var_EventDictionary.ContainsKey(_eventName))
        {
            Debug.Log($"包含函数key{_eventName}");
            var_EventDictionary[_eventName]?.Invoke(_data);
        }
        else
        {
            Debug.Log($"不包含函数key{_eventName}");
        }
    }

    // 清除所有事件
    public void ClearEvents()
    {
        var_EventDictionary.Clear();
    }
}