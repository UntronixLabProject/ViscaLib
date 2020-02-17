using System;

namespace ViscaLib.Data
{
    public class ZoomPosition
    {
        private int _encoderCount;

        public ZoomPosition()
        {
            EncoderCount = 0;
        }

        public ZoomPosition(Tuple<double, short>[] zoomValues)
        {
            ZoomValues = zoomValues;
        }

        public ZoomPosition(ZoomPosition rhs)
        {
            EncoderCount = rhs.EncoderCount;
            ZoomValues = rhs.ZoomValues;
        }

        public Tuple<double, short>[] ZoomValues { get; } = new Tuple<double, short>[] {Tuple.Create(0.0, (short) 0)};

        public int EncoderCount
        {
            get => _encoderCount;
            set
            {
                for (var idx = 0; idx < ZoomValues.Length - 1; ++idx)
                    if (value >= ZoomValues[idx].Item2)
                    {
                        if (value <= ZoomValues[idx + 1].Item2)
                        {
                            if (value == EncoderCount)
                                return;
                            _encoderCount = value;
                            return;
                        }
                    }

                var error = "Zoom encoder count outside expected range";
                throw new ArgumentOutOfRangeException(error);
            }
        }

        public double Ratio
        {
            get
            {
                for (var idx = 0; idx < ZoomValues.Length - 1; ++idx)
                    if (EncoderCount >= ZoomValues[idx].Item2)
                    {
                        if (EncoderCount <= ZoomValues[idx + 1].Item2)
                        {
                            return (ZoomValues[idx + 1].Item1 - ZoomValues[idx].Item1) /
                                (ZoomValues[idx + 1].Item2 - ZoomValues[idx].Item2) *
                                (EncoderCount - ZoomValues[idx].Item2) + ZoomValues[idx].Item1;
                        }
                    }

                return 0;
            }
            set
            {
                for (var idx = 0; idx < ZoomValues.Length - 1; ++idx)
                {
                    var eps = value - ZoomValues[idx].Item1;
                    if (Math.Abs(eps) < 1e-6)
                    {
                        EncoderCount = ZoomValues[idx].Item2;
                        return;
                    }

                    eps = value - ZoomValues[idx + 1].Item1;
                    if (Math.Abs(eps) < 1e-6)
                    {
                        EncoderCount = ZoomValues[idx + 1].Item2;
                        return;
                    }

                    if (value > ZoomValues[idx].Item1)
                    {
                        if (value < ZoomValues[idx + 1].Item1)
                        {
                            var diffBetweenEncoders = ZoomValues[idx + 1].Item2 - ZoomValues[idx].Item2;
                            var diffBetweenZoomRatios = ZoomValues[idx + 1].Item1 - ZoomValues[idx].Item1;
                            var coeff = value - ZoomValues[idx].Item1;
                            var multiple = diffBetweenEncoders / diffBetweenZoomRatios * coeff;
                            EncoderCount =
                                (short) Math.Round(multiple + ZoomValues[idx].Item2);
                            return;
                        }
                    }
                }

                var error = "Zoom Ratio outside expected range";
                throw new ArgumentOutOfRangeException(error);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ZoomPosition) != obj.GetType())
                return false;
            if (obj is ZoomPosition pos) return EncoderCount == pos.EncoderCount;

            return false;
        }

        public override string ToString()
        {
            return $"ZoomPosition(Ratio = {Ratio},EncoderCount = {EncoderCount})";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}