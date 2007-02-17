using System;
using EnvDTE;
using EnvDTE80;

namespace Aurora
{
	abstract class CommandBase
    {
        abstract public void OnCommand(DTE2 application, OutputWindowPane pane);
        abstract public bool IsEnabled(DTE2 application);
    }
}
