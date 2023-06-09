using System;

namespace CmdletHelpEditor.Abstract;

public interface IProgressBar {
    Double Progress { get; set; }
    String ProgressText { get; set; }
    Boolean IsRunning { get; }

    void Start();
    void Stop();
    void End();
}