using System;
using PsCmdletHelpEditor.Core;

namespace CmdletHelpEditor.Abstract;

public interface IProgressBar : IProgress {
    String ProgressText { get; set; }
    Boolean IsRunning { get; }

    void Start();
    void Stop();
    void End();
}