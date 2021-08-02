using UnityEngine;

namespace CommonCs
{
    public class GameSystem : MonoBehaviour
    {
        public static GameSystem Inst;

        public static Transform UIRoot;
        
        private void Start()
        {
            Inst = this;
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
