#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("C9QQD2jiiEMgny1loI4HH7l+ycFhD7B8UduZK6ll2HtMLLyOqvvGJW8vc5uqgwO3KBn8UECNklHPo3UjrV3dWXS57AfLDWhAnIynlNugZ1/5ITO46b5iNQBksnyI9MnMRqzDzSLDxC96mXUZ+YzFdSD0MMDsKDc4GCVtvzVAIASNN1GIUkKhng+lJZpRT0Ft6MKnHYwx4pn/9kuH4kMdZgF6ytdGR7Q1a/TXxsUieHuGgTXcqhibuKqXnJOwHNIcbZebm5ufmpkmyVblaUeDI9xiOeRaN3QmxwRSozidylrNN5YYgz+99msaUXvNrCxFGJuVmqoYm5CYGJubmirFVzz2t5k73wGNSgeiSnP3E6W120E8k+ATBnln57tVDWwzKZiZm5qb");
        private static int[] order = new int[] { 3,4,11,5,10,9,7,7,13,9,11,12,12,13,14 };
        private static int key = 154;

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
