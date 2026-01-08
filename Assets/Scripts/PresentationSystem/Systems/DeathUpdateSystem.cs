#nullable enable

using Arch.Core;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.PresentationSystem.Components;
using Game.PresentationSystem.Controllers;

namespace Game.PresentationSystem.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DeathUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _deathStartQuery = new QueryDescription()
            .WithAll<InstanceLink, DeathState, DamageHitTag>()
            .WithNone<Destroy>();

        protected override void OnUpdate()
        {
            var commandBuffer = GetOrCreateCommandBuffer();
            World.Query(_deathStartQuery, 
                (Entity entity, ref InstanceLink instanceLink, ref DeathState deathState) =>
                {
                    var transform = instanceLink.Instance;
                    var deathAnimator = transform.gameObject.GetComponent<DeathAnimator>();
                    if (deathAnimator == null)
                    {
                        commandBuffer.Add(entity, new Destroy());
                        return;
                    }
                    
                    deathAnimator.SetDeathTime(deathState.DeathTime);
                    deathState.Duration = deathAnimator.Duration;
                });
        }
    }
}