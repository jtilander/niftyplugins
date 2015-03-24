# Introduction #

A collection of frequently asked questions.


## How do I reset the plugins? ##

Sometimes visual studio manages to mess up the toolbars, or the installation failed or something else goes wrong. There is a way out for this, you can call visual studio from the command line and ask it to reset every setting regarding a particular plugin. In order to reset NiftyPerforce for example, you type this:

```
devenv /ResetAddin Aurora.NiftyPerforce.Connect
```

You can also do this automatically for all the Nifty plugins by running [this](http://code.google.com/p/niftyplugins/source/browse/trunk/bin/NiftyReset.vbs) script.


## Where can I get the necessary perforce software? ##

You need the p4win component. Unfortunately it's been discontinued and not readily available from the main perforce webpage, but you can still download the latest released version from the ftp:

  * [p4win 64 bit](ftp://ftp.perforce.com/perforce/r08.1/bin.ntx64/p4winst64.exe)
  * [p4win 32 bit](ftp://ftp.perforce.com/perforce/r08.1/bin.ntx86/p4winst.exe)

You also need the latest visual client, which you can download [here](http://www.perforce.com/perforce/downloads/products/p4v.html).


## Where can I get binaries of the experimental line? ##

I usually check in binary installers for the very latest bleeding edge. This is mostly useful for testing, but you can download these versions and install them if you are brave from here:

  * [NiftyPerforce](http://niftyplugins.googlecode.com/svn/trunk/Build/Experimental_NiftyPerforce.msi)
  * [NiftySolution](http://niftyplugins.googlecode.com/svn/trunk/Build/Experimental_NiftySolution.msi)



## How can I setup my environment to debug the plugins? ##


A tip for how to debug the plugins:

  * Make sure that you uninstall any niftyperforce plugin you have on the system beforehand.
  * Let's assume that you've got the source synced to c:\work\niftyplugins (yeah, I've got it there)
  * Copy the file `c:\work\niftyplugins\NiftyPerforce\NiftyPerforce.Addin` to `c:\Documents and Settings\<Username>\My Documents\Visual Studio 2005\Addins`
  * Edit the file so that it point's back at whatever the working copy will spit out. It's a regular xml file so you just need to change the assembly tag to read:

```
<Assembly>c:\work\niftyplugins\NiftyPerforce\bin\NiftyPerforce.dll</Assembly>
```

  * Go to Visual Studio and make sure that plugins are not loaded automatically at startup, this to prevent that the devenv that you're editing the niftyperforce code in locks the binary and then you can't overwrite it.
  * Shut all visual studios down.
  * Open up the NiftyPlugins.sln and set the NiftyPerforce as the startup project.
  * Give the debug line `"<path to devenv> <solutionfile>"` in the properties dialog
  * You can now set breakpoints, single step etc.