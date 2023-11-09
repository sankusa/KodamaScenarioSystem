using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem
{
    [Serializable]
    public class ScenarioPage
    {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeReference] private List<CommandBase> _commands = new List<CommandBase>();
        public IList<CommandBase> Commands => _commands;
    }
}