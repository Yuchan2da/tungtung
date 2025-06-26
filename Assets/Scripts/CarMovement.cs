using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 10f;
    public float destroyZPosition = -15f;

    private void Start()
    {
        Debug.Log($"<color=cyan>자동차 시작됨 - 위치: {transform.position.z}, 파괴 Z: {destroyZPosition}</color>");
    }

    private void Update()
    {
        if (GameManager.Instance.isPaused) return;

        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        if (transform.position.z <= destroyZPosition)
        {
            Debug.Log($"<color=red>자동차 파괴됨 - 위치: {transform.position.z}</color>");
            Destroy(gameObject);
        }
    }
}
