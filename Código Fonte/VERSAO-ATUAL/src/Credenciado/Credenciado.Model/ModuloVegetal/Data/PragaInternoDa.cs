using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Data
{
	public class PragaInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		private CulturaInternoDa _culturaDa = new CulturaInternoDa();

		#endregion

		#region Obter/Listar

		internal List<Cultura> ObterCulturas(int pragaId, BancoDeDados banco = null)
		{
			List<Cultura> retorno = new List<Cultura>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.texto nome from tab_praga p, tab_praga_cultura pc, tab_cultura c where pc.cultura = c.id 
				and pc.praga = p.id and p.id = :id");
				comando.AdicionarParametroEntrada("id", pragaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultura item;

					while (reader.Read())
					{
						item = new Cultura();
						item = _culturaDa.Obter(reader.GetValue<int>("id"));
						retorno.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion
	}
}