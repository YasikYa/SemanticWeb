using System;
using System.Collections.Generic;
using System.Text;

namespace XMLParsing.XML
{
    interface IDocumentEditor
    {
        void AddAttribute();
        void RemoveAttribute();
        void AddTextValue();
        void AddComplexTypeElement();
        void SaveDocument();
        void AddComplex();
    }
}
