//script icon image: ... iconchecked ... .png;

using UnityEngine;
using ExampleThirdPersonShooter.Player;
using ExampleThirdPersonShooter.Player.Modules;


namespace ExampleThirdPersonShooter.Zones
{

    [AddComponentMenu("Zones/Slow Zone")]

    public sealed class SlowZone : ZoneMonoBehaviour
    {
        #region alterable values
        public const string MOD_TAG = "slow-zone";
        #endregion


        #region methods
        private void OnTriggerStay(Collider other)
        {
            if (activated is false) return;

            if (other.TryGetComponent<PlayerCarcass>(out PlayerCarcass player))
            {
                // �������� ��� ������������ � ������ �� ������� ������
                PlayerMovementModule.SpeedModifier[] mods = player.movementModule.GetModifiers();

                // ���� � ������ ��� ���� ��� � ����� MOD_TAG, �� ������ �� ��������.
                for (ushort slot = 0; slot < mods.Length; slot++)
                {
                    if (mods[slot].tag == MOD_TAG)
                    {
                        return;
                    }
                }
                
                // � ��������� ������ ����������� ����� ������ �����
                player.movementModule.AddModifier(0.4f, 96, MOD_TAG);
            }
        }
        #endregion
    }
}
