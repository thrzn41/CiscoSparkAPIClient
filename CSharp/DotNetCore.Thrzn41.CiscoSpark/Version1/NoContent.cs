using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    [JsonObject(MemberSerialization.OptIn)]
    public class NoContent : SparkData
    {

        public NoContent()
        {
            this.HasValues = false;
        }

    }

}
