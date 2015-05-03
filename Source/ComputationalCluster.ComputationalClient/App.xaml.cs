using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace ComputationalCluster.ComputationalClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // bad(simplest) way
        public static IPAddress ArgAddress;
        public static int ArgPort;
 
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                for (int i = 0; i < e.Args.Length; i += 2)
                {
                    switch (e.Args[i])
                    {
                        case "-address":
                            ArgAddress = IPAddress.Parse(e.Args[i + 1]);
                            break;
                        case "-port":
                            ArgPort = int.Parse(e.Args[i + 1]);
                            break;
                        default:
                            this.Shutdown();
                            break;
                    }
                }
            }
            catch (Exception)
            {
                //todo: logi
                this.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}
