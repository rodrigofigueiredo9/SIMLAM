using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data
{
	public class AtividadeCredenciadoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private string EsquemaBanco
		{
			get
			{
				return _configSys.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado);
			}
		}

		#endregion

		internal List<Lista> ObterAtividadesLista(int requerimentoId)
		{
			List<Lista> lst = new List<Lista>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{

				comando = bancoDeDados.CriarComando(@"select ta.id, ta.atividade, ta.codigo from {0}tab_requerimento_atividade t, {0}tab_atividade ta where t.atividade = ta.id
				and t.requerimento = :requerimento", EsquemaBanco);
				
				comando.AdicionarParametroEntrada("requerimento", requerimentoId);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new Lista()
					{
						Id = item.GetValue<string>("id"),
						Texto = item.GetValue<string>("atividade"),
						Codigo = item.GetValue<string>("codigo"),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<AtividadeSolicitada> ObterAtividadesListaReq(int requerimentoId)
		{
			var lst = new List<AtividadeSolicitada>();

			var sql = @"
			select ta.id Id, 
					ta.atividade Texto, 
					ta.situacao IsAtivo
				from tab_requerimento_atividade tr, 
					tab_atividade ta
				where tr.atividade = ta.id
				and tr.requerimento = :requerimentoId";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				using (var comando = bancoDeDados.CriarComando(sql, EsquemaBanco))
				{
					comando.AdicionarParametroEntrada("requerimentoId", requerimentoId);
					lst = bancoDeDados.ObterEntityList<AtividadeSolicitada>(comando);
				}
			}

			return lst;
		}
	}
}