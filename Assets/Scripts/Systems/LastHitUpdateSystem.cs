#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.Components;
using Game.Controllers;
using Game.DamageSystem.Components;

namespace Game.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class LastHitUpdateSystem : AbstractSystem
    {
        private static readonly QueryDescription _lastHitQuery = new QueryDescription()
            .WithAll<InstanceLink, HealthState>();

        protected override void OnUpdate()
        {
            // TODO: Filter by last hit component
            World.Query(_lastHitQuery, 
                (ref InstanceLink instanceLink, ref HealthState healthState) =>
                {
                    var transform = instanceLink.Instance;
                    var hitAnimator = transform.gameObject.GetComponent<HitAnimator>();
                    if (hitAnimator == null)
                    {
                        return;
                    }
                    
                    hitAnimator.UpdateLastHit(healthState.LastHitTime);
                });
        }
    }
}