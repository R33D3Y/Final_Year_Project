using System;
using System.Drawing;
using System.Windows.Forms;

namespace Final_Year_Project
{
    public partial class Friend_Request : UserControl
    {
        private Color darkColour;
        private string username;

        public Friend_Request(Color d, string u)
        {
            InitializeComponent();
            
            darkColour = d;
            username = u;
        }

        private void Friend_Request_Load(object sender, System.EventArgs e)
        {
            BackColor = darkColour;
            label5.Text = "You have a new friend request\nfrom " + username + "!";
        }

        private void Label_MouseHover(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.BackColor = Color.Silver;
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.BackColor = Color.White;
        }
    }
}
