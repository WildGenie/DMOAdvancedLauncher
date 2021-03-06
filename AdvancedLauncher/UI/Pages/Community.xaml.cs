﻿// ======================================================================
// DIGIMON MASTERS ONLINE ADVANCED LAUNCHER
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using AdvancedLauncher.Database;
using AdvancedLauncher.Model;
using AdvancedLauncher.Model.Proxy;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Management.Configuration;
using AdvancedLauncher.SDK.Model.Config;
using AdvancedLauncher.SDK.Model.Entity;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.Model.Web;
using AdvancedLauncher.UI.Validation;
using Ninject;

namespace AdvancedLauncher.UI.Pages {

    public partial class Community : AbstractPage, IWebProviderEventAccessor, IDisposable {

        private delegate void DoOneText(string text);

        private IWebProvider webProvider;

        private IServersProvider serversProvider;

        private GuildInfoViewModel GuildInfoModel;

        private Guild CurrentGuild = new Guild() {
            Id = -1
        };

        private WebProviderEventAccessor Proxy;

        [Inject]
        public IConfigurationManager ConfigurationManager {
            get; set;
        }

        [Inject]
        public IDialogManager DialogManager {
            get; set;
        }

        [Inject]
        public MergeHelper MergeHelper {
            get; set;
        }

        public Community() {
            Proxy = new WebProviderEventAccessor(this);
            InitializeComponent();
            GuildInfoModel = new GuildInfoViewModel(LanguageManager);
            GuildInfo.DataContext = GuildInfoModel;
        }

        protected override void OnProfileChanged(object sender, BaseEventArgs e) {
            IConfiguration currentConfiguration = ConfigurationManager.GetConfiguration(ProfileManager.CurrentProfile.GameModel);
            serversProvider = currentConfiguration.ServersProvider;
            webProvider = currentConfiguration.CreateWebProvider();
            GuildInfoModel.UnLoadData();
            TDBlock_.ClearAll();
            IsDetailedCheckbox.IsChecked = false;
            // use lazy ServerList initialization to prevent first long EF6 database
            // init causes the long app start time
            if (IsPageActivated) {
                LoadServerList();
            }
        }

        protected override void OnShow() {
            LoadServerList();
        }

        private void LoadServerList() {
            //Загружаем новый список серверов
            ComboBoxServer.ItemsSource = serversProvider.ServerList;
            Profile currentProfile = ProfileManager.CurrentProfile;
            //Если есть название гильдии в ротации, вводим его и сервер
            if (!string.IsNullOrEmpty(currentProfile.Rotation.Guild)) {
                foreach (Server serv in ComboBoxServer.Items) {
                    //Ищем сервер с нужным идентификатором и выбираем его
                    if (serv.Identifier == currentProfile.Rotation.ServerId) {
                        ComboBoxServer.SelectedValue = serv;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(GuildNameTextBox.Text)) {
                    GuildNameTextBox.Text = currentProfile.Rotation.Guild;
                }
            } else {
                GuildNameTextBox.Clear();
                if (ComboBoxServer.Items.Count > 0) {
                    ComboBoxServer.SelectedIndex = 0;
                }
            }
        }

        private void OnGetInfoClick(object sender, RoutedEventArgs e) {
            if (IsValidName(GuildNameTextBox.Text)) {
                webProvider.DownloadStarted += Proxy.OnDownloadStarted;
                webProvider.DownloadCompleted += Proxy.OnDownloadCompleted;
                webProvider.StatusChanged += Proxy.OnStatusChanged;
                webProvider.GetActualGuildAsync((Server)ComboBoxServer.SelectedValue,
                    GuildNameTextBox.Text,
                    (bool)IsDetailedCheckbox.IsChecked,
                    1);
            }
        }

        public void BlockControls(bool block) {
            ProfileManager.OnProfileLocked(block);
            GuildNameTextBox.IsEnabled = !block;
            ComboBoxServer.IsEnabled = !block;
            SearchButton.IsEnabled = !block;
            IsDetailedCheckbox.IsEnabled = !block;
        }

        #region Обработка поля ввода имени гильдии

        public bool IsValidName(string name) {
            if (name == LanguageManager.Model.CommGuildName) {
                DialogManager.ShowErrorDialog(LanguageManager.Model.CommGuildNameEmpty);
                return false;
            }
            GuildNameValidationRule validationRule = new GuildNameValidationRule();
            ValidationResult result = validationRule.Validate(name, new System.Globalization.CultureInfo(1, false));
            if (!result.IsValid) {
                DialogManager.ShowErrorDialog(result.ErrorContent.ToString());
            }
            return result.IsValid;
        }

        #endregion Обработка поля ввода имени гильдии

        #region Event handlers

        public void OnDownloadStarted(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnDownloadStarted(s, e2);
                }), sender, e);
                return;
            }
            BlockControls(true);
            LoadProgressBar.Value = 0;
            LoadProgressBar.Maximum = 100;
            LoadProgressStatus.Text = string.Empty;
            ProgressBlock.Visibility = System.Windows.Visibility.Visible;
        }

        public void OnDownloadCompleted(object sender, DownloadCompleteEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadCompleteEventHandler((s, e2) => {
                    OnDownloadCompleted(s, e2);
                }), sender, e);
                return;
            }

            BlockControls(false);

            webProvider.DownloadStarted -= Proxy.OnDownloadStarted;
            webProvider.DownloadCompleted -= Proxy.OnDownloadCompleted;
            webProvider.StatusChanged -= Proxy.OnStatusChanged;

            ProgressBlock.Visibility = Visibility.Collapsed;
            switch (e.Code) {
                case DMODownloadResultCode.OK:
                    {
                        CurrentGuild = MergeHelper.Merge(e.Guild);
                        GuildInfoModel.LoadData(CurrentGuild);
                        TDBlock_.SetGuild(CurrentGuild);
                        break;
                    }
                case DMODownloadResultCode.CANT_GET:
                    {
                        DialogManager.ShowErrorDialog(LanguageManager.Model.CantGetError);
                        break;
                    }
                case DMODownloadResultCode.NOT_FOUND:
                    {
                        DialogManager.ShowErrorDialog(LanguageManager.Model.GuildNotFoundError);
                        break;
                    }
                case DMODownloadResultCode.WEB_ACCESS_ERROR:
                    {
                        DialogManager.ShowErrorDialog(LanguageManager.Model.ConnectionError);
                        break;
                    }
            }
        }

        public void OnStatusChanged(object sender, DownloadStatusEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new DownloadStatusChangedEventHandler((s, e2) => {
                    OnStatusChanged(s, e2);
                }), sender, e);
                return;
            }
            switch (e.Code) {
                case DMODownloadStatusCode.GETTING_GUILD:
                    {
                        LoadProgressStatus.Text = LanguageManager.Model.CommSearchingGuild;
                        break;
                    }
                case DMODownloadStatusCode.GETTING_TAMER:
                    {
                        LoadProgressStatus.Text = string.Format(LanguageManager.Model.CommGettingTamer, e.Info);
                        break;
                    }
            }
            LoadProgressBar.Maximum = e.MaxProgress;
            LoadProgressBar.Value = e.Progress;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    GuildInfoModel.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(true);
        }

        #endregion IDisposable Support

        #endregion Event handlers
    }
}