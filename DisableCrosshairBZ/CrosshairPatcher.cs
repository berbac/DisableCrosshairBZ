using HarmonyLib;
using System;
using System.IO;

namespace DisableCrosshairBZ
{


    [HarmonyPatch(typeof(uGUI), "Update")]
    public static class CrosshairPatcher
    {

        internal static bool _crosshairOff;

        public static bool Prefix()
        {
            if (_crosshairOff && CrosshairMenu.Config.DisableCrosshairCompletely)
            {
                return true;
            }

            else if (!_crosshairOff && CrosshairMenu.Config.DisableCrosshairCompletely)
            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }

            if (!_crosshairOff &&
                ((CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit)))
            {
                HandReticle.main.RequestCrosshairHide();
                return false;
            }

            else if (_crosshairOff &&
                ((!Player.main.inExosuit && !Player.main.inSeatruckPilotingChair) ||
                (Player.main.inExosuit && !CrosshairMenu.Config.NoCrosshairInPrawnSuit) ||
                (Player.main.inSeatruckPilotingChair && !CrosshairMenu.Config.NoCrosshairInSeatruck)))
            {
                HandReticle.main.UnrequestCrosshairHide();
                _crosshairOff = false;
                return false;
            }

            return true;
        }

    }

}

