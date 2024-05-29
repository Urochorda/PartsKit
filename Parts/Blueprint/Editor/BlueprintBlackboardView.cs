using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PartsKit
{
    public class BlueprintBlackboardView : Blackboard
    {
        public BlueprintView OwnerView { get; private set; }
        public BlueprintBlackboard Blackboard { get; private set; }
        public Func<IEnumerable<BlueprintParameterCreateInfo>> OnGetCreateParameterInfo { get; set; }

        public virtual void Init(BlueprintView ownerViewVal, BlueprintBlackboard blackboardVal)
        {
            Blackboard = blackboardVal;
            OwnerView = ownerViewVal;

            InitView();

            addItemRequested = _ =>
            {
                var parameterType = new GenericMenu();

                foreach (var paramInfo in GetCreateParameterInfo())
                {
                    parameterType.AddItem(new GUIContent(paramInfo.CreateName), false,
                        () => { OwnerView.AddParameter(paramInfo); });
                }

                parameterType.ShowAsContext();
            };
        }

        private void InitView()
        {
            foreach (IBlueprintParameter parameter in Blackboard.Parameters)
            {
                AddField(parameter);
            }
        }

        private IEnumerable<BlueprintParameterCreateInfo> GetCreateParameterInfo()
        {
            IEnumerable<BlueprintParameterCreateInfo> targetInfo = OnGetCreateParameterInfo?.Invoke();
            return targetInfo ?? new List<BlueprintParameterCreateInfo>();
        }

        public void AddField(IBlueprintParameter parameter)
        {
            Type fieldType = BlueprintTypeCache.GetParameterFieldType(parameter.GetType());
            object viewObj = Activator.CreateInstance(fieldType);
            if (viewObj is not BlueprintBlackboardField field)
            {
                return;
            }

            //先加入再初始化
            Add(field);
            field.Init(OwnerView, parameter);
        }

        public void RemoveField(BlueprintBlackboardField field)
        {
            Remove(field);
        }
    }
}