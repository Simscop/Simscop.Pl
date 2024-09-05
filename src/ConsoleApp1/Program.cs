using Zaber.Motion.Ascii;

while (true)
{
    Console.WriteLine();
    try
    {
        await Test();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}
async Task Test()
{
    Console.Write("Input the com :");

    var com = Console.ReadLine();
    com = com!.Replace(" ", "").Replace("\n", "");

    var connection = Connection.OpenSerialPort(com);
    connection.EnableAlerts();
    var devices = connection.DetectDevices();
    if (devices.Length != 1)
    {
        Console.WriteLine("没有设备");
        return;
    }

    var axis = devices[0].GetAxis(1);

    Console.WriteLine("找到轴");
    Console.WriteLine("开始回零");
    await Task.Run(() =>
    {
        axis.Home();
        Console.WriteLine("回零结果: " + axis.IsHomed());
    });

}