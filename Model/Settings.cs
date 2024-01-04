using System;
using System.Collections.Generic;

namespace openexchangerates_api.Model
{
    [Serializable]
    public class Settings
    {
        public List<CCode>? Ints { get; set; }
        public Settings() { }
        public Settings(bool def)
        {
            Ints = new List<CCode>()
            {
                CCode.RUB,
                CCode.SDG,
                CCode.SDG,
                CCode.SDG,
                CCode.SDG,
            };
        }
    }
}
