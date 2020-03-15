using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using XMLParsing.XML;

namespace XMLParsing
{
    public class VacancyXmlSchema : ISchemaService
    {
        public XmlSchema BuildSchema(TextWriter output)
        {
            var schema = new XmlSchema();

            schema.Items.Add(Root());

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallbackOne);
            schemaSet.Add(schema);
            schemaSet.Compile();

            XmlSchema compiledSchema = null;

            foreach (XmlSchema schema1 in schemaSet.Schemas())
            {
                compiledSchema = schema1;
            }
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            compiledSchema.Write(output, nsmgr);

            return compiledSchema;
        }

        private XmlSchemaElement Root()
        {
            var rootArrayElement = new XmlSchemaElement();
            rootArrayElement.Name = "ArrayOfVacancy";

            var rootType = new XmlSchemaComplexType();
            var sequence = new XmlSchemaSequence();
            sequence.Items.Add(Vacancy());
            rootType.Particle = sequence;

            rootArrayElement.SchemaType = rootType;
            return rootArrayElement;
        }

        private XmlSchemaElement Vacancy()
        {
            var vacancyElement = new XmlSchemaElement();
            vacancyElement.Name = "Vacancy";

            var vacancyType = new XmlSchemaComplexType();
            vacancyType.Attributes.Add(Id());
            vacancyType.Attributes.Add(Hot());

            var sequence = new XmlSchemaSequence();
            sequence.Items.Add(Name());
            sequence.Items.Add(Date());
            sequence.Items.Add(CityName());
            sequence.Items.Add(ShortDescription());
            sequence.Items.Add(CompanyName());
            vacancyType.Particle = sequence;

            vacancyElement.SchemaType = vacancyType;
            vacancyElement.MaxOccursString = "unbounded";
            return vacancyElement;
        }

        private XmlSchemaElement Name()
        {
            var nameElement = new XmlSchemaElement();
            nameElement.Name = "Name";
            nameElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            return nameElement;
        }

        private XmlSchemaElement Date()
        {
            var dateElement = new XmlSchemaElement();
            dateElement.Name = "Date";
            dateElement.SchemaTypeName = new XmlQualifiedName("dateTime", "http://www.w3.org/2001/XMLSchema");
            return dateElement;
        }

        private XmlSchemaElement CityName()
        {
            var cityNameElement = new XmlSchemaElement();
            cityNameElement.Name = "CityName";

            var cityNameType = new XmlSchemaSimpleType();
            var cityNameRestriction = new XmlSchemaSimpleTypeRestriction();
            cityNameRestriction.BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");

            var maxLength = new XmlSchemaMaxLengthFacet();
            maxLength.Value = "15";
            var minLenght = new XmlSchemaMinLengthFacet();
            minLenght.Value = "4";

            cityNameRestriction.Facets.Add(maxLength);
            cityNameRestriction.Facets.Add(minLenght);
            cityNameType.Content = cityNameRestriction;

            cityNameElement.SchemaType = cityNameType;
            return cityNameElement;
        }

        private XmlSchemaElement ShortDescription()
        {
            var shortDescriptionElement = new XmlSchemaElement();
            shortDescriptionElement.Name = "ShortDescription";
            shortDescriptionElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            return shortDescriptionElement;
        }

        private XmlSchemaElement CompanyName()
        {
            var companyNameElement = new XmlSchemaElement();
            companyNameElement.Name = "CompanyName";
            companyNameElement.SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema");
            return companyNameElement;
        }

        private XmlSchemaAttribute Id()
        {
            var idAttribute = new XmlSchemaAttribute();
            idAttribute.Name = "Id";
            idAttribute.Use = XmlSchemaUse.Required;
            idAttribute.SchemaTypeName = new XmlQualifiedName("int", "http://www.w3.org/2001/XMLSchema");
            return idAttribute;
        }

        private XmlSchemaAttribute Hot()
        {
            var hotAttribute = new XmlSchemaAttribute();
            hotAttribute.Name = "Hot";
            hotAttribute.Use = XmlSchemaUse.Optional;
            hotAttribute.SchemaTypeName = new XmlQualifiedName("boolean", "http://www.w3.org/2001/XMLSchema");
            return hotAttribute;
        }

        static void ValidationCallbackOne(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                Console.Write("WARNING: ");
                Console.WriteLine(e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                Console.Write("ERROR: ");
                Console.WriteLine(e.Message);
            }
        }

        public void ValidateSchema(string inputURI ,XmlSchema schema)
        {
            XmlReaderSettings vacancySettings = new XmlReaderSettings();
            vacancySettings.Schemas.Add(schema);
            vacancySettings.ValidationType = ValidationType.Schema;
            vacancySettings.ValidationEventHandler += new ValidationEventHandler(ValidationCallbackOne);

            XmlReader books = XmlReader.Create(inputURI, vacancySettings);

            while (books.Read()) { }
        }
    }
}
