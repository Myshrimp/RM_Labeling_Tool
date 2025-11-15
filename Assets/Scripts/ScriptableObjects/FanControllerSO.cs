using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "FanControllerConfig", menuName = "SO_Config/FanControllerConfig", order = 0)]
    public class FanControllerSO : ScriptableObject
    {
        public float speed;
        public float frequency;
    }
}