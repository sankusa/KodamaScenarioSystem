using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public static class UniTaskExtension {
        public static void ForgetAndLogException(this UniTask uniTask, string headerMessage = null) {
            uniTask.Forget(e => {
                if(e is OperationCanceledException) return;
                Debug.LogError(string.IsNullOrEmpty(headerMessage) ? e.ToString() : headerMessage + "\n" + e.ToString());
            });
        }
    }
}