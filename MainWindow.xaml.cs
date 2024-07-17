using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow i_MainWindow;

        public MainWindow()
        {
            InitializeComponent();

            i_MainWindow = this;

            if (Main.Initialize())
            {
                btnStartBot.IsEnabled = true;
            }
        }

        internal static void OnNewChat(string _text)
        {
            i_MainWindow.Dispatcher.Invoke(() =>
            {
                i_MainWindow.txtLastChat.Text = _text;
            });
        }

        private async void btnStartBot_Click(object sender, RoutedEventArgs e)
        {
            // DEBUG
            /*int _OneRow = Int32.Parse(inptOneRow.Text);
            int _OneColumn = Int32.Parse(inptOneColumn.Text);
            int _TwoRow = Int32.Parse(inptTwoRow.Text);
            int _TwoColumn = Int32.Parse(inptTwoColumn.Text);

            Main.CompareTest(_OneRow, _OneColumn, _TwoRow, _TwoColumn);*/
            
            // DEBUG
            /*for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Main.CompareTest(i, j, 0, 0);
                }
            }*/
            
            // DEBUG
            //Thread.Sleep(2000);
            //await Task.Run(Main.WatchChestTest);
            //btnStopBot.IsEnabled = true;
            
            // DEBUG
            //Thread.Sleep(2000);
            //OCRAPI.ReturnCurrentItemName();

            // start watching chat to see if someone greets us
            Main.StartFreshOCRChatLoop();
            btnStopBot.IsEnabled = true;
        }

        private void btnStopBot_Click(object sender, RoutedEventArgs e)
        {
            Main.StopOCRChatLoop();
            btnStopBot.IsEnabled = false;
        }

        internal static void OnUpdateTransactionTimer(double _time)
        {
            i_MainWindow.Dispatcher.Invoke(() =>
            {
                i_MainWindow.txtTradingTimer.Text = _time.ToString("N1");
            });
        }

        internal static void OnUpdateStatus(string _text)
        {
            i_MainWindow.Dispatcher.Invoke(() =>
            {
                i_MainWindow.txtBotStatus.Text = _text;
            });
        }

        internal static void OnUpdateTransaction(Transaction _transaction) 
        {
            i_MainWindow.Dispatcher.Invoke(() =>
            {
                switch (_transaction.Status)
                {
                    case Library.TransactionStatus.Greeting:
                        i_MainWindow.txtTransaction.Text = String.Format("ID: {0} | Greeting {1}", _transaction.ID, _transaction.Name);
                        break;
                    case Library.TransactionStatus.Discussing:
                        i_MainWindow.txtTransaction.Text = String.Format("ID: {0} | {1} wants {2} x{3}", _transaction.ID, _transaction.Name, _transaction.Wants, _transaction.WantsQuantity);
                        break;
                    case Library.TransactionStatus.Trading:
                        i_MainWindow.txtTransaction.Text = String.Format("ID: {0} | waiting for {1}'s {2} x{3} in the chest", _transaction.ID, _transaction.Name, _transaction.Has, _transaction.HasQuantity);
                        break;
                    case Library.TransactionStatus.Thanking:
                        i_MainWindow.txtTransaction.Text = String.Format("ID: {0} | thanking {1}, sold {2} x{3} for {4} x{5}", _transaction.ID, _transaction.Name, _transaction.Wants, _transaction.WantsQuantity, _transaction.Has, _transaction.HasQuantity);
                        break;
                }
            });
        }

        internal static void OnEndTransaction(Transaction _transaction) 
        {
            i_MainWindow.Dispatcher.Invoke(() =>
            {
                i_MainWindow.txtTransaction.Text = String.Format("ID: {0} | sold {1} {2} x{3} for {4} x{5}", _transaction.ID, _transaction.Name, _transaction.Wants, _transaction.WantsQuantity, _transaction.Has, _transaction.HasQuantity);
            });
            
        }
    }
}