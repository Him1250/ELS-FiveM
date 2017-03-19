﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using ELS.Siren;
using Control = CitizenFX.Core.Control;
using CitizenFX.Core.Native;
using System;
using System.Drawing;
using System.Security.Permissions;

namespace ELS
{
    public class ELS : BaseScript
    {
        static PlayerList _playerList;
        public static int localplayerid;
        public static bool isStopped;
        private SirenManager _sirenManager;
        private FileLoader _FileLoader;
        private configuration.ControlConfiguration controlConfiguration;
        private configuration.SoundConfig soundConfig;
        public ELS()
        {
            localplayerid = LocalPlayer.ServerId;
            controlConfiguration = new configuration.ControlConfiguration();
            soundConfig = new configuration.SoundConfig();
            _FileLoader = new FileLoader(this);
            _sirenManager = new SirenManager();
            EventHandlers["onClientResourceStart"] += new Action<string>(
                (string obj) =>
                {
                    _FileLoader.RunLoadeer(obj);
                    handleExistingVehicle();
                });
            EventHandlers["ELS:SirenUpdated"] += new Action<int,string,bool>(_sirenManager.UpdateSirens);
            EventHandlers["onClientResourceStop"] += new Action(() => {
                Debug.WriteLine("Stop");
                isStopped = true;
                _sirenManager.CleanUp();
            });
            EventHandlers["onPlayerJoining"] += new Action(() =>
              {
                  _playerList = Players;
                  localplayerid = LocalPlayer.ServerId;
              });
            Tick += Class1_Tick;
        }

        private void handleExistingVehicle()
        {
            if (LocalPlayer.Character.IsInVehicle())
            {
                _sirenManager.SetCurrentSiren(LocalPlayer.Character.CurrentVehicle);
            }
        }

        private async Task Class1_Tick()
        {
            Text text = new Text("ELS Build v0.0.2\nhttp://ELS.ejb1123.tk", new PointF(640f, 10f), 0.5f);
            text.Alignment = Alignment.Center;
            text.Centered = true;
            text.Draw();
            if (LocalPlayer.Character.IsGettingIntoAVehicle)
            {
                if (LocalPlayer.Character.VehicleTryingToEnter != null)
                {
                    _sirenManager.SetCurrentSiren(LocalPlayer.Character.VehicleTryingToEnter);
                }
            }
            if (LocalPlayer.Character.IsInVehicle() && LocalPlayer.Character.IsSittingInVehicle())
            {
                _sirenManager.runtick();
            }
        }
    }
}

