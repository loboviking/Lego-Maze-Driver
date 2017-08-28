using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Dss.ServiceModel.Dssp;

namespace Robotics.Services.LegoMazeDriver
{
    class LegoMazeAutomated
    {
        LegoMazeDriverOperations _mainPort;
        System.Timers.Timer _turnTimer;
        const int _turnInterval = 1200;  // time for 90 degree turn in microseconds

        public LegoMazeAutomated(LegoMazeDriverOperations mainPort)
        {
            _mainPort = mainPort;

            // Timer for left and right turns to approximate 90 degree turns
            _turnTimer = new System.Timers.Timer(_turnInterval);
            _turnTimer.Elapsed += new System.Timers.ElapsedEventHandler(_turnTimer_Elapsed);

        }

        void _turnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // When timer elapses, stop the turn
            _mainPort.Post(new Stop());

            _turnTimer.Enabled = false;
        }

        public void Close(EventArgs e)
        {
            _mainPort.Post(new DsspDefaultDrop(DropRequestType.Instance));
        }

        public void Stop(object sender, EventArgs e)
        {
            _mainPort.Post(new Stop());
        }

        private void Forward(object sender, EventArgs e)
        {
            _mainPort.Post(new Forward());
        }

        private void Backward(object sender, EventArgs e)
        {
            _mainPort.Post(new Backward());
        }

        private void TurnLeft(object sender, EventArgs e)
        {
            _mainPort.Post(new TurnLeft());

            _turnTimer.Interval = _turnInterval;
            _turnTimer.Enabled = true;
        }

        private void TurnRight(object sender, EventArgs e)
        {
            _mainPort.Post(new TurnRight());

            _turnTimer.Interval = _turnInterval;
            _turnTimer.Enabled = true;
        }
    }
}
