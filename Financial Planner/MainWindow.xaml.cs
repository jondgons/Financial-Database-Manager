using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

namespace Financial_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        private void DisplayDateRange_Click(object sender, RoutedEventArgs e)
        {
            // wipes DataDisplay
            DataDisplay.Text = "";

            // creates a DateTime from StartDatePicker
            DateTime StartDate = DateTime.ParseExact(dateTextHelper(StartDatePicker.Text), "MMddyyyy", CultureInfo.InvariantCulture);
            DateTime EndDate = DateTime.ParseExact(dateTextHelper(EndDatePicker.Text), "MMddyyyy", CultureInfo.InvariantCulture);

            foreach (DateTime day in EachDay(StartDate, EndDate))
            {
                // get data from database and add to DataDisplay
                DataDisplay.Text += day.ToString() + "\n";
            }
        }

        private string dateTextHelper(string dateText)
        {
            int slash = 0;
            string MM = "";
            string dd = "";
            string yyyy = "";

            // loop through dateText
            foreach(char c in dateText)
            {
                if (c == '/')
                {
                    slash++;
                }
                else if (slash == 0)
                {
                    MM += c;
                }
                else if (slash == 1)
                {
                    dd += c;
                }
                else if (slash == 2)
                {
                    yyyy += c;
                }
            }

            // check if month and day have less than two digits
            if (MM.Length < 2)
            {
                MM = "0" + MM;
            }
            if (dd.Length < 2)
            {
                dd = "0" + dd;
            }

            return MM + dd + yyyy;
        }

        /*
         *  STUFF THAT NEEDS TO GET DONE:
         *      - form to enter:
         *          - amount (dynamic income or expense)
         *          - description
         *          - date
         *      - display field to show a selected range's information
         *      - tie stuff to database
         *          - querying for info
         *          - storing info
         *          - categorizing info?
         */

        //amount needs to enter NUMBERS ONLY - allow negative & decimals
    }
}
