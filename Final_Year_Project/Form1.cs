using System;
using System.Collections.Generic;
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
            Database database = new Database("", "");
            calendar = new Calendar(tableLayoutPanel);
            calendar.SetData(database.GetData());

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

        public List<List<CalendarEvent>> GetData()
        {
            // ============================= FAKE DATASET =============================

            int month = 8;

            List<List<CalendarEvent>> data = new List<List<CalendarEvent>>();
            List<CalendarEvent> tempList = new List<CalendarEvent>();

            int daycount = 1;
            DateTime dt = new DateTime(2018, month, daycount);

            try
            {
                for (int i = 0; i < 10; i++)
                {
                    tempList.Add(new CalendarEvent("Football", dt, "Footy Pitch"));
                    tempList.Add(new CalendarEvent("Shopping", dt, "Tesco"));
                    tempList.Add(new CalendarEvent("Lab Write Up", dt, "Home"));

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(2018, month, daycount);
                    Console.WriteLine(dt);

                    tempList.Add(new CalendarEvent("Birthday", dt, "John's House"));
                    tempList.Add(new CalendarEvent("Dinner", dt, "Home"));

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(2018, month, daycount);

                    tempList.Add(new CalendarEvent("Date", dt, "Nandos"));

                    data.Add(tempList);

                    tempList = new List<CalendarEvent>();
                    daycount++;
                    dt = new DateTime(2018, month, daycount);
                }

                tempList.Add(new CalendarEvent("Date", dt, "Nandos"));
            }

            catch(Exception e)
            {

            }

            data.Add(tempList);

            // ============================= FAKE DATASET =============================

            return data;
        }
    }

    public class CalendarEvent
    {
        string name;
        DateTime dateTime;
        string location;

        public CalendarEvent(string n, DateTime dt, string l)
        {
            name = n;
            dateTime = dt;
            location = l;

            //Console.WriteLine(n + ", " + dt + ", " + l);
        }

        public string getName()
        {
            return name;
        }

        public DateTime getDateTime()
        {
            return dateTime;
        }

        public string getLocation()
        {
            return location;
        }
    }

    public class Calendar
    {
        TableLayoutPanel calendar;
        List<List<CalendarEvent>> data;

        public Calendar(TableLayoutPanel t)
        {
            calendar = t;
        }

        public void Render()
        {
            string[] day_names = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st"};
            
            int day_count = 0;
            int data_count = 0;
            
            calendar.Visible = false;
            calendar.Controls.Clear();

            for (int i = 0; i < 7; i++)
            {
                calendar.Controls.Add(new Label() { Text = day_names[i] });
            }
            
            switch (data[0][0].getDateTime().DayOfWeek)
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

                        if ((calendar.GetRowHeights()[i] - 1) == 20)
                        {
                            try
                            {
                                DateTime dt = data[0][0].getDateTime();

                                if (days.Length == day_count || day_count >= dt.AddMonths(1).AddDays(-1).Day)
                                {
                                    calendar.Controls.Add(new Label() { Text = "" });
                                }

                                else
                                {
                                    calendar.Controls.Add(new Label() { Text = days[day_count] });
                                }
                            }

                            catch (Exception ex)
                            {

                            }

                            day_count++;
                        }

                        else
                        {
                            try
                            {
                                TableLayoutPanel p = new TableLayoutPanel();
                                p.ColumnCount = 1;
                                p.RowCount = data[data_count].Count;
                                p.Dock = DockStyle.Fill;

                                p.Click += (s, e) =>
                                {
                                    PanelClickEvent(s, e);
                                };

                                for (int h = 0; h < data[data_count].Count; h++)
                                {
                                    Label l = new Label() { Text = data[data_count][h].getName() };
                                    l.Click += (s, e) =>
                                    {
                                        s = l.Parent;
                                        PanelClickEvent(s, e);
                                    };

                                    p.Controls.Add(l);
                                }

                                calendar.Controls.Add(p);
                            }

                            catch(Exception e)
                            {

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
                if (data[(d.Day - 1)][i].getName().Equals(n))
                {
                    Console.WriteLine("Success: " + data[(d.Day - 1)][i].getDateTime());

                    return data[(d.Day - 1)][i];
                }
            }

            Console.WriteLine("Failed");

            return null;
        }

        public void AddEvent(CalendarEvent e)
        {
            data[(e.getDateTime().Day - 1)].Add(e);

            Render();
        }

        public void RemoveEvent(CalendarEvent e)
        {
            data[(e.getDateTime().Day - 1)].Remove(e);

            Render();
        }

        public void SetData(List<List<CalendarEvent>> d)
        {
            data = d;

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
