using AutoPonto.Watcher;
using AutoPonto.Repository;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using AutoPonto.Models;
using System.Runtime.InteropServices;

namespace AutoPonto
{
    public class Program
    {
        private static ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();

        private static ChromeOptions options = new ChromeOptions();

        private static ManualResetEvent quitEvent = new ManualResetEvent(false);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);

        public static void Main()
        {
            IWebRepository webContext = GetWebContext();

            if (webContext != default)
            {
                //Inicia os serviços
                new TimeWatch(GetWebContext(), new UserPreferences());

                MessageBox((IntPtr)0, $"AutoPontoService em execução com o PID {Process.GetCurrentProcess().Id}.", "AutoPonto", 0);

                //Interrompe o console de fechar
                Stop();
            }            
        }

        private static IWebRepository GetWebContext()
        {
            try
            {
                ///Elimina todos os processos existentes do chromedriver
                foreach (Process instance in Process.GetProcessesByName("chromedriver.exe"))
                {
                    instance.Kill();
                }

                //Define propriedade pra esconder o prompt                
                driverService.HideCommandPromptWindow = true;

                //Argumentos para iniciar o chrome em modo silencioso
                options.AddArguments(new List<string>() {
                "--silent-launch",
                "--no-startup-window",
                "no-sandbox",
                "headless",});

                ChromeDriver driver = new ChromeDriver(driverService, options);

                IWebRepository webContext = new WebRepository(driver);

                return webContext;
            }
            catch (Exception ex)
            {
                MessageBox((IntPtr)0, ex.ToString(), "AutoPonto", 0);
                return default;
            }            
        }

        private static void Stop()
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                quitEvent.Set();
                eArgs.Cancel = true;
            };

            quitEvent.WaitOne();
        }
    }
}