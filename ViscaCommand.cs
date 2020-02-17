using System;
using System.Collections.Generic;
using ViscaLib.Data;

namespace ViscaLib
{
    public class ViscaCommand
    {
        private int _cameraNum;
        protected ViscaLimits CameraLimits { get; set; }

        protected int CameraNum
        {
            get => _cameraNum;
            set
            {
                if (value < 0 || value > 8)
                {
                    throw new ArgumentException("Invalid camera number");
                }

                _cameraNum = value;
            }
        }

        public int Status { get; set; }

        protected virtual Byte[] RawSerialData => null;
        public ISource ControlSource { get; set; }

        public ViscaCommand(int cameraNum, ISource source, ViscaLimits limits)
        {
            CameraNum = cameraNum;
            CameraLimits = limits;
            ControlSource = source;
        }

        public virtual CameraResponseModel Execute()
        {
            ControlSource.Send(RawSerialData);
            var response = ControlSource.GetResponse();
            if(response.ResponseType == ViscaCodes.ResponseError)
                throw new ArgumentException($"Request Error {(Byte)response.ResponseType}");
            return response;
        }

        public ViscaCommand(ViscaCommand rhs)
        {
            CameraNum = rhs.CameraNum;
            CameraLimits = rhs.CameraLimits;
            ControlSource = rhs.ControlSource;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ViscaCommand) != obj.GetType())
                return false;
            if (obj is ViscaCommand cmd)
            {
                return ((CameraNum == cmd.CameraNum) && (CameraLimits.Equals(cmd.CameraLimits)));
            }

            return false;
        }

        protected virtual string ToStringDetail()
        {
            return ToString() + " " + "CameraNumber:" + CameraNum;
        }
        public override string ToString()
        {
            return "Generic Command";
        }
        public override int GetHashCode()
        {
            return ToStringDetail().GetHashCode();
        }

        protected void DeepClone(ViscaCommand rhs)
        {
            CameraNum = rhs.CameraNum;
        }
    }

    public class ViscaConnectCommand : ViscaCommand
    {
        protected override byte[] RawSerialData => new byte[] {(ViscaCodes.Header | (1 << 3)) & 0xF8, 0x30, 0x01, ViscaCodes.Terminator};

        public ViscaConnectCommand(ISource source, ViscaLimits limits) : base(0, source, limits)
        {
        }

        public ViscaConnectCommand(ViscaCommand rhs) : base(rhs)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ViscaConnectCommand) != obj.GetType())
                return false;
            if (obj is ViscaConnectCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd));
            }

            return false;
        }

        public override string ToString()
        {
            return "Connect command";
        }
        public override int GetHashCode()
        {
            return ToStringDetail().GetHashCode();
        }

        public override CameraResponseModel Execute()
        {
            var ackn = base.Execute();
            while (ackn.ResponseType == ViscaCodes.ResponseAck)
                ackn = ControlSource.GetResponse();
            if(ackn.ResponseType != ViscaCodes.ResponseAddress)
                throw new ArgumentException($"Request Error {(Byte)ackn.ResponseType}");
            return ackn;
        }
    }

    public class ViscaPanTiltJogCommand:ViscaCommand
    {
        private double _directionRad;

        public double DirectionRad
        {
            get => _directionRad;
            set
            {
                if (value > Math.PI || value < -Math.PI)
                {
                    throw new ArgumentException("Invalid direction for jog of pan/tilt drive");
                }
                _directionRad = value;
            }
        }

        public double DirectionDegree
        {
            get => DirectionRad * (180 / Math.PI);
            set => DirectionRad = value * (Math.PI / 180);

        }

        private int _panTiltSpeed;

        public int PanTiltSpeed
        {
            get => _panTiltSpeed;
            set
            {
                if(value < CameraLimits.HardwareMinPanTiltSpeed || value > CameraLimits.HardwareMaxPanTiltSpeed)
                    throw new ArgumentException("speed must be in range (", CameraLimits.HardwareMinPanTiltSpeed + ", " + CameraLimits.HardwareMaxPanTiltSpeed + ")");
                _panTiltSpeed = value;
            }
        }

        public int TiltSpeed
        {
            get => (int)Math.Round(Math.Sin(DirectionRad) * PanTiltSpeed);
            set
            {
                try
                {
                    double newDirection = Math.Atan2(value, PanSpeed);
                    int newPanTiltSpeed = (int)Math.Round(Math.Pow(PanSpeed, 2) + Math.Pow(value, 2));
                    PanTiltSpeed = newPanTiltSpeed;
                    DirectionRad = newDirection;

                }
                catch
                {
                    throw new ArgumentException("Invalid speed for pan/tilt drive");
                }
            }
        }

        public int PanSpeed
        {
            get => (int)Math.Round(Math.Cos(DirectionRad) * PanTiltSpeed);
            set
            {
                try
                {
                    double newDirection = Math.Atan2(TiltSpeed, value);
                    int newPanTiltSpeed = (int)Math.Round(Math.Sqrt(Math.Pow(value, 2) + Math.Pow(TiltSpeed, 2)));
                    PanTiltSpeed = newPanTiltSpeed;
                    DirectionRad = newDirection;
                }
                catch
                {
                    throw new ArgumentException("Invalid speed for pan/tilt drive");
                }
            }
        }

        protected override byte[] RawSerialData
        {
            get
            {
                byte[] command = new byte[9];
                command[0] = ViscaCodes.Header;
                command[0] |= (byte)CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryPanTilter;
                command[3] = ViscaCodes.PtDrive;
                if (PanSpeed != 0)
                {
                    command[4] = (Byte) (Math.Abs(PanSpeed));
                }
                else
                {
                    command[4] = (Byte) (Math.Abs(PanTiltSpeed));
                }

                if (TiltSpeed != 0)
                {
                    command[5] = (Byte) (Math.Abs(TiltSpeed));
                }
                else
                {
                    command[5] = (Byte) (Math.Abs(PanTiltSpeed));
                }

                if (TiltSpeed > 0)
                {
                    command[6] = ViscaCodes.PtDriveHorizRight;
                }
                else if(TiltSpeed < 0)
                {
                    command[6] = ViscaCodes.PtDriveHorizLeft;
                }
                else
                {
                    command[6] = ViscaCodes.PtDriveHorizStop;
                }

                if (PanSpeed > 0)
                {
                    command[7] = ViscaCodes.PtDriveVertUp;
                } 
                else if (PanSpeed < 0)
                {
                    command[7] = ViscaCodes.PtDriveVertDown;
                }
                else
                {
                    command[7] = ViscaCodes.PtDriveVertStop;
                }

                command[8] = ViscaCodes.Terminator;
                return command;

            }
        }
        

        public ViscaPanTiltJogCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaPanTiltJogCommand(ViscaPanTiltJogCommand rhs) : base(rhs)
        {
            DirectionRad = rhs.DirectionRad;
            PanTiltSpeed = rhs.PanTiltSpeed;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ViscaPanTiltJogCommand) != obj.GetType())
                return false;
            if (obj is ViscaPanTiltJogCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd)) && (Math.Abs(DirectionRad - cmd.DirectionRad) < 1e-6) && PanTiltSpeed == cmd.PanTiltSpeed;
            }

            return false;
        }
        public void DeepClone(ViscaPanTiltJogCommand rhs)
        {
            base.DeepClone(rhs);
            DirectionRad = rhs.DirectionRad;
            PanTiltSpeed = rhs.PanTiltSpeed;
        }
        public override string ToString()

        {

            return "PAN/TILT JOG";

        }
        protected override string ToStringDetail()

        {

            return base.ToStringDetail() + " " + "Direction(Degrees):" + DirectionDegree + " " + "Pan/TiltSpeed:" + PanTiltSpeed;

        }
        public override int GetHashCode()
        {
            return ToStringDetail().GetHashCode();
        }
    }

    public class ViscaPanTiltStopJogCommand : ViscaCommand
    {
        protected override byte[] RawSerialData
        {
            get
            {
                byte[] command = new byte[9];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte) CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryPanTilter;
                command[3] = ViscaCodes.PtDrive;
                command[4] = 0;
                command[5] = 0;
                command[6] = ViscaCodes.PtDriveHorizStop;
                command[7] = ViscaCodes.PtDriveVertStop;
                command[8] = ViscaCodes.Terminator;
                return command;
            }
        }
        public ViscaPanTiltStopJogCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaPanTiltStopJogCommand(ViscaCommand rhs) : base(rhs)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ViscaPanTiltStopJogCommand) != obj.GetType())
                return false;
            if (obj is ViscaPanTiltStopJogCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd));
            }

            return false;


        }
        public override string ToString()
        {
            return "PAN/TILT STOP JOG";
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
    public class ViscaPanTiltAbsoluteCommand: ViscaCommand
    {
        private AngularPosition _panPosition;

        public AngularPosition PanPosition
        {
            get => _panPosition;
            set
            {
                if (value.EncoderCount >= CameraLimits.HardwareMinPanAngle.EncoderCount &&
                    value.EncoderCount <= CameraLimits.HardwareMaxPanAngle.EncoderCount)
                {
                    _panPosition = new AngularPosition(value);
                }
                else
                {
                    throw new ArgumentException("Invalid angle for pan drive");
                }

            }
        }

        private AngularPosition _tiltPosition;

        public AngularPosition TiltPosition
        {
            get => _tiltPosition;
            set
            {
                if (value.EncoderCount >= CameraLimits.HardwareMinTiltAngle.EncoderCount &&
                    value.EncoderCount <= CameraLimits.HardwareMaxTiltAngle.EncoderCount)
                {
                    _tiltPosition = new AngularPosition(value);
                }
                else
                {
                    throw new ArgumentException("Invalid angle for pan drive");
                }

            }
        }

        private int _panTiltSpeed;

        public int PanTiltSpeed
        {
            get => _panTiltSpeed;
            set
            {
                if (value >= CameraLimits.HardwareMinPanTiltSpeed && value <= CameraLimits.HardwareMaxPanTiltSpeed)
                {
                    _panTiltSpeed = value;
                }
                else
                {
                    throw new ArgumentException("Invalid speed for pan/tilt drive");  
                }
            }

        }

        protected override byte[] RawSerialData
        {
            get
            {
                Byte[] command = new byte[15];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte) CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryPanTilter;
                command[3] = ViscaCodes.PtAbsolutePosition;
                command[4] = (Byte) PanTiltSpeed;
                command[5] = (Byte) PanTiltSpeed;
                command[6] = (byte)((PanPosition.EncoderCount & 0xF000) >> 12);
                command[7] = (byte)((PanPosition.EncoderCount & 0x0F00) >> 8);
                command[8] = (byte)((PanPosition.EncoderCount & 0x00F0) >> 4);
                command[9] = (byte)(PanPosition.EncoderCount & 0x000F);
                command[10] = (byte)((TiltPosition.EncoderCount & 0xF000) >> 12);
                command[11] = (byte)((TiltPosition.EncoderCount & 0x0F00) >> 8);
                command[12] = (byte)((TiltPosition.EncoderCount & 0x00F0) >> 4);
                command[13] = (byte)(TiltPosition.EncoderCount & 0x000F);
                command[14] = ViscaCodes.Terminator;
                return command;

            }
        }

        public ViscaPanTiltAbsoluteCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
            PanPosition = new AngularPosition();
            TiltPosition = new AngularPosition();
        }

        public ViscaPanTiltAbsoluteCommand(ViscaPanTiltAbsoluteCommand rhs) : base(rhs)
        {
            PanTiltSpeed = rhs.PanTiltSpeed;
            PanPosition = rhs.PanPosition;
            TiltPosition = rhs.TiltPosition;
        }
        public override  bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (typeof(ViscaPanTiltAbsoluteCommand) != obj.GetType())
                return false;
            if (obj is ViscaPanTiltAbsoluteCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd)) && PanPosition.Equals(cmd.PanPosition) &&
                       TiltPosition.Equals(cmd.TiltPosition);
            }

            return false;
        }

        public void DeepClone(ViscaPanTiltAbsoluteCommand rhs)
        {
            base.DeepClone(rhs);
            PanTiltSpeed = rhs.PanTiltSpeed;
            PanPosition = new AngularPosition(rhs.PanPosition);
            TiltPosition = new AngularPosition(rhs.TiltPosition);
        }
        public override string ToString()
        {
            return "PAN/TILT ABSOLUTE";
        }

        protected override string ToStringDetail()
        {
            return base.ToStringDetail() + " " + "PanPosition(Degrees):" + PanPosition.Degrees + " " + "TiltPosition(Degrees):" + TiltPosition.Degrees + " " + "Pan/TiltSpeed:" + PanTiltSpeed;
        }
        public override int GetHashCode()
        {
            return ToStringDetail().GetHashCode();
        }
    }
    public class ViscaPanTiltInquiryCommand:ViscaCommand
    {
        protected override byte[] RawSerialData
        {
            get
            {
                byte[] command = new byte[5];
                command[0] = ViscaCodes.Header;
                command[0] |= (byte) CameraNum;
                command[1] = ViscaCodes.Inquiry;
                command[2] = ViscaCodes.CategoryPanTilter;
                command[3] = ViscaCodes.PtPositionInq;
                command[4] = ViscaCodes.Terminator;
                return command;
            }
        }

        private void CheckCorrectness(CameraResponseModel response)
        {
            if (response.ResponseType != ViscaCodes.ResponseCompleted)
            {
                if (response.ResponseType == ViscaCodes.ResponseError)
                    throw new ArgumentException("Syntax Error");
                if (response.ResponseType == (ViscaCodes.ResponseError | 1))
                    throw new ArgumentException("Command is not executable");
                if(response.ResponseType == ViscaCodes.ResponseTimeout)
                    throw new ArgumentException("Repeat Request");
            }
            if (response.Response.Count != 11)
            {
                throw new ArgumentException("another command");
            }

        }

        public override CameraResponseModel Execute()
        {
            var response = base.Execute();
            CheckCorrectness(response);
            short tempPanEncoder = (short)((response.Response[2] << 12) | (response.Response[3] << 8) | (response.Response[4] << 4) | (response.Response[5]));
            short tempTiltEncoder = (short)((response.Response[6] << 12) | (response.Response[7] << 8) | (response.Response[8] << 4) | (response.Response[9]));
            return new CameraResponseModel
            {
                ResponseType = ViscaCodes.ResponseCompleted, Response = new List<int> {tempPanEncoder, tempTiltEncoder}
            };
        }


        public ViscaPanTiltInquiryCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaPanTiltInquiryCommand(ViscaCommand rhs) : base(rhs)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            // Check if types match, ensures symmetry
            if (typeof(ViscaPanTiltInquiryCommand) != obj.GetType())
                return false;
            if (obj is ViscaPanTiltInquiryCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd));
            }

            return false;
        }
        public override string ToString()
        {
            return "PAN/TILT INQUIRY";
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public class ViscaPanTiltMaxSpeedInquiryCommand : ViscaCommand
    {
        protected override byte[] RawSerialData
        {
            get
            { 
                Byte[] command = new Byte[5];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte)CameraNum;
                command[1] = ViscaCodes.Inquiry;
                command[2] = ViscaCodes.CategoryPanTilter;
                command[3] = ViscaCodes.PtMaxspeedInq;
                command[4] = ViscaCodes.Terminator;
                return command;

            }
        }

        public ViscaPanTiltMaxSpeedInquiryCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaPanTiltMaxSpeedInquiryCommand(ViscaCommand rhs) : base(rhs)
        {
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            // Check if types match, ensures symmetry
            if (typeof(ViscaPanTiltInquiryCommand) != obj.GetType())
                return false;
            if (obj is ViscaPanTiltMaxSpeedInquiryCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd));
            }

            return false;
        }
        public override string ToString()
        {
            return "PAN/TILT MAX SPEED INQUIRY";
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public class ViscaZoomJogCommand : ViscaCommand
    {
        public ZoomDirection Direction { get; set; }
        private int _zoomSpeed;

        public int ZoomSpeed
        {
            get => _zoomSpeed;
            set
            {
                if (value >= CameraLimits.HardwareMinZoomSpeed && value <= CameraLimits.HardwareMaxZoomSpeed)
                    _zoomSpeed = value;
                else
                {
                    throw new ArgumentException("Invalid Speed for zoom");
                }
            }
        }

        protected override byte[] RawSerialData
        {
            get
            {
                if (Direction == ZoomDirection.None)
                    throw new ArgumentException("Zoom must have a direction");
                byte[] command = new byte[6];
                command[0] = ViscaCodes.Header;
                command[0] |= (byte) CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryCamera1;
                command[3] = ViscaCodes.Zoom;
                if (Direction == ZoomDirection.In)
                {
                    command[4] = (byte) (ViscaCodes.ZoomTeleSpeed | ZoomSpeed);
                }
                else
                {
                    command[4] = (byte) (ViscaCodes.ZoomWideSpeed | ZoomSpeed);
                }

                command[5] = ViscaCodes.Terminator;
                return command;
            }
        }

        public ViscaZoomJogCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
            ZoomSpeed = CameraLimits.DefaultZoomSpeed;
            Direction = ZoomDirection.None;
        }

        public ViscaZoomJogCommand(ViscaZoomJogCommand rhs) : base(rhs)
        {
            ZoomSpeed = rhs.ZoomSpeed;
            Direction = rhs.Direction;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            // Check if types match, ensures symmetry
            if (typeof(ViscaZoomJogCommand) != obj.GetType())
                return false;
            if (obj is ViscaZoomJogCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd)) && ZoomSpeed == cmd.ZoomSpeed && Direction == cmd.Direction;
            }

            return false;
        }
        public void DeepClone(ViscaZoomJogCommand rhs)
        {
            base.DeepClone(rhs);
            Direction = rhs.Direction;
            ZoomSpeed = rhs.ZoomSpeed;
        }

        public override string ToString()
        {
            return "ZOOM JOG";
        }

        protected override string ToStringDetail()
        {
            string d;
            if (Direction == ZoomDirection.In)
                d = "IN";
            else if (Direction == ZoomDirection.Out)
                d = "OUT";
            else
                d = "NONE";
            return base.ToStringDetail() + " " + "ZoomDirection:" + d + " " + "ZoomSpeed:" + ZoomSpeed;

        }
        public override int GetHashCode()
        {
            return ToStringDetail().GetHashCode();
        }
    }

    public class ViscaZoomAbsoluteCommand : ViscaCommand
    {
        private ZoomPosition _zoomPos;
        public ZoomPosition ZoomPosition
        {
            get => _zoomPos;
            set
            {
                if (value.EncoderCount < CameraLimits.MinZoomRatio.EncoderCount || value.EncoderCount >= CameraLimits.MaxZoomRatio.EncoderCount)
                    throw new ArgumentException("Invalid zoom Ratio for zoom drive");
                _zoomPos = value;
            }
        }

        protected override byte[] RawSerialData
        {
            get
            {
                Byte[] command = new Byte[9];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte)CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryCamera1;
                command[3] = ViscaCodes.ZoomValue;
                command[4] = (byte)((ZoomPosition.EncoderCount & 0xF000) >> 12);
                command[5] = (byte)((ZoomPosition.EncoderCount & 0x0F00) >> 8);
                command[6] = (byte)((ZoomPosition.EncoderCount & 0x00F0) >> 4);
                command[7] = (byte)((ZoomPosition.EncoderCount & 0x000F));
                command[8] = ViscaCodes.Terminator;
                return command;
            }
        }

        public ViscaZoomAbsoluteCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
            ZoomPosition = new ZoomPosition(limits.ZoomValues) {EncoderCount = 1};
        }

        public ViscaZoomAbsoluteCommand(ViscaZoomAbsoluteCommand rhs) : base(rhs)
        {
            ZoomPosition = rhs.ZoomPosition;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            // Check if types match, ensures symmetry
            if (typeof(ViscaZoomAbsoluteCommand) != obj.GetType())
                return false;
            if (obj is ViscaZoomAbsoluteCommand cmd)
            {
                return base.Equals(new ViscaCommand(cmd)) && ZoomPosition.Equals(cmd.ZoomPosition);
            }

            return false;
        }
        public void DeepClone(ViscaZoomAbsoluteCommand rhs)
        {
            base.DeepClone(rhs);
            ZoomPosition = new ZoomPosition(rhs.ZoomPosition);
        }
        public override string ToString()
        {
            return "ZOOM ABSOLUTE";
        }
        protected override string ToStringDetail()
        {
            return base.ToStringDetail() + " " + "ZoomPosition(Ratio):" + ZoomPosition.Ratio;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class ViscaZoomStopJogCommand : ViscaCommand
    {
        protected override byte[] RawSerialData
        {
            get
            {
                Byte[] command = new Byte[6];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte)CameraNum;
                command[1] = ViscaCodes.Command;
                command[2] = ViscaCodes.CategoryCamera1;
                command[3] = ViscaCodes.Zoom;
                command[4] = ViscaCodes.ZoomStop;
                command[5] = ViscaCodes.Terminator;
                return command;
            }
        }

        public ViscaZoomStopJogCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaZoomStopJogCommand(ViscaCommand rhs) : base(rhs)
        {
        }

    }
    public class ViscaZoomInquiryCommand : ViscaCommand
    {
        protected override byte[] RawSerialData
        {
            get
            {
                Byte[] command = new Byte[5];
                command[0] = ViscaCodes.Header;
                command[0] |= (Byte)CameraNum;
                command[1] = ViscaCodes.Inquiry;
                command[2] = ViscaCodes.CategoryCamera1;
                command[3] = ViscaCodes.ZoomValue;
                command[4] = ViscaCodes.Terminator;
                return command;
            }
        }

        public override CameraResponseModel Execute()
        {
            var response = base.Execute();
            while (response.Response.Count != 7)
                response = base.Execute();
            short zoomState = (short)((response.Response[2] << 12) | (response.Response[3] << 8) | (response.Response[4] << 4) | (response.Response[5]));
            return new CameraResponseModel { ResponseType = zoomState };
        }

        public ViscaZoomInquiryCommand(int cameraNum, ISource source, ViscaLimits limits) : base(cameraNum, source, limits)
        {
        }

        public ViscaZoomInquiryCommand(ViscaCommand rhs) : base(rhs)
        {
        }
    }
}
