using UnityEngine;
using Zenject;

namespace PlayerControllers
{
    public class DragMoveController
    {
        private GameObject _highLight;

        [Inject]
        public DragMoveController(GameObject highLight)
        {
            _highLight = highLight;
        }
        
        
    }
}