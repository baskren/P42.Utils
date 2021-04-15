using System;
using System.Collections.Generic;
using System.Text;

namespace P42.Utils.Uno
{
    public static class GC
    {
        public static void Collect()
        {
            System.GC.Collect();
#if __ANDROID__
            Java.Lang.JavaSystem.Gc();
#endif
        }
    }
}
