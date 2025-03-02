using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder
{
	const string AppName = "Hoge";

    [MenuItem("UnityMobileAppBuildTest/Build/Development Build")]
    public static void DevelopmentBuild()
    {
        Build(isDevelopmentBuild: true);
        Debug.Log("Development build done.");
    }

    [MenuItem("UnityMobileAppBuildTest/Build/Release Build")]
    public static void ReleaseBuild()
    {
        Build(isDevelopmentBuild: false);
		Debug.Log("Release build done.");
	}

    public static void Build()
    {
#if DEVELOPMENT_BUILD
        DevelopmentBuild();
#else
        ReleaseBuild();
#endif
	}

	public static void Build(bool isDevelopmentBuild)
	{
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/Scenes/SampleScene.unity" };

#if UNITY_IOS
		SupportedPlatform supportedPlatform = SupportedPlatform.iOS;
#elif UNITY_ANDROID
		SupportedPlatform supportedPlatform = SupportedPlatform.Android;
#elif UNITY_EDITOR_WIN
		SupportedPlatform supportedPlatform = SupportedPlatform.StandaloneWindows64;
#endif
		PrepareBuild(supportedPlatform, ref buildPlayerOptions);

        buildPlayerOptions.options = isDevelopmentBuild ? (BuildOptions.Development | BuildOptions.AllowDebugging) : BuildOptions.None;

        BuildReport buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

		if (buildReport.summary.result == BuildResult.Succeeded)
		{
			Debug.Log($"Succeeded: size:{buildReport.summary.totalSize} bytes");
			EditorApplication.Exit(0);
		}
		else
		{
			Debug.LogError( "Error: ");
			EditorApplication.Exit(1);
		}
	}

	enum SupportedPlatform
	{
		iOS,
		Android,
		StandaloneWindows64,
	}

	static void PrepareBuild(SupportedPlatform supportedPlatform, ref BuildPlayerOptions options)
	{
		switch (supportedPlatform)
		{
			case SupportedPlatform.iOS:
				{
					var outputFile = Application.dataPath + "/../XcodeProject";
					if (Directory.Exists(outputFile))
					{
						Directory.Delete(outputFile, true);
					}
					options.locationPathName = outputFile;
					options.target = BuildTarget.iOS;
				}
				break;
			case SupportedPlatform.Android:
				{
					var outputFile = Application.dataPath + $"/../Build/Android/{AppName}.apk";
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}
					options.locationPathName = outputFile;
					options.target = BuildTarget.Android;
				}
				break;

			case SupportedPlatform.StandaloneWindows64:
				{
					var outputFile = Application.dataPath + $"/../Build/Windows/{AppName}.exe";
					if (File.Exists(outputFile))
					{
						File.Delete(outputFile);
					}
					options.locationPathName = outputFile;
					options.target = BuildTarget.StandaloneWindows64;
				}
				break;
		}
	}

	public static void iPhoneDevelopmentBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
		DevelopmentBuild();
		Debug.Log("iPhone Development build done.");
	}

	public static void iPhoneReleaseBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
		ReleaseBuild();
		Debug.Log("iPhone Release build done.");
	}

	public static void AndroidDevelopmentBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		DevelopmentBuild();
		Debug.Log("Android Development build done.");
	}

	public static void AndroidReleaseBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
		ReleaseBuild();
		Debug.Log("Android Release build done.");
	}

	public static void WindowsDevelopmentBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
		DevelopmentBuild();
		Debug.Log("Windows Development build done.");
	}

	public static void WindowsReleaseBuild()
	{
		EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
		ReleaseBuild();
		Debug.Log("Windows Release build done.");
	}
}

