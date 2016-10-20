using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARSoft.Tools.Net.Test
{
    class TestEnv
    {

        public enum PlatformID
        {
            Win32S = 0,
            Win32Windows = 1,
            Win32NT = 2,
            WinCE = 3,
            Unix = 4,
            // Since NET 3.5 SP1 or silverlight
            Xbox = 5,
            MacOSX = 6,
        }

        public class OperatingSystem
        {
            PlatformID _platform;


            public OperatingSystem()
            {
                _platform = PlatformID.Unix;
            }

            public PlatformID Platform
            {
                get
                {
                    return _platform;
                }
            }
        }


        public class MyEnvironment
        {

            private static OperatingSystem m_OS;

            static MyEnvironment()
            {
                m_OS = new OperatingSystem();
            }
            


            public static OperatingSystem OSVersion
            {
                get
                {
                    return m_OS;
                }
            }

        }

        public static void lol()
        {
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix)
                return;
        }


    }
}
