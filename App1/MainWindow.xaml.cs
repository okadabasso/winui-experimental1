using App1.ViewModels;
using App1.Views;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.Graphics.Display;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly IServiceProvider _provider;
        public MainWindow(IServiceProvider provider)
        {
            this.InitializeComponent();
            this.Activated += OnActivated;
            _provider = provider;
            NavView.SelectedItem = NavView.MenuItems[0]; // 初期表示: Home

            // プロバイダーから HomePage を取得して表示（取得できなければ従来の Navigate を使用）
            NavigateTo<HomePage>();

            // 呼び出し例: コンストラクタで一度だけ補正したいなら
        }



        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "home":
                        NavigateTo<HomePage>();
                        break;
                    case "sample":
                        NavigateTo<SamplePage>();
                        break;
                    case "forms":
                        NavigateTo<FormsSamplePage>();
                        break;
                }
            }
        }

        // IServiceProvider からページを解決して表示するヘルパー
        // 解決できればそのインスタンスを ContentFrame.Content に設定し、
        // 解決できなければ型で Navigate する（従来の動作をフォールバックで維持）
        private void NavigateTo<TPage>() where TPage : Page
        {
            if (_provider != null)
            {
                var resolved = _provider.GetService(typeof(TPage)) as Page;
                if (resolved != null)
                {
                    // すでに同じインスタンスが表示されていれば何もしない
                    if (ContentFrame.Content == resolved as object)
                    {
                        return;
                    }

                    // 既に初期化済みのページインスタンスがあれば、それに切り替える
                    // 直接 Content を差し替えると Frame.Navigate 時の遷移アニメーションが走らないため
                    // DI から解決したページに EntranceThemeTransition を追加して見た目を揃える
                    if (resolved is FrameworkElement fe)
                    {
                        if (fe.Transitions == null || fe.Transitions.Count == 0)
                        {
                            fe.Transitions = new TransitionCollection
                            {
                                new EntranceThemeTransition()
                            };
                        }
                    }

                    ContentFrame.Content = resolved;
                    return;
                }
            }

            // フォールバック: 型で Navigate
            ContentFrame.Navigate(typeof(TPage));
        }
        private void OnActivated(object sender, WindowActivatedEventArgs e)
        {
            this.Activated -= OnActivated;

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // 現在の DPI を取得（Win32 API）
            uint dpi = GetDpiForWindow(hwnd);
            double scale = dpi / 96.0;

            // 論理単位(DIP)を物理ピクセルに変換
            int widthPx = (int)Math.Round(1280 * scale);
            int heightPx = (int)Math.Round(800 * scale);

            appWindow.Resize(new SizeInt32(widthPx, heightPx));
        }

        [DllImport("User32.dll")]
        private static extern uint GetDpiForWindow(IntPtr hwnd);
    }
}
