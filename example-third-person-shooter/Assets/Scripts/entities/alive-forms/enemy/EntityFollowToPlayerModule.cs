using UnityEngine;
using UnityEngine.AI;

using ExampleThirdPersonShooter.Player;

[RequireComponent(typeof(NavMeshAgent))]
[AddComponentMenu("Universal Entity Modules/Follow To Player Module")]
public sealed class EntityFollowToPlayerModule : MonoBehaviour
{
    public static PlayerCarcass player;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        //��������� ���� ��� �� ����� � �� ����� ����������� � �������:
        // 1. ����� ��� ������;
        // 2. ����� ��� �����;
        if (player is null && !PlayerCarcass.isDead)
        {
            player = FindObjectOfType<PlayerCarcass>();
        }
    }
    private void Update()
    {
        if (PlayerCarcass.isDead)
        {
            return;
        }
        if (player != null)
        {
            navMeshAgent.SetDestination(player.transform.position);
        }
    }
}
