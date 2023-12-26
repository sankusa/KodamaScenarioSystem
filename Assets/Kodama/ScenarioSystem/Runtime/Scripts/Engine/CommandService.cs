using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class CommandService : ICommandService {
        private readonly IPagePlayProcess _pagePlayProcess;
        public IPagePlayProcess PagePlayProcess => _pagePlayProcess;

        private readonly IServiceLocator _serviceLocator;
        public IServiceLocator ServiceLocator => _serviceLocator;

        public CommandService(IPagePlayProcess pagePlayProcess, IServiceLocator serviceLocator) {
            _pagePlayProcess = pagePlayProcess;
            _serviceLocator = serviceLocator;
        }
    }
}