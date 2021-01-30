﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using NLog;
using Sync;
using Sync.Behaviour;

namespace CoopFramework
{
    
    /// <summary>
    ///     Base class to extend a type to be managed by the Coop framework.
    /// <code>
    /// </code>
    ///     
    /// </summary>
    public abstract class CoopManaged<TSelf, TExtended> where TExtended : class
    {
        #region Patcher
        /// <summary>
        ///     Enables an automatic injection of this synchronization class into every instance of <see cref="TExtended"/>
        ///     that is being created.
        /// </summary>
        /// <param name="factoryMethod">Factory method that creates an instance of the concrete inheriting class."/></param>
        protected static void EnabledForAllInstances(Func<TExtended, CoopManaged<TSelf, TExtended>> factoryMethod)
        {
            if (m_ConstructorPatch != null || m_DestructorPatch != null)
            {
                throw new Exception($"Constructors for {nameof(TExtended)} are already patched!");
            }
            
            m_ConstructorPatch = new ConstructorPatch<TSelf>(typeof(TExtended)).PostfixAll();
            if (!m_ConstructorPatch.Methods.Any())
            {
                throw new Exception(
                    $"Class {typeof(TExtended)} has no constructor. Cannot wrap instances automatically");
            }
            foreach (MethodAccess methodAccess in m_ConstructorPatch.Methods)
            {
                methodAccess.Postfix.SetGlobalHandler((origin, instance, args) =>
                {
                    OnConstructed(factoryMethod(instance as TExtended));
                });
            }

            m_DestructorPatch = new DestructorPatch<TSelf>(typeof(TExtended)).Prefix();
            if (!m_DestructorPatch.Methods.Any())
            {
                throw new Exception(
                    $"Class {typeof(TExtended)} has no destructor. Cannot unwrap instances automatically");
            }
            foreach (MethodAccess methodAccess in m_DestructorPatch.Methods)
            {
                methodAccess.Prefix.SetGlobalHandler((origin, instance, args) =>
                {
                    var managedInstances = m_ManagedInstances
                        .Where(wrapper => wrapper.Instance.TryGetTarget(out TExtended o) && o == instance)
                        .ToList();
                    foreach (var managedInstance in managedInstances)
                    {
                        managedInstance.OnBeforeFinalize(instance as TExtended);
                        m_ManagedInstances.Remove(managedInstance);
                    }

                    return ECallPropagation.CallOriginal; // Always call the original desctructor!
                });
            }
        }

        /// <summary>
        ///     Enables patching of static functions. Default: Off
        ///
        ///     ATTENTION: Call this method AFTER defining your static patches in your static constructor. Otherwise
        ///     they will not be recognized.
        /// </summary>
        protected static void EnableForStatics()
        {
            foreach (MethodId methodId in PatchedMethods())
            {
                MethodAccess method = MethodRegistry.IdToMethod[methodId];
                CallBehaviourBuilder behaviourBuilderLocal = _callers[EActionOrigin.Local].GetCallBehaviour(methodId);
                CallBehaviourBuilder behaviourBuilderAuth = _callers[EActionOrigin.Authoritative].GetCallBehaviour(methodId);
                method.Prefix.SetGlobalHandler((eOrigin, instance, args) =>
                {
                    if (instance != null)
                    {
                        // Default behaviour: instance methods are handled by the corresponding CoopManaged instance.
                        return ECallPropagation.CallOriginal;
                    }
                    
                    // So it's a static call -> Dispatch it
                    switch (eOrigin)
                    {
                        case EActionOrigin.Local:
                            return StaticDispatch(method, behaviourBuilderLocal, args);
                        case EActionOrigin.Authoritative:
                            return StaticDispatch(method, behaviourBuilderAuth, args);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eOrigin), eOrigin, null);
                    }
                });
            }
        }

        /// <summary>
        ///     Patches a setter of a property on the extended class. Please of the nameof operator instead of raw
        ///     strings if possible. This allows for compile time errors with updated game versions:
        /// 
        ///     <code>Setter(nameof(T.Foo));</code>
        /// </summary>
        /// <param name="sPropertyName"></param>
        /// <returns></returns>
        protected static MethodAccess Setter(string sPropertyName)
        {
            return new PropertyPatch<TSelf>(typeof(TExtended)).InterceptSetter(sPropertyName).Setters.First();
        }
        /// <summary>
        ///     Patches a method on the extended class with a prefix. Please of the nameof operator instead of raw
        ///     strings if possible. This allows for compile time errors with updated game versions:
        /// 
        ///     <code>Method(nameof(T.Foo));</code>
        ///     
        /// </summary>
        /// <param name="sMethodName"></param>
        /// <returns></returns>
        protected static MethodAccess Method(string sMethodName)
        {
            return new MethodPatch<TSelf>(typeof(TExtended)).Intercept(sMethodName).Methods.First();
        }

        /// <summary>
        ///     Sets up a field to be monitored for changes. Please of the nameof operator instead of raw
        ///     strings if possible. This allows for compile time errors with updated game versions:
        ///
        ///     <code>Field(nameof(T.Foo));</code>
        /// </summary>
        /// <param name="sFieldName"></param>
        /// <typeparam name="TField"></typeparam>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        protected static FieldAccess Field<TField>(string sFieldName)
        {
            FieldInfo info = AccessTools.Field(typeof(TExtended), sFieldName);
            if (info == null)
            {
                throw new Exception($"Field {typeof(TExtended)}.{sFieldName} not found.");
            }

            if (info.FieldType != typeof(TField))
            {
                throw new Exception($"Unexpected field type for {typeof(TExtended)}.{sFieldName}. Expected {typeof(TField)}, got {info.FieldType}.");
            }

            return new FieldAccess<TExtended, TField>(info);
        }
        /// <summary>
        ///     Starting point of a patch for any action. The patch will only be active if the context of the call
        ///     is equal to <paramref name="eActionOrigin"/>.
        /// </summary>
        /// <param name="eActionOrigin"></param>
        /// <returns></returns>
        protected static ActionBehaviourBuilder When(EActionOrigin eActionOrigin)
        {
            return _callers[eActionOrigin];
        }
        #endregion

        /// <summary>
        ///     Creates synchronization for a given instance of <typeparamref name="TExtended"/>.
        /// </summary>
        /// <param name="instance">Instance that should be synchronized.</param>
        /// <exception cref="ArgumentOutOfRangeException">When the instance is null.</exception>
        protected CoopManaged([NotNull] TExtended instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            Instance = new WeakReference<TExtended>(instance, true);
            m_GetSync = new Lazy<Func<ISynchronization>>(() =>
            {
                foreach (MethodInfo method in typeof(TSelf).GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic))
                {
                    if (Attribute.IsDefined(method, typeof(SyncFactoryAttribute)) && 
                        method.ReturnType == typeof(ISynchronization))
                    {
                        return () => method.Invoke(this, new object[]{}) as ISynchronization;
                    }
                }

                return m_GetSyncStatic?.Value;
            });

            foreach (MethodId methodId in PatchedMethods())
            {
                MethodAccess method = MethodRegistry.IdToMethod[methodId];
                CallBehaviourBuilder behaviourBuilderLocal = _callers[EActionOrigin.Local].GetCallBehaviour(methodId);
                CallBehaviourBuilder behaviourBuilderAuth = _callers[EActionOrigin.Authoritative].GetCallBehaviour(methodId);
                method.Prefix.SetHandler(instance, (eOrigin, args) =>
                {
                    switch (eOrigin)
                    {
                        case EActionOrigin.Local:
                            return RuntimeDispatch(method, behaviourBuilderLocal, args);
                        case EActionOrigin.Authoritative:
                            return RuntimeDispatch(method, behaviourBuilderAuth, args);
                        default:
                            throw new ArgumentOutOfRangeException(nameof(eOrigin), eOrigin, null);
                    }
                });
            }
        }
        
        /// <summary>
        ///     Returns the synchronized instance.
        /// </summary>
        [NotNull] protected WeakReference<TExtended> Instance { get; set; }

        public static IReadOnlyCollection<CoopManaged<TSelf, TExtended>> ManagedInstances => m_ManagedInstances;

        #region Private
        
        private static void OnConstructed(CoopManaged<TSelf, TExtended> newInstance)
        {
            m_ManagedInstances.Add(newInstance);
        }

        private static ECallPropagation StaticDispatch(MethodAccess methodAccess, CallBehaviourBuilder behaviourBuilder, object[] args)
        {
            ISynchronization sync = m_GetSyncStatic.Value?.Invoke();
            if (behaviourBuilder.DoBroadcast)
            {
                if (sync == null)
                {
                    throw new SynchronizationNotInitializedException("No ISynchronization implementation was provided. Unable to use the synchronization behaviours.");
                }
                sync.Broadcast(methodAccess.Id, null, args);
            }

            if (behaviourBuilder.MethodCallHandlerInstance != null)
            {
                return behaviourBuilder.MethodCallHandlerInstance.Invoke(null,
                    new PendingMethodCall(methodAccess.Id, sync, null, args));
            }
            if (behaviourBuilder.MethodCallHandler != null)
            {
                return behaviourBuilder.MethodCallHandler.Invoke(new PendingMethodCall(methodAccess.Id, sync, null, args));
            }
            return behaviourBuilder.CallPropagationBehaviour;
        }

        private ECallPropagation RuntimeDispatch(MethodAccess methodAccess, CallBehaviourBuilder behaviourBuilder, object[] args)
        {
            if (!Instance.TryGetTarget(out TExtended instance))
            {
                // The instance went out of scope?
                Logger.Warn("Coop synced {Instance} seems to have expired", ToString());
                return ECallPropagation.Suppress; // Will not work anyways
            }

            ISynchronization sync = m_GetSync.Value?.Invoke();
            
            if (behaviourBuilder.DoBroadcast)
            {
                if (sync == null)
                {
                    throw new SynchronizationNotInitializedException("No ISynchronization implementation was provided. Unable to use the synchronization behaviours.");
                }
                sync.Broadcast(methodAccess.Id, instance, args);
            }

            if (behaviourBuilder.MethodCallHandlerInstance != null)
            {
                return behaviourBuilder.MethodCallHandlerInstance.Invoke(this,
                    new PendingMethodCall(methodAccess.Id, sync, instance, args));
            }
            if (behaviourBuilder.MethodCallHandler != null)
            {
                return behaviourBuilder.MethodCallHandler.Invoke(new PendingMethodCall(methodAccess.Id, sync, instance, args));
            }
            return behaviourBuilder.CallPropagationBehaviour;
        }

        private static IEnumerable<MethodId> PatchedMethods()
        {
            HashSet<MethodId> ids = new HashSet<MethodId>();
            foreach (ActionBehaviourBuilder origin in _callers.Values)
            {
                ids.UnionWith(origin.CallBehaviours.Keys);
            }
            return ids;
        }

        private void OnBeforeFinalize(TExtended instance)
        {
            foreach (MethodId methodId in PatchedMethods())
            {
                MethodAccess method = MethodRegistry.IdToMethod[methodId];
                method.Prefix.RemoveHandler(instance);
            }

            Instance = new WeakReference<TExtended>(null);
        }

        private static readonly Dictionary<EActionOrigin, ActionBehaviourBuilder> _callers =
            new Dictionary<EActionOrigin, ActionBehaviourBuilder>()
            {
                {EActionOrigin.Local, new ActionBehaviourBuilder()},
                {EActionOrigin.Authoritative, new ActionBehaviourBuilder()}
            };

        private static readonly List<CoopManaged<TSelf, TExtended>> m_ManagedInstances = new List<CoopManaged<TSelf, TExtended>>();
        private static ConstructorPatch<TSelf> m_ConstructorPatch;
        private static DestructorPatch<TSelf> m_DestructorPatch;
        
        private class PendingMethodCall : IPendingMethodCall
        {
            public PendingMethodCall(MethodId method, [CanBeNull] ISynchronization sync, [CanBeNull] object instance, [NotNull] object[] args)
            {
                m_Method = method;
                m_Sync = sync;
                Instance = instance;
                Parameters = args;
            }
            public void Broadcast()
            {
                if (m_Sync == null)
                {
                    throw new SynchronizationNotInitializedException("No ISynchronization implementation was provided. Unable to use the synchronization behaviours.");
                }
                
                m_Sync.Broadcast(m_Method, Instance, Parameters);
            }

            public object Instance { get; }
            public object[] Parameters { get; }

            [CanBeNull] private readonly ISynchronization m_Sync;

            private readonly MethodId m_Method;
        }
        
        [CanBeNull] private static readonly Lazy<Func<ISynchronization>> m_GetSyncStatic =
            new Lazy<Func<ISynchronization>>(() =>
            {
                foreach (MethodInfo method in typeof(TSelf).GetMethods(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.NonPublic))
                {
                    if (Attribute.IsDefined(method, typeof(SyncFactoryAttribute)) && 
                        method.ReturnType == typeof(ISynchronization))
                    {
                        return () => method.Invoke(null, new object[]{}) as ISynchronization;
                    }
                }

                return null;
            });
        
        [CanBeNull] private readonly Lazy<Func<ISynchronization>> m_GetSync;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        #endregion
    }
}