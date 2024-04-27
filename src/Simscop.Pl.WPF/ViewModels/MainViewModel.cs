using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Simscop.Pl.Core;

namespace Simscop.Pl.WPF.ViewModels;


public partial class MainViewModel : ObservableObject
{
    public LineChartViewModel LineChartViewModel = VmManager.LineChartViewModel;


}