using UnityEngine;

namespace MapGenerator
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private GameObject highLight;

        private int _x;
        private int _y;

        public void Initialize(int x, int y)
        {
            _x = x;
            _y = y;
        }
        
        private void OnMouseEnter()
        {
            highLight.SetActive(true);
        }
        
        private void OnMouseExit()
        {
            highLight.SetActive(false);
        }

        private void OnMouseDown()
        {
            print($"Tile({_x},{_y})");
        }
    }
}