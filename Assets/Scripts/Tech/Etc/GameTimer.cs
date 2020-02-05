using UnityEngine;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public class GameTimer : MonoBehaviour
{
    public delegate void SubTimerCallback();

    class SubTimer
    {
        public String name;

        float _originalTime;
        public float time;

        public SubTimerCallback callBack;

        public bool shouldDestory;
        public bool repeat;
        public bool atStart;

        public SubTimer(String _name, float _time, bool _repeat, bool _atStart, SubTimerCallback _callBack)
        {
            name = _name;
            time = _time;
            _originalTime = _time;
            repeat = _repeat;
            atStart = _atStart;
            callBack = _callBack;

            if (atStart && callBack != null)
                callBack();
        }

        public void Update(float diff)
        {
            if (shouldDestory)
                return;

            time -= diff;
            if (time <= 0)
            {
                if (callBack != null)
                    callBack();

                if (repeat)
                    time = _originalTime;
                else
                    shouldDestory = true;
            }
        }

        public override String ToString()
        {
            return name + "(" + String.Format("{0:0.00}", _originalTime - time) + "/" + _originalTime + ")";
        }
    }

    Dictionary<String, SubTimer> _timerDict = new Dictionary<String, SubTimer>();

	[HideInInspector]
    public String timersString;

    static GameTimer _instance = null;
    public static GameTimer instance
    {
        get
        {
            if (_instance == null)
                _instance = (GameTimer)FindObjectOfType(typeof(GameTimer));

            return _instance;
        }
    }

    public bool Add(String _name, float _time, bool _repeat, bool _atStart, SubTimerCallback _callBack)
    {
        if (!_timerDict.ContainsKey(_name))
        {
            _timerDict.Add(_name, new SubTimer(_name, _time, _repeat, _atStart, _callBack));
            return true;
        }

        return false;
    }

    public bool Remove(String _name)
    {
        if (_timerDict.ContainsKey(_name))
            return _timerDict.Remove(_name);
        else
            return false;
    }

    public int RemoveContaining(String _name)
    {
        int removedCount = 0;
        List<String> toRemove = new List<String>();
        foreach (KeyValuePair<String, SubTimer> keyValue in _timerDict)
        {
            if (keyValue.Key.Contains(_name))
            {
                toRemove.Add(keyValue.Key);
                removedCount++;
            }
        }

        foreach (string key in toRemove)
            _timerDict.Remove(key);

        return removedCount;
    }

    public bool Contains(String _name)
    {
        return _timerDict.ContainsKey(_name);
    }

    void Update()
    {
        if (Time.timeScale <= 0)
            return;

        try
        {
            Dictionary<String, SubTimer> _timerDictCopy = new Dictionary<String, SubTimer>(_timerDict); // voor sync issues, moet iets beters voor zijn...

            List<String> toRemove = new List<String>();

            timersString = "GameTimer timers:\n";
            foreach (KeyValuePair<String, SubTimer> keyValue in _timerDictCopy)
            {
                timersString += keyValue.Value + "\n";

                if (!keyValue.Value.shouldDestory)
                    keyValue.Value.Update(Time.deltaTime);
                else
                    toRemove.Add(keyValue.Key);
            }

            foreach (String key in toRemove)
                _timerDict.Remove(key);
        }
        catch 
		{
        }
    }
}
