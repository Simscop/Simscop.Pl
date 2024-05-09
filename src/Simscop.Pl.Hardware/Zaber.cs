using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Simscop.Pl.Core.Services;
using Zaber.Motion.Ascii;
using Zaber.Motion.Exceptions;

namespace Simscop.Pl.Hardware;

public class Zaber : IMotorService
{
    public const string XCom = "COM6";

    public const string YCom = "COM4";

    public const string ZCom = "COM5";

    private Axis? _xAxis;

    private Axis? _yAxis;

    private Axis? _zAxis;

    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public string? Fireware { get; set; }
    public string? HardwareVersion { get; set; }
    public Dictionary<string, string>? Reserved { get; set; }



    public bool Valid()
    {
        return true;
    }

    public bool Initialize()
    {
        _xAxis = GetAxis(XCom);
        _yAxis = GetAxis(YCom);
        _zAxis = GetAxis(ZCom);

        _xAxis.Home();
        _yAxis.Home();
        _zAxis.Home();

        return _xAxis.IsHomed() && _yAxis.IsHomed() && _zAxis.IsHomed();
    }

    Axis GetAxis(string com)
    {
        var connection = Connection.OpenSerialPort(com);

        connection.EnableAlerts();
        var devices = connection.DetectDevices();
        if (devices.Length != 1) throw new Exception();

        return devices[0].GetAxis(1);

    }

    public bool DeInitialize()
    {
        return true;
    }

    public string? LastErrorMessage { get; }
    public string Unit { get; }
    public (double X, double Y, double Z) Xyz => (X, Y, Z);
    public double X => _xAxis?.GetPosition() ?? double.NaN;
    public double Y => _yAxis?.GetPosition() ?? double.NaN;
    public double Z => _zAxis?.GetPosition() ?? double.NaN;

    public void SetRelativePosition(bool[] index, double[] pos)
    {

    }

    public double Threshold { get; set; } = 1;

    public double RepeatCount { get; set; } = 50;

    public int IntervalTime { get; set; } = 100;

    public Task AsyncSetRelativePosition(bool[] index, double[] pos) => Task.Run(() =>
    {
        // ReSharper disable once ConvertToLocalFunction
        var funcPos = (int i) => i switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new NotImplementedException()
        };

        // ReSharper disable once ConvertToLocalFunction
        var funcAxis = (int i) => i switch
        {
            0 => _xAxis,
            1 => _yAxis,
            2 => _zAxis,
            _ => throw new NotImplementedException()
        };

        Enumerable.Range(0, 3).ToList().ForEach(item =>
        {
            try
            {
                var axis = funcAxis(item);
                if (!index[item] || axis is null) return;

                var temp = funcPos(item);
                var count = 0;

                axis.MoveRelative(pos[item]);

                while (Math.Abs(temp - funcPos(item)) > Threshold && count < RepeatCount)
                {
                    count++; // 防止卡死
                    temp = funcPos(item);

                    Thread.Sleep(IntervalTime);
                }
            }
            catch (CommandFailedException e)
            {
                WeakReferenceMessenger.Default.Send("超出移动范围", "ToastWarning");
            }
            catch (Exception e)
            {
                WeakReferenceMessenger.Default.Send(e.Message, "ToastError");
            }
        });
    });


    public void SetAbsolutePosition(bool[] index, double[] pos)
    {

    }

    public Task AsyncSetAbsolutePosition(bool[] index, double[] pos) => Task.Run(() =>
    {
        // ReSharper disable once ConvertToLocalFunction
        var funcPos = (int i) => i switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new NotImplementedException()
        };

        // ReSharper disable once ConvertToLocalFunction
        var funcAxis = (int i) => i switch
        {
            0 => _xAxis,
            1 => _yAxis,
            2 => _zAxis,
            _ => throw new NotImplementedException()
        };

        Enumerable.Range(0, 3).ToList().ForEach(item =>
        {
            try
            {
                var axis = funcAxis(item);
                if (!index[item] || axis is null) return;

                var temp = funcPos(item);
                var count = 0;

                axis.MoveAbsolute(pos[item]);

                while (Math.Abs(temp - funcPos(item)) > Threshold && count < RepeatCount)
                {
                    count++; // 防止卡死
                    temp = funcPos(item);

                    Thread.Sleep(IntervalTime);
                }
            }
            catch (CommandFailedException e)
            {
                WeakReferenceMessenger.Default.Send("超出移动范围", "ToastWarning");
            }
            catch (Exception e)
            {
                WeakReferenceMessenger.Default.Send(e.Message, "ToastError");
            }
        });
    });
}