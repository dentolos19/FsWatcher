﻿using System;
using System.Windows;
using WxMonitor.Core;

namespace WxMonitor.Graphics
{

    public partial class WnExceptionHandler
    {

        public WnExceptionHandler(Exception error)
        {
            InitializeComponent();
            MessageText.Text = error.Message;
            StackTraceText.Text = error.StackTrace ?? "The error info doesn't contain stack trace data.";
        }

        private void Restart(object sender, RoutedEventArgs args)
        {
            Utilities.RestartApp();
        }

        private void Exit(object sender, RoutedEventArgs args)
        {
            Application.Current.Shutdown();
        }

    }

}