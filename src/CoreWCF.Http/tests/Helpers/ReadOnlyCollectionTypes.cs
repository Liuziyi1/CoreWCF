using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Helpers
{

    [DataContract(Name = "ReadOnlyMemberClass")]
    public class ReadOnlyMemberClass
    {
        [DataMember]
        public Guid GetOnlyProperty
        {
            get
            {
                return Guid.NewGuid();
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    public class DeserializableClass
    {
        [DataMember(Name = "GetOnlyProperty")]
        public virtual Guid GetSetProperty
        {
            get;
            set;
        }
    }

    [DataContract]
    public abstract class ReadyOnlyMemberAbstractClass
    {
        [DataMember]
        public abstract DateTime CurrentDateTime
        {
            get;
        }
    }

    public class ReadOnlyAbstractImplementedClass : ReadyOnlyMemberAbstractClass
    {
        public override DateTime CurrentDateTime
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    [DataContract]
    public class ReadOnlyClassMemberType
    {
        [DataMember]
        public DeserializableClass GetOnlyProperty
        {
            get
            {
                return new DeserializableClass();
            }
        }
    }

    [DataContract]
    public class ReadOnlyClassReadOnlyMemberType
    {
        [DataMember]
        public ReadOnlyMemberClass GetOnlyProperty
        {
            get
            {
                return new ReadOnlyMemberClass();
            }
        }
    }

    [DataContract]
    public class NonDataContractReadOnlyClassMemberType
    {
        [DataMember]
        public int GetSetProperty
        {
            get;
            set;
        }

        [DataMember]
        public CustomNonDataContract NonDataContractReadOnlyMember
        {
            get
            {
                return new CustomNonDataContract();
            }
        }
    }

    public class CustomNonDataContract
    {
        public int x = 7;

        public int y = 10;
    }

    [DataContract]
    public class MultipleReadonlyMembersClass
    {
        [DataMember]
        public Guid GetOnlyProperty1
        {
            get
            {
                return Guid.NewGuid();
            }
        }

        [DataMember]
        public int GetOnlyProperty2
        {
            get
            {
                return 7;
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    internal class ReadOnlyClassPrivateMember
    {
        [DataMember]
        private Guid GetOnlyProperty
        {
            get
            {
                return Guid.NewGuid();
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    internal class ReadOnlyClassProtectedMember
    {
        protected Guid getOnlyProperty = Guid.NewGuid();

        [DataMember]
        protected virtual Guid GetOnlyProperty
        {
            get
            {
                return this.getOnlyProperty;
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    internal class ReadOnlyClassInternalMember
    {
        [DataMember]
        internal Guid GetOnlyProperty
        {
            get
            {
                return Guid.NewGuid();
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    internal class ReadOnlyClassProtectedInternalMember
    {
        [DataMember]
        protected internal virtual Guid GetOnlyProperty
        {
            get
            {
                return Guid.NewGuid();
            }
        }
    }

    [DataContract]
    public class DerivedReadOnlyClass1 : DeserializableClass1
    {
        [DataMember]
        public DateTime OtherGetOnlyProperty
        {
            get
            {
                return DateTime.Now;
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    public class DeserializableClass1
    {
        [DataMember(Name = "GetOnlyProperty")]
        protected virtual Guid GetSetProperty
        {
            get;
            set;
        }
    }

    [DataContract]
    internal class DerivedReadOnlyClass2 : ReadOnlyClassProtectedMember
    {
        private Guid otherGetOnlyProperty = Guid.NewGuid();

        [DataMember(Name = "GetOnlyProperty")]
        public Guid OtherGetOnlyProperty
        {
            get
            {
                return this.otherGetOnlyProperty;
            }
            set
            {
                this.otherGetOnlyProperty = value;
            }
        }
    }

    [DataContract]
    internal class DerivedReadOnlyClass3 : ReadOnlyClassProtectedInternalMember
    {
        private DateTime otherGetOnlyProperty = DateTime.Today;

        [DataMember]
        public DateTime OtherGetOnlyProperty
        {
            get
            {
                return this.otherGetOnlyProperty;
            }
            set
            {
                this.otherGetOnlyProperty = value;
            }
        }
    }
    [DataContract]
    public class DerivedReadOnlyClass4 : DeserializableClass1
    {
        protected override Guid GetSetProperty
        {
            get
            {
                return base.GetSetProperty;
            }
        }
    }

    [DataContract(Name = "ReadOnlyMemberClass")]
    internal class DerivedReadOnlyClass5 : ReadOnlyClassProtectedMember
    {
        protected override Guid GetOnlyProperty
        {
            get
            {
                return base.GetOnlyProperty;
            }
        }
    }
}
