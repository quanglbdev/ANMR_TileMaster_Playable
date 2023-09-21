#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("vz5W4lQKPIJmvYET0Gt98WlQa7K7NKP6AQAOnAW/LxggetsyA9VsGC5vYGoubWt8emdoZ21vemdhYC5+1zhxz4lb16mXtzxM9dbbf5Bwr1xca2Jnb2Btay5hYC56Zmd9Lm1rfCBOqPlJQ3EGUD4RCA1bEy0KFj4YLmFoLnpmay56ZmtgLm9+fmJnbW9pgQa6LvnFoiIuYX64MQ8+grlNwQYlCA8LCwkMDxgQZnp6fn00ISF5CD4BCA1bEx0PD/EKCz4NDw/xPhNsYmsufXpvYGpvfGouemt8Y30ubwsODYwPAQ4+jA8EDIwPDw7qn6cHKD4qCA1bCgUdE09+fmJrLk1rfHo4l0IjdrnjgpXS/XmV/HjceT5Bz2o7LRtFG1cTvZr5+JKQwV60z1ZeemZhfGd6dz8YPhoIDVsKDR0DT35+YmsuXGFhei5NTz4QGQM+OD46PFepCwdyGU5YHxB63bmFLTVJrdthcU+mlvffxGiSKmUf3q216hUkzREYPhoIDVsKDR0DT35+YmsuXGFhej4fCA1bCgQdBE9+fmJrLkdgbSA/jA8OCAckiEaI+W1qCw8+j/w+JAh8b216Z21rLn16b3prY2tgen0gPgMIBySIRoj5Aw8PCwsODYwPDw5Sdy5vfX17Y2t9Lm9tbWt+em9gbWuBfY9uyBVVByGcvPZKRv5uNpAb+44aJd5nSZp4B/D6ZYMgTqj5SUNxZ2hnbW96Z2FgLk97emZhfGd6dz9LcBFCZV6YT4fKemwFHo1PiT2Ej4UXh9D3RWL7CaUsPgzmFjD2XgfdEYuNixWXM0k5/KeVToAi2r+eHNZ0PowPeD4ACA1bEwEPD/EKCg0MD8cXfPtTANtxUZX8Kw20W4FDUwP/IT6PzQgGJQgPCwsJDAw+j7gUj70IDVsTAAoYChol3mdJmngH8PplgwZQPowPHwgNWxMuCowPBj6MDwo+ptJwLDvEK9vXAdhl2qwqLR/5r6KlrX+cSV1bz6EhT7329e1+w+itQpuQdAKqSYVV2hg5PcXKAUPAGmffMyhpLoQ9ZPkDjMHQ5a0h911kVWq5FbOdTCocJMkBE7hDklBtxkWOGbD6fZXg3GoBxXdBOtasMPd28WXGeXkgb35+YmsgbWFjIW9+fmJrbW9iay5HYG0gPyg+KggNWwoFHRNPfs5tPXn5NAkiWOXUAS8A1LR9F0G7YGoubWFgamd6Z2FgfS5haC57fWt6Z2hnbW96ay5sdy5vYHcufm98ej04VD5sPwU+BwgNWwoIHQxbXT8dEZ/VEEle5QvjUHeKI+U4rFlCW+JH1niRPRprr3maxyMMDQ8OD62MD35iay5Na3x6Z2hnbW96Z2FgLk97Lk1PPowPLD4DCAckiEaI+QMPDw87PD86Pj04VBkDPTs+PD43PD86Pirs5d+5ftEBS+8pxP9jduPpuxkZCggdDFtdPx0+HwgNWwoEHQRPfn4J4nM3jYVdLt02yr+xlEEEZfEl8j6MCrU+jA2trg0MDwwMDww+AwgHIi5ta3x6Z2hnbW96ay5+YWJnbXckiEaI+QMPDwsLDj5sPwU+BwgNWwGTM/0lRyYUxvDAu7cA11AS2MUzXqSE29Tq8t4HCTm+e3sv");
        private static int[] order = new int[] { 26,23,45,58,38,31,41,21,35,29,42,14,42,48,20,31,41,52,52,42,29,34,46,47,40,55,32,29,55,41,49,55,42,37,49,37,54,40,52,56,59,50,54,58,57,58,56,59,55,50,52,55,52,53,58,55,58,57,59,59,60 };
        private static int key = 14;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
            //return null;
        }
    }
}
#endif
