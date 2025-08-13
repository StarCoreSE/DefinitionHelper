using System;
using System.Collections.Generic;
using System.Linq;
using DefinitionHelper.Logging;

namespace DefinitionHelper.Definitions
{
    public static class DefinitionManager
    {
        internal static Dictionary<Type, Dictionary<string, byte[]>> SerializedDefinitions = new Dictionary<Type, Dictionary<string, byte[]>>();

        internal static Dictionary<Type, Dictionary<string, Dictionary<string, Delegate>>> DefinitionDelegates =
            new Dictionary<Type, Dictionary<string, Dictionary<string, Delegate>>>();

        internal static Dictionary<Type, Action<string, int>> UpdateActions =
            new Dictionary<Type, Action<string, int>>();

        public static void Close()
        {
            SerializedDefinitions.Clear();
            DefinitionDelegates.Clear();
        }

        /// <summary>
        /// Registers a Definition.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="definition"></param>
        /// <exception cref="Exception"></exception>
        public static void RegisterDefinition(string definitionId, Type type, byte[] definition)
        {
            type = GetClosestType(type);

            // Add serialized definition in either case
            if (!SerializedDefinitions.ContainsKey(type))
            {
                SerializedDefinitions.Add(type, new Dictionary<string, byte[]>());
                DefinitionDelegates.Add(type, new Dictionary<string, Dictionary<string, Delegate>>());
            }

            //if (SerializedDefinitions[type].ContainsKey(definitionId))
            //    throw new Exception($"Duplicate DefinitionId {type.Name}::{definitionId}");
            SerializedDefinitions[type][definitionId] = definition;

            if (UpdateActions.ContainsKey(type))
                UpdateActions[type].Invoke(definitionId, (int) UpdateType.NewOrUpdate);

            HeartLog.Info("Registered definition " + definitionId);
        }

        /// <summary>
        /// Registers a Definition's Delegates.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="definition"></param>
        /// <exception cref="Exception"></exception>
        public static void RegisterDelegates(string definitionId, Type type, Dictionary<string, Delegate> delegates)
        {
            type = GetClosestType(type);

            if (!DefinitionDelegates.ContainsKey(type))
                throw new InvalidOperationException($"Delegates cannot be added to type {type.Name} before it has been registered.");

            DefinitionDelegates[type][definitionId] = delegates;

            if (UpdateActions.ContainsKey(type))
                UpdateActions[type].Invoke(definitionId, (int) UpdateType.DelegateUpdate);

            HeartLog.Info($"Registered {delegates?.Count} delegates for definition " + definitionId);
        }

        /// <summary>
        /// Removes a definition and its delegates.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <param name="type"></param>
        public static void RemoveDefinition(string definitionId, Type type)
        {
            type = GetClosestType(type);

            if (!SerializedDefinitions.ContainsKey(type))
                return;
            SerializedDefinitions[type].Remove(definitionId);
            DefinitionDelegates[type].Remove(definitionId);

            if (UpdateActions.ContainsKey(type))
                UpdateActions[type].Invoke(definitionId, (int) UpdateType.Removal);

            HeartLog.Info("Removed definition " + definitionId);
        }

        /// <summary>
        /// Retrieve a Definition of a given type and ID.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static byte[] GetDefinition(string definitionId, Type type)
        {
            type = GetClosestType(type);

            if (!SerializedDefinitions[type].ContainsKey(definitionId))
                throw new Exception($"Invalid DefinitionId {type.FullName}::{definitionId}");
            return SerializedDefinitions[type][definitionId];
        }

        /// <summary>
        /// Retrieve a Definition of a given type and ID.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static Dictionary<string, Delegate> GetDelegates(string definitionId, Type type)
        {
            type = GetClosestType(type);

            if (!DefinitionDelegates[type].ContainsKey(definitionId))
                return null;
            return DefinitionDelegates[type][definitionId];
        }

        /// <summary>
        /// Retrieves all definitions of a given type.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string[] GetDefinitionsOfType(Type type)
        {
            type = GetClosestType(type);

            if (!SerializedDefinitions.ContainsKey(type))
                return Array.Empty<string>();
            return SerializedDefinitions[type].Keys.ToArray();
        }

        public static void RegisterOnUpdate(Type type, Action<string, int> action)
        {
            type = GetClosestType(type);

            if (!UpdateActions.ContainsKey(type))
                UpdateActions[type] = (a, b) => { };
            UpdateActions[type] += action;

            HeartLog.Info("Registered OnUpdate for type " + type.Name);
        }

        public static void UnregisterOnUpdate(Type type, Action<string, int> action)
        {
            type = GetClosestType(type);

            if (!UpdateActions.ContainsKey(type))
                return;
            UpdateActions[type] -= action;

            HeartLog.Info("Unregistered OnUpdate for type " + type.Name);
        }

        private static Type GetClosestType(Type type)
        {
            foreach (var existingType in SerializedDefinitions.Keys)
                if (existingType.Name.Equals(type.Name))
                    return existingType;
            return type;
        }

        private enum UpdateType
        {
            NewOrUpdate = 0,
            Removal = 1,
            DelegateUpdate = 2,
        }

        public static bool HasDefinition(string definitionId, Type type)
        {
            type = GetClosestType(type);

            return SerializedDefinitions.ContainsKey(type) && SerializedDefinitions[type].ContainsKey(definitionId);
        }
    }
}
