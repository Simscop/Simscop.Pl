using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zaber.Motion;

namespace Simscop.Pl.Hardware
{
    public interface IRotary
    {
        public double Wheel { get; }
        public double Filter { get; }

        public Units Unit { get; set; }

        public bool InitRotary();

        public Task SetWheelPosition(int wheelPosition);

        public Task SetFilterPosition(int filtePosition);


        public Task MoveXRelative(double x);

        public Task MoveYRelative(double y);

        public Task MoveZRelative(double z);

        public Task MoveXAbsolute(double x);

        public Task MoveYAbsolute(double y);

        public Task MoveZAbsolute(double z);


        void ReadPosition();

        void ResetPosition();

        bool Stop();

    }
}
