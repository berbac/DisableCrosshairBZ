using QModManager.API.ModLoading;

namespace DisableCrosshairSZ
{
    [QModCore]
    public static class Loader
    {
        [QModPatch]
        public static void Initialize()
        {
            
            CrosshairMenu.Patch();


        }

    }
}
