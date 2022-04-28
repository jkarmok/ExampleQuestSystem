using Application.Quests.Data;
using Application.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests
{
    /// <summary>
    /// Содержит данные о квесте
    /// </summary>
    public class QuestInfo
    {
        public QuestData questData;

        private List<Item> RewardItems = new List<Item>();

        public QuestManager QuestManager;
        public IHumanoidUnit questGiver;
        public QuestInfo(QuestData questData, IHumanoidUnit questGiver, QuestManager questManager)
        {
            this.questGiver = questGiver;
            this.QuestManager = questManager;
            LoadQuest(questData.Clone());
        }

        public bool CheckInteraction(IHumanoidUnit unit)
        {
            if (questGiver == null)
            {
                return true;
            }
            return false;
        }
        private void LoadQuest(QuestData questData)
        {
            this.questData = questData;
            InitKeyValueList();
            questData.Title = Text.FillPattern(questData.Title, QuestKeyValueList);
            questData.Description = Text.FillPattern(questData.Description, QuestKeyValueList);
            for (int i = 0; i < questData.OnCancel.Length; i++)
            {
                questData.OnCancel[i] = Text.FillPattern(questData.OnCancel[i], QuestKeyValueList);
            }
            for (int i = 0; i < questData.OnInit.Length; i++)
            {
                questData.OnInit[i] = Text.FillPattern(questData.OnInit[i], QuestKeyValueList);
            }
            for (int i = 0; i < questData.OnComplite.Length; i++)
            {
                questData.OnComplite[i] = Text.FillPattern(questData.OnComplite[i], QuestKeyValueList);
            }
            for (int i = 0; i < questData.OnFail.Length; i++)
            {
                questData.OnFail[i] = Text.FillPattern(questData.OnFail[i], QuestKeyValueList);
            }
            for (int i = 0; i < questData.OnCreate.Length; i++)
            {
                questData.OnCreate[i] = Text.FillPattern(questData.OnCreate[i], QuestKeyValueList);
            }
        }
        public List<QuestRewardData> GetInstanceRewards()
        {
            return questData.Rewards;
        }
        public List<QuestStageData> GetInstanceStages(List<QuestStageData> stageData)
        {
            foreach (var stage in stageData)
            {
                var stageClone = stage.Clone();
                stageClone.Title = Text.FillPattern(stageClone.Title, QuestKeyValueList);
                stageClone.Description = Text.FillPattern(stageClone.Description, QuestKeyValueList);

                for (int i = 0; i < stageClone.CancelConditions.Length; i++)
                {
                    stageClone.CancelConditions[i] = Text.FillPattern(stageClone.CancelConditions[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.FailConditions.Length; i++)
                {
                    stageClone.FailConditions[i] = Text.FillPattern(stageClone.FailConditions[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.CompliteConditions.Length; i++)
                {
                    stageClone.CompliteConditions[i] = Text.FillPattern(stageClone.CompliteConditions[i], QuestKeyValueList);
                }

                for (int i = 0; i < stageClone.OnCancel.Length; i++)
                {
                    stageClone.OnCancel[i] = Text.FillPattern(stageClone.OnCancel[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.OnInit.Length; i++)
                {
                    stageClone.OnInit[i] = Text.FillPattern(stageClone.OnInit[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.OnComplite.Length; i++)
                {
                    stageClone.OnComplite[i] = Text.FillPattern(stageClone.OnComplite[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.OnFail.Length; i++)
                {
                    stageClone.OnFail[i] = Text.FillPattern(stageClone.OnFail[i], QuestKeyValueList);
                }
                for (int i = 0; i < stageClone.OnCreate.Length; i++)
                {
                    stageClone.OnCreate[i] = Text.FillPattern(stageClone.OnCreate[i], QuestKeyValueList);
                }               
            }
            return stageData;
        }
        public Dictionary<string, string> QuestKeyValueList;
        void InitKeyValueList()
        {
            QuestKeyValueList = new Dictionary<string, string>() {
                { "quest_title", this.questData.Title },
                { "quest_desc", this.questData.Description },
            };
            if (questGiver != null)
                QuestKeyValueList.Add("giver_name", questGiver.CharacterName);
        }


        public static Dictionary<long, QuestData> questDataList = new Dictionary<long, QuestData>();
        public static void Init()
        {
            QuestData[] questList = Resources.LoadAll<QuestData>("");

            questDataList.Clear();
            foreach (var questData in questList)
            {
                questDataList.Add(questData.Id, questData);
            }
        }
 
    }
}
