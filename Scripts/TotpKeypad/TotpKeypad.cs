using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace Sonic853.Udon.Keypad
{
    public class TotpKeypad : KeypadInputField
    {
        [SerializeField] private TOTP Totp;
        protected override void Start()
        {
            if (Totp == null)
            {
                Totp = (TOTP)gameObject.GetComponentInChildren(typeof(UdonBehaviour));
            }
            if (Totp == null
            || Totp.GetUdonTypeName() != "Sonic853.Udon.TOTP")
            {
                Debug.LogError("TotpKeypad: Totp is not set!");
            }
            if (!string.IsNullOrWhiteSpace(Passcode))
            {
                Totp.SetSecret(Passcode);
            }
            base.Start();
        }
        public override string GetPasscode() => Totp.ComputeTotp();
        public override void SetPasscode(string secret) => Totp.SetSecret(secret);
        public override bool VerifyPasscode(string passcode) => Totp.VerifyTotp(passcode);
    }
}
