using HarmonyLib;
using System.Reflection;
using QModManager.API.ModLoading;

namespace DisableCrosshairBZ
{
    [QModCore]
    public static class Loader
    {
        private const string myGUID = "com.berbac.subnautica.disablecrosshair.mod";
        private const string modName = "DisableCrosshairBZ";
        private const string versionString = "1.4.1";

        //private static readonly Harmony harmony = new Harmony(myGUID);

        [QModPatch]
        public static void Initialize()
        {
            Harmony.CreateAndPatchAll(typeof(CrosshairPatcher));
            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            CrosshairMenu.Patch();
        }
    }
}
