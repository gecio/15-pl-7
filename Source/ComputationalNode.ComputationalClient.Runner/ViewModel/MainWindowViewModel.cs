using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows.Input;

namespace ComputationalCluster.ComputationalClient.Runner.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            _cClient = new ComputationalClientRunner();
        }

        //================================
        private ICommand _loadFileCommand;
        private ICommand _sendSolveRequestCommand;
        public string _fileContent;
        private string _problemType;
        private ComputationalClientRunner _cClient;
        private int _lastSolveRequestId;
        private int _timeout;
        //================================

        public string ProblemType
        {
            get { return _problemType; }
            set
            {
                _problemType = value;
                RaisePropertyChanged(() => ProblemType);
            }
        }
        public ICommand LoadFileCommand
        {
            get
            {
                if (_loadFileCommand == null)
                {
                    _loadFileCommand = new RelayCommand(LoadFile, () => { return String.IsNullOrWhiteSpace(_fileContent); });
                }
                return _loadFileCommand;
            }
        }
        public ICommand SendSolveRequestCommand
        {
            get
            {
                if (_sendSolveRequestCommand == null)
                {
                    _sendSolveRequestCommand = new RelayCommand(SendSolveRequest, () =>
                    {
                        return !String.IsNullOrWhiteSpace(_fileContent) && !String.IsNullOrWhiteSpace(ProblemType);
                    });
                }
                return _sendSolveRequestCommand;
            }
        }
        public int LastSolveRequestId
        {
            get { return _lastSolveRequestId; }
            set
            {
                _lastSolveRequestId = value;
                RaisePropertyChanged();
            }
        }
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
                RaisePropertyChanged();
            }
        }

        private void LoadFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                using (var sr = new StreamReader(ofd.OpenFile()))
                {
                    _fileContent = sr.ReadToEnd();
                }
            }
        }

        private void SendSolveRequest()
        {
            ulong problemId;
            if (Timeout != 0) 
            {
                problemId = _cClient.SendSolveRequest(_fileContent, ProblemType,(ulong)Timeout);
            }
            else
            {
                problemId = _cClient.SendSolveRequest(_fileContent, ProblemType);
            }
            
            _fileContent = null;
            ProblemType = null;
            Timeout = 0;
        }
    }
}