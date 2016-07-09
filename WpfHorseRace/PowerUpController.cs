﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHorseRace {
    public class PowerUpController {
        private List<RaceHorse> _horses;
        private List<IPowerUp> _powerUps;

        public PowerUpController(List<RaceHorse> horses) {
            this._horses = horses;
        }

        internal void StartNewRace() {
            foreach (var horse in _horses) {
                horse.SetPowerUp(PowerUpFactory.GetRandomPowerUp(_horses));
                horse.ShowPowerUp = false;
            }
        }

        public class PowerUpFactory {
            private static Random __random = new Random();

            public static IPowerUp GetRandomPowerUp(List<RaceHorse> horses) {

                int index = __random.Next(4);


                if (index == 0) {
                    return new DoubleMovePowerUp("Bullet_Bill.jpg", horses);
                } else if (index == 1) {
                    return new MoveOthersBackPowerUp("banana.jpg", horses);

                } else if (index == 2) {
                    return new FreezeOtherPlayerPowerUp("Triple_Banana.jpg", horses);

                } else {
                    return new MoveTwoStepsPowerUp("Star.png", horses);

                }

            }
        }
    }
}
