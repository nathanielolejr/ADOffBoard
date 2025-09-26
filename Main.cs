using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ADOffBoard
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private async void btnProcess_Click_1(object sender, EventArgs e)
        {
            string targetUsername = txtTargetUsername.Text.Trim();

            if (string.IsNullOrEmpty(targetUsername))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            try
            {
                progressBar1.Visible = true; // show loading

                await Task.Run(() =>
                {
                    using (PrincipalContext pc = new PrincipalContext(
                        ContextType.Domain,
                        "aboitizfeedall.net",
                        SessionData.LoggedInUsername,
                        SessionData.GetPasswordAsString()
                    ))
                    {
                        UserPrincipal user = UserPrincipal.FindByIdentity(pc, targetUsername);

                        if (user == null)
                            throw new Exception("User not found.");

                        // Remove from groups
                        foreach (GroupPrincipal group in user.GetGroups())
                        {
                            try
                            {
                                group.Members.Remove(user);
                                group.Save();
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.Contains("primary group"))
                                    continue;
                            }
                        }

                        // Move user
                        string username = SessionData.LoggedInUsername;
                        string password = SessionData.GetPasswordAsString();

                        DirectoryEntry de = (DirectoryEntry)user.GetUnderlyingObject();
                        de.Username = username;
                        de.Password = password;

                        DirectoryEntry newOU = new DirectoryEntry(
                            "LDAP://OU=Disabled Users,OU=User Accounts,DC=aboitizfeedall,DC=net",
                            username,
                            password
                        );
                        de.MoveTo(newOU);
                        de.CommitChanges();

                        // Disable
                        user.Enabled = false;
                        user.Save();
                    }
                });

                MessageBox.Show($"User {targetUsername} successfully offboarded and group memberships are removed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                progressBar1.Visible = false; // hide loading
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
