using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Final_Year_Project
{
    public partial class Form1 : Form
    {
        private Calendar calendar;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DateTime dt = new DateTime(2018, 12, 1);
            Database database = new Database("", "");
            calendar = new Calendar(tableLayoutPanel, tableLayoutPanelCalendarHeader);
            calendar.SetData(database.GetData(dt), dt);

            //calendar.AddEvent(new CalendarEvent("Work Due", DateTime.Now, "Computer"));
            //calendar.RemoveEvent(calendar.GetEvent(DateTime.Now.AddDays(-2), "Football"));
        }
    }

    public class Database
    {
        private readonly string connection;
        private readonly string password;

        public Database(string c, string p)
        {
            connection = c;
            password = p;
        }

        public List<List<CalendarEvent>> GetData(DateTime dt)
        {
            // ============================= FAKE DATASET =============================

            List<List<CalendarEvent>> data = new List<List<CalendarEvent>>();
            List<CalendarEvent> tempList = new List<CalendarEvent>();

            int daycount = 1;
            dt = new DateTime(dt.Year, dt.Month, daycount);

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    tempList.Add(new CalendarEvent("Football", dt, "Footy Pitch", new CalendarGroup("Sport", Color.OrangeRed)));
                    tempList.Add(new CalendarEvent("Shopping", dt, "Tesco", new CalendarGroup("Shopping", Color.YellowGreen)));
                    tempList.Add(new CalendarEvent("Lab Write Up", dt, "Home", new CalendarGroup("Work", Color.LightPink)));
                    //tempList.Add(null);

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(dt.Year, dt.Month, daycount);

                    tempList.Add(new CalendarEvent("Birthday", dt, "John's House", new CalendarGroup("Family", Color.Green)));
                    tempList.Add(new CalendarEvent("Dinner", dt, "Emily's", new CalendarGroup("Family", Color.Aqua)));
                    //tempList.Add(null);

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(dt.Year, dt.Month, daycount);

                    tempList.Add(new CalendarEvent("Work", dt, "Office", new CalendarGroup("Work", Color.LightPink)));
                    //tempList.Add(null);

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(dt.Year, dt.Month, daycount);
                }

                tempList.Add(new CalendarEvent("Work", dt, "Office", new CalendarGroup("Work", Color.LightPink)));
                //tempList.Add(null);
            }

            catch(Exception e)
            {

            }

            data.Add(tempList);

            // ============================= FAKE DATASET =============================

            return data;
        }
    }

    public class CalendarGroup
    {
        string name;
        Color color;

        public CalendarGroup(string n, Color c)
        {
            name = n;
            color = c;
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
        string name;
        DateTime dateTime;
        string location;
        CalendarGroup group;

        public CalendarEvent(string n, DateTime dt, string l, CalendarGroup g)
        {
            name = n;
            dateTime = dt;
            location = l;
            group = g;

            //Console.WriteLine(n + ", " + dt + ", " + l);
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
    }

    public class Calendar
    {
        TableLayoutPanel calendar;
        TableLayoutPanel header;
        List<List<CalendarEvent>> data;
        DateTime startDate;

        public Calendar(TableLayoutPanel t, TableLayoutPanel h)
        {
            calendar = t;
            header = h;
        }

        public void Render()
        {
            string[] day_names = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st"};
            
            int day_count = 0;
            int data_count = 0;

            bool switch_colour = true;

            calendar.Visible = false;
            calendar.Controls.Clear();

            TableLayoutPanel y = new TableLayoutPanel();
            y.Dock = DockStyle.Fill;
            y.Margin = new Padding(0, 0, 0, 0);
            y.BackColor = Color.FromArgb(40, 40, 40);
            y.Controls.Add(new Label() { Text = startDate.ToString("MMMM") + " " + startDate.Year, Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Anchor = AnchorStyles.None });
            header.Controls.Add(y);

            for (int i = 0; i < 7; i++)
            {
                TableLayoutPanel p = new TableLayoutPanel();
                p.Dock = DockStyle.Fill;
                p.Margin = new Padding(0, 0, 0, 0);
                p.BackColor = Color.FromArgb(40, 40, 40);
                p.Controls.Add(new Label() { Text = day_names[i], ForeColor = Color.White, Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold), AutoSize = true, Anchor = AnchorStyles.None });
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
                                    calendar.Controls.Add(new Label() { Text = ""});
                                }

                                else
                                {
                                    calendar.Controls.Add(new Label() { Text = days[day_count], Font = new Font("Microsoft Sans Serif", 8), ForeColor = Color.White, BackColor = Color.DimGray, Anchor = AnchorStyles.Left, Margin = new Padding(0, 0, 0, 0) });
                                }
                            }

                            catch (Exception ex)
                            {

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
                                    TableLayoutPanel p = new TableLayoutPanel();
                                    p.ColumnCount = 1;
                                    p.RowCount = data[data_count].Count;
                                    p.Dock = DockStyle.Fill;
                                    p.Margin = new Padding(0, 0, 0, 0);

                                    p.Click += (s, e) =>
                                    {
                                        PanelClickEvent(s, e);
                                    };

                                    for (int h = 0; h < data[data_count].Count; h++)
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

                                        ToolTip t = new ToolTip();
                                        t.IsBalloon = true;
                                        t.SetToolTip(l, data[data_count][h].GetLocation());

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

                                catch (NullReferenceException e)
                                {
                                    TableLayoutPanel p = new TableLayoutPanel();
                                    p.Dock = DockStyle.Fill;
                                    p.Margin = new Padding(0, 0, 0, 0);

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

                                catch (Exception e)
                                {

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
            for (int i = 0; i < data[(d.Day - 1)].Count; i++)
            {
                if (data[(d.Day - 1)][i].GetName().Equals(n))
                {
                    Console.WriteLine("Success: " + data[(d.Day - 1)][i].GetDateTime());

                    return data[(d.Day - 1)][i];
                }
            }

            Console.WriteLine("Failed");

            return null;
        }

        public void AddEvent(CalendarEvent e)
        {
            data[(e.GetDateTime().Day - 1)].Add(e);

            Render();
        }

        public void RemoveEvent(CalendarEvent e)
        {
            data[(e.GetDateTime().Day - 1)].Remove(e);

            Render();
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
                data.RemoveAt((data.Count - 1));
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
                            Console.WriteLine("Empty Field");
                        }

                        else
                        {
                            Console.WriteLine("Date: " + parent.GetControlFromPosition(g, datePoint).Text);

                            string[] numbers = Regex.Split(parent.GetControlFromPosition(g, datePoint).Text, @"\D+");
                            int index = int.Parse(numbers[0]);
                            Render();
                            DateTime dt = new DateTime(2018, 1, index);
                            RemoveEvent(GetEvent(dt, "Football"));
                        }
                    }
                }
            }

            if (!found)
            {
                Console.WriteLine("Nothing found");
            }
        }
    }
}
