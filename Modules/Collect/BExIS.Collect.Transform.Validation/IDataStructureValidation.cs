﻿
namespace BExIS.DCM.Transform.Validation
{
    public interface IDataStructureValidation
    {
        DsType AppliedTo { get; }
        //Error Execute(int id);

    }

    public enum DsType
    {
        Dataset,
        Datastructure
    }
}
