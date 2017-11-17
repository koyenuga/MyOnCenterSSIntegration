using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Oncenter.BackOffice.Common.Extension
{
    public static class SerializeExtension
    {
        public static byte[] ToXmlByteArray<T>(this T value)
        {
            if (value == null)
                return null;

            byte[] data;
            using (var ms = new MemoryStream())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(ms, value);
                data = ms.ToArray();
            }

            return data;
        }
    }
}
