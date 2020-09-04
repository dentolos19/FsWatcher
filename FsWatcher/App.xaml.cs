using System.Windows;
using System.Windows.Threading;
using FsWatcher.Graphics;

namespace FsWatcher
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
            new WnException(args.Exception).ShowDialog();
        }

    }

}