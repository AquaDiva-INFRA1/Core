﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BExIS.Dlm.Entities.DataStructure;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using Vaiona.Persistence.Api;

namespace BExIS.Dlm.Services.DataStructure
{
    public sealed class GlobalizationInfoManager
    {
        public GlobalizationInfoManager() 
        {
            //// define aggregate paths
            ////AggregatePaths.Add((Unit u) => u.ConversionsIamTheSource);            
            this.Repo = this.GetUnitOfWork().GetReadOnlyRepository<GlobalizationInfo>();
        }

        #region Data Readers

        // provide read only repos for the whole aggregate area
        public IReadOnlyRepository<GlobalizationInfo> Repo { get; private set; }

        #endregion

        #region GlobalizationInfo

        public GlobalizationInfo Create(string name, string description, string displayName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(Contract.Result<GlobalizationInfo>() != null && Contract.Result<GlobalizationInfo>().Id >= 0);

            GlobalizationInfo u = new GlobalizationInfo()
            {
                CultureId = name,
                Description = description,
                DisplayName = displayName,
            };

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<GlobalizationInfo> repo = uow.GetRepository<GlobalizationInfo>();
                repo.Put(u);
                uow.Commit();
            }
            return (u);            
        }

        public bool Delete(GlobalizationInfo entity)
        {
            Contract.Requires(entity != null);
            Contract.Requires(entity.Id >= 0);

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<GlobalizationInfo> repo = uow.GetRepository<GlobalizationInfo>();

                entity = repo.Reload(entity);                
                //relation to DataContainer is managed by the other end
                repo.Delete(entity);
                uow.Commit();
            }
            // if any problem was detected during the commit, an exception will be thrown!
            return (true);
        }

        public bool Delete(IEnumerable<GlobalizationInfo> entities)
        {
            Contract.Requires(entities != null);
            Contract.Requires(Contract.ForAll(entities, (GlobalizationInfo e) => e != null));
            Contract.Requires(Contract.ForAll(entities, (GlobalizationInfo e) => e.Id >= 0));

            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<GlobalizationInfo> repo = uow.GetRepository<GlobalizationInfo>();

                foreach (var entity in entities)
                {
                    var latest = repo.Reload(entity);
                    //relation to DataContainer is managed by the other end
                    repo.Delete(latest);
                }
                uow.Commit();
            }
            return (true);
        }

        public GlobalizationInfo Update(GlobalizationInfo entity)
        {
            Contract.Requires(entity != null, "provided entity can not be null");
            Contract.Requires(entity.Id >= 0, "provided entity must have a permant ID");

            Contract.Ensures(Contract.Result<GlobalizationInfo>() != null && Contract.Result<GlobalizationInfo>().Id >= 0, "No entity is persisted!");

            using (IUnitOfWork uow = entity.GetUnitOfWork())
            {
                IRepository<GlobalizationInfo> repo = uow.GetRepository<GlobalizationInfo>();
                repo.Put(entity); // Merge is required here!!!!
                uow.Commit();
            }
            return (entity);    
        }

        #endregion

    }
}
