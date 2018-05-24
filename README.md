# Installation

As this project makes heavy use of OptiTrack, Leap Motion and Microsoft Kinect in specific versions you have to install some software to get things working.

## Requirements

- .NET Framework 4.5.x
- Microsoft Kinect SDK 1.8
- Microsoft Visual Studio 2017 Enterprise (only for gitlab runner)

The leap motion requires 4.5.x! 4.6 and higher might work but are not tested. For Windows 10 users the kinect sdk has to be installed in windows 7 compatibility mode! Otherwise runtime errors are going to occure. If you want use a gitlab runner instance on your machine to execute continuous integration Visual Studio 2017 Enterprise is required. Take a look at the `.gitlab-ci.yml`. There is an absolute path to the `MSBuild.exe`.

## GitLab Runner

On the GitLab is a [.NET project](https://git.uni-due.de/VinteR/TheApplication) created which has CI configurations. Unfortunately, CI jobs cannot run on the GitLab because an installed Visual Studio 2017 Enterprise is required. Currently, the jobs are running on different developer machines. A GitLab runner has been installed and registered for this purpose. This was done according to the following instructions:

1.[Installation](https://docs.gitlab.com/runner/install/windows.html)
2.[Registration](https://docs.gitlab.com/runner/register/index.html#windows)

Registration requires a token to authorize the Runner to send results back to GitLab. In the[CI Settings](https://git.uni-due.de/VinteR/TheApplication/settings/ci_cd) -> Runner Settings -> Specific Runners -> Setup a specific Runner manually you can see an installation manual containing the token.

If the Runner is successfully installed and registered, the configuration file `config.toml` must be added to the gitlab runner exe. If necessary, this is already available. For the Runner to run from the Powershell, the file must be modified:

> executor = "shell"
> shell = "powershell"