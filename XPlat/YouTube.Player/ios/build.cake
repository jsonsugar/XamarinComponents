
#load "../../../common.cake"

var TARGET = Argument ("t", Argument ("target", "Default"));

var IOS_PODS = new List<string> {
	"platform :ios, '6.0'",
	"install! 'cocoapods', :integrate_targets => false",
	"target 'Xamarin' do",
	"pod 'youtube-ios-player-helper', '0.1.6'",
	"end",
};

var buildSpec = new BuildSpec () {
	Libs = new ISolutionBuilder [] {
		new IOSSolutionBuilder {
			SolutionPath = "./source/YouTube.Player.iOS.sln",
			Configuration = "Release",
			BuildsOn = BuildPlatforms.Mac,
			OutputFiles = new [] { 
				new OutputFileCopy {
					FromFile = "./source/YouTube.Player.iOS/bin/Release/YouTube.Player.iOS.dll",
					ToDirectory = "./output/"
				},
			}
		},	
	},

	Samples = new ISolutionBuilder [] {
	},
};

Task ("externals").IsDependentOn ("externals-base").Does (() =>
{
	EnsureDirectoryExists ("./externals");

	if (CocoaPodVersion () < new System.Version (1, 0))
		IOS_PODS.RemoveAt (1);

	FileWriteLines ("./externals/Podfile", IOS_PODS.ToArray ());

	CocoaPodInstall ("./externals", new CocoaPodInstallSettings { NoIntegrate = true });

	BuildXCodeFatLibrary ("./Pods/Pods.xcodeproj", "youtube-ios-player-helper", "youtube-ios-player-helper", null, null, "youtube-ios-player-helper");
});

Task ("clean").IsDependentOn ("clean-base").Does (() =>
{
	DeleteFiles ("./externals/Podfile.lock");
	CleanXCodeBuild ("./Pods/", null);
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);
