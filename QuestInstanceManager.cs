
using MadMollyCO.WorldOfAbyss;
using Application.Quests;
using Application.Quests.Data;
using Application.Quests.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests
{
    public class QuestInstanceManager
    {
        private Dictionary<string, QuestInstance> InstanceList = new Dictionary<string, QuestInstance>();
        private IHumanoidUnit player;
        private QuestManager questManager;
        private IQuestInstanceDbProvider questInstanceDbProvider;
        public event Action<QuestRewardData> OnGetReward;
        public QuestInstanceManager(IQuestInstanceDbProvider questInstanceDbProvider, IHumanoidUnit player, QuestManager questManager)
        {
            this.questInstanceDbProvider = questInstanceDbProvider;
            this.player = player;
            this.questManager = questManager;
            Load();

        }

        /// <summary>
        /// Грузит инстансы квестов
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            List<QuestInstanceData> list = await questInstanceDbProvider.GetByCharacterId(player.CharacterId);
 
            foreach (var instanceDate in list)
            {
                await GetInstance(instanceDate.QuestId, instanceDate , false);
            }
        }
 
        public bool TaskStageComplite(long questId, long stageId)
        {
            foreach (var instance in InstanceList.Values)
            {
                if (instance.QuestId == questId)
                {
                    if (instance.InstanceData.State != State.InProcess)
                    {
                        continue;
                    }
                    foreach (var stage in instance.Stages)
                    {
                        if (stage.stageData.Id == stageId)
                        {
                            if (instance.State != State.Complite)
                                continue;
                            return instance.InstanceData.State == State.Complite;
                        }
                    }
                }
            }
            return false;
        }

        public bool TaskInProcess(long questId)
        {
            foreach (var instance in InstanceList.Values)
            {
                if (instance.QuestId == questId)
                {
                    if (instance.State != State.Complite)
                        continue;
                    return instance.InstanceData.State == State.InProcess;
                }
            }
            return false;
        }

        public bool TaskHasComplite(long questId)
        {
            foreach (var instance in InstanceList.Values)
            {
                if (instance.QuestId == questId)
                {
                    if (instance.State != State.Complite)
                        continue;
                    return instance.State == State.Complite;
                }
            }
            return false;
        }
        public List<QuestInstance> GetInstanceListByQuestId(long questId)
        {
            List<QuestInstance> result = new List<QuestInstance>();
            foreach (var instance in InstanceList.Values)
            {
                if (instance.QuestId == questId)
                    result.Add(instance);
            }
            return result;
        }
        public QuestInstance GetInstanceById(string questInstanceId)
        {
            QuestInstance questInstance;
            InstanceList.TryGetValue(questInstanceId, out questInstance);
            return questInstance;
        }

        /// <summary>
        /// Функия проверяет, можно ли взять квест
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public bool CanTakeQuest(long questId)
        {
            foreach (var questInst in GetInstanceListByQuestId(questId))
            {
                if (questInst.InstanceData.State == State.InProcess) // если квест уже в процессе то нельзя
                    return false;
                if (questInst.QuestInfo.questData.TaskType != TaskFilter.dailyTasks && questInst.InstanceData.State == State.Complite) // Если квест уже выполнен, но не является ежедневным
                    return false;
            }
            return true;
        }
        const int MaxQuestCount = 5;

        public async Task<QuestInstance> GetInstance(long questId, QuestInstanceData instanceData, bool isCreate)
        {
            QuestInstance questInstance = null;

            if (!CanTakeQuest(questId))
            {
                string exeptionText = "QuestInstance Exception: QuestInstance already instance " + questId;
                throw new Exception(exeptionText);
            }
            int activeCount = 0;
            foreach (var instance in InstanceList)
            {
                if (instance.Value.State == State.InProcess)
                {
                    activeCount++;
                }
            }
            if (activeCount > MaxQuestCount)
            {
                string exeptionText = "QuestInstance Exception: QuestInstance active instance > max quest count" + questId;
                throw new Exception(exeptionText);
            }


            // сработает если нпс с таким квестом не существует, заинстайтим квест без привязки к нпс
            questManager.LoadQuestInfo(questId, null);
            var questInfo = questManager.QuestInfoList[questId];
            if (questInfo.CheckInteraction(player))
            {
                questInstance = new QuestInstance(player, questInfo, instanceData);
                questInstance.OnUpdate += QuestInstance_OnUpdate;
                questInstance.OnGetReward += QuestInstance_OnGetReward; 
                questInstance.Init(isCreate);
                InstanceList.Add(questInstance.Id, questInstance);
            }

            return questInstance;
        }

        private void QuestInstance_OnGetReward(QuestRewardData obj)
        {
            OnGetReward?.Invoke(obj);
        }

        private void QuestInstance_OnUpdate(QuestInstanceData questInstanceData)
        {
            questInstanceDbProvider.UpdateByCharacterId(player.CharacterId, new List<QuestInstanceData>() { questInstanceData });
        }

        public async Task CancelByQuestId(long QuestId)
        {
            var questList = GetInstanceListByQuestId(QuestId);
            foreach (var instance in questList)
            {
                instance.Cancel();
            }
        }
        private async Task Cancel(string questInstanceId)
        {
            var instance = GetInstanceById(questInstanceId);
            instance.Cancel();

        }
 
        public void Destroy(long QuestId)
        {
            string instanceId = null;
            foreach (var instance in InstanceList.Values)
            {
                if (instance.QuestId == QuestId)
                {
                    instanceId = instance.Id;
                    instance.Destroy();
                }
            }
            InstanceList.Remove(instanceId);
        }
        public void Destroy()
        {
            foreach (var instance in InstanceList)
            {
                instance.Value.Destroy();
            }
            InstanceList.Clear();
        }
        public void InvokeUpdate(float deltaTime)
        {
            foreach (var instance in InstanceList)
            {
                instance.Value.InvokeUpdate(deltaTime);
            }
        }
    }
}
