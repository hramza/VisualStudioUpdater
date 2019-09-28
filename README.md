# Update your solution targeted visual studio version

Command that let you upgrade the visual studio version for any solution

How to use it:
Run the powershell command: .\InstallPackageTool.ps1 to pack the package and install it in your machine.
After, you should be able to run the command as bellow:

VisualStudioUpdater -p "solution directory" -v "vs version to target: example: VS2019" -l "C#version, example: 7.3"

The powershell uses dotnet commands