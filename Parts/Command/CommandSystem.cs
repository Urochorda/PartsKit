using System;
using System.Collections.Generic;
using System.Linq;

namespace PartsKit
{
    [Serializable]
    public class CommandStringData
    {
        public int Id { get; set; }
        public string Factor { get; set; }
    }

    public class CommandSystem
    {
        public static CommandSystem Global { get; } = new CommandSystem(); //默认全局命令系统

        private readonly Dictionary<int, ICommandItem> mIdCommandItems = new Dictionary<int, ICommandItem>();
        private readonly Dictionary<Type, ICommandItem> mTypeCommandItems = new Dictionary<Type, ICommandItem>();

        /// <summary>
        /// 向命令池注册
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Register<T>() where T : ICommandItem, new()
        {
            AddCommand<T>();
        }

        /// <summary>
        /// 根据命令的Type执行命令（可用于代码逻辑的命令）
        /// </summary>
        /// <param name="factor">命令数据</param>
        /// <param name="nullResult">在找不到命令时默认返回结果</param>
        /// <typeparam name="T">命令类型</typeparam>
        /// <typeparam name="TD">命令数据类型</typeparam>
        /// <returns></returns>
        public bool ExecuteByType<T, TD>(TD factor, bool nullResult = false) where T : CommandItem<T, TD>, new()
        {
            return GetCommandByType(out T t) ? t.ExecuteByData(factor) : nullResult;
        }

        /// <summary>
        /// 根据命令的Id执行命令（可用于通过json等配置文件的命令）
        /// </summary>
        /// <param name="id">命令id</param>
        /// <param name="dataStr">命令数据</param>
        /// <param name="nullResult">在找不到命令时默认返回结果</param>
        /// <returns></returns>
        public bool ExecuteById(int id, string dataStr, bool nullResult = false)
        {
            return GetCommandById(id, out ICommandItem e) ? e.ExecuteByString(dataStr) : nullResult;
        }

        /// <summary>
        /// 根据命令的Id列表执行命令（可用于通过json等配置文件的命令）
        /// </summary>
        /// <param name="dataList">命令id和数据列表</param>
        /// <param name="nullResult">在找不到命令时默认返回结果</param>
        /// <returns></returns>
        public bool ExecuteByIds(List<CommandStringData> dataList, bool nullResult = false)
        {
            return dataList == null || dataList.All(item => ExecuteById(item.Id, item.Factor, nullResult));
        }

        private void AddCommand<T>() where T : ICommandItem, new()
        {
            T t = new T();

            mIdCommandItems.Add(t.Id, t);
            mTypeCommandItems.Add(typeof(T), t);
        }

        private bool GetCommandById(int id, out ICommandItem e)
        {
            return mIdCommandItems.TryGetValue(id, out e);
        }

        private bool GetCommandByType<T>(out T t) where T : ICommandItem
        {
            bool has = mTypeCommandItems.TryGetValue(typeof(T), out ICommandItem e);
            t = has ? (T)e : default;
            return has;
        }
    }
}