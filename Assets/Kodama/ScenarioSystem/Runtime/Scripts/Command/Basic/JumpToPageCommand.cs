using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    [Serializable]
    public class JumpToPageCommand : AsyncCommandBase {
        [SerializeField] private string _pageName;
        [SerializeField] private bool _return;

        public override async UniTask ExecuteAsync(ICommandService service, CancellationToken cancellationToken) {
            try{
            if(_return) {
                await ScenarioPlayLoop.PlayAsync("", _pageName, service.ServiceLocator, null, service.PagePlayProcess as PagePlayProcess, cancellationToken);
            }
            else {
                service.PagePlayProcess.SubsequentPageName = _pageName;
                service.PagePlayProcess.JumpToEndIndex();
            }
            }
            catch(Exception e) {
                Debug.Log(e);
                throw;
            }
        }
    }
}