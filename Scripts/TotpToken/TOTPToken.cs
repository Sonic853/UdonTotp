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
