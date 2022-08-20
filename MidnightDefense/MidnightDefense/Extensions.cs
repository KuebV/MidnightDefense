using MidnightDefense.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidnightDefense
{
    public static class Extensions
    {

        public static void NoDuplicateAdd(this List<CheatsEnum> cheatsEnum, CheatsEnum cheat)
        {
            if (!cheatsEnum.Contains(cheat))
                cheatsEnum.Add(cheat);
        }
    }
}
