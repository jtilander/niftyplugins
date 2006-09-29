using System;
using System.Collections.Generic;
using System.Text;

namespace Aurora
{
	namespace NiftyPerforce
	{
		class Config
		{
			public bool autoCheckout = false;
			public bool autoAdd = true;
			public bool autoDelete = true;

			public Config()
			{
				autoCheckout = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoCheckout", autoCheckout.ToString()));
				autoAdd = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoAdd", autoAdd.ToString()));
				autoDelete = bool.Parse(RegistrySettingsProvider.GetPropertyValue("autoDelete", autoDelete.ToString()));
			}

			public void ShowDialog()
			{
				ConfigDialog dlg = new ConfigDialog();

				dlg.checkBox1.Checked = autoCheckout;
				dlg.checkBox2.Checked = autoAdd;
				dlg.checkBox3.Checked = autoDelete;

				dlg.ShowDialog();

				if (dlg.wasCancelled)
					return;

				autoCheckout = dlg.checkBox1.Checked;
				autoAdd = dlg.checkBox2.Checked;
				autoDelete = dlg.checkBox3.Checked;

				RegistrySettingsProvider.SetPropertyValue("autoCheckout", autoCheckout.ToString());
				RegistrySettingsProvider.SetPropertyValue("autoAdd", autoAdd.ToString());
				RegistrySettingsProvider.SetPropertyValue("autoDelete", autoDelete.ToString());
				System.Windows.Forms.MessageBox.Show("You need to restart the plugin for the settings to take effect", "NiftyPerforce");
			}

		}
	}
}
