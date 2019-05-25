#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("0PoluA8+RDtL9DSlAc+en/Ki0QBBMdPtjW30lFXWIandkJjbGEyo9H22sX1QT5IEcTGk3XAN45rmQta2sOuLT+b1RhZfJHHr7fuu9vnchz1Q0M7fNPDH+ygnJxbVTt+D7y4/fsFCTENzwUJJQcFCQkPYD/bFEz5SNSBPhhLlZ687P99eiVG6MMud4eBYn4qso65CVTywDJKzr3tN01KkCHPBQmFzTkVKacULxbROQkJCRkNAIuITQ1z3SjpGm7LAfe+BKKLrHLMTEBeLinvKCiYsBrDp024D+9BVwMPcsZNkx//eAlxo4WUfyOHb1Qr/hhr/sNbMCxzHBS3tzt/lC3zVYgfCIxIOhimk428OzUydY/Kj24bfkJxDcu99sABzvkFAQkNC");
        private static int[] order = new int[] { 11,13,8,7,10,13,13,7,11,10,10,11,12,13,14 };
        private static int key = 67;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
