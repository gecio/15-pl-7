using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using ComputationalCluster.ComputationalClient;
using GalaSoft.MvvmLight.Command;

namespace ComputationalNode.ComputationalClient.Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ICommand _sendCommand;
        private ComputationalClientRunner _computationalClient;
        public String Data { get; set; }
        public String ProblemType { get; set; }

        public ICommand SendCommand
        {
            get
            {
                if (_sendCommand == null)
                {
                    _sendCommand =  new RelayCommand(Send);
                }
                return _sendCommand;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            _computationalClient = new ComputationalClientRunner();
        }

        private void Send()
        {
            _computationalClient.SendSolveRequest(Data,ProblemType);
        }
    }
}
