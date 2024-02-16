using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public interface ICommandService {
        IPagePlayProcess PageProcess {get;}
        IServiceLocator ServiceLocator {get;}
    }
}