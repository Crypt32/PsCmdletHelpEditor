using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using PsCmdletHelpEditor.BLL.Abstraction;
using PsCmdletHelpEditor.BLL.Abstraction.Controls;
using PsCmdletHelpEditor.BLL.Models;
using PsCmdletHelpEditor.BLL.Tools;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace PsCmdletHelpEditor.BLL.ViewModels {
    public class OnlinePublishProgressVM : ClosableDialogViewModel, IOnlinePublishProgressVM {
        const String StartTitle = "Publish";
        const String StopTitle = "Stop";
        PsModuleObject module;
        Double pbValue;
        OnlinePublishEntry selectedEntry;
        String buttonTitle;
        Boolean isRunning, stopRequested;

        public OnlinePublishProgressVM() {
            buttonTitle = StartTitle;
            PublishCommand = new RelayCommand(publish);
        }

        public ICommand PublishCommand { get; }

        public ObservableCollection<OnlinePublishEntry> Cmdlets { get; }
            = new ObservableCollection<OnlinePublishEntry>();

        public String ButtonTitle {
            get => buttonTitle;
            set {
                buttonTitle = value;
                OnPropertyChanged(nameof(ButtonTitle));
            }
        }

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
            if (isRunning) {
                stopRequested = true;
                return;
            }
            if (!(obj is IScrollableListView lv)) {
                MsgBox.Show("Invalid Parameter", "The supplied parameter is incorrect.");
                return;
            }
            ButtonTitle = StopTitle;
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                cmdlet.Status = OnlinePublishStatusEnum.Pending;
                cmdlet.StatusText = "Pending for publish";
            }
            PbValue = 0;
            var blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            Double duration = 100.0 / Cmdlets.Count;
            PbValue = 0;
            if (!isRunning) {
                isRunning = true;
            }
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                if (stopRequested) {
                    break;
                }
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
            }
            isRunning = false;
            stopRequested = false;
            ButtonTitle = StartTitle;
        }

        public void SetModule(PsModuleObject moduleObject) {
            module = moduleObject;
            foreach (CmdletObject cmdlet in module.Cmdlets) {
                Cmdlets.Add(new OnlinePublishEntry { Cmdlet = cmdlet });
            }
        }
    }
}
