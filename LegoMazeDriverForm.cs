
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dss.ServiceModel.Dssp;

namespace Robotics.Services.LegoMazeDriver
{
    public partial class LegoMazeDriverForm : Form
    {
        LegoMazeDriverOperations _mainPort;
        System.Timers.Timer _turnTimer;
        const int _turnInterval = 1200;  // time for 90 degree turn in microseconds

        public LegoMazeDriverForm(LegoMazeDriverOperations mainPort)
        {
            _mainPort = mainPort;

            // Timer for left and right turns to approximate 90 degree turns
            _turnTimer = new System.Timers.Timer(_turnInterval);
            _turnTimer.Elapsed += new System.Timers.ElapsedEventHandler(_turnTimer_Elapsed);

            InitializeComponent();
        }

        void _turnTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // When timer elapses, stop the turn
            _mainPort.Post(new Stop());

            _turnTimer.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            _mainPort.Post(new DsspDefaultDrop(DropRequestType.Instance));

            base.OnClosed(e);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new Stop());
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new Forward());
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new Backward());
        }

        private void tnTurnLeft_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new TurnLeft());

            _turnTimer.Interval = _turnInterval;
            _turnTimer.Enabled = true;
        }

        private void btnTurnRight_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new TurnRight());

            _turnTimer.Interval = _turnInterval;
            _turnTimer.Enabled = true;
        }
    }
}