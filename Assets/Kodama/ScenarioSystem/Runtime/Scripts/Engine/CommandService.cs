using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CommandService : ICommandService {
        private readonly IPlayProcess _playProcess;
        public IPlayProcess PlayProcess => _playProcess;

        private readonly IServiceLocator _serviceLocator;
        public IServiceLocator ServiceLocator => _serviceLocator;

        public CommandService(IPlayProcess playProcess, IServiceLocator serviceLocator) {
            _playProcess = playProcess;
            _serviceLocator = serviceLocator;
        }
    }
}