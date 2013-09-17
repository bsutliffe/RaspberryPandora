using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RaspberryPandora {
	public partial class LoginForm : Form {
		public delegate void LoginEventHandler(object sender, string username, string password);
		public event LoginEventHandler Submitted;

		public LoginForm() {
			InitializeComponent();

			this.Shown += LoginForm_Shown;
		}

		void LoginForm_Shown(object sender, EventArgs e) {
			txtUsername.Focus();
		}

		private void btnSubmit_Click(object sender, EventArgs e) {
			string username = txtUsername.Text;
			string password = txtPassword.Text;
			LoginEventHandler submittedHandler = Submitted;
			if (submittedHandler != null)
				submittedHandler(this, username, password);
		}
	}
}
