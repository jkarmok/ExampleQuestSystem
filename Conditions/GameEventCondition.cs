using Application.Events;
using Application.Quests;
using Application.Quests.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Application.Quests.Conditions
{
    public class GameEventCondition : BaseCondition, IEventHandler
    {
        GameEvent _event;
        IHumanoidUnit _playerCharacter;
        public GameEventCondition(QuestInstance questInstance, QuestStageInfo stageInfo, string[] args) : base(questInstance, stageInfo, args)
        {
            if (args == null)
                throw new Exception("Quests.Conditions.GlobalEventCondition condition args is null");
            if (args.Length < 1)
                throw new Exception("Quests.Conditions.GlobalEventCondition condition args.Length < 1");
            string eventName = args[0];
            if (!EventManager.GetInstance().Contains(eventName))
            {
                _event = new GameEvent(eventName, false);
                EventManager.GetInstance().AddEvent(_event);
            }
            _playerCharacter = questInstance.PlayerCharacter;
            _event = EventManager.GetInstance().GetEvent(eventName);
            _event.AddEventHandler(this);
        }

        private bool done;

        public void Action(bool eventState, Transform transform, string param)
        {
            bool check = false;
            /*
            IHumanoidUnit humanoidUnit = transform.GetComponent<IHumanoidUnit>();
            if (humanoidUnit.CharacterId == _playerCharacter.CharacterId)
            {
                check = true;
            }
            else
            {
                // check member unit if needed
            }*/
            check = true;
            if (check)
            {
                done = eventState;
            }
        }

        public override bool Invoke()
        {
            return done;
        }
    }
}
