using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tests
{
    [TestClass]
    public class UnidadeProducaoGeneratorTest
    {
        [TestMethod]
        public void DeveRetornarCodigoDaPropriedadeCom11Digitos()
        {
            Int64 result = UnidadeProducaoGenerator.GerarCodigoPropriedade(3205200, 0001);
            Int64 expected = 32052000001;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeveRetornarCodigoDaUPCom17Digitos()
        {
            Int64 result = UnidadeProducaoGenerator.GerarCodigoUnidadeProducao(3205200, 33, "16", 1);
            Int64 expected = 32052000033160001;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DeveRetornarSequencialCom4Digitos()
        {
            int sequencial = 32;
            String expected = "0032";

            Assert.AreEqual(expected, UnidadeProducaoGenerator.GetPaddedSequencial(sequencial));
        }

        [TestMethod]
        public void DeveExtrairSequencialDoCodigoDaPropriedade()
        {
            String expected = 32052000001
                .ToString()
                .Substring(11 - 4, 4);

            Int64 codigoPropriedadeTest = 32052000001;

            Assert.AreEqual(expected, UnidadeProducaoGenerator.GetSequencialFromCodigoPropriedade(codigoPropriedadeTest));
        }

        [TestMethod]
        public void DeveRetornarCodigoDaPropriedadeSeTiver4DigitosOuMenos()
        {
            Assert.AreEqual("0001", UnidadeProducaoGenerator.GetSequencialFromCodigoPropriedade(1));
            Assert.AreEqual("0011", UnidadeProducaoGenerator.GetSequencialFromCodigoPropriedade(11));
            Assert.AreEqual("0111", UnidadeProducaoGenerator.GetSequencialFromCodigoPropriedade(111));
            Assert.AreEqual("1111", UnidadeProducaoGenerator.GetSequencialFromCodigoPropriedade(1111));
        }

        [TestMethod]
        public void DeveRetornarTrueSeCodigoDaPropriedadeEstiverNoCodigoUp()
        {
            Assert.IsTrue(UnidadeProducaoGenerator.CodigoUpHasCodigoPropriedade(32052000033, 32052000033160001));
        }

        [TestMethod]
        public void DeveRetornarFalseSeCodigoDaPropriedadeNaoEstiverNoCodigoUp()
        {
            Assert.IsFalse(UnidadeProducaoGenerator.CodigoUpHasCodigoPropriedade(32052000003, 32052000033160001));
        }
    }
}
