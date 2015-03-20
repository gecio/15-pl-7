using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace ComputationalNode.ComputationalClient.Runner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //todo: podpiąć do cofiga
            int port;
            IPAddress ipAddress;
            try
            {
                for (int i = 0; i < e.Args.Length; i += 2)
                {
                    switch (e.Args[i])
                    {
                        case "-address":
                            ipAddress = IPAddress.Parse(e.Args[i + 1]);
                            break;
                        case "-port":
                            port = int.Parse(e.Args[i + 1]);
                            break;
                        default:
                            this.Shutdown();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //todo: logi
                this.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
