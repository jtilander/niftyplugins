' Some references:
' http://www.herongyang.com/VBScript/
' http://www.edgewordstraining.co.uk/downloads/howto.pdf
'

'
' Given a visual studio version and a full path to a plugin (C# class name) we can unregister the plugin properly.
'
Sub UnInstallPlugin(ByVal vsVersion, ByVal pluginName)
	Set shell = CreateObject("WScript.Shell")
	
	On Error Resume Next
	installDir8 = shell.RegRead("HKLM\SOFTWARE\Microsoft\VisualStudio\" & vsVersion & "\InstallDir")
	On Error Goto 0
	
	If installDir8 <> "" Then
		commandline = Chr(34) & installDir8 & "devenv.exe" & Chr(34) & " /ResetAddIn " & pluginName & " /command File.Exit"
		'WScript.StdOut.Writeline commandline
		shell.Run commandline, 2, true
	End If
	Set shell = Nothing
End Sub

Sub RemoveAddIn(ByVal pluginName)
	Set shell = CreateObject("WScript.Shell")

	On Error Resume Next
	appData = shell.RegRead("HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders\AppData")
	On Error Goto 0

	If appData <> "" Then
		addinpath = appData & "\Microsoft\MSEnvShared\Addins\" & pluginName & ".Addin"
		'WScript.StdOut.Writeline "About to delete " & addinpath
		
		Set fso = CreateObject("Scripting.FileSystemObject")
		
		On Error Resume Next
		Set file = fso.GetFile(addinpath)
		file.Delete
		On Error Goto 0
		
	End If
	Set shell = Nothing
End Sub

'
' First we need to delete the actual addin file (so that when we call visual studio the plugin can not be loaded, even found, anymore)
'
call RemoveAddIn("NiftyPerforce")
Call UnInstallPlugin("8.0", "Aurora.NiftyPerforce.Connect")
Call UnInstallPlugin("9.0", "Aurora.NiftyPerforce.Connect")
Call UnInstallPlugin("10.0", "Aurora.NiftyPerforce.Connect")
