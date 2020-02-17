using System;

namespace ViscaLib.Data
{
    public abstract class ViscaLimits
    {
        public abstract int HardwareMinZoomSpeed { get; }
        public abstract int HardwareMaxZoomSpeed { get; }
        public abstract AngularPosition HardwareMinPanAngle { get; }
        public abstract AngularPosition HardwareMaxPanAngle { get; }
        public abstract AngularPosition HardwareMinTiltAngle { get; }
        public abstract AngularPosition HardwareMaxTiltAngle { get; }

        public abstract double PanDegreesPerEncoderCount { get; }

        public abstract double TiltDegreesPerEncoderCount { get; }

        public abstract int DefaultPanTiltSpeed { get; }

        public abstract int HardwareMinPanTiltSpeed { get; }

        public abstract int HardwareMaxPanTiltSpeed { get; }

        public abstract int DefaultZoomSpeed { get; }
        public Tuple<double, short>[] ZoomValues
        {
            get 
            {
                Tuple<double, short>[] zoom = new Tuple<double, short>[29];
                zoom[0] = Tuple.Create(1.0, (short)0);
                zoom[1] = Tuple.Create(2.0, (short)5638);
                zoom[2] = Tuple.Create(3.0, (short)8529);
                zoom[3] = Tuple.Create(4.0, (short)10336);
                zoom[4] = Tuple.Create(5.0, (short)11445);
                zoom[5] = Tuple.Create(6.0, (short)12384);
                zoom[6] = Tuple.Create(7.0, (short)13011);
                zoom[7] = Tuple.Create(8.0, (short)13637);
                zoom[8] = Tuple.Create(9.0, (short)14119);
                zoom[9] = Tuple.Create(10.0, (short)14505);
                zoom[10] = Tuple.Create(11.0, (short)14914);
                zoom[11] = Tuple.Create(12.0, (short)15179);
                zoom[12] = Tuple.Create(13.0, (short)15493);
                zoom[13] = Tuple.Create(14.0, (short)15733);
                zoom[14] = Tuple.Create(15.0, (short)15950);
                zoom[15] = Tuple.Create(16.0, (short)16119);
                zoom[16] = Tuple.Create(17.0, (short)16288);
                zoom[17] = Tuple.Create(18.0, (short)16384);
                zoom[18] = Tuple.Create(36.0, (short)24576);
                zoom[19] = Tuple.Create(54.0, (short)27264);
                zoom[20] = Tuple.Create(72.0, (short)28672);
                zoom[21] = Tuple.Create(90.0, (short)29504);
                zoom[22] = Tuple.Create(108.0, (short)30016);
                zoom[23] = Tuple.Create(126.0, (short)30400);
                zoom[24] = Tuple.Create(144.0, (short)30720);
                zoom[25] = Tuple.Create(162.0, (short)30976);
                zoom[26] = Tuple.Create(180.0, (short)31104);
                zoom[27] = Tuple.Create(198.0, (short)31296);
                zoom[28] = Tuple.Create(216.0, (short)31424);
                return zoom;
            }
        }
        public abstract ZoomPosition MinZoomRatio { get; }
        public abstract ZoomPosition MaxZoomRatio { get; }
    }

    public class ScopiaFlexLimits : ViscaLimits
    {
        public override int HardwareMinZoomSpeed { get; } = 0x00;
        public override int HardwareMaxZoomSpeed { get; } = 0x07;
        public override AngularPosition HardwareMinPanAngle { get; } = new AngularPosition { EncoderCount = -8800 };
        public override AngularPosition HardwareMaxPanAngle { get; } = new AngularPosition { EncoderCount = 8800 };
        public override AngularPosition HardwareMinTiltAngle { get; } = new AngularPosition { EncoderCount = -2560 };
        public override AngularPosition HardwareMaxTiltAngle { get; } = new AngularPosition { EncoderCount = 2560 };

        public override double PanDegreesPerEncoderCount { get; } = 0.075;
        public override double TiltDegreesPerEncoderCount { get; } = 0.075;
        public override int DefaultPanTiltSpeed { get; } = 5;
        public override int HardwareMinPanTiltSpeed { get; } = 1;
        public override int HardwareMaxPanTiltSpeed { get; } = 24;
        public override int DefaultZoomSpeed { get; } = 0x00;
        public override ZoomPosition MinZoomRatio => new ZoomPosition(this.ZoomValues) { EncoderCount = this.ZoomValues[0].Item2 };
        public override ZoomPosition MaxZoomRatio => new ZoomPosition(this.ZoomValues) { EncoderCount = this.ZoomValues[17].Item2 };
    }
}