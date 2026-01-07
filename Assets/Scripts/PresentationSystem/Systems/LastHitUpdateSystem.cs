#nullable enable

using Arch.Core;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.DamageSystem.Components;
using Game.PresentationSystem.Components;
using Game.PresentationSystem.Controllers;

namespace Game.PresentationSystem.Systems
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class LastHitUpdateSystem : AbstractSystem
    {
        private readonly QueryDescription _lastHitQuery = new QueryDescription()
            .WithAll<InstanceLink, HealthState, DamageHitTag>();

        protected override void OnUpdate()
        {
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