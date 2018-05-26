using System.Collections.Generic;
using System.Linq;
using LogicSpawn.RPGMaker.Core;
using Newtonsoft.Json;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchivementsLog
    {
        public List<Achievement> Achievements ;
        public int TotalScore ;

        public AchivementsLog()
        {
            Achievements = new List<Achievement>();
            TotalScore = 0;
        }

        public void Init()
        {
            var achievementsNodeBank = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank;
            foreach (var achievementTree in achievementsNodeBank.NodeTrees)
            {
                var achievementInfoNode = achievementTree.Nodes.FirstOrDefault(n => n is AchievementStartNode);
                var id = achievementTree.ID;
                var name = (string)achievementInfoNode.ValueOf("Name");
                var description = (string)achievementInfoNode.ValueOf("Description");
                var spritePath = (string)achievementInfoNode.ValueOf("Image");
                var hasProgress = (bool)achievementInfoNode.ValueOf("Has Progress?");
                var achievement = new Achievement(id, name, spritePath, description, 1) {HasProgress = hasProgress};
                if(achievement.HasProgress)
                {
                    var nodeTree = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank.NodeTrees.FirstOrDefault(n => n.ID == achievement.ID);
                    var maxProgressChain = new NodeChain(nodeTree, typeof(AchievementMaxProgress));
                    while (!maxProgressChain.Done)
                    {
                        maxProgressChain.Evaluate();
                    }

                    achievement.Progress.TargetValue = maxProgressChain.IntValue;
                }

                Achievements.Add(achievement);
            }
        }

        [JsonIgnore]
        public List<Achievement> GetAllAchievements
        {
            get
            {
                return Achievements;
            }
        }

        public Achievement GetAchievement(string achievementName)
        {
            return Achievements.First(o => o.Name == achievementName);
        } 
    }
}