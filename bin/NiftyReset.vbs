' Some references:
' http://www.herongyang.com/VBScript/
' http://www.edgewordstraining.co.uk/downloads/howto.pdf
'

'
' Given a visual studio version and a full path to a plugin (C# class name) we can unregister the plugin properly.
'
Sub ResetPlugin(ByVal vsVersion, ByVal pluginName)
	
	Set shell = CreateObject("WScript.Shell")
	
	On Error Resume Next
	vsKey = "HKLM\SOFTWARE\Microsoft\VisualStudio\" & vsVersion & "\InstallDir"
	'Wscript.echo("VSKEY=" & vsKey)
	installDir8 = shell.RegRead(vsKey)
	On Error Goto 0
	
	'Wscript.echo("INSTALLDIR=" & installDir8)
	If installDir8 <> "" Then
		commandline = Chr(34) & installDir8 & "devenv.exe" & Chr(34) & " /ResetAddIn " & pluginName & " /command File.Exit"
		'Wscript.echo("CMD=" & commandline)
		shell.Run commandline, 2, true
	End If
	Set shell = Nothing
End Sub

Call ResetPlugin("8.0", "Aurora.NiftyPerforce.Connect")
Call ResetPlugin("9.0", "Aurora.NiftyPerforce.Connect")
Call ResetPlugin("10.0", "Aurora.NiftyPerforce.Connect")
Call ResetPlugin("8.0", "Aurora.NiftySolution.Connect")
Call ResetPlugin("9.0", "Aurora.NiftySolution.Connect")
Call ResetPlugin("10.0", "Aurora.NiftySolution.Connect")
