
using System;

namespace SeekU.Domain
{
    /// <summary>
    /// Represents an aggregate root that can create snapshots 
    /// based on runtime criteria (often based on the version)
    /// </summary>
    /// <typeparam name="T">Type of object containing snapshot data</typeparam>
    public abstract class AggregateRootWithSnapshot<T> : AggregateRoot, IAggregateRootWithSnapshot where T : class
    {
        /// <summary>
        /// Override to load state from a snapshot of type T
        /// </summary>
        /// <param name="snapshot">Type of snapshot to load</param>
        public virtual void LoadFromSnapshot(T snapshot)
        {
        }

        /// <summary>
        /// Set's the root's version to the snapshot's version
        /// </summary>
        /// <param name="snapshot">Type of snapshot to load</param>
        internal void InitializeFromSnapshot(Snapshot<T> snapshot)
        {
            Id = snapshot.AggregateRootId;
            Version = snapshot.Version;
        }

        /// <summary>
        /// Creates an object of type T with relevant snapshot data
        /// </summary>
        /// <returns>Instance of type T</returns>
        protected virtual T CreateSnapshot()
        {
            return null;
        }

        /// <summary>
        /// Override to create boolean logic for determining 
        /// when a snapshot should be created
        /// </summary>
        /// <returns>True if a snapshot should be taken; otherwise false</returns>
        protected virtual bool ShouldCreateSnapshot()
        {
            return false;
        }

        /// <summary>
        /// Creates a snapshot object of type T with the hydrated snapshot data
        /// </summary>
        /// <returns>Snapshot of type T cast as an object</returns>
        object IAggregateRootWithSnapshot.CreateGenericSnapshot()
        {
            return new Snapshot<T>(Id, Version, CreateSnapshot());
        } 

        /// <summary>
        /// Gets the generic type T of the snapshot object
        /// </summary>
        /// <returns>Type of snapshot to create</returns>
        Type IAggregateRootWithSnapshot.GetGenericType()
        {
            return typeof (T);
        }
    }
}
