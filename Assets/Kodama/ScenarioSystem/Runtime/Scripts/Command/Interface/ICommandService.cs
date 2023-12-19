using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface ICommandService {
        IPlayProcess PlayProcess {get;}
        IServiceLocator ServiceLocator {get;}
    }
}