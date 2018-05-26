using System.Linq;
using Assets.Scripts.Beta.NewImplementation;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class DestroyHelper : MonoBehaviour
    {
        private IRPGController _controller;
        private DestroyCondition _condition;
        private object _parameter;
        private bool _initialised;


        public bool ManualTime;
        public float Time;

        void Update()
        {
            if (!_initialised) return;

            if(_condition == DestroyCondition.ActionNotPlaying)
            {
                if(_controller.CurrentAction == null || _controller.CurrentAction.ID != (string)_parameter)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.DoTNotActive)
            {

                if (_controller.Character.CurrentDoTs.FirstOrDefault(d => d.InstanceID == (string)_parameter) == null)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.FriendlyAuraNotAvailable)
            {
                var friendlyAura = _controller.Character.FriendlyAuras.FirstOrDefault(d => d != null && d.AuraEffect.SkillId == (string) _parameter);
                if(friendlyAura == null)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.AuraEffectNotActive)
            {

                if (_controller.Character.AuraEffects.FirstOrDefault(d => d.SkillId == (string)_parameter) == null)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.StatusEffectNotActive)
            {

                if (_controller.Character.StatusEffects.FirstOrDefault(d => d.ID == (string)_parameter) == null)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.TimedPassiveNotActive)
            {

                if (_controller.Character.TimedPassiveEffects.FirstOrDefault(d => d.ID == (string)_parameter) == null)
                {
                    DestroyGameObject();
                }
            }
            else if (_condition == DestroyCondition.GameObjectIsNull)
            {
                if ((_parameter as GameObject) == null)
                {
                    DestroyGameObject();
                }
            }
        }

        private void DestroyGameObject()
        {
            Destroy(gameObject);
        }

        public void Init(DestroyCondition condition, IRPGController controller, object parameter)
        {
            _controller = controller;
            Init(condition, parameter);
        }

        public void Init(DestroyCondition condition, object parameter)
        {

            if(ManualTime)
            {
                _condition = DestroyCondition.Time;
                Destroy(gameObject, Time);
                return;
            }
            
            if (condition == DestroyCondition.Time)
            {
                Destroy(gameObject, (float)parameter);
                return;
            }

            _condition = condition;
            _parameter = parameter;
            _initialised = true;
        }
    }

    public enum DestroyCondition
    {
        ActionNotPlaying,
        Time,
        StatusEffectNotActive,
        AuraEffectNotActive,
        /*        
        public List<Restoration> Restorations ;
        public List<SkillImmunity> SkillMetaImmunitiesID;
        public List<SkillMetaSusceptibility> SkillMetaSusceptibilities;
                 public List<VitalRegenBonus> VitalRegenBonuses;
*/
        DoTNotActive,
        TimedPassiveNotActive,
        GameObjectIsNull,
        FriendlyAuraNotAvailable
    }
}