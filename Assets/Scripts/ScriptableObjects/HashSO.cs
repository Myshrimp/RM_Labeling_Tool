using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "HashConfig", menuName = "SO_Config/HashConfig", order = 0)]
    public class HashSO : ScriptableObject
    {
        [SerializeField] private List<HashElement> _elements;
        private Dictionary<string, string> _hashMap;

        public void Init()
        {
            _hashMap = new Dictionary<string, string>();
            foreach (var ele in _elements)
            {
                _hashMap[ele.key] = ele.value;
            }
        }

        public string Get(string key)
        {
            return _hashMap[key];
        }
    }

    [Serializable]
    public struct HashElement
    {
        public string key;
        public string value;
    }
}