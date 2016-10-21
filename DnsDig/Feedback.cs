using System;

namespace DnsDig
{
	class FeedbackWriter : System.IO.TextWriter
	{
		internal FeedbackWriter(System.Windows.Forms.TextBox textBox)
			: base()
		{
			_textBox = textBox;
		}

		private System.Windows.Forms.TextBox _textBox;


		public override System.Text.Encoding Encoding
		{
			get { return System.Text.Encoding.Default; }
		}

		private delegate void WriteDelegate(string value);
		public override void Write(string value)
		{
			if (_textBox.InvokeRequired)
			{
				_textBox.Invoke(new WriteDelegate(Write), new object[] { value });
			}
			else
			{
				_textBox.AppendText(value.Replace("\n", base.NewLine));
			}
		}

		public override void WriteLine(string value)
		{
			this.Write(value);
			this.Write(base.NewLine);
		}
	}
}
