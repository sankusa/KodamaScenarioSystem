using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    [CreateAssetMenu(fileName = nameof(PageGraph), menuName = nameof(Kodama) + "/" + nameof(ScenarioSystem) + "/" + nameof(PageGraph))]
    public class PageGraph : ScriptableObject {
        public List<SerializableNode> _nodes = new List<SerializableNode>();
    }

    [System.Serializable]
    public class SerializableNode
    {
        public Vector2 position;
        public List<SerializableEdge> edges = new List<SerializableEdge>();
    }

    [System.Serializable]
    public class SerializableEdge
    {
        public SerializableNode toNode;
    }
}