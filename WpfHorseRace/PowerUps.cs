using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHorseRace {
    public abstract class AbstractPowerUps: IPowerUp {
        protected List<RaceHorse> _horses;
        string _imagePath;
        protected RaceHorse _owner;
        readonly System.Windows.Threading.DispatcherTimer _timer;
        Random _random;
        bool _isWaitingToShow;
        bool _isDisabled;

        public AbstractPowerUps(string imagePath, List<RaceHorse> horses) {
            this._horses = horses;
            this._imagePath = imagePath;
            _isWaitingToShow = true;
            _isDisabled = false;
            _random = new Random(DateTime.Now.Millisecond);

            _timer = new System.Windows.Threading.DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(_random.Next(10, 21));
            this._timer.Tick += this.timer_Tick;
        }

        internal abstract void ActivePowerUp();


        private void timer_Tick(object sender, EventArgs e) {
            _timer.Stop();

            if (_isWaitingToShow) {
                _owner.PowerUpPercentComplete = _owner.PercentComplete + 20;
                _owner.ShowPowerUp = true;
                _isWaitingToShow = false;

                _timer.Interval = TimeSpan.FromSeconds(15);
                _timer.Start();


            } else {
                _owner.ShowPowerUp = false;
            }


        }

        public string DisplayImage {
            get {
                return _imagePath;
            }
        }

        public void SetOwner(RaceHorse owner) {
            _owner = owner;
            _owner.ShowPowerUp = false;
            _owner.OnObtainedPowerUp += _owner_OnObtainedPowerUp;
            _timer.Start();
        }

        private void _owner_OnObtainedPowerUp() {
            if (_isDisabled == false) {
                _isDisabled = true;
                _owner.ShowPowerUp = false;
                _timer.Stop();
                ActivePowerUp();
            }
        }



        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    this._timer.Stop();
                    this._timer.Tick -= this.timer_Tick;
                    _owner.OnObtainedPowerUp -= _owner_OnObtainedPowerUp;

                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AbstractPowerUps() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }


    public class MoveOthersBackPowerUp : AbstractPowerUps {
        public MoveOthersBackPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }

        internal override void ActivePowerUp() {
            
            foreach(var horse in _horses) {
                if(horse != _owner) {
                    if (horse.PercentComplete >= 10) {
                        horse.Move(-10);
                    }else {
                        horse.Move(-horse.PercentComplete);
                    }
                }
            }
        }
    }

    public class DoubleMovePowerUp : AbstractPowerUps {
        readonly System.Windows.Threading.DispatcherTimer _activeTimer;

        public DoubleMovePowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
            _activeTimer = new System.Windows.Threading.DispatcherTimer();
            _activeTimer.Interval = TimeSpan.FromSeconds(10);
            _activeTimer.Tick += _activeTimer_Tick;

        }

        private void _activeTimer_Tick(object sender, EventArgs e) {
            _activeTimer.Stop();
            _owner.SetMoveMultiplier(1);
        }

        internal override void ActivePowerUp() {
            _owner.SetMoveMultiplier(2);
            _activeTimer.Start();
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            _activeTimer.Stop();
            _activeTimer.Tick -= _activeTimer_Tick;
        }

    }

    public class MoveTwoStepsPowerUp: AbstractPowerUps {
        public MoveTwoStepsPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }

        internal override void ActivePowerUp() {
            _owner.Move(10);
        }
    }


    public class FreezeOtherPlayerPowerUp : AbstractPowerUps {
        readonly System.Windows.Threading.DispatcherTimer _activeTimer;

        public FreezeOtherPlayerPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
            _activeTimer = new System.Windows.Threading.DispatcherTimer();
            _activeTimer.Interval = TimeSpan.FromSeconds(10);
            _activeTimer.Tick += _activeTimer_Tick;
        }

        internal override void ActivePowerUp() {

            foreach (var horse in _horses) {
                if (horse != _owner) {
                    horse.SetMoveMultiplier(0);
                    horse.Opacity = .50;
                }
            }

            _activeTimer.Start();
        }


        private void _activeTimer_Tick(object sender, EventArgs e) {
            _activeTimer.Stop();
            foreach (var horse in _horses) {
                if (horse != _owner) {
                    horse.SetMoveMultiplier(1);
                    horse.Opacity = 1;

                }
            }
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            _activeTimer.Stop();
            _activeTimer.Tick -= _activeTimer_Tick;
        }
    }





    public interface IPowerUp : IDisposable {
        string DisplayImage { get; }
        void SetOwner(RaceHorse owner);
    }
}
