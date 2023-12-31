﻿using System;
using UnityEngine;
using Zenject;

namespace Ai
{
    public class TurnManager : IFixedTickable
    {
        private float _speedMultiplier;
        private float _timer;
        private const float BaseTimeBetweenTurns = 1;
        private bool _onPause;

        public event Action OnEarlyTurnTrigger; 
        public event Action OnTurnTrigger; 
        public event Action OnLateTurnTrigger; 

        [Inject]
        public TurnManager()
        {
            _timer = BaseTimeBetweenTurns;
            _speedMultiplier = 1;
        }

        public void FixedTick()
        {
            if (_onPause) return;
            
            _timer -= Time.fixedDeltaTime * _speedMultiplier;

            if (_timer >= 0) return;
            
            _timer = BaseTimeBetweenTurns; 
            OnEarlyTurnTrigger?.Invoke();
            OnTurnTrigger?.Invoke();
            OnLateTurnTrigger?.Invoke();
        }

        public void ChangeGameSpeed(float speedMultiplier)
        {
            _speedMultiplier = speedMultiplier;
        }

        public void TogglePause()
        {
            _onPause = !_onPause;
        }
    }
}