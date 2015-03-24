# Introduction #

Originally NiftySolution was born out of the need to quickly open any file in the solution without touching the mouse. There are some ways to do this already, either you can open the command console in Visual Studio and type "of start\_of\_filename" and then press return, or you can install other plugins like Visual Assist to do this for you. None of the above solutions were as handy as I wanted to I wrote my own plugin a long time ago to do this for me. As it happens, I've added a couple of other features into the plugin, but the main goal is to keep it as small as possible and still be useful for common programming tasks inside visual studio.


## QuickOpen ##

So this is the main feature. You can bind the command "Aurora.NiftySolution.Connect.NiftyOpen" to any key combination using the standard options dialog in visual studio (I tend to use ctrl-o). Once you've done this you should be able to invoke the open dialog. Once the dialog is opened, you can start typing any part of the filename in question and it should start filtering the filenames found. For example, if you notice below I want to find all the .Designer.cs files in the solution.

![http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/quickopen.png](http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/quickopen.png)

You can additionally also instruct the dialog to also scan a fixed set of directories to populate the open dialog. This is very handy if you have include files (like system includes) that you often want to open, but are not in the solution. You can add these directories by opening up the configuration dialog and add a semi colon delimited list in the "ExtraSearchDirs" option.


![http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/includedirs.png](http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/includedirs.png)

## SolutionTimes ##

By default the plugin will also add hooks for timing each full build of the solution. The normal build time printout only does it for the individual projects, but usually I want to know what the total time is. Once you've installed the plugin you should have a printout at the bottom of the build stating the total time it took.

![http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/totalsolutiontime.png](http://niftyplugins.googlecode.com/svn/trunk/Images/NiftySolution/totalsolutiontime.png)


## Toggle header / body ##

The command "Aurora.NiftySolution.Connect.NiftyToggle" can be bound to a key combo (I use ctrl-enter) to toggle between .h and .cpp file. It's slightly smarter than that, it tries to cycle through the .inl, .m, .mm files as well. If it can not find the file on the filesystem next to each other, then it tries to find a similar name in the list of opened documents in visual studio.

## Close tools windows ##

The command "Aurora.NiftySolution.Connect.NiftyClose" tries to close all the little extra "tool" windows, so that you will be left with only the source code window. I use this frequently when I'm working on a smaller monitor and have large source files, just go reclaim screen real estate.