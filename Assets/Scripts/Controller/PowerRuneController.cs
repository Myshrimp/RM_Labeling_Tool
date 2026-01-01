using System;
using Flower;
using UnityEngine;

namespace Controller
{
    public class PowerRuneController : MonoBehaviour
    {
        [SerializeField] private FanController _red;
        [SerializeField] private FanController _blue;
        [SerializeField] private int _controlMode = 0x01;
        private int red = 0x01;
        private int blue = 0x10;
        private int both = 0x11;
        private void Update()
        {
            bool isRotate = MyGameEntry.Input.GetBool("Rotate");
            bool isTogglePowerOn = MyGameEntry.Input.GetBool("TogglePowerOn");
            bool isChangeMode = MyGameEntry.Input.GetBool("ChangeMode");

            if (MyGameEntry.Input.GetBool("ControlModeRed")) _controlMode = red;
            if (MyGameEntry.Input.GetBool("ControlModeBlue")) _controlMode = blue;
            if (MyGameEntry.Input.GetBool("ControlModeBoth")) _controlMode = both;

            int isRed = _controlMode & red;
            int isBlue = _controlMode & blue;
            if (isRotate)
            {
                if(isRed > 0) _red.ToggleRotate();
                if(isBlue> 0) _blue.ToggleRotate();
            }

            if (isTogglePowerOn)
            {
                if(isRed > 0) _red.ToggleIsPowerOn();
                if(isBlue> 0) _blue.ToggleIsPowerOn();
            }
            
            if (isChangeMode)
            {
                if(isRed > 0) _red.ChangeFanLight();
                if(isBlue> 0) _blue.ChangeFanLight();
            }
        }
    }
}