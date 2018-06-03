using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Generic;
using Newtonsoft.Json;

namespace LogicSpawn.RPGMaker.Core
{
    public class TalentHandler
    {
        public List<Talent> Talents ;

        [JsonIgnore]
        public PlayerCharacter _player;

        [JsonIgnore]
        public PlayerCharacter Player
        {
            get
            {
                return _player ?? (_player = GetObject.PlayerMono.Player);
            }
        }

        public string ClassID ;

        public TalentHandler(string playerClassID)
        {
            Talents = new List<Talent>();
            ClassID = playerClassID;
        }

        public void LoadTalents()
        {
            var loadedTalents = Rm_RPGHandler.Instance.Repositories.Talents.GetTalents(ClassID);
            loadedTalents = loadedTalents.Concat(Rm_RPGHandler.Instance.Repositories.Talents.AllTalents.Where(t => t.AllClasses)).ToList();
            if(loadedTalents.Count > 0)
                Talents.AddRange(loadedTalents);
        }


        public void ApplyTalents()
        {
            if (Player == null) return;

            foreach (var talent in Talents.Where(talent => talent.IsActive))
            {
                var talentEffect = talent.TalentEffect.Effect;
                Player.ApplyPassiveEffect(talentEffect);


                if(talent.TalentEffect.Effect.HasProcEffect)
                {
                    Player.AddProcEffect(talent.TalentEffect.Effect.ProcEffect);
                }
            }
        }


        public void LevelTalent()
        {
            //todo: turn the talent off, level it, then turn it back on
            //this way we can handle enable/disables correctly
        }

        public void Init(PlayerCharacter character)
        {
            _player = character;
        }
    }
}