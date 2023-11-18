using HarmonyLib;
//using System.Reflection;
using BepInEx;
using System.Reflection;

namespace DisableCrosshairBZ
{
    [BepInPlugin(myGUID, modName, versionString)]
    public class DisableCrosshairBZ : BaseUnityPlugin
    {
        private const string myGUID = "com.berbac.subnautica.disablecrosshair.mod";
        private const string modName = "DisableCrosshairBZ";
        private const string versionString = "1.5.0";

        private static readonly Harmony harmony = new Harmony(myGUID);

        public void Awake()
     
        {
            //Harmony.CreateAndPatchAll(typeof(CrosshairPatcher));
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            CrosshairMenu.Patch();
        }
    }
}
    