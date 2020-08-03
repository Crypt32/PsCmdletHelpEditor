using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.Tools;
using PsCmdletHelpEditor.XmlRpc;
using SysadminsLV.WPF.OfficeTheme.Toolkit;
using SysadminsLV.WPF.OfficeTheme.Toolkit.Commands;

namespace CmdletHelpEditor.API.ViewModels {
    class OnlinePublishProgressVM : AsyncViewModel {
        Boolean stopRequested;
        String publishCaption;
        ModuleObject module;
        Double pbValue;
        OnlinePublishEntry selectedEntry;

        public OnlinePublishProgressVM() {
            Cmdlets = new ObservableCollection<OnlinePublishEntry>();
            PublishCommand = new AsyncCommand(publish);
            RetryCommand = new AsyncCommand(retry);
            PublishCaption = "Publish";
        }

        public IAsyncCommand PublishCommand { get; set; }
        public ICommand RetryCommand { get; set; }

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

        public String PublishCaption {
            get => publishCaption;
            set {
                publishCaption = value;
                OnPropertyChanged(nameof(PublishCaption));
            }
        }
        async Task publish(IList<CmdletObject> cmdlets) {

        }
        async Task retry(Object o, CancellationToken token) {
            if (IsBusy) {
                return;
            }
            IScrollToView lv = (IScrollToView)o;
            PbValue = 0;
            WpXmlRpcClient blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            Double duration = 100.0 / Cmdlets.Count;
            PbValue = 0;
            IsBusy = true;
            PublishCaption = "Stop";
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                if (stopRequested) {
                    break;
                }
                if (cmdlet.Status != OnlinePublishStatusEnum.Failed) {
                    PbValue += duration;
                    continue;
                }
                lv.ScrollIntoView(cmdlet);
                if (cmdlet.Cmdlet.Publish) {
                    try {
                        await MetaWeblogWrapper.PublishSingle(cmdlet.Cmdlet, module, blogger);
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
            PbValue = 100;
            IsBusy = false;
            PublishCaption = "Publish";
        }
        async Task publish(Object o, CancellationToken token) {
            IScrollToView lv = (IScrollToView)o;
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                cmdlet.Status = OnlinePublishStatusEnum.Pending;
                cmdlet.StatusText = "Pending for publish";
            }
            PbValue = 0;
            WpXmlRpcClient blogger = Utils.InitializeBlogger(module.Provider);
            if (blogger == null) {
                MsgBox.Show("Warning", Strings.WarnBloggerNeedsMoreData, MessageBoxImage.Exclamation);
                return;
            }
            Double duration = 100.0 / Cmdlets.Count;
            PbValue = 0;
            IsBusy = true;
            PublishCaption = "Stop";
            foreach (OnlinePublishEntry cmdlet in Cmdlets) {
                if (stopRequested) {
                    break;
                }
                lv.ScrollIntoView(cmdlet);
                if (cmdlet.Cmdlet.Publish) {
                    try {
                        await MetaWeblogWrapper.PublishSingle(cmdlet.Cmdlet, module, blogger);
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
            PbValue = 100;
            IsBusy = false;
            PublishCaption = "Publish";
        }

        public void SetModule(ModuleObject moduleObject) {
            module = moduleObject;
            foreach (CmdletObject cmdlet in module.Cmdlets) {
                Cmdlets.Add(new OnlinePublishEntry { Cmdlet = cmdlet });
            }
        }
    }
}
