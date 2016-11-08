using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
    public class PragaValidar
    {
        #region Propriedades

		PragaDa _da = new PragaDa();

        #endregion



        public bool Salvar(Praga praga)
        {
            if (praga == null)
            {
                Validacao.Add(Mensagem.Praga.ObjetoNulo);
            }

            if (string.IsNullOrEmpty(praga.NomeCientifico))
            {
                Validacao.Add(Mensagem.Praga.NomeCientificoObrigatorio);
            }

            if (_da.Existe(praga))
            {
                Validacao.Add(Mensagem.Praga.JaExiste);
            }
                      
            return Validacao.EhValido;
        }

		internal bool AssociarCulturas(Praga praga)
		{

			List<string> culturasRepetidas = praga.Culturas.Where(x => praga.Culturas.Count(y => y.Id == x.Id) > 1)
				.Select(x => x.Nome.DeixarApenasUmEspaco())
				.ToList();

			culturasRepetidas.ForEach(x => Validacao.Add(Mensagem.Praga.CulturaExistente(x)));

			return Validacao.EhValido;
		}
	}
}
