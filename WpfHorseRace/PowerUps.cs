using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHorseRace {
    public abstract class AbstractPowerUps: IPowerUp {
        private List<RaceHorse> _horses;
        string _imagePath;
        private RaceHorse _owner;

        public AbstractPowerUps(string imagePath, List<RaceHorse> horses) {
            this._horses = horses;
            this._imagePath = imagePath;
        }

        public string DisplayImage {
            get {
                return _imagePath;
            }
        }

        public void SetOwner(RaceHorse owner) {
            _owner = owner;
            _owner.ShowPowerUp = false;
        }
    }


    public class MoveOthersBackPowerUp : AbstractPowerUps {
        public MoveOthersBackPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }
    }

    public class DoubleMovePowerUp : AbstractPowerUps {
        public DoubleMovePowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }


    }

    public class MoveTwoStepsPowerUp: AbstractPowerUps {
        public MoveTwoStepsPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }
    }


    public class FreezeOtherPlayerPowerUp : AbstractPowerUps {
        public FreezeOtherPlayerPowerUp(string imagePath, List<RaceHorse> horses) : base(imagePath, horses) {
        }
    }





    public interface IPowerUp {
        string DisplayImage { get; }
        void SetOwner(RaceHorse owner);
    }
}
