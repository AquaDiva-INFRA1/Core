﻿using BExIS.Security.Entities.Objects;
using System.Collections.Generic;
using System.Linq;
using Vaiona.Persistence.Api;

namespace BExIS.Security.Services.Objects
{
    public class FeatureManager
    {
        public FeatureManager()
        {
            var uow = this.GetUnitOfWork();

            FeatureRepository = uow.GetReadOnlyRepository<Feature>();
        }

        public IQueryable<Feature> Entities => FeatureRepository.Query();
        public IReadOnlyRepository<Feature> FeatureRepository { get; }

        public void Create(Feature feature)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var featureRepository = uow.GetRepository<Feature>();
                featureRepository.Put(feature);
                uow.Commit();
            }
        }

        public void Delete(Feature feature)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var featureRepository = uow.GetRepository<Feature>();
                featureRepository.Delete(feature);
                uow.Commit();
            }
        }

        public Feature FindById(long featureId)
        {
            return FeatureRepository.Get(featureId);
        }

        public List<Feature> FindRoots()
        {
            return FeatureRepository.Query(f => f.Parent == null).ToList();
        }

        public void Update(Feature feature)
        {
            using (var uow = this.GetUnitOfWork())
            {
                var featureRepository = uow.GetRepository<Feature>();
                featureRepository.Put(feature);
                uow.Commit();
            }
        }
    }
}