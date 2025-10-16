using System;
using System.Threading;
using System.Windows;

namespace Battify
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private static Mutex _mutex = null;
        private const string MutexName = "Global\\Battify_SingleInstance_Mutex_9F8A3B2C";

        protected override void OnStartup(StartupEventArgs e)
        {
            // 뮤텍스 생성 시도
            _mutex = new Mutex(true, MutexName, out bool createdNew);

            if (!createdNew)
            {
                // 이미 실행 중인 인스턴스가 있음
                /*
                System.Windows.MessageBox.Show(
                    "Battify가 이미 실행 중입니다.\n시스템 트레이를 확인해주세요.",
                    "Battify",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                */

                // 애플리케이션 종료
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // 뮤텍스 해제
            _mutex?.ReleaseMutex();
            _mutex?.Dispose();
            
            base.OnExit(e);
        }
    }
}
