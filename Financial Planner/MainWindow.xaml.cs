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
using System.Data.OleDb;
using System.Text.RegularExpressions;

namespace Financial_Planner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OleDbConnection cn;

        public MainWindow()
        {
            InitializeComponent();
            cn = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\Financial_Data.mdb");
            DisplayTable = GenerateTable();
            DisplayFlowDocument.Blocks.Add(DisplayTable);
            CreateAmountNud();
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

            // creating database variables
            string query = "SELECT * FROM Financials WHERE FinDate BETWEEN #" + StartDate + "# AND #" + EndDate +"#";
            OleDbCommand cmd = new OleDbCommand(query, cn);
            cn.Open();
            OleDbDataReader read = cmd.ExecuteReader();
            int currentRowNum = 2;

            // removes old table and makes a new one
            DisplayFlowDocument.Blocks.Remove(DisplayTable);
            DisplayTable = GenerateTable();
            DisplayFlowDocument.Blocks.Add(DisplayTable);

            // fills table with data
            while (read.Read())
            {
                // add new rows
                DisplayTable.RowGroups[0].Rows.Add(new TableRow());
                TableRow currentRow = DisplayTable.RowGroups[0].Rows[currentRowNum];

                // Global formatting for the row.
                currentRow.FontSize = 12;
                currentRow.FontWeight = FontWeights.Normal;

                // fill rows with info
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(read[2].ToString().Remove(read[2].ToString().Length - 12))))); // date
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(read[3].ToString())))); // desc
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run("$" + Convert.ToDouble(read[1].ToString()).ToString("0." + new string('0', 2)))))); // amount

                // Bold the first cell.
                currentRow.Cells[0].FontWeight = FontWeights.Bold;

                // increments currentRowNum
                currentRowNum++;
            }

            cn.Close();
        }

        // adds new entry to database
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

            // creating database variables
            string query = "INSERT INTO Financials (Amount,FinDate,Description) VALUES (@Amount,@FinDate,@Description)";
            OleDbCommand cmd = new OleDbCommand(query, cn);

            // adding to cmd for storing
            cmd.Parameters.Add("@Amount", OleDbType.Double).Value = 12.12; //TODO make this from parsed Amount.Text
            cmd.Parameters.Add("@FinDate", OleDbType.Date).Value = EntryDate;
            cmd.Parameters.AddWithValue("@Description", DescriptionText.Text);

            // open, add new, close
            cn.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch(OleDbException exc)
            {
                MessageBox.Show(exc.Message);
            }
            cn.Close();
        }

        // creates NumericUpDown for Amount
        public void CreateAmountNud()
        {
            // Create and initialize a NumericUpDown control.
            System.Windows.Forms.NumericUpDown amountNud = new System.Windows.Forms.NumericUpDown();

            // sets parameters for amountNud
            amountNud.Value = 0; // initial value
            amountNud.DecimalPlaces = 2; // decimal places
            amountNud.Increment = .50M; // how much a click should increment
            //amountNud.Margin;
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

        // generates "empty" table for usage of display
        private Table GenerateTable()
        {
            Table newTable = new Table();

            newTable.CellSpacing = 10;
            newTable.Background = Brushes.White;

            int numberOfColumns = 3;
            for (int x = 0; x < numberOfColumns; x++)
            {
                newTable.Columns.Add(new TableColumn());

                // alternating column colors
                if (x % 2 == 0)
                    newTable.Columns[x].Background = Brushes.LightGray;
                else
                    newTable.Columns[x].Background = Brushes.LightSlateGray;
            }

            // Create and add an empty TableRowGroup to hold the table's Rows.
            newTable.RowGroups.Add(new TableRowGroup());

            // Add the first (title) row.
            newTable.RowGroups[0].Rows.Add(new TableRow());

            // Alias the current working row for easy reference.
            TableRow currentRow = newTable.RowGroups[0].Rows[0];

            // format title row
            currentRow.Background = Brushes.AliceBlue;
            currentRow.FontSize = 22;
            currentRow.FontWeight = FontWeights.Bold;

            // adds title row and spans all columns
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Financials"))));
            currentRow.Cells[0].ColumnSpan = numberOfColumns;


            // Add the header row.
            newTable.RowGroups[0].Rows.Add(new TableRow());
            currentRow = newTable.RowGroups[0].Rows[1];

            // Global formatting for the header row.
            currentRow.FontWeight = FontWeights.SemiBold;

            // Add cells with content to the second row.
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Date"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Description"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Amount"))));

            return newTable;
        }

        // prevents non-numeric characters from being entered (except . and -)
        private void AmountText_TextChanged(object sender, TextChangedEventArgs e)
        {
                AmountText.Text = Regex.Replace(AmountText.Text, "[^0-9.-]+", "");
        }

        /*
         *  STUFF THAT NEEDS TO GET DONE:
         *      - form to enter:
         *          + amount (dynamic income or expense)
         *              - need to parse
         *              + numbers only (and '-' & '.')
         *          + description
         *          + date
         *      + display field to show a selected range's information
         *      + tie stuff to database
         *          + querying for info
         *          + storing info
         */
    }
}
