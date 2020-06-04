using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using System.Xml;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CoreWCF.IdentityModel.Policy;
using CoreWCF.Primitives.Tests.CustomSecurity;
using Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;
using Helpers;

namespace CoreWCF.Http.Tests
{
    public class ReadOnlyContractSerializerTest
    {
        private ITestOutputHelper _output;
        public ReadOnlyContractSerializerTest(ITestOutputHelper output)
        {
            _output = output;

        }

        [Fact]
        public void VerifyDataContractJSONSerializerConstructors()
        {
            string serializingErrorString = "";
            string deserializingErrorString = "";
#if NET472
            ResourceManager rm = new ResourceManager("System.Runtime.Serialization", typeof(DataContractJsonSerializer).Assembly);
            var ReadOnlyClassDeserialization = rm.GetString("ReadOnlyClassDeserialization");
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");           
            string serializingErrorMessage = NoSetMethodForProperty;
            string deserializingErrorMessage = String.Format(ReadOnlyClassDeserialization, NoSetMethodForProperty); 
            serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            deserializingErrorString = string.Format(deserializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");

#else
            ResourceManager rm = new ResourceManager("FxResources.System.Private.DataContractSerialization.SR", typeof(DataContractJsonSerializer).Assembly);
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");
            string serializingErrorMessage = NoSetMethodForProperty;
            serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            deserializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
#endif
            List<XmlObjectSerializer> list = new List<XmlObjectSerializer>();
            ReadOnlyTestsExtendedCommon readOnlyTestsExtendedCommon = new ReadOnlyTestsExtendedCommon();
            readOnlyTestsExtendedCommon.GetJSONCtrSerializers(typeof(ReadOnlyMemberClass), "enabled", out list);
            foreach (XmlObjectSerializer current in list)
            {
                readOnlyTestsExtendedCommon.VerifyRoundTrip(new ReadOnlyMemberClass(), current, current, deserializingErrorString, _output);
            }
            list.Clear();
            readOnlyTestsExtendedCommon.GetJSONCtrSerializers(typeof(ReadOnlyMemberClass), "disabled", out list);
            foreach (XmlObjectSerializer current2 in list)
            {
                readOnlyTestsExtendedCommon.VerifyRoundTrip(new ReadOnlyMemberClass(), current2, current2, serializingErrorString, _output);
            }
        }

        [Fact]
        public void VerifyDataContractSerializerConstructors()
        {
            string serializingErrorString = "";
            string deserializingErrorString = "";
#if NET472
            ResourceManager rm = new ResourceManager("System.Runtime.Serialization", typeof(DataContractJsonSerializer).Assembly);
            var ReadOnlyClassDeserialization = rm.GetString("ReadOnlyClassDeserialization");
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");           
            string serializingErrorMessage = NoSetMethodForProperty;
            string deserializingErrorMessage = String.Format(ReadOnlyClassDeserialization, NoSetMethodForProperty); 
            serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            deserializingErrorString = string.Format(deserializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");

#else
            ResourceManager rm = new ResourceManager("FxResources.System.Private.DataContractSerialization.SR", typeof(DataContractJsonSerializer).Assembly);
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");
            string serializingErrorMessage = NoSetMethodForProperty;
            serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            deserializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
#endif

            ReadOnlyTestsExtendedCommon readOnlyTestsExtendedCommon = new ReadOnlyTestsExtendedCommon();
            XmlDictionaryString readOnlyClassXmlDictString = new XmlDictionaryString(XmlDictionary.Empty, "ReadOnlyClass", 0);
            XmlDictionaryString emptyXmlDictString = new XmlDictionaryString(XmlDictionary.Empty, String.Empty, 0);

            //Call in to all constructors
            List<XmlObjectSerializer> serializers = new List<XmlObjectSerializer>();
            //Only intends to verify all the possible constructors irrespective of values passed in 
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass)));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), (IEnumerable<Type>)null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null));
#if NET472
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), null, Int32.MaxValue, false, false, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), null, Int32.MaxValue, false, false, null, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null, Int32.MaxValue, false, false, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null, Int32.MaxValue, false, false, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null, Int32.MaxValue, false, false, null, null));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), readOnlyClassXmlDictString, emptyXmlDictString, null, Int32.MaxValue, false, false, null, null));
#endif
            //ReadOnly serialization is disabled 
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { SerializeReadOnlyTypes = false }));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { RootName = readOnlyClassXmlDictString, RootNamespace = emptyXmlDictString, SerializeReadOnlyTypes = false }));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { RootName = new XmlDictionary().Add("ReadOnlyClass"), RootNamespace = XmlDictionaryString.Empty, SerializeReadOnlyTypes = false }));

            foreach (XmlObjectSerializer serializer in serializers)
            {
                readOnlyTestsExtendedCommon.VerifyRoundTrip(new ReadOnlyMemberClass(), serializer, serializer, serializingErrorString, _output);
            }

            //ReadOnly serialization is enabled
            serializers.Clear();
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { SerializeReadOnlyTypes = true }));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { RootName = readOnlyClassXmlDictString, RootNamespace = emptyXmlDictString, SerializeReadOnlyTypes = true }));
            serializers.Add(new DataContractSerializer(typeof(ReadOnlyMemberClass), new DataContractSerializerSettings { RootName = new XmlDictionary().Add("ReadOnlyClass"), RootNamespace = XmlDictionaryString.Empty, SerializeReadOnlyTypes = true }));

            foreach (XmlObjectSerializer serializer in serializers)
            {
                readOnlyTestsExtendedCommon.VerifyRoundTrip(new ReadOnlyMemberClass(), serializer, serializer, deserializingErrorString, _output);
            }
        }

        [Fact]
        public void VerifyReadOnlyMemberTypes()
        {
            //string serializingErrorString = "";
            //string deserializingErrorString = "";
            string serializingErrorMessage = "";
            string deserializingErrorMessage = "";
#if NET472
            ResourceManager rm = new ResourceManager("System.Runtime.Serialization", typeof(DataContractJsonSerializer).Assembly);
            var ReadOnlyClassDeserialization = rm.GetString("ReadOnlyClassDeserialization");
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");           
             serializingErrorMessage = NoSetMethodForProperty;
             deserializingErrorMessage = String.Format(ReadOnlyClassDeserialization, NoSetMethodForProperty); 
            //serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            //deserializingErrorString = string.Format(deserializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");

#else
            ResourceManager rm = new ResourceManager("FxResources.System.Private.DataContractSerialization.SR", typeof(DataContractJsonSerializer).Assembly);
            var NoSetMethodForProperty = rm.GetString("NoSetMethodForProperty");
            serializingErrorMessage = NoSetMethodForProperty;
            deserializingErrorMessage = NoSetMethodForProperty;
            //serializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
            //deserializingErrorString = string.Format(serializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty");
#endif
            ReadOnlySerializationDriver.Run(new ReadOnlyMemberClass(), typeof(ReadOnlyMemberClass), String.Format(deserializingErrorMessage, typeof(ReadOnlyMemberClass).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyAbstractImplementedClass(), typeof(ReadOnlyAbstractImplementedClass), String.Format(deserializingErrorMessage, typeof(ReadyOnlyMemberAbstractClass).FullName, "CurrentDateTime"), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyClassMemberType(), typeof(ReadOnlyClassMemberType), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassMemberType).FullName, "GetOnlyProperty"), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyClassReadOnlyMemberType(), typeof(ReadOnlyClassReadOnlyMemberType), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassReadOnlyMemberType).FullName, "GetOnlyProperty"), _output);
            ReadOnlySerializationDriver.Run(new NonDataContractReadOnlyClassMemberType(), typeof(NonDataContractReadOnlyClassMemberType), String.Format(deserializingErrorMessage, typeof(NonDataContractReadOnlyClassMemberType).FullName, "NonDataContractReadOnlyMember"), _output);
            ReadOnlySerializationDriver.Run(new MultipleReadonlyMembersClass(), typeof(MultipleReadonlyMembersClass), String.Format(deserializingErrorMessage, typeof(MultipleReadonlyMembersClass).FullName, "GetOnlyProperty2"), _output);


            ReadOnlySerializationDriver.Run(new ReadOnlyClassPrivateMember(), typeof(ReadOnlyClassPrivateMember), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassPrivateMember).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyClassProtectedMember(), typeof(ReadOnlyClassProtectedMember), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassProtectedMember).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyClassInternalMember(), typeof(ReadOnlyClassInternalMember), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassInternalMember).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new ReadOnlyClassProtectedInternalMember(), typeof(ReadOnlyClassProtectedInternalMember), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassProtectedInternalMember).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new DerivedReadOnlyClass1(), typeof(DerivedReadOnlyClass1), String.Format(deserializingErrorMessage, typeof(DerivedReadOnlyClass1).FullName, "OtherGetOnlyProperty"), _output);
            ReadOnlySerializationDriver.Run(new DerivedReadOnlyClass4(), typeof(DerivedReadOnlyClass4), String.Empty, _output);
            Utility.VerifyRoundTrip(new DerivedReadOnlyClass2(), typeof(DerivedReadOnlyClass2), SerializerType.DataContractSerializer, true, String.Format(deserializingErrorMessage, typeof(ReadOnlyClassProtectedMember).FullName, "GetOnlyProperty"), _output);
            ReadOnlySerializationDriver.Run(new DerivedReadOnlyClass5(), typeof(DerivedReadOnlyClass5), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassProtectedMember).FullName, "GetOnlyProperty"), typeof(DeserializableClass), _output);
            ReadOnlySerializationDriver.Run(new DerivedReadOnlyClass3(), typeof(DerivedReadOnlyClass3), String.Format(deserializingErrorMessage, typeof(ReadOnlyClassProtectedInternalMember).FullName, "GetOnlyProperty"), _output);

        }
    }

}
