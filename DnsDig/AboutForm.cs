using System;
using System.Windows.Forms;
using System.Diagnostics;

using Heijden.DNS;

namespace DnsDig
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.heijden.com/alphons/mail/");
		}
	}
}