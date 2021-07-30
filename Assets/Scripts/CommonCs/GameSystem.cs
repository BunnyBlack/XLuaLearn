using UnityEngine;

namespace CommonCs
{
    public class GameSystem : MonoBehaviour
    {
        private void Start()
        {
            ResManager.Inst().Init(gameObject);
            
            TestLoad();
        }
        
        
        private void TestLoad()
        {
            
        }
        
    }
}
