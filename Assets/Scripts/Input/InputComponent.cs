using System;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace CustomInput
{
    [DisallowMultipleComponent]
    public class InputComponent : GameFrameworkComponent
    {
        private Dictionary<string, int> _intMap;
        private Dictionary<string, Vector2> _vecMap;
        private Dictionary<string, bool> _boolMap;

        protected override void Awake()
        {
            base.Awake();
            _intMap = new Dictionary<string, int>();
            _vecMap = new Dictionary<string, Vector2>();
            _boolMap = new Dictionary<string, bool>();
        }

        private void Update()
        {
            HandleBoolInput();
            HandleVec2Input();
        }

        private void HandleVec2Input()
        {
            float mouseX = Input.GetAxis("Mouse X"); //鼠标移动向量
            float mouseY = Input.GetAxis("Mouse Y");
            _vecMap["CameraRotate"] = new Vector2(mouseX, mouseY);

            float moveX = Input.GetAxis("Horizontal"); //对应WASD四个键
            float moveY = Input.GetAxis("Vertical");
            _vecMap["CameraMove"] = new Vector2(moveX, moveY);
        }

        private void HandleBoolInput()
        {
            //GetKey和GetKeyDown的区别：前者返回键的状态是按下还是松开，后者返回当前帧这个键是否被按下
            _boolMap["CameraAscend"] = Input.GetKey(KeyCode.Space);
            _boolMap["CameraDescend"] = Input.GetKey(KeyCode.LeftControl);
            _boolMap["ChangeMode"] = Input.GetKeyDown(KeyCode.T);
            _boolMap["TogglePowerOn"] = Input.GetKeyDown(KeyCode.P);
            _boolMap["ScreenShot"] = Input.GetKeyDown(KeyCode.O);
            _boolMap["Rotate"] = Input.GetKeyDown(KeyCode.R);
        }

        public int GetInt(string key)
        {
            if (!_intMap.ContainsKey(key)) return 0;
            return _intMap[key];
        }

        public bool GetBool(string key)
        {
            if (!_boolMap.ContainsKey(key)) return false;
            return _boolMap[key];
        }

        public Vector2 GetVec2(string key)
        {
            if (!_vecMap.ContainsKey(key))return Vector2.zero;
            return _vecMap[key];
        }
    }
}