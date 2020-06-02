using System.Windows;
using FsWatcher.Core;
using FsWatcher.Graphics;

namespace FsWatcher
{

    public partial class App
    {

        private void Initialize(object sender, StartupEventArgs args)
        {
            var accent = Utilities.GetRandomAccent();
            Utilities.SetAppTheme(accent);
            new WnMain().Show();
        }

    }

}
