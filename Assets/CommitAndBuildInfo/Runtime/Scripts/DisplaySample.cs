using System;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class DisplaySample : MonoBehaviour
{
	[SerializeField] BuildInfo _buildInfo;
	[SerializeField] CommitInfo _commitInfo;
	[SerializeField] TextMeshProUGUI _textLeftTop;

    GUIStyle _style;
    string _text; 

    private void Start()
    {
        var utcOffset = GetUtcOffset();
        _text = $"commit {_commitInfo.ToString(utcOffset)}\nbuild {_buildInfo.ToString(utcOffset)}";

        _textLeftTop.text = _text;

        _style = new GUIStyle();
        _style.fontSize = 40;
        _style.normal.textColor = Color.white;
    }

    void OnGUI()
    {
		GUI.Label(new Rect (0, Screen.height - 100, 700, 100), _text, _style);
	}

    int GetUtcOffset()
    {
        var localTime = DateTime.Now;
        return localTime.Hour - TimeZoneInfo.ConvertTimeToUtc(localTime).Hour;
    }

    string RemoveSecond(string text)
    {
        return text.Substring(0, text.Length - 3);
    }

}
