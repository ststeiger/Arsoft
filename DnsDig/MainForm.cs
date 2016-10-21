using System;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

using Heijden.DNS;

namespace DnsDig
{
	public partial class MainForm : Form
	{
		Dig dig;
		List<QuerySet> list;
		int QuerySetIndex;

		private struct QuerySet
		{
			public string Name;
			public QType Type;
			public QClass Class;
			public QuerySet(string Name, QType Type, QClass Class)
			{
				this.Name = Name;
				this.Type = Type;
				this.Class = Class;
			}
		}

		public MainForm()
		{
			InitializeComponent();

			SetupComboBox(typeof(QType), this.comboBox1);
			SetupComboBox(typeof(QClass), this.comboBox2);

			dig = new Dig();

			list = new List<QuerySet>();
			QuerySetIndex = -1;

			this.checkBox1.Checked = dig.resolver.Recursion;
			this.checkBox3.Checked = dig.resolver.UseCache;
			this.textBox2.Text = dig.resolver.DnsServers[0].Address.ToString();
			this.textBoxAttempts.Text = dig.resolver.Retries.ToString();
			this.textBoxTimeout.Text = dig.resolver.TimeOut.ToString();

			Console.SetOut(new FeedbackWriter(this.textBox1));
		}

		private void SetupComboBox(System.Type type, ComboBox comboBox)
		{
			Array types = Enum.GetValues(type);
			for (int intI = 0; intI < types.Length; intI++)
				comboBox.Items.Add(types.GetValue(intI));
			comboBox.SelectedIndex = 0;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void Send(int intIndex)
		{
			this.textBox1.Clear();
			this.textBox1.WordWrap = this.checkBox4.Checked;
			dig.resolver.Recursion = this.checkBox1.Checked;
			dig.resolver.UseCache = this.checkBox3.Checked;
			dig.resolver.DnsServer = this.textBox2.Text;

			int intTimeout,intAttempts;
			int.TryParse(this.textBoxTimeout.Text,out intTimeout);
			int.TryParse(this.textBoxAttempts.Text,out intAttempts);
			dig.resolver.TimeOut = intTimeout;
			dig.resolver.Retries = intAttempts;

			if (intIndex >= 1)
			{
				if (intIndex <= list.Count)
				{
					QuerySet qs = list[intIndex-1];
					this.textBox3.Text = qs.Name;
					this.comboBox1.SelectedItem = qs.Type;
					this.comboBox2.SelectedItem = qs.Class;
				}
			}

			string strName = this.textBox3.Text.Trim();
			QType qType = (QType)this.comboBox1.SelectedItem;
			QClass qClass = (QClass)this.comboBox2.SelectedItem;

			if(intIndex < 0)
				list.Add(new QuerySet(strName, qType, qClass));

			if (intIndex < 0)
				intIndex = list.Count;

			this.labelQuerySet.Text = string.Format("{0} ({1})", intIndex, list.Count);

			this.buttonPrev.Enabled = (intIndex > 1);
			this.buttonNext.Enabled = (intIndex < list.Count);

			QuerySetIndex = intIndex;

			if(qType == QType.AXFR) // zone transfers only use TCP
				this.radioButton2.Checked = true;

			if(this.radioButton1.Checked)
				dig.resolver.TransportType = Heijden.DNS.TransportType.Udp;
			if (this.radioButton2.Checked)
				dig.resolver.TransportType = Heijden.DNS.TransportType.Tcp;

			if (this.checkBox2.Checked)
			{
				if (qType == QType.PTR)
				{
					IPAddress ip;
					if (IPAddress.TryParse(strName, out ip))
						strName = Resolver.GetArpaFromIp(ip);
				}
				if (qType == QType.NAPTR)
				{
						strName = Resolver.GetArpaFromEnum(strName);
				}
			}

			// console output is redirected to textBox1
			dig.BeginDigIt(strName, qType, qClass);
			//dig.DigIt(strName, qType, qClass);

			// show address of currently used DNS server
			this.textBox2.Text = dig.resolver.DnsServers[0].Address.ToString();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Send(-1);
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AboutForm aboutForm = new AboutForm();
			aboutForm.ShowDialog(this);
		}

		private void textBox3_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Enter)
			{
				Send(-1);
				e.SuppressKeyPress = true;
			}
		}

		private void buttonPrev_Click(object sender, EventArgs e)
		{
			Send(QuerySetIndex - 1);
		}

		private void buttonNext_Click(object sender, EventArgs e)
		{
			Send(QuerySetIndex + 1);
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.textBox2.Text = Resolver.GetDnsServers()[0].Address.ToString();
		}

	}
}