using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace StackBuild
{
    public class InGameLifetimeScope : LifetimeScope
    {
        [SerializeField] private InGameSettings settings;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(settings);
            builder.RegisterInstance(settings.partsMeshList);
            builder.RegisterInstance(settings.spawnRuleList);
        }
    }
}
