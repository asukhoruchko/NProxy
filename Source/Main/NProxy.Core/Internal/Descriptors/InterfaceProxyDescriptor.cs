﻿//
// NProxy is a library for the .NET framework to create lightweight dynamic proxies.
// Copyright © Martin Tamme
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;

namespace NProxy.Core.Internal.Descriptors
{
    /// <summary>
    /// Represents an interface proxy descriptor.
    /// </summary>
    internal sealed class InterfaceProxyDescriptor : ProxyDescriptorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InterfaceProxyDescriptor"/> class.
        /// </summary>
        /// <param name="declaringType">The declaring type.</param>
        /// <param name="interfaceTypes">The interface types.</param>
        public InterfaceProxyDescriptor(Type declaringType, IEnumerable<Type> interfaceTypes)
            : base(declaringType, typeof (object), interfaceTypes)
        {
        }

        #region IDescriptor Members

        /// <inheritdoc/>
        public override void Accept(IDescriptorVisitor descriptorVisitor)
        {
            base.Accept(descriptorVisitor);

            // Visit declaring interface types.
            descriptorVisitor.VisitInterfaces(DeclaringInterfaceTypes);

            // Visit parent type members.
            descriptorVisitor.VisitMembers(ParentType);
        }

        /// <inheritdoc/>
        public override TInterface Cast<TInterface>(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            var interfaceType = typeof (TInterface);

            if (!interfaceType.IsInterface)
                throw new ArgumentException(String.Format("Type '{0}' is not an interface type", interfaceType));

            return (TInterface) instance;
        }

        /// <inheritdoc/>
        public override object CreateInstance(Type type, object[] arguments)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (arguments == null)
                throw new ArgumentNullException("arguments");

            return Activator.CreateInstance(type, arguments);
        }

        #endregion
    }
}