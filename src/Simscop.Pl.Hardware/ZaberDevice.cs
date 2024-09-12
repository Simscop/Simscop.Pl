using CommunityToolkit.Mvvm.Messaging;
using Simscop.Pl.Core.Services;
using System.Diagnostics;
using Zaber.Motion;
using Zaber.Motion.Ascii;
using Zaber.Motion.Exceptions;
using Zaber.Motion.Microscopy;
using Connection = Zaber.Motion.Ascii.Connection;

namespace Simscop.Pl.Hardware
{
    public class ZaberDevice : IMotorService
    {
        public string? Model { get; set; }
        public string? SerialNumber { get; set; }
        public string? Fireware { get; set; }
        public string? HardwareVersion { get; set; }
        public Dictionary<string, string>? Reserved { get; set; }
        public string? LastErrorMessage { get; }

        private string _port = "COM9";
        private Axis? _xAxis;
        private Axis? _yAxis;
        private Axis? _zAxis;
        private Connection? connection;
        private ObjectiveChanger? _objective;
        private FilterChanger? _filter;
        private double Threshold { get; set; } = 1;
        private double RepeatCount { get; set; } = 50;
        private int IntervalTime { get; set; } = 100;

        public string Unit => Units.Length_Micrometres.ToString();
        public (double X, double Y, double Z) Xyz => (X, Y, Z);
        public double X => _xAxis?.GetPosition() ?? double.NaN;
        public double Y => _yAxis?.GetPosition() ?? double.NaN;
        public double Z => _zAxis?.GetPosition() ?? double.NaN;
        public double Wheel => _objective?.GetCurrentObjective() ?? double.NaN;
        public double Filter => _objective?.GetCurrentObjective() ?? double.NaN;
        public double XSpeed { get; private set; }
        public double YSpeed { get; private set; }
        public double ZSpeed { get; private set; }

        public bool DeInitialize()
        {
            throw new NotImplementedException();
        }

        public bool Initialize()
        {
            //return InitMotor() && InitRotary();//todo,正置获取转轮异常
            return InitMotor();
        }

        public Task AsyncSetAbsolutePosition(bool[] index, double[] pos) => Task.Run(() => { SetAbsolutePosition(index, pos); });

        public Task AsyncSetRelativePosition(bool[] index, double[] pos) => Task.Run(() => { SetRelativePosition (index, pos); });

        public void SetAbsolutePosition(bool[] index, double[] pos)
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
                    //axis.MoveAbsolute(pos[item], Units.Length_Millimetres, true, XSpeed, Units.Velocity_MillimetresPerSecond);

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
        }

        public void SetRelativePosition(bool[] index, double[] pos)
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
        }

        public async Task SetWheelPosition(int wheelPosition) => await _objective!.ChangeAsync(wheelPosition);

        public async Task SetFilterPosition(int filtePosition) => await _filter!.ChangeAsync(filtePosition);

        public bool Valid() => true;

        private bool InitMotor()
        {
            try
            {
                connection = Connection.OpenSerialPortAsync(_port).Result;

                connection.EnableAlerts();

                var deviceList = connection.DetectDevices(true);

                Console.WriteLine($"Found {deviceList.Length} devices.");

                if (deviceList.Length < 5) return false;

                //Home
                //Task.Run(() =>
                //{

                //deviceList[3].AllAxes.Home();
                //deviceList[5].AllAxes.Home();


                //});
                //Task.Run(() =>
                //{
                //_xAxis.Home();
                //_yAxis.Home();
                //_zAxis.Home();
                //});

                _zAxis = deviceList[3].GetAxis(1);
                _xAxis = deviceList[5].GetAxis(1);
                _yAxis = deviceList[5].GetAxis(2);

                XSpeed = _xAxis.Settings.Get("maxspeed", Units.Velocity_MillimetresPerSecond);
                YSpeed = _yAxis.Settings.Get("maxspeed", Units.Velocity_MillimetresPerSecond);
                ZSpeed = _zAxis.Settings.Get("maxspeed", Units.Velocity_MillimetresPerSecond);

                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        private bool InitRotary()
        {
            try
            {
                var deviceList = connection?.DetectDevices(true);

                Console.WriteLine($"Found {deviceList?.Length} devices.");

                if (deviceList?.Length < 5) return false;

                var microscope = Microscope.Find(connection);
                _objective = microscope.ObjectiveChanger;
                if (_objective == null) return false;

                _filter = microscope.FilterChanger;

                var num = _objective.GetNumberOfObjectives();

                var objs = _objective.GetCurrentObjective();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool ResetPosition()
        {
            //Task.Run(() =>
            //{
                _xAxis?.Home();
                _yAxis?.Home();
                //_zAxis?.Home();
            //});
            return _xAxis!.IsHomed() && _yAxis!.IsHomed() && _zAxis!.IsHomed();
        }

        public bool Stop()
        {
            //Task.Run(() =>
            //{
            //    _xAxis?.Stop();
            //    _yAxis?.Stop();
            //    _zAxis?.Stop();
            //});
            connection?.StopAll();
            return true;
        }

    }
}
