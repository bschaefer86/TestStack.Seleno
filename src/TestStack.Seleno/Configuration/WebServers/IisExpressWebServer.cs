﻿using System;
using System.Diagnostics;
using System.Threading;

using TestStack.Seleno.Configuration.Contracts;

namespace TestStack.Seleno.Configuration.WebServers
{
    public class IisExpressWebServer : IWebServer
    {
        private static Process _webHostProcess;
        private static int _port;
        private static string _websitePath;

        public IisExpressWebServer(string websitePath, int port)
        {
            _websitePath = websitePath;
            _port = port;
        }

        public void Start()
        {
            var thread = new Thread(StartIisExpress) { IsBackground = true };
            thread.Start();
        }

        public void Stop()
        {
            KillHosts();
        }

        public string BaseUrl
        {
            get { return string.Format("http://localhost:{0}/", _port); }
        }

        private static void StartIisExpress()
        {
            var webHostStartInfo = ProcessStartInfo(_websitePath, _port);

            try
            {
                _webHostProcess = Process.Start(webHostStartInfo);
                _webHostProcess.WaitForExit();
            }
            catch
            {
                KillHosts();
            }
        }

        private static ProcessStartInfo ProcessStartInfo(string applicationPath, int port)
        {
            var key = Environment.Is64BitOperatingSystem ? "programfiles(x86)" : "programfiles";
            var programfiles = Environment.GetEnvironmentVariable(key);

            return new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                Arguments = String.Format("/path:\"{0}\" /port:{1}", applicationPath, port),
                FileName = string.Format("{0}\\IIS Express\\iisexpress.exe", programfiles)
            };
        }

        private static void KillHosts()
        {
            try
            {
                _webHostProcess.CloseMainWindow();
                _webHostProcess.Dispose();
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
                // Whatever dude. I do not care               
            }
        }
    }
}
