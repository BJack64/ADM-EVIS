using System.Collections.Generic;
using eFakturADM.Logic.Core;
using eFakturADM.Logic.Objects;

namespace eFakturADM.Logic.Collections
{
    public class MapJnsTransJnsDoks : ApplicationCollection<MapJnsTransJnsDok, SpBase>
    {
        public static List<MapJnsTransJnsDok> Get()
        {
            var sp = new SpBase(@"SELECT map.[Id]
      ,[FCode]
      ,[JenisDokumen]
      ,[JenisTransaksi]
	  ,jdok.[Description] AS JenisDokumenDescription
FROM [dbo].[MapJnsTransJnsDok] map INNER JOIN
		[dbo].FPJenisDokumen jdok ON map.JenisDokumen = jdok.Id");
            return GetApplicationCollection(sp);
        }
        
    }
}
