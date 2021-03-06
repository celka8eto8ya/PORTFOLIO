using E2Book.BL.A_Model;
using E2Book.BL.C_Controller;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace KAM_19_08_2020_E2Book
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        bool b11 = true;
        private void Tb11_GotFocus(object sender, RoutedEventArgs e)
        {
            b11 = true;

            if (b11)
            {
                Tb11.Text = "";

            }
        }

        bool b10 = true;
        private void Tb10_GotFocus(object sender, RoutedEventArgs e)
        {
            b10 = true;

            if (b10)
            {
                Tb10.Text = "";

            }
        }


        /// <summary>
        /// Create new Note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn12_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Tb10.Text.Length > 0 && Tb11.Text.Length > 0 && DatePicker1.Text.Length > 0)
                {
                    NoteController.Add(Tb10.Text, Tb11.Text, DatePicker1, ref calendar1, ref dataGrid2);
                }
                else
                {
                    MessageBox.Show("Not all fields have a data.","Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "|" + ex);
            }
        }


        /// <summary>
        /// Delete chosen Note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn13_Click(object sender, RoutedEventArgs e)
        {
           
            NoteController.Delete(ref dataGrid2, ref calendar1);
        }


        /// <summary>
        /// Checked chosen Note
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn14_Click(object sender, RoutedEventArgs e)
        {
          
            NoteController.Checked(ref dataGrid2, ref calendar1);
        }


        /// <summary>
        /// Checked definate date at the calendar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Calendar1_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                NoteController.Show(calendar1.SelectedDate.ToString(), ref dataGrid2);
                NoteController.UpdateCalendar(ref calendar1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "--->" + ex);
            }
        }


        /// <summary>
        /// Event: (Start app)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteController.Filter("All current", ref dataGrid2);
                comboBox1.Text = "All current";
                DatePicker1.Text = DateTime.Now.ToShortDateString();
                NoteController.UpdateCalendar(ref calendar1);

                //AccountController.Show(ref dataGrid3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " " + ex);
            }
        }


        /// <summary>
        /// Event: (Choose category of sorting)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            ComboBoxItem selectedItem = (ComboBoxItem)comboBox.SelectedItem;

            NoteController.Filter(selectedItem.Content.ToString(), ref dataGrid2);
        }

        /// <summary>
        /// Event: (Open window for creating new account)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateNewAcc_Click(object sender, RoutedEventArgs e)
        {
            CreateAccount createAccount = new CreateAccount();
            createAccount.Show();
        }

      
        /// <summary>
        /// Show info about accounts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            AccountController.Show(ref dataGrid3);
            AccountController.GetNameAccounts(ref comboBox2);
        }

        /// <summary>
        /// Delete chosed account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountController.Delete(ref dataGrid3);
            AccountController.GetNameAccounts(ref comboBox2);
        }
    }
}
