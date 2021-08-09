using CommonCs.ResDownloader;
using CommonCs.ResMgr;
using UnityEngine;

namespace CommonCs
{
    public class GameSystem : MonoBehaviour
    {
        public static GameSystem Inst;
        public GameMode _mode = GameMode.Editor;
        public Transform UIRoot { get; set; }

        private void Awake()
        {
            Inst = this;
            Global.GameMode = _mode;
        }

        private void Start()
        {
            HotFixManager.Inst().Init(gameObject);
            UIRoot = FindObjectOfType<Canvas>().transform;
            ResManager.Inst().Init(gameObject);

            TestLoad();
        }


        private void TestLoad()
        {
            ResManager.Inst().LoadResByName("Text", OnTestComplete);
        }

        private void OnTestComplete(Object obj)
        {
            var go = Instantiate(obj) as GameObject;
            if (go != null)
            {
                go.transform.SetParent(UIRoot);
                go.transform.localPosition = Vector3.zero;

            }
        }
    }
}
