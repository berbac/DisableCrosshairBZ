using HarmonyLib;
using QModManager.API.ModLoading;
using System.Reflection;

namespace DisableCrosshairSZ
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "DisableCrosshairSZ");
            CrosshairMenu.Patch();
        }

    }
}
