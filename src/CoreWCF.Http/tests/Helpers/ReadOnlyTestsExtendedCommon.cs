using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using Xunit.Abstractions;

namespace Helpers
{
   public class ReadOnlyTestsExtendedCommon
    {
        // MS.Test.NetFx45.TestCases.Metadata.Extended.Serialization.Common.ReadOnlyTestsExtendedCommon
        public void GetJSONCtrSerializers(Type serializingType, string flag, out List<XmlObjectSerializer> serializers)
        {
            string text = "root";
            XmlDictionaryString rootName = new XmlDictionaryString(XmlDictionary.Empty, text, 0);
            serializers = new List<XmlObjectSerializer>();
            if (flag.Equals("enabled"))
            {
                serializers.Add(new DataContractJsonSerializer(serializingType, new DataContractJsonSerializerSettings
                {
                    SerializeReadOnlyTypes = true
                }));
                serializers.Add(new DataContractJsonSerializer(serializingType, new DataContractJsonSerializerSettings
                {
                    RootName = text,
                    SerializeReadOnlyTypes = true
                }));
                return;
            }
            if (flag.Equals("disabled"))
            {
                serializers.Add(new DataContractJsonSerializer(serializingType));
                serializers.Add(new DataContractJsonSerializer(serializingType, text));
                serializers.Add(new DataContractJsonSerializer(serializingType, new List<Type>()));
                serializers.Add(new DataContractJsonSerializer(serializingType, rootName));
                serializers.Add(new DataContractJsonSerializer(serializingType, text, new List<Type>()));
                serializers.Add(new DataContractJsonSerializer(serializingType, rootName, null));
#if NET472
                 serializers.Add(new DataContractJsonSerializer(serializingType, null, 2147483647, false, null, false));
                 serializers.Add(new DataContractJsonSerializer(serializingType, text, null, 2147483647, false, null, false));
                 serializers.Add(new DataContractJsonSerializer(serializingType, rootName, null, 2147483647, false, null, false));
#endif
                serializers.Add(new DataContractJsonSerializer(serializingType, new DataContractJsonSerializerSettings
                {
                    SerializeReadOnlyTypes = false
                }));
                serializers.Add(new DataContractJsonSerializer(serializingType, new DataContractJsonSerializerSettings
                {
                    RootName = text,
                    SerializeReadOnlyTypes = false
                }));
                return;
            }
            throw new ApplicationException("Unexpected flag value specified.");
        }
        // MS.Test.NetFx45.TestCases.Metadata.Serialization.ReadOnlyTypesSerialization.Utility
        public void VerifyRoundTrip(object serializingObject, XmlObjectSerializer serializer, XmlObjectSerializer deserializer, string expectedError, ITestOutputHelper _output)
        {
            string text = string.Empty;
            try
            {
                MemoryStream memoryStream;
                if (serializer.GetType() == typeof(DataContractJsonSerializer))
                {
                    memoryStream = new MemoryStream(65536);
                    serializer.WriteObject(memoryStream, serializingObject);
                }
                else
                {
                    memoryStream = GetXmlFormattedSerializedStream(serializingObject, serializer,_output);
                }
                memoryStream.Position = 0L;
                deserializer.ReadObject(memoryStream);
            }
            catch (Exception ex)
            {
                text = ex.Message;
                _output.WriteLine(ex.ToString());
            }
            if (text != expectedError)
            {
                throw new ApplicationException(string.Format("Expected '{0}' but got '{1}'", expectedError, text));
            }
            _output.WriteLine("Serialization roundtrip completed as expected.");
            _output.WriteLine(string.Empty);
        }

        public  MemoryStream GetXmlFormattedSerializedStream(object serializingObject, XmlObjectSerializer serializer, ITestOutputHelper _output)
        {
            MemoryStream memoryStream = new MemoryStream(65536);
            StreamWriter w = new StreamWriter(memoryStream);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(w);
            xmlTextWriter.Formatting = Formatting.Indented;
            serializer.WriteObject(xmlTextWriter, serializingObject);
            xmlTextWriter.Flush();
            memoryStream.Position = 0L;
            StreamReader streamReader = new StreamReader(memoryStream);
            string message;
            while ((message = streamReader.ReadLine()) != null)
            {
                _output.WriteLine(message);
            }
            return memoryStream;
        }
    }
}
