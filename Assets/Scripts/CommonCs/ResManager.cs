using UnityEngine;

namespace CommonCs
{
    public class ResManager
    {
        private static ResManager _inst;

        private static ResLoader _loader;

        public static ResManager Inst()
        {
            return _inst ?? (_inst = new ResManager());
        }

        public void Init(GameObject go)
        {
            _loader = go.AddComponent<ResLoader>();
            _loader.Init();
        }
        
        
    }
}
