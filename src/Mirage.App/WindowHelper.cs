using Microsoft.UI.Xaml;
using Windows.Graphics;

namespace Mirage.App;

internal static class WindowHelper
{
    public static void ResizeOnFirstActivate(this Window window, int width, int height)
    {
        window.Activated += OnActivated;

        void OnActivated(object sender, WindowActivatedEventArgs e)
        {
            window.Activated -= OnActivated;
            if (window.AppWindow is not null)
            {
                window.AppWindow.Resize(new SizeInt32(width, height));
            }
        }
    }
}
