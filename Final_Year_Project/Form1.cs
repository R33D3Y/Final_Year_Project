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
            calendar = new Calendar(DateTime.Now);
            calendar.Render();
            Controls.Add(calendar.GetCalendar());
        }
    }

    public class Calendar
    {
        DateTime start;
        TableLayoutPanel calendar;

        public Calendar(DateTime s)
        {
            start = s;
            calendar = new TableLayoutPanel();
        }

        public void Render()
        {
            calendar.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            calendar.ColumnCount = 7;
            calendar.RowCount = 1;

            calendar.AutoSize = true;

            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
            calendar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));

            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
            calendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

            calendar.Controls.Add(new Label() { Text = "Monday" }, 0, 0);
            calendar.Controls.Add(new Label() { Text = "Tuesday" }, 1, 0);
            calendar.Controls.Add(new Label() { Text = "Wednesday" }, 2, 0);
            calendar.Controls.Add(new Label() { Text = "Thursday" }, 3, 0);
            calendar.Controls.Add(new Label() { Text = "Friday" }, 4, 0);
            calendar.Controls.Add(new Label() { Text = "Saturday" }, 5, 0);
            calendar.Controls.Add(new Label() { Text = "Sunday" }, 6, 0);

            calendar.Controls.Add(new Label() { Text = "1st" }, 0, 1);
            calendar.Controls.Add(new Label() { Text = "2nd" }, 1, 1);
            calendar.Controls.Add(new Label() { Text = "3rd" }, 2, 1);
            calendar.Controls.Add(new Label() { Text = "4th" }, 3, 1);
            calendar.Controls.Add(new Label() { Text = "6th" }, 4, 1);
            calendar.Controls.Add(new Label() { Text = "7th" }, 5, 1);
            calendar.Controls.Add(new Label() { Text = "8th" }, 6, 1);

            calendar.Controls.Add(new Label() { Text = "" }, 0, 2);
            calendar.Controls.Add(new Label() { Text = "Football" }, 1, 2);
            calendar.Controls.Add(new Label() { Text = "" }, 2, 2);
            calendar.Controls.Add(new Label() { Text = "Meeting" }, 3, 2);
            calendar.Controls.Add(new Label() { Text = "" }, 4, 2);
            calendar.Controls.Add(new Label() { Text = "" }, 5, 2);
            calendar.Controls.Add(new Label() { Text = "Shopping" }, 6, 2);

            calendar.Controls.Add(new Label() { Text = "9th" }, 0, 3);
            calendar.Controls.Add(new Label() { Text = "10th" }, 1, 3);
            calendar.Controls.Add(new Label() { Text = "11th" }, 2, 3);
            calendar.Controls.Add(new Label() { Text = "12th" }, 3, 3);
            calendar.Controls.Add(new Label() { Text = "13th" }, 4, 3);
            calendar.Controls.Add(new Label() { Text = "14th" }, 5, 3);
            calendar.Controls.Add(new Label() { Text = "15th" }, 6, 3);

            calendar.Controls.Add(new Label() { Text = "" }, 0, 4);
            calendar.Controls.Add(new Label() { Text = "" }, 1, 4);
            calendar.Controls.Add(new Label() { Text = "Birthday" }, 2, 4);
            calendar.Controls.Add(new Label() { Text = "Meeting" }, 3, 4);
            calendar.Controls.Add(new Label() { Text = "" }, 4, 4);
            calendar.Controls.Add(new Label() { Text = "Laundry" }, 5, 4);
            calendar.Controls.Add(new Label() { Text = "" }, 6, 4);
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

        public void Sync()
        {

        }

        public TableLayoutPanel GetCalendar()
        {
            return calendar;
        }
    }
}
