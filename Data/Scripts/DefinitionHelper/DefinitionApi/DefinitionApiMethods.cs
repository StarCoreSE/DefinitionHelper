using System;
using System.Collections.Generic;
using DefinitionHelper.Definitions;
using DefinitionHelper.Logging;

namespace DefinitionHelper.DefinitionApi
{
    /// <summary>
    /// Contains every DefinitionApi method.
    /// </summary>
    internal class DefinitionApiMethods
    {
        internal readonly IReadOnlyDictionary<string, Delegate> ModApiMethods = new Dictionary<string, Delegate>
        {
            // Definitions
            ["RegisterDefinition"] = new Action<string, Type, byte[]>(DefinitionManager.RegisterDefinition),
            ["GetDefinition"] = new Func<string, Type, byte[]>(DefinitionManager.GetDefinition),
            ["GetDefinitionsOfType"] = new Func<Type, string[]>(DefinitionManager.GetDefinitionsOfType),
            ["RemoveDefinition"] = new Action<string, Type>(DefinitionManager.RemoveDefinition),

            // Delegates
            ["RegisterDelegates"] = new Action<string, Type, Dictionary<string, Delegate>>(DefinitionManager.RegisterDelegates),
            ["GetDelegates"] = new Func<string, Type, Dictionary<string, Delegate>>(DefinitionManager.GetDelegates),

            // Actions
            ["RegisterOnUpdate"] = new Action<Type, Action<string, int>>(DefinitionManager.RegisterOnUpdate),
            ["UnregisterOnUpdate"] = new Action<Type, Action<string, int>>(DefinitionManager.UnregisterOnUpdate),

            // Logging
            ["LogDebug"] = new Action<string>(HeartLog.Debug),
            ["LogInfo"] = new Action<string>(HeartLog.Info),
            ["LogException"] = new Action<Exception, Type>(HeartLog.Exception),
        };
    }
}
