using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public static class CompanyHelper
    {
        private static readonly Dictionary<CompanyType, string> CompanyDefines = new Dictionary<CompanyType, string>()
        {
            { CompanyType.None, string.Empty },
            { CompanyType.Vaveda, "Vaveda" },
        };

        public static CompanyType GetTargetCompany()
        {
            var targetStore = CompanyType.None;

#if Vaveda
            targetStore = CompanyType.Vaveda;
#endif
            
            return targetStore;
        }
        
        public static string GetDefineByCompany(CompanyType companyType)
        {
            if (CompanyDefines.TryGetValue(companyType, out string define))
            {
                return define;
            }
            else
            {
                Debug.LogException(new Exception($"Not found define for {nameof(CompanyType)} - {companyType}"));
            }

            return string.Empty;
        }

        public static string[] GetCompanyDefines()
        {
            List<string> defines = new List<string>();

            foreach (var companyDefine in CompanyDefines)
            {
                defines.Add(companyDefine.Value);
            }

            return defines.ToArray();
        }
    }
}