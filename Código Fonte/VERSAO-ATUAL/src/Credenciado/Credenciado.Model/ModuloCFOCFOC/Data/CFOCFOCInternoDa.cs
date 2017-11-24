using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data
{
	public class CFOCFOCInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public CFOCFOCInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public int SetarNumeroUtilizado(string numero, int tipoNumero, eDocumentoFitossanitarioTipo tipoDocumento, string serieNumero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
                Comando comando;
                if (string.IsNullOrEmpty(serieNumero))
                {
                    comando = bancoDeDados.CriarComando(@"
				    update tab_numero_cfo_cfoc t set t.utilizado = 1 
				    where t.tipo_documento = :tipo_documento and t.tipo_numero = :tipo_numero and t.numero = :numero and t.serie is null
				    returning t.id into :id", EsquemaBanco);
                }
                else
                {
                    comando = bancoDeDados.CriarComando(@"
				    update tab_numero_cfo_cfoc t set t.utilizado = 1 
				    where t.tipo_documento = :tipo_documento and t.tipo_numero = :tipo_numero and t.numero = :numero and serie = :serie 
				    returning t.id into :id", EsquemaBanco);
                    comando.AdicionarParametroEntrada("serie", serieNumero, DbType.String);
                }
				
				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);
				comando.AdicionarParametroEntrada("tipo_documento", (int)tipoDocumento, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_numero", tipoNumero, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int id = comando.ObterValorParametro<int>("id");

				Historico.Gerar(id, eHistoricoArtefato.numerocfocfoc, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();

				return id;
			}
		}

		internal bool VerificarCPFAssociadoALiberacao(string cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_liberacao_cfo_cfoc l, tab_credenciado c, {0}tab_pessoa p 
				where l.responsavel_tecnico = c.id and c.pessoa = p.id and p.cpf =:cpf", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("cpf", cpf, DbType.String);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal List<NumeroCFOCFOC> FiltrarConsulta(ConsultaFiltro filtro, BancoDeDados banco = null)
		{
			List<NumeroCFOCFOC> retorno = new List<NumeroCFOCFOC>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.* from tab_numero_cfo_cfoc t, hst_liberacao_cfo_cfoc h 
				where h.responsavel_tecnico_id = :credenciado_id and h.liberacao_id = t.liberacao and t.tipo_numero = :tipo_numero ");

				comando.AdicionarParametroEntrada("tipo_numero", filtro.TipoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado_id", User.FuncionarioId, DbType.Int32);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.numero", "numero", filtro.Numero);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.tipo_documento", "tipo_documento", filtro.TipoDocumento);

				if (!string.IsNullOrEmpty(filtro.DataInicialEmissao))
				{
					comando.DbCommand.CommandText += " and h.data_execucao >= :data_inicial ";
					comando.AdicionarParametroEntrada("data_inicial", filtro.DataInicialEmissao, DbType.DateTime);
				}

                if (!string.IsNullOrEmpty(filtro.Serie))
                {
                    comando.DbCommand.CommandText += " and t.serie = :serie ";
                    comando.AdicionarParametroEntrada("serie", filtro.Serie, DbType.String);
                }

				if (!string.IsNullOrEmpty(filtro.DataFinalEmissao))
				{
					comando.DbCommand.CommandText += " and h.data_execucao <= :data_final";
					comando.AdicionarParametroEntrada("data_final", filtro.DataFinalEmissao, DbType.DateTime);
				}

				comando.DbCommand.CommandText += " order by t.numero";

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					NumeroCFOCFOC item = null;
					while (reader.Read())
					{
						item = new NumeroCFOCFOC();

						item.Id = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<long>("numero");
                        item.Serie = reader.GetValue<string>("serie");
						item.Situacao = reader.GetValue<bool>("situacao");
						item.Tipo = reader.GetValue<int>("tipo_documento");
						item.TipoNumero = filtro.TipoNumero;
						item.Utilizado = reader.GetValue<bool>("utilizado");
						item.Motivo = reader.GetValue<string>("motivo");
						retorno.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}
	}
}