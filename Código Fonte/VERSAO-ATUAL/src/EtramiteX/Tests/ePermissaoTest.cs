using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.Blocos.Entities.Interno.Security;

namespace Tests
{
    [TestClass]
    public class ePermissaoTest
    {
        [TestMethod]
        public void DeveTerEnumDeclaracaoAdicionalDefinido()
        {
            bool enumIsDefined = Enum.IsDefined(typeof(ePermissao), "DeclaracaoAdicional");

            if (!enumIsDefined)
            {
                Assert.Fail("Enum 'DeclaracaoAdicional' não está definido no arquivo ePermissao.cs");
            }

            Assert.IsTrue(enumIsDefined);
        }
    }
}