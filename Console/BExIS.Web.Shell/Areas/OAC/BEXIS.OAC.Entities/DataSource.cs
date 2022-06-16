using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEXIS.OAC.Entities
{
    public enum DataSource :Int32
    {
        BioGPS = 1,
        EBI = 2,
        NCBI = 3
    }

    public static class PortalSource
    {
        public const string BioGPS = "http://biogps.org/dataset/";
        public const string EBI = "https://www.ebi.ac.uk/ena/portal/api/filereport?fields=sample_accession%2Cstudy_accession&result=read_run&accession=";
        public const string NCBI = "https://www.ebi.ac.uk/ena/portal/api/filereport?fields=sample_accession%2Cstudy_accession&result=read_run&accession=";
    }
}
