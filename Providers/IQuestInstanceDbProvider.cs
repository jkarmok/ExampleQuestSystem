using Application.Quests.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Quests.Providers
{
    public interface IQuestInstanceDbProvider
    {
        Task<List<QuestInstanceData>> GetByCharacterId(long characterId);
        Task<List<QuestInstanceData>> UpdateByCharacterId(long characterId, List<QuestInstanceData> questsInstance);
    }
}
