using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Thrzn41.CiscoSpark.Version1
{

    [JsonObject(MemberSerialization.OptIn)]
    public class NoContent : SparkObject
    {

        public NoContent()
        {
            this.HasValues = false;
        }

    }

}
