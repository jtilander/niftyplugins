// Copyright (C) 2006-2015 Jim Tilander. See COPYING for and README for more details.
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
using EnvDTE;
using System.Resources;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.IO;
using System.Reflection;
using Aurora.NiftyPerforce;
using Aurora;
using System.Windows.Forms;

namespace NiftyPerforce
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
	[Guid(NiftyPerforcePackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class NiftyPerforcePackage : Package
	{
		/// <summary>
		/// Package GUID string.
		/// </summary>
		public const string PackageGuidString = "47a20418-f762-4ce9-a34d-a8c96611a172";
		public const string PackageGuidGroup = "d8ef26a8-e88c-4ad1-85fd-ddc48a207530";

		private Plugin m_plugin = null;
		private CommandRegistry m_commandRegistry = null;

		public NiftyPerforcePackage()
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
			string optionsFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "NiftyPerforce.xml");
			Config options = Config.Load(optionsFileName);

			// Every plugin needs a command bar.
			DTE2 application = GetGlobalService(typeof(DTE)) as DTE2;
			IVsProfferCommands3 profferCommands3 = base.GetService(typeof(SVsProfferCommands)) as IVsProfferCommands3;
			OleMenuCommandService oleMenuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

			ImageList icons = new ImageList();
			icons.Images.AddStrip(Properties.Resources.Icons);
			m_plugin = new Plugin(application, profferCommands3, icons, oleMenuCommandService, "NiftyPerforce", "Aurora.NiftyPerforce.Connect", options);

//			Cleanup(); // TODO: remove this line

			CommandBar commandBar = m_plugin.AddCommandBar("NiftyPerforce", MsoBarPosition.msoBarTop);
			m_commandRegistry = new CommandRegistry(m_plugin, commandBar, new Guid(PackageGuidString), new Guid(PackageGuidGroup));

			// Initialize the logging system.
			if (Log.HandlerCount == 0)
			{
#if DEBUG
				Log.AddHandler(new DebugLogHandler());
#endif
				Log.AddHandler(new VisualStudioLogHandler(m_plugin));
				Log.Prefix = "NiftyPerforce";
			}

			// Now we can take care of registering ourselves and all our commands and hooks.
			Log.Debug("Booting up...");
			Log.IncIndent();

			bool doContextCommands = true;

			bool doBindings = options.EnableBindings;
			m_commandRegistry.RegisterCommand(doBindings, new NiftyConfigure(m_plugin, "NiftyConfig"), true);

			m_commandRegistry.RegisterCommand(doBindings, new P4EditModified(m_plugin, "NiftyEditModified"), true);

			m_commandRegistry.RegisterCommand(doBindings, new P4EditItem(m_plugin, "NiftyEdit"), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4EditItem(m_plugin, "NiftyEditItem"), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4EditSolution(m_plugin, "NiftyEditSolution"), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4DiffItem(m_plugin, "NiftyDiff"));
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4DiffItem(m_plugin, "NiftyDiffItem"), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4DiffSolution(m_plugin, "NiftyDiffSolution"), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4RevisionHistoryItem(m_plugin, "NiftyHistory", false), true);
			m_commandRegistry.RegisterCommand(doBindings, new P4RevisionHistoryItem(m_plugin, "NiftyHistoryMain", true), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevisionHistoryItem(m_plugin, "NiftyHistoryItem", false), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevisionHistoryItem(m_plugin, "NiftyHistoryItemMain", true), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevisionHistorySolution(m_plugin, "NiftyHistorySolution"), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4TimeLapseItem(m_plugin, "NiftyTimeLapse", false), true);
			m_commandRegistry.RegisterCommand(doBindings, new P4TimeLapseItem(m_plugin, "NiftyTimeLapseMain", true), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4TimeLapseItem(m_plugin, "NiftyTimeLapseItem", false), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4TimeLapseItem(m_plugin, "NiftyTimeLapseItemMain", true), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4RevisionGraphItem(m_plugin, "NiftyRevisionGraph", false), true);
			m_commandRegistry.RegisterCommand(doBindings, new P4RevisionGraphItem(m_plugin, "NiftyRevisionGraphMain", true), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevisionGraphItem(m_plugin, "NiftyRevisionGraphItem", false), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevisionGraphItem(m_plugin, "NiftyRevisionGraphItemMain", true), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4RevertItem(m_plugin, "NiftyRevert", false), true);
			m_commandRegistry.RegisterCommand(doBindings, new P4RevertItem(m_plugin, "NiftyRevertUnchanged", true), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevertItem(m_plugin, "NiftyRevertItem", false), false);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4RevertItem(m_plugin, "NiftyRevertUnchangedItem", true), false);

			m_commandRegistry.RegisterCommand(doBindings, new P4ShowItem(m_plugin, "NiftyShow"), true);
			if (doContextCommands) m_commandRegistry.RegisterCommand(doBindings, new P4ShowItem(m_plugin, "NiftyShowItem"), false);

			m_plugin.AddFeature(new AutoAddDelete(m_plugin));
			m_plugin.AddFeature(new AutoCheckoutProject(m_plugin));
			m_plugin.AddFeature(new AutoCheckoutOnBuild(m_plugin));
			m_plugin.AddFeature(new AutoCheckoutTextEdit(m_plugin));
			m_plugin.AddFeature(new AutoCheckoutOnSave(m_plugin));

#if DEBUG
			// Use this to track down event GUIDs.
			//m_plugin.AddFeature(new FindEvents(m_plugin));
#endif

			P4Operations.CheckInstalledFiles();

			AsyncProcess.Init();

			Log.DecIndent();
			Log.Debug("Initialized...");

#if DEBUG
			Log.Info("NiftyPerforce (Debug)");
#else
				//Log.Info("NiftyPerforce (Release)");
#endif
			// Show where we are and when we were compiled...
			Log.Info("I'm running {0} compiled on {1}", Assembly.GetExecutingAssembly().Location, System.IO.File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location));
		}

		/// <summary>
		///  Removes all installed commands and controls for testing purposes.
		/// </summary>
		private void Cleanup()
		{
			IVsProfferCommands3 profferCommands3 = base.GetService(typeof(SVsProfferCommands)) as IVsProfferCommands3;
			RemoveCommandBar("NiftyPerforceCmdBar", profferCommands3);
			RemoveCommandBar("NiftyPerforce", profferCommands3);

			RemoveCommand("NiftyConfig", profferCommands3);
			RemoveCommand("NiftyEditModified", profferCommands3);
			RemoveCommand("NiftyEdit", profferCommands3);
			RemoveCommand("NiftyEditItem", profferCommands3);
			RemoveCommand("NiftyEditSolution", profferCommands3);
			RemoveCommand("NiftyDiff", profferCommands3);
			RemoveCommand("NiftyDiffItem", profferCommands3);
			RemoveCommand("NiftyDiffSolution", profferCommands3);
			RemoveCommand("NiftyHistory", profferCommands3);
			RemoveCommand("NiftyHistoryMain", profferCommands3);
			RemoveCommand("NiftyHistoryItem", profferCommands3);
			RemoveCommand("NiftyHistoryItemMain", profferCommands3);
			RemoveCommand("NiftyHistorySolution", profferCommands3);
			RemoveCommand("NiftyTimeLapse", profferCommands3);
			RemoveCommand("NiftyTimeLapseMain", profferCommands3);
			RemoveCommand("NiftyTimeLapseItem", profferCommands3);
			RemoveCommand("NiftyTimeLapseItemMain", profferCommands3);
			RemoveCommand("NiftyRevisionGraph", profferCommands3);
			RemoveCommand("NiftyRevisionGraphMain", profferCommands3);
			RemoveCommand("NiftyRevisionGraphItem", profferCommands3);
			RemoveCommand("NiftyRevisionGraphItemMain", profferCommands3);
			RemoveCommand("NiftyRevert", profferCommands3);
			RemoveCommand("NiftyRevertItem", profferCommands3);
			RemoveCommand("NiftyRevertUnchanged", profferCommands3);
			RemoveCommand("NiftyRevertUnchangedItem", profferCommands3);
			RemoveCommand("NiftyShow", profferCommands3);
			RemoveCommand("NiftyShowItem", profferCommands3);
		}

		private void RemoveCommand(string name, IVsProfferCommands3 profferCommands3)
		{
			try
			{
				Command cmd = m_plugin.Commands.Item(name, -1);
				if (cmd != null)
				{
					profferCommands3.RemoveNamedCommand(name);
				}
			} catch (Exception) {
			}

			string[] bars = {
				"Project",
				"Item",
				"Easy MDI Document Window",
				"Cross Project Multi Item",
				"Cross Project Multi Project"
				};

			string Absname = m_plugin.Prefix + "." + name;


			foreach (string bar in bars)
			{
				CommandBar b = ((CommandBars)m_plugin.App.CommandBars)[bar];
				if (null != b)
				{
					bool done = false;
					while (!done)
					{
						bool found = false;
						foreach (CommandBarControl ctrl in b.Controls)
						{
							if (ctrl.Caption == name || ctrl.Caption == Absname)
							{
								found = true;
								try
								{
									profferCommands3.RemoveCommandBarControl(ctrl);
								}
								catch (Exception)
								{
								}
								break;
							}
						}
						done = !found;
					}
				}
			}
		}
		
		private void RemoveCommandBar(string name, IVsProfferCommands3 profferCommands3)
		{
			// Remove a command bar and contained controls
			DTE2 dte = GetGlobalService(typeof(DTE)) as DTE2;
			CommandBars commandBars = (CommandBars)dte.CommandBars;
			Commands commands = dte.Commands;
			CommandBar existingCmdBar = null;

			try
			{
				existingCmdBar = commandBars[name];
			}
			catch (Exception)
			{
			}

			if (existingCmdBar != null)
			{
				// Remove all buttons

				while (existingCmdBar.Controls.Count > 0)
				{
					foreach (CommandBarControl ctrl in existingCmdBar.Controls)
					{
						profferCommands3.RemoveCommandBarControl(ctrl);
						break;
					}
				}
			}
			profferCommands3.RemoveCommandBar(existingCmdBar);
		}

		#endregion
	}
}
