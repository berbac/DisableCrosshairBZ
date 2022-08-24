using HarmonyLib;
using System.IO;
using System.Reflection;

namespace DisableCrosshairBZ
{
    [HarmonyPatch(typeof(GUIHand), "OnUpdate")]
    public static class CrosshairPatcher
    {
        internal static bool _crosshairOff;
        public static void Postfix(GUIHand __instance)
        {
            /*              Targeting.GetTarget(Player.main.gameObject, 50, out var tar, out var num);
                            File.AppendAllText("DisableCrosshairSNLog.txt", "\n __instance.GetActiveTarget() : " + __instance.GetActiveTarget() +
                               "\ntar : "+ tar );*/


            if (Player.main == null) // skip this if no Player.main instance exists
                return;
            
            var activeTarget = __instance.GetActiveTarget();
            //File.AppendAllText("DisableCrosshairSNLog.txt", "\n __instance2.text : " + __instance2.text);

            if (_crosshairOff)
            {
                if ((activeTarget && !Player.main.inSeatruckPilotingChair && !Player.main.inExosuit))// || crosshairHasText)
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    _crosshairOff = false;
                    return;
                }

                else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                    return;

                else if ((!Player.main.inExosuit && !Player.main.inSeatruckPilotingChair) ||
                    (!CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit) ||
                    (!CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair))
                {
                    HandReticle.main.UnrequestCrosshairHide();
                    _crosshairOff = false;
                    return;
                }
                else return;
            }

            else //(!_crosshairOff)
            {
                if ((activeTarget && !(Player.main.inSeatruckPilotingChair || Player.main.inExosuit)))// || crosshairHasText)
                    return;

                else if (CrosshairMenu.Config.NoCrosshairOnFoot && Player.main.currentMountedVehicle == null)
                {
                    HandReticle.main.RequestCrosshairHide();
                    _crosshairOff = true;
                    return;
                }

                else if ((CrosshairMenu.Config.NoCrosshairInSeatruck && Player.main.inSeatruckPilotingChair) ||
                        (CrosshairMenu.Config.NoCrosshairInPrawnSuit && Player.main.inExosuit))
                {
                    HandReticle.main.RequestCrosshairHide();
                    _crosshairOff = true;
                }
            }
        }
    }
}

/*        public static bool Prefix()
        {
            if (_crosshairOff && CrosshairMenu.Config.NoCrosshairOnFoot)
            {
                return true;
            }

            else if (!_crosshairOff && CrosshairMenu.Config.NoCrosshairOnFoot)
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
*/
