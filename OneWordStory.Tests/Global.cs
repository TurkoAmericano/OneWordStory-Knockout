using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using NUnit.Framework;
using OneWordStory.Domain.Indexes;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace OneWordStory.Tests
{
    public static class Global
    {

        public static void UpdateIndex<T>(IDocumentSession session)
        {
            RavenQueryStatistics stats;
            var results = session.Query<T>()
                .Statistics(out stats)
                .Customize(x => x.WaitForNonStaleResults())
                .ToArray();
        }


        public static IDocumentStore GetInMemoryStore()
        {
            var store = new EmbeddableDocumentStore()
            {
                RunInMemory = true
            }.Initialize();

            IndexCreation.CreateIndexes(typeof(UserByEmail).Assembly, store);
            IndexCreation.CreateIndexes(typeof(StoriesByUser).Assembly, store);

            return store;

        }



        public static void PropertyValuesAreEquals(object actual, object expected)
        {
            PropertyInfo[] properties = expected.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {

                
                if (property.GetIndexParameters().Count() > 0) continue;

                object expectedValue = property.GetValue(expected, null);
                object actualValue = property.GetValue(actual, null);
                
                if (actualValue == null)
                    Assert.IsNull(expectedValue);
                else if (actualValue is IList)
                    AssertListsAreEquals(property, (IList)actualValue, (IList)expectedValue);
                else if (!actualValue.GetType().IsPrimitive && !(actualValue is String))
                    PropertyValuesAreEquals(actualValue, expectedValue);
                else if (!Equals(expectedValue, actualValue))
                    Assert.Fail("Property {0}.{1} does not match. Expected: {2} but was: {3}", property.DeclaringType.Name, property.Name, expectedValue, actualValue);
            }
        }

        private static void AssertListsAreEquals(PropertyInfo property, IList actualList, IList expectedList)
        {
            if (actualList.Count != expectedList.Count)
                Assert.Fail("Property {0}.{1} does not match. Expected IList containing {2} elements but was IList containing {3} elements", property.PropertyType.Name, property.Name, expectedList.Count, actualList.Count);

            for (int i = 0; i < actualList.Count; i++)
                if (!Equals(actualList[i], expectedList[i]))
                    Assert.Fail("Property {0}.{1} does not match. Expected IList with element {1} equals to {2} but was IList with element {1} equals to {3}", property.PropertyType.Name, property.Name, expectedList[i], actualList[i]);
        }

        public static void AreEqualByJson(object expected, object actual)
        {
            var oSerializer = new JavaScriptSerializer();
            var expectedJson = oSerializer.Serialize(expected);
            var actualJson = oSerializer.Serialize(actual);
            Assert.AreEqual(expectedJson, actualJson);
        }

    }
}
