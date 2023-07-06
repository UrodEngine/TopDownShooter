using UnityEngine;

public sealed class BulletMove : MonoBehaviour
{
    #region alterable values
    private Rigidbody   rigbody;
    public  GameObject  owner;

    [SerializeField] private float  speedMultiplier = 1;
    [SerializeField] private byte   damage;

    public float    bulletLifeDistance  = 256;
    public bool     dieOnMousePoint     = false;

    [HideInInspector] public Vector3    startPosition;
    [HideInInspector] public Vector3    bufferedMousePoint;

    [SerializeField] private GameObject createOnDeath;
    #endregion


    private void Start          ()
    {
        rigbody         = GetComponent<Rigidbody>();
        startPosition   = transform.position;

        if (dieOnMousePoint)
        {
            transform.LookAt(bufferedMousePoint);
        }
    }
    private void FixedUpdate    ()
    {
        rigbody.velocity = transform.forward * speedMultiplier;

        // 1. ���� ���� ������� ������ �� ������ �����, ��� �����
        // ���
        // 2. ���� ��������� ������ ��������� ���������, ��� �����
        if (Vector3.Distance(transform.position, Vector3.zero) > 64 || Vector3.Distance(transform.position, startPosition) > bulletLifeDistance)
        {
            Destroy(gameObject);
        }

        // ���� ���� ��������� ������� �� ����� ���� � ��� �� ��� ��������, ��� �����
        if (dieOnMousePoint && Vector3.Distance(transform.position, bufferedMousePoint) < speedMultiplier*0.2f)
        {
            Destroy(gameObject);
        }
    }
    private void OnDestroy      ()
    {
        if (createOnDeath)
        {
            GameObject instance = Instantiate(createOnDeath, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        try
        {
            if (other.TryGetComponent<HPModule>(out HPModule hpModule))
            {
                if (other != owner.gameObject.GetComponent<Collider>())
                {
                    hpModule.Damage((sbyte)damage);
                    Destroy(gameObject);
                }
            }
        }
        catch
        {
            return;
        }
    }
}
