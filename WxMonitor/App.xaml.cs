﻿using System.Windows;
using System.Windows.Threading;
using WxMonitor.Graphics;

namespace WxMonitor
{

    public partial class App
    {

        private void Initialize(object sender, StartupEventArgs args)
        {
            new WnMain().Show();
        }

        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            args.Handled = true;
            new WnExceptionHandler(args.Exception).ShowDialog();
        }

    }

}