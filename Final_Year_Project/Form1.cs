using System;
using System.Collections.Generic;
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

            calendar.AddEvent(new CalendarEvent("Work Due", DateTime.Now.AddDays(-1), "Computer"));
            calendar.RemoveEvent(calendar.GetEvent(DateTime.Now.AddDays(-2), "Dinner"));
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
            List<List<CalendarEvent>> data = new List<List<CalendarEvent>>();
            List<CalendarEvent> tempList = new List<CalendarEvent>();

            for (int i = 0; i < 10; i++)
            {
                tempList.Add(new CalendarEvent("Football", DateTime.Now, "Footy Pitch"));
                tempList.Add(new CalendarEvent("Shopping", DateTime.Now, "Tesco"));
                tempList.Add(new CalendarEvent("Lab Write Up", DateTime.Now, "Home"));

                data.Add(tempList);

                tempList = new List<CalendarEvent>();

                tempList.Add(new CalendarEvent("Birthday", DateTime.Now, "John's House"));
                tempList.Add(new CalendarEvent("Dinner", DateTime.Now, "Home"));

                data.Add(tempList);

                tempList = new List<CalendarEvent>();

                tempList.Add(new CalendarEvent("Date", DateTime.Now, "Nandos"));

                data.Add(tempList);

                tempList = new List<CalendarEvent>();
            }

            tempList.Add(new CalendarEvent("Date", DateTime.Now, "Nandos"));

            data.Add(tempList);

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
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st", "", "", "", "" };
            
            int day_count = 0;
            int data_count = 0;

            calendar.Controls.Clear();

            for (int i = 0; i < 7; i++)
            {
                calendar.Controls.Add(new Label() { Text = day_names[i] });
            }

            for (int i = 1; i < 6; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    try
                    {
                        calendar.Controls.Add(new Label() { Text = days[day_count] });
                    }

                    catch (Exception ex)
                    {

                    }

                    day_count++;
                }

                for (int j = 0; j < 7; j++)
                {
                    if (data[data_count].Count > 1)
                    {
                        TableLayoutPanel p = new TableLayoutPanel();
                        p.ColumnCount = 1;
                        p.RowCount = data[data_count].Count;
                        p.Dock = DockStyle.Fill;

                        for (int h = 0; h < data[data_count].Count; h++)
                        {
                            p.Controls.Add(new Label() { Text = data[data_count][h].getName() });
                        }

                        calendar.Controls.Add(p);
                    }

                    else
                    {
                        calendar.Controls.Add(new Label() { Text = data[data_count][0].getName() });
                    }

                    data_count++;
                }
            }
        }

        public CalendarEvent GetEvent(DateTime d, string n)
        {
            for (int i = 0; i < data[(d.Day - 1)].Count; i++)
            {
                if (data[(d.Day - 1)][i].getName().Equals(n))
                {
                    return data[(d.Day - 1)][i];
                }
            }

            return data[(d.Day - 1)][0];
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
            List<CalendarEvent> tempList = new List<CalendarEvent>();

            for (int i = 0; i < 4; i++)
            {
                tempList.Add(new CalendarEvent("", DateTime.Now, ""));

                d.Add(tempList);

                tempList = new List<CalendarEvent>();
            }

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
    }
}
