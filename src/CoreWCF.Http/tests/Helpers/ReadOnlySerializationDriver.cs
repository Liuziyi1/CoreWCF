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
    public enum SerializerType
    {
        DataContractSerializer,
        DataContractJsonSerializer
    }

    public static class ReadOnlySerializationDriver
    {
        public static void Run(Object serializingObject, Type serializingType, string expectedError, ITestOutputHelper _output)
        {
            //roundtrip serializingobject with serializer with the serializableReadOnlyFlag and expect the expectedError message

            _output.WriteLine(String.Format("Serialization roundtrip of type '{0}' with serializableReadOnlyFlag enabled DataContractSerializer.", serializingType.FullName));
            Utility.VerifyRoundTrip(serializingObject, serializingType, SerializerType.DataContractSerializer, true, expectedError,_output);

            _output.WriteLine("Serialization roundtrip of type '{0}' with serializableReadOnlyFlag enabled DataContractJsonSerializer.", serializingType.FullName);
            Utility.VerifyRoundTrip(serializingObject, serializingType, SerializerType.DataContractJsonSerializer, true, expectedError,_output);

            _output.WriteLine(String.Empty);
        }

        public static void Run(Object serializingObject, Type serializingType, string expectedError, Type successfullDeserializingType, ITestOutputHelper _output)
        {
            Run(serializingObject, serializingType, expectedError,_output);

            //Serialize serializingobject with the serializableReadOnlyFlag and de-serialize with successfullDeserializingType - does not expect any error

            _output.WriteLine("Serialization roundtrip of type '{0}' to type '{1}' with serializableReadOnlyFlag enabled DataContractSerializer.", serializingType.FullName, successfullDeserializingType.FullName);
            Utility.VerifyRoundTrip(serializingObject, serializingType, successfullDeserializingType, SerializerType.DataContractSerializer, true, String.Empty,_output);

            _output.WriteLine("Serialization roundtrip of type '{0}' to type '{1}' with serializableReadOnlyFlag enabled DataContractJsonSerializer.", serializingType.FullName, successfullDeserializingType.FullName);
            Utility.VerifyRoundTrip(serializingObject, serializingType, successfullDeserializingType, SerializerType.DataContractJsonSerializer, true, String.Empty,_output);

            _output.WriteLine(String.Empty);
        }
    }

    public static class Utility
    {
        public static MemoryStream GetXmlFormattedSerializedStream(Object serializingObject, XmlObjectSerializer serializer, ITestOutputHelper _output)
        {
            MemoryStream memStream = new MemoryStream(64 * 1024);
            StreamWriter writer = new StreamWriter(memStream);
            XmlTextWriter xmlTextWriter = new XmlTextWriter(writer);
            xmlTextWriter.Formatting = Formatting.Indented;

            //Serialize
            serializer.WriteObject(xmlTextWriter, serializingObject);
            xmlTextWriter.Flush();

            //Logs to console
            string line;
            memStream.Position = 0;
            StreamReader reader = new StreamReader(memStream);
            while ((line = reader.ReadLine()) != null)
            {
                _output.WriteLine(line);
            }

            return memStream;
        }

        public static bool IsXPathPresentInSerializedStream(Object serializingObject, DataContractSerializer serializer, string xPath, ITestOutputHelper _output)
        {
            MemoryStream memStream = GetXmlFormattedSerializedStream(serializingObject, serializer,_output);

            memStream.Position = 0;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(memStream);

            //To add namespaceUri as a prefix before searching for a node
            XmlNode firstChild = xmlDoc.FirstChild;
            XmlNamespaceManager xmlns = new XmlNamespaceManager(new NameTable());
            xmlns.AddNamespace("tempPrefix", firstChild.NamespaceURI);

            XmlNode matchNode = xmlDoc.SelectSingleNode(xPath, xmlns);

            if (matchNode == null)
            {
                _output.WriteLine(String.Format("XPath {0} is not present in serialized stream.", xPath));
                return false;
            }
            else
            {
                _output.WriteLine(String.Format("XPath {0} is present in serialized stream.", xPath));
                return true;
            }
        }

        public static XmlObjectSerializer GetSerializer(Type type, SerializerType serializerType, bool serializeReadOnlyTypes)
        {
            XmlObjectSerializer serializer;

            if (serializeReadOnlyTypes)
            {
                if (serializerType == SerializerType.DataContractSerializer)
                {
                    serializer = new DataContractSerializer(type, new DataContractSerializerSettings { SerializeReadOnlyTypes = true });
                }
                else if (serializerType == SerializerType.DataContractJsonSerializer)
                {
                    serializer = new DataContractJsonSerializer(type, new DataContractJsonSerializerSettings { SerializeReadOnlyTypes = true });
                }
                else
                {
                    throw new InvalidOperationException("Unexpected SerializerType enum specified.");
                }
            }
            else
            {
                if (serializerType == SerializerType.DataContractSerializer)
                {
                    serializer = new DataContractSerializer(type);
                }
                else if (serializerType == SerializerType.DataContractJsonSerializer)
                {
                    serializer = new DataContractJsonSerializer(type);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected SerializerType enum specified.");
                }
            }

            return serializer;
        }

        public static void VerifyRoundTrip(Object serializingObject, Type serializingType, SerializerType serializerType, bool serializeReadOnlyTypes, string expectedError, ITestOutputHelper _output)
        {
            XmlObjectSerializer serializer = GetSerializer(serializingType, serializerType, serializeReadOnlyTypes);
            VerifyRoundTrip(serializingObject, serializer, serializer, expectedError,_output);
        }

        public static void VerifyRoundTrip(Object serializingObject, Type serializingType, Type deserializingType, SerializerType serializerType, bool serializeReadOnlyTypes, string expectedError, ITestOutputHelper _output)
        {
            XmlObjectSerializer serializer = GetSerializer(serializingType, serializerType, serializeReadOnlyTypes);
            XmlObjectSerializer deserializer = GetSerializer(deserializingType, serializerType, serializeReadOnlyTypes);

            VerifyRoundTrip(serializingObject, serializer, deserializer, expectedError,_output);
        }

        public static void VerifyRoundTrip(Object serializingObject, XmlObjectSerializer serializer, XmlObjectSerializer deserializer, string expectedError, ITestOutputHelper _output)
        {
            string errorMessage = String.Empty;
            try
            {
                MemoryStream memStream;

                if (serializer.GetType() == typeof(DataContractJsonSerializer))
                {
                    memStream = new MemoryStream(64 * 1024);
                    serializer.WriteObject(memStream, serializingObject);
                }
                else
                {
                    memStream = GetXmlFormattedSerializedStream(serializingObject, serializer,_output);
                }

                memStream.Position = 0;
                deserializer.ReadObject(memStream);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                _output.WriteLine(e.ToString());
            }

            if (errorMessage != expectedError)
            {
                throw new ApplicationException(String.Format("Expected '{0}' but got '{1}'", expectedError, errorMessage));
            }
            else
            {
                _output.WriteLine("Serialization roundtrip completed as expected.");
                _output.WriteLine(String.Empty);
            }
        }
    }
}
