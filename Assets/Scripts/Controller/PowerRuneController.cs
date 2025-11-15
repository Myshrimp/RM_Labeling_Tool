using System;
using Flower;
using UnityEngine;

namespace Controller
{
    public class PowerRuneController : MonoBehaviour
    {
        [SerializeField] private FanController _red;
        [SerializeField] private FanController _blue;
        private void Update()
        {
            bool isRotate = MyGameEntry.Input.GetBool("Rotate");
            bool isTogglePowerOn = MyGameEntry.Input.GetBool("TogglePowerOn");
            bool isChangeMode = MyGameEntry.Input.GetBool("ChangeMode");
            if (isRotate)
            {
                _red.ToggleRotate();
                _blue.ToggleRotate();
            }

            if (isTogglePowerOn)
            {
                _red.ToggleIsPowerOn();
                _blue.ToggleIsPowerOn();
            }
            
            if (isChangeMode)
            {
                _red.ChangeFanLight();
                _blue.ChangeFanLight();
            }
        }
    }
}