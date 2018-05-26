using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogicSpawn.RPGMaker.Core;
using LogicSpawn.RPGMaker.Generic;
using UnityEngine;

namespace LogicSpawn.RPGMaker.Core
{
    public class AchievementHandler : MonoBehaviour
    {
        private PlayerSave _playerSave;
        private PlayerSave PlayerSave
        {
            get { return _playerSave ?? (_playerSave = GetObject.PlayerSave); }
        }

        private List<Achievement> Unachieved
        {
            get { return PlayerSave.AchivementsLog.Achievements.Where(achievement => !achievement.IsAchieved).ToList(); }
        }

        private bool checkingAchievements;
        private int achievementToCheck = 0;

        void Update()
        {
            if (!GameMaster.GameLoaded) return;
            if (checkingAchievements) return;
            if(!Unachieved.Any()) return;

            if(achievementToCheck > Unachieved.Count - 1)
            {
                achievementToCheck = 0;
            }

            StartCoroutine(CheckAchievement(Unachieved[achievementToCheck]));
        }

        private IEnumerator CheckAchievement(Achievement achievement)
        {
            checkingAchievements = true;
            bool completed = false;

            //todo: optimise the achievement checking

            var nodeTree = Rm_RPGHandler.Instance.Nodes.AchievementsNodeBank.NodeTrees.FirstOrDefault(n => n.ID == achievement.ID);
            if(nodeTree != null)
            {
                var nodeChain = new NodeChain(nodeTree, typeof (AchievementStartNode));
                while(!nodeChain.Done)
                {
                    nodeChain.Evaluate();
                    yield return null;
                }

                if(achievement.HasProgress)
                {
                    var minProgressChain = new NodeChain(nodeTree, typeof(AchievementMinProgress));
                    while (!minProgressChain.Done)
                    {
                        minProgressChain.Evaluate();
                        yield return null;
                    }

                    achievement.Progress.CurrentValue = minProgressChain.IntValue;
                }
                

                if(nodeChain.CurrentNode is AchievementEndNode)
                {
                    completed = true;
                }
            }

            if(completed) CompleteAchievement(achievement);
            checkingAchievements = false;
        }

        public void CompleteAchievement(Achievement achievement)
        {
            achievement.DateAchieved = DateTime.Now;
            achievement.IsAchieved = true;
            PlayerSave.AchivementsLog.TotalScore += achievement.Score;
            AchievementUI.Instance.ShowAchievement(achievement);
            Debug.Log("Completed Achievement: " + achievement.Name);
        }
    }
}
