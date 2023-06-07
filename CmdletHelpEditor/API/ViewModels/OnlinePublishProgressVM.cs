﻿using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CmdletHelpEditor.API.MetaWeblog;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    class OnlinePublishProgressVM : ViewModelBase {
        ModuleObject module;
        Double pbValue;
        OnlinePublishEntry selectedEntry;

        public OnlinePublishProgressVM() {
            Cmdlets = new ObservableCollection<OnlinePublishEntry>();
            PublishCommand = new RelayCommand(publish);
        }

        public ICommand PublishCommand { get; set; }

        public ObservableCollection<OnlinePublishEntry> Cmdlets { get; set; }

        public OnlinePublishEntry SelectedEntry {
            get => selectedEntry;
            set {
                selectedEntry = value;
                OnPropertyChanged(nameof(SelectedEntry));
            }
        }
        public Double PbValue {
            get => pbValue;
            set {
                pbValue = value;
                OnPropertyChanged(nameof(PbValue));
            }
        }

        async void publish(Object obj) {
            ListView lv = (ListView)obj;
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                cmdlet.Status = OnlinePublishStatusEnum.Pending;
                cmdlet.StatusText = "Pending for publish";
            }
            PbValue = 0;
            Blogger blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                Utils.MsgBox("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            Double duration = 100.0 / Cmdlets.Count;
            PbValue = 0;
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                lv.ScrollIntoView(cmdlet);
                if (cmdlet.Cmdlet.Publish) {
                    try {
                        await MetaWeblogWrapper.PublishSingle(cmdlet.Cmdlet, module, blogger, true);
                        cmdlet.Status = OnlinePublishStatusEnum.Succeed;
                        cmdlet.StatusText = "The operation completed successfully.";
                    } catch (Exception e) {
                        cmdlet.Status = OnlinePublishStatusEnum.Failed;
                        cmdlet.StatusText = e.Message;
                    }
                } else {
                    cmdlet.Status = OnlinePublishStatusEnum.Skipped;
                    cmdlet.StatusText = "The item is not configured for publishing";
                }

                PbValue += duration;
                await Task.Factory.StartNew(() => Thread.Sleep(1000));
            }
            PbValue = 100;
        }

        public void SetModule(ModuleObject moduleObject) {
            module = moduleObject;
            foreach (CmdletObject cmdlet in module.Cmdlets) {
                Cmdlets.Add(new OnlinePublishEntry { Cmdlet = cmdlet });
            }
        }
    }
}
