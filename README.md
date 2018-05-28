# Installation

As this project makes heavy use of OptiTrack, Leap Motion and Microsoft Kinect in specific versions, you have to install some software to get things working.

## Requirements

- .NET Framework 4.5.x
- Microsoft Kinect SDK 1.8
- Leap Motion Developer Kit (Orion) 3.2.1
- Microsoft Visual Studio 2017 Enterprise (only for gitlab runner)

The leap motion requires the .NET Framework 4.5.x! 4.6 and higher might work but are not tested. For Windows 10 users the kinect sdk has to be installed in windows 7 compatibility mode! Otherwise runtime errors are going to occure. To get the frames from the Leap Motion, you need to install the Leap Motion Orion Software on your machine, for building or running the solution it is not necessary. If you want use a gitlab runner instance on your machine to execute continuous integration Visual Studio 2017 Enterprise is required. Take a look at the `.gitlab-ci.yml`. There is an absolute path to the `MSBuild.exe`.

## GitLab Runner

On the GitLab is a [.NET project](https://git.uni-due.de/VinteR/TheApplication) created which has CI configurations. Unfortunately, CI jobs cannot run on the GitLab because an installed Visual Studio 2017 Enterprise is required. Currently, the jobs are running on different developer machines. A GitLab runner has been installed and registered for this purpose. This was done according to the following instructions:

1. [Installation](https://docs.gitlab.com/runner/install/windows.html)
2. [Registration](https://docs.gitlab.com/runner/register/index.html#windows)

Registration requires a token to authorize the Runner to send results back to GitLab. In the[CI Settings](https://git.uni-due.de/VinteR/TheApplication/settings/ci_cd) -> Runner Settings -> Specific Runners -> Setup a specific Runner manually you can see an installation manual containing the token.

If the Runner is successfully installed and registered, the configuration file `config.toml` must be added to the gitlab runner exe. If necessary, this is already available. For the Runner to run from the Powershell, the file must be modified:

```ini
executor = "shell"
shell = "powershell"
```

To execute the runner locally execute the following command in a powershell instance.

```console
"C:\GitLab-Runner\gitlab-runner.exe" exec shell build --shell=powershell
```

This command must be executed inside the folder that contains the solution (`VinteR.sln`).

# Writing tests

Unit and integration tests can be written with the use of [NUnit](https://github.com/nunit/docs/wiki/). Write your tests inside the `VinteR.Tests` project. All test classes must be annotated with the `[TestFixture]` attribute. Test methods must also be annotated with the `[Test]` attribute. All tests are executed on the continuous integration system. If you want to execute your tests on your local machine open a terminal window and navigate to the `TheApplication` folder. To run all tests you have to make a copy of the `vinter.config.json` and `vinter.config.schema.json` otherwise the `TestVinterConfigurationService` failes.

```console
# copy configuration
xcopy VinteR\vinter.config.json .
xcopy VinteR\vinter.config.schema.json .

# run the tests
VinteR\packages\NUnit.ConsoleRunner.3.8.0\tools\nunit3-console.exe .\VinteR.Tests\bin\x64\Release\VinteR.Tests.dll
```
