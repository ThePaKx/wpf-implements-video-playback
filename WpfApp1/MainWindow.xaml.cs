using Microsoft.Win32;
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

namespace MediaPro
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Timers.Timer timer = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 打开选择播放文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = @"视频文件(*.avi格式)|*.avi|视频文件(*.wav格式)
                |*.wav |视频文件(*.wmv)|*.wmv |视频文件(*.mp4格式)|*.mp4|All Files|*.*";
            if (ofd.ShowDialog() == false)
            {
                return;
            }
            string path = "";
            path = ofd.FileName;
            if (path == "")
                return;
            me.Source = new Uri(path, UriKind.Absolute);
            btn_play.IsEnabled = true;
            me.Play();
            timer.Start();
            btn_play.Content = "暂停";
        }

        private void btn_play_Click(object sender, RoutedEventArgs e)
        {
            SetPlayer(true);
            PlayerPause();
        }

        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            PlayerStop();
        }

        /// <summary>
        /// 回退10s
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            me.Pause();
            timer.Stop();
            me.Position = me.Position - TimeSpan.FromSeconds(10);
            SetTime();
            me.Play();
            timer.Start();
        }

        private void btn_foreward_Click(object sender, RoutedEventArgs e)
        {
            me.Pause();
            timer.Stop();
            me.Position = me.Position + TimeSpan.FromSeconds(10);
            SetTime();
            me.Play();
            timer.Start();
        }

        private void changeMax()
        {
            sl_progress.Maximum = me.NaturalDuration.TimeSpan.TotalSeconds;
            tb_time.Text = "0:00:00";
            SetPlayer(true);
        }

        private void me_MediaEnded(object sender, RoutedEventArgs e)
        {
            tb_time.Text = "播放完毕！";
            timer.Stop();
        }

        private void me_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PlayerPause();
        }

        private void sl_progress_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            me.Pause();
            timer.Stop();
            int val = (int)sl_progress.Value;
            me.Position = new TimeSpan(0, 0, 0, val);
            SetTime();
            me.Play();
            timer.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;
            //每间隔一秒要做的事
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                sl_progress.Value = me.Position.TotalSeconds;
                SetTime();
            }));
        }

        private void SetTime()
        {
            tb_time.Text = string.Format("{0:00}:{1:00}:{2:00}",
                me.Position.Hours, me.Position.Minutes,
                me.Position.Seconds);
        }

        private void PlayerPause()
        {
            if (btn_play.Content.ToString() == "播放")
            {
                me.Play();
                timer.Start();
                btn_play.Content = "暂停";
                me.ToolTip = "点击暂停";
            }
            else
            {
                me.Pause();
                timer.Stop();
                btn_play.Content = "播放";
                me.ToolTip = "点击播放";
            }
        }

        private void SetPlayer(bool bl)
        {
            btn_play.IsEnabled = bl;
            btn_stop.IsEnabled = bl;
            btn_foreward.IsEnabled = bl;
            btn_back.IsEnabled = bl;
        }

        private void PlayerStop()
        {
            timer.Stop();
            me.Stop();
            sl_progress.Value = 0;
            tb_time.Text = "0:00:00";
            btn_play.Content = "播放";
        }

        private void me_MediaOpened_1(object sender, RoutedEventArgs e)
        {
            changeMax();
        }
    }
}