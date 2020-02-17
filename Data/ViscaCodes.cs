using System;

namespace ViscaLib.Data
{
    public static class ViscaCodes
    {
        public const Byte Header = 0x80;
        public const Byte Command = 0x01;
        public const Byte Inquiry = 0x09;
        public const Byte Terminator = 0xFF;

        public const Byte CategoryCamera1 = 0x04;
        public const Byte CategoryPanTilter = 0x06;
        public const Byte CategoryCamera2 = 0x07;

        public const Byte Success = 0x00;
        public const Byte Failure = 0xFF;

        // Response types
        public const byte ResponseClear = 0x40;

        public const byte ResponseAddress = 0x30;
        public const byte ResponseAck = 0x40;
        public const byte ResponseCompleted = 0x50;
        public const byte ResponseError = 0x60;
        public const byte ResponseTimeout = 0x70;  // Not offical, I created this to handle serial port timeouts

        // Commands/inquiries codes
        public const byte Power = 0x00;

        public const byte DeviceInfo = 0x02;
        public const byte Keylock = 0x17;
        public const byte Id = 0x22;
        public const byte Zoom = 0x07;
        public const byte ZoomStop = 0x00;
        public const byte ZoomTele = 0x02;
        public const byte ZoomWide = 0x03;
        public const byte ZoomTeleSpeed = 0x20;
        public const byte ZoomWideSpeed = 0x30;
        public const byte ZoomValue = 0x47;
        public const byte ZoomFocusValue = 0x47;
        public const byte Dzoom = 0x06;
        public const byte Focus = 0x08;
        public const byte FocusStop = 0x00;
        public const byte FocusFar = 0x02;
        public const byte FocusNear = 0x03;
        public const byte FocusFarSpeed = 0x20;
        public const byte FocusNearSpeed = 0x30;
        public const byte FocusValue = 0x48;
        public const byte FocusAuto = 0x38;
        public const byte FocusAutoMan = 0x10;
        public const byte FocusOnePush = 0x18;
        public const byte FocusOnePushTrig = 0x01;
        public const byte FocusOnePushInf = 0x02;
        public const byte FocusAutoSense = 0x58;
        public const byte FocusAutoSenseHigh = 0x02;
        public const byte FocusAutoSenseLow = 0x03;
        public const byte FocusNearLimit = 0x28;
        public const byte Wb = 0x35;
        public const byte WbAuto = 0x00;
        public const byte WbIndoor = 0x01;
        public const byte WbOutdoor = 0x02;
        public const byte WbOnePush = 0x03;
        public const byte WbAtw = 0x04;
        public const byte WbManual = 0x05;
        public const byte WbOnePushTrig = 0x05;
        public const byte Rgain = 0x03;
        public const byte RgainValue = 0x43;
        public const byte Bgain = 0x04;
        public const byte BgainValue = 0x44;
        public const byte AutoExp = 0x39;
        public const byte AutoExpFullAuto = 0x00;
        public const byte AutoExpManual = 0x03;
        public const byte AutoExpShutterPriority = 0x0A;
        public const byte AutoExpIrisPriority = 0x0B;
        public const byte AutoExpGainPriority = 0x0C;
        public const byte AutoExpBright = 0x0D;
        public const byte AutoExpShutterAuto = 0x1A;
        public const byte AutoExpIrisAuto = 0x1B;
        public const byte AutoExpGainAuto = 0x1C;
        public const byte SlowShutter = 0x5A;
        public const byte SlowShutterAuto = 0x02;
        public const byte SlowShutterManual = 0x03;
        public const byte Shutter = 0x0A;
        public const byte ShutterValue = 0x4A;
        public const byte Iris = 0x0B;
        public const byte IrisValue = 0x4B;
        public const byte Gain = 0x0C;
        public const byte GainValue = 0x4C;
        public const byte Bright = 0x0D;
        public const byte BrightValue = 0x4D;
        public const byte ExpComp = 0x0E;
        public const byte ExpCompPower = 0x3E;
        public const byte ExpCompValue = 0x4E;
        public const byte BacklightComp = 0x33;
        public const byte Aperture = 0x02;
        public const byte ApertureValue = 0x42;
        public const byte ZeroLux = 0x01;
        public const byte IrLed = 0x31;
        public const byte WideMode = 0x60;
        public const byte WideModeOff = 0x00;
        public const byte WideModeCinema = 0x01;
        public const byte WideMode169 = 0x02;
        public const byte Mirror = 0x61;
        public const byte Freeze = 0x62;
        public const byte PictureEffect = 0x63;
        public const byte PictureEffectOff = 0x00;
        public const byte PictureEffectPastel = 0x01;
        public const byte PictureEffectNegative = 0x02;
        public const byte PictureEffectSepia = 0x03;
        public const byte PictureEffectBw = 0x04;
        public const byte PictureEffectSolarize = 0x05;
        public const byte PictureEffectMosaic = 0x06;
        public const byte PictureEffectSlim = 0x07;
        public const byte PictureEffectStretch = 0x08;
        public const byte DigitalEffect = 0x64;
        public const byte DigitalEffectOff = 0x00;
        public const byte DigitalEffectStill = 0x01;
        public const byte DigitalEffectFlash = 0x02;
        public const byte DigitalEffectLumi = 0x03;
        public const byte DigitalEffectTrail = 0x04;
        public const byte DigitalEffectLevel = 0x65;
        public const byte Memory = 0x3F;
        public const byte MemoryReset = 0x00;
        public const byte MemorySet = 0x01;
        public const byte MemoryRecall = 0x02;
        public const byte Display = 0x15;
        public const byte DisplayToggle = 0x10;
        public const byte DateTimeSet = 0x70;
        public const byte DateDisplay = 0x71;
        public const byte TimeDisplay = 0x72;
        public const byte TitleDisplay = 0x74;
        public const byte TitleDisplayClear = 0x00;
        public const byte TitleSet = 0x73;
        public const byte TitleSetParams = 0x00;
        public const byte TitleSetPart1 = 0x01;
        public const byte TitleSetPart2 = 0x02;
        public const byte Irreceive = 0x08;
        public const byte IrreceiveOn = 0x02;
        public const byte IrreceiveOff = 0x03;
        public const byte IrreceiveOnoff = 0x10;
        public const byte PtDrive = 0x01;
        public const byte PtDriveHorizLeft = 0x01;
        public const byte PtDriveHorizRight = 0x02;
        public const byte PtDriveHorizStop = 0x03;
        public const byte PtDriveVertUp = 0x01;
        public const byte PtDriveVertDown = 0x02;
        public const byte PtDriveVertStop = 0x03;
        public const byte PtAbsolutePosition = 0x02;
        public const byte PtRelativePosition = 0x03;
        public const byte PtHome = 0x04;
        public const byte PtReset = 0x05;
        public const byte PtLimitset = 0x07;
        public const byte PtLimitsetSet = 0x00;
        public const byte PtLimitsetClear = 0x01;
        public const byte PtLimitsetSetUr = 0x01;
        public const byte PtLimitsetSetDl = 0x00;
        public const byte PtDatascreen = 0x06;
        public const byte PtDatascreenOn = 0x02;
        public const byte PtDatascreenOff = 0x03;
        public const byte PtDatascreenOnoff = 0x10;

        public const byte PtVideosystemInq = 0x23;
        public const byte PtModeInq = 0x10;
        public const byte PtMaxspeedInq = 0x11;
        public const byte PtPositionInq = 0x12;
        public const byte PtDatascreenInq = 0x06;
    }

    public enum ZoomDirection
    {
        In = 0,
        Out = 1,
        None = 2
    }

    public enum DriveStatus
    {
        FullStop,
        Jog,
        StopJog,
        Absolute,
        StopAbsolute
    }

    public enum FocusDirection
    {
        Near,
        Far,
        None
    }
}