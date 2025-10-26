using App1.Views;
using App1.ViewModels;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App1
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private IConfiguration _configuration = null!;
        private IServiceProvider _serviceProvider = null!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // 1. Configuration の構築
            _configuration = BuildConfiguration();

            // 2. DI Container の構築
            _serviceProvider = BuildServiceProvider();

            // 3. MainWindow の表示
            _window = _serviceProvider.GetRequiredService<MainWindow>();
            if (_window == null)
            {
                throw new InvalidOperationException("MainWindow の取得に失敗しました。");
            }

            _window.Activate();
        }
        private IConfiguration BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // 環境変数があれば環境固有の設定ファイルも読み込み
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            return builder.Build();
        }

        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();

            // Configuration をサービスに登録
            services.AddSingleton<IConfiguration>(_configuration);
            // Logging の設定
            ConfigureLogging(services);

            // Settings の設定
            ConfigureSettings(services);

            // DryIocコンテナの設定
            var container = BuildContainer(services);

            return container.BuildServiceProvider();
        }
        private IContainer BuildContainer(IServiceCollection services)
        {
            // DryIocコンテナの設定
            var container = new Container(rules => rules.With(propertiesAndFields: PropertiesAndFields.Auto))
                .WithDependencyInjectionAdapter(services);

            // ViewModelsとViewsの登録
            container.Register<MainWindowViewModel>(Reuse.Singleton);
            container.Register<MainWindow>(Reuse.Singleton);

            // View を登録する
            container.Register<HomePage>(Reuse.Singleton);
            container.Register<FormsSamplePage>(Reuse.Singleton);
            // ViewModel を登録する
            container.Register<HomePageViewModel>(Reuse.Singleton);
            container.Register<FormSamplePageViewModel>(Reuse.Singleton);

            // tab factory
            return container;
        }

        private void ConfigureLogging(IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog();
            });
        }

        private void ConfigureSettings(IServiceCollection services)
        {
            // 設定クラスがある場合は Options パターンで登録
            // これにより ViewModel 等で IOptions<SampleSettings> を注入できる
            services.Configure<SampleSettings>(_configuration.GetSection("SampleSettings"));
        }

    }
}
