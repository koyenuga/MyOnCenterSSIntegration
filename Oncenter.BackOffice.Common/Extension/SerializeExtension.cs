using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Oncenter.BackOffice.Common.Extension
{
    public static class SerializeExtension
    {
        public static byte[] Serialize<T>( this T value)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, value);
                data = ms.ToArray();
            }

            return data;
        }
    }
}
