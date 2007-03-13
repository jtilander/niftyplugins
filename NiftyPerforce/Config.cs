// Copyright (C) 2006-2007 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class Config
		{
			bool m_autoCheckout = false;
			bool m_autoAdd = true;
			bool m_autoDelete = false;
			string m_port = "";
			string m_client = "";
			string m_username = "";
			string m_password = "";

			[Category("Operation"), Description("Controls if we automagically check out files from perforce upon keypress")]
			public bool autoCheckout
			{
				get { return m_autoCheckout; }
				set { m_autoCheckout = value; }
			}

			[Category("Operation"), Description("Automagically add files to perforce")]
			public bool autoAdd
			{
				get { return m_autoAdd; }
				set { m_autoAdd = value; }
			}

			[Category("Operation"), Description("Automagically delete files from perforce when we're deleting files from visual studio")]
			public bool autoDelete
			{
				get { return m_autoDelete; }
				set { m_autoDelete = value; }
			}

			[Category("Connection"), Description("Perforce port number")]
			public string port
			{
				get { return m_port; }
				set { m_port = value; }
			}

			[Category("Connection"), Description("Perforce client")]
			public string client
			{
				get { return m_client; }
				set { m_client = value; }
			}

			[Category("Connection"), Description("Perforce username")]
			public string username
			{
				get { return m_username; }
				set { m_username = value; }
			}

			public Config()
			{
				m_autoCheckout = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoCheckout", m_autoCheckout.ToString()));
				m_autoAdd = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoAdd", m_autoAdd.ToString()));
				m_autoDelete = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoDelete", m_autoDelete.ToString()));
				m_port = RegistrySettingsProvider.GetPropertyValue("port", "");
				m_client = RegistrySettingsProvider.GetPropertyValue("client", "");
				m_username = RegistrySettingsProvider.GetPropertyValue("username", "");
			}

			public void ShowDialog()
			{
				ConfigDialog dlg = new ConfigDialog();
				dlg.propertyGrid1.SelectedObject = this;
				dlg.ShowDialog();

				if (dlg.wasCancelled)
					return;

				RegistrySettingsProvider.SetPropertyValue("autoCheckout", m_autoCheckout.ToString());
				RegistrySettingsProvider.SetPropertyValue("autoAdd", m_autoAdd.ToString());
				RegistrySettingsProvider.SetPropertyValue("autoDelete", m_autoDelete.ToString());
				RegistrySettingsProvider.SetPropertyValue("port", m_port);
				RegistrySettingsProvider.SetPropertyValue("client", m_client);
				RegistrySettingsProvider.SetPropertyValue("username", m_username);
			}

		}
	}
}
