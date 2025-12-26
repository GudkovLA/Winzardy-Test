using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Controllers;

namespace Game.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class LastHitDamageUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _lastHitDamageQuery = new QueryDescription()
            .WithAll<TransformLink, HealthState>();

        protected override void OnUpdate()
        {
            // TODO: Filter by hit damage component
            World.Query(_lastHitDamageQuery, 
                (ref TransformLink transformLink, ref HealthState  healthState) =>
                {
                    var transform = transformLink.Transform;
                    var damageController = transform.gameObject.GetComponent<DamageController>();
                    if (damageController == null)
                    {
                        return;
                    }
                    
                    damageController.UpdateLastHitDamage(healthState.LastHitTime);
                });
        }
    }
}