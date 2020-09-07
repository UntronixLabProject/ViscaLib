using ViscaLib.Data;

namespace ViscaLib
{
    public abstract class ViscaCamera
    {
        public ISource Source { get; set; }
        public ViscaLimits Limits { get; set; }
        public bool HardwareConnected { get; private set; }
        private int _cameraNum;

        protected ViscaCamera(ISource source,ViscaLimits limits)
        {
            Source = source;
            Limits = limits;
        }
        protected ViscaCamera(ViscaLimits limits)
        {
            Limits = limits;
        }

        protected ViscaCamera()
        {
        }

        public bool Connect(bool needSetup = true)
        {
            try
            {
                Source.Connect();
                if(needSetup)
                {
                    ViscaConnectCommand connectCommand = new ViscaConnectCommand(Source,Limits);
                    var cameraAnswer = connectCommand.Execute();
                    if (cameraAnswer.ResponseType == ViscaCodes.ResponseError)
                    {
                        return false;
                    }

                    _cameraNum = cameraAnswer.Response[2] - 1;
                }
                else
                {
                    _cameraNum = 1;
                }
                HardwareConnected = true;
                return HardwareConnected;
            } 
            catch
            {
                return false;
            }
        }
        public int PanTiltMove(double degrees,int? speed = null)
        {
            ViscaPanTiltJogCommand jogCommand = new ViscaPanTiltJogCommand(_cameraNum, Source, Limits)
            {
                PanTiltSpeed = speed ?? Limits.DefaultPanTiltSpeed, DirectionDegree = degrees
            };
            var cameraAnswer = jogCommand.Execute();
            return cameraAnswer.ResponseType;
        }

        public int PanTiltAbsolute(double panDegrees, double tiltDegrees,int speed)
        {
            ViscaPanTiltAbsoluteCommand absCommand = new ViscaPanTiltAbsoluteCommand(_cameraNum, Source, Limits)
            {
                PanPosition = new AngularPosition(Limits.PanDegreesPerEncoderCount) {Degrees = panDegrees},
                TiltPosition = new AngularPosition(Limits.TiltDegreesPerEncoderCount) {Degrees = tiltDegrees},
                PanTiltSpeed = speed
            };
            var cameraAnswer = absCommand.Execute();
            return cameraAnswer.ResponseType;
        }
        public int PanTiltMoveAbsolute(int panEncoder, int tiltEncoder, int speed = 10)
        {
            ViscaPanTiltAbsoluteCommand absCommand = new ViscaPanTiltAbsoluteCommand(_cameraNum, Source, Limits)
            {
                PanPosition = new AngularPosition(Limits.PanDegreesPerEncoderCount) {EncoderCount = panEncoder},
                TiltPosition = new AngularPosition(Limits.TiltDegreesPerEncoderCount) {EncoderCount = tiltEncoder},
                PanTiltSpeed = speed
            };
            var cameraAnswer = absCommand.Execute();
            return cameraAnswer.ResponseType;
        }
        public int PanTiltStop()
        {
            ViscaPanTiltStopJogCommand stopCommand = new ViscaPanTiltStopJogCommand(_cameraNum, Source, Limits);
            var response = stopCommand.Execute();
            return response.ResponseType;
        }
        public int ZoomJog(int? speed,ZoomDirection direction = ZoomDirection.Out)
        {
            ViscaZoomJogCommand zoomCommand = new ViscaZoomJogCommand(_cameraNum, Source, Limits)
            {
                Direction = direction, ZoomSpeed = speed ?? Limits.DefaultZoomSpeed
            };
            var response = zoomCommand.Execute();
            return response.ResponseType;
        }
        public int ZoomAbsolute(double ratio)
        {
            ViscaZoomAbsoluteCommand zoomAbsCommand = new ViscaZoomAbsoluteCommand(_cameraNum, Source, Limits)
            {
                ZoomPosition = new ZoomPosition(Limits.ZoomValues) {Ratio = ratio}
            };
            var response = zoomAbsCommand.Execute();
            return response.ResponseType;
        }
        public double ZoomInquiry()
        {
            ViscaZoomInquiryCommand viscaZoomInquiry = new ViscaZoomInquiryCommand(_cameraNum, Source, Limits);
            var cameraResponse = viscaZoomInquiry.Execute();
            var zoomRatio = new ZoomPosition(Limits.ZoomValues) {EncoderCount = cameraResponse.ResponseType};
            return zoomRatio.Ratio;
        }

        public PositionCoordinates PanTiltInquiry()
        {
            ViscaPanTiltInquiryCommand viscaPanTilt = new ViscaPanTiltInquiryCommand(_cameraNum, Source, Limits);
            var cameraResponse = viscaPanTilt.Execute();
            return new PositionCoordinates { PanPosition = new AngularPosition { EncoderCount = (short)(cameraResponse.Response[0]) }, TiltPosition = new AngularPosition { EncoderCount = (short)(cameraResponse.Response[1]) } };
        }
        public int ZoomStop()
        {
            ViscaZoomStopJogCommand zoomStop = new ViscaZoomStopJogCommand(_cameraNum, Source, Limits);
            var response = zoomStop.Execute();
            return response.ResponseType;
        }
        public override string ToString()
        {
            return $"Visca Camera {_cameraNum}";
        }
    }

    public class ScopiaFlexCamera : ViscaCamera
    {
        public string Name { get; set; }

        public ScopiaFlexCamera(ISource source, ViscaLimits limits,string name) : base(source, limits)
        {
            Name = name;
        }

        public ScopiaFlexCamera(ISource source, string name)
        {
            Source = source;
            Name = name;
            Limits = new ScopiaFlexLimits();
        }
        public override string ToString()
        {
            return Name;
        }
    }

    public class BirdogP100 : ViscaCamera
    {
        public string Name { get; set; }
        public BirdogP100(ISource source,string name)
        {
            Source = source;
            Name = name;
            Limits = new BirdogP100Limits();
        }
        public BirdogP100(ISource source,ViscaLimits limits,string name): base(source,limits)
        {
            Name = name;
        }
        public override string ToString()
        {
            return Name;
        }

    }

}
