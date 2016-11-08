using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLicenca.Data
{
	public class LicencaDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		private String EsquemaBanco { get; set; }

		#endregion

		public LicencaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal bool ExisteBarragem(int barragemId, BancoDeDados banco = null)
		{
			bool existeBarragem = false;
			object valor = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select count(1)
					  from {0}crt_barragem_barragens t
					 where t.id = :barragemId", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("barragemId", barragemId, DbType.Int32);

				valor = bancoDeDados.ExecutarScalar(comando);

				existeBarragem = valor != null && !Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0;
			}

			return existeBarragem;
		}
	}
}