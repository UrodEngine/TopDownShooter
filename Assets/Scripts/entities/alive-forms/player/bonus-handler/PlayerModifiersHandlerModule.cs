//script icon image: back-dark.png;

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ExampleThirdPersonShooter.Player.Modules
{
    /// <summary> ����� ������ �� ��������. ��� ������� ��������� ������������� ������ <br/>
    /// ������� ����� ������ -> <see cref="PlayerCarcass"/></summary>

    [AddComponentMenu("Player/Module/Bonus Handler Module")]

    public sealed class PlayerModifiersHandlerModule : MonoBehaviour
    {
        #region alterable values
        private PlayerCarcass player;

        [SerializeField] private List<Modifier> modifiers = new List<Modifier>(4);
        #endregion

        #region methods
        private void Start              ()
        {
            player = GetComponent<PlayerCarcass>();
        }
        public  void Update             ()
        {
            for (short slot = 0; slot < modifiers.Count; slot++)
            {
                modifiers[slot].Handle();
            }

            for (short slot = (short)(modifiers.Count - 1); slot >= 0; slot--)
            {
                if (modifiers[slot].lifetime <= 0)
                {
                    modifiers.RemoveAt(slot);
                }
            }
        }
        public  void HandleGettedBonus  (in ModifierBonus.Type _type)
        {
            switch (_type)
            {
                case ModifierBonus.Type.NONE:
                    return;

                case ModifierBonus.Type.SPEEDUP:
                    {
                        player.movementModule.AddModifier(1.5f, 10000);
                    }
                    return;

                case ModifierBonus.Type.INVULNERABILITY:
                    {
                        Modifier mod = new Modifier();

                        mod.OnStart += () => { player.hpModule.lockNonForcedDamage = true; };        
                        mod.OnEnd   += () => { player.hpModule.lockNonForcedDamage = false; };

                        mod.lifetime = 10000;
                        mod.type = _type;
                        mod.Activate();
                        modifiers.Add(mod);

                    }
                    return;

                default:
                    return;
            }
        }
        #endregion


        [System.Serializable]
        public sealed class Modifier
        {
            #region alterable values
            // ��� ������������
            public ModifierBonus.Type   type;

            // ���� ����� ������������
            public ushort               lifetime;

            // ��� �������� ������ �������������� � �������� ������. ������ ��� Update ��� FixedUpdate
            public System.Action        OnStart;
            public System.Action        OnWork;
            public System.Action        OnEnd;
            #endregion


            #region methods
            public void Activate()
            {
                // ������ ���������� ��������
                if (OnStart != null) OnStart();

                // ������ ����� ����� ������������ 
                Task.Run(async () =>
                {
                    try
                    {
                        while (lifetime > 0)
                        {
                            lifetime--;
                            await Task.Delay(1);
                        }
                    }
                    catch (System.Exception exception)
                    {
                        Debug.Log(exception.Message);
                    }
                });
            }
            public void Handle()
            {
                // ���� ����������� ���, �������������� ������� OnWork
                if (lifetime > 0)
                {
                    if (OnWork != null) OnWork();
                }
                // ����� ������� ����������� �������� ������� OnEnd
                else
                {
                    if (OnEnd != null) OnEnd();
                }
            }
            #endregion
        }
    }
}