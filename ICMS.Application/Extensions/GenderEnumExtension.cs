using ICMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMS.Application.Extensions
{
    public static class GenderEnumExtension
    {
        public static Gender FromStringToGenderEnum(this string type)
        {
            if (Enum.TryParse(type, true, out Gender gender))
            {
                return gender;
            }

            throw new InvalidCastException("No such a Gender exist!");

        }

    }
}
