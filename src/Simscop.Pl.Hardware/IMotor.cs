using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Hardware
{
    public interface IMotor
    {
        public double XYSpeed { get; }
        public double ZSpeed { get; }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public bool InitMotor(out string connectState);

        public bool UnInitializeMotor();

        public bool SetXPosition(double xPosition);

        public bool SetYPosition(double yPosition);

        public bool SetZPosition(double zPosition);

        public bool SetXOffset(double x);

        public bool SetYOffset(double y);

        public bool SetZOffset(double z);

        void ReadPosition();

        void ResetPosition();

        bool Stop();

    }
}
