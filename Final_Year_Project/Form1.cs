using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Final_Year_Project
{
    public partial class Form1 : Form
    {
        private Database database;
        
        private DateTime dt;
        private List<int> visibleGroups = new List<int>();
        private List<Emoji> emojis;
        private List<Notification> notifications = new List<Notification>();

        private Color lightColour = Color.CornflowerBlue;
        private Color darkColour = Color.RoyalBlue;

        private readonly bool populate = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (CheckForInternetConnection())
            {
                PictureBox_Internet.Visible = false;
                Label_Internet.Visible = false;
            }

            else
            {
                Login_Button.Enabled = false;
                SignUp_Button.Enabled = false;
            }

            Set_Colours();
        }

        private bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
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

            Dashboard_Add_Event.ForeColor = lightColour;
            Dashboard_Add_Group.ForeColor = lightColour;
            Dashboard_Search_Button.ForeColor = lightColour;
            Find_Friends_Button.ForeColor = lightColour;

            Dashboard_Control_Panel.BackColor = darkColour;
            Dashboard_Search.BackColor = darkColour;
            Groups_Data.BackgroundColor = darkColour;
            Groups_Data.DefaultCellStyle.BackColor = darkColour;
            Groups_Data.DefaultCellStyle.SelectionForeColor = lightColour;

            // Event Panel
            Event_Panel.BackColor = lightColour;
            Event_Control_Panel.BackColor = darkColour;

            TextBox_Name_Event.BackColor = darkColour;
            TextBox_Description.BackColor = darkColour;
            ComboBox_Group.BackColor = darkColour;
            Event_TextBox_Emoji.BackColor = darkColour;
            TextBox_Location_Search.BackColor = darkColour;

            Event_Button_Emoji.ForeColor = lightColour;
            Search_Location_Button.ForeColor = lightColour;
            Add_Event_Button.ForeColor = lightColour;
            Update_Event_Button.ForeColor = lightColour;
            Remove_Event_Button.ForeColor = lightColour;
            Map_Type_Button.ForeColor = lightColour;

            // Friends Panel
            Friends_Panel.BackColor = lightColour;
            Friends_Control_Panel.BackColor = darkColour;

            TextBox_Search_Username.BackColor = darkColour;
            TextBox_Friends_Nickname.BackColor = darkColour;

            Search_Username_Button.ForeColor = lightColour;
            Add_Friend_Button.ForeColor = lightColour;
            Remove_Friend_Button.ForeColor = lightColour;
            Update_Friend_Button.ForeColor = lightColour;
            Search_Friends.BackgroundColor = darkColour;
            Search_Friends.DefaultCellStyle.BackColor = darkColour;
            Search_Friends.DefaultCellStyle.SelectionForeColor = lightColour;
            Data_Friends.BackgroundColor = darkColour;
            Data_Friends.DefaultCellStyle.BackColor = darkColour;
            Data_Friends.DefaultCellStyle.SelectionForeColor = lightColour;

            // Group Panel
            Group_Panel.BackColor = lightColour;
            Group_Control_Panel.BackColor = darkColour;

            TextBox_Name_Group.BackColor = darkColour;
            TextBox_Group_Update.BackColor = darkColour;

            ColourPicker_Button.ForeColor = lightColour;
            Add_Group_Button.ForeColor = lightColour;
            Update_Group.ForeColor = lightColour;
            Update_Colour_Group.ForeColor = lightColour;
            Remove_Group_Name.ForeColor = lightColour;

            Data_Groups.BackgroundColor = darkColour;
            Data_Groups.DefaultCellStyle.BackColor = darkColour;
            Data_Groups.DefaultCellStyle.SelectionForeColor = lightColour;
            Data_Display_Groups.BackgroundColor = darkColour;
            Data_Display_Groups.DefaultCellStyle.BackColor = darkColour;
            Data_Display_Groups.DefaultCellStyle.SelectionForeColor = lightColour;
            Data_Groups_Friends.BackgroundColor = darkColour;
            Data_Groups_Friends.DefaultCellStyle.BackColor = darkColour;
            Data_Groups_Friends.DefaultCellStyle.SelectionForeColor = lightColour;

            // Search Panel
            Search_Panel.BackColor = lightColour;
            Search_Control_Panel.BackColor = darkColour;

            TextBox_Search.BackColor = darkColour;
            Search_Description.BackColor = darkColour;

            Search_Panel_Button.ForeColor = lightColour;
            Search_Switch_Map_Button.ForeColor = lightColour;
            Search_Event_Update.ForeColor = lightColour;
            Search_Data.BackgroundColor = darkColour;
            Search_Data.DefaultCellStyle.BackColor = darkColour;
            Search_Data.DefaultCellStyle.SelectionForeColor = lightColour;

            // Settings Panel
            Settings_Panel.BackColor = lightColour;
            Settings_Control_Panel.BackColor = darkColour;

            Settings_Light_Button.ForeColor = lightColour;
            Settings_Dark_Button.ForeColor = lightColour;
            Settings_Commit.ForeColor = lightColour;
            Settings_Light_Panel.BackColor = lightColour;
            Settings_Dark_Panel.BackColor = darkColour;

            // Emoji Panel
            Emoji_Panel.BackColor = lightColour;
            Emoji_Control_Panel.BackColor = darkColour;

            Table_Layout_Panel_Emoji.BackColor = darkColour;

            // Notification Panel
            Notification_Panel.BackColor = lightColour;
            Notification_Control_Panel.BackColor = darkColour;

            TableLayoutPanel_Notifications.BackColor = darkColour;
        }

        private void Calendar_Back_Click(object sender, EventArgs e)
        {
            dt = dt.AddMonths(-1);
            SetData(database.GetData(dt), dt);
        }

        private void Calendar_Forward_Click(object sender, EventArgs e)
        {
            dt = dt.AddMonths(1);
            SetData(database.GetData(dt), dt);
        }

        private void PictureBox_MouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.FromArgb(84, 84, 84);
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
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
            database = new Database();

            Login_Panel.Visible = false;
            Signup_Panel.Visible = true;

            PictureBox_Signup_Back.Visible = true;
        }

        private void Login()
        {
            database = new Database(Textbox_Username.Text, Textbox_Password.Text);

            if (database.GetUser() != null)
            {
                PictureBox_Username_Cross.Visible = false;
                PictureBox_Password_Cross.Visible = false;
                
                DateTime tempDt = DateTime.Now;
                dt = new DateTime(tempDt.Year, tempDt.Month, 1);

                if (populate)
                {
                    database.Populate(dt); // TODO: REMOVE ME
                }

                Setup_Emojis();

                calendar = tableLayoutPanel;
                header = tableLayoutPanelCalendarHeader;
                Setup_Dashboard_Groups(false);
                SetData(database.GetData(dt), dt);

                List<Color> temp = database.Get_User_Colours();
                lightColour = temp[0];
                darkColour = temp[1];

                Set_Colours();

                Dashboard_Panel.Visible = true;
                Login_Panel.Visible = false;
                PictureBox_Logout.Visible = true;
                PictureBox_Settings.Visible = true;
            }

            else
            {
                PictureBox_Username_Cross.Visible = true;
                PictureBox_Password_Cross.Visible = true;
            }
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

        private bool UsePasswordMask = true;

        private void Lock_Click(object sender, EventArgs e)
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

        private void PictureBox_Close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void PictureBox_Minimise_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void PictureBox_Form_MouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = lightColour;
        }

        private void PictureBox_Form_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = darkColour;
        }

        private void Emoji_MouseHover(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.BackColor = lightColour;
        }

        private void Emoji_MouseLeave(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.BackColor = darkColour;
        }

        private void PictureBox_Logout_Click(object sender, EventArgs e)
        {
            if (database != null)
            {
                Textbox_Username.Text = "Username";
                Textbox_Password.Text = "Password";
                database = null;
            }

            visibleGroups = new List<int>();

            ResetForm();

            lightColour = Color.CornflowerBlue;
            darkColour = Color.RoyalBlue;

            Set_Colours();

            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = false;

            PictureBox_Settings.Visible = false;
            PictureBox_Back.Visible = false;
            PictureBox_Logout.Visible = false;

            Login_Panel.Visible = true;
        }

        private void Setup_Emojis()
        {
            TextReader tr = new StreamReader(@"Emojis.txt", Encoding.Unicode, true);
            emojis = new List<Emoji>();
            string line;

            while ((line = tr.ReadLine()) != null)
            {
                string[] split = line.Split(',');
                emojis.Add(new Emoji(split[0], split[1]));
            }

            database.SetEmojis(emojis);

            Table_Layout_Panel_Emoji.Controls.Clear();

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

                Table_Layout_Panel_Emoji.Controls.Add(label);
            }
        }

        private void Setup_Groups()
        {
            List<CalendarGroup> cg = database.GetGroups();

            ComboBox_Group.Items.Clear();

            for (int i = 0; i < cg.Count; i++)
            {
                ComboBox_Group.Items.Add(cg[i].GetName());
            }
        }

        private void Add_Event_Button_Click(object sender, EventArgs e)
        {
            if (!SQLSafe(TextBox_Name_Event.Text) || TextBox_Name_Event.Text.Equals(""))
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

                database.Add_Event(TextBox_Name_Event.Text, TextBox_Description.Text, datetime, TextBox_Location.Text, Event_TextBox_Emoji.Text, group);

                DateTime tempDt = DateTime.Now;
                dt = new DateTime(tempDt.Year, tempDt.Month, 1);

                calendar = tableLayoutPanel;
                header = tableLayoutPanelCalendarHeader;
                SetData(database.GetData(dt), dt);

                Dashboard_Panel.Visible = true;
                Event_Panel.Visible = false;
                PictureBox_Back.Visible = false;
            }
        }

        private bool SQLSafe(string str)
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

        private void DatePicker_DateSelected(object sender, DateRangeEventArgs e)
        {
            DateTime tempDt = e.Start;
            dt = new DateTime(tempDt.Year, tempDt.Month, 1);

            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);
        }

        private void Dashboard_Add_Event_Click(object sender, EventArgs e)
        {
            ResetForm();

            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private void PictureBox_Back_Click(object sender, EventArgs e)
        {
            ResetForm();

            Dashboard_Panel.Visible = true;

            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = false;
            Notification_Panel.Visible = false;
            PictureBox_Back.Visible = false;
            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
        }

        private void ColourPicker_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();
            
            Colour_Panel.BackColor = cd.Color;
        }

        private void Dashboard_Add_Group_Click(object sender, EventArgs e)
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

        private void Add_Group_Button_Click(object sender, EventArgs e)
        {
            if (TextBox_Name_Group.Text.Equals("") || !SQLSafe(TextBox_Name_Group.Text))
            {
                Group_Cross_Name.Visible = true;
            }

            else
            {
                Group_Cross_Name.Visible = false;

                int Group_ID = database.Add_Group(TextBox_Name_Group.Text, Colour_Panel.BackColor.ToArgb());

                foreach (DataGridViewRow row in Data_Groups.Rows)
                {
                    if ((bool)row.Cells[0].Value == true)
                    {
                        int User_ID = (int)row.Cells[1].Value;

                        database.Add_Friend_To_Group(User_ID, Group_ID);
                    }
                }

                ResetForm();
            }
        }

        private void Search_Location_Click(object sender, EventArgs e)
        {
            GMap_Control.SetPositionByKeywords(TextBox_Location_Search.Text);
            PointLatLng latLng = GMap_Control.Position;
            double lat = latLng.Lat;
            double lng = latLng.Lng;

            GMap_Control.Overlays.Clear();

            GMapOverlay markers = new GMapOverlay("markers");
            GMap_Control.Overlays.Add(markers);

            GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
            markers.Markers.Add(marker);

            TextBox_Location.Text = lat + "," + lng;
        }

        private void GMap_Control_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                double lat = GMap_Control.FromLocalToLatLng(e.X, e.Y).Lat;
                double lng = GMap_Control.FromLocalToLatLng(e.X, e.Y).Lng;

                GMap_Control.Overlays.Clear();

                GMapOverlay markers = new GMapOverlay("markers");
                GMap_Control.Overlays.Add(markers);

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                markers.Markers.Add(marker);

                TextBox_Location.Text = lat + "," + lng;
            }
        }

        private bool maptype = false;

        private void Map_Type_Click(object sender, EventArgs e)
        {
            if (maptype)
            {
                GMap_Control.MapProvider = BingMapProvider.Instance;
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

        private void Search_Button_Click(object sender, EventArgs e)
        {
            Search(TextBox_Search.Text);
        }

        private void Search(string text)
        {
            Search_Data.DataSource = database.Get_Search_Results(text);
            
            foreach (DataGridViewRow row in Search_Data.Rows)
            {
                row.Cells[4].Value = database.Emoji((string)row.Cells[4].Value, false);
            }

            Search_Data.Columns[0].Visible = false; // Event ID
            Search_Data.Columns[2].Visible = false; // Event Description
            Search_Data.Columns[5].Visible = false; // Group ID
            Search_Data.Columns[7].Visible = false; // Event Location

            Search_Data.Columns[1].HeaderText = "Event Name";
            Search_Data.Columns[3].HeaderText = "Date/Time";
            Search_Data.Columns[4].HeaderText = "Emoji";
            Search_Data.Columns[6].HeaderText = "Group";

            Search_Data.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Search_Data.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Search_Data.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Search_Data_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Search_Data.SelectedRows)
            {
                int Event_ID = (int)row.Cells[0].Value;
                string Event_Name = row.Cells[1].Value.ToString();
                string Event_Description = row.Cells[2].Value.ToString();
                DateTime Event_DateTime = (DateTime)row.Cells[3].Value;
                string Event_Emoji = database.Emoji(row.Cells[4].Value.ToString(), false);
                int Group_ID = (int)row.Cells[5].Value;
                string Group_Name = row.Cells[6].Value.ToString();

                if (!row.Cells[7].Value.ToString().Equals(","))
                {
                    string[] Event_Location = row.Cells[7].Value.ToString().Split(',');

                    double lat = Convert.ToDouble(Event_Location[0]);
                    double lng = Convert.ToDouble(Event_Location[1]);

                    GMap_Control_Search.MapProvider = BingMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control_Search.Position = new PointLatLng(lat, lng);
                    GMap_Control_Search.ShowCenter = false;

                    GMap_Control_Search.Overlays.Clear();

                    GMapOverlay markers = new GMapOverlay("markers");
                    GMap_Control_Search.Overlays.Add(markers);

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                    markers.Markers.Add(marker);
                }

                Search_Description.Text = Event_Description;
            }
        }

        private bool maptype_search = false;

        private void Search_Switch_Map_Button_Click(object sender, EventArgs e)
        {
            if (maptype_search)
            {
                GMap_Control_Search.MapProvider = BingMapProvider.Instance;
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

        private void Dashboard_Search_Button_Click(object sender, EventArgs e)
        {
            ResetForm();

            Search(Dashboard_Search.Text);

            Dashboard_Panel.Visible = false;
            Search_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private bool findLocation = true;

        private void ResetForm()
        {
            double latitude = 0;
            double longitude = 0;

            if (findLocation)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
                    GeoCoordinate coord = watcher.Position.Location;

                    while (coord.IsUnknown && sw.ElapsedMilliseconds < 3000)
                    {
                        watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
                        coord = watcher.Position.Location;

                        if (coord.IsUnknown != true)
                        {
                            latitude = coord.Latitude;
                            longitude = coord.Longitude;
                        }
                    }

                    sw.Stop();

                    if (latitude == 0 && longitude == 0)
                    {
                        findLocation = false;
                    }
                }

                catch (COMException)
                {
                    findLocation = false;
                }
            }

            GMap_Control.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control.Position = new PointLatLng(latitude, longitude);
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

            GMap_Control_Search.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control_Search.Position = new PointLatLng(latitude, longitude);
            GMap_Control_Search.ShowCenter = false;
            maptype_search = false;
            GMap_Control_Search.Overlays.Clear();

            if (database != null)
            {
                Setup_Emojis();
                Setup_Groups();
                Setup_Friends();
                Setup_Dashboard_Groups(true);
            }
            
            ComboBox_Group.Text = "Select Group";

            TextBox_Search.Text = "Enter Search Criteria";
            Search_Description.Text = "Event Description";

            Colour_Panel.BackColor = Color.Black;
            TextBox_Name_Group.Text = "Enter Group Name";
            TextBox_Group_Update.Text = "Group Name";
            Colour_Panel_Update.BackColor = Color.Black;

            Dashboard_Search.Text = "Enter Search Criteria";

            Add_Friend_Button.Text = "Select User To Add As Friend";
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
        }

        // Calendar Class

        private TableLayoutPanel calendar;
        private TableLayoutPanel header;
        private List<List<CalendarEvent>> data;
        private DateTime startDate;

        public void Render()
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

        private void RenderEmptyCells(TableLayoutPanel t, int x)
        {
            for (int i = 0; i < x; i++)
            {
                t.Controls.Add(new Label() { Text = "" }, i, 1);
                t.Controls.Add(new Label() { Text = "" }, i, 2);
            }
        }

        public CalendarEvent GetEvent(DateTime d, string n)
        {
            for (int i = 0; i < data[d.Day - 1].Count; i++)
            {
                if (data[d.Day - 1][i].GetName().Equals(n))
                {
                    //Console.WriteLine("Success: " + data[d.Day - 1][i].GetDateTime());

                    return data[d.Day - 1][i];
                }
            }

            //Console.WriteLine("Failed");

            return null;
        }

        public void SetData(List<List<CalendarEvent>> d, DateTime dt)
        {
            data = d;
            startDate = dt;

            Render();
        }

        public List<List<CalendarEvent>> GetData()
        {
            for (int i = 0; i < 4; i++)
            {
                data.RemoveAt(data.Count - 1);
            }

            //Console.WriteLine(data);

            return data;
        }

        public void PanelClickEvent(object s, EventArgs e)
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

                            Search(selected_dt.ToString("dd/MM/yyyy"));
                            TextBox_Search.Text = selected_dt.ToString("dd/MM/yyyy");

                            Search_Panel.Visible = true;
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

        private void Update_Event_Button_Click(object sender, EventArgs e)
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

            database.Update_Event(Convert.ToInt32(TextBox_Event_ID.Text), TextBox_Name_Event.Text, TextBox_Description.Text, datetime, TextBox_Location.Text, Event_TextBox_Emoji.Text, group);
            
            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);

            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
            PictureBox_Back.Visible = false;
            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
        }

        private void Remove_Event_Button_Click(object sender, EventArgs e)
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

        private void Search_Event_Update_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Search_Data.SelectedRows)
            {
                int Event_ID = (int)row.Cells[0].Value;
                string Event_Name = row.Cells[1].Value.ToString();
                string Event_Description = row.Cells[2].Value.ToString();
                DateTime Event_DateTime = (DateTime)row.Cells[3].Value;
                string Event_Emoji = row.Cells[4].Value.ToString();
                int Group_ID = (int)row.Cells[5].Value;
                string Group_Name = row.Cells[6].Value.ToString();

                if (!row.Cells[7].Value.ToString().Equals(","))
                {
                    string[] Event_Location = row.Cells[7].Value.ToString().Split(',');

                    double lat = Convert.ToDouble(Event_Location[0]);
                    double lng = Convert.ToDouble(Event_Location[1]);

                    GMap_Control.MapProvider = BingMapProvider.Instance;
                    GMaps.Instance.Mode = AccessMode.ServerOnly;
                    GMap_Control.Position = new PointLatLng(lat, lng);
                    GMap_Control.ShowCenter = false;

                    GMap_Control.Overlays.Clear();

                    GMapOverlay markers = new GMapOverlay("markers");
                    GMap_Control.Overlays.Add(markers);

                    GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                    markers.Markers.Add(marker);

                    TextBox_Location.Text = lat + "," + lng;
                }

                TextBox_Event_ID.Text = Convert.ToString(Event_ID);
                TextBox_Name_Event.Text = Event_Name;
                TextBox_Description.Text = Event_Description;
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

        private void Search_Username_Button_Click(object sender, EventArgs e)
        {
            Search_Friends.DataSource = database.Get_Friend_Results(TextBox_Search_Username.Text);

            Search_Friends.Columns[0].Visible = false; // User ID

            Search_Friends.Columns[1].HeaderText = "User Name";

            Search_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Search_Friends_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Search_Friends.SelectedRows)
            {
                int User_ID = (int)row.Cells[0].Value;
                string User_Name = row.Cells[1].Value.ToString();

                TextBox_Friends_Nickname.Text = User_Name;
                Add_Friend_Button.Text = "Add " + User_Name + " As A Friend!";
            }
        }

        private void Add_Friend_Button_Click(object sender, EventArgs e)
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
        }

        private void TextBox_Friends_Nickname_TextChanged(object sender, EventArgs e)
        {
            if (Search_Friends.DataSource != null)
            {
                Add_Friend_Button.Text = "Add " + TextBox_Friends_Nickname.Text + "!";
            }
        }

        private void Setup_Friends()
        {
            List<Friend> f = database.GetFriends();

            Data_Groups.Rows.Clear();

            for (int i = 0; i < f.Count; i++)
            {
                Data_Groups.Rows.Add(false, f[i].GetID(), f[i].GetUserName(), f[i].GetNickName());
            }
        }

        private void Setup_Dashboard_Groups(bool render)
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

        private void Group_Data_Click(object sender, EventArgs e)
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

        private void Data_Groups_Click(object sender, EventArgs e)
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

        private void Find_Friends_Button_Click(object sender, EventArgs e)
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

        private void PictureBox_Drag_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void Settings_Light_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Settings_Light_Panel.BackColor = cd.Color;
        }

        private void Settings_Dark_Button_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Settings_Dark_Panel.BackColor = cd.Color;
        }

        private void Settings_Commit_Click(object sender, EventArgs e)
        {
            lightColour = Settings_Light_Panel.BackColor;
            darkColour = Settings_Dark_Panel.BackColor;

            Set_Colours();

            database.Update_User_Colours(lightColour, darkColour);
        }

        private void PictureBox_Settings_Click(object sender, EventArgs e)
        {
            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;
            Settings_Panel.Visible = true;

            PictureBox_Back.Visible = true;

            Update_Event_Button.Visible = false;
            Remove_Event_Button.Visible = false;
        }

        private bool goodUsername = false;
        private bool goodEmail = false;
        private bool goodPassword = false;
        private bool goodPasswordRetype = false;

        private void Signup_TextBox_Username_TextChanged(object sender, EventArgs e)
        {
            if (!database.Username_Lookup(Signup_TextBox_Username.Text) && Signup_TextBox_Username.Text.Length > 0 && SQLSafe(Signup_TextBox_Username.Text))
            {
                goodUsername = true;
                Signup_Tick_Username.Visible = true;
                Signup_Cross_Username.Visible = false;
            }

            else
            {
                goodUsername = false;
                Signup_Tick_Username.Visible = false;
                Signup_Cross_Username.Visible = true;
            }
        }

        private void Signup_TextBox_Email_TextChanged(object sender, EventArgs e)
        {
            if (!database.Email_Lookup(Signup_TextBox_Email.Text) && Signup_TextBox_Email.Text.Contains("@") && Signup_TextBox_Email.Text.Contains("."))
            {
                goodEmail = true;
                Signup_Tick_Email.Visible = true;
                Signup_Cross_Email.Visible = false;
            }

            else
            {
                goodEmail = false;
                Signup_Tick_Email.Visible = false;
                Signup_Cross_Email.Visible = true;
            }
        }

        private void Signup_TextBox_Password_TextChanged(object sender, EventArgs e)
        {
            if (Signup_TextBox_Password.Text.Length > 6)
            {
                goodPassword = true;
                Signup_Tick_Password.Visible = true;
                Signup_Cross_Password.Visible = false;
            }

            else
            {
                goodPassword = false;
                Signup_Tick_Password.Visible = false;
                Signup_Cross_Password.Visible = true;
            }
        }

        private void Signup_TextBox_Password_Retype_TextChanged(object sender, EventArgs e)
        {
            if (Signup_TextBox_Password.Text.Equals(Signup_TextBox_Password_Retype.Text) && Signup_TextBox_Password_Retype.Text.Length > 6)
            {
                goodPasswordRetype = true;
                Signup_Tick_Password_Retype.Visible = true;
                Signup_Cross_Password_Retype.Visible = false;
            }

            else
            {
                goodPasswordRetype = false;
                Signup_Tick_Password_Retype.Visible = false;
                Signup_Cross_Password_Retype.Visible = true;
            }
        }

        private void Signup_Panel_Button_Click(object sender, EventArgs e)
        {
            if (goodEmail && goodUsername && goodPassword && goodPasswordRetype)
            {
                database.Add_User(Signup_TextBox_Username.Text, Signup_TextBox_Password.Text, Signup_TextBox_Email.Text, -10185235, -12490271);
                
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

        private void PictureBox_Signup_Back_Click(object sender, EventArgs e)
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

        private void Event_Button_Emoji_Click(object sender, EventArgs e)
        {
            Emoji_Panel.Visible = true;
            Event_Panel.Visible = false;
        }

        private void Dashboard_Panel_VisibleChanged(object sender, EventArgs e)
        {
            if (Dashboard_Panel.Visible)
            {
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
        }

        private void PictureBox_Notification_Click(object sender, EventArgs e)
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

        private void TextBox_Name_Group_TextChanged(object sender, EventArgs e)
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

        private void TextBox_Name_Event_TextChanged(object sender, EventArgs e)
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

        private void TextBox_Description_TextChanged(object sender, EventArgs e)
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

        private void Friends_Control_Panel_VisibleChanged(object sender, EventArgs e)
        {
            Data_Friends.DataSource = database.Get_Friends();

            Data_Friends.Columns[0].Visible = false; // User ID

            Data_Friends.Columns[1].HeaderText = "Friends";

            Data_Friends.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void Notification_Panel_VisibleChanged(object sender, EventArgs e)
        {
            if (Dashboard_Panel.Visible)
            {
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
        }

        private void Remove_Friend_Button_Click(object sender, EventArgs e)
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

        private void Update_Friend_Button_Click(object sender, EventArgs e)
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

        private void Data_Display_Groups_Click(object sender, EventArgs e)
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

        private void Data_Groups_Friends_Click(object sender, EventArgs e)
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

        private void Update_Colour_Group_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            Colour_Panel_Update.BackColor = cd.Color;
        }

        private void Remove_Group_Name_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Data_Display_Groups.SelectedRows)
            {
                int Group_ID = (int)row.Cells[0].Value;

                database.Remove_Group(Group_ID);
            }
        }

        private void TextBox_Group_Update_TextChanged(object sender, EventArgs e)
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

        private void Update_Group_Click(object sender, EventArgs e)
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

        public void Add_Event(string name, string description, DateTime datetime, string location, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Add_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji, true);
            cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = location;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = user.GetID();

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
        }

        public void Update_Event(int id, string name, string description, DateTime datetime, string location, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Update_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji, true);
            cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = location;
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
                "SELECT Event_ID, Event_Name, Event_Description, Event_DateTime, Event_Emoji, Group_ID, Group_Name, Event_Location FROM [Final_Year_Project].[dbo].[Event_Table] " +
                "FULL JOIN Group_Table On Group_Table.Group_ID = Event_Table.Event_Group " +
                "WHERE (Event_Owner = " + user.GetID() + " OR Group_Owner = " + user.GetID() + ") " +
                "AND Event_ID IS NOT NULL " +
                "AND (Event_Name Like '" + text + "' " +
                "OR Event_Description Like '" + text + "' " +
                "OR CONVERT(VARCHAR(10), Event_DateTime, 103) like '" + text + "' " +
                "OR Event_Emoji Like '" + text + "' " +
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

        public void Populate(DateTime dt)
        {
            int daycount = 1;
            dt = new DateTime(dt.Year, dt.Month, daycount);

            ClearDB();

            for (int i = 0; i < 10; i++)
            {
                PopulateDB("Football", "Footy with the lads", dt, "Football", "51.7644403180351,0.23895263671875", 2, 1);
                PopulateDB("Shopping", "Christmas shopping", dt, "Football", "51.7848338937353,0.3790283203125", 1, 1);
                PopulateDB("Work", "Lab Write Up", dt, "Football", "51.907001886741,0.3570556640625", 1, 1);
                
                daycount++;
                dt = new DateTime(dt.Year, dt.Month, daycount);
                
                PopulateDB("Work", "Office", dt, "Football", "51.7848338937353,0.3790283203125", 1, 1);

                daycount++;
                dt = new DateTime(dt.Year, dt.Month, daycount);

                PopulateDB("Birthday", "John's House", dt, "Football", "51.7644403180351,0.23895263671875", 3, 1);
                PopulateDB("Dinner", "Emily's", dt, "Football", "51.907001886741,0.3570556640625", 3, 1);

                daycount++;
                dt = new DateTime(dt.Year, dt.Month, daycount);
            }

            dt = new DateTime(dt.Year, 11, 15);
            PopulateDB("Test", "Testing", dt, "Test", "51.7848338937353,0.3790283203125", 1, 1);
        }

        private void PopulateDB(string name, string description, DateTime datetime, string emoji, string location, int group, int owner)
        {
            SqlCommand cmd = new SqlCommand("Add_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = emoji;
            cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = location;
            cmd.Parameters.Add("@Group", SqlDbType.Int).Value = group;
            cmd.Parameters.Add("@Owner", SqlDbType.Int).Value = owner;

            //Console.WriteLine(name + " " + description + " " + datetime + " " + emoji + " " + location + " " + emoji + " " + group + " " + owner);

            connection.Open();
            cmd.ExecuteNonQuery();
            connection.Close();
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
                tempList.Add(new CalendarEvent((int)rdr[0], (string)rdr[1], (string)rdr[2], (DateTime)rdr[3], (string)rdr[5], Emoji((string)rdr[4], false), new CalendarGroup((int)rdr[8], (string)rdr[9], Color.FromArgb((int)rdr[11]))));
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
                        cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = data[i][j].GetLocation();
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
        private int id;
        private string name;
        private string description;
        private DateTime dateTime;
        private string location;
        private string emoji;
        private CalendarGroup group;

        public CalendarEvent(int i, string n, string d, DateTime dt, string l, string e, CalendarGroup g)
        {
            id = i;
            name = n;
            description = d;
            dateTime = dt;
            location = l;
            emoji = e;
            group = g;
        }

        public int GetID()
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

        public string GetLocation()
        {
            return location;
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
