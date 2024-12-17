using System;
using System.Collections.Generic;
using DefinitionHelper.Definitions;
using DefinitionHelper.Logging;
using Sandbox.ModAPI;
using VRage;
using VRage.Game.Components;
using VRageMath;

namespace DefinitionHelper.DefinitionApi
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    internal class ApiSender : MySessionComponentBase
    {
        private const long DefinitionApiChannel = 8754; // https://xkcd.com/221/
        private const int ApiVersion = 1;

        private readonly IReadOnlyDictionary<string, Delegate> _methods = new DefinitionApiMethods().ModApiMethods;
        private HeartLog _log;

        public override void LoadData()
        {
            _log = new HeartLog();
            MyAPIGateway.Utilities.RegisterMessageHandler(DefinitionApiChannel, ReceiveApiMethods);
            HeartLog.Info("DefinitionHelper: DefinitionAPISender ready.");
            MyAPIGateway.Utilities.SendModMessage(DefinitionApiChannel, new MyTuple<int, IReadOnlyDictionary<string, Delegate>>(ApiVersion, _methods)); // Update mods that loaded before this one
        }

        protected override void UnloadData()
        {
            HeartLog.Info("Closing DefinitionHelper.");
            MyAPIGateway.Utilities.SendModMessage(DefinitionApiChannel, new MyTuple<int, IReadOnlyDictionary<string, Delegate>>(ApiVersion, null));
            MyAPIGateway.Utilities.UnregisterMessageHandler(DefinitionApiChannel, ReceiveApiMethods);
            DefinitionManager.Close();
            _log.Close();
        }

        /// <summary>
        /// Listens for an API request.
        /// </summary>
        /// <param name="data"></param>
        private void ReceiveApiMethods(object data)
        {
            if (!(data is string))
                return;

            MyAPIGateway.Utilities.SendModMessage(DefinitionApiChannel, new MyTuple<int, IReadOnlyDictionary<string, Delegate>>(ApiVersion, _methods));
            HeartLog.Info("DefinitionHelper: DefinitionApiSender send methods.");
        }
    }
}
