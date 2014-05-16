using System;
using Xunit.Abstractions;
#if !K10
using System.Runtime.Serialization;
using System.Security;
#endif

namespace Xunit.Sdk
{
#if !K10
    [Serializable]
    partial class XunitTestCollection : ISerializable
    {
        /// <inheritdoc/>
        protected XunitTestCollection(SerializationInfo info, StreamingContext context)
        {
            DisplayName = info.GetString("DisplayName");
            ID = Guid.Parse(info.GetString("ID"));

            var assemblyName = info.GetString("DeclarationAssemblyName");
            var typeName = info.GetString("DeclarationTypeName");

            if (!String.IsNullOrWhiteSpace(assemblyName) && String.IsNullOrWhiteSpace(typeName))
                CollectionDefinition = Reflector.Wrap(Reflector.GetType(assemblyName, typeName));
        }

        /// <inheritdoc/>
        [SecurityCritical]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("DisplayName", DisplayName);
            info.AddValue("ID", ID.ToString());

            if (CollectionDefinition != null)
            {
                info.AddValue("DeclarationAssemblyName", CollectionDefinition.Assembly.Name);
                info.AddValue("DeclarationTypeName", CollectionDefinition.Name);
            }
            else
            {
                info.AddValue("DeclarationAssemblyName", null);
                info.AddValue("DeclarationTypeName", null);
            }
        }
    }
#endif

    /// <summary>
    /// The implementation of <see cref="ITestCollection"/> that is used by xUnit.net v2.
    /// </summary>
    public partial class XunitTestCollection : LongLivedMarshalByRefObject, ITestCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XunitTestCollection"/> class.
        /// </summary>
        public XunitTestCollection()
        {
            ID = Guid.NewGuid();
        }

        /// <inheritdoc/>
        public ITypeInfo CollectionDefinition { get; set; }

        /// <inheritdoc/>
        public string DisplayName { get; set; }

        /// <inheritdoc/>
        public Guid ID { get; set; }
    }
}