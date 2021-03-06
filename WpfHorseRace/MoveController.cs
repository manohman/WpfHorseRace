﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHorseRace {
    public class RaceController {
        public event RaceOverEventHandler OnRaceOver;
        public event MoveEventHandler OnMove;

        IMover _mover;

        public List<RaceHorse> Horses { get; internal set; }
        public IMover Mover { get {
                return _mover;
            }
            set {
                if(_mover != value) {
                    _mover.MoveRequested -= _mover_MoveRequested;

                    _mover = value;
                    _mover.MoveRequested += _mover_MoveRequested;

                }
            }
        }

        public RaceController(IMover mover) {
            _mover = mover;
            _mover.MoveRequested += _mover_MoveRequested;

        }

         private void _mover_MoveRequested(int index) {

            if (index >= 0 && index < Horses.Count) {
                var raceHorse = Horses[index];


                bool moved = raceHorse.Move(5);

                if (moved && OnMove != null) {
                    OnMove(index);
                }


                if (raceHorse.IsWinner) {
                    _mover.GameOver();

                    if (OnRaceOver != null) {
                        OnRaceOver();
                    }
                }
            }

        }

        internal void StartNewRace(string port) {
            _mover.StartNewRace(port);
        }
    }

    public delegate void RaceOverEventHandler();
    public delegate void MoveEventHandler(int index);

}
