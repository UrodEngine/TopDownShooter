using UnityEngine;
using System.Threading.Tasks;

public sealed class SpawnEnemiesOutsideCamBounds : MonoBehaviour
{
    #region alterable values
    //----------------------------------------- �������
    [SerializeField]    private Enemy[]         enemies;

    //----------------------------------------- ��������� async �������
    [SerializeField]    private ushort          charge      = 0;
    [SerializeField]    private ushort          interval    = 2000;
                        private const ushort    SUBSTRACT   = 100;

    //----------------------------------------- ���������� ����� async ������ � ������� ������� Update
                        private bool            elapsed     = false;

    //----------------------------------------- ������, �� ������ ������� �������� �������
                        private new Camera      camera;
    #endregion


    #region methods
    private         void        Start           ()
    {
        camera = Camera.main;
        Task.Run(StartLoop);
    }
    private async   Task        StartLoop       ()
    {
        try
        {
            ushort bufferedInterval = interval;

            while (true)
            {
                charge++;
                interval--;

                if (interval <= 0)
                {
                    elapsed = true;
                    interval = bufferedInterval;
                }

                if (charge >= 10000)
                {       
                    charge  = 0;

                    if (bufferedInterval > 500) bufferedInterval -= SUBSTRACT;
                }
                await Task.Delay(1);
            }
        }
        catch
        {
            return;
        }
    }
    private         void        Update          ()
    {
        if (elapsed)
        {
            elapsed = false;
            TrySpawn();
        }
    }
    private         void        TrySpawn        ()
    {
        // 0 = up | 1 = left | 2 = down | 3 = right.
        byte spawnBoundMode = (byte)Random.Range(0, 4);

        // ��� ����� �������� ����� ��� ������ �� ��������� ������.
        RaycastHit spawnRaycastHit = RaycastHitGet(spawnBoundMode);

        // ��� ��������� ������ � ������������ ������ + �������� ����� ������ �� ������ �����
        SpawnEnemy(new Vector3(Mathf.Clamp(spawnRaycastHit.point.x,-20,20), spawnRaycastHit.point.y,Mathf.Clamp(spawnRaycastHit.point.z,-15,15)));
    }
    private         RaycastHit  RaycastHitGet   (in byte mode)
    {
        Vector3 notFormattedPoint = new Vector3(0,0,0);
        switch (mode)
        {
            case 0: // ����� �� ������� �������� ������
                notFormattedPoint = camera.ScreenToWorldPoint(new Vector3(Random.Range(0,camera.pixelWidth), camera.pixelHeight + 1, 0));
                break;
            case 1: // ����� �� ����� �������� ������
                notFormattedPoint = camera.ScreenToWorldPoint(new Vector3(-1, Random.Range(0, camera.pixelHeight), 0));
                break;
            case 2: // ����� �� ������ �������� ������
                notFormattedPoint = camera.ScreenToWorldPoint(new Vector3(Random.Range(0, camera.pixelWidth), -1, 0));
                break;
            case 3: // ����� �� ������ �������� ������
                notFormattedPoint = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth+1, Random.Range(0, camera.pixelHeight), 0));
                break;
        }

        // ������� ��� �� �����
        Physics.Raycast(notFormattedPoint, camera.transform.forward, out RaycastHit hit);
        return hit;
    }
    private         void        SpawnEnemy      (in Vector3 position)
    {
        // ��������� �������, �������� ��� ����
        for (byte slot = 0; slot < enemies.Length; slot++)
        {
            if (enemies[slot].chance > Random.Range(0, 100))
            {
                GameObject instance = Instantiate(enemies[slot].prefab, position, Quaternion.identity);
                return;
            }
        }

        // ���� � ������� �� ���� ������ ����� ������, ������ ���������
        SpawnEnemy(position);
        return;
    }
    #endregion


    [System.Serializable]
    public sealed class Enemy
    {
        public GameObject   prefab;
        public byte         chance;
    }
}
