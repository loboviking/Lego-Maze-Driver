
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Ccr.Core.Arbiters;

namespace Robotics.Services.LegoMazeDriver
{
    public partial class LegoMazeAutomatedForm : Form
    {
        LegoMazeDriverOperations _mainPort;
        System.Timers.Timer _moveTimer;
        const int _turnInterval = 1150;  // time for 90 degree turn in microseconds
        const int _forwardInterval = 1000;  // time for moving one "cell" forward in microseconds
        const int _backwardInterval = 1500;  // time for moving one "cell" forward in microseconds
        const int _initialDelayInterval = 5000;  // delay after the first movement post to the nxt to establish a link

        const char forwardChar = 'F';
        const char rightChar = 'R';
        const char leftChar = 'L';
        const char stopChar = 'S';

        public LegoMazeAutomatedForm(LegoMazeDriverOperations mainPort)
        {
            _mainPort = mainPort;

            // Timer for left and right turns to approximate 90 degree turns
            _moveTimer = new System.Timers.Timer(_turnInterval);
            _moveTimer.Elapsed += new System.Timers.ElapsedEventHandler(_moveTimer_Elapsed);

            InitializeComponent();
        }

        void _moveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // When timer elapses, stop the move
            _mainPort.Post(new Stop());

            _moveTimer.Enabled = false;
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

            _moveTimer.Interval = _forwardInterval;
            _moveTimer.Enabled = true;

            // Wait until this move is processed before processing the next
            while (_moveTimer.Enabled) ;
        }

        private void btnBackward_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new Backward());

            _moveTimer.Interval = _backwardInterval;
            _moveTimer.Enabled = true;

            // Wait until this move is processed before processing the next
            while (_moveTimer.Enabled) ;
        }

        private void btnTurnLeft_Click(object sender, EventArgs e)
        {
            _mainPort.Post(new TurnLeft());

            _moveTimer.Interval = _turnInterval;
            _moveTimer.Enabled = true;

            // Wait until this move is processed before processing the next
            while (_moveTimer.Enabled) ;
        }

        private void btnTurnRight_Click(object sender, EventArgs e)
        {
            TurnRight turnRight = new TurnRight();

            _mainPort.Post(turnRight);

            //while (turnRight.ResponsePort.P1.ItemCount <= 0 && turnRight.ResponsePort.P0.ItemCount <= 0) ;
                //MessageBox.Show("Has headers");


            //if (turnRight.ResponsePort.P0.ItemCount <= 0)
            //    MessageBox.Show("Has headers");


            _moveTimer.Interval = _turnInterval;
            _moveTimer.Enabled = true;

            // Wait until this move is processed before processing the next
            while (_moveTimer.Enabled) ;
            
        }

        private void btnDrive_Click(object sender, EventArgs e)
        {
            System.IO.StreamReader reader = new System.IO.StreamReader(@"c:\Users\Pehr\Documents\robotSolution.txt");

            string moveInstructions = reader.ReadToEnd();

            for (int i = 0; i < moveInstructions.Length; i++)
            {
                switch (moveInstructions[i])
                {
                    case forwardChar:
                        this.btnForward_Click(sender, e);
                        break;
                    case rightChar:
                        this.btnTurnRight_Click(sender, e);
                        break;
                    case leftChar:
                        this.btnTurnLeft_Click(sender, e);
                        break;
                    case stopChar:
                        this.btnStop_Click(sender, e);
                        break;
                    default:
                        break;

                }
                reader.Close();
            }

            /*
            this.btnForward_Click(sender, e);
            this.btnTurnLeft_Click(sender, e);
            this.btnForward_Click(sender, e);
            this.btnTurnLeft_Click(sender, e);
            this.btnForward_Click(sender, e);
            this.btnTurnLeft_Click(sender, e);
            this.btnForward_Click(sender, e);

            this.btnBackward_Click(sender, e);
            this.btnTurnRight_Click(sender, e);
            this.btnBackward_Click(sender, e);
            this.btnTurnRight_Click(sender, e);
            this.btnBackward_Click(sender, e);
            this.btnTurnRight_Click(sender, e);
            this.btnBackward_Click(sender, e);*/
            this.btnTurnRight_Click(sender, e);

        }
    }
}