using System;
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
            calendar = new Calendar(DateTime.Now, tableLayoutPanel);
            calendar.Render();
        }
    }

    public class Calendar
    {
        DateTime start;
        TableLayoutPanel calendar;

        public Calendar(DateTime s, TableLayoutPanel t)
        {
            start = s;
            calendar = t;
        }

        public void Render()
        {
            //calendar.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            //calendar.ColumnCount = 7;
            //calendar.RowCount = 1;

            //calendar.AutoSize = true;

            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            //calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));

            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            //calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            //calendar.Controls.Add(new Label() { Text = "Monday" }, 0, 0);
            //calendar.Controls.Add(new Label() { Text = "Tuesday" }, 1, 0);
            //calendar.Controls.Add(new Label() { Text = "Wednesday" }, 2, 0);
            //calendar.Controls.Add(new Label() { Text = "Thursday" }, 3, 0);
            //calendar.Controls.Add(new Label() { Text = "Friday" }, 4, 0);
            //calendar.Controls.Add(new Label() { Text = "Saturday" }, 5, 0);
            //calendar.Controls.Add(new Label() { Text = "Sunday" }, 6, 0);

            //calendar.Controls.Add(new Label() { Text = "1st" }, 0, 1);
            //calendar.Controls.Add(new Label() { Text = "2nd" }, 1, 1);
            //calendar.Controls.Add(new Label() { Text = "3rd" }, 2, 1);
            //calendar.Controls.Add(new Label() { Text = "4th" }, 3, 1);
            //calendar.Controls.Add(new Label() { Text = "6th" }, 4, 1);
            //calendar.Controls.Add(new Label() { Text = "7th" }, 5, 1);
            //calendar.Controls.Add(new Label() { Text = "8th" }, 6, 1);

            //calendar.Controls.Add(new Label() { Text = "" }, 0, 2);
            //calendar.Controls.Add(new Label() { Text = "Football" }, 1, 2);
            //calendar.Controls.Add(new Label() { Text = "" }, 2, 2);
            //calendar.Controls.Add(new Label() { Text = "Meeting" }, 3, 2);
            //calendar.Controls.Add(new Label() { Text = "" }, 4, 2);
            //calendar.Controls.Add(new Label() { Text = "" }, 5, 2);
            //calendar.Controls.Add(new Label() { Text = "Shopping" }, 6, 2);

            //calendar.Controls.Add(new Label() { Text = "9th" }, 0, 3);
            //calendar.Controls.Add(new Label() { Text = "10th" }, 1, 3);
            //calendar.Controls.Add(new Label() { Text = "11th" }, 2, 3);
            //calendar.Controls.Add(new Label() { Text = "12th" }, 3, 3);
            //calendar.Controls.Add(new Label() { Text = "13th" }, 4, 3);
            //calendar.Controls.Add(new Label() { Text = "14th" }, 5, 3);
            //calendar.Controls.Add(new Label() { Text = "15th" }, 6, 3);

            //calendar.Controls.Add(new Label() { Text = "" }, 0, 4);
            //calendar.Controls.Add(new Label() { Text = "" }, 1, 4);
            //calendar.Controls.Add(new Label() { Text = "Birthday" }, 2, 4);
            //calendar.Controls.Add(new Label() { Text = "Meeting" }, 3, 4);
            //calendar.Controls.Add(new Label() { Text = "" }, 4, 4);
            //calendar.Controls.Add(new Label() { Text = "Laundry" }, 5, 4);
            //calendar.Controls.Add(new Label() { Text = "" }, 6, 4);

            //calendar.Controls.Add(new Label() { Text = "16th" }, 0, 5);
            //calendar.Controls.Add(new Label() { Text = "17th" }, 1, 5);
            //calendar.Controls.Add(new Label() { Text = "18th" }, 2, 5);
            //calendar.Controls.Add(new Label() { Text = "19th" }, 3, 5);
            //calendar.Controls.Add(new Label() { Text = "20th" }, 4, 5);
            //calendar.Controls.Add(new Label() { Text = "21st" }, 5, 5);
            //calendar.Controls.Add(new Label() { Text = "22nd" }, 6, 5);

            //calendar.Controls.Add(new Label() { Text = "Dinner" }, 0, 6);
            //calendar.Controls.Add(new Label() { Text = "" }, 1, 6);
            //calendar.Controls.Add(new Label() { Text = "Football" }, 2, 6);
            //calendar.Controls.Add(new Label() { Text = "" }, 3, 6);
            //calendar.Controls.Add(new Label() { Text = "Match" }, 4, 6);
            //calendar.Controls.Add(new Label() { Text = "Cleaner" }, 5, 6);
            //calendar.Controls.Add(new Label() { Text = "" }, 6, 6);

            string[] day_names = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};
            string[] days = { "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st" };
            int day_count = 0;

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
                    calendar.Controls.Add(new Label() { Text = "New Event" });
                }
            }
        }

        public void AddEvent()
        {

        }

        public void RemoveEvent()
        {

        }

        public void NextWeek()
        {

        }

        public void PreviousWeek()
        {

        }

        public void SetData(string [] [] data)
        {

        }

        public string [] [] GetData()
        {
            return new string[31][];
        }
    }
}
