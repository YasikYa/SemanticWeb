using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Schema;

namespace XMLParsing.XML
{
    public interface ISchemaService
    {
        /// <summary>
        /// Creates an XML schema, and return builded schema.
        /// </summary>
        /// <param name="output">Output stream where compiled schema is saved</param>
        /// <returns></returns>
        XmlSchema BuildSchema(TextWriter output);

        void ValidateSchema(string inputURI, XmlSchema schema);
    }
}
