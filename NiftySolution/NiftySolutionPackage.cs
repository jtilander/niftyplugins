//------------------------------------------------------------------------------
// <copyright file="Command1Package.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.IO;
using Aurora.NiftySolution;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using Aurora;
using EnvDTE;
using System.Reflection;
using System.Windows.Forms;

namespace NiftySolution
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.NoSolution_string)] // Note: the package must be loaded on startup to create and bind commands
    [Guid(NiftySolutionPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class NiftySolutionPackage : Package
    {
        /// <summary>
        /// Package GUID string.
        /// </summary>
        public const string PackageGuidString = "9030b111-6eef-4999-91ba-0dd0b7aa4f37";
        public const string PackageGuidGroup = "f426067e-b699-4dc5-99d7-25977ddce806";

		private Plugin m_plugin;
		private SolutionBuildTimings m_timings;
		private CommandRegistry m_commandRegistry;
		private DebuggerEvents m_debuggerEvents;
        /// <summary>
        /// Initializes a new instance of the <see cref="Command1"/> class.
        /// </summary>
        public NiftySolutionPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

			// Load up the options from file.
			string optionsFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NiftySolution.xml");
			Options options = Options.Load(optionsFileName);

			// Create our main plugin facade.
			DTE2 application = GetGlobalService(typeof(DTE)) as DTE2;
			IVsProfferCommands3 profferCommands3 = base.GetService(typeof(SVsProfferCommands)) as IVsProfferCommands3;
			OleMenuCommandService oleMenuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

			ImageList icons = new ImageList();
			icons.Images.AddStrip(Properties.Resources.Icons);
            m_plugin = new Plugin(application, profferCommands3, icons, oleMenuCommandService, "NiftySolution", "Aurora.NiftySolution.Connect", options);
				
			// Every plugin needs a command bar.
			CommandBar commandBar = m_plugin.AddCommandBar("NiftySolution", MsoBarPosition.msoBarTop);
			m_commandRegistry = new CommandRegistry(m_plugin, commandBar, new Guid(PackageGuidString), new Guid(PackageGuidGroup));

			// Initialize the logging system.
			if(Log.HandlerCount == 0)
			{
				#if DEBUG
				Log.AddHandler(new DebugLogHandler());
				#endif

				Log.AddHandler(new VisualStudioLogHandler(m_plugin));
				Log.Prefix = "NiftySolution";
			}

			// Now we can take care of registering ourselves and all our commands and hooks.
			Log.Debug("Booting up...");
			Log.IncIndent();


			bool doBindings = options.EnableBindings;

			m_commandRegistry.RegisterCommand(doBindings, new QuickOpen(m_plugin, "NiftyOpen"));
			m_commandRegistry.RegisterCommand(doBindings, new ToggleFile(m_plugin, "NiftyToggle"));
			m_commandRegistry.RegisterCommand(doBindings, new CloseToolWindow(m_plugin, "NiftyClose"));
			m_commandRegistry.RegisterCommand(doBindings, new Configure(m_plugin, "NiftyConfigure"));

            if (options.SilentDebuggerExceptions || options.IgnoreDebuggerExceptions)
            {
                m_debuggerEvents = application.Events.DebuggerEvents;
                m_debuggerEvents.OnExceptionNotHandled += new _dispDebuggerEvents_OnExceptionNotHandledEventHandler(OnExceptionNotHandled);
            }

			m_timings = new SolutionBuildTimings(m_plugin);

			Log.DecIndent();
			Log.Debug("Initialized...");
        }

        private void OnExceptionNotHandled(string exceptionType, string name, int code, string description, ref dbgExceptionAction exceptionAction)
        {
            Options options = ((Options)m_plugin.Options);

            if (options.IgnoreDebuggerExceptions)
            {
                exceptionAction = dbgExceptionAction.dbgExceptionActionContinue;
            }
            else if (options.SilentDebuggerExceptions)
            {
                exceptionAction = dbgExceptionAction.dbgExceptionActionBreak;
            }
        }
        #endregion
    }
}
