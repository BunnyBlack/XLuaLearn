using System;
using UnityEngine;
using Object = UnityEngine.Object;

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

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resName">资源名（不含后缀名）</param>
        /// <param name="callback">回调函数</param>
        public void LoadResByName(string resName, Action<Object> callback)
        {
            _loader.LoadResByName(resName, callback);
        }
    }
}
