using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tests
{
    [TestClass]
    public class UnidadeProducaoGeneratorTest
    {
        UnidadeProducaoGenerator generator;

        [TestInitialize()]
        public void initialize()
        {
            generator = new UnidadeProducaoGenerator();
        }

        [TestMethod]
        public void DeveRetornarCodigoDaPropriedadeCom11Digitos()
        {
            Int64 result = generator.GerarCodigoPropriedade(3205200, 0001);
            Int64 expected = 32052000001;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeveRetornarCodigoDaUPCom17Digitos()
        {
            Int64 result = generator.GerarCodigoUnidadeProducao(32052000001, 16, 0001);
            Int64 expected = 32052000001160001;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeveRetornarSequencialCom4Digitos()
        {
            int sequencial = 32;
            String expected = "0032";

            Assert.AreEqual(expected, UnidadeProducaoGenerator.GetPaddedSequencial(sequencial));
        }
    }
}
