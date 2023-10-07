﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using BetterGenshinImpact.Helpers.Extensions;
using Fischless.WindowCapture;

namespace BetterGenshinImpact.View
{
    /// <summary>
    /// CaptureTestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CaptureTestWindow : Window
    {
        private IWindowCapture? _capture;


        private long _captureTime;
        private long _transferTime;
        private long _captureCount;

        public CaptureTestWindow()
        {
            _captureTime = 0;
            _transferTime = 0;
            _captureCount = 0;
            InitializeComponent();
            Closed += (sender, args) =>
            {
                CompositionTarget.Rendering -= Loop;
                _capture?.Stop();

                Debug.WriteLine("平均截图耗时:" + _captureTime * 1.0 / _captureCount);
                Debug.WriteLine("平均转换耗时:" + _transferTime * 1.0 / _captureCount);
                Debug.WriteLine("平均总耗时:" + (_captureTime + _transferTime) * 1.0 / _captureCount);
            };
        }

        public void StartCapture(IntPtr hWnd, CaptureModes captureMode)
        {
            if (hWnd == IntPtr.Zero)
            {
                MessageBox.Show("请选择窗口");
                return;
            }


            _capture = WindowCaptureFactory.Create(captureMode);
            _capture.IsClientEnabled = true;
            _capture.Start(hWnd);

            CompositionTarget.Rendering += Loop;
        }

        private void Loop(object? sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();
            var bitmap = _capture?.Capture();
            sw.Stop();
            Debug.WriteLine("截图耗时:" + sw.ElapsedMilliseconds);
            _captureTime += sw.ElapsedMilliseconds;

            if (bitmap != null)
            {
                _captureCount++;
                sw.Reset();
                sw.Start();
                DisplayCaptureResultImage.Source = bitmap.ToBitmapImage();
                sw.Stop();
                Debug.WriteLine("转换耗时:" + sw.ElapsedMilliseconds);
                _transferTime += sw.ElapsedMilliseconds;
            }
            else
            {
                Debug.WriteLine("截图失败");
            }
        }
    }
}