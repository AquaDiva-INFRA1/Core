using BExIS.Utils.Models;
using System.Collections.Generic;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Security.Entities.Subjects;

namespace BExIS.Ddm.Api
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>        
    public interface ISearchDesigner
    {
        List<SearchAttribute> Get();
        void Set(List<SearchAttribute> SearchAttributeList);
        void Set(List<SearchAttribute> SearchAttributeList, bool includePrimaryData);

        void Reset();
        void Reload();

        void Dispose();

        List<SearchMetadataNode> GetMetadataNodes();
        List<Variable> GetVariablesNodes();

        bool IsPrimaryDataIncluded();
        List<Group> GetProjectsNodes();
    }

    public enum IndexingAction { CREATE, UPDATE, DELETE }
}
