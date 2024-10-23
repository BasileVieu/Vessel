using UnityEngine;

public class Boat : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position - new Vector3(0.0f, target.localScale.y / 1.5f, 0.0f), 3.0f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, 3.0f * Time.deltaTime);
    }
}