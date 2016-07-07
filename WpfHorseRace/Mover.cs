using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows.Threading;

namespace WpfHorseRace {

    public class SerialMover : IMover {
        public event MoveRequestEventHandler MoveRequested;

        System.IO.Ports.SerialPort _serialPort;
        bool _isActive;

        public SerialMover() {
            _serialPort = new System.IO.Ports.SerialPort("COM3", 9600);

            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;
            _serialPort.RtsEnable = true;
            _serialPort.DataReceived += _serialPort_DataReceived;
            _isActive = false;

        }

        private void _serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            string currentString = _serialPort.ReadExisting();

            if (_isActive) {
                foreach (char character in currentString) {
                    int index = CharToInt1(character);


                    if (MoveRequested != null) {
                        MoveRequested(index);
                    }
                }
            }
        }

        public static int CharToInt1(char input) {
            int result = -1;

            if (input >= 48 && input <= 57) {
                result = input - '0';
            }

            return result;
        }

        public void GameOver() {
            _isActive = false;
        }

        public void StartNewRace(string port) {
            try {
                if (_serialPort.IsOpen) {
                    _serialPort.Close();
                }

                _serialPort.PortName = port;

                _serialPort.Open();
                _isActive = true;
                //_serialPort.Write("Hello world");

            } catch {
                _isActive = false;
            }
        }
    }


    public class RandomMover : IMover {
        public event MoveRequestEventHandler MoveRequested;

        readonly DispatcherTimer _timer;
        private Random _random;

        public RandomMover() {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            this._timer.Tick += this.timer_Tick;

            _random = new Random(DateTime.Now.Millisecond);
        }

        private void timer_Tick(object sender, EventArgs e) {

            if (MoveRequested != null) {
                MoveRequested(_random.Next(0, 3));
            }



        }

        public void GameOver() {
            _timer.Stop();
        }

        public void StartNewRace(string port) {
            _timer.Start();
        }
    }


    public interface IMover {
        event MoveRequestEventHandler MoveRequested;

        void GameOver();
        void StartNewRace(string port);
    }



    public delegate void MoveRequestEventHandler(int index);
}
