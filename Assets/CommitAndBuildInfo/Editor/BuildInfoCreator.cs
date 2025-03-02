using UnityEngine;
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildInfoCreator : IPreprocessBuildWithReport
{
    const string DestinationPath = "Assets/CommitAndBuildInfo/Runtime/Assets";
    const string BuildInfoFilename = "BuildInfo.asset";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        DateTime dateTime = TimeZoneInfo.ConvertTimeFromUtc(report.summary.buildStartedAt, TimeZoneInfo.Utc);
        CreateBuildTimestamp(dateTime);
    }

    static void CreateBuildTimestamp(DateTime dateTime)
    {
        if (!Directory.Exists(DestinationPath))
        {
            Directory.CreateDirectory(DestinationPath);
        }

        var buildInfo = AssetDatabase.LoadAssetAtPath<BuildInfo>($"{DestinationPath}/{BuildInfoFilename}");
        if (buildInfo == null)
        {
            buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
            AssetDatabase.CreateAsset(buildInfo, $"{DestinationPath}/{BuildInfoFilename}");
        }

        buildInfo.Year = dateTime.Year;
        buildInfo.Month = dateTime.Month;
        buildInfo.Day = dateTime.Day;
        buildInfo.Hour = dateTime.Hour;
        buildInfo.Minute = dateTime.Minute;
        buildInfo.Second = dateTime.Second;

        EditorUtility.SetDirty(buildInfo);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("UnityMobileAppBuildTest/Create BuildTimestamp Test")]
    static void CreateTest()
    {
        DateTime dateTime = DateTime.Now;
        CreateBuildTimestamp(dateTime);
    }
}