using UnityEngine;

namespace CommonCs.ResDownloader
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
            go.AddComponent<HotFixHelper>();
            helper.Init();
        }
        
        
    }
}
