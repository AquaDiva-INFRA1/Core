﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace BExIS.ASM.Services
{
    public interface IStatisticsExtractor : IDisposable
    {
        JObject allMetadata_extract();
        JObject metadata_extract(string id);
        JObject datastructure_extract(string datastructure_id);
        JObject allDatastructure_extract();
        JArray annotation_extract(JToken json_variables_, string id);
        JObject allAnnotation_extract(JObject obj);
        JObject reset();
        JObject get_extra_stats();
    }
}
