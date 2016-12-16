using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business
{
    public class UnidadeProducaoGenerator
    {
        public static int sequencialLength = 4;

        public static Int64 GerarCodigoPropriedade(int codigoIbge, int sequencial)
        {
            String result = new StringBuilder()
                .Append(codigoIbge)
                .Append(GetPaddedSequencial(sequencial))
                .ToString();

            return Int64.Parse(result);
        }

        public static Int64 GerarCodigoUnidadeProducao(Int64 ibge, Int64 codigoPropriedade, String anoVigente, Int64 sequencialProducao)
        {
            String result = new StringBuilder()
                .Append(ibge)
                .Append(GetSequencialFromCodigoPropriedade(codigoPropriedade))
                .Append(anoVigente)
                .Append(GetPaddedSequencial(sequencialProducao))
                .ToString();

            return Int64.Parse(result);
        }

        public static String GetPaddedSequencial(dynamic sequencial)
        {
            return sequencial
                .ToString()
                .PadLeft(UnidadeProducaoGenerator.sequencialLength, '0');
        }

        public static String GetSequencialFromCodigoPropriedade(Int64 codigoPropriedade)
        {
            String codigoPropriedadeAsString = codigoPropriedade.ToString();
            int length = codigoPropriedadeAsString.Length;

            if (length <= sequencialLength)
            {
                return UnidadeProducaoGenerator.GetPaddedSequencial(codigoPropriedade);
            }

            int startIndex = length - sequencialLength;

            return codigoPropriedadeAsString.Substring(startIndex, sequencialLength);
        }

        public static bool CodigoUpHasCodigoPropriedade(Int64 codigoPropriedade, Int64 codigoUp)
        {
            String sequencial = GetSequencialFromCodigoPropriedade(codigoPropriedade);

            return (codigoUp.ToString().Substring(7, 4) == sequencial);
        }
    }
}
