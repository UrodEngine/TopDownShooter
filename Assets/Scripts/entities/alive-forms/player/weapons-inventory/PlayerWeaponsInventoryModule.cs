//script icon image: back-dark.png;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ExampleThirdPersonShooter.Player.Modules
{
    /// <summary> ����� ������ �� ��������. ��� ������� �������� � ��������� ������ <br/>
    /// ������� ����� ������ -> <see cref="PlayerCarcass"/></summary>
    
    [AddComponentMenu("Player/Module/Weapons Inventory Module")]

    public sealed class PlayerWeaponsInventoryModule : MonoBehaviour
    {
        #region alterable values
        private PlayerCarcass player;

        [SerializeField] private GameObject startBonus;
        [SerializeField] private Transform  weaponTransform;

        [SerializeField] private ushort                 currentSlot = ushort.MaxValue;
        [SerializeField] private List<WeaponProperties> weapons     = new List<WeaponProperties>(4);

        public WeaponProperties[] weaponItems { get => weapons.ToArray(); }
        public ushort currentItem { get => currentSlot; }
        #endregion

        #region methods
        private void        Start           ()
        {
            if (startBonus)
            {
                player                      = GetComponent<PlayerCarcass>();
                GameObject startBonusInst   = Instantiate(startBonus, transform.position, Quaternion.identity);
                startBonus = null;
            }
        }
        private void        Update          ()
        {
            SelectWeapon();
            TryShoot();

            // ���� � ���� ������ ��������, �� ������ �� ��������, � ��� ���� ���� ���
            if (currentSlot < weapons.Count && weapons[currentSlot] != null && weaponTransform.childCount == 0)
            {
                AttachModel();
            }
        }
        public  void        GetWeapon       (in WeaponProperties _weapon)
        {
            weapons.Add(new WeaponProperties()
            {
                weapon              = _weapon.weapon,
                ammo                = _weapon.ammo,
                interval            = _weapon.interval,
                bullet              = _weapon.bullet,
                bulletPerShoot      = _weapon.bulletPerShoot,
                bulletLifeDistance  = _weapon.bulletLifeDistance,
                dieOnMousePoint     = _weapon.dieOnMousePoint,
            });
        }
        private void        SelectWeapon    ()
        {
            if (Input.inputString != "")
            {
                try
                {
                    // ���� ������������� �������� ������� � �������� �������� �� 0 �� 9
                    ushort keycode = ushort.Parse(Input.inputString);

                    // ���� ������� �� ���� ������ ������, �� ���� � ���� ��������
                    if (keycode != currentSlot + 1)
                    {
                        currentSlot = (ushort)(keycode - 1);

                        ClearHand();
                        AttachModel();
                    }
                }
                catch
                {
                    return;
                }
            }
        }
        private void        TryShoot        ()
        {
            if (Input.GetMouseButton(InputConstants.SHOOT_MOUSE))
            {
                if (currentSlot < weapons.Count && weapons[currentSlot] != null && weapons[currentSlot].currentInverval <= 0 && weapons[currentSlot].ammo > 0)
                {
                    // ������� ��������� ����� ������
                    weapons[currentSlot].ammo--;
                    weapons[currentSlot].currentInverval        = weapons[currentSlot].interval;
                    weapons[currentSlot].bufferedMousePoint     = player.rotationModule.lastRaycastPointer;

                    // ������� ����
                    // (�����. �������, �����������, �������� ����, ��������� ������� ����)
                    weapons[currentSlot].Shoot(weaponTransform.position, transform.rotation, this.gameObject);

                    // ����������� ����� ������, ���� �������� ������ ���. + ������������ ����
                    if (weapons[currentSlot].ammo <= 0)
                    {
                        weapons.RemoveAt(currentSlot);
                        currentSlot = ushort.MaxValue;

                        ClearHand();

                        return;
                    }

                    // ���� ���� ������ ��� ��� ����� ������, ��� ����� ����� ��������
                    Task.Run(IntervalCreate);
                }
            }
        }
        private async Task  IntervalCreate  ()
        {
            try
            {
                // ���� �������� ��������������� �������� �����, ����� �� �������� �������� ������ ������ �� ������, ���� ����� ��� ������ � ����
                ushort bufferedSlotID = currentSlot;

                // do magic
                while (weapons[bufferedSlotID].currentInverval > 0)
                {
                    await Task.Delay(1);
                    weapons[bufferedSlotID].currentInverval--;

                    if (weapons[bufferedSlotID].currentInverval > weapons[bufferedSlotID].interval)
                    {
                        weapons[bufferedSlotID].currentInverval = weapons[bufferedSlotID].interval;
                    }
                }
            }
            catch
            {
                // �� ������ MissingReferenceException ��� ����� ������ ����� ������ �� ������
                Debug.Log("Weapon has been destroyed or reference is invalid");
            }
        }
        private void        ClearHand       ()
        {
            for (ushort slot = 0; slot < weaponTransform.childCount; slot++) { Destroy(weaponTransform.GetChild(slot).gameObject); }
        }
        private void        AttachModel     ()
        {
            //�������� � Transform � ���� ������
            GameObject weaponObj                = Instantiate(weapons[currentSlot].weapon, weaponTransform);
            weaponObj.transform.localPosition   = new Vector3(0, 0, 0);
            weaponObj.transform.localScale      = new Vector3(1, 1, 1);
        }
        #endregion
    }
}