using UnityEngine;

public class Booster : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out IBoostable boostable))
        {
            boostable.Boost(transform.forward);
        }
    }
}