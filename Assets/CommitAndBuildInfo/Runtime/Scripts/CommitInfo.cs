using System;
using UnityEngine;
using System.Globalization;

public class CommitInfo : ScriptableObject
{
    public string CommitHash;
    public string CommitHashShort;
    public int Year;
    public int Month;
    public int Day;
    public int Hour;
    public int Minute;
    public int Second;

    public override string ToString()
    {
        var dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second).ToString(CultureInfo.CurrentCulture);
        return $"{CommitHashShort}@{dateTime}";
    }

    public string ToString(double utcOffset)
    {
        var dateTime = new DateTime(Year, Month, Day, Hour, Minute, Second).AddHours(utcOffset).ToString(CultureInfo.CurrentCulture);
        return $"{CommitHashShort}@{dateTime}";
    }
}