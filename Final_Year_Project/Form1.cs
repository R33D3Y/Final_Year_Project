﻿using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Device.Location;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Final_Year_Project
{
    public partial class Form1 : Form
    {
        private Database database;
        private DateTime dt;
        private readonly bool populate = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
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
            SignUp();
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

                calendar = tableLayoutPanel;
                header = tableLayoutPanelCalendarHeader;
                SetData(database.GetData(dt), dt);

                ResetForm();

                Dashboard_Panel.Visible = true;
                Login_Panel.Visible = false;
                PictureBox_Logout.Visible = true;
            }

            else
            {
                PictureBox_Username_Cross.Visible = true;
                PictureBox_Password_Cross.Visible = true;
            }
        }

        private void SignUp()
        {

        }

        private void Login_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                Login();
            }
        }

        private void Login_MouseHover(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            l.BackColor = Color.Silver;
        }

        private void Login_MouseLeave(object sender, EventArgs e)
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
            if (database != null)
            {
                database.SetData(GetData());
            }

            Application.Exit();
        }

        private void PictureBox_Minimise_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void PictureBox_Form_MouseHover(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.CornflowerBlue;
        }

        private void PictureBox_Form_MouseLeave(object sender, EventArgs e)
        {
            PictureBox pb = (PictureBox)sender;
            pb.BackColor = Color.RoyalBlue;
        }

        private void PictureBox_Logout_Click(object sender, EventArgs e)
        {
            ResetForm();

            if (database != null)
            {
                database.SetData(GetData());
                Textbox_Username.Text = "Username";
                Textbox_Password.Text = "Password";
                database = null;
            }

            Dashboard_Panel.Visible = false;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;

            Login_Panel.Visible = true;
        }

        private void Setup_Emojis()
        {
            ComboBox_Emoji.Items.Clear();

            ComboBox_Emoji.Items.Add("⚽️");
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

            database.Add_Event(TextBox_Name_Event.Text, TextBox_Description.Text, datetime, TextBox_Location.Text, ComboBox_Emoji.Text, group);

            DateTime tempDt = DateTime.Now;
            dt = new DateTime(tempDt.Year, tempDt.Month, 1);

            calendar = tableLayoutPanel;
            header = tableLayoutPanelCalendarHeader;
            SetData(database.GetData(dt), dt);

            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
            PictureBox_Back.Visible = false;
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

        private void Setup_Location()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            GeoCoordinate coord = watcher.Position.Location;

            double latitude = 0;
            double longitude = 0;

            while (coord.IsUnknown)
            {
                watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
                coord = watcher.Position.Location;

                if (coord.IsUnknown != true)
                {
                    latitude = coord.Latitude;
                    longitude = coord.Longitude;
                }
            }

            GMap_Control.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control.Position = new PointLatLng(latitude, longitude);
            GMap_Control.ShowCenter = false;
        }

        private void PictureBox_Back_Click(object sender, EventArgs e)
        {
            Dashboard_Search.Text = "Enter Search Criteria";

            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;
            Friends_Panel.Visible = false;

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

            Dashboard_Panel.Visible = false;
            Group_Panel.Visible = true;
            PictureBox_Back.Visible = true;
        }

        private void Add_Group_Button_Click(object sender, EventArgs e)
        {
            int Group_ID = database.Add_Group(TextBox_Name_Group.Text, Colour_Panel.BackColor.ToArgb());
            
            foreach (DataGridViewRow row in Data_Groups.Rows)
            {
                if ((bool)row.Cells[0].Value == true)
                {
                    int User_ID = (int)row.Cells[1].Value;

                    database.Add_Friend_To_Group(User_ID, Group_ID);
                }
            }

            Dashboard_Panel.Visible = true;
            Event_Panel.Visible = false;
            Group_Panel.Visible = false;
            Search_Panel.Visible = false;

            PictureBox_Back.Visible = false;
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
                row.Cells[4].Value = database.Emoji((string)row.Cells[4].Value);
            }

            Search_Data.Columns[0].Visible = false; // Event ID
            Search_Data.Columns[2].Visible = false; // Event Description
            Search_Data.Columns[5].Visible = false; // Group ID
            Search_Data.Columns[7].Visible = false; // Event Location

            Search_Data.Columns[1].HeaderText = "Event Name";
            Search_Data.Columns[3].HeaderText = "Date/Time";
            Search_Data.Columns[4].HeaderText = "Emoji";
            Search_Data.Columns[6].HeaderText = "Group";
        }

        private void Search_Data_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in Search_Data.SelectedRows)
            {
                int Event_ID = (int)row.Cells[0].Value;
                string Event_Name = row.Cells[1].Value.ToString();
                string Event_Description = row.Cells[2].Value.ToString();
                DateTime Event_DateTime = (DateTime)row.Cells[3].Value;
                string Event_Emoji = database.Emoji(row.Cells[4].Value.ToString());
                int Group_ID = (int)row.Cells[5].Value;
                string Group_Name = row.Cells[6].Value.ToString();
                string [] Event_Location = row.Cells[7].Value.ToString().Split(',');

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

        private void ResetForm()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            GeoCoordinate coord = watcher.Position.Location;

            double latitude = 0;
            double longitude = 0;

            while (coord.IsUnknown)
            {
                watcher.TryStart(false, TimeSpan.FromMilliseconds(2000));
                coord = watcher.Position.Location;

                if (coord.IsUnknown != true)
                {
                    latitude = coord.Latitude;
                    longitude = coord.Longitude;
                }
            }

            GMap_Control.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control.Position = new PointLatLng(latitude, longitude);
            GMap_Control.ShowCenter = false;
            maptype = false;

            TextBox_Event_ID.Text = "";
            TextBox_Name_Event.Text = "Enter Event Name";
            TextBox_Description.Text = "Enter Description";
            ComboBox_Emoji.Items.Clear();
            ComboBox_Group.Items.Clear();
            TextBox_Location.Text = ",";
            TextBox_Location_Search.Text = "Enter Address or Place";
            DateTimePicker_Date.Text = DateTime.Now.ToLongDateString();
            DateTimePicker_Time.Text = DateTime.Now.ToLongTimeString();

            GMap_Control_Search.MapProvider = BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerOnly;
            GMap_Control_Search.Position = new PointLatLng(latitude, longitude);
            GMap_Control_Search.ShowCenter = false;
            maptype_search = false;

            if (database != null)
            {
                Setup_Emojis();
                Setup_Groups();
                Setup_Friends();
            }

            ComboBox_Emoji.Text = "Select Emoji";
            ComboBox_Group.Text = "Select Group";

            TextBox_Search.Text = "Enter Search Criteria";
            Search_Description.Text = "Event Description";

            Colour_Panel.BackColor = Color.Black;
            TextBox_Name_Group.Text = "Enter Group Name";
        }

        private TableLayoutPanel calendar;
        private TableLayoutPanel header;
        private List<List<CalendarEvent>> data;
        private DateTime startDate;

        // Calendar Class

        public void Render()
        {
            string[] day_names = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st" };

            int day_count = 0;
            int data_count = 0;

            bool switch_colour = true;

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

                                    for (int h = 0; (h < data[data_count].Count && h < 5); h++)
                                    {
                                        Color txt = Color.Black;

                                        if (data[data_count][h].GetCalendarGroup().GetColor().GetBrightness() < 0.3)
                                        {
                                            txt = Color.White;
                                        }

                                        Label l = new Label() { Text = data[data_count][h].GetName(), ForeColor = txt, BackColor = data[data_count][h].GetCalendarGroup().GetColor(), Height = 15, Margin = new Padding(0, 0, 0, 0) };
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

                                        p.Controls.Add(l);
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
                                    //Console.WriteLine(ex_var.Message);

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

            database.Update_Event(Convert.ToInt32(TextBox_Event_ID.Text), TextBox_Name_Event.Text, TextBox_Description.Text, datetime, TextBox_Location.Text, ComboBox_Emoji.Text, group);
            
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
                string[] Event_Location = row.Cells[7].Value.ToString().Split(',');

                double lat = Convert.ToDouble(Event_Location[0]);
                double lng = Convert.ToDouble(Event_Location[1]);

                ResetForm();

                GMap_Control.MapProvider = BingMapProvider.Instance;
                GMaps.Instance.Mode = AccessMode.ServerOnly;
                GMap_Control.Position = new PointLatLng(lat, lng);
                GMap_Control.ShowCenter = false;

                GMap_Control.Overlays.Clear();

                GMapOverlay markers = new GMapOverlay("markers");
                GMap_Control.Overlays.Add(markers);

                GMapMarker marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_dot);
                markers.Markers.Add(marker);

                TextBox_Event_ID.Text = Convert.ToString(Event_ID);
                TextBox_Name_Event.Text = Event_Name;
                TextBox_Description.Text = Event_Description;
                ComboBox_Group.Text = Group_Name;
                ComboBox_Emoji.Text = Event_Emoji;
                DateTimePicker_Date.Text = Event_DateTime.ToLongDateString();
                DateTimePicker_Time.Text = Event_DateTime.ToLongTimeString();
                TextBox_Location.Text = lat + "," + lng;

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

                Search_Friends.DataSource = database.Get_Friend_Results(TextBox_Search_Username.Text);

                Search_Friends.Columns[0].Visible = false; // User ID

                Search_Friends.Columns[1].HeaderText = "User Name";
            }
        }

        private void TextBox_Friends_Nickname_TextChanged(object sender, EventArgs e)
        {
            Add_Friend_Button.Text = "Add " + TextBox_Friends_Nickname.Text + " As A Friend!";
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

        public void Add_Event(string name, string description, DateTime datetime, string location, string emoji, int group)
        {
            SqlCommand cmd = new SqlCommand("Add_Event", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
            cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = description;
            cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = datetime;
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji);
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
            cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(emoji);
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
                if (tempList.Count > 0 && tempList[0].GetDateTime().Date != ((DateTime)rdr[3]).Date)
                {
                    data.Add(tempList);
                    tempList = new List<CalendarEvent>();
                    day++;
                }

                else
                {
                    while (day != ((DateTime)rdr[3]).Day)
                    {
                        tempList = new List<CalendarEvent>();
                        tempList.Add(null);
                        data.Add(tempList);
                        tempList = new List<CalendarEvent>();
                        day++;
                    }
                }
                
                tempList.Add(new CalendarEvent((int)rdr[0], (string)rdr[1], (string)rdr[2], (DateTime)rdr[3], (string)rdr[5], Emoji((string)rdr[4]), new CalendarGroup((int)rdr[6], (string)rdr[9], Color.FromArgb((int)rdr[11]))));
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
                    try
                    {
                        SqlCommand cmd = new SqlCommand("Update_Event", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = data[i][j].GetID();
                        cmd.Parameters.Add("@Name", SqlDbType.VarChar).Value = data[i][j].GetName();
                        cmd.Parameters.Add("@Description", SqlDbType.VarChar).Value = data[i][j].GetDescription();
                        cmd.Parameters.Add("@DateTime", SqlDbType.DateTime).Value = data[i][j].GetDateTime();
                        cmd.Parameters.Add("@Emoji", SqlDbType.VarChar).Value = Emoji(data[i][j].GetEmoji());
                        cmd.Parameters.Add("@Location", SqlDbType.VarChar).Value = data[i][j].GetLocation();

                        cmd.ExecuteNonQuery();
                    }

                    catch (Exception)
                    {

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

        public string Emoji(string e)
        {
            if (e == "Football")
            {
                return "⚽️";
            }

            else if (e == "⚽️")
            {
                return "Football";
            }

            else
            {
                return "Emoji Not Found: " + e;
            }
        }
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
 * References -
     * Logo: https://www.logolynx.com/topic/calendar
     * Icons: https://icons8.com/
     * Arrow Images: https://emojipedia.org/softbank/
     * MS MySQL: https://www.microsoft.com/en-us/download/details.aspx?id=54257
     * Map:
         * Bing - https://www.bing.com/maps
         * NuGet Package - https://archive.codeplex.com/?p=greatmaps
         * Tutorial - http://www.independent-software.com/gmap-net-beginners-tutorial-maps-markers-polygons-routes-updated-for-vs2015-and-gmap1-7.html
*/
