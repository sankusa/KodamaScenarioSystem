using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface ICommandService {
        IScenarioPlayerForCommand Player {get;}
        IServiceLocator ServiceLocator {get;}
    }
}