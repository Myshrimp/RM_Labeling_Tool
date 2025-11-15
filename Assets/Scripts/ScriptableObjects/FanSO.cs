using System;
using System.Collections.Generic;
using Controller;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "FanSO", menuName = "SO_Config/FanSO", order = 0)]
    public class FanSO : ScriptableObject
    {
        public List<LightStateItem> _lightStateItems;
        private Dictionary<FanState, List<int>> _lightStateMap;

        public void Init()
        {
            _lightStateMap = new Dictionary<FanState, List<int>>();
            foreach (var val in _lightStateItems)
            {
                _lightStateMap[val.fanState] = val.lights;
            }
        }

        public List<int> GetLights(FanState state)
        {
            return _lightStateMap[state];
        }
    }

    [Serializable]
    public struct LightStateItem
    {
        public FanState fanState;
        public List<int> lights;
    }
}