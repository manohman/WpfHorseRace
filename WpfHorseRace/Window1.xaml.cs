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

namespace WpfHorseRace
{
	public partial class Window1 : System.Windows.Window
	{

        RaceController _raceController;
        MediaPlayer _player;
        MediaPlayer _player2;

        List<string> _songFileNames;
        Random _random;
        List<string> _ports;

        public Window1()
		{
			InitializeComponent();

            _random = new Random();
            _songFileNames = new List<string>();

            DirectoryInfo di = new DirectoryInfo("Resources\\Sounds");


            foreach(var fileInfo in di.GetFiles()) {
                _songFileNames.Add(fileInfo.Name);
            }

            

            _player = new MediaPlayer();
            _player2 = new MediaPlayer();

            _player.MediaEnded += _player_MediaEnded;
            var horses = CreateRaceHorses();
            this.raceTrack.ItemsSource = horses;

            //_raceController = new RaceController(new RandomMover());
            _raceController = new RaceController(new SerialMover());
            _raceController.Horses = horses;
            _raceController.OnRaceOver += _raceController_OnRaceOver;

            this.Loaded += delegate { OnLoaded(); };
			this.lnkStartNewRace.Click += delegate { this.StartRace(); };

            Ports = new List<string>(SerialPort.GetPortNames());
            comboBox1.ItemsSource = Ports;


            this.MouseDoubleClick += Window1_MouseDoubleClick;

        }

        private void Window1_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            
            if(WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.SingleBorderWindow;
            } else if(WindowState == WindowState.Normal) {
                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;
            }

        }

        private void OnLoaded() {
            if(Ports.Count > 0) {
                comboBox1.SelectedIndex = 0;
            }
        }

        public List<string> Ports {
            get { return _ports; }
            set { _ports = value; NotifyPropertyChanged("Ports"); }
        }

        delegate void SetIndicatorCallback();

        private void NotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private void _raceController_OnRaceOver() {
            if(CheckAccess()== false) {
                SetIndicatorCallback d = new SetIndicatorCallback(_raceController_OnRaceOver);
                this.Dispatcher.BeginInvoke(d);
                //return;


            }else {
                _player.Stop();
            }
            
        }

        private void _player_MediaEnded(object sender, EventArgs e) {
            //string songName = _songFileNames[1];
            PlayRandomFile();
        }

        private void PlayRandomFile() {
            string songName = _songFileNames[_random.Next(_songFileNames.Count - 1)];
            //string songName = @"Resources\TwinkleTwinkleLittleStar.mp3";

            Uri uri = new Uri(songName, UriKind.Relative);
            _player.Open(uri);
            _player.Play();
        }

        void StartRace() {
            foreach (RaceHorse raceHorse in this.raceTrack.Items) {
                raceHorse.StartNewRace();
            }

            var selectedValue = comboBox1.SelectedValue;

            if (selectedValue != null) {
                string port = selectedValue.ToString();

                _raceController.StartNewRace(port);

                Uri uri = new Uri(@"Resources\AndTheyreoff.mp3", UriKind.Relative);


                _player.MediaFailed += (o, args) => {
                    int index = 0;
                    index++;
                    //here you can get hint of what causes the failure 
                    //from method parameter args 
                };


                _player2.Open(uri);
                _player2.Play();

                PlayRandomFile();



            }

        }

        static List<RaceHorse> CreateRaceHorses()
		{
			List<RaceHorse> raceHorses = new List<RaceHorse>();

			raceHorses.Add( new RaceHorse( "Star 1", "r1.png" ) );
			raceHorses.Add( new RaceHorse( "Star 2", "r2.png" ) );
			raceHorses.Add( new RaceHorse( "Star 3", "r3.png" ) );
			//raceHorses.Add( new RaceHorse( "Fresh Spice" ) );
			//raceHorses.Add( new RaceHorse( "Bluegrass" ) );
			//raceHorses.Add( new RaceHorse( "Kit Madison" ) );

			return raceHorses;
		}
	}
}