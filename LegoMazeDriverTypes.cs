
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;

using System;
using System.Collections.Generic;
using W3C.Soap;

using mazeDriver = Robotics.Services.LegoMazeDriver;

namespace Robotics.Services.LegoMazeDriver
{

    public static class Contract
    {
        public const string Identifier = "http://schemas.microsoft.com/robotics/2006/06/roboticstutorial4.user.html";
    }

    [DataContract]
    public class LegoMazeDriverState
    {
        private bool _motorEnabled;

        [DataMember]
        public bool MotorEnabled
        {
            get { return _motorEnabled; }
            set { _motorEnabled = value; }
        }

    }

    [ServicePort]
    public class LegoMazeDriverOperations : PortSet<
        DsspDefaultLookup,
        DsspDefaultDrop,
        Get,
        Replace,
        Stop,
        Forward,
        Backward,
        TurnLeft,
        TurnRight>
    {
    }

    public class Get : Get<GetRequestType, PortSet<LegoMazeDriverState, Fault>>
    {
    }

    public class Replace : Replace<LegoMazeDriverState, PortSet<DefaultReplaceResponseType, Fault>>
    {
        public Replace()
        {
        }

        public Replace(LegoMazeDriverState body)
            : base(body)
        {
        }
    }

    public class Stop : Submit<StopRequest, PortSet<DefaultSubmitResponseType, Fault>>
    {
        public Stop()
            : base(new StopRequest())
        {
        }
    }

    [DataContract]
    public class StopRequest { }

    public class Forward : Submit<ForwardRequest, PortSet<DefaultSubmitResponseType, Fault>>
    {
        public Forward()
            : base(new ForwardRequest())
        {
        }
    }

    [DataContract]
    public class ForwardRequest { }

    public class Backward : Submit<BackwardRequest, PortSet<DefaultSubmitResponseType, Fault>>
    {
        public Backward()
            : base(new BackwardRequest())
        {
        }
    }

    [DataContract]
    public class BackwardRequest { }

    public class TurnLeft : Submit<TurnLeftRequest, PortSet<DefaultSubmitResponseType, Fault>>
    {
        public TurnLeft()
            : base(new TurnLeftRequest())
        {
        }
    }

    [DataContract]
    public class TurnLeftRequest { }

    public class TurnRight : Submit<TurnRightRequest, PortSet<DefaultSubmitResponseType, Fault>>
    {
        public TurnRight()
            : base(new TurnRightRequest())
        {
        }
    }

    [DataContract]
    public class TurnRightRequest { }
}
