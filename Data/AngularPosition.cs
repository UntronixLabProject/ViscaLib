using System;

namespace ViscaLib.Data
{
    public class AngularPosition
    {
        public int EncoderCount { get; set; }
        private double DegreePerEncoderCount { get;}


        public double Radians
        {
            get => EncoderCount * DegreePerEncoderCount * (Math.PI / 180.0);
            set
            {
                var inRadiance = value * (180 / Math.PI);
                var encoderCount = inRadiance / DegreePerEncoderCount;
                EncoderCount = (int) Math.Round(encoderCount);
            }
        }

        public double Degrees
        {
            get => Radians * (180 / Math.PI);
            set => EncoderCount = (int) Math.Round(value / DegreePerEncoderCount);
        }
        public AngularPosition(double degreePerEncoderCount)
        {
            DegreePerEncoderCount = degreePerEncoderCount;
        }

        public AngularPosition()
        {

        }

        public AngularPosition(AngularPosition rhs)
        {
            this.DegreePerEncoderCount = rhs.DegreePerEncoderCount;
            this.EncoderCount = rhs.EncoderCount;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(AngularPosition) != obj.GetType())
                return false;
            if (obj is AngularPosition pos)
            {
                return (pos.EncoderCount == EncoderCount) &&
                       (Math.Abs(pos.DegreePerEncoderCount - DegreePerEncoderCount) < 1e-6);
            }

            return false;
        }

        public override string ToString()
        {
            return $"AngularPosition(degreePerEncoderCount:{DegreePerEncoderCount})";
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }
}
