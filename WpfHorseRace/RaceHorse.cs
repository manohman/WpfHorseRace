using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace WpfHorseRace {
    /// <summary>
    /// Represents a horse in a race.
    /// </summary>
    public class RaceHorse : INotifyPropertyChanged {
        #region Data 
        public delegate void ObtainedPowerUpEventHandler();

        delegate void SetIndicatorCallback();
        // Static fields
        //readonly static Random random;
        static RaceHorse raceWinner = null;

        // Instance fields
        readonly DispatcherTimer timer = new DispatcherTimer();
        readonly string name;
        int percentComplete;
        string _imageSource;

        IPowerUp _powerUp;

        string _powerUpImageSource;
        private bool _showPowerUp;
        private int _powerUpPercentComplete;
        private int _moveMultiplier;
        private double _opacity;
        private bool _isSpeedUp;

        public event ObtainedPowerUpEventHandler OnObtainedPowerUp;

        #endregion // Data

        #region Constructors

        static RaceHorse() {
            
            //RaceHorse.random = new Random( DateTime.Now.Millisecond );
        }




        public RaceHorse(string name, string imageSource) {

            this._imageSource = imageSource;
            this.name = name;
            this.percentComplete = 0;
            _moveMultiplier = 1;
            _opacity = 100;

            //	this.timer.Tick += this.timer_Tick;			
        }

        internal void SetPowerUp(IPowerUp powerUp) {

            if(_powerUp != null) {
                _powerUp.Dispose();

            }

            _powerUp = powerUp;
            _powerUp.SetOwner(this);

            PowerUpDisplayImage = _powerUp.DisplayImage;

            //ShowPowerUp = false;

        }

        internal bool Move(int spaces) {
            int spacesToMove = spaces > 0 ? (spaces * _moveMultiplier) : spaces;
            PercentComplete += spacesToMove;
            return spacesToMove > 0;
        }






        #endregion // Constructors

        #region Public Properties

        public bool IsFinished {
            get { return this.PercentComplete >= 100; }
        }

        public bool IsWinner {
            get { return RaceHorse.raceWinner == this; }
        }

        public string Name {
            get { return this.name; }
        }


        public string DisplayedImagePath {
            get { return _imageSource; }
        }

        public int PowerUpPercentComplete {
            get {
                return _powerUpPercentComplete;
            }
            set {
                _powerUpPercentComplete = value;
                this.RaisePropertyChanged("PowerUpPercentComplete");

            }
        }

        public double Opacity {
            get { return _opacity; }
            set { _opacity = value;
                this.RaisePropertyChanged("Opacity");

            }
        }

        public int PercentComplete {
            get { return this.percentComplete; }
            private set {
                if (this.percentComplete == value)
                    return;

                if (value < 0 || value > 100)
                    return;

                bool wasFinished = this.IsFinished;

                this.percentComplete = value;

                this.RaisePropertyChanged("PercentComplete");

                if (wasFinished != this.IsFinished) {
                    if (this.IsFinished && RaceHorse.raceWinner == null) {
                        RaceHorse.raceWinner = this;
                        this.RaisePropertyChanged("IsWinner");
                    }

                    this.RaisePropertyChanged("IsFinished");
                }

                // In case this horse was the previous winner and a new race has begun,
                // notify the world that the IsWinner property has changed on this horse.
                if (wasFinished && value == 0)
                    this.RaisePropertyChanged("IsWinner");

                if(percentComplete == _powerUpPercentComplete && _showPowerUp) {
                    if(OnObtainedPowerUp != null) {
                        OnObtainedPowerUp();
                    }
                }
            }
        }

        public string PowerUpDisplayImage {
            get { return _powerUpImageSource; }
            private set {
                _powerUpImageSource = value;

                this.RaisePropertyChanged("PowerUpDisplayImage");
            }
        }

        internal void SetMoveMultiplier(int moveMultiplier) {
            _moveMultiplier = moveMultiplier;

            IsSpeedUp = _moveMultiplier > 1;

        }

        public bool ShowPowerUp {
            get { return _showPowerUp; }
            set {
                _showPowerUp = value;
                this.RaisePropertyChanged("ShowPowerUp");
            }
        }

        public bool IsSpeedUp {
            get { return _isSpeedUp; }
            private set {
                _isSpeedUp = value;
                this.RaisePropertyChanged("IsSpeedUp");

            }
        }

        #endregion // Public Properties

            #region Public Methods

        public void StartNewRace() {
            // When a race begins, remove a reference to the previous winner.
            if (RaceHorse.raceWinner != null)
                RaceHorse.raceWinner = null;

            // Put the horse back at the start of the track.
            this.PercentComplete = 0;
            _moveMultiplier = 1;
            // Give the horse a random "speed" to run at.
            //		this.timer.Interval = TimeSpan.FromMilliseconds( RaceHorse.random.Next( 20, 100 ) );

            // Start the DispatcherTimer, which ticks when the horse should "move."
            //			if( ! this.timer.IsEnabled )
            //			this.timer.Start();
        }

        #endregion // Public Methods		

        #region timer_Tick

        void timer_Tick(object sender, EventArgs e) {
            if (!this.IsFinished)
                ++this.PercentComplete;

            if (this.IsFinished)
                this.timer.Stop();
        }

        #endregion // timer_Tick

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName) {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}