using System.Windows;
using FsWatcher.Graphics;

namespace FsWatcher
{

    public partial class App
    {

        private void Initialize(object sender, StartupEventArgs e)
        {
            new WnMain().Show();
        }

    }

}