using UnityEngine;

public class GameObjectRotation : MonoBehaviour
{
    private float rotateSpeed = 60f;
    private float t = 0f;

    void Update()
    {
        t += Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, t * rotateSpeed);
        if (t * rotateSpeed >= 360) t = 0; //t값이 너무 커지지 않게 조절
    }

    private void OnDestroy()
    {
        Destroy(gameObject);
    }

    private void OnApplicationQuit()
    {
        rotateSpeed = 0;
    }
}
