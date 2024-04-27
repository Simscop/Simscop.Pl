using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using OpenCvSharp;

namespace Simscop.Pl.WPF.Managers;



#region Camera

public class CaptureRequestMessage : RequestMessage<Mat?> { }



#endregion