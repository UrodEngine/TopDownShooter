using UnityEngine;

public sealed class WeaponBonusSpawnerSystem : BonusSpawnerSystemBase
{    
    public override void GenerateBonusInstance()
    {
        byte randomPoint = (byte)Random.Range(0, bonuses.Length);

        //������ �� ������ ���� ������ 9 � �������
        if (player.weaponsModule.weaponItems.Length > 8)
        {
            return;
        }

        //���������� ��������, ����:
        // 1. ����� ������ ������ �� 0 �� 8.
        // 2. � ������ ������ ���� ������ � ���������
        // 3. ����� � ���� ������ ������ � ��������� ���-�� ������������ ������
        if (player.weaponsModule.currentItem < 9 && player.weaponsModule.weaponItems.Length > 0 && player.weaponsModule.currentItem < player.weaponsModule.weaponItems.Length)
        {
            //���� � ������ � ���� ����� ������, ������� ��� ��� �������� �� �����, ���������� ������������� �� ������ ��������� ������
            if (bonuses[randomPoint].GetComponent<WeaponBonus>().Properties.weapon.name == player.weaponsModule.weaponItems[player.weaponsModule.currentItem].weapon.name)
            {
                GenerateBonusInstance();
                return;
            }
        }


        GameObject instance = Instantiate(bonuses[randomPoint], spawnPoint, Quaternion.identity);

        Destroy(instance, bonusLifetime);
    }
}
