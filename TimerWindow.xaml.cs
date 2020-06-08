using System.Drawing;
using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls;
using WpfApp1;
using System.Windows.Threading;
using System.Threading;
using Hardcodet.Wpf;
using System.Globalization;
using System.Diagnostics;

namespace WpfApp1
{

   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class TimerWindow : MetroWindow
   {

      ControlForm _controlForm = null;
      Rectangle _bounds = new Rectangle(0, 0, 0, 0);//Screen bounds.
      Dispatcher _mainDisp = null;
      double _origW = 0;
      double _origH = 0;
      #region Public: Methods
      public TimerWindow()
      {
         InitializeComponent();
         WindowStyle = WindowStyle.None;
         _mainDisp = Application.Current.MainWindow.Dispatcher;
         Visibility = Visibility.Hidden;
         WindowStartupLocation = WindowStartupLocation.Manual;
      }
      public void Create(ControlForm cf, Rectangle bounds)
      {
         this._controlForm = cf;
         this._bounds = bounds;
         _origW = this.Width;
         _origH = this.Height;
         _lblTimer.Content = "0:00";
      }
      public void PopStart(bool fs)
      {
         Visibility = Visibility.Visible;
         this.Topmost = true;

         //Center
         if (!fs)
         {
            this.Left = _bounds.Left + _bounds.Width / 2 - this.Width / 2;
            this.Top = _bounds.Top + _bounds.Height / 2 - this.Height / 2;
            this.Width = _origW;
            this.Height = _origH;
         }
         else
         {
            this.Left = _bounds.Left;
            this.Top = _bounds.Top;
            this.Width = _bounds.Width;
            this.Height = _bounds.Height;
         }
      }
      #endregion

      #region Private: UI Methods
      private void _btnCancel_Click(object sender, RoutedEventArgs e)
      {
         _controlForm.StopAndHide();
      }
      private void _btnStop_Click(object sender, RoutedEventArgs e)
      {
         _controlForm.Pause();
      }
      #endregion

      #region Private: Methods

      public void UpdateLabelTimer(double time_ms)
      {
         _mainDisp.BeginInvoke(new Action(() =>
         {
            string ms = "." + (int)((time_ms % 1000) / 1000 * 10);
            _lblTimer.Content = TimeSpan.FromMilliseconds(time_ms).ToString(@"mm\:ss") + ms;
         }));
      }

      #endregion

   }
}
