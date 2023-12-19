using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public class Block {
        /// <summary> ContinueやBreakの対象にしたい場合にする </summary>
        public interface ILoopBlock {}
        public int StartIndex {get; internal set;}
        public int EndIndex {get; internal set;}
    }
}