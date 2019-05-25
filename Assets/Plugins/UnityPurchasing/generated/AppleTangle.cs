#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("gZ2U0bKUg4WYl5iSkIWYnp/RsIQ46IMErP8kjq5qA9TyS6R+vKz8AETLXAX+//Fj+kDQ59+FJM38KpPn9ffi86SiwOLB4PfypPX74vuwgYHd0ZKUg4WYl5iSkIWU0YGenZiSiPfB/vfypOzi8PAO9fTB8vDwDsHsx2i93IlGHH1qLQKGagOHI4bBvjDRnpfRhZmU0YWZlJ/RkIGBnZiSkLSP7r2aoWeweDWFk/rhcrB2wntwMZLChgbL9t2nGiv+0P8rS4LovkR+gnCRN+qq+N5jQwm1uQGRyW/kBIaG35CBgZ2U35KenN6QgYGdlJKQceXaIZi2ZYf4DwWafN+xVwa2vI7+bMwC2rjZ6zkPP0RI/yiv7Sc6zNUTGiBGgS7+tBDWOwCciRwWRObmgZ2U0aOenoXRsrDB7+b8wcfBxcNG6kxis9Xj2zb+7Ee8ba+SObpx5ufB5ffypPXy4vywgYGdlNGjnp6F9/Kk7P/15/Xl2iGYtmWH+A8Fmnzbd7l3Bvzw8PT08cGTwPrB+PfypNGysMFz8NPB/Pf423e5dwb88PDwjrBZaQggO5dt1ZrgIVJKFerbMu7fsVcGtryO+a/B7vfypOzS9enB55iXmJKQhZien9GwhIWZnoOYhYjA3sFwMvf52vfw9PT28/PBcEfrcEKVxNLkuuSo7EJlBgdtbz6hSzCpocHg9/Kk9fvi+7CBgZ2U0bifkt/Aeuh4Lwi6nQT2WtPB8xnpzwmh+CLEw8DFwcLHq+b8wsTBw8HIw8DFwczXltF7wpsG/HM+LxpS3giim6qVnZTRuJ+S38DXwdX38qT1+uLssIGoVvT4jeaxp+DvhSJGetLKtlIknvz3+Nt3uXcG/PDw9PTx8nPw8PGtKMeOMHakKFZoSMOzCikkgG+PUKP52vfw9PT28/Dn75mFhYGCy97ehlktj9PEO9QkKP4nmiVT1dLgBlBdn5XRkp6flZiFmJ6fgtGel9GEgpSI0ZCCgoSclILRkJKSlIGFkJ+SlIWYl5iSkIWU0ZOI0ZCfiNGBkIOFk52U0YKFkJ+VkIOV0YWUg5yC0ZDRkJ+V0ZKUg4WYl5iSkIWYnp/RgcFz9UrBc/JSUfLz8PPz8PPB/Pf418HV9/Kk9fri7LCBgZ2U0bKUg4X5r8Fz8OD38qTs0fVz8PnBc/D1wUDBqR2r9cN9mUJ+7C+Ugg6Wr5RN9PHyc/D+8cFz8Pvzc/Dw8RVgWPiDkJKFmJKU0YKFkIWUnJSfhYLfwe5gKu+2oRr0HK+Iddwax1OmvaQdi8Fz8IfB//fypOz+8PAO9fXy8/Bkb4v9VbZ6qiXnxsI6Nf68P+WYIO50cnTqaMy2xgNYarF/3SVAYeMphZmeg5iFiMDnweX38qT18uL8sIFaUoBjtqKkMF7esEIJChKBPBdSvU8FgmofI5X+Ooi+xSlTzwiJDpo5uCmHbsLllFCGZTjc8/Lw8fBSc/DCx6vBk8D6wfj38qT19+LzpKLA4vYdjMhyeqLRIsk1QE5rvvuaDtoNln75RdEGOl3d0Z6BR87wwX1Gsj5z8PH3+Nt3uXcGkpX08MFwA8Hb96OUnZiQn5KU0Z6f0YWZmILRkpSDoVt7JCsVDSH49sZBhITQ");
        private static int[] order = new int[] { 4,19,23,23,44,29,48,41,25,57,55,36,32,22,53,48,59,32,30,19,22,32,31,36,37,31,57,59,41,56,36,52,47,58,35,54,43,40,39,42,45,44,55,58,57,53,46,48,57,58,58,56,53,58,59,58,56,57,59,59,60 };
        private static int key = 241;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
