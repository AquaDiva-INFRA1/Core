﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BExIS.Dlm.Entities.Common;
using BExIS.Dlm.Entities.MetadataStructure;
using BExIS.Dlm.Services.MetadataStructure;

namespace BExIS.Dcm.CreateDatasetWizard
{
    public class UsageHelper
    {

        public static BaseUsage GetMetadataAttributeUsageById(long Id)
        {
            BaseUsage usage = new BaseUsage();

            MetadataPackageManager mpm = new MetadataPackageManager();

            var q = from p in mpm.MetadataPackageRepo.Get()
                    from u in p.MetadataAttributeUsages
                    where u.Id == Id // && p.Id.Equals(parentId)
                    select u;

            return q.FirstOrDefault();

       
        }

        public static BaseUsage GetMetadataCompoundAttributeUsageById(long Id)
        {
            BaseUsage usage = new BaseUsage();

            MetadataAttributeManager mam = new MetadataAttributeManager();

            var x = from c in mam.MetadataCompoundAttributeRepo.Get()
                    from u in c.Self.MetadataNestedAttributeUsages
                    where u.Id == Id //&& c.Id.Equals(parentId)
                    select u;

            return x.FirstOrDefault();

        }
        


        public static BaseUsage GetSimpleUsageById(long Id)
        {
            BaseUsage usage = new BaseUsage();

            MetadataPackageManager mpm = new MetadataPackageManager();

            var q = from p in mpm.MetadataPackageRepo.Get()
                    from u in p.MetadataAttributeUsages
                    where u.Id == Id && u.MetadataAttribute.Self is MetadataSimpleAttribute
                    select u;

            if (q != null && q.ToList().Count > 0)
            {
                return q.FirstOrDefault();
            }
            else
            {
                MetadataAttributeManager mam = new MetadataAttributeManager();

                var x = from c in mam.MetadataCompoundAttributeRepo.Get()
                        from u in c.Self.MetadataNestedAttributeUsages
                        where u.Id == Id && u.Member.Self is MetadataSimpleAttribute
                        select u;

                return x.FirstOrDefault();
            }
        }

        public static BaseUsage GetUsageById(long Id)
        {
            BaseUsage usage = new BaseUsage();

            MetadataStructureManager msm = new MetadataStructureManager();

            var q = from p in msm.PackageUsageRepo.Get()
                    where p.Id ==  Id
                    select p;

            if (q != null && q.ToList().Count > 0)
            {
                return q.FirstOrDefault();
            }
            else
            {
                MetadataAttributeManager mam = new MetadataAttributeManager();

                var x = from c in mam.MetadataCompoundAttributeRepo.Get()
                        from u in c.Self.MetadataNestedAttributeUsages
                        where u.Id == Id
                        select u;

                return x.FirstOrDefault();
            }
        }

        public static List<BaseUsage> GetChildren(BaseUsage usage)
        {
            List<BaseUsage> temp = new List<BaseUsage>();

            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;

                foreach (BaseUsage childUsage in mpu.MetadataPackage.MetadataAttributeUsages)
                {
                    temp.Add(childUsage);
                }
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mau.MetadataAttribute.Self).MetadataNestedAttributeUsages)
                    {
                        temp.Add(childUsage);
                    }
                }
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                if (mnau.Member.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mnau.Member.Self).MetadataNestedAttributeUsages)
                    {
                        temp.Add(childUsage);
                    }
                }
            }

            return temp;
        }

        public static List<BaseUsage> GetCompoundChildrens(BaseUsage usage)
        {
            List<BaseUsage> temp = new List<BaseUsage>();

            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;

                foreach (BaseUsage childUsage in mpu.MetadataPackage.MetadataAttributeUsages)
                {
                    if(IsCompound(childUsage))
                        temp.Add(childUsage);
                }
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mau.MetadataAttribute.Self).MetadataNestedAttributeUsages)
                    {
                        if (IsCompound(childUsage))
                            temp.Add(childUsage);
                    }
                }
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                if (mnau.Member.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mnau.Member.Self).MetadataNestedAttributeUsages)
                    {
                        if (IsCompound(childUsage))
                            temp.Add(childUsage);
                    }
                }
            }

            return temp;
        }

        private static bool IsCompound(BaseUsage usage)
        {
            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                if (mau.MetadataAttribute.Self is MetadataCompoundAttribute) return true;
                
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                if (mnau.Member.Self is MetadataCompoundAttribute) return true;
            }
            
            return false;
        }

        private static bool IsSimple(BaseUsage usage)
        {
            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                if (mau.MetadataAttribute.Self is MetadataSimpleAttribute) return true;

            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                if (mnau.Member.Self is MetadataSimpleAttribute) return true;
            }

            return false;
        }

        public static string GetNameOfType(BaseUsage usage)
        { 

            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;
                return mpu.MetadataPackage.Name;
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                return mau.MetadataAttribute.Name;
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                return mnau.Member.Name;
            }

            return "";
        }

        public static long GetIdOfType(BaseUsage usage)
        {

            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;
                return mpu.MetadataPackage.Id;
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                return mau.MetadataAttribute.Id;
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                return mnau.Member.Id;
            }

            return 0;
        }

        public static bool HasUsagesWithSimpleType(BaseUsage usage)
        {
            if (usage is MetadataPackageUsage)
            {
                MetadataPackageUsage mpu = (MetadataPackageUsage)usage;

                foreach (BaseUsage childUsage in mpu.MetadataPackage.MetadataAttributeUsages)
                {
                    if(IsSimple(childUsage)) return true;
                }
            }

            if (usage is MetadataAttributeUsage)
            {
                MetadataAttributeUsage mau = (MetadataAttributeUsage)usage;
                if (mau.MetadataAttribute.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mau.MetadataAttribute.Self).MetadataNestedAttributeUsages)
                    {
                       if(IsSimple(childUsage)) return true;
                    }
                }
            }

            if (usage is MetadataNestedAttributeUsage)
            {
                MetadataNestedAttributeUsage mnau = (MetadataNestedAttributeUsage)usage;
                if (mnau.Member.Self is MetadataCompoundAttribute)
                {
                    foreach (BaseUsage childUsage in ((MetadataCompoundAttribute)mnau.Member.Self).MetadataNestedAttributeUsages)
                    {
                        if (IsSimple(childUsage)) return true;
                    }
                }
            }

            return false;
        }
    }
}
