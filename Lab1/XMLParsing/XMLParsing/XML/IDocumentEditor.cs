using System;
using System.Collections.Generic;
using System.Text;

namespace XMLParsing.XML
{
    interface IDocumentEditor
    {
        void AddAttribute();
        void AddTextValue();
        void AddComplexTypeElement();
    }
}
