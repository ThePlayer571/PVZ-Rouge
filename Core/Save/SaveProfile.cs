using System;
using System.Collections.Generic;

namespace TPL.PVZR.Core.Save
{
    
    public class SaveProfile
    {
        public string version = "1.0.0"; // 版本标识
        public DateTime timestamp;
        public Dictionary<string, string> moduleData = new();
    }
}