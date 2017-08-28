using Microsoft.Ccr.Core;
using Microsoft.Ccr.Adapters.WinForms;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using System.Security.Permissions;
using xml = System.Xml;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using W3C.Soap;
using Robotics.Services.LegoMazeDriver.Properties;
using Microsoft.Robotics.Services.Drive.Proxy;
using System.ComponentModel;
//using contactSensorProxy = Microsoft.Robotics.Services.ContactSensor.Proxy;
using contactSensorProxy = Microsoft.Robotics.Services.Sample.HiTechnic.ColorV2;


namespace Robotics.Services.LegoMazeDriver
{
    [DisplayName("LOKI Project: genetic algorithms maze solver")]
    [Description("Drives the solution to a maze that is derived with genetic algorithms in the application GeneticMazeSolver")]
    [DssServiceDescription("http://msdn.microsoft.com/library/bb483053.aspx")]
    [Contract(Contract.Identifier)]
    public class LegoMazeDriver : DsspServiceBase
    {
        [ServiceState]
        private LegoMazeDriverState _state = new LegoMazeDriverState();

        [ServicePort("/LegoMazeDriver", AllowMultipleInstances=false)]
        private LegoMazeDriverOperations _mainPort = new LegoMazeDriverOperations();

        [Partner("Drive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        private drive.DriveOperations _drivePort = new drive.DriveOperations();
        private drive.DriveOperations _driveNotify = new drive.DriveOperations();

        [Partner("ColorSensor", Contract = contactSensorProxy.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        private contactSensorProxy.ColorV2SensorOperations _colorSensorPort = new contactSensorProxy.ColorV2SensorOperations();
        
        public LegoMazeDriver(DsspServiceCreationPort creationPort) :
                base(creationPort)
        {
        }

        protected override void Start()
        {
            base.Start();

            WinFormsServicePort.Post(new RunForm(StartForm));

            // Start listening for colors
            //SubscribeToColorSensor();

            _drivePort.Subscribe(_driveNotify);
            Activate(Arbiter.Receive<drive.Update>(true, _driveNotify, NotifyDriveUpdate));
        }

        private System.Windows.Forms.Form StartForm()
        {
            //TODO: put form and logic to choose form at startup
            //LegoMazeDriverForm form = new LegoMazeDriverForm(_mainPort);
            LegoMazeAutomatedForm form = new LegoMazeAutomatedForm(_mainPort);

            Invoke(delegate()
                {
                    PartnerType partner = FindPartner("Drive");
                    Uri uri = new Uri(partner.Service);
                    form.Text = string.Format(
                        Resources.Culture,
                        Resources.Title,
                        uri.AbsolutePath
                    );
                }
            );

            return form;
        }

        private void Invoke(System.Windows.Forms.MethodInvoker mi)
        {
            WinFormsServicePort.Post(new FormInvoke(mi));
        }


        /// <summary>
        /// Replace Handler
        /// </summary>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReplaceHandler(Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> StopHandler(Stop stop)
        {
            drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
            request.LeftWheelPower = 0;
            request.RightWheelPower = 0;

            yield return Arbiter.Choice(
                _drivePort.SetDrivePower(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to stop", fault);
                }
            );
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> ForwardHandler(Forward forward)
        {
            if (!_state.MotorEnabled)
            {
                yield return EnableMotor();
            }

            // This sample sets the power to 75%.
            // Depending on your robotic hardware,
            // you may wish to change these values.
            drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
            request.LeftWheelPower = 0.75;
            request.RightWheelPower = 0.75;

            yield return Arbiter.Choice(
                _drivePort.SetDrivePower(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to drive forwards", fault);
                }
            );
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> BackwardHandler(Backward backward)
        {
            if (!_state.MotorEnabled)
            {
                yield return EnableMotor();
            }

            drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
            request.LeftWheelPower = -0.6;
            request.RightWheelPower = -0.6;

            yield return Arbiter.Choice(
                _drivePort.SetDrivePower(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to drive backwards", fault);
                }
            );
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> TurnLeftHandler(TurnLeft turnLeft)
        {
            if (!_state.MotorEnabled)
            {
                yield return EnableMotor();
            }

            drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
            request.RightWheelPower = -0.5;
            request.LeftWheelPower = 0.5;

            yield return Arbiter.Choice(
                _drivePort.SetDrivePower(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to turn left", fault);
                }

            );



        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> TurnRightHandler(TurnRight forward)
        {
            if (!_state.MotorEnabled)
            {
                yield return EnableMotor();
            }

            drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
            request.RightWheelPower = 0.5;
            request.LeftWheelPower = -0.5;

            yield return Arbiter.Choice(
                _drivePort.SetDrivePower(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to turn right", fault);
                }
            );
        }

        private Choice EnableMotor()
        {
            drive.EnableDriveRequest request = new drive.EnableDriveRequest();
            request.Enable = true;

            return Arbiter.Choice(
                _drivePort.EnableDrive(request),
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault)
                {
                    LogError(null, "Unable to enable motor", fault);
                }
            );
        }

        private void NotifyDriveUpdate(drive.Update update)
        {
            LegoMazeDriverState state = new LegoMazeDriverState();
            state.MotorEnabled = update.Body.IsEnabled;

            _mainPort.Post(new Replace(state));
        }

        /// <summary>
        /// Handle Color Notifications
        /// </summary>
        /// <param name="notification">Update notification</param>
        private void ColorHandler(contactSensorProxy.ColorV2SensorUpdate notification)
        {

            // Check for a "red" value.  Color values are from 0 to 255, so a high red value and low blue and green values means we have a red
            if (notification.Body.Red >= 150 && notification.Body.Green <= 50 && notification.Body.Blue <= 50)
                return;

        }
        /*
        /// <summary>
        /// Subscribe to notifications when bumpers are pressed
        /// </summary>
        void SubscribeToColorSensor()
        {
            // Create color sensor notification port
            contactSensorProxy.ColorV2SensorOperations colorSensorNotificationPort = new contactSensorProxy.ColorV2SensorOperations();
            
            // Subscribe to the color sensor service, send notifications to colorSensorNotificationPort
            _colorSensorPort.Subscribe(colorSensorNotificationPort);
 
            // Start listening for color update notifications
            Activate(
                Arbiter.Receive<contactSensorProxy.Update>
                    (true, colorSensorNotificationPort, ColorHandler));
        }
        */
    }
}
