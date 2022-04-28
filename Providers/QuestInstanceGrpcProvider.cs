
#if UNITY_STANDALONE || UNITY_SERVER
using Application.Quests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WOTABackendAPI.Service;

namespace Application.Quests.Providers
{
    public class QuestInstanceGrpcProvider : IQuestInstanceDbProvider
    {
        QuestService questService;
        public QuestInstanceGrpcProvider(QuestService questService)
        {
            this.questService = questService;
        }
        private WOTABackend.QuestInstanceData GetQuestInstanceData(QuestInstanceData questInstanceDto)
        {
            WOTABackend.QuestInstanceData questInstanceData = new WOTABackend.QuestInstanceData()
            {
                Id = questInstanceDto.Id,
                CompliteTaskDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(questInstanceDto.CompliteTaskDate.ToUniversalTime()),
                Date = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(questInstanceDto.Date.ToUniversalTime()),
                StageCount = questInstanceDto.StageCount,
                Priority = questInstanceDto.Priority,
                QuestId = questInstanceDto.QuestId,
                State = (WOTABackend.State)questInstanceDto.State
            };
            questInstanceData.CreateStagesId.AddRange(questInstanceDto.CreateStagesId);
            questInstanceData.CompliteStagesId.AddRange(questInstanceDto.CompliteStagesId);
            return questInstanceData;
        }
        private QuestInstanceData GetQuestInstanceDto(WOTABackend.QuestInstanceData questInstanceData)
        {
            QuestInstanceData questInstanceDto = new QuestInstanceData()
            {
                Date = questInstanceData.Date.ToDateTime(),
                CompliteStagesId = questInstanceData.CompliteStagesId.ToArray(),
                CompliteTaskDate = questInstanceData.CompliteTaskDate.ToDateTime(),
                StageCount = questInstanceData.StageCount,
                State = (State)questInstanceData.State,
                CreateStagesId = questInstanceData.CreateStagesId.ToArray(),
                Priority = questInstanceData.Priority,
                Id = questInstanceData.Id,
                QuestId = questInstanceData.QuestId
            };
            return questInstanceDto;
        }
        public async Task<List<QuestInstanceData>> GetByCharacterId(long characterId)
        {
            List<QuestInstanceData> questInstances = new List<QuestInstanceData>();
            var reply = await questService.GetByCharacterId(characterId);
            if(reply.Error.Success == false)
            {
                throw new Exception(reply.Error.Message);
            }
            foreach (var questInstanceData in reply.List)
            {
                questInstances.Add(GetQuestInstanceDto(questInstanceData));
            }
            return questInstances;
        }

        public async Task<List<QuestInstanceData>> UpdateByCharacterId(long characterId, List<QuestInstanceData> questsInstance)
        {
            List<QuestInstanceData> questInstances = new List<QuestInstanceData>();
            Debug.Log($"UpdateByCharacterId: {characterId} Count:{questsInstance.Count}");
            try
            { 
                List<WOTABackend.QuestInstanceData> instances = new List<WOTABackend.QuestInstanceData>();
                foreach (var questInstanceData in questsInstance)
                {
                    instances.Add(GetQuestInstanceData(questInstanceData));
                }
                var reply = await questService.UpdateByCharacterId(characterId, instances);
                if (reply.Error.Success == false)
                {
                    throw new Exception(reply.Error.Message);
                }
                foreach (var questInstanceData in reply.List)
                {
                    questInstances.Add(GetQuestInstanceDto(questInstanceData));
                }
            }catch(Exception ex)
            {
                Debug.LogError(ex.ToString());
            }
            return questInstances;
        }
    }
}
#endif