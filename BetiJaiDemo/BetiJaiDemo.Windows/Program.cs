using System.Diagnostics;
using WaveEngine.Common.Graphics;
using WaveEngine.DirectX11;
using WaveEngine.Forms;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveEngine.XAudio2;

namespace BetiJaiDemo.Windows
{
    internal class Program
    {
        private static void Main()
        {
            // Create app
            var application = new MyApplication();

            // Create Services
            uint width = 1280;
            uint height = 720;
            WindowsSystem windowsSystem = new FormsWindowsSystem();
            application.Container.RegisterInstance(windowsSystem);
            var window = windowsSystem.CreateWindow("BetiJaiDemo", width, height);

            ConfigureGraphicsContext(application, window);

            // Creates XAudio device
            var xaudio = new XAudioDevice();
            application.Container.RegisterInstance(xaudio);

            var notifier = new HotspotNotifier();
            application.Container.RegisterInstance<IHotspotNotifier>(notifier);

            var clockTimer = Stopwatch.StartNew();
            windowsSystem.Run(
                () => { application.Initialize(); },
                () =>
                {
                    var gameTime = clockTimer.Elapsed;
                    clockTimer.Restart();

                    application.UpdateFrame(gameTime);
                    application.DrawFrame(gameTime);
                });
        }

        private static void ConfigureGraphicsContext(Application application, Window window)
        {
            GraphicsContext graphicsContext = new DX11GraphicsContext();
            graphicsContext.CreateDevice();
            var swapChainDescription = new SwapChainDescription
            {
                SurfaceInfo = window.SurfaceInfo,
                Width = window.Width,
                Height = window.Height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = TextureSampleCount.None,
                IsWindowed = true,
                RefreshRate = 60
            };
            var swapChain = graphicsContext.CreateSwapChain(swapChainDescription);
            swapChain.VerticalSync = true;

            var graphicsPresenter = application.Container.Resolve<GraphicsPresenter>();
            var firstDisplay = new Display(window, swapChain);
            graphicsPresenter.AddDisplay("DefaultDisplay", firstDisplay);

            application.Container.RegisterInstance(graphicsContext);
        }
    }
}