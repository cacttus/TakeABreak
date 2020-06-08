using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using WpfApp1.UnManaged;
using MahApps.Metro.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Diagnostics;

//We need 1 timer per window for multi-window displays.
namespace WpfApp1
{
   public enum TimerState
   {
      Run, Stop
   }

   /// <summary>
   /// Interaction logic for ControlForm.xaml
   /// </summary>
   public partial class ControlForm : MetroWindow
   {
      List<TimerWindow> _timerWindows = new List<TimerWindow>();
      WpfApp1.UnManaged.HotKey _hotkey = null;
      double _curTimeMs = 0;
      TimerState _timerState = TimerState.Stop;
      Stopwatch _stopWatch = new Stopwatch();
      MicroLibrary.MicroTimer dispatcherTimer = new MicroLibrary.MicroTimer();
      Dispatcher _mainDisp = null;
      bool _bFullscreen = true; 

      public ControlForm()
      {
         InitializeComponent();
         ShowTitleBar = false;
         WindowStyle = WindowStyle.None;
         WindowState = WindowState.Normal;
         this.Visibility = Visibility.Hidden;
         _mainDisp = System.Windows.Application.Current.MainWindow.Dispatcher;

         //Create windows
         foreach (var screen in Screen.AllScreens)
         {
            TimerWindow t = new TimerWindow();
            t.Create(this, screen.Bounds);
            _timerWindows.Add(t);
         }
         StartTimer();

         SetupHotkeys();
      }
      public void Pause()
      {
         _stopWatch.Stop();
         _curTimeMs = 0;
         _timerState = TimerState.Stop;
      }
      public void StopAndHide()
      {
         Pause();
         HideAllTimers();
      }
      private void SetupHotkeys()
      {
         _hotkey = new UnManaged.HotKey(Key.Space, KeyModifier.Ctrl, (x) =>
         {
            AddTime(30 * 1000);

            foreach (TimerWindow t in _timerWindows)
            {
               t.PopStart(_bFullscreen);
            }
         });
      }
      private void AddTime(double time)
      {
         _curTimeMs += time;
         _timerState = TimerState.Run;
         UpdateWindowLabelTimers();
      }
      private void UpdateWindowLabelTimers()
      {
         foreach (TimerWindow t in _timerWindows)
         {
            t.UpdateLabelTimer(_curTimeMs);
         }
      }
      private void Stop()
      {
         Pause();
      }
      private void HideAllTimers()
      {
         foreach (TimerWindow t in _timerWindows)
         {
            t.Hide();
         }
      }
      private void StartTimer()
      {
         dispatcherTimer.MicroTimerElapsed += (x, y) =>
         {
            if (_timerState == TimerState.Run)
            {
               if (!_stopWatch.IsRunning)
               {
                  _stopWatch.Reset();
                  _stopWatch.Start();
               }

               if (_stopWatch.ElapsedMilliseconds > 100)
               {
                  _stopWatch.Stop();

                  if (_curTimeMs > 0)
                  {
                     _curTimeMs -= 100;
                     UpdateWindowLabelTimers();
                  }
                  if (_curTimeMs <= 0)
                  {
                     _curTimeMs = 0;
                     _mainDisp.BeginInvoke(new Action(() =>
                     {
                        StopAndHide();
                     }));
                  }
                  else
                  {
                     _stopWatch.Reset();
                     _stopWatch.Start();
                  }
               }
            }

         };
         dispatcherTimer.Interval = (long)(10000);//millis
         dispatcherTimer.Start();
      }

      private void _btnClose_Click(object sender, RoutedEventArgs e)
      {
         Environment.Exit(0);
      }
   }
}
