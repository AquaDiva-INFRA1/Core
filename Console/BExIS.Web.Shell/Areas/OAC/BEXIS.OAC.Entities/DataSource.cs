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
        public const string EBI = "https://www.ebi.ac.uk/ena/data/warehouse/filereport?result=read_run&fields=sample_accession,study_accession&accession=";
        public const string NCBI = "https://www.ebi.ac.uk/ena/data/warehouse/filereport?result=read_run&fields=sample_accession,study_accession&accession=";
    }
}
