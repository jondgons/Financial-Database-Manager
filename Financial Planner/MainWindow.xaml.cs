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

        // allows foreach() across a range of DateTimes
        public IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        // updates DataDisplay with range of information from StartDate to EndDate
        private void DisplayDateRange_Click(object sender, RoutedEventArgs e)
        {
            DateTime StartDate = new DateTime();
            DateTime EndDate = new DateTime();

            // wipes DataDisplay
            DataDisplay.Text = "";

            // creates a DateTime from StartDatePicker & checks if valid
            if (StartDatePicker.Text != "")
            {
                StartDate = DateTime.ParseExact(dateTextHelper(StartDatePicker.Text), "MMddyyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                MessageBox.Show("Please enter a start date.", "ERROR: Bad Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // creates a DateTime from EndDatePicker & checks if valid
            if (EndDatePicker.Text != "")
            {
                EndDate = DateTime.ParseExact(dateTextHelper(EndDatePicker.Text), "MMddyyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                MessageBox.Show("Please enter an end date.", "ERROR: Bad Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (DateTime day in EachDay(StartDate, EndDate))
            {
                // get data from database and add to DataDisplay
                DataDisplay.Text += day.ToShortDateString() + "\n";
            }
        }

        // converts date format to something readable by DateTime's ParseExact
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

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            // creates DateTime to read correct date from DateEntryPicker
            DateTime EntryDate = new DateTime();
            if (DateEntryPicker.Text != "")
            {
                EntryDate = DateTime.ParseExact(dateTextHelper(DateEntryPicker.Text), "MMddyyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                MessageBox.Show("Please enter an entry date.", "ERROR: Bad Date", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        // TODO fix VerifyAmount
        private bool nonNumberEntered = false;
        private void VerifyAmount_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void VerifyAmount_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (nonNumberEntered)
            {
                e.Handled = true;
            }
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
