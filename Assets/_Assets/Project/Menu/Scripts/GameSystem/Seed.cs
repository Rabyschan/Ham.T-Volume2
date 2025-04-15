using UnityEngine;
public class Seed : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 450f;
    [SerializeField] GameObject seed;
    public int scoreValue = 0;

    Vector3 _origin;

    private void Awake()
    {
        _origin = transform.eulerAngles;
    }

    private void Update()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y += rotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, _origin.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.isEating = true;
            challangeCondition.Instance.SeedScore();
            GameManager.Instance.AddScore(scoreValue);
            Debug.Log(scoreValue);
            Destroy(gameObject);

            Debug.Log("스코어 증가");
            seed.SetActive(false);
        }   
    }
}
