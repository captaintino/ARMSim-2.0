using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ARMSim_2._0
{
    class Options
    {
        public string fileName = "";
        public uint memorySize = 32768;
        public bool testMode = false;

        public Options(string[] arguments)
        {
            try
            {
                for (int i = 0; i < arguments.Length; i++)
                {
                    switch (arguments[i])
                    {
                        case "--load":
                            fileName = arguments[++i];
                            break;
                        case "--mem":
                            memorySize = Convert.ToUInt32(arguments[++i]);
                            if (memorySize > 104857600) // 100 Megabytes of memory maximum
                                throw new Exception();
                            break;
                        case "--test":
                            testMode = true;
                            break;
                    }
                }
            }
            catch
            {
                fileName = "";
            }
        }
    }
}
