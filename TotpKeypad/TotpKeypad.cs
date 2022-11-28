// -*- coding: utf-8 -*-
/*
MIT License

Copyright (c) 2022-present Sonic853

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Sonic853.Udon.UdonKeypad
{
    public class TotpKeypad : UdonSharpBehaviour
    {
        [SerializeField] private TOTP Totp;
        public string _passcode
        {
            get
            {
                return inputField.text;
            }
            set
            {
                inputField.text = value;
            }
        }
        [Header("是锁定的")]
        [SerializeField] public bool _isLocked = true;
        [Header("自动输入")]
        [SerializeField] public bool _autoEnter = false;
        [Header("显示文字")]
        [SerializeField] private Text Placeholder;
        [Header("输入框")]
        [SerializeField] private InputField inputField;
        [Header("解锁时显示的物体")]
        [SerializeField] public GameObject[] _lockHiedObjects;
        [Header("锁定时显示的物体")]
        [SerializeField] public GameObject[] _lockShowObjects;
        [Header("是否打乱按钮顺序")]
        [SerializeField] public bool _isRandomButton = false;
        [NonSerialized] private GameObject[] Buttons;
        void Start()
        {
            if (Placeholder == null)
            {
                Placeholder = (Text)gameObject.GetComponentInChildren(typeof(Text));
            }
            if (Placeholder == null)
            {
                Debug.LogError("Keypad: Placeholder is not set!");
            }
            if (inputField == null)
            {
                inputField = (InputField)gameObject.GetComponentInChildren(typeof(InputField));
            }
            if (inputField == null)
            {
                Debug.LogError("Keypad: inputField is not set!");
            }
            if (_lockHiedObjects == null)
            {
                Debug.LogError("Keypad: _lockHiedObjects is not set!");
            }
            if (_lockShowObjects == null)
            {
                Debug.LogError("Keypad: _lockShowObjects is not set!");
            }
            // 寻找名为 KeypadButtons 的 GameObject
            var KeypadButtons = gameObject.transform.Find("KeypadButtons");
            if (KeypadButtons == null)
            {
                Debug.LogError("Keypad: KeypadButtons is not set!");
            }
            else
            {
                // 获取 KeypadButtons 下的所有子物体
                Buttons = new GameObject[KeypadButtons.childCount];
                for (int i = 0; i < KeypadButtons.childCount; i++)
                {
                    Buttons[i] = KeypadButtons.GetChild(i).gameObject;
                }
            }
            RandomButton();

            if (_isLocked)
            {
                Lock();
                Placeholder.text = "Locked";
                _passcode = "";
            }
            else
            {
                Unlock();
                Placeholder.text = "Unlocked";
                _passcode = "";
            }
        }
        public bool Lock()
        {
            if (_isLocked)
            {
                return true;
            }
            _isLocked = true;
            Placeholder.text = "Locked";
            _passcode = "";
            foreach (var obj in _lockHiedObjects)
            {
                obj.SetActive(false);
            }
            foreach (var obj in _lockShowObjects)
            {
                obj.SetActive(true);
            }
            return true;
        }
        public bool Unlock()
        {
            if (!_isLocked)
            {
                return true;
            }
            _isLocked = false;
            Placeholder.text = "Unlocked";
            _passcode = "";
            foreach (var obj in _lockHiedObjects)
            {
                obj.SetActive(true);
            }
            foreach (var obj in _lockShowObjects)
            {
                obj.SetActive(false);
            }
            return true;
        }
        public string ButtonPush(string buttonValue)
        {
            switch (buttonValue)
            {
                case "Enter":
                    {
                        if (CheckPasscode())
                        {
                            Unlock();
                            return "Unlocked";
                        }
                        else
                        {
                            if (!_isLocked)
                            {
                                return "Unlocked";
                            }
                            return "Incorrect";
                        }
                    }
                case "Clear":
                    {
                        Placeholder.text = "Cleared";
                        _passcode = "";
                        if (!_isLocked)
                        {
                            Lock();
                        }
                        return "Cleared";
                    }
                default:
                    {
                        if (!_isLocked)
                        {
                            return "Unlocked";
                        }
                        _passcode += buttonValue;
                        if (_autoEnter && Totp.digits == _passcode.Length)
                        {
                            return ButtonPush("Enter");
                        }
                        return _passcode;
                    }
            }
        }
        bool CheckPasscode()
        {
            RandomButton();
            if (Totp.VerifyTotp(_passcode))
            {
                Placeholder.text = "Unlocked";
                _passcode = "";
                return true;
            }
            else
            {
                Placeholder.text = "Incorrect";
                _passcode = "";
                return false;
            }
        }
        public void RandomButton()
        {
            if (!_isRandomButton)
                return;
            if (Buttons.Length <= 0)
            {
                Debug.LogError("Keypad: Buttons is not set!");
                return;
            }
            // 打乱按钮顺序
            for (int i = 0; i < Buttons.Length; i++)
            {
                var temp = Buttons[i];
                var randomIndex = UnityEngine.Random.Range(i, Buttons.Length);
                Buttons[i] = Buttons[randomIndex];
                Buttons[randomIndex] = temp;
            }
            // 重新设置按钮顺序
            for (int i = 0; i < Buttons.Length; i++)
            {
                Buttons[i].transform.SetSiblingIndex(i);
            }
        }
        public string ButtonPush1() => ButtonPush("1");
        public string ButtonPush2() => ButtonPush("2");
        public string ButtonPush3() => ButtonPush("3");
        public string ButtonPush4() => ButtonPush("4");
        public string ButtonPush5() => ButtonPush("5");
        public string ButtonPush6() => ButtonPush("6");
        public string ButtonPush7() => ButtonPush("7");
        public string ButtonPush8() => ButtonPush("8");
        public string ButtonPush9() => ButtonPush("9");
        public string ButtonPush0() => ButtonPush("0");
        public string ButtonPushEnter() => ButtonPush("Enter");
        public string ButtonPushClear() => ButtonPush("Clear");
    }
}
