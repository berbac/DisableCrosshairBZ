using HarmonyLib;

namespace DisableCrosshairBZ
{
    [HarmonyPatch(typeof(uGUI), "Update")]
    public static class CrosshairPatcher
    {
        private static bool _crosshairOff;

        [HarmonyPrefix]
        public static bool Prefix()
        {
            if (_crosshairOff != true && CrosshairMenu.Config.DisableCrosshair)
            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
                return false;
            }
            else if (_crosshairOff && CrosshairMenu.Config.DisableCrosshair)
            {
                return false;
            }

            else if (_crosshairOff != true &&
                ((CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit)))

            {
                HandReticle.main.RequestCrosshairHide();
                _crosshairOff = true;
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

