//
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
using System.Reflection;
using NProxy.Core.Internal.Reflection;
using NProxy.Core.Test.Common.Types;
using NUnit.Framework;

namespace NProxy.Core.Test.Internal.Reflection
{
    [TestFixture]
    public sealed class MemberInfoExtensionsTestFixture
    {
        [Test]
        public void GetCustomAttributesTest()
        {
            // Arrange
            var methodInfo = typeof (INonIntercepted).GetMethod("Method");

            // Act
            var attributes = methodInfo.GetCustomAttributes<NonInterceptedAttribute>(false);

            // Assert
            Assert.That(attributes, Is.All.InstanceOf<NonInterceptedAttribute>());
        }

        [Test]
        public void IsDefinedTest()
        {
            // Arrange
            var methodInfo = typeof (INonIntercepted).GetMethod("Method");

            // Act
            var isDefined = methodInfo.IsDefined<NonInterceptedAttribute>(false);

            // Assert
            Assert.That(isDefined, Is.True);
        }

        [Test]
        public void GetDeclaringTypeTest()
        {
            // Arrange
            var methodInfo = typeof (Action).GetMethod("Invoke");

            // Act
            var declaringType = methodInfo.GetDeclaringType();

            // Assert
            Assert.That(declaringType, Is.EqualTo(typeof (Action)));
        }

        [Test]
        public void GetFullNameFromClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (Nested).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.Nested.Method"));
        }

        [Test]
        public void GetFullNameFromGenericClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (GenericNested<>).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.GenericNested`1.Method"));
        }

        [Test]
        public void GetFullNameFromNestedGenericClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (Nested.Generic<>).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.Nested.Generic`1.Method"));
        }

        [Test]
        public void GetFullNameFromNestedClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (Nested.NonGeneric).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.Nested.NonGeneric.Method"));
        }

        [Test]
        public void GetFullNameFromGenericNestedGenericClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (GenericNested<>.Generic<>).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.GenericNested`1.Generic`1.Method"));
        }

        [Test]
        public void GetFullNameFromGenericNestedClassMethodTest()
        {
            // Arrange
            var methodInfo = typeof (GenericNested<>.NonGeneric).GetMethod("Method");

            // Act
            var fullName = methodInfo.GetFullName();

            // Assert
            Assert.That(fullName, Is.EqualTo("NProxy.Core.Test.Common.Types.GenericNested`1.NonGeneric.Method"));
        }

        [Test]
        public void GetTokenEqualityTest()
        {
            // Arrange
            var firstMethodInfo = typeof (Action).GetMethod("Invoke");
            var secondMethodInfo = typeof (Action).GetMethod("Invoke");

            // Act
            var firstMemberToken = firstMethodInfo.GetToken();
            var secondMemberToken = secondMethodInfo.GetToken();

            // Assert
            Assert.That(firstMemberToken, Is.EqualTo(secondMemberToken));
        }

        [Test]
        public void GetTokenUniquenessTest()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var metadataTokensByModule = new Dictionary<Module, HashSet<int>>();
            var memberTokens = new HashSet<MemberToken>();

            foreach (var assembly in assemblies)
            {
                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException)
                {
                    continue;
                }

                foreach (var type in types)
                {
                    var memberInfos = type.GetMembers();

                    foreach (var memberInfo in memberInfos)
                    {
                        // Arrange
                        HashSet<int> metadataTokens;

                        if (!metadataTokensByModule.TryGetValue(memberInfo.Module, out metadataTokens))
                        {
                            metadataTokens = new HashSet<int>();
                            metadataTokensByModule.Add(memberInfo.Module, metadataTokens);
                        }

                        if (!metadataTokens.Add(memberInfo.MetadataToken))
                            continue;

                        // Act
                        var memberToken = memberInfo.GetToken();

                        // Assert
                        Assert.That(memberTokens.Add(memberToken), Is.True, "Member identifier is not unique");
                    }
                }
            }
        }
    }
}