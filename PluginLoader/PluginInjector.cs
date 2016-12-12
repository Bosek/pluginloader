using Game.Client;
using Game.Ship;
using Game.Ship.Lockstep.ClientState;
using Game.Ship.Lockstep.State;
using System;
using System.Collections.Generic;

namespace PluginLoader
{
    internal delegate void IREvent(object arg = null);
    public class PluginInjector
    {
        internal readonly Game.Framework.Logger Log;

        private readonly Dictionary<Type, Type> clientDevices = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, Type> clientDevicesExterior = new Dictionary<Type, Type>();

        private readonly Dictionary<string, Type> devices = new Dictionary<string, Type>();

        private readonly PluginManager pluginManager;

        public PluginInjector(Game.Framework.Logger log)
        {
            Log = log;
            pluginManager = new PluginManager(this);
        }

        internal event IREvent OnAgosGuiInitialized;
        internal event IREvent OnGameClientActivated;
        internal event IREvent OnGameClientDeactivated;
        internal event IREvent OnGameClientInitialized;
        internal event IREvent OnGameClientUnload;
        internal event IREvent OnGameClientUpdate;
        internal event IREvent OnGameEditorInitialized;
        internal event IREvent OnGameEditorUnload;
        internal event IREvent OnGameEditorUpdate;
        internal event IREvent OnGameMenuInitialized;
        internal event IREvent OnGameServerInitialized;
        internal event IREvent OnGameServerUnload;
        internal event IREvent OnGameServerUpdate;
        public void IRActivateGameClient()
        {
            OnGameClientActivated?.Invoke();
        }

        public ClientDevice IRCreateClientDevice(ClientDevice.InitData initData)
        {
            if (clientDevices.ContainsKey(initData.ODevice.GetType()))
                return (ClientDevice)Activator.CreateInstance(clientDevices[initData.ODevice.GetType()], initData);
            return new ClientDevice(initData);
        }

        public ClientDeviceExterior IRCreateClientDeviceExterior(ClientDevice.InitData initData, CShip ship)
        {
            if (clientDevicesExterior.ContainsKey(initData.ODevice.GetType()))
                return (ClientDeviceExterior)Activator.CreateInstance(clientDevicesExterior[initData.ODevice.GetType()], initData, ship);
            return null;
        }

        public Device IRCreateDevice(string deviceIdent)
        {
            if (this.devices.ContainsKey(deviceIdent))
                return (Device)Activator.CreateInstance(this.devices[deviceIdent]);
            return null;
        }

        public void IRDeactivateGameClient()
        {
            OnGameClientDeactivated?.Invoke();
        }

        public void IRInitializeAgosGui(AgosGui agosGui)
        {
            OnAgosGuiInitialized?.Invoke(agosGui);
        }

        public void IRInitializeGameClient(Game.GameStates.GameClient gameClient)
        {
            OnGameClientInitialized?.Invoke(gameClient);
        }

        public void IRInitializeGameEditor(Game.GameStates.GameShipEditor gameEditor)
        {
            OnGameEditorInitialized?.Invoke(gameEditor);
        }

        public void IRInitializeGameMenu(Game.GameStates.GameMainMenu gameMenu)
        {
            OnGameMenuInitialized?.Invoke(gameMenu);
        }

        public void IRInitializeGameServer(Game.GameStates.GameServer gameServer)
        {
            OnGameServerInitialized?.Invoke(gameServer);
        }

        public void IRUnloadGameClient()
        {
            OnGameClientUnload?.Invoke();
        }

        public void IRUnloadGameEditor()
        {
            OnGameEditorUnload?.Invoke();
        }

        public void IRUnloadGameServer()
        {
            OnGameServerUnload?.Invoke();
        }

        public void IRUpdateGameClient(float dT)
        {
            OnGameClientUpdate?.Invoke(dT);
        }

        public void IRUpdateGameEditor(float dT)
        {
            OnGameEditorUpdate?.Invoke(dT);
        }

        public void IRUpdateGameServer(float dT)
        {
            OnGameServerUpdate?.Invoke(dT);
        }

        internal void AddDevice(object deviceIdent, Type deviceType)
        {
            if (deviceType == typeof(Device))
                devices.Add((string)deviceIdent, deviceType);
            else if (deviceType == typeof(ClientDevice))
                clientDevices.Add((Type)deviceIdent, deviceType);
            else if (deviceType == typeof(ClientDeviceExterior))
                clientDevicesExterior.Add((Type)deviceIdent, deviceType);
        }
    }
}
