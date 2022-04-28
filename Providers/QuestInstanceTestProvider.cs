 
using Application.Quests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_STANDALONE || UNITY_SERVER
using WOTABackendAPI.Service;
#endif

namespace Application.Quests.Providers
{
    public class QuestInstanceTestProvider : IQuestInstanceDbProvider
    {
        public QuestInstanceTestProvider()
        {
            
        }

        public async Task<List<QuestInstanceData>> GetByCharacterId(long characterId)
        {
            List<QuestInstanceData> questInstances = new List<QuestInstanceData>();
            return questInstances;
        }

        public async Task<List<QuestInstanceData>> UpdateByCharacterId(long characterId, List<QuestInstanceData> questsInstance)
        {
            List<QuestInstanceData> questInstances = new List<QuestInstanceData>();
            Debug.Log($"UpdateByCharacterId questsInstance.Count:{questsInstance.Count}");
            return questInstances;
        }
    }
}
