using UnityEngine;
using System.Collections.Generic;

namespace ExampleThirdPersonShooter.Zones
{
    [AddComponentMenu("Zones/Tools/Zone Initializer At Start")]
    public sealed class ZoneInitializerAtStart : MonoBehaviour
    {
        [SerializeField] private ZonePacket[] zonePackets;

        private void Update         ()
        {
            // ������ ��������� (�������� / �����������)
            bool isCorruptedGenerate = false;
            // ����� � ������
            List<GameObject> array = new List<GameObject>(8);


            // ��� ���� ���������
            CreateZones(ref array);

            // ��� �������� �� ����������� ���������
            CheckZones(ref array, ref isCorruptedGenerate);

            // ��� ������������� ���������, ���� ��� �������� �����������, ���� ��������� ��������� ����� �������� ���������
            TryAppendZones(ref array, ref isCorruptedGenerate);

            // P.S: ���� ��������� �������, ���� ������������ -> activated = true;
        }

        private void CreateZones    (ref List<GameObject> _list)
        {           
            for (byte packet = 0; packet < zonePackets.Length; packet++)
            {
                for (byte slot = 0; slot < zonePackets[packet].count; slot++)
                {
                    // ������������� ����
                    GameObject zone = Instantiate(zonePackets[packet].zone, new Vector3(
                        Random.Range(-zonePackets[packet].generatorBounds.x / 2, zonePackets[packet].generatorBounds.x / 2),
                        Random.Range(-zonePackets[packet].generatorBounds.y / 2, zonePackets[packet].generatorBounds.y / 2),
                        Random.Range(-zonePackets[packet].generatorBounds.z / 2, zonePackets[packet].generatorBounds.z / 2)), Quaternion.identity);

                    // ������� ����
                    zone.transform.localScale = new Vector3(zonePackets[packet].radius, zonePackets[packet].radius, zonePackets[packet].radius);

                    // �������� � ����� �� ������, ���� ��������� ����� �������� �����������
                    _list.Add(zone);
                }
            }
        }
        private void CheckZones     (ref List<GameObject> _list, ref bool _isCorruptedGenerate)
        {
            for (byte checker = 0; checker < _list.Count; checker++)
            {
                for (byte checking = 0; checking < _list.Count; checking++)
                {
                    if (_list[checker] == _list[checking]) continue;

                    // �������� �����������, ���� ���� ������� ������ � ������
                    if (Vector3.Distance(_list[checker].transform.position, Vector3.zero) < 6)
                    {
                        _isCorruptedGenerate = true;
                        break;
                    }

                    // �������� �����������, ���� ���� ����� � ������ ���� �� 3 �����
                    if (Vector3.Distance(_list[checker].transform.position, _list[checking].transform.position) < 3)
                    {
                        _isCorruptedGenerate = true;
                        break;
                    }
                }
            }
        }
        private void TryAppendZones (ref List<GameObject> _list, ref bool _isCorruptedGenerate)
        {
            // �������� ��������� �����������, �������� �� � ��������� �����
            if (_isCorruptedGenerate)
            {
                for (byte slot = 0; slot < _list.Count; slot++)
                {
                    Destroy(_list[slot]);
                }
                _list.Clear();

                Debug.Log("Generation is corrupted");
            }

            // ��������, ��� ��������� �� ��������� � ��������� ����� � �������� ���������� ���� ���
            else
            {
                // ��������� ��� ��� ���������� ������
                for (byte slot = 0; slot < _list.Count; slot++)
                {
                    _list[slot].GetComponent<ZoneMonoBehaviour>().activated = true;
                }

                Destroy(this);
                _list.Clear();
                return;
            }
        }


        [System.Serializable]
        public sealed class ZonePacket
        {
            public GameObject   zone;
            public byte         count;
            public Vector3      generatorBounds;
            public float        radius;
        }
    }
}

