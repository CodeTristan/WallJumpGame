// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("RHllEKrU8Ol7InfVRPb4D71nu3T9BzkZapX7l8aeDoCGZ8MgKoF99WyeOsn1jPq8MxDT8pixLfl3su7He1YI9MFDq5N2OgScfVXj8NwzvmsxKpSI/XOvgkUrqRuPndjegGpdZXPw/vHBc/D783Pw8PFaPa1Psm7eS+tuRueUjfAe0+VSlC5t83aM7Kc11REz/mikQ/UCkcmAvYqBWo9+Wjx7HyaA8ZDSG0IJEl/4OA8q7+Unqw+fG8EX0OGs3GaH8LnuL885YV+B7dqQWDB2yntX09JdaNuj7xTOBgVawNHtGZHi+VJd944ckl28De0d47qeCJ7lLh9OMVcy6wWghj4heePBc/DTwfz3+Nt3uXcG/PDw8PTx8iwQ/D+43kAxevPy8PHw");
        private static int[] order = new int[] { 8,7,10,3,9,7,12,9,11,9,10,13,12,13,14 };
        private static int key = 241;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
