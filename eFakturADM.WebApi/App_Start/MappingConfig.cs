using AutoMapper;
using eFakturADM.Logic.Objects;
using eFakturADM.WebApi.Models;

namespace eFakturADM.WebApi
{
    public class MappingConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        /// <summary>
        /// 
        /// </summary>
        public static void RegisterMapping()
        {
            Mapper.Initialize(config =>
            {
                config.CreateMap<FakturPajakRequestModel, FakturPajak>();
                config.CreateMap<FakturPajakDetailRequestModel, FakturPajakDetail>();
            });
        }
    }
}