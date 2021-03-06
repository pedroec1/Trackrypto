﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HMY.Helpers.Collections;
using HMY.Infrastructure.AsyncResponse;
using Mapster;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trackrypto.Helpers;
using Trackrypto.Model;
using Trackrypto.Model.Entities;
using Trackrypto.ViewModel.EntityViewModel;
using Trackrypto.ViewModel.Messenger;
using Trackrypto.ViewModel.Navigation;
using Trackrypto.ViewModel.ViewViewModel;

namespace Trackrypto.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        #region private
        private readonly Domain model;

        private IPageViewModel currentPageViewModel;

        private string path;
        #endregion

        public IPageViewModel CurrentPageViewModel
        {
            get => currentPageViewModel;
            set
            {
                currentPageViewModel = value;
                RaisePropertyChanged();
            }
        }


        public string Path
        {
            get => path;
            set
            {
                path = value;
                RaisePropertyChanged();
            }
        }


        public MainViewModel()
        {
            model = Domain.GetModel();
            Path = "";
            
            WireCommands();
        }


        #region comands
        public ICommand OpenCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand SaveAsCommand { get; private set; }

        public ICommand GoToSummaryCommand { get; private set; }
        public ICommand GoToTransactionListCommand { get; private set; }

        private void WireCommands()
        {
            OpenCommand = new RelayCommand(() => OpenFile());
            SaveCommand = new RelayCommand(() => SaveTransactions());
            SaveAsCommand = new RelayCommand(() => SaveTransactions(true));

            GoToSummaryCommand = new RelayCommand(() => GoToSummary());
            GoToTransactionListCommand = new RelayCommand(() => GoToTransactionList());
        }
        #endregion


        #region navigation
        private void GoToSummary()
        {
            var viewModel = new SummaryViewModel();
            CurrentPageViewModel = viewModel;
        }

        private void GoToTransactionList()
        {
            var viewModel = new TransactionListViewModel();
            CurrentPageViewModel = viewModel;
        }
        #endregion


        #region file manage
        private void SaveTransactions(bool saveAs = false)
        {
            if ((saveAs == true) || string.IsNullOrWhiteSpace(Path))
            {
                bool selected = SelectSavePath();
                if (selected == false) return;
            }


            model.Save(Path);
            // Pasar a sin cambios
        }


        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog { Filter = "JSON (.json)|*.json" };
            if (openFileDialog.ShowDialog() == false) return;

            Path = openFileDialog.FileName;
            model.OpenFile(Path);

            CurrentPageViewModel?.Update(true);
        }


        private bool SelectSavePath()
        {
            var dialog = new SaveFileDialog
            {
                FileName = "Transacciones",
                DefaultExt = ".json",
                Filter = "JSON (.json)|*.json"
            };

            var result = dialog.ShowDialog();
            if (result == true)
            {
                Path = dialog.FileName;
                return true;
            }

            return false;
        }
        #endregion
    }
}