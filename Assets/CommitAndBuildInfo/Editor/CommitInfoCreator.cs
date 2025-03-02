using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class CommitInfoCreator : IPreprocessBuildWithReport
{
    const string DestinationPath = "Assets/CommitAndBuildInfo/Runtime/Assets";
    const string CommitInfoFilename = "CommitInfo.asset";

    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        CreateCommitInfo().Forget();
    }

    async static Task CreateCommitInfo()
    {
        if (!Directory.Exists(DestinationPath))
        {
            Directory.CreateDirectory(DestinationPath);
        }

        var commitInfo = AssetDatabase.LoadAssetAtPath<CommitInfo>($"{DestinationPath}/{CommitInfoFilename}");
        if (commitInfo == null)
        {
            commitInfo = ScriptableObject.CreateInstance<CommitInfo>();
            AssetDatabase.CreateAsset(commitInfo, $"{DestinationPath}/{CommitInfoFilename}");
        }

        string gitPath = GetGitPath();

        const int Timeout = 5000;
        commitInfo.CommitHash = (await ProcessUtility.GetStandardOutputFromProcessAsync(gitPath, "rev-parse HEAD", Timeout)).Trim();
        commitInfo.CommitHashShort = (await ProcessUtility.GetStandardOutputFromProcessAsync(gitPath, "rev-parse --short HEAD", Timeout)).Trim();
        if (string.IsNullOrEmpty(commitInfo.CommitHash))
        {
            Debug.LogWarning($"CommitHash query failed.");
        }
        if (string.IsNullOrEmpty(commitInfo.CommitHashShort))
        {
            Debug.LogWarning($"CommitHashShort query failed.");
        }

        var dateTimeString = (await ProcessUtility.GetStandardOutputFromProcessAsync(gitPath, @"show --date=iso --quiet --pretty=format:""%cd"" HEAD", Timeout)).Trim();
        if (!string.IsNullOrEmpty(dateTimeString))
        {
            DateTime dateTime = DateTime.Parse(dateTimeString);
            dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
            commitInfo.Year = dateTime.Year;
            commitInfo.Month = dateTime.Month;
            commitInfo.Day = dateTime.Day;
            commitInfo.Hour = dateTime.Hour;
            commitInfo.Minute = dateTime.Minute;
            commitInfo.Second = dateTime.Second;
        }
        else
        {
            Debug.LogWarning($"Commit date and time query failed.");
        }

        EditorUtility.SetDirty(commitInfo);
        AssetDatabase.SaveAssets();

    }

    [MenuItem("UnityMobileAppBuildTest/Create CommitInfo Test")]
    static void CreateTest()
    {
        CreateCommitInfo().Forget();
    }

    static string GetGitPath()
    {
        if (Application.platform == RuntimePlatform.OSXEditor)
        {
            string[] exePaths =
            {
                "/usr/local/bin/git",
                "/usr/bin/git"
            };
            return exePaths.FirstOrDefault(exePath => File.Exists(exePath));
        }
        return "git";
    }
}

public static class TaskExtensions
{
	public static void Forget(this Task task)
	{
		task.ContinueWith(
			t => Debug.LogException(t.Exception),
			TaskContinuationOptions.OnlyOnFaulted);
	}
}

