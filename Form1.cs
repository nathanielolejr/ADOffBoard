using System;
using System.DirectoryServices.AccountManagement;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;


namespace ADOffBoard
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private SecureString ConvertToSecureString(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            SecureString secure = new SecureString();
            foreach (char c in password)
                secure.AppendChar(c);

            secure.MakeReadOnly();
            return secure;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = textUsername.Text.Trim();
            string password = txtPassword.Text;

            try
            {
                using (var pc = new PrincipalContext(
                    ContextType.Domain,
                    "yourdomain.com")) // replace with your AD domain
                {
                    bool isValid = pc.ValidateCredentials(username, password);
                    if (isValid)
                    {
                        // ✅ Store credentials for reuse in Main.cs
                        SessionData.LoggedInUsername = username;
                        SessionData.LoggedInPassword = ConvertToSecureString(password);

                        this.Hide();
                        Main mainForm = new Main();
                        mainForm.ShowDialog();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or password.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }


    public static class SessionData
    {
        public static string LoggedInUsername { get; set; }
        public static SecureString LoggedInPassword { get; set; }

        public static string GetPasswordAsString()
        {
            if (LoggedInPassword == null)
                return null;

            IntPtr ptr = Marshal.SecureStringToBSTR(LoggedInPassword);
            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }
    }


}
