using Application.Quests.Conditions;
using Application.Quests.Data;
using Application.Quests.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
 

namespace Application.Quests
{
    /// <summary>
    /// Управляет загрузкой, выдачей и обращением к квестам
    /// </summary>
    public class QuestManager
    {
        public Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public Dictionary<long, QuestInfo> QuestInfoList = new Dictionary<long, QuestInfo>();

        static QuestManager _instance;
        protected QuestManager()
        {
            // conditions
            Add("test_bool_condition", typeof(TestBoolCondition));
            Add("game_event_condition", typeof(GameEventCondition));
            //handlers
            Add("debug_handler", typeof(DebugHandler));
        }
        void Add(string name, Type type)
        {
            name = name.Trim().ToLower();
            Types.Add(name, type);
        }
        public void LoadCache()
        {
            QuestInfo.Init();
        }
        /// <summary>
        /// используется при загрузке нпс
        /// </summary>
        /// <param name="GiverEntity">// чьи квесты загрузим</param>
        public void LoadQuestInfo(Monster GiverEntity)
        {
            foreach (var questData in QuestInfo.questDataList.Values)
            {
                if (questData.GiverEntityId == GiverEntity.CharacterId)
                {
                    if (!QuestInfoList.ContainsKey(questData.Id))
                    {
                        QuestInfoList.Add(questData.Id, new QuestInfo(questData, GiverEntity, this));
                    }
                }
            }
        }
        /// <summary>
        /// используется если берем квест не у нпс
        /// </summary>
        /// <param name="id">ид квеста</param>
        /// <param name="GiverEntity">кого регистрировать как квестодатель</param>
        public void LoadQuestInfo(long id, Monster GiverEntity)
        {
            QuestData questData;
            if (QuestInfo.questDataList.TryGetValue(id, out questData))
            {
                if (!QuestInfoList.ContainsKey(questData.Id))
                {
                    if (GiverEntity != null)
                        questData.GiverEntityId = GiverEntity.CharacterId;
                    QuestInfoList.Add(id, new QuestInfo(questData, GiverEntity, this));
                }
            }
        }


        public static QuestManager GetInstance()
        {
            if (_instance == null)
                _instance = new QuestManager();
            return _instance;
        }

        /// <summary>
        /// Получить условие с параметрами из строки
        /// Example: TestMethod(\"test\",2,3f,4,5,6) - Имя метода и аргументы
        /// </summary>
        /// <param name="InvokeExpression"></param>
        public BaseCondition GetCondition(QuestInstance questInstance, QuestStageInfo stageInfo, string InvokeExpression)
        {
            var invokeData = Parser(InvokeExpression);
            return GetCondition(questInstance, stageInfo, invokeData);
        }

        public BaseCondition GetCondition(QuestInstance questInstance, QuestStageInfo stageInfo, MethodInvokerData invokerData)
        {
            Type type;
            if (Types.TryGetValue(invokerData.Name, out type))
            {
                BaseCondition condition = (BaseCondition)Activator.CreateInstance(type, questInstance, stageInfo, invokerData.Args);
                return condition;
            }
            throw new ArgumentException(string.Format("InvokeCondition: Condition {0} is not found", invokerData.Name));
        }
        /// <summary>
        /// Получить условие с параметрами из строки
        /// Example: TestMethod(\"test\",2,3f,4,5,6) - Имя метода и аргументы
        /// </summary>
        /// <param name="InvokeExpression"></param>
        public BaseHandler GetHandler(QuestInstance questInstance, QuestStageInfo stageInfo, string InvokeExpression)
        {
            var invokeData = Parser(InvokeExpression);
            return GetHandler(questInstance, stageInfo, invokeData);
        }
        public BaseHandler GetHandler(QuestInstance questInstance, QuestStageInfo stageInfo, MethodInvokerData invokerData)
        {
            Type type;
            if (Types.TryGetValue(invokerData.Name, out type))
            {
                BaseHandler baseHandler = (BaseHandler)Activator.CreateInstance(type, questInstance, stageInfo, invokerData.Args);
                return baseHandler;
            }
            throw new ArgumentException(string.Format("InvokerData: Handler {0} is not found", invokerData.Name));
        }

        /// <summary>
        /// Парсит вызовы функций в строках
        /// Example: TestMethod(\"test\",2,3f,4,5,6) - Имя метода и аргументы
        /// </summary>
        /// <param name="InvokeExpression"></param>
        public static MethodInvokerData Parser(string InvokeExpression)
        {
            if (string.IsNullOrWhiteSpace(InvokeExpression))
            {
                throw new ArgumentException("InvokeExpression: IsNullOrWhiteSpace", nameof(InvokeExpression));
            }

            Match reg = Regex.Match(InvokeExpression, @"(?<method>.*?)\((?<args>.*?)\)");
            string args = reg.Groups["args"].Value;
            string method = reg.Groups["method"].Value.Trim().ToLower();

            string[] argsSplitList = new string[0];
            if (!string.IsNullOrWhiteSpace(args))
            {
                argsSplitList = ArgsParser(args).ToArray();
            }

            return new MethodInvokerData()
            {
                Name = method,
                Args = argsSplitList
            };
        }

        public static List<string> ArgsParser(string inputString)
        {
            var itemList = new List<string>();
            var currentIem = "";
            var quotesOpen = false;

            for (int i = 0; i < inputString.Length; i++)
            {
                if (inputString[i] == '"')
                {
                    quotesOpen = !quotesOpen;
                    continue;
                }

                if (inputString[i] == ',' && !quotesOpen)
                {
                    itemList.Add(currentIem);
                    currentIem = "";
                    continue;
                }

                if (currentIem == "" && inputString[i] == ' ') continue;
                currentIem += inputString[i];
            }

            if (currentIem != "") itemList.Add(currentIem);

            return itemList;
        }

        public struct MethodInvokerData
        {
            public string Name;
            public string[] Args;
        }
    }
}
