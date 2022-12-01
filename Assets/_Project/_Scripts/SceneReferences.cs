using UnityEngine;

namespace _Project._Scripts
{
    public class SceneReferences : MonoBehaviour
    {
        public VehiclePhysics car;
        public static SceneReferences Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = this;
        }

        public void InitCar(VehiclePhysics vehicle)
        {
            car = vehicle;
        }
    }
}