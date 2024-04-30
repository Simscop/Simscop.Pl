using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lift.UI.Controls;
using Lift.UI.Data;

namespace Simscop.Pl.WPF.Managers;



public static class ToastManager
{
    
    public static void Success(string msg, int wait = 1)
        => Growl.Success(new GrowlInfo()
        {
            Message = msg,
            WaitTime = wait,
            ShowInMain = true,
            Type = InfoType.Success,
            ShowCloseButton = false,
            ShowDateTime = false
        });

    public static void Info(string msg, int wait = 1)
        => Growl.Info(new GrowlInfo()
        {
            Message = msg,
            WaitTime = wait,
            ShowInMain = true,
            Type = InfoType.Info,
            ShowCloseButton = false,
            ShowDateTime = false
        });

    public static void Warning(string msg, int wait = 1)
        => Growl.Warning(new GrowlInfo()
        {
            Message = msg,
            WaitTime = wait,
            ShowInMain = true,
            Type = InfoType.Warning,
            ShowCloseButton = false,
            ShowDateTime = false
        });

    /// <summary>
    /// 运行错误，运行结果可能不正确，但是不影响继续运行
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="wait"></param>
    public static void Error(string msg, int wait = 1)
        => MessageBox.Error(msg, "Error");

    /// <summary>
    /// 无法处理的错误，这个时候必须弹窗
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="wait"></param>
    /// <exception cref="NotImplementedException"></exception>
    public static void Fatal(string msg, int wait = 1) => MessageBox.Fatal(msg, "Fatal");

    [Conditional("DEBUG")]
    public static void Debug(string msg) => Warning(msg);

}