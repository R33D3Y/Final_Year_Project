// Dependencies
using ConsoleApplication;
using Gecko;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GoogleApi;
using GoogleApi.Entities.Places.AutoComplete.Request;
using GoogleApi.Entities.Places.Search.Text.Request;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_Year_Project
{
    public partial class Form1 : Form
    {
        private Database database; // Object for interacting with the database
        
        private DateTime dt; // Current datetime of the calendar
        private List<int> visibleGroups = new List<int>(); // Groups selected as visible by the user
        private List<Emoji> emojis; // List of emojis for the user to choose from
        private List<Notification> notifications = new List<Notification>(); // List of current notifications for the user

        private Color lightColour = Color.CornflowerBlue; // Default primary colour
        private Color darkColour = Color.RoyalBlue; // Default secondary colour

        public Form1()
        {
            InitializeComponent();
            Xpcom.Initialize("Firefox");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (CheckForInternetConnection()) // Checks for internet connection
            {
                PictureBox_Internet.Visible = false;
                Label_Internet.Visible = false;
            }

            else
            {
                PictureBox_Internet.Visible = true;
                Label_Internet.Visible = true;
                Login_Button.Enabled = false;
                SignUp_Button.Enabled = false;
            }

            Database db = new Database();

            if (db.Test_Connection()) // Checks for connection to database
            {
                PictureBox_Internet.Visible = false;
                Label_Server.Visible = false;
            }

            else
            {
                PictureBox_Internet.Visible = true;
                Label_Server.Visible = true;
                Login_Button.Enabled = false;
                SignUp_Button.Enabled = false;
            }

            Set_Colours(); // Sets programs colours all to default

            nsICookieManager CookieMan;
            CookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
            CookieMan = Xpcom.QueryInterface<nsICookieManager>(CookieMan);
            CookieMan.RemoveAll();

            // https://stackoverflow.com/questions/13513063/gecko-clear-cache-history-cookies/26111307
        }

        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204")) // Attempts to access internet page, failure dispays lack of connection
                {
                    return true;
                }
            }

            catch
            {
                return false;
            }

            //https://stackoverflow.com/questions/2031824/what-is-the-best-way-to-check-for-internet-connectivity-using-net
        }

        private void Set_Colours()
        {
            Color temp = Color.Black; // Temp colour is used when the contrast would cause a lack of readability

            // Form Panel
            Form_Button_Panel.BackColor = darkColour;
            PictureBox_Back.BackColor = darkColour;
            PictureBox_Settings.BackColor = darkColour;
            PictureBox_Logout.BackColor = darkColour;
            PictureBox_Drag.BackColor = darkColour;
            PictureBox_Minimise.BackColor = darkColour;
            PictureBox_Close.BackColor = darkColour;
            PictureBox_Notification.BackColor = darkColour;

            // Dashboard Panel
            Dashboard_Panel.BackColor = lightColour;
            
            Dashboard_Control_Panel.BackColor = darkColour;
            Dashboard_Search.BackColor = darkColour;
            Groups_Data.BackgroundColor = darkColour;
            Groups_Data.DefaultCellStyle.BackColor = darkColour;
            

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Dashboard_Add_Event.ForeColor = temp;
                Dashboard_Add_Group.ForeColor = temp;
                Dashboard_Search_Button.ForeColor = temp;
                Find_Friends_Button.ForeColor = temp;
                Groups_Data.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                Dashboard_Add_Event.ForeColor = lightColour;
                Dashboard_Add_Group.ForeColor = lightColour;
                Dashboard_Search_Button.ForeColor = lightColour;
                Find_Friends_Button.ForeColor = lightColour;
                Groups_Data.DefaultCellStyle.SelectionForeColor = lightColour;
            }

            // Event Panel
            Event_Panel.BackColor = lightColour;
            Event_Control_Panel.BackColor = darkColour;

            TextBox_Name_Event.BackColor = darkColour;
            TextBox_Description.BackColor = darkColour;
            ComboBox_Group.BackColor = darkColour;
            Event_TextBox_Emoji.BackColor = darkColour;
            TextBox_Location_Search.BackColor = darkColour;
            Data_Location_Lookup.BackgroundColor = darkColour;
            Data_Location_Lookup.DefaultCellStyle.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Event_Button_Emoji.ForeColor = temp;
                //Search_Location_Button.ForeColor = temp;
                Add_Event_Button.ForeColor = temp;
                Update_Event_Button.ForeColor = temp;
                Remove_Event_Button.ForeColor = temp;
                Map_Type_Button.ForeColor = temp;
                Data_Location_Lookup.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                Event_Button_Emoji.ForeColor = lightColour;
                //Search_Location_Button.ForeColor = lightColour;
                Add_Event_Button.ForeColor = lightColour;
                Update_Event_Button.ForeColor = lightColour;
                Remove_Event_Button.ForeColor = lightColour;
                Map_Type_Button.ForeColor = lightColour;
                Data_Location_Lookup.DefaultCellStyle.SelectionForeColor = lightColour;
            }

            // Friends Panel
            Friends_Panel.BackColor = lightColour;
            Friends_Control_Panel.BackColor = darkColour;

            TextBox_Search_Username.BackColor = darkColour;
            TextBox_Friends_Nickname.BackColor = darkColour;
            
            Search_Friends.BackgroundColor = darkColour;
            Search_Friends.DefaultCellStyle.BackColor = darkColour;
            Data_Friends.BackgroundColor = darkColour;
            Data_Friends.DefaultCellStyle.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Search_Username_Button.ForeColor = temp;
                Add_Friend_Button.ForeColor = temp;
                Remove_Friend_Button.ForeColor = temp;
                Update_Friend_Button.ForeColor = temp;
                Search_Friends.DefaultCellStyle.SelectionForeColor = temp;
                Data_Friends.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                Search_Username_Button.ForeColor = lightColour;
                Add_Friend_Button.ForeColor = lightColour;
                Remove_Friend_Button.ForeColor = lightColour;
                Update_Friend_Button.ForeColor = lightColour;
                Search_Friends.DefaultCellStyle.SelectionForeColor = lightColour;
                Data_Friends.DefaultCellStyle.SelectionForeColor = lightColour;
            }

            // Group Panel
            Group_Panel.BackColor = lightColour;
            Group_Control_Panel.BackColor = darkColour;

            TextBox_Name_Group.BackColor = darkColour;
            TextBox_Group_Update.BackColor = darkColour;

            Data_Groups.BackgroundColor = darkColour;
            Data_Groups.DefaultCellStyle.BackColor = darkColour;
            Data_Display_Groups.BackgroundColor = darkColour;
            Data_Display_Groups.DefaultCellStyle.BackColor = darkColour;
            Data_Groups_Friends.BackgroundColor = darkColour;
            Data_Groups_Friends.DefaultCellStyle.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                ColourPicker_Button.ForeColor = temp;
                Add_Group_Button.ForeColor = temp;
                Update_Group.ForeColor = temp;
                Update_Colour_Group.ForeColor = temp;
                Remove_Group_Name.ForeColor = temp;
                Data_Groups.DefaultCellStyle.SelectionForeColor = temp;
                Data_Display_Groups.DefaultCellStyle.SelectionForeColor = temp;
                Data_Groups_Friends.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                ColourPicker_Button.ForeColor = lightColour;
                Add_Group_Button.ForeColor = lightColour;
                Update_Group.ForeColor = lightColour;
                Update_Colour_Group.ForeColor = lightColour;
                Remove_Group_Name.ForeColor = lightColour;
                Data_Groups.DefaultCellStyle.SelectionForeColor = lightColour;
                Data_Display_Groups.DefaultCellStyle.SelectionForeColor = lightColour;
                Data_Groups_Friends.DefaultCellStyle.SelectionForeColor = lightColour;
            }

            // Search Panel
            Search_Panel.BackColor = lightColour;
            Search_Control_Panel.BackColor = darkColour;

            TextBox_Search.BackColor = darkColour;
            Search_Description.BackColor = darkColour;

            Search_Data.BackgroundColor = darkColour;
            Search_Data.DefaultCellStyle.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Search_Panel_Button.ForeColor = temp;
                Search_Add_Event_Button.ForeColor = temp;
                Search_Switch_Map_Button.ForeColor = temp;
                Search_Event_Update.ForeColor = temp;
                Busiest_Days_Button.ForeColor = temp;
                Search_Data.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                Search_Panel_Button.ForeColor = lightColour;
                Search_Add_Event_Button.ForeColor = lightColour;
                Search_Switch_Map_Button.ForeColor = lightColour;
                Search_Event_Update.ForeColor = lightColour;
                Busiest_Days_Button.ForeColor = lightColour;
                Search_Data.DefaultCellStyle.SelectionForeColor = lightColour;
            }

            // Settings Panel
            Settings_Panel.BackColor = lightColour;
            Settings_Control_Panel.BackColor = darkColour;

            Settings_Dark_Panel.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Settings_Light_Button.ForeColor = temp;
                Settings_Dark_Button.ForeColor = temp;
                Settings_Commit.ForeColor = temp;
                Settings_Light_Panel.BackColor = temp;
                Link_Facebook_Button.ForeColor = temp;
            }

            else
            {
                Settings_Light_Button.ForeColor = lightColour;
                Settings_Dark_Button.ForeColor = lightColour;
                Settings_Commit.ForeColor = lightColour;
                Settings_Light_Panel.BackColor = lightColour;
                Link_Facebook_Button.ForeColor = lightColour;
            }

            // Emoji Panel
            Emoji_Panel.BackColor = lightColour;
            Emoji_Control_Panel.BackColor = darkColour;

            Table_Layout_Panel_Emoji.BackColor = darkColour;

            // Notification Panel
            Notification_Panel.BackColor = lightColour;
            Notification_Control_Panel.BackColor = darkColour;

            TableLayoutPanel_Notifications.BackColor = darkColour;

            // Facebook Panel
            Facebook_Panel.BackColor = lightColour;
            Facebook_Control_Panel.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Link_Skip_Button.ForeColor = temp;
            }

            else
            {
                Link_Skip_Button.ForeColor = lightColour;
            }

            // Busy Panel
            Busiest_Day_Panel.BackColor = lightColour;
            Busiest_Day_Control_Panel.BackColor = darkColour;

            Data_Busy_Days.BackgroundColor = darkColour;
            Data_Busy_Days.DefaultCellStyle.BackColor = darkColour;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                Select_Button.ForeColor = temp;
                Data_Busy_Days.DefaultCellStyle.SelectionForeColor = temp;
            }

            else
            {
                Select_Button.ForeColor = lightColour;
                Data_Busy_Days.DefaultCellStyle.SelectionForeColor = lightColour;
            }
        }

        private void Calendar_Back_Click(object sender, EventArgs e) // Sends calendar back 1 month
        {
            dt = dt.AddMonths(-1);
            SetData(database.GetData(dt), dt);
        }

        private void Calendar_Forward_Click(object sender, EventArgs e) // Sends calendar forward 1 month
        {
            dt = dt.AddMonths(1);
            SetData(database.GetData(dt), dt);
        }

        private void PictureBox_MouseHover(object sender, EventArgs e) // Changes back colour to display mouse hover
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.FromArgb(84, 84, 84);
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e) // Returns back color to default to display mouse leave
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.FromArgb(64, 64, 64);
        }

        private void Login_Button_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void SignUp_Button_Click(object sender, EventArgs e)
        {
            database = new Database(); // New empty data object to check for exsisting usernames and emails

            Login_Panel.Visible = false;
            Signup_Panel.Visible = true;

            PictureBox_Signup_Back.Visible = true;
        }

        private void Login()
        {
            database = new Database(Textbox_Username.Text, GetHashString(Textbox_Password.Text)); // Logs in using a password hash and username

            if (database.GetUser() != null) // Success
            {
                PictureBox_Username_Cross.Visible = false;
                PictureBox_Password_Cross.Visible = false;
                
                DateTime tempDt = DateTime.Now;
                dt = new DateTime(tempDt.Year, tempDt.Month, 1); // Sets calendar to current date

                Setup_Emojis(); // Gets emojis and displays them

                calendar = tableLayoutPanel;
                header = tableLayoutPanelCalendarHeader;
                Setup_Dashboard_Groups(false);
                SetData(database.GetData(dt), dt); // Gets user events and displays on calendar

                List<Color> temp = database.Get_User_Colours(); // Get user defined colours
                lightColour = temp[0];
                darkColour = temp[1];

                Set_Colours(); // Sets user defined colours

                if (database.Get_Facebook_Link()) // Check for facebook link
                {
                    Facebook_Browser.Navigate("https://www.facebook.com/v3.2/dialog/oauth?client_id=1227276437422824&redirect_uri=https://www.facebook.com/connect/login_success.html&state={st=state123abc,ds=123456789}&scope=user_events"); // Requests user to re-login

                    Facebook_Panel.Visible = true;
                    Login_Panel.Visible = false;
                    PictureBox_Back.Visible = true;
                    PictureBox_Logout.Visible = true;
                    PictureBox_Settings.Visible = true;
                }

                else
                {
                    Dashboard_Panel.Visible = true;
                    Login_Panel.Visible = false;
                    PictureBox_Logout.Visible = true;
                    PictureBox_Settings.Visible = true;
                }
            }

            else // Failure
            {
                PictureBox_Username_Cross.Visible = true;
                PictureBox_Password_Cross.Visible = true;
            }
        }

        public static byte[] GetHash(string inputString) // Hashing algorithm
        {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString) // Hash call method
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        private void Label_MouseHover(object sender, EventArgs e) // Mouse hover for buttons
        {
            Label l = (Label)sender;
            l.BackColor = Color.Silver;
        }

        private void Label_MouseLeave(object sender, EventArgs e) // Mouse leave for buttons
        {
            Label l = (Label)sender;
            l.BackColor = Color.White;
        }

        private bool UsePasswordMask = true;

        private void Lock_Click(object sender, EventArgs e) // Toggles password visible
        {
            if (UsePasswordMask)
            {
                Textbox_Password.UseSystemPasswordChar = false;
                UsePasswordMask = false;
            }

            else
            {
                Textbox_Password.UseSystemPasswordChar = true;
                UsePasswordMask = true;
            }
        }

        private void PictureBox_Close_Click(object sender, EventArgs e) // Exits application
        {
            Application.Exit();
        }

        private void PictureBox_Minimise_Click(object sender, EventArgs e) // Minimise application
        {
            WindowState = FormWindowState.Minimized;
        }

        private void PictureBox_Form_MouseHover(object sender, EventArgs e) // Main form toolbox mouse hover
        {
            PictureBox pb = (PictureBox)sender;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                pb.BackColor = Color.Black;
            }

            else
            {
                pb.BackColor = lightColour;
            }
        }

        private void PictureBox_Form_MouseLeave(object sender, EventArgs e) // Main form toolbox mouse leave
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = darkColour;
        }

        private void Emoji_MouseHover(object sender, EventArgs e) // Emoji panel mouse hover
        {
            Label l = (Label)sender;

            if (lightColour.GetBrightness() > 0.7) // If text would be unreadable
            {
                l.BackColor = Color.Black;
            }

            else
            {
                l.BackColor = lightColour;
            }
        }

        private void Emoji_MouseLeave(object sender, EventArgs e) // Emoji panel mouse leave
        {
            Label l = (Label)sender;
            l.BackColor = darkColour;
        }

        private void PictureBox_Logout_Click(object sender, EventArgs e)
        {
            if (database != null) // Set database to null
            {
                Textbox_Username.Text = "Username";
                Textbox_Password.Text = "Password";
                database = null;
            }

            visibleGroups = new List<int>(); // Reset visible groups
            
            ResetForm(); // Clear form of all interactions

            lightColour = Color.CornflowerBlue; // Reset default colours
            darkColour = Color.RoyalBlue;

            Set_Colours(); // Apply colours

            Event_Panel.Visible = false; // Hide all screens
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = false;
            Notification_Panel.Visible = false;
            Emoji_Panel.Visible = false;
            Facebook_Panel.Visible = false;
            Busiest_Day_Panel.Visible = false;
            Dashboard_Panel.Visible = false;

            PictureBox_Settings.Visible = false;
            PictureBox_Back.Visible = false;
            PictureBox_Logout.Visible = false;
            PictureBox_Notification.Visible = false;

            Login_Panel.Visible = true; // Display login screen
        }

        private void Setup_Emojis() // Gets emojis from file and places into table
        {
            TextReader tr = new StreamReader(@"Emojis.txt", Encoding.Unicode, true);
            emojis = new List<Emoji>();
            string line;

            while ((line = tr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                emojis.Add(new Emoji(split[0], split[1]));
            }

            database.SetEmojis(emojis); // Toggles emojis from to text icon

            Table_Layout_Panel_Emoji.Controls.Clear(); // Empties table

            foreach (Emoji emoji in emojis)
            {
                Label label = new Label() { Text = emoji.GetIcon(), Font = new Font("Candara", 16), ForeColor = Color.White, Width = 40, Height = 40, Cursor = Cursors.Hand, TextAlign = ContentAlignment.MiddleCenter };
                label.Click += (se, ev) =>
                {
                    Event_TextBox_Emoji.Text = label.Text;
                    Emoji_Panel.Visible = false;
                    Event_Panel.Visible = true;
                };
                label.MouseHover += (se, ev) =>
                {
                    Emoji_MouseHover(se, ev);
                };
                label.MouseLeave += (se, ev) =>
                {
                    Emoji_MouseLeave(se, ev);
                };

                Table_Layout_Panel_Emoji.Controls.Add(label); // Adds to table with event handlers
            }
        }

        private void Setup_Groups() // Gets groups to display or hide
        {
            List<CalendarGroup> cg = database.GetGroups();

            ComboBox_Group.Items.Clear();

            for (int i = 0; i < cg.Count; i++)
            {
                ComboBox_Group.Items.Add(cg[i].GetName());
            }
        }

        private void Add_Event_Button_Click(object sender, EventArgs e) // Adds new event to calendar
        {
            if (!SQLSafe(TextBox_Name_Event.Text) || TextBox_Name_Event.Text.Equals("")) // Checks inputs for unsafe characters
            {
                Event_Cross_Name.Visible = true;
            }

            else
            {
                Event_Cross_Name.Visible = false;
            }

            if (!SQLSafe(TextBox_Description.Text))
            {
                Event_Cross_Description.Visible = true;
            }

            else
            {
                Event_Cross_Description.Visible = false;
            }

            if (ComboBox_Group.Text.Equals(""))
            {
                Event_Cross_Group.Visible = true;
            }

            else
            {
                Event_Cross_Group.Visible = false;
            }

            if (SQLSafe(TextBox_Name_Event.Text) && SQLSafe(TextBox_Description.Text) && !ComboBox_Group.Text.Equals("") && !TextBox_Name_Event.Text.Equals(""))
            {
                Event_Cross_Name.Visible = false;
                Event_Cross_Description.Visible = false;
                Event_Cross_Group.Visible = false;

                DateTime datetime = new DateTime(DateTimePicker_Date.Value.Year, DateTimePicker_Date.Value.Month, DateTimePicker_Date.Value.Day, DateTimePicker_Time.Value.Hour, DateTimePicker_Time.Value.Minute, DateTimePicker_Time.Value.Second);

                List<CalendarGroup> cg = database.GetGroups();
                int group = 0;

                for (int i = 0; i < cg.Count; i++)
                {
                    if (ComboBox_Group.Text == cg[i].GetName())
                    {
                        group = cg[i].GetID();
                    }
                }

                string location = "";

                foreach (DataGridViewRow row in Data_Location_Lookup.SelectedRows)
                {
                    location = row.Cells[0].Value.ToString();
                }

                database.Add_Event(TextBox_Name_Event.Text, TextBox_Description.Text, datetime, location, TextBox_Location.Text, Event_TextBox_Emoji.Text, group); // Gets all formatted data and inputs correctly into database

                DateTime tempDt = DateTime.Now;
                dt = new DateTime(tempDt.Year, tempDt.Month, 1);

                calendar = tableLayoutPanel;
                header = tableLayoutPanelCalendarHeader;
                SetData(database.GetData(dt), dt); // Resets calendar to display new event if in current month

                Dashboard_Panel.Visible = true;
                Event_Panel.Visible = false;
                PictureBox_Back.Visible = false;
            }
        }

        private bool SQLSafe(string str) // Checks for SQL not safe characters
        {
            string[] illegalCharacters = { "'", ";", ",", "@" };

            foreach (string cha in illegalCharacters)
            {
                if (str.Contains(cha))
                {
                    return false;
                }
            }

            return true;
        }

        private void DatePicker_DateSelected(object sender, DateRangeEventArgs e) // Sets calendar to date selected on date picker
        {
            DateTime tempDt = e.Start;
            dt = new DateTime(tempDt.Year, tempDt.Month, 1);

            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);
        }

        private void Dashboard_Add_Event_Click(object sender, EventArgs e) // Takes user to add event screen
        {
            ResetForm();

            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private void PictureBox_Back_Click(object sender, EventArgs e) // Returns user to dashboard and restes all inputs
        {
            ResetForm();

            Dashboard_Panel.Visible = true;

            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = false;
            Notification_Panel.Visible = false;
            Emoji_Panel.Visible = false;
            Facebook_Panel.Visible = false;
            Busiest_Day_Panel.Visible = false;
            PictureBox_Back.Visible = false;
            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
        }

        private void ColourPicker_Click(object sender, EventArgs e) // Sets chosen colour
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            
            Colour_Panel.BackColor = cd.Color;
        }

        private void Dashboard_Add_Group_Click(object sender, EventArgs e) // Takes user to groups screen
        {
            ResetForm();

            List<CalendarGroup> g = database.GetGroups();

            Data_Display_Groups.Rows.Clear();

            for (int i = 0; i < g.Count; i++)
            {
                Data_Display_Groups.Rows.Add(g[i].GetID(), g[i].GetName());
            }

            Dashboard_Panel.Visible = false;
            Group_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private void Add_Group_Button_Click(object sender, EventArgs e) // Adds group to the users profile
        {
            if (TextBox_Name_Group.Text.Equals("") || !SQLSafe(TextBox_Name_Group.Text)) // Checks for SQL non-accepted characters
            {
                Group_Cross_Name.Visible = true;
            }

            else
            {
                Group_Cross_Name.Visible = false;

                int Group_ID = database.Add_Group(TextBox_Name_Group.Text, Colour_Panel.BackColor.ToArgb()); // Creates group

                foreach (DataGridViewRow row in Data_Groups.Rows)
                {
                    if ((bool)row.Cells[0].Value == true)
                    {
                        int User_ID = (int)row.Cells[1].Value;

                        database.Add_Friend_To_Group(User_ID, Group_ID); // Adds other users to new group
                    }
                }

                ResetForm();
            }

            List<CalendarGroup> g = database.GetGroups(); // Gets all groups that user is in

            Data_Display_Groups.Rows.Clear();

            for (int i = 0; i < g.Count; i++)
            {
                Data_Display_Groups.Rows.Add(g[i].GetID(), g[i].GetName());
            }
        }

        private void GMap_Control_MouseClick(object sender, MouseEventArgs e) // Selects location
        {
            if (e.Button == MouseButtons.Left)
            {
                double lat = GMap_Control.FromLocalToLatLng(e.X, e.Y).Lat; // Gets latitude and longtitude
                double lng = GMap_Control.FromLocalToLatLng(e.X, e.Y).Lng;

                GMap_Control.Overlays.Clear();

                GMapOverlay markers = new GMapOverlay("markers");
                GMap_Control.Overlays.Add(markers);

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                markers.Markers.Add(marker); // Adds marker to map to display location selected

                TextBox_Location.Text = lat + "," + lng;

                PictureBox_Directions.Visible = true;
            }
        }

        private bool maptype = false;

        private void Map_Type_Click(object sender, EventArgs e) // Toggles map type between map and satellite view
        {
            if (maptype)
            {
                GMap_Control.MapProvider = OpenStreetMapProvider.Instance;
                GMaps.Instance.Mode = AccessMode.ServerOnly;

                maptype = false;
            }

            else
            {
                GMap_Control.MapProvider = BingSatelliteMapProvider.Instance;
                GMaps.Instance.Mode = AccessMode.ServerOnly;

                maptype = true;
            }
        }

        private void Search_Button_Click(object sender, EventArgs e) // Searchs events for user defined text
        {
            Search(TextBox_Search.Text);
        }

        private void Search(string text)
        {
            Search_Data.DataSource = database.Get_Search_Results(text); // Search database
            
            foreach (DataGridViewRow row in Search_Data.Rows)
            {
                row.Cells[4].Value = database.Emoji((string)row.Cells[4].Value, false); // Sets Emojis to icons
            }

            // Spreadsheet format
            Search_Data.Columns[0].Visible = false; // Event ID
            Search_Data.Columns[2].Visible = false; // Event Description
            Search_Data.Columns[5].Visible = false; // Group ID
            Search_Data.Columns[7].Visible = false; // Event Location Name
            Search_Data.Columns[8].Visible = false; // Event Location Geo

            Search_Data.Columns[1].HeaderText = "Event Name";
            Search_Data.Columns[3].HeaderText = "Date/Time";
            Search_Data.Columns[4].HeaderText = "Emoji";
            Search_Data.Columns[6].HeaderText = "Group";

            Search_Data.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Search_Data.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Search_Data.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Search_Data_Click(object sender, EventArgs e) // Fired when the use clicks on a row from the search data
        {
            Search_Event_Update.Visible = true;

            foreach (DataGridViewRow row in Search_Data.SelectedRows)
            {
                string Event_Description = row.Cells[2].Value.ToString();

                if (!row.Cells[8].Value.ToString().Equals(","))
                {
                    string[] Event_Location = row.Cells[8].Value.ToString().Split(',');

                    double lat = Convert.ToDouble(Event_Location[0]);
                    double lng = Convert.ToDouble(Event_Location[1]);

                    GMap_Control_Search.MapProvider = OpenStreetMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control_Search.Position = new PointLatLng(lat, lng);
                    GMap_Control_Search.ShowCenter = false;

                    GMap_Control_Search.Overlays.Clear();

                    GMapOverlay markers = new GMapOverlay("markers");
                    GMap_Control_Search.Overlays.Add(markers);

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot); // Sets map to display location of event
                    markers.Markers.Add(marker);
                }

                else
                {
                    GMap_Control_Search.MapProvider = OpenStreetMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control_Search.Position = new PointLatLng(0, 0);
                    GMap_Control_Search.ShowCenter = false;

                    GMap_Control_Search.Overlays.Clear();
                }

                Search_Description.Text = Event_Description;
            }
        }

        private bool maptype_search = false;

        private void Search_Switch_Map_Button_Click(object sender, EventArgs e) // Toggles map type
        {
            if (maptype_search)
            {
                GMap_Control_Search.MapProvider = OpenStreetMapProvider.Instance;
                GMaps.Instance.Mode = AccessMode.ServerOnly;

                maptype_search = false;
            }

            else
            {
                GMap_Control_Search.MapProvider = BingSatelliteMapProvider.Instance;
                GMaps.Instance.Mode = AccessMode.ServerOnly;

                maptype_search = true;
            }
        }

        private void Dashboard_Search_Button_Click(object sender, EventArgs e) // Searches for events from the dashboard
        {
            string text = Dashboard_Search.Text;

            ResetForm();

            Search(text);

            Dashboard_Panel.Visible = false;
            Search_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private bool findLocation = true;
        private double userLatitude = 0;
        private double userLongitude = 0;

        private void ResetForm() // Resets the program fields/components
        {
            if (findLocation) // Checks to see if user has location enabled
            {
                try
                {
                    Stopwatch sw = new Stopwatch(); // Prevents system timing out and crashing if location service is turned off
                    sw.Start();
                    GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                    GeoCoordinate coord = watcher.Position.Location;

                    while (coord.IsUnknown)
                    {
                        if (sw.ElapsedMilliseconds > 3000)
                        {
                            break;
                        }

                        watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
                        coord = watcher.Position.Location;

                        if (coord.IsUnknown != true)
                        {
                            userLatitude = coord.Latitude;
                            userLongitude = coord.Longitude;
                        }
                    }

                    sw.Stop();
                }

                catch (COMException)
                {

                }

                findLocation = false;
            }

            GMap_Control.MapProvider = OpenStreetMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control.Position = new PointLatLng(userLatitude, userLongitude);
            GMap_Control.ShowCenter = false;
            maptype = false;
            GMap_Control.Overlays.Clear();

            TextBox_Event_ID.Text = "";
            TextBox_Name_Event.Text = "Enter Event Name";
            TextBox_Description.Text = "Enter Description";
            Event_TextBox_Emoji.Text = "";
            ComboBox_Group.Items.Clear();
            TextBox_Location.Text = ",";
            TextBox_Location_Search.Text = "Enter Address or Place";
            DateTimePicker_Date.Text = DateTime.Now.ToLongDateString();
            string time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":00";
            DateTimePicker_Time.Text = time; //DateTime.Now.ToLongTimeString();
            PictureBox_Directions.Visible = false;

            GMap_Control_Search.MapProvider = OpenStreetMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control_Search.Position = new PointLatLng(userLatitude, userLongitude);
            GMap_Control_Search.ShowCenter = false;
            maptype_search = false;
            GMap_Control_Search.Overlays.Clear();

            if (database != null)
            {
                Setup_Emojis();
                Setup_Groups();
                Setup_Friends();
                Setup_Dashboard_Groups(true);

                notifications = database.Get_Notifications();

                if (notifications.Count > 0)
                {
                    PictureBox_Notification.Visible = true;
                }

                else
                {
                    PictureBox_Notification.Visible = false;
                }
            }
            
            ComboBox_Group.Text = "Select Group";

            TextBox_Search.Text = "Enter Search Criteria";
            Search_Description.Text = "Event Description";
            Search_Event_Update.Visible = false;

            Colour_Panel.BackColor = Color.Black;
            TextBox_Name_Group.Text = "Enter Group Name";
            TextBox_Group_Update.Text = "Group Name";
            Colour_Panel_Update.BackColor = Color.Black;

            Dashboard_Search.Text = "Enter Search Criteria";

            Add_Friend_Button.Text = "Add As Friend";
            TextBox_Search_Username.Text = "Enter Username";
            TextBox_Friends_Nickname.Text = "Enter A Nickname";

            Data_Friends.DataSource = null;
            Data_Friends.Refresh();
            Data_Groups.DataSource = null;
            Data_Groups.Refresh();
            Search_Data.DataSource = null;
            Search_Data.Refresh();
            Search_Friends.DataSource = null;
            Search_Friends.Refresh();
            Data_Display_Groups.DataSource = null;
            Data_Display_Groups.Refresh();
            Data_Groups_Friends.DataSource = null;
            Data_Groups_Friends.Refresh();
            Data_Busy_Days.DataSource = null;
            Data_Busy_Days.Refresh();

            Facebook_Browser.Navigate("www.Facebook.com");
        }

        // Calendar Class

        private TableLayoutPanel calendar;
        private TableLayoutPanel header;
        private List<List<CalendarEvent>> data;
        private DateTime startDate;

        public void Render() // This method creates the look and actions made by the main claendaer section
        {
            string[] day_names = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st" };

            int day_count = 0;
            int data_count = 0;

            bool switch_colour = true;

            SuspendLayout();
            calendar.Visible = false;
            calendar.Controls.Clear();

            if (header.GetControlFromPosition(1, 0) == null)
            {
                header.Controls.Add(new Label() { Text = startDate.ToString("MMMM") + " " + startDate.Year, Font = new Font("Candara", 16, FontStyle.Bold), ForeColor = Color.White, AutoSize = false, Anchor = AnchorStyles.None, Dock = DockStyle.Fill, BackColor = Color.FromArgb(40, 40, 40), TextAlign = ContentAlignment.MiddleCenter });
            }

            else
            {
                header.GetControlFromPosition(1, 0).Text = startDate.ToString("MMMM") + " " + startDate.Year;
            }

            for (int i = 0; i < 7; i++)
            {
                TableLayoutPanel p = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(0, 0, 0, 0),
                    BackColor = Color.FromArgb(40, 40, 40)
                };

                p.Controls.Add(new Label() { Text = day_names[i], ForeColor = Color.White, Font = new Font("Candara", 9, FontStyle.Bold), AutoSize = true, Anchor = AnchorStyles.None });
                calendar.Controls.Add(p);
            }

            switch (startDate.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    break;
                case DayOfWeek.Tuesday:
                    RenderEmptyCells(calendar, 1);
                    break;
                case DayOfWeek.Wednesday:
                    RenderEmptyCells(calendar, 2);
                    break;
                case DayOfWeek.Thursday:
                    RenderEmptyCells(calendar, 3);
                    break;
                case DayOfWeek.Friday:
                    RenderEmptyCells(calendar, 4);
                    break;
                case DayOfWeek.Saturday:
                    RenderEmptyCells(calendar, 5);
                    break;
                case DayOfWeek.Sunday:
                    RenderEmptyCells(calendar, 6);
                    break;
            }

            for (int i = 0; i < calendar.RowCount; i++)
            {
                for (int j = 0; j < calendar.ColumnCount; j++)
                {
                    if (calendar.GetControlFromPosition(j, i) == null)
                    {
                        //Console.WriteLine(calendar.GetRowHeights()[i]);

                        if (calendar.GetRowHeights()[i] == 20)
                        {
                            try
                            {
                                if (days.Length == day_count || day_count >= startDate.AddMonths(1).AddDays(-1).Day)
                                {
                                    calendar.Controls.Add(new Label() { Text = "" });
                                }

                                else
                                {
                                    calendar.Controls.Add(new Label() { Text = days[day_count], Font = new Font("Candara", 9), ForeColor = Color.White, BackColor = Color.DimGray, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 0, 0) });
                                }
                            }

                            catch (Exception)
                            {
                                //Console.WriteLine(ex_var.Message);
                            }

                            day_count++;
                        }

                        else
                        {
                            if (days.Length == data_count || data_count >= startDate.AddMonths(1).AddDays(-1).Day)
                            {
                                calendar.Controls.Add(new Label() { Text = "" });

                                //Console.WriteLine(data_count);
                            }

                            else
                            {
                                try
                                {
                                    TableLayoutPanel p = new TableLayoutPanel
                                    {
                                        ColumnCount = 1,
                                        RowCount = data[data_count].Count,
                                        Dock = DockStyle.Fill,
                                        Margin = new Padding(0, 0, 0, 0)
                                    };

                                    p.Click += (s, e) =>
                                    {
                                        PanelClickEvent(s, e);
                                    };

                                    List<CalendarGroup> lcg = new List<CalendarGroup>();

                                    foreach (CalendarEvent ce in data[data_count])
                                    {
                                        if (lcg.Count == 0)
                                        {
                                            lcg.Add(ce.GetCalendarGroup());
                                        }

                                        else
                                        {
                                            bool found = false;
                                            foreach (CalendarGroup cg in lcg)
                                            {
                                                if (ce.GetCalendarGroup().GetID() == cg.GetID())
                                                {
                                                    found = true;
                                                }
                                            }

                                            if (!found)
                                            {
                                                lcg.Add(ce.GetCalendarGroup());
                                            }
                                        }
                                    }

                                    //Console.WriteLine(lcg.Count);

                                    if (data[data_count].Count > 5)
                                    {
                                        int addedCount = 0;

                                        foreach (CalendarGroup cg in lcg)
                                        {
                                            Color txt = Color.Black;

                                            if (cg.GetColor().GetBrightness() < 0.3)
                                            {
                                                txt = Color.White;
                                            }

                                            Label l = new Label() { ForeColor = txt, BackColor = cg.GetColor(), Height = 15, Margin = new Padding(0, 0, 0, 0) };
                                            l.Click += (s, e) =>
                                            {
                                                s = l.Parent;
                                                PanelClickEvent(s, e);
                                            };

                                            ToolTip t = new ToolTip
                                            {
                                                IsBalloon = true
                                            };

                                            t.SetToolTip(l, cg.GetName());

                                            int count = 0;

                                            foreach (CalendarEvent ce in data[data_count])
                                            {
                                                //Console.WriteLine(count);
                                                if (ce.GetCalendarGroup().GetID() == cg.GetID() && count < 6)
                                                {
                                                    l.Text = l.Text + ce.GetEmoji() + " ";
                                                    //Console.WriteLine("Fire");
                                                    count++;
                                                }
                                            }

                                            if (count == 1)
                                            {
                                                foreach (CalendarEvent ce in data[data_count])
                                                {
                                                    //Console.WriteLine(count);
                                                    if (ce.GetCalendarGroup().GetID() == cg.GetID() && count < 6)
                                                    {
                                                        l.Text = ce.GetEmoji() + " " + ce.GetName();
                                                        
                                                        t.SetToolTip(l, ce.GetDescription());
                                                    }
                                                }
                                            }

                                            if (visibleGroups.Contains(cg.GetID()))
                                            {
                                                if (addedCount < 5)
                                                {
                                                    p.Controls.Add(l);
                                                    addedCount++;
                                                }

                                                //else if (addedCount < 5)
                                                //{
                                                //    l = new Label() { ForeColor = txt, BackColor = Color.White, Height = 15, Margin = new Padding(0, 0, 0, 0) };
                                                //    l.Click += (s, e) =>
                                                //    {
                                                //        s = l.Parent;
                                                //        PanelClickEvent(s, e);
                                                //    };

                                                //    t.SetToolTip(l, "More Events");

                                                //    l.Text = "            ...";

                                                //    p.Controls.Add(l);
                                                //    addedCount++;
                                                //}
                                            }
                                        }
                                    }

                                    else
                                    {
                                        for (int h = 0; (h < data[data_count].Count && h < 5); h++)
                                        {
                                            Color txt = Color.Black;

                                            //Console.WriteLine(data[data_count][h].GetName() + " " + data[data_count][h].GetDateTime() + " " + data_count); // Output

                                            if (data[data_count][h].GetCalendarGroup().GetColor().GetBrightness() < 0.3)
                                            {
                                                txt = Color.White;
                                            }

                                            Label l = new Label() { Text = data[data_count][h].GetEmoji() + " " + data[data_count][h].GetName(), ForeColor = txt, BackColor = data[data_count][h].GetCalendarGroup().GetColor(), Height = 15, Margin = new Padding(0, 0, 0, 0) };
                                            l.Click += (s, e) =>
                                            {
                                                s = l.Parent;
                                                PanelClickEvent(s, e);
                                            };

                                            ToolTip t = new ToolTip
                                            {
                                                IsBalloon = true
                                            };

                                            t.SetToolTip(l, data[data_count][h].GetDescription());

                                            if (visibleGroups.Contains(data[data_count][h].GetCalendarGroup().GetID()))
                                            {
                                                p.Controls.Add(l);
                                            }
                                        }
                                    }

                                    if (switch_colour)
                                    {
                                        p.BackColor = Color.DarkGray;
                                        switch_colour = false;
                                    }

                                    else
                                    {
                                        p.BackColor = Color.LightGray;
                                        switch_colour = true;
                                    }

                                    calendar.Controls.Add(p);
                                }

                                catch (NullReferenceException)
                                {
                                    //Console.WriteLine("Empty Day " + data_count);

                                    TableLayoutPanel p = new TableLayoutPanel
                                    {
                                        Dock = DockStyle.Fill,
                                        Margin = new Padding(0, 0, 0, 0)
                                    };

                                    if (switch_colour)
                                    {
                                        p.BackColor = Color.DarkGray;
                                        switch_colour = false;
                                    }

                                    else
                                    {
                                        p.BackColor = Color.LightGray;
                                        switch_colour = true;
                                    }

                                    p.Click += (s, ev) =>
                                    {
                                        PanelClickEvent(s, ev);
                                    };

                                    calendar.Controls.Add(p);
                                }

                                catch (Exception)
                                {
                                    //Console.WriteLine(ex_var.Message);
                                }
                            }

                            data_count++;
                        }
                    }
                }
            }

            ResumeLayout();
            calendar.Visible = true;
        }

        private void RenderEmptyCells(TableLayoutPanel t, int x) // Renders empty cells for days with no events
        {
            for (int i = 0; i < x; i++)
            {
                t.Controls.Add(new Label() { Text = "" }, i, 1);
                t.Controls.Add(new Label() { Text = "" }, i, 2);
            }
        }

        public void SetData(List<List<CalendarEvent>> d, DateTime dt) // Sets the data that the calendar section looks at to display events
        {
            data = d;
            startDate = dt;

            Render();
        }

        public void PanelClickEvent(object s, EventArgs e) // Event fired when a panel within the calendar section is clicked
        {
            TableLayoutPanel temp = (TableLayoutPanel)s;
            TableLayoutPanel parent = (TableLayoutPanel)temp.Parent;
            bool found = false;

            for (int g = 0; g < 7; g++)
            {
                for (int h = 0; h < 11; h++)
                {
                    if (parent.GetControlFromPosition(g, h) == temp)
                    {
                        int datePoint = h - 1;
                        found = true;

                        if (parent.GetControlFromPosition(g, datePoint).Text.Equals(""))
                        {
                            //Console.WriteLine("Empty Field");
                        }

                        else
                        {
                            string[] numbers = Regex.Split(parent.GetControlFromPosition(g, datePoint).Text, @"\D+");
                            int index = int.Parse(numbers[0]);

                            DateTime selected_dt = new DateTime(dt.Year, dt.Month, index);

                            ResetForm();

                            MouseEventArgs me = (MouseEventArgs) e;

                            if (me.Button == MouseButtons.Left)
                            {
                                Search(selected_dt.ToString("dd/MM/yyyy"));
                                TextBox_Search.Text = selected_dt.ToString("dd/MM/yyyy");

                                Search_Panel.Visible = true;
                            }
                            
                            else if (me.Button == MouseButtons.Right)
                            {                                
                                DateTimePicker_Date.Text = selected_dt.ToLongDateString();

                                Event_Panel.Visible = true;
                            }
                            
                            Dashboard_Panel.Visible = false;
                            PictureBox_Back.Visible = true;
                        }
                    }
                }
            }

            if (!found)
            {
                //Console.WriteLine("Nothing found");
            }
        }

        private void Update_Event_Button_Click(object sender, EventArgs e) // Sets the current data input by the user over the top of the original event
        {
            DateTime datetime = new DateTime(DateTimePicker_Date.Value.Year, DateTimePicker_Date.Value.Month, DateTimePicker_Date.Value.Day, DateTimePicker_Time.Value.Hour, DateTimePicker_Time.Value.Minute, DateTimePicker_Time.Value.Second);

            List<CalendarGroup> cg = database.GetGroups();
            int group = 0;

            for (int i = 0; i < cg.Count; i++)
            {
                if (ComboBox_Group.Text == cg[i].GetName())
                {
                    group = cg[i].GetID();
                }
            }

            string location = "";

            foreach (DataGridViewRow row in Data_Location_Lookup.SelectedRows)
            {
                location = row.Cells[0].Value.ToString();
            }

            if (location == "" && !TextBox_Location_Search.Text.Equals("Enter Address or Place"))
            {
                location = TextBox_Location_Search.Text;
            }

            database.Update_Event(Convert.ToInt64(TextBox_Event_ID.Text), TextBox_Name_Event.Text, TextBox_Description.Text, datetime, location, TextBox_Location.Text, Event_TextBox_Emoji.Text, group);
            
            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);

            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
            PictureBox_Back.Visible = false;
            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
        }

        private void Remove_Event_Button_Click(object sender, EventArgs e) // Deletes current event from the database
        {
            database.Delete_Event(Convert.ToInt32(TextBox_Event_ID.Text));

            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);

            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
            PictureBox_Back.Visible = false;
            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
        }

        private void Search_Event_Update_Click(object sender, EventArgs e) // Takes the user to the update event page along with all the correct data
        {
            foreach (DataGridViewRow row in Search_Data.SelectedRows)
            {
                Int64 Event_ID = (Int64)row.Cells[0].Value;
                string Event_Name = row.Cells[1].Value.ToString();
                string Event_Description = row.Cells[2].Value.ToString();
                DateTime Event_DateTime = (DateTime)row.Cells[3].Value;
                string Event_Emoji = row.Cells[4].Value.ToString();
                int Group_ID = (int)row.Cells[5].Value;
                string Group_Name = row.Cells[6].Value.ToString();
                string Event_Location_Name = row.Cells[7].Value.ToString();

                if (!row.Cells[8].Value.ToString().Equals(","))
                {
                    string[] Event_Location = row.Cells[8].Value.ToString().Split(',');

                    double lat = Convert.ToDouble(Event_Location[0]);
                    double lng = Convert.ToDouble(Event_Location[1]);

                    GMap_Control.MapProvider = OpenStreetMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control.Position = new PointLatLng(lat, lng);
                    GMap_Control.ShowCenter = false;

                    GMap_Control.Overlays.Clear();

                    GMapOverlay markers = new GMapOverlay("markers");
                    GMap_Control.Overlays.Add(markers);

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                    markers.Markers.Add(marker);

                    TextBox_Location.Text = lat + "," + lng;
                    PictureBox_Directions.Visible = true;
                }

                TextBox_Event_ID.Text = Convert.ToString(Event_ID);
                TextBox_Name_Event.Text = Event_Name;
                TextBox_Description.Text = Event_Description;
                TextBox_Location_Search.Text = Event_Location_Name;
                ComboBox_Group.Text = Group_Name;
                Event_TextBox_Emoji.Text = Event_Emoji;
                DateTimePicker_Date.Text = Event_DateTime.ToLongDateString();
                DateTimePicker_Time.Text = Event_DateTime.ToLongTimeString();

                Search_Panel.Visible = false;
                Event_Panel.Visible = true;
                Update_Event_Button.Visible = true;
                Remove_Event_Button.Visible = true;
            }
        }

        private void Search_Username_Button_Click(object sender, EventArgs e) // Looks up the username
        {
            Search_Friends.DataSource = database.Get_Friend_Results(TextBox_Search_Username.Text);

            Search_Friends.Columns[0].Visible = false; // User ID

            Search_Friends.Columns[1].HeaderText = "User Name";

            Search_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Search_Friends_Click(object sender, EventArgs e) // Changes the button to associate with the row clicked on by the user
        {
            foreach (DataGridViewRow row in Search_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;
                string User_Name = row.Cells[1].Value.ToString();

                TextBox_Friends_Nickname.Text = User_Name;
                Add_Friend_Button.Text = "Add " + User_Name + " As A Friend!";
            }
        }

        private void Add_Friend_Button_Click(object sender, EventArgs e) // Sends the user specified a friend request
        {
            foreach (DataGridViewRow row in Search_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;
                string User_Name = row.Cells[1].Value.ToString();
                string User_Nickname;

                if (TextBox_Friends_Nickname.Text == "Enter Nickname")
                {
                    User_Nickname = "No Nickname";
                }

                else
                {
                    User_Nickname = TextBox_Friends_Nickname.Text;
                }

                database.Add_Friend(User_ID, User_Name, User_Nickname);

                int id = database.Get_Friend_Request(User_ID);

                if (id != 0)
                {
                    database.Remove_Notification(id);
                }

                else
                {
                    database.Add_Notification(User_ID, 1);
                }

                Search_Friends.DataSource = database.Get_Friend_Results(TextBox_Search_Username.Text);

                Search_Friends.Columns[0].Visible = false; // User ID

                Search_Friends.Columns[1].HeaderText = "User Name";
            }

            Data_Friends.DataSource = database.Get_Friends();

            Data_Friends.Columns[0].Visible = false; // User ID

            Data_Friends.Columns[1].HeaderText = "Friends";

            Data_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            notifications = database.Get_Notifications();

            if (notifications.Count > 0)
            {
                PictureBox_Notification.Visible = true;
            }

            else
            {
                PictureBox_Notification.Visible = false;
            }
        }

        private void TextBox_Friends_Nickname_TextChanged(object sender, EventArgs e) // Changes the button to use the nickname specified
        {
            if (Search_Friends.DataSource != null)
            {
                Add_Friend_Button.Text = "Add " + TextBox_Friends_Nickname.Text + "!";
            }
        }

        private void Setup_Friends() // Displays all the users friend from the database
        {
            List<Friend> f = database.GetFriends();

            Data_Groups.Rows.Clear();

            for (int i = 0; i < f.Count; i++)
            {
                Data_Groups.Rows.Add(false, f[i].GetID(), f[i].GetUserName(), f[i].GetNickName());
            }
        }

        private void Setup_Dashboard_Groups(bool render) // Gets all the groups that the user has and displays on the dashboard
        {
            List<int> temp = database.GetGroupIDs();

            foreach (int i in temp)
            {
                if (!visibleGroups.Contains(i))
                {
                    visibleGroups.Add(i);
                }
            }

            for (int i = 0; i < visibleGroups.Count; i++)
            {
                if (!temp.Contains(visibleGroups[i]))
                {
                    visibleGroups.Remove(visibleGroups[i]);
                }
            }

            foreach (int i in visibleGroups)
            {
                if (!temp.Contains(i))
                {
                    visibleGroups.Remove(i);
                }
            }

            Groups_Data.Rows.Clear();

            List<CalendarGroup> cg = database.GetGroups();

            foreach (CalendarGroup c in cg)
            {
                bool visible = false;

                if (visibleGroups.Contains(c.GetID()))
                {
                    visible = true;
                }

                Groups_Data.Rows.Add(c.GetID(), c.GetName(), visible);
            }

            if (render)
            {
                Render();
            }
        }

        private void Group_Data_Click(object sender, EventArgs e) // Toggles which groups are visible
        {
            foreach (DataGridViewRow row in Groups_Data.SelectedRows)
            {
                if ((bool)row.Cells[2].Value)
                {
                    row.Cells[2].Value = false;

                    visibleGroups.Remove((int)row.Cells[0].Value);

                    Render();
                }

                else
                {
                    row.Cells[2].Value = true;

                    visibleGroups.Add((int)row.Cells[0].Value);

                    Render();
                }
            }
        }

        private void Data_Groups_Click(object sender, EventArgs e) // Toggles the check on groups
        {
            foreach (DataGridViewRow row in Data_Groups.SelectedRows)
            {
                if ((bool)row.Cells[0].Value)
                {
                    row.Cells[0].Value = false;
                }

                else
                {
                    row.Cells[0].Value = true;
                }
            }
        }

        private void Find_Friends_Button_Click(object sender, EventArgs e) // Displays friend screen
        {
            PictureBox_Back.Visible = true;
            Dashboard_Panel.Visible = false;
            Friends_Panel.Visible = true;
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
                         int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void PictureBox_Drag_MouseDown(object sender, MouseEventArgs e) // Allows the user to drag the program around their display
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Settings_Light_Click(object sender, EventArgs e) // Sets the primary colour
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Settings_Light_Panel.BackColor = cd.Color;
        }

        private void Settings_Dark_Button_Click(object sender, EventArgs e) // Sets the secondary colour
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Settings_Dark_Panel.BackColor = cd.Color;
        }

        private void Settings_Commit_Click(object sender, EventArgs e) // Sets both colours in the data and then applies to program
        {
            lightColour = Settings_Light_Panel.BackColor;
            darkColour = Settings_Dark_Panel.BackColor;

            Set_Colours();

            database.Update_User_Colours(lightColour, darkColour);
        }

        private void PictureBox_Settings_Click(object sender, EventArgs e) // Changes screen to settings
        {
            if (database.Get_Facebook_Link())
            {
                Link_Facebook_Button.Text = "Unlink With Facebook";
            }

            else
            {
                Link_Facebook_Button.Text = "Link With Facebook";
            }

            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Notification_Panel.Visible = false;
            Emoji_Panel.Visible = false;
            Facebook_Panel.Visible = false;
            Busiest_Day_Panel.Visible = false;
            Dashboard_Panel.Visible = false;

            PictureBox_Back.Visible = true;
            Settings_Panel.Visible = true;

            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
        }

        private bool goodUsername = false;
        private bool goodEmail = false;
        private bool goodPassword = false;
        private bool goodPasswordRetype = false;

        private void Signup_TextBox_Username_TextChanged(object sender, EventArgs e) // Checks the entered username if is SQL safe and not already in use
        {
            if (!database.Username_Lookup(Signup_TextBox_Username.Text) && Signup_TextBox_Username.Text.Length > 0 && SQLSafe(Signup_TextBox_Username.Text))
            {
                goodUsername = true;
                Signup_Tick_Username.Visible = true;
                Signup_Cross_Username.Visible = false;
                Warning_Label.Visible = false;
            }

            else
            {
                goodUsername = false;
                Signup_Tick_Username.Visible = false;
                Signup_Cross_Username.Visible = true;
                Warning_Label.Text = "That Username is already in use.";
                Warning_Label.Visible = true;
            }
        }

        private void Signup_TextBox_Email_TextChanged(object sender, EventArgs e) // Checks the entered email if is SQL safe and not already in use
        {
            if (!database.Email_Lookup(Signup_TextBox_Email.Text) && Signup_TextBox_Email.Text.Contains("@") && Signup_TextBox_Email.Text.Contains("."))
            {
                goodEmail = true;
                Signup_Tick_Email.Visible = true;
                Signup_Cross_Email.Visible = false;
                Warning_Label.Visible = false;
            }

            else
            {
                goodEmail = false;
                Signup_Tick_Email.Visible = false;
                Signup_Cross_Email.Visible = true;
                Warning_Label.Text = "That email is in use or is not valid.";
                Warning_Label.Visible = true;
            }
        }

        private void Signup_TextBox_Password_TextChanged(object sender, EventArgs e) // Checks the entered password if is SQL safe
        {
            if (Signup_TextBox_Password.Text.Length > 6)
            {
                goodPassword = true;
                Signup_Tick_Password.Visible = true;
                Signup_Cross_Password.Visible = false;
                Warning_Label.Visible = false;
            }

            else
            {
                goodPassword = false;
                Signup_Tick_Password.Visible = false;
                Signup_Cross_Password.Visible = true;
                Warning_Label.Text = "Passwords must consist of at least 7 characters.";
                Warning_Label.Visible = true;
            }
        }

        private void Signup_TextBox_Password_Retype_TextChanged(object sender, EventArgs e) // Checks the re-typed password and password are the same
        {
            if (Signup_TextBox_Password.Text.Equals(Signup_TextBox_Password_Retype.Text) && Signup_TextBox_Password_Retype.Text.Length > 6)
            {
                goodPasswordRetype = true;
                Signup_Tick_Password_Retype.Visible = true;
                Signup_Cross_Password_Retype.Visible = false;
                Warning_Label.Visible = false;
            }

            else
            {
                goodPasswordRetype = false;
                Signup_Tick_Password_Retype.Visible = false;
                Signup_Cross_Password_Retype.Visible = true;
                Warning_Label.Text = "Typed Password does not match.";
                Warning_Label.Visible = true;
            }
        }

        private void Signup_Panel_Button_Click(object sender, EventArgs e) // Signs the user up
        {
            if (goodEmail && goodUsername && goodPassword && goodPasswordRetype)
            {
                // Adds user to database with given information, a shashed password and the default colour settings
                database.Add_User(Signup_TextBox_Username.Text, GetHashString(Signup_TextBox_Password.Text), Signup_TextBox_Email.Text, -10185235, -12490271);
                
                // Emails the signed user to notify them of their sign up
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                var mail = new MailMessage();
                mail.From = new MailAddress("calendarapplicationreed@gmail.com");
                mail.To.Add(Signup_TextBox_Email.Text);
                mail.Subject = "Calendar Application Registration";
                mail.IsBodyHtml = true;
                string htmlBody;
                htmlBody = "Hi " + Signup_TextBox_Username.Text + "!<br /><br />Welcome to a new calendar experience. You are now all ready to start using the application with the Username: " + Signup_TextBox_Username.Text + " and the password you entered.<br /><br />Kind Regards,<br />Jack Reed";
                mail.Body = htmlBody;
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential("calendarapplicationreed@gmail.com", "EC16325_Reed");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                Signup_TextBox_Username.Text = "";
                Signup_TextBox_Email.Text = "";
                Signup_TextBox_Password.Text = "";
                Signup_TextBox_Password_Retype.Text = "";

                Signup_Cross_Username.Visible = false;
                Signup_Cross_Email.Visible = false;
                Signup_Cross_Password.Visible = false;
                Signup_Cross_Password_Retype.Visible = false;

                database = null;

                Signup_Panel.Visible = false;
                Login_Panel.Visible = true;

                PictureBox_Signup_Back.Visible = false;
            }
        }

        private void PictureBox_Signup_Back_Click(object sender, EventArgs e) // Takes the user back to the login page from the sign up page
        {
            Signup_TextBox_Username.Text = "";
            Signup_TextBox_Email.Text = "";
            Signup_TextBox_Password.Text = "";
            Signup_TextBox_Password_Retype.Text = "";

            Signup_Cross_Username.Visible = false;
            Signup_Cross_Email.Visible = false;
            Signup_Cross_Password.Visible = false;
            Signup_Cross_Password_Retype.Visible = false;

            database = null;

            Login_Panel.Visible = true;
            Signup_Panel.Visible = false;

            PictureBox_Signup_Back.Visible = false;
        }

        private void Event_Button_Emoji_Click(object sender, EventArgs e) // Takes the user to the emoji screen to select an emoji
        {
            Emoji_Panel.Visible = true;
            Event_Panel.Visible = false;
        }

        private void PictureBox_Notification_Click(object sender, EventArgs e) // Takes the users to the notifications screen

        {
            TableLayoutPanel_Notifications.Controls.Clear();

            foreach (Notification n in notifications)
            {
                if (n.GetNType() == 1)
                {
                    string username = database.User_Lookup(n.GetSender());
                    Friend_Request fr = new Friend_Request(darkColour, lightColour, username);
                    fr.Add_Button.Click += (se, ev) =>
                    {
                        Search_Friends.DataSource = database.Get_Friend_Results(username);

                        Search_Friends.Columns[0].Visible = false; // User ID

                        Search_Friends.Columns[1].HeaderText = "User Name";

                        Search_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                        Notification_Panel.Visible = false;
                        Friends_Panel.Visible = true;
                    };

                    fr.Delete_Button.Click += (se, ev) =>
                    {
                        database.Remove_Notification(n.GetID());
                        TableLayoutPanel_Notifications.Controls.Remove(fr);

                        if (TableLayoutPanel_Notifications.Controls.Count == 0)
                        {
                            Notification_Panel.Visible = false;
                            Dashboard_Panel.Visible = true;
                            PictureBox_Back.Visible = false;
                        }
                    };

                    TableLayoutPanel_Notifications.Controls.Add(fr);
                }
            }

            Notification_Panel.Visible = true;
            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = false;
            PictureBox_Back.Visible = false;
            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
            PictureBox_Back.Visible = true;
        }

        private void TextBox_Name_Group_TextChanged(object sender, EventArgs e) // Checks the group name given is SQL safe and not empty
        {
            if (TextBox_Name_Group.Text.Equals("") || !SQLSafe(TextBox_Name_Group.Text))
            {
                Group_Cross_Name.Visible = true;
            }

            else
            {
                Group_Cross_Name.Visible = false;
            }
        }

        private void TextBox_Name_Event_TextChanged(object sender, EventArgs e) // Checks the event name given is SQL safe and not empty
        {
            if (TextBox_Name_Event.Text.Equals("") || !SQLSafe(TextBox_Name_Event.Text))
            {
                Event_Cross_Name.Visible = true;
            }

            else
            {
                Event_Cross_Name.Visible = false;
            }
        }

        private void TextBox_Description_TextChanged(object sender, EventArgs e) // Checks the description given is SQL safe and not empty
        {
            if (!SQLSafe(TextBox_Description.Text))
            {
                Event_Cross_Description.Visible = true;
            }

            else
            {
                Event_Cross_Description.Visible = false;
            }
        }

        private void Friends_Control_Panel_VisibleChanged(object sender, EventArgs e) // Formats the spreadsheet on the friends screen
        {
            Data_Friends.DataSource = database.Get_Friends();

            Data_Friends.Columns[0].Visible = false; // User ID

            Data_Friends.Columns[1].HeaderText = "Friends";

            Data_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Remove_Friend_Button_Click(object sender, EventArgs e) // Removes friend selected by user
        {
            foreach (DataGridViewRow row in Data_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;

                database.Remove_Friend(User_ID);
            }

            Data_Friends.DataSource = database.Get_Friends();

            Data_Friends.Columns[0].Visible = false; // User ID

            Data_Friends.Columns[1].HeaderText = "Friends";

            Data_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Update_Friend_Button_Click(object sender, EventArgs e) // Updates friends
        {
            foreach (DataGridViewRow row in Data_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;

                database.Update_Friend(User_ID, TextBox_Friends_Nickname.Text);
            }

            Data_Friends.DataSource = database.Get_Friends();

            Data_Friends.Columns[0].Visible = false; // User ID

            Data_Friends.Columns[1].HeaderText = "Friends";

            Data_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Data_Display_Groups_Click(object sender, EventArgs e) // Gets the friends for selected group
        {
            foreach (DataGridViewRow row in Data_Display_Groups.SelectedRows)
            {
                int Group_ID = (int)row.Cells[0].Value;

                List<Friend> fg = database.Get_Friends_From_Groups(Group_ID);

                List<Friend> fa = database.GetFriends();

                Data_Groups_Friends.Rows.Clear();

                foreach (Friend f in fa)
                {
                    bool found = false;
                    //Console.WriteLine("F: " + f.GetUserName());

                    foreach (Friend ff in fg)
                    {
                        //Console.WriteLine("FF: " + ff.GetUserName());
                        if (ff.GetID() == f.GetID())
                        {
                            Data_Groups_Friends.Rows.Add(true, f.GetID(), f.GetUserName(), f.GetNickName());
                            //Console.WriteLine("Found");
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        Data_Groups_Friends.Rows.Add(false, f.GetID(), f.GetUserName(), f.GetNickName());
                    }
                }

                List<CalendarGroup> lcg = database.GetGroups();

                foreach (CalendarGroup cg in lcg)
                {
                    if (cg.GetID() == Group_ID)
                    {
                        TextBox_Group_Update.Text = cg.GetName();
                        Colour_Panel_Update.BackColor = cg.GetColor();
                    }
                }
            }
        }

        private void Data_Groups_Friends_Click(object sender, EventArgs e) // Toggles the selected friend
        {
            foreach (DataGridViewRow row in Data_Groups_Friends.SelectedRows)
            {
                if ((bool)row.Cells[0].Value)
                {
                    row.Cells[0].Value = false;
                }

                else
                {
                    row.Cells[0].Value = true;
                }
            }
        }

        private void Update_Colour_Group_Click(object sender, EventArgs e) // Changes the colour of the group
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Colour_Panel_Update.BackColor = cd.Color;
        }

        private void Remove_Group_Name_Click(object sender, EventArgs e) // Removes selected group from the user
        {
            foreach (DataGridViewRow row in Data_Display_Groups.SelectedRows)
            {
                int Group_ID = (int)row.Cells[0].Value;

                database.Remove_Group(Group_ID);
            }
        }

        private void TextBox_Group_Update_TextChanged(object sender, EventArgs e) // Checks if the group name is safe for SQL
        {
            if (SQLSafe(TextBox_Group_Update.Text))
            {
                Groups_Cross_Update_Name.Visible = false;
            }

            else
            {
                Groups_Cross_Update_Name.Visible = true;
            }
        }

        private void Update_Group_Click(object sender, EventArgs e) // Updates the group with the user defined data
        {
            foreach (DataGridViewRow row in Data_Display_Groups.SelectedRows)
            {
                int Group_ID = (int)row.Cells[0].Value;

                foreach (DataGridViewRow row2 in Data_Groups_Friends.SelectedRows)
                {
                    int Friend_ID = (int)row2.Cells[1].Value;

                    if ((bool)row2.Cells[0].Value)
                    {
                        database.Remove_From_Group(Group_ID, Friend_ID);
                        database.Add_Friend_To_Group(Friend_ID, Group_ID);
                    }

                    else
                    {
                        database.Remove_From_Group(Group_ID, Friend_ID);
                    }
                }

                database.Update_Group(Group_ID, TextBox_Group_Update.Text, Colour_Panel_Update.BackColor.ToArgb());
            }

            List<CalendarGroup> g = database.GetGroups();

            Data_Display_Groups.Rows.Clear();

            for (int i = 0; i < g.Count; i++)
            {
                
                Data_Display_Groups.Rows.Add(g[i].GetID(), g[i].GetName());
            }
            
            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);
        }

        private void Data_Friends_Click(object sender, EventArgs e) // Selects user to add as friend
        {
            foreach (DataGridViewRow row in Data_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;
                string User_Name = row.Cells[1].Value.ToString();

                TextBox_Friends_Nickname.Text = User_Name;
                Add_Friend_Button.Text = "Add As Friend";
            }
        }

        private void Search_Add_Event_Button_Click(object sender, EventArgs e) // Looks for a date to try and guess date for user
        {
            string[] split = TextBox_Search.Text.Split('/');
            DateTime temp;
            try
            {
                temp = new DateTime(Convert.ToInt16(split[2]), Convert.ToInt16(split[1]), Convert.ToInt16(split[0]));
            }

            catch (IndexOutOfRangeException)
            {
                temp = DateTime.Now;
            }

            ResetForm();

            DateTimePicker_Date.Text = temp.ToLongDateString();
            
            Search_Panel.Visible = false;
            Event_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private void PictureBox_Directions_Click(object sender, EventArgs e) // Opens a browser to show google maps directions to specified location
        {
            string d = TextBox_Location.Text;

            if (userLatitude == 0 && userLongitude == 0)
            {
                Process.Start("https://www.google.com/maps/dir//" + d + "/@" + d);
            }

            else
            {
                Process.Start("https://www.google.com/maps/dir/" + userLatitude + "," + userLongitude + "/" + d + "/@" + d);
            }
        }

        string [] defaultTexts = { "Enter Search Criteria", "Enter Event Name", "Enter Address or Place", "Enter Group Name", "Enter Username", "Username", "Password", "Enter Description" };

        private void TextBox_Click(object sender, EventArgs e) // When the user clicks a textbox select the whole box if it has any default text
        {
            TextBox tb = (TextBox)sender;

            foreach (string str in defaultTexts)
            {
                if (str == tb.Text)
                {
                    tb.SelectAll();
                    break;
                }
            }
        }

        private void RichTextBox_Click(object sender, EventArgs e) // When the user clicks a rich textbox select the whole box if it has any default text
        {
            RichTextBox tb = (RichTextBox)sender;

            foreach (string str in defaultTexts)
            {
                if (str == tb.Text)
                {
                    tb.SelectAll();
                    break;
                }
            }
        }
        
        private void TextBox_Location_Search_TextChanged(object sender, EventArgs e) // Waits for search suggestions and then displays them
        {
            if (TextBox_Location_Search.Text.Length > 0)
            {
                Data_Location_Lookup.Rows.Clear();

                var request = new PlacesAutoCompleteRequest
                {
                    Key = "AIzaSyDEJCMJ2qejdJhHnhMHOJLBemgHXxaeqe4",
                    Input = TextBox_Location_Search.Text
                };

                var response = GooglePlaces.AutoComplete.Query(request);

                foreach (var a in response.Predictions)
                {
                    Data_Location_Lookup.Rows.Add(a.Description);
                }
            }
        }

        private void Data_Location_Lookup_Click(object sender, EventArgs e) // Looks up lat and lng for exact location from suggested text
        {
            foreach (DataGridViewRow row in Data_Location_Lookup.SelectedRows)
            {
                string loc = row.Cells[0].Value.ToString();

                var request_query = new PlacesTextSearchRequest
                {
                    Key = "AIzaSyDEJCMJ2qejdJhHnhMHOJLBemgHXxaeqe4",
                    Query = loc
                };

                var response_query = GooglePlaces.TextSearch.Query(request_query);

                double lat = 0.0;
                double lng = 0.0;

                foreach (var b in response_query.Results)
                {
                    lat = b.Geometry.Location.Latitude;
                    lng = b.Geometry.Location.Longitude;
                }

                PointLatLng latLng = new PointLatLng(lat, lng);
                GMap_Control.Position = latLng;

                GMap_Control.Overlays.Clear();

                GMapOverlay markers = new GMapOverlay("markers");
                GMap_Control.Overlays.Add(markers);

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                markers.Markers.Add(marker);

                TextBox_Location.Text = lat + "," + lng;
            }
        }

        private void Facebook_GetEvents() // Get events from Facebook
        {
            var facebookClient = new FacebookClient();
            var facebookService = new FacebookService(facebookClient);

            var getEventTask = facebookService.GetEventAsync(FacebookSettings.AccessToken);

            Task.WaitAll(getEventTask);
            var events = getEventTask.Result;

            int group = database.Group_Facebook(Color.RoyalBlue.ToArgb());

            foreach (Event ev in events)
            {
                //Console.WriteLine("=================================");
                //Console.WriteLine($"ID: {ev.Id}");
                //Console.WriteLine($"Name: {ev.Name}");
                //Console.WriteLine($"Place: {ev.Place}");
                //Console.WriteLine($"Description: {ev.Description}");
                //Console.WriteLine($"Date: {ev.Date}");
                //Console.WriteLine("\n");

                string loc = ev.Place;

                var request_query = new PlacesTextSearchRequest
                {
                    Key = "AIzaSyDEJCMJ2qejdJhHnhMHOJLBemgHXxaeqe4",
                    Query = loc
                };

                var response_query = GooglePlaces.TextSearch.Query(request_query);

                double lat = 0.0;
                double lng = 0.0;

                foreach (var b in response_query.Results)
                {
                    lat = b.Geometry.Location.Latitude;
                    lng = b.Geometry.Location.Longitude;
                }

                loc = lat + "," + lng;

                Int64 id;
                Int64.TryParse(ev.Id, out id);

                string[] split = ev.Date.Split('/');
                int day;
                int.TryParse(split[1], out day);
                int month;
                int.TryParse(split[0], out month);

                split = split[2].Split(' ');

                int year;
                int.TryParse(split[0], out year);

                split = split[1].Split(':');

                int hour;
                int.TryParse(split[0], out hour);
                int minute;
                int.TryParse(split[1], out minute);
                int second;
                int.TryParse(split[2], out second);

                DateTime temp = new DateTime(year, month, day, hour, minute, second);

                string emoji = "X";

                string[] split_desc = (ev.Name + " " + ev.Description + " " + ev.Place).Split(' ');

                bool break_loop = false;

                foreach (string str in split_desc)
                {
                    foreach (Emoji e in database.Get_Emojis())
                    {
                        if (str.ToLower().Equals(e.GetName().ToLower()))
                        {
                            //Console.WriteLine("1: " + emoji + " " +  ev.Name);
                            emoji = e.GetName();
                            break_loop = true;
                            //Console.WriteLine("2: " + emoji + " " + ev.Name);
                            break;
                        }
                    }

                    if (break_loop)
                    {
                        break;
                    }
                }

                //Console.WriteLine("3: " + emoji + " " + ev.Name);

                database.Event_Facebook(id, ev.Name, ev.Description, temp, ev.Place, loc, emoji, group);
            }

            database.User_Linked_Facebook();
        }

        private void Link_Facebook_Button_Click(object sender, EventArgs e)
        {
            if (database.Get_Facebook_Link())
            {
                database.Remove_Facebook_Link();

                Link_Facebook_Button.Text = "Link With Facebook";
            }

            else
            {
                Facebook_Browser.Navigate("https://www.facebook.com/v3.2/dialog/oauth?client_id=1227276437422824&redirect_uri=https://www.facebook.com/connect/login_success.html&state={st=state123abc,ds=123456789}&scope=user_events"); // Requests user to re-login

                Facebook_Browser.Visible = true;
                Facebook_Panel.Visible = true;
                Settings_Panel.Visible = false;
                PictureBox_Back.Visible = true;
            }
        }

        private void Slider_Control_ValueChanged(object sender, EventArgs e)
        {
            Slider_Value.Text = Convert.ToString("Find Days With " + Slider_Control.Value + " Events:");

            Data_Busy_Days.DataSource = database.Get_Busiest_Day(DateTime_Start_Busy.Value, DateTime_End_Busy.Value, Slider_Control.Value);

            Data_Busy_Clean_Up();
        }

        private void Busiest_Days_Button_Click(object sender, EventArgs e)
        {
            Slider_Control.Maximum = database.Get_Busiest_Max();
            Slider_Control.Minimum = database.Get_Busiest_Min();

            Slider_Value.Text = Convert.ToString("Find Days With " + Slider_Control.Minimum + " Events:");

            Data_Busy_Days.DataSource = database.Get_Busiest_Day(DateTime_Start_Busy.Value, DateTime_End_Busy.Value, Slider_Control.Value);

            Data_Busy_Clean_Up();

            Search_Panel.Visible = false;
            Busiest_Day_Panel.Visible = true;
        }

        private void DateTime_Start_Busy_ValueChanged(object sender, EventArgs e)
        {
            Data_Busy_Days.DataSource = database.Get_Busiest_Day(DateTime_Start_Busy.Value, DateTime_End_Busy.Value, Slider_Control.Value);

            Data_Busy_Clean_Up();
        }

        private void Data_Busy_Clean_Up()
        {
            Data_Busy_Days.Columns[0].Visible = false; // User ID
            Data_Busy_Days.Columns[1].HeaderText = "Name";
            Data_Busy_Days.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data_Busy_Days.Columns[2].HeaderText = "Description";
            Data_Busy_Days.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data_Busy_Days.Columns[3].HeaderText = "Date";
            Data_Busy_Days.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data_Busy_Days.Columns[4].HeaderText = "Emoji";
            Data_Busy_Days.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data_Busy_Days.Columns[5].Visible = false; // Group ID
            Data_Busy_Days.Columns[6].Visible = false; // Group Name
            Data_Busy_Days.Columns[7].HeaderText = "Location";
            Data_Busy_Days.Columns[7].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Data_Busy_Days.Columns[8].Visible = false; // Geo
            Data_Busy_Days.Columns[9].Visible = false; // Count

            foreach (DataGridViewRow row in Data_Busy_Days.Rows)
            {
                row.Cells[4].Value = database.Emoji((string)row.Cells[4].Value, false);
            }
        }

        private void Select_Button_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Data_Busy_Days.SelectedRows)
            {
                Int64 Event_ID = (Int64)row.Cells[0].Value;
                string Event_Name = row.Cells[1].Value.ToString();
                string Event_Description = row.Cells[2].Value.ToString();
                DateTime Event_DateTime = (DateTime)row.Cells[3].Value;
                string Event_Emoji = row.Cells[4].Value.ToString();
                int Group_ID = (int)row.Cells[5].Value;
                string Group_Name = row.Cells[6].Value.ToString();
                string Event_Location_Name = row.Cells[7].Value.ToString();

                if (!row.Cells[8].Value.ToString().Equals(","))
                {
                    string[] Event_Location = row.Cells[8].Value.ToString().Split(',');

                    double lat = Convert.ToDouble(Event_Location[0]);
                    double lng = Convert.ToDouble(Event_Location[1]);

                    GMap_Control.MapProvider = OpenStreetMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control.Position = new PointLatLng(lat, lng);
                    GMap_Control.ShowCenter = false;

                    GMap_Control.Overlays.Clear();

                    GMapOverlay markers = new GMapOverlay("markers");
                    GMap_Control.Overlays.Add(markers);

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                    markers.Markers.Add(marker);

                    TextBox_Location.Text = lat + "," + lng;
                    PictureBox_Directions.Visible = true;
                }

                TextBox_Event_ID.Text = Convert.ToString(Event_ID);
                TextBox_Name_Event.Text = Event_Name;
                TextBox_Description.Text = Event_Description;
                TextBox_Location_Search.Text = Event_Location_Name;
                ComboBox_Group.Text = Group_Name;
                Event_TextBox_Emoji.Text = Event_Emoji;
                DateTimePicker_Date.Text = Event_DateTime.ToLongDateString();
                DateTimePicker_Time.Text = Event_DateTime.ToLongTimeString();

                Busiest_Day_Panel.Visible = false;
                Event_Panel.Visible = true;
                Update_Event_Button.Visible = true;
                Remove_Event_Button.Visible = true;
            }
        }

        private void Facebook_Browser_Navigated(object sender, GeckoNavigatedEventArgs e)
        {
            if (Facebook_Browser.Url.ToString().Contains("https://www.facebook.com/connect/login_success.html"))
            {
                Facebook_Browser.Visible = false;

                string[] split = Facebook_Browser.Url.ToString().Split('=');
                split = split[1].Split('&');

                string html = string.Empty;
                string url = @"https://graph.facebook.com/v3.2/oauth/access_token?client_id=1227276437422824&redirect_uri=https://www.facebook.com/connect/login_success.html&client_secret=9560c5077c5d7e2ab45763f05a473fb0&code=" + split[0];

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                }

                JObject json = JObject.Parse(html);

                string token = (string)json["access_token"];

                FacebookSettings.AccessToken = token;

                Facebook_GetEvents();

                nsICookieManager CookieMan;
                CookieMan = Xpcom.GetService<nsICookieManager>("@mozilla.org/cookiemanager;1");
                CookieMan = Xpcom.QueryInterface<nsICookieManager>(CookieMan);
                CookieMan.RemoveAll();

                // https://stackoverflow.com/questions/13513063/gecko-clear-cache-history-cookies/26111307

                SetData(database.GetData(dt), dt);

                ResetForm();

                PictureBox_Back.Visible = false;
                Dashboard_Panel.Visible = true;
                Facebook_Browser.Visible = true;
                Facebook_Panel.Visible = false;
                PictureBox_Logout.Visible = true;
                PictureBox_Settings.Visible = true;
            }
        }

        private void Link_Skip_Button_Click(object sender, EventArgs e)
        {
            ResetForm();

            Facebook_Panel.Visible = false;
            Dashboard_Panel.Visible = true;
            PictureBox_Back.Visible = false;
        }
    }

    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
    }

    public class Friend
    {
        private int id;
        private string username;
        private string nickname;

        public Friend(int i, string u, string n)
        {
            id = i;
            username = u;
            nickname = n;
        }

        public int GetID()
        {
            return id;
        }

        public string GetUserName()
        {
            return username;
        }

        public string GetNickName()
        {
            return nickname;
        }
    }

    public class Database
    {
        private SqlConnection connection;
        private User user;
        private List<CalendarGroup> groups;
        private List<Friend> friends;
        private List<Emoji> emojis;

        public Database(string u, string p)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = @"R33D3Y.ddns.net\SQLEXPRESS";
            builder["User ID"] = "DB_Reader";
            builder["Password"] = "Reed_DB_Reader";
            builder["Database"] = "Final_Year_Project";

            connection = new SqlConnection(builder.ToString());

            user = LoginUser(u, p);
        }

        public Database()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder["Server"] = @"R33D3Y.ddns.net\SQLEXPRESS";
            builder["User ID"] = "DB_Reader";
            builder["Password"] = "Reed_DB_Reader";
            builder["Database"] = "Final_Year_Project";

            connection = new SqlConnection(builder.ToString());
        }

        public bool Test_Connection()
        {
            try
            {
                connection.Open();
                connection.Close();
            }

            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private User LoginUser(string username, string password)
        {
            SqlCommand cmd = new SqlCommand("Login_User", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = username;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = password;

            connection.Open();
             
            SqlDataReader rdr = cmd.ExecuteReader();

            string u = "";
            int i = 0;

            if (rdr.Read())
            {
                i = (int)rdr[0];
                u = (string)rdr[1];
            }
            
            else
            {
                connection.Close();
                
                return null;
            }

            connection.Close();

            return new User(i, u);
        }

        public void Update_User_Colours(Color light, Color dark)
        {
            SqlCommand cmd = new SqlCommand("Update_User_Colours", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Light", SqlDbType.Int).Value = light.ToArgb();
            cmd.Parameters.Add("@Dark", SqlDbType.Int).Value = dark.ToArgb();

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public List<Color> Get_User_Colours()
        {
            SqlCommand cmd = new SqlCommand("Get_User_Colours", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.VarChar).Value = user.GetID();

            List<Color> temp = new List<Color>();
            
            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                temp.Add(Color.FromArgb((int)rdr[0]));
                temp.Add(Color.FromArgb((int)rdr[1]));
            }

            connection.Close();

            return temp;
        }

        public void Add_Event(string name, string description, DateTime datetime, string locationName, string locationGeo, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Add_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji, true);
            cmd.Parameters.Add("@Location_Name", SqlDbType.VarChar).Value = locationName;
            cmd.Parameters.Add("@Location_Geo", SqlDbType.VarChar).Value = locationGeo;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Event_Facebook(Int64 id, string name, string description, DateTime datetime, string locationName, string locationGeo, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Add_Event_Facebook", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = emoji;
            cmd.Parameters.Add("@Location_Name", SqlDbType.VarChar).Value = locationName;
            cmd.Parameters.Add("@Location_Geo", SqlDbType.VarChar).Value = locationGeo;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Update_Event(Int64 id, string name, string description, DateTime datetime, string locationName, string locationGeo, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Update_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.BigInt).Value = id;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji, true);
            cmd.Parameters.Add("@Location_Name", SqlDbType.VarChar).Value = locationName;
            cmd.Parameters.Add("@Location_Geo", SqlDbType.VarChar).Value = locationGeo;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Delete_Event(int id)
        {
            SqlCommand cmd = new SqlCommand("Remove_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public int Add_Group(string name, int colour)
        {
            SqlCommand cmd = new SqlCommand("Add_Group", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@User_ID", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Colour", SqlDbType.Int).Value = colour;

            int id = 0;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                id = Convert.ToInt32(rdr[0]);
            }

            connection.Close();

            return id;
        }

        public int Group_Facebook(int colour)
        {
            SqlCommand cmd = new SqlCommand("Add_Group_Facebook", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Colour", SqlDbType.Int).Value = colour;

            int id = 0;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                id = Convert.ToInt32(rdr[0]);
            }

            connection.Close();

            return id;
        }

        public void Add_Friend_To_Group(int user, int group)
        {
            SqlCommand cmd = new SqlCommand("Add_Friend_To_Group", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Add_Friend(int id, string name, string nickname)
        {
            SqlCommand cmd = new SqlCommand("Add_Friend", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID_1", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@ID_2", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Name_1", SqlDbType.VarChar).Value = user.GetUsername();
            cmd.Parameters.Add("@Name_2", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Nickname_1", SqlDbType.VarChar).Value = user.GetUsername();
            cmd.Parameters.Add("@Nickname_2", SqlDbType.VarChar).Value = nickname;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public List<Friend> GetFriends()
        {
            SqlCommand cmd = new SqlCommand("Get_Friends", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            friends = new List<Friend>();

            while (rdr.Read())
            {
                friends.Add(new Friend((int)rdr[0], (string)rdr[1], (string)rdr[2]));
            }

            connection.Close();

            return friends;
        }

        public void SetEmojis(List<Emoji> le)
        {
            emojis = le;
        }

        public BindingSource Get_Search_Results(string text)
        {
            if (text == "Enter Search Criteria")
            {
                text = "%";
            }

            else
            {
                text = "%" + text + "%";
            }

            SqlDataAdapter sqa = new SqlDataAdapter("" +
                "SELECT Event_ID, Event_Name, Event_Description, Event_DateTime, Event_Emoji, Group_ID, Group_Name, Event_Location_Name, Event_Location_Geo FROM [Final_Year_Project].[dbo].[Event_Table] " +
                "FULL JOIN Group_Table On Group_Table.Group_ID = Event_Table.Event_Group " +
                "WHERE (Event_Owner = " + user.GetID() + " OR Group_Owner = " + user.GetID() + " OR EXISTS (Select * From Group_Association_Table Where Group_ID = Group_Table.Group_ID AND User_ID = " + user.GetID() + ")) " +
                "AND Event_ID IS NOT NULL " +
                "AND (Event_Name Like '" + text + "' " +
                "OR Event_Description Like '" + text + "' " +
                "OR CONVERT(VARCHAR(10), Event_DateTime, 103) like '" + text + "' " +
                "OR Event_Emoji Like '" + text + "' " +
                "OR Event_Location_Name Like '" + text + "' " +
                "OR Group_Name Like '" + text + "')", connection);

            connection.Open();

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            sqa.Fill(table);

            connection.Close();

            BindingSource bs = new BindingSource();
            bs.DataSource = table;

            return bs;
        }

        public BindingSource Get_Friend_Results(string text)
        {
            if (text == "Enter Username")
            {
                text = "%";
            }

            else
            {
                text = "%" + text + "%";
            }

            SqlDataAdapter sqa = new SqlDataAdapter("" +
                "SELECT User_ID, User_Name " +
                "FROM [Final_Year_Project].[dbo].[User_Table] " +
                "WHERE User_ID != " + user.GetID() + " " +
                "AND User_ID NOT IN (SELECT User_ID_2 " +
                                    "FROM[Final_Year_Project].[dbo].[Friends_Table] " +
                                    "WHERE User_ID_1 = " + user.GetID() + ")", connection);

            connection.Open();

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            sqa.Fill(table);

            connection.Close();

            BindingSource bs = new BindingSource();
            bs.DataSource = table;

            return bs;
        }

        private void ClearDB()
        {
            SqlCommand cmd = new SqlCommand("Clear_Event_Table", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public List<List<CalendarEvent>> GetData(DateTime dt)
        {
            SqlCommand cmd = new SqlCommand("View_Month", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = dt;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            List<List<CalendarEvent>> data = new List<List<CalendarEvent>>();
            List<CalendarEvent> tempList = new List<CalendarEvent>();

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            int day = 1;

            while (rdr.Read())
            {
                //Console.WriteLine("CONDITION 1: " + tempList[0].GetDateTime().Date);
                //Console.WriteLine("Incoming Event Date: " + ((DateTime)rdr[3]).Date);

                if (tempList.Count > 0 && tempList[0].GetDateTime().Date != ((DateTime)rdr[3]).Date)
                {
                    //Console.WriteLine("Reset List " + day);

                    data.Add(tempList);
                    tempList = new List<CalendarEvent>();
                    day++;
                }

                //Console.WriteLine("Incoming Day " + ((DateTime)rdr[3]).Day + " Day " + day);

                while (day != ((DateTime)rdr[3]).Day)
                {
                    //Console.WriteLine("Skip Day " + day);
                    tempList = new List<CalendarEvent>();
                    tempList.Add(null);
                    data.Add(tempList);
                    tempList = new List<CalendarEvent>();
                    day++;
                }

                //Console.WriteLine("Add Day " + day);
                tempList.Add(new CalendarEvent((Int64)rdr[0], (string)rdr[1], (string)rdr[2], (DateTime)rdr[3], (string)rdr[5], (string)rdr[6], Emoji((string)rdr[4], false), new CalendarGroup((int)rdr[9], (string)rdr[10], Color.FromArgb((int)rdr[12]))));
            }

            data.Add(tempList);

            while (day != 32)
            {
                tempList = new List<CalendarEvent>();
                tempList.Add(null);
                data.Add(tempList);
                tempList = new List<CalendarEvent>();
                day++;
            }

            connection.Close();

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    if (data[i][j] != null)
                    {
                        //Console.WriteLine(i + " " + data[i][j].GetDateTime() + " " + data[i][j].GetName() + " " + data[i][j].GetDateTime().Day);
                    }

                    else
                    {
                        //Console.WriteLine(i + " Empty Day");
                    }
                }
            }

            return data;
        }

        public User GetUser()
        {
            return user;
        }

        public void SetData(List<List<CalendarEvent>> data)
        {
            connection.Open();

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < data[i].Count; j++)
                {
                    if (data[i][j] != null)
                    {
                        SqlCommand cmd = new SqlCommand("Update_Event", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = data[i][j].GetID();
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = data[i][j].GetName();
                        cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = data[i][j].GetDescription();
                        cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = data[i][j].GetDateTime();
                        cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(data[i][j].GetEmoji(), true);
                        cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = data[i][j].GetLocationGeo();
                        cmd.Parameters.Add("@Group", SqlDbType.Int).Value = data[i][j].GetCalendarGroup().GetID();

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            connection.Close();
        }

        private void RetrieveGroups()
        {
            SqlCommand cmd = new SqlCommand("Get_Groups", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            groups = new List<CalendarGroup>();

            while (rdr.Read())
            {
                groups.Add(new CalendarGroup((int)rdr[0], (string)rdr[1], Color.FromArgb((int)rdr[2])));
            }

            connection.Close();
        }

        public List<CalendarGroup> GetGroups()
        {
            RetrieveGroups();

            return groups;
        }

        public List<int> GetGroupIDs()
        {
            RetrieveGroups();

            List<int> gs = new List<int>();

            foreach (CalendarGroup g in groups)
            {
                gs.Add(g.GetID());
            }

            return gs;
        }

        public string Emoji(string i, bool name)
        {
            foreach (Emoji emoji in emojis)
            {
                if (name)
                {
                    if (emoji.GetIcon().Equals(i))
                    {
                        return emoji.GetName();
                    }
                }

                else
                {
                    if (emoji.GetName().Equals(i))
                    {
                        return emoji.GetIcon();
                    }
                }
            }

            return "X";
        }

        public bool Email_Lookup(string text)
        {
            SqlCommand cmd = new SqlCommand("Email_Lookup", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = text;

            bool exsists = false;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                exsists = (bool)rdr[0];
            }

            connection.Close();

            return exsists;
        }

        public bool Username_Lookup(string text)
        {
            SqlCommand cmd = new SqlCommand("Username_Lookup", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = text;

            bool exsists = false;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                exsists = (bool)rdr[0];
            }

            connection.Close();

            return exsists;
        }

        public void Add_User(string text1, string text2, string text3, int v1, int v2)
        {
            SqlCommand cmd = new SqlCommand("Create_User", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Username", SqlDbType.VarChar).Value = text1;
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = text2;
            cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = text3;
            cmd.Parameters.Add("@Light", SqlDbType.Int).Value = v1;
            cmd.Parameters.Add("@Dark", SqlDbType.Int).Value = v2;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void Add_Notification(int user_ID, int v)
        {
            SqlCommand cmd = new SqlCommand("Add_Notification", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Sender_User_ID", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Reciever_User_ID", SqlDbType.Int).Value = user_ID;
            cmd.Parameters.Add("@Type", SqlDbType.Int).Value = v;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void Remove_Notification(int id)
        {
            SqlCommand cmd = new SqlCommand("Remove_Notification", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Notification_ID", SqlDbType.Int).Value = id;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public List<Notification> Get_Notifications()
        {
            SqlCommand cmd = new SqlCommand("Get_Notifications", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.Int).Value = user.GetID();

            List<Notification> notifications = new List<Notification>();

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                notifications.Add(new Notification((int)rdr[0], (int)rdr[1], (int)rdr[2], (int)rdr[3]));
            }

            connection.Close();

            return notifications;
        }

        public int Get_Friend_Request(int sender)
        {
            SqlCommand cmd = new SqlCommand("Get_Friend_Request", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Sender_ID", SqlDbType.Int).Value = sender;
            cmd.Parameters.Add("@Reciever_ID", SqlDbType.Int).Value = user.GetID();

            int id = 0;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                id = (int)rdr[0];
            }

            connection.Close();

            return id;
        }

        public string User_Lookup(int v)
        {
            SqlCommand cmd = new SqlCommand("User_Lookup", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = v;

            string username = "";

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                username = (string)rdr[0];
            }

            connection.Close();

            return username;
        }

        public BindingSource Get_Friends()
        {
            SqlDataAdapter sqa = new SqlDataAdapter("" +
                "SELECT User_ID_2, User_Nickname_2 " +
                "FROM Friends_Table " +
                "WHERE User_ID_1 = " + user.GetID() + "" +
                "AND EXISTS (Select * From Friends_Table Where User_ID_2 = " + user.GetID() + ")", connection);

            connection.Open();

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            sqa.Fill(table);

            connection.Close();

            BindingSource bs = new BindingSource();
            bs.DataSource = table;

            return bs;
        }

        public void Remove_Friend(int id)
        {
            SqlCommand cmd = new SqlCommand("Remove_Friend", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Friend_ID", SqlDbType.Int).Value = id;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        internal void Update_Friend(int user_ID, string text)
        {
            SqlCommand cmd = new SqlCommand("Update_Friend", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User_ID", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Friend_ID", SqlDbType.Int).Value = user_ID;
            cmd.Parameters.Add("@Nickname", SqlDbType.VarChar).Value = text;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public List<Friend> Get_Friends_From_Groups(int g)
        {
            SqlCommand cmd = new SqlCommand("Get_Friends_From_Groups", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = g;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            friends = new List<Friend>();

            while (rdr.Read())
            {
                friends.Add(new Friend((int)rdr[0], (string)rdr[1], (string)rdr[2]));
            }

            connection.Close();

            return friends;
        }

        public bool Get_Facebook_Link()
        {
            SqlCommand cmd = new SqlCommand("Get_Facebook_Link", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            bool linked = false;

            while (rdr.Read())
            {
                if ((int)rdr[0] == user.GetID())
                {
                    linked = true;
                }
            }

            connection.Close();

            return linked;
        }

        public void Remove_Group(int group_ID)
        {
            SqlCommand cmd = new SqlCommand("Remove_Group", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Group_ID", SqlDbType.Int).Value = group_ID;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void Remove_From_Group(int group_ID, int friend_ID)
        {
            SqlCommand cmd = new SqlCommand("Remove_From_Group", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Group_ID", SqlDbType.Int).Value = group_ID;
            cmd.Parameters.Add("@Friend_ID", SqlDbType.Int).Value = friend_ID;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void Update_Group(int group_ID, string text, int v)
        {
            SqlCommand cmd = new SqlCommand("Update_Group", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = group_ID;
            cmd.Parameters.Add("@Group_Name", SqlDbType.VarChar).Value = text;
            cmd.Parameters.Add("@Group_Colour", SqlDbType.Int).Value = v;

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public void User_Linked_Facebook()
        {
            SqlCommand cmd = new SqlCommand("User_Linked_Facebook", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = user.GetID();

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public BindingSource Get_Busiest_Day(DateTime start, DateTime end, int amount)
        {
            SqlCommand cmd = new SqlCommand("Get_Busiest_Days", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();
            cmd.Parameters.Add("@Date_Start", SqlDbType.Date).Value = start;
            cmd.Parameters.Add("@Date_End", SqlDbType.Date).Value = end;
            cmd.Parameters.Add("@Amount", SqlDbType.Int).Value = amount;

            SqlDataAdapter sqa = new SqlDataAdapter(cmd);

            connection.Open();

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;
            sqa.Fill(table);

            connection.Close();

            BindingSource bs = new BindingSource();
            bs.DataSource = table;

            return bs;
        }

        public int Get_Busiest_Min()
        {
            SqlCommand cmd = new SqlCommand("Get_Min_Busiest_Days", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            int amount = 0;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                amount = (int)rdr[0];
            }

            connection.Close();

            return amount;
        }

        public int Get_Busiest_Max()
        {
            SqlCommand cmd = new SqlCommand("Get_Max_Busiest_Days", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            int amount = 0;

            connection.Open();

            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                amount = (int)rdr[0];
            }

            connection.Close();

            return amount;
        }

        public void Remove_Facebook_Link()
        {
            SqlCommand cmd = new SqlCommand("Remove_Facebook_Link", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@User", SqlDbType.Int).Value = user.GetID();

            connection.Open();

            cmd.ExecuteNonQuery();

            connection.Close();
        }

        public List<Emoji> Get_Emojis()
        {
            return emojis;
        }
    }

    public class Emoji
    {
        private string name;
        private string icon;

        public Emoji(string n, string i)
        {
            name = n;
            icon = i;
        }

        public string GetName()
        {
            return name;
        }

        public string GetIcon()
        {
            return icon;
        }
    }

    public class Notification
    {
        private int id;
        private int type;
        private int sender;
        private int reciever;

        public Notification(int a, int b, int c, int d)
        {
            id = a;
            type = b;
            sender = c;
            reciever = d;
        }

        public int GetID() { return id; }

        public int GetNType() { return type; }

        public int GetSender() { return sender; }

        public int GetReciever() { return reciever; }
    }

    public class User
    {
        private int id;
        private string username;

        public User(int i, string u)
        {
            id = i;
            username = u;
        }

        public int GetID()
        {
            return id;
        }

        public string GetUsername()
        {
            return username;
        }
    }

    public class CalendarGroup
    {
        private int id;
        private string name;
        private Color color;

        public CalendarGroup(int i, string n, Color c)
        {
            id = i;
            name = n;
            color = c;
        }

        public int GetID()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        public Color GetColor()
        {
            return color;
        }
    }

    public class CalendarEvent
    {
        private Int64 id;
        private string name;
        private string description;
        private DateTime dateTime;
        private string locationName;
        private string locationGeo;
        private string emoji;
        private CalendarGroup group;

        public CalendarEvent(Int64 i, string n, string d, DateTime dt, string ln, string lg, string e, CalendarGroup g)
        {
            id = i;
            name = n;
            description = d;
            dateTime = dt;
            locationName = ln;
            locationGeo = lg;
            emoji = e;
            group = g;
        }

        public Int64 GetID()
        {
            return id;
        }

        public string GetName()
        {
            return name;
        }

        public DateTime GetDateTime()
        {
            return dateTime;
        }

        public string GetLocationName()
        {
            return locationName;
        }

        public string GetLocationGeo()
        {
            return locationGeo;
        }

        public CalendarGroup GetCalendarGroup()
        {
            return group;
        }

        public string GetDescription()
        {
            return description;
        }

        public string GetEmoji()
        {
            return emoji;
        }
    }
}

/*
 * TODO -
    * Create tests
    * Speed and integrity improvements
 * References -
     * Logo: https://www.logolynx.com/topic/calendar
     * Icons: https://icons8.com/
     * Arrow Images: https://emojipedia.org/softbank/
     * MS MySQL: https://www.microsoft.com/en-us/download/details.aspx?id=54257
     * Map:
         * Bing - https://www.bing.com/maps
         * NuGet Package - https://archive.codeplex.com/?p=greatmaps
         * Tutorial - http://www.independent-software.com/gmap-net-beginners-tutorial-maps-markers-polygons-routes-updated-for-vs2015-and-gmap1-7.html
     * Code Assitance:
         * Code Project - https://www.codeproject.com/
         * Stack Overflow - https://stackoverflow.com/
*/
