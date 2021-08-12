using UnityEngine;

namespace CommonCs.HotFixMgr
{
    public class HotFixManager
    {
        private static HotFixManager _inst;

        private HotFixHelper helper;

        public static HotFixManager Inst()
        {
            return _inst ?? (_inst = new HotFixManager());
        }

        public void Init(GameObject go)
        {
            helper = go.AddComponent<HotFixHelper>();
            if (helper.IsFirstInstall() && helper.IsWholePackageMode())
            {
                helper.ReleaseStreamingAssets();
            }
            else
            {
                helper.CheckUpdate();
            }
        }
    }
}
