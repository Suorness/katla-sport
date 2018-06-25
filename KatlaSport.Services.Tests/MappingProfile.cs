using System;
using AutoMapper;
using KatlaSport.Services.HiveManagement;
using KatlaSport.Services.ProductManagement;

namespace KatlaSport.Services.Tests
{
    public class MappingProfile
    {
        private static readonly Lazy<MappingProfile> _instanse = new Lazy<MappingProfile>(() => new MappingProfile());

        private MappingProfile()
        {
            Mapper.Reset();
            Mapper.Initialize(routeMap =>
            {
                routeMap.AddProfile(new ProductManagementMappingProfile());
                routeMap.AddProfile(new HiveManagementMappingProfile());
            });
        }

        public static MappingProfile Initialize()
        {
            return _instanse.Value;
        }
    }
}
