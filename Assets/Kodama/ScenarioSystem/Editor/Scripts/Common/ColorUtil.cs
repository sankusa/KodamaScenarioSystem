using System.Collections;
using System.Collections.Generic;
using PlasticPipe.PlasticProtocol.Messages;
using UnityEngine;

namespace Kodama.ScenarioSystem.Editor {
    public static class ColorUtil {
        private static char[] _colorStringRGBA = new char[8];
        private static string _hexDigits = "0123456789ABCDEF";
        public static char[] ToHtmlStringRGBA(Color color) {
            int r = (int)(255 * color.r);
            int g = (int)(255 * color.g);
            int b = (int)(255 * color.b);
            int a = (int)(255 * color.a);

            _colorStringRGBA[0] = _hexDigits[r / 16];
            _colorStringRGBA[1] = _hexDigits[r % 16];
            _colorStringRGBA[2] = _hexDigits[g / 16];
            _colorStringRGBA[3] = _hexDigits[g % 16];
            _colorStringRGBA[4] = _hexDigits[b / 16];
            _colorStringRGBA[5] = _hexDigits[b % 16];
            _colorStringRGBA[6] = _hexDigits[a / 16];
            _colorStringRGBA[7] = _hexDigits[a % 16];
            
            return _colorStringRGBA;
        }
    }
}