using BExIS.App.Testing;
using BExIS.Utils.Config;
using BEXIS.JSON.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using NUnit.Framework;
using System;
using System.IO;
using System.Net;

namespace BExIS.JSON.Helpers.UnitTests
{
    public class MetadataStructureManagerTests
    {
        private TestSetupHelper helper = null;
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            helper = new TestSetupHelper(WebApiConfig.Register, false);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            helper.Dispose();
        }

        [Test]
        public void ConvertToJsonSchema_validJsonSchema_returnJsonSchema()
        {
            //Arrange
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string schemaPath = Path.Combine(path, "jsonschema-draft7.json");

            var metadataStructureConverter = new MetadataStructureConverter();


            //Act
            var schema = metadataStructureConverter.ConvertToJsonSchema(1);


            //Assert

            using (StreamReader file = File.OpenText(schemaPath))
            {
                JSchema jsonSchemaStandard = JSchema.Parse(file.ReadToEnd());
                JObject jsonSchema = JObject.Parse(schema.ToString());

                bool valid = jsonSchema.IsValid(jsonSchemaStandard);
                Assert.IsTrue(valid);
            }

            Assert.NotNull(schema);
        }

        //

        [Test]
        public void ConvertToJsonSchema_IdNotExist_ArgumentException()
        {
            var metadataStructureConverter = new MetadataStructureConverter();

            Assert.That(() => metadataStructureConverter.ConvertToJsonSchema(10), Throws.ArgumentNullException);

        }

        [Test]
        public void ConvertToJsonSchema_IdIs0_ArgumentException()
        {
            var metadataStructureConverter = new MetadataStructureConverter();

            Assert.That(() => metadataStructureConverter.ConvertToJsonSchema(0), Throws.ArgumentException);

        }
    }
}