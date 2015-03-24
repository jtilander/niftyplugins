# Introduction #

Why would you want to run this plugin? The [normal SCC plugin](http://www.perforce.com/perforce/doc.current/manuals/p4plugins/index.html) that comes with the standard perforce install suffers from the fact that it tries to mimic the plugin architecture of the old sourcesafe source control system (as well as some other [issues](http://www.perforce.com/perforce/technotes/note064.html)). Among the things that it causes are slow startup times, polling of server to check for status etc.

This plugin adds a couple of handlers for automatic checkout of files when modified, as well as shortcuts to common operations like edit, view history and timelapse view. Most of the commands can be bound to visual studio keys as well as be accessed through the context menus in the solution explorer.

# Installation #

[Download](http://code.google.com/p/niftyplugins/downloads/list) the .msi package and double click on it. After installation it will show up as one of the items in Tools -> Add in Manager.

# Usage #

This section will discuss various usage scenarios of the plugin.

## Configuration ##

The configuration can be brought up through the toolbar:

![http://niftyplugins.googlecode.com/svn/trunk/Images/toolbar.png](http://niftyplugins.googlecode.com/svn/trunk/Images/toolbar.png)

Click on the rightmost icon and the following dialog should pop up:

![http://niftyplugins.googlecode.com/svn/trunk/Images/config.png](http://niftyplugins.googlecode.com/svn/trunk/Images/config.png)

The different categories are as follows:

  * Connection, this is all related to how you connect to your perforce server.
    * client: this is your perforce client name
    * port: this is the address and port of your perforce server
    * username: this is your perforce username
    * useSystemEnv: if this is set to true, all the above options are ignored in favour of the options obtained through P4CONFIG / or the system registry. Not however that the timelapse view will **still** use the settings above (yeah, internal wierdness of perforce).
  * Operation, this controls the automatic hooks of the plugin
    * autoAdd: automatically add any files you add to the project/solution to perforce
    * autoCheckout: automagically checkout any files that you modify inside the editor (consider using the Aurora.NiftyPerforce.Connect.NiftyPerforceEditAllModified command instead).
    * autoDelete: automatically deletes any files you delete from the project in perforce as well.


## Context menus ##

In the solution explorer there are several context menus that you can use for quickly accessing the perforce commands inside visual studio. For example here is a context menu for a source file:

![http://niftyplugins.googlecode.com/svn/trunk/Images/contextmenu.png](http://niftyplugins.googlecode.com/svn/trunk/Images/contextmenu.png)

You can also multiselect items of the same type and bring up the context menu (useful for multi checkout):

![http://niftyplugins.googlecode.com/svn/trunk/Images/multiselect.png](http://niftyplugins.googlecode.com/svn/trunk/Images/multiselect.png)


## Output ##

The output from the plugin is shown in a separate output window. Here is a sample session (where things go wrong):

![http://niftyplugins.googlecode.com/svn/trunk/Images/output.png](http://niftyplugins.googlecode.com/svn/trunk/Images/output.png)