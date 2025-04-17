using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class MapJnsTransaksiKdJnsTransaksis : ApplicationCollection<MapJnsTransaksiKdJnsTransaksi, SpBase>
    {
        public static List<MapJnsTransaksiKdJnsTransaksi> Get()
        {
            var sp = new SpBase(@"SELECT map.[Id]
      ,[FCode]
      ,[KdJenisTransaksi]
      ,[JenisTransaksi]
	  ,kdTrans.[Description] AS KdJenisDescription
FROM [dbo].[MapJnsTransaksiKdJnsTransaksi] map INNER JOIN
		[dbo].FPKdJenisTransaksi kdTrans ON map.KdJenisTransaksi = kdTrans.Id");
            return GetApplicationCollection(sp);
        }
        
    }
}
