using System.Collections;
using System.Collections.Generic;
using Kodama.ScenarioSystem;
using UnityEngine;

public class TestCommand : CommandBase
{
    public override void Execute(IScenarioEngine engine)
    {
        Debug.Log(engine.Resolve<Rigidbody2D>().bodyType);
    }
}
