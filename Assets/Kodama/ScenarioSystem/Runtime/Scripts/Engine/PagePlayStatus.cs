using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    internal class PagePlayStatus {
        public ScenarioPage CurrentPage {get;}
        public int CurrentCommandIndex {get;set;}
        public CommandBase CurrentCommand => CurrentPage.Commands[CurrentCommandIndex];
        public readonly Stack<Block> BlockStack = new Stack<Block>();
        public PagePlayStatus(ScenarioPage page) {
            CurrentPage = page;
            BlockStack = new Stack<Block>();
        }
    }
}