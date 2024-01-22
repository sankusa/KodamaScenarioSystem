using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kodama.ScenarioSystem {
    public static class Labels {
        public const string Tag_ColorBegin_Red = "<color=#FF4444>";

        public const string Label_DefaultPage = "<Default>";
        public const string Label_DefaultPage_Dark = "<color=grey>" + Labels.Label_DefaultPage + "</color>";

        public const string Label_Null = "<Null>";
        public const string Label_Null_Red = Tag_ColorBegin_Red + Labels.Label_Null + "</color>";

        public const string Label_Empty = "<Empty>";
        public const string Label_Empty_Red = Tag_ColorBegin_Red + Labels.Label_Empty + "</color>";

        public const string Label_Value = "<Value>";

        public const string Label_MissingReference_Red = Tag_ColorBegin_Red + "<Missing Reference></color>";
    }
}