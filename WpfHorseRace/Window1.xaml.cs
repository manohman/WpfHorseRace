using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfHorseRace {
    public partial class Window1 : System.Windows.Window {
        delegate void SetIndicatorCallback();
        delegate void SetMoveCallback(int index);

        RaceController _raceController;
        PowerUpController _powerUpController;

        MediaPlayer _gameMusicPlayer;
        MediaPlayer _startEndPlayer;
        MediaPlayer _powerUpPlayer;

        List<string> _songFileNames;
        Random _random;
        List<string> _ports;
        List<MediaPlayer> _movePlayers;
        int _currentSongIndex;

        public Window1() {
            InitializeComponent();

            _random = new Random();
            _songFileNames = new List<string>();

            DirectoryInfo di = new DirectoryInfo("Resources\\Sounds");

            foreach (var fileInfo in di.GetFiles()) {
                _songFileNames.Add(fileInfo.FullName);
            }

            _gameMusicPlayer = new MediaPlayer();
            _startEndPlayer = new MediaPlayer();
            _powerUpPlayer = new MediaPlayer();
            _currentSongIndex = 0;

            _gameMusicPlayer.MediaEnded += _player_MediaEnded;
            var horses = CreateRaceHorses();

            foreach(var horse in horses) {
                horse.OnObtainedPowerUp += Horse_OnObtainedPowerUp;
            }

            this.raceTrack.ItemsSource = horses;

            _movePlayers = CreateMoveSoundPlayers();
            
            _raceController = new RaceController(new RandomMover());
            //_raceController = new RaceController(new SerialMover());
            _raceController.Horses = horses;
            _raceController.OnRaceOver += _raceController_OnRaceOver;
            _raceController.OnMove += _raceController_OnMove;

            _powerUpController = new PowerUpController(horses);


            this.Loaded += delegate { OnLoaded(); };
            this.lnkStartNewRace.Click += delegate { this.StartRace(); };

            Ports = new List<string>(SerialPort.GetPortNames());
            comboBox1.ItemsSource = Ports;
            
            this.MouseDoubleClick += Window1_MouseDoubleClick;

        }

        private void Horse_OnObtainedPowerUp() {
            if (CheckAccess() == false) {
                SetIndicatorCallback d = new SetIndicatorCallback(Horse_OnObtainedPowerUp);
                this.Dispatcher.BeginInvoke(d);
            } else {
                Uri uri = new Uri(@"Resources\powerup.mp3", UriKind.Relative);
                _powerUpPlayer.Open(uri);
                _powerUpPlayer.Play();
            }
        }

        private List<MediaPlayer> CreateMoveSoundPlayers() {
            List<MediaPlayer> movePlayers = new List<MediaPlayer>();

            MediaPlayer mediaPlayer1 = new MediaPlayer();
            MediaPlayer mediaPlayer2 = new MediaPlayer();
            MediaPlayer mediaPlayer3 = new MediaPlayer();

            Uri uri = new Uri(@"Resources\move1.mp3", UriKind.Relative);
            mediaPlayer1.Open(uri);


            Uri uri2 = new Uri(@"Resources\move2.mp3", UriKind.Relative);
            mediaPlayer2.Open(uri2);


            Uri uri3 = new Uri(@"Resources\move3.mp3", UriKind.Relative);
            mediaPlayer3.Open(uri3);

            movePlayers.Add(mediaPlayer1);
            movePlayers.Add(mediaPlayer2);
            movePlayers.Add(mediaPlayer3);

            return movePlayers;
        }



        private void Window1_MouseDoubleClick(object sender, MouseButtonEventArgs e) {

            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            } else if (WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }

        }

        private void OnLoaded() {
            if (Ports.Count > 0) {
                comboBox1.SelectedIndex = 0;
            }
        }

        public List<string> Ports {
            get { return _ports; }
            set { _ports = value; NotifyPropertyChanged("Ports"); }
        }



        private void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void _raceController_OnMove(int index) {
            if (CheckAccess() == false) {
                SetMoveCallback d = new SetMoveCallback(_raceController_OnMove);
                this.Dispatcher.BeginInvoke(d,index);
                //return;


            } else {
                _movePlayers[index].Position = TimeSpan.FromMilliseconds(1); ;
                _movePlayers[index].Play();

            }
        }

        private void _raceController_OnRaceOver() {
            if (CheckAccess() == false) {
                SetIndicatorCallback d = new SetIndicatorCallback(_raceController_OnRaceOver);
                this.Dispatcher.BeginInvoke(d);
                //return;


            } else {
                _gameMusicPlayer.Stop();
                PlayWinnerSound();
            }

        }

        private void PlayWinnerSound() {
            Uri uri = new Uri(@"Resources\Winner.mp3", UriKind.Relative);
            _startEndPlayer.Open(uri);
            _startEndPlayer.Play();
        }

        private void _player_MediaEnded(object sender, EventArgs e) {
            PlayRandomFile();
        }

        private void PlayRandomFile() {

            //int songIndex = _currentSongIndex++;

            if(_currentSongIndex >= _songFileNames.Count) {
                _currentSongIndex = 0;
            }


            string songName = _songFileNames[_currentSongIndex++];
            Uri uri = new Uri(songName, UriKind.Relative);
            _gameMusicPlayer.Open(uri);
            _gameMusicPlayer.Play();
        }

        void StartRace() {
            foreach (RaceHorse raceHorse in this.raceTrack.Items) {
                raceHorse.StartNewRace();
            }

            var selectedValue = comboBox1.SelectedValue;

            if (selectedValue != null) {
                string port = selectedValue.ToString();

                _raceController.StartNewRace(port);
                _powerUpController.StartNewRace();


                Uri uri = new Uri(@"Resources\startbell.mp3", UriKind.Relative);


                _gameMusicPlayer.MediaFailed += (o, args) => {
                    int index = 0;
                    index++;
                    //here you can get hint of what causes the failure 
                    //from method parameter args 
                };


                _startEndPlayer.Open(uri);
                _startEndPlayer.Play();

                PlayRandomFile();
            }
        }

        static List<RaceHorse> CreateRaceHorses() {
            List<RaceHorse> raceHorses = new List<RaceHorse>();

            raceHorses.Add(new RaceHorse("1", @"r1.png"));
            raceHorses.Add(new RaceHorse("2", @"r2.png"));
            raceHorses.Add(new RaceHorse("3", @"r3.png"));


            return raceHorses;
        }
    }
}