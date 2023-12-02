using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CommandService : ICommandService {
        private readonly IScenarioPlayerForCommand _player;
        public IScenarioPlayerForCommand Player => _player;

        private readonly IServiceLocator _serviceLocator;
        public IServiceLocator ServiceLocator => _serviceLocator;

        public CommandService(IScenarioPlayerForCommand player, IServiceLocator serviceLocator) {
            _player = player;
            _serviceLocator = serviceLocator;
        }
    }
}