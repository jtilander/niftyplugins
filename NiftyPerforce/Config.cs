// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Aurora
{
	namespace NiftyPerforce
	{
		[Serializable]
		public class Config
		{
			private bool mDirty = false;
			private bool mEnableBindings = false;
			private bool m_autoCheckout = false;
			private bool m_autoCheckoutProject = false;
			private bool m_autoCheckoutOnSave = false;
			private bool m_autoAdd = true;
			private bool m_autoDelete = false;
			private bool m_useSystemConnection = true;
			private string m_port = "";
			private string m_client = "";
			private string m_username = "";

			[XmlIgnore]
			public string mFileName = "";

			[XmlIgnore]
			[BrowsableAttribute(false)]
			public bool Dirty
			{
				get
				{
					return mDirty;
				}
			}
			
			[Category("General"), Description("Register Key Bindings")]
			public bool EnableBindings
			{
				get
				{
					return mEnableBindings;
				}

				set
				{
					mEnableBindings = value;
					mDirty = true;
				}
			}

			[Category("Operation"), Description("Controls if we automagically check out files from perforce upon keypress")]
			public bool autoCheckout
			{
				get { return m_autoCheckout; }
				set { m_autoCheckout = value; mDirty = true; }
			}

			[Category("Operation"), Description("Automatically check out projects on edit properties")]
			public bool autoCheckoutProject
			{
				get { return m_autoCheckoutProject; }
				set { m_autoCheckoutProject = value; mDirty = true; }
			}

			[Category("Operation"), Description("Controls if we automagically check out files from perforce before saving")]
			public bool autoCheckoutOnSave
			{
				get { return m_autoCheckoutOnSave; }
				set { m_autoCheckoutOnSave = value; mDirty = true; }
			}

			[Category("Operation"), Description("Automagically add files to perforce")]
			public bool autoAdd
			{
				get { return m_autoAdd; }
				set { m_autoAdd = value; mDirty = true; }
			}

			[Category("Operation"), Description("Automagically delete files from perforce when we're deleting files from visual studio")]
			public bool autoDelete
			{
				get { return m_autoDelete; }
				set { m_autoDelete = value; mDirty = true; }
			}

			[Category("Connection"), Description("Use config from system. Effectivly disables the settings inside this dialog for the client etc and picks up the settings from the registry/p4config environment.")]
			public bool useSystemEnv
			{
				get { return m_useSystemConnection; }
				set { m_useSystemConnection = value; mDirty = true; }
			}

			[Category("Connection"), Description("Perforce port number")]
			public string port
			{
				get { return m_port; }
				set { m_port = value; mDirty = true; }
			}

			[Category("Connection"), Description("Perforce client")]
			public string client
			{
				get { return m_client; }
				set { m_client = value; mDirty = true; }
			}

			[Category("Connection"), Description("Perforce username")]
			public string username
			{
				get { return m_username; }
				set { m_username = value; mDirty = true; }
			}

			public static Config Load(string filename)
			{
				Config o;
				if(System.IO.File.Exists(filename))
				{
					o = File.LoadXML<Config>(filename);
				}
				else
				{
					o = new Config();
				}
				o.mFileName = filename;
				Singleton<Config>.Instance = o;
				return o;
			}

			public void Save()
			{
				if(mDirty)
				{
					File.SaveXML<Config>(mFileName, this);
					mDirty = false;
					Singleton<Config>.Instance = this;
				}
			}
		}
	}
}
