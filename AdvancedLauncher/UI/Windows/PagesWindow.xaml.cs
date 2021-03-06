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
using System.Windows.Media;
using AdvancedLauncher.Management;
using AdvancedLauncher.Model;
using AdvancedLauncher.SDK.Management;
using AdvancedLauncher.SDK.Model;
using AdvancedLauncher.SDK.Model.Events;
using AdvancedLauncher.SDK.UI;
using AdvancedLauncher.UI.Extension;
using Ninject;

namespace AdvancedLauncher.UI.Windows {

    public partial class PagesWindow : AbstractWindowControl {
        private PageContainer currentTab;

        [Inject]
        public IEnvironmentManager EnvironmentManager {
            get; set;
        }

        [Inject]
        public IProfileManager ProfileManager {
            get; set;
        }

        public PagesWindow() {
            InitializeComponent();
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject())) {
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);
                WindowManager WM = App.Kernel.Get<IWindowManager>() as WindowManager;
                NavControl.ItemsSource = WM.PageItems.GetLinkedProxy<PageItem, PageItemViewModel>(LanguageManager);
                EnvironmentManager.FileSystemLocked += OnFileSystemLocked;
                ProfileManager.ProfileChanged += OnProfileChanged;
                OnProfileChanged(this, BaseEventArgs.Empty);
            }
        }

        private void OnTabChanged(object sender, SelectionChangedEventArgs e) {
            PageItemViewModel selectedTab = NavControl.SelectedValue as PageItemViewModel;
            if (selectedTab == null) {
                return;
            }
            PageContainer selectedPage = selectedTab.Item.Content as PageContainer;
            if (selectedPage != null) {
                //Prevent handling over changing inside tab item
                if (currentTab == selectedPage) {
                    return;
                }
                if (currentTab != null) {
                    try {
                        currentTab.OnClose();
                    } catch (AppDomainUnloadedException) { }
                }
                currentTab = selectedPage;
                currentTab.OnShow();
            }
        }

        private void OnFileSystemLocked(object sender, LockedEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new LockedChangedHandler((s, e2) => {
                    OnFileSystemLocked(sender, e2);
                }), sender, e);
                return;
            }
            if (e.IsLocked) {
                NavControl.SelectedIndex = 0;
            }
        }

        private void OnProfileChanged(object sender, BaseEventArgs e) {
            if (!this.Dispatcher.CheckAccess()) {
                this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new BaseEventHandler((s, e2) => {
                    OnProfileChanged(sender, e2);
                }), sender, e);
                return;
            }
            NavControl.SelectedIndex = 0;
        }
    }
}