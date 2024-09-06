//using Zaber.Motion.Ascii;

//while (true)
//{
//    Console.WriteLine();
//    try
//    {
//        await Test();
//    }
//    catch (Exception e)
//    {
//        Console.WriteLine(e);
//    }
//}
//async Task Test()
//{
//    Console.Write("Input the com :");

//    var com = Console.ReadLine();
//    com = com!.Replace(" ", "").Replace("\n", "");

//    var connection = Connection.OpenSerialPort(com);
//    connection.EnableAlerts();
//    var devices = connection.DetectDevices();
//    if (devices.Length != 1)
//    {
//        Console.WriteLine("没有设备");
//        return;
//    }

//    var axis = devices[0].GetAxis(1);

//    Console.WriteLine("找到轴");
//    Console.WriteLine("开始回零");
//    await Task.Run(() =>
//    {
//        axis.Home();
//        Console.WriteLine("回零结果: " + axis.IsHomed());
//    });

//}

using Simscop.Pl.Hardware;

ZaberDevice zaberDevice = new ZaberDevice();
Console.WriteLine(zaberDevice.Initialize());

//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 1000000, 1000000, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 2000000, 2000000, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 3000000, 3000000, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 4000000, 4000000, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 5000000, 5000000, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");
//zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { 0, 0, 0 });
//Console.WriteLine($"x_{zaberDevice.X} y_{zaberDevice.Y} x_{zaberDevice.Z}");

double tran = 2 * 1000000;//微米，步长
int count = 5;//次数
for (int i = 0; i < count; i++)
{
    for (int j = 0; j < count; j++)
    {
        double x = 0;
        double y = 0;
        if (i % 2 == 0)
        {
            x = i;
            y = j;
        }
        else if (i % 2 == 1)
        {
            x = i;
            y = count - j - 1;
        }

        zaberDevice.SetAbsolutePosition(new[] { true, true, true }, new double[] { x * tran, y * tran, 15233078 });
        Console.WriteLine($"{x + 1}_{y + 1}   x_{zaberDevice.X} y_{zaberDevice.Y} z_{zaberDevice.Z}");
    }
}

//ICameraService Camera = new ToupTek();
//Console.WriteLine(Camera.Valid());
//Console.WriteLine(Camera.Initialize());
//Console.WriteLine(Camera.IsAutoExposure=false);
//Console.WriteLine(Camera.Resolution = Camera.Resolutions[2]);
//VmManager.CameraViewModel.FirstInit();
//HardwareManager.Camera!.OnCaptureChanged += img =>
//{
//    //Console.WriteLine(Camera.Capture(out Mat? img));
//    img?.SaveImage($"C:\\Users\\Simsc\\Desktop\\ZZJ\\{DateTime.Now.ToString("hh-mm-ss-fff")}.bmp");
//};






