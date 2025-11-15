using System;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Controller
{
    public class FanController : MonoBehaviour
    {
        [SerializeField] private FanControllerSO _fanControllerConfig;
        [SerializeField] private FanSO _fanConfig;
        [SerializeField] private float _curSpeed = 0;

        private ObjectRotator _rotator;
        private bool _isRotating = false;
        private bool _isPowerOn = false;
        private float _timer = 0;
        private Dictionary<int, Fan> _fans;
        private int _index = 0;

        private void Awake()
        {
            _fans = new Dictionary<int, Fan>();
            _rotator = GetComponent<ObjectRotator>();
            _fanConfig.Init();
            FindFans();
            StopRotate();
        }

        public void ToggleRotate()
        {
            _isRotating = !_isRotating;
            if (!_isRotating) StopRotate();
        }

        public void ToggleIsPowerOn()
        {
            _isPowerOn = !_isPowerOn;
            if (!_isPowerOn)
            {
                StopAllLights();
            }
            else
            {
                ChangeFanLight();
            }
        }

        public void ChangeFanLight()
        {
            if (!_isPowerOn) _isPowerOn = true;
            int rand1 = Random.Range(0, _fans.Count);
            int rand2 = rand1;
            while (rand2 == rand1)
            {
                rand2 = Random.Range(0, _fans.Count);
            }

            for (int i = 0; i < _fans.Count; i++)
            {
                if (i == rand1 || i == rand2)
                {
                    _fans[i].ChangeState(FanState.PowerOn);
                }
                else
                {
                    _fans[i].ChangeState(FanState.HalfPowerUp);
                }
            }
        }

    private void StopAllLights()
        {
            for (int i = 0; i < _fans.Count; i++)
            {
                _fans[i].ChangeState(FanState.PowerOff);
            }
        }

    private void Update()
        {
            _timer += Time.deltaTime;
            if(_isRotating)Rotate();
        }

        private void Rotate()
        {
            float halfSpeed = _fanControllerConfig.speed / 2;
            _curSpeed =halfSpeed +  halfSpeed *
                            Mathf.Sin(
                                _timer * _fanControllerConfig.frequency
                                )
                            ;
            _rotator.SetRotation(transform.up, _curSpeed, ObjectRotator.RotationDirection.CounterClockwise);
        }

        private void StopRotate()
        {
            _rotator.SetRotation(transform.up, 0, ObjectRotator.RotationDirection.Clockwise);
        }

        private void FindFans()
        {
            foreach (Transform fanTransform in transform)
            {
                if (fanTransform.name.StartsWith("fan"))
                {
                    _fans.Add(_index, new Fan(fanTransform, _fanConfig));
                    _index += 1;
                }
            }
        }
    }
    
    public enum FanState
    {
        PowerOff, HalfPowerUp, PowerOn
    }
    public class Fan
    {
        private Dictionary<int, Transform> _lights;
        private int _index = 0;
        private FanState _curState;
        private FanSO _fanConfig;
        public void Init(Transform parent, FanSO config)
        {
            _lights = new Dictionary<int, Transform>();
            foreach (Transform child in parent)
            {
                _lights[_index] = child;
                _index += 1;
            }

            _fanConfig = config;

            _curState = FanState.PowerOff;
            OnStateChange(FanState.PowerOff);
        }

        public void ChangeState(FanState nextState)
        {
            if (_curState == nextState) return;
            _curState = nextState;
            OnStateChange(nextState);
        }

        private void OnStateChange(FanState nextState)
        {
            TurnOffAllLights();
            List<int> turnOnLights = _fanConfig.GetLights(nextState);
            if (turnOnLights.Count == 0) return;
            turnOnLights.Sort();
            foreach (var key in turnOnLights)
            {
                _lights[key].gameObject.SetActive(true);
            }
        }

        private void TurnOffAllLights()
        {
            for (int i = 0; i < _lights.Count; i++)
            {
                _lights[i].gameObject.SetActive(false);
            }
        }

        public Fan(){}

        public Fan(Transform fanTransform, FanSO fanConfig)
        {
            Init(fanTransform, fanConfig);
        }
    }
}