using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CommandService : ICommandService {
        private readonly IPagePlayProcess _pageProcess;
        public IPagePlayProcess PageProcess => _pageProcess;

        private readonly IServiceLocator _serviceLocator;
        public IServiceLocator ServiceLocator => _serviceLocator;

        public CommandService(IPagePlayProcess pageProcess, IServiceLocator serviceLocator) {
            _pageProcess = pageProcess;
            _serviceLocator = serviceLocator;
        }
    }
}