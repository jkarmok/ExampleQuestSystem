using Application.Events;
using Application.Quests.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests
{
    public class QuestReward:IEventHandler
    {
        QuestInstance questInstance;
        QuestRewardData rewardData;
        Player player;
        private GameEvent gameEvent;
        public QuestRewardData RewardData { get { return rewardData; } }
        public string GroupName { get { return rewardData.Name; } }
        public QuestReward(QuestInstance questInstance, QuestRewardData rewardData)
        {
            this.rewardData = rewardData;
            this.questInstance = questInstance;
            rewardData.EventName = rewardData.EventName.ToLower().Trim();
            string eventName = rewardData.EventName;
            player = (Player)questInstance.PlayerCharacter;
            InitEvent();
        }
        protected void InitEvent()
        {
            if (!EventManager.GetInstance().Contains(rewardData.EventName))
            {
                gameEvent = new GameEvent(rewardData.EventName, false);
                EventManager.GetInstance().AddEvent(gameEvent);
            }
            else
            {
                gameEvent = EventManager.GetInstance().GetEvent(rewardData.EventName);
            }
        }
 
        public void Action(bool eventState, Transform transform, string param)
        {
            bool check = false;
            Player target = transform.GetComponent<Player>();
            if (target.CharacterId == questInstance.PlayerCharacter.CharacterId)
            {
                check = true;
            }
            else
            {
                if (target != null)
                    if (target.GetPartyBehaviour().ContainsMemberServer(questInstance.PlayerCharacter as Player))
                    {
                        check = true;
                    }
            }
            if (check)
            {
                questInstance.SetRewardGroup(rewardData.Name);
            }
        }
    }
}
