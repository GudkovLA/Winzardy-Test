#nullable enable

using System.Collections.Generic;
using Arch.Core;
using Game.CharacterSystem.Components;
using Game.Common;
using Game.Common.Components;
using Game.Common.Systems;
using Game.Common.Systems.Attributes;
using Game.ProjectileSystem.Components;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.ProjectileSystem.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ProjectileUpdateSystem))]
    public class ProjectileHitDetectionSystem : AbstractSystem
    {
        private static readonly QueryDescription _projectileQuery = new QueryDescription()
            .WithAll<Position, ProjectileState, Fraction>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _contactQuery = new QueryDescription()
            .WithAll<ProjectileContact>()
            .WithNone<Destroy>();

        private static readonly QueryDescription _targetQuery = new QueryDescription()
            .WithAll<Position, ProjectileCollider, Fraction>()
            .WithNone<Destroy, IsDeadTag>();

        protected override void OnUpdate()
        {
            using var _ = ListPool<ProjectileCachedData>.Get(out var projectilesCache);
            World.Query(_projectileQuery,
                (Entity entity, ref Position position, ref ProjectileData projectileData, ref Fraction fraction) =>
                {
                    projectilesCache.Add(new ProjectileCachedData
                    {
                        Entity = entity,
                        Position = position.Value,
                        ProjectileData = projectileData,
                        Fraction = fraction
                    });
                });

            using var __ = ListPool<ContactData>.Get(out var exisingContacts);
            World.Query(_contactQuery,
                (Entity entity, ref ProjectileContact projectileContact) =>
                {
                    projectileContact.ContactPhase = ProjectileContactPhase.Finish;
                    exisingContacts.Add(new ContactData
                    {
                        Entity = entity,
                        ProjectileContact = projectileContact,
                    });
                });

            var commandBuffer = Context.GetOrCreateCommandBuffer(this); 
                
            // TODO: Heavy operation, possible to optimize with burst
            World.Query(_targetQuery, 
                (Entity entity, ref Position position, ref ProjectileCollider projectileCollider, ref Fraction fraction) =>
                {
                    for (var i = projectilesCache.Count - 1; i >= 0; i--)
                    {
                        var projectileData = projectilesCache[i].ProjectileData;
                        var projectileFraction = projectilesCache[i].Fraction;
                        var projectileEntity = projectilesCache[i].Entity;
                        if ((projectileFraction.EnemiesMask & fraction.AlliesMask) == 0)
                        {
                            continue;
                        }
                        
                        var delta = position.Value - projectilesCache[i].Position;
                        delta.y = 0;
                        
                        if (delta.magnitude > projectileData.HitRadius + projectileCollider.Radius)
                        {
                            continue;
                        }

                        var contactData = GetExistingProjectileContact(exisingContacts, entity, projectileEntity);
                        if (contactData != null)
                        {
                            var projectileContact = contactData.Value.ProjectileContact; 
                            projectileContact.ContactPhase = ProjectileContactPhase.Continue;
                            commandBuffer.Set(contactData.Value.Entity, projectileContact);
                            return;
                        }

                        var hitEntity = Context.World.Create();
                        commandBuffer.Add(hitEntity, new ProjectileContact
                        {
                            ProjectileEntity = new EntityHandle(projectileEntity),
                            TargetEntity = new EntityHandle(entity),
                            ContactPhase = ProjectileContactPhase.Start 
                        });

                        if (projectileData.DestroyOnHit)
                        {
                            commandBuffer.Add(projectilesCache[i].Entity, new Destroy());
                            projectilesCache.RemoveAt(i);
                        }
                    }
                });
        }

        private static ContactData? GetExistingProjectileContact(
            List<ContactData> existingContacts, 
            Entity targetEntity,
            Entity projectileEntity)
        {
            for (var i = 0; i < existingContacts.Count; i++)
            {
                if (existingContacts[i].ProjectileContact.TargetEntity.Value == targetEntity
                    && existingContacts[i].ProjectileContact.ProjectileEntity.Value == projectileEntity)
                {
                    return existingContacts[i];
                }
            }
            
            return null;
        }

        private struct ProjectileCachedData
        {
            public Entity Entity;
            public Vector3 Position;
            public ProjectileData ProjectileData;
            public Fraction Fraction;
        }

        private struct ContactData
        {
            public Entity Entity;
            public ProjectileContact ProjectileContact;
        }
    }
}