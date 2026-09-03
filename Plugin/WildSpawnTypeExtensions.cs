using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrayZone
{
    public static class WildSpawnTypeExtensions
    {
        public static List<int> GZTypeEnums = new List<int> { 7190400 };

        public static bool IsGrayZone(WildSpawnType type)
        {
            return GZTypeEnums.Contains((int)type);
        }
    }
}
