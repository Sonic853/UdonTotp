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

namespace Sonic853.Udon.Token
{
    public class TOTPToken : UdonSharpBehaviour
    {
        [SerializeField] private TOTP Totp;
        [SerializeField] private Text showNumber;
        [SerializeField] private Text showTime;
        [SerializeField] private Image roundImage;
        private string number;
        private string showTimeText;
        void Start()
        {
            UpdateTotpToken();
        }
        void Update()
        {
            UpdateTotpToken();
        }
        void UpdateTotpToken()
        {
            // 带有两位小数的unix时间戳
            double unixTime = Math.Round((Networking.GetNetworkDateTime().ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds, 2);
            // 根据 Totp.period 计算剩余时间
            float timeLeft = (float)(Totp.period - unixTime % Totp.period);
            showTimeText = ((int)timeLeft).ToString();
            if (showTime != null)
            {
                showTime.text = showTimeText;
            }
            if (roundImage != null)
            {
                roundImage.fillAmount = timeLeft / Totp.period;
            }
            // 每秒更新一次
            if (showTimeText != number)
            {
                number = showTimeText;
                if (showNumber != null)
                {
                    showNumber.text = Totp.ComputeTotp();
                }
            }
        }
    }
}
