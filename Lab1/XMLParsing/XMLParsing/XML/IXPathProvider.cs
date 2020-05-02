using System;
using System.Collections.Generic;
using System.Text;

namespace XMLParsing.XML
{
    interface IXPathProvider
    {
        void CountVacancy();

        void SelectAttributes();

        void SelectInfoSingle();

        void SelectOnLength();

        void SelectFirstChild();

        void SelectSecondChild();

        void SelectThirdChild();

        void SelectOnStatus();

        void EveryFifthData();

        void OddCount();
    }
}
