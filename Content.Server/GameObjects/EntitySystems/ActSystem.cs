﻿using System;
using System.Linq;
using JetBrains.Annotations;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Interfaces.GameObjects;
using Content.Shared.GameObjects;

namespace Content.Server.GameObjects.EntitySystems
{
    /// <summary>
    /// This interface gives components behavior on getting destoyed.
    /// </summary>
    public interface IDestroyAct
    {
        /// <summary>
        /// Called when object is destroyed
        /// </summary>
        void OnDestroy(DestructionEventArgs eventArgs);
    }

    public class DestructionEventArgs : EventArgs
    {
        public IEntity Owner { get; set; }
        public bool IsSpawnWreck { get; set; }
    }

    public interface IExAct
    {
        /// <summary>
        /// Called when explosion reaches the entity
        /// </summary>
        void OnExplosion(ExplosionEventArgs eventArgs);
    }

    public class ExplosionEventArgs : EventArgs
    {
        public IEntity Source { get; set; }
        public IEntity Target { get; set; }
        public ExplosionSeverity Severity { get; set; }
    }

    [UsedImplicitly]
    public sealed class ActSystem : EntitySystem
    {
        public void HandleDestruction(IEntity owner, bool isWreck)
        {
            var eventArgs = new DestructionEventArgs
            {
                Owner = owner,
                IsSpawnWreck = isWreck
            };
            var destroyActs = owner.GetAllComponents<IDestroyAct>().ToList();

            foreach (var destroyAct in destroyActs)
            {
                destroyAct.OnDestroy(eventArgs);
            }
            owner.Delete();
        }

        public void HandleExplosion(IEntity source, IEntity target, ExplosionSeverity severity)
        {
            var eventArgs = new ExplosionEventArgs
            {
                Source = source,
                Target = target,
                Severity = severity
            };
            var exActs = target.GetAllComponents<IExAct>().ToList();

            foreach (var exAct in exActs)
            {
                exAct.OnExplosion(eventArgs);
            }
        }
    }
    public enum ExplosionSeverity
    {
        Destruction,
        Heavy,
        Light,
    }
}