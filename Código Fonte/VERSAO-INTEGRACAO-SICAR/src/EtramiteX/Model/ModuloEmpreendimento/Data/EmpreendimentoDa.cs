using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloGeo.Business;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloEmpreendimento.Data
{
	public class EmpreendimentoDa
	{
		#region Empreendimento

		GerenciadorConfiguracao<ConfiguracaoEmpreendimento> _config = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private string EsquemaBanco { get; set; }
		private string EsquemaBancoGeo { get; set; }

		#endregion

		public EmpreendimentoDa(string strBancoDeDados = null, string strBancoDeDadosGeo = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}

			if (!string.IsNullOrEmpty(strBancoDeDadosGeo))
			{
				EsquemaBancoGeo = strBancoDeDadosGeo;
			}
		}

		#region Ações de DML

		public void Salvar(Empreendimento empreendimento, BancoDeDados banco = null)
		{
			if (empreendimento == null)
			{
				throw new Exception("Empreendimento é nulo.");
			}

			if (empreendimento.Id <= 0)
			{
				Criar(empreendimento, banco);
			}
			else
			{
				Editar(empreendimento, banco);
			}
		}

		internal int Criar(Empreendimento empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento e (id, codigo, segmento, cnpj, denominador, nome_fantasia, atividade, tid) 
				values ({0}seq_empreendimento.nextval, {0}seq_empreendimento_codigo.nextval, :segmento, :cnpj, :denominador, :nome_fantasia, :atividade, :tid) 
				returning e.id, e.codigo into :id, :codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, empreendimento.CNPJ);
				comando.AdicionarParametroEntrada("denominador", DbType.String, 100, empreendimento.Denominador);
				comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 100, empreendimento.NomeFantasia);
				comando.AdicionarParametroEntrada("segmento", empreendimento.Segmento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", (empreendimento.Atividade.Id > 0) ? empreendimento.Atividade.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroSaida("codigo", DbType.Int64);

				bancoDeDados.ExecutarNonQuery(comando);

				empreendimento.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				empreendimento.Tid = GerenciadorTransacao.ObterIDAtual();
				empreendimento.Codigo = Convert.ToInt64(comando.ObterValorParametro("codigo"));

				#endregion

				#region Responsáveis

				if (empreendimento.Responsaveis.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_responsavel(id, empreendimento, responsavel, tipo, data_vencimento, especificar, 
					origem, origem_texto, credenciado_usuario_id, tid) values({0}seq_empreendimento_responsavel.nextval, :empreendimento, :responsavel, :tipo, :data_vencimento, 
					:especificar, :origem, :origem_texto, :credenciado_usuario_id, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("responsavel", DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", DbType.Int32);
					comando.AdicionarParametroEntrada("data_vencimento", DbType.DateTime);
					comando.AdicionarParametroEntrada("especificar", DbType.String);
					comando.AdicionarParametroEntrada("credenciado_usuario_id", DbType.Int32);
					comando.AdicionarParametroEntrada("origem", DbType.Int32);
					comando.AdicionarParametroEntrada("origem_texto", DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Responsavel item in empreendimento.Responsaveis)
					{
						comando.SetarValorParametro("responsavel", item.Id);
						comando.SetarValorParametro("tipo", item.Tipo);
						comando.SetarValorParametro("data_vencimento", (item.DataVencimento.HasValue && item.DataVencimento.Value != DateTime.MinValue) ? item.DataVencimento : null);
						comando.SetarValorParametro("especificar", (String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto));
						comando.SetarValorParametro("credenciado_usuario_id", item.CredenciadoUsuarioId);
						comando.SetarValorParametro("origem", item.Origem);
						comando.SetarValorParametro("origem_texto", item.OrigemTexto);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Endereço

				if (empreendimento.Enderecos != null && empreendimento.Enderecos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_endereco (id, empreendimento, correspondencia, zona, cep, logradouro, bairro, estado, municipio, numero, distrito, corrego, caixa_postal, complemento, tid)
					values({0}seq_empreendimento_endereco.nextval, :empreendimento, :correspondencia, :zona, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :corrego, :caixa_postal, :complemento, :tid )", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("correspondencia", DbType.Int32);
					comando.AdicionarParametroEntrada("zona", DbType.Int32);
					comando.AdicionarParametroEntrada("cep", DbType.String, 15);
					comando.AdicionarParametroEntrada("logradouro", DbType.String, 500);
					comando.AdicionarParametroEntrada("bairro", DbType.String, 100);
					comando.AdicionarParametroEntrada("estado", DbType.Int32);
					comando.AdicionarParametroEntrada("municipio", DbType.Int32);
					comando.AdicionarParametroEntrada("numero", DbType.String, 6);
					comando.AdicionarParametroEntrada("distrito", DbType.String, 100);
					comando.AdicionarParametroEntrada("corrego", DbType.String, 100);
					comando.AdicionarParametroEntrada("caixa_postal", DbType.String, 50);
					comando.AdicionarParametroEntrada("complemento", DbType.String, 500);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Endereco item in empreendimento.Enderecos)
					{
						comando.SetarValorParametro("correspondencia", item.Correspondencia);
						comando.SetarValorParametro("zona", item.ZonaLocalizacaoId);
						comando.SetarValorParametro("cep", item.Cep);
						comando.SetarValorParametro("logradouro", item.Logradouro);
						comando.SetarValorParametro("bairro", item.Bairro);
						comando.SetarValorParametro("estado", item.EstadoId);
						comando.SetarValorParametro("municipio", item.MunicipioId);
						comando.SetarValorParametro("numero", item.Numero);
						comando.SetarValorParametro("distrito", item.DistritoLocalizacao);
						comando.SetarValorParametro("corrego", item.Corrego);
						comando.SetarValorParametro("caixa_postal", item.CaixaPostal);
						comando.SetarValorParametro("complemento", item.Complemento);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Coordenadas/Geometria

				if (empreendimento.Coordenada.Tipo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_coord
					(id, empreendimento, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, latitude_gms, longitude_gms, latitude_gdec, longitude_gdec, forma_coleta, local_coleta, tid)
					values ({0}seq_empreendimento_coord.nextval, :empreendimento, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :latitude_gms, 
					:longitude_gms, :latitude_gdec, :longitude_gdec, :forma_coleta, :local_coleta, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_coordenada", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("easting_utm", empreendimento.Coordenada.EastingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("northing_utm", empreendimento.Coordenada.NorthingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("fuso_utm", empreendimento.Coordenada.FusoUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio_utm", empreendimento.Coordenada.HemisferioUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("latitude_gms", DbType.String, 15, empreendimento.Coordenada.LatitudeGms);
					comando.AdicionarParametroEntrada("longitude_gms", DbType.String, 15, empreendimento.Coordenada.LongitudeGms);
					comando.AdicionarParametroEntrada("latitude_gdec", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("longitude_gdec", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("forma_coleta", empreendimento.Coordenada.FormaColeta, DbType.Int32);
					comando.AdicionarParametroEntrada("local_coleta", empreendimento.Coordenada.LocalColeta, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					#region Geometria

					comando = bancoDeDados.CriarComando(@"declare"
						+ " v_x number;"
						+ " v_y number;"
						+ " v_fuso number;"
						+ " v_hemisferio number;"
						+ " geo mdsys.sdo_geometry;"
					+ " begin"
						+ " case :tipo"
							+ " when 1 then /*Grau, minuto e Segundo : Geografico*/"
								+ " {0}coordenada.gms2gdec({0}coordenada.formatagms(:x), {0}coordenada.formatagms(:y), v_x, v_y);"
							+ " when 2 then /*Grau decimal: Geografico*/"
								+ " v_x := :x;"
								+ " v_y := :y;"
							+ " when 3 then /*UTM: Projetado*/"
								+ " {0}coordenada.utm2gdec(:datum_sigla, :x, :y, :fuso, :hemisferio, v_x, v_y);"
						+ " end case;"
						+ " geo := {0}coordenada.gdec2spatialpoint(v_x, v_y, :datum);"
						+ " insert into {0}geo_emp_localizacao(id,empreendimento,geometry,tid) values ({0}seq_geo_emp_localizacao.nextval, :empreendimento, geo, :tid);"
					+ " end;", EsquemaBancoGeo);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum_sigla", empreendimento.Coordenada.Datum.Sigla, DbType.String);
					comando.AdicionarParametroEntrada("fuso", DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio", DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					if (empreendimento.Coordenada.Tipo.Id == 1)
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGms, DbType.String);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGms, DbType.String);
					}
					else if (empreendimento.Coordenada.Tipo.Id == 2)
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
					}
					else
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.EastingUtm, DbType.Double);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.NorthingUtm, DbType.Double);

						comando.SetarValorParametro("fuso", empreendimento.Coordenada.FusoUtm.Value);
						comando.SetarValorParametro("hemisferio", empreendimento.Coordenada.HemisferioUtm.Value);
					}

					bancoDeDados.ExecutarNonQuery(comando);
					Comando cmd = bancoDeDados.CriarComando("alter session set NLS_NUMERIC_CHARACTERS = ',.'");
					bancoDeDados.ExecutarNonQuery(cmd);

					#endregion
				}

				#endregion

				#region Meios de contato

				if (empreendimento.MeiosContatos != null && empreendimento.MeiosContatos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_contato(id, empreendimento, meio_contato, valor, tid) values
					({0}seq_empreendimento_contato.nextval, :empreendimento, :meio_contato, :valor, : tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("meio_contato", DbType.Int32);
					comando.AdicionarParametroEntrada("valor", DbType.String, 1000);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Contato item in empreendimento.MeiosContatos)
					{
						comando.SetarValorParametro("meio_contato", Convert.ToInt32(item.TipoContato));
						comando.SetarValorParametro("valor", item.Valor);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico/Consulta/Posse

				Historico.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, eHistoricoAcao.criar, bancoDeDados);
				Consulta.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return empreendimento.Id;
			}
		}

		internal void Editar(Empreendimento empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Empreendimento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento e set e.segmento = :segmento, e.cnpj = :cnpj, e.denominador = :denominador, 
				e.nome_fantasia = :nome_fantasia, e.atividade = :atividade, e.tid = :tid where e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("cnpj", DbType.String, 20, empreendimento.CNPJ);
				comando.AdicionarParametroEntrada("denominador", DbType.String, 100, empreendimento.Denominador);
				comando.AdicionarParametroEntrada("nome_fantasia", DbType.String, 100, empreendimento.NomeFantasia);
				comando.AdicionarParametroEntrada("segmento", empreendimento.Segmento ?? (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", (empreendimento.Atividade.Id > 0) ? empreendimento.Atividade.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				empreendimento.Tid = GerenciadorTransacao.ObterIDAtual();

				#endregion

				#region Limpar os dados do banco

				//Meios de Contato
				comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_contato c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.MeiosContatos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Responsavel
				comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_responsavel c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.Responsaveis.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Endereço
				comando = bancoDeDados.CriarComando("delete from {0}tab_empreendimento_endereco c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.empreendimento = :empreendimento{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, empreendimento.Enderecos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Responsáveis

				if (empreendimento.Responsaveis != null && empreendimento.Responsaveis.Count > 0)
				{
					foreach (Responsavel item in empreendimento.Responsaveis)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update tab_empreendimento_responsavel r set r.empreendimento = :empreendimento, r.responsavel = :responsavel, 
							r.tipo = :tipo, r.data_vencimento = :data_vencimento, r.especificar = :especificar, r.origem = :origem, r.origem_texto = :origem_texto, 
							r.credenciado_usuario_id = :credenciado_usuario_id, r.tid = :tid where r.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_responsavel(id, empreendimento, responsavel, tipo, data_vencimento, especificar, 
							origem, origem_texto, credenciado_usuario_id, tid) values ({0}seq_empreendimento_responsavel.nextval, :empreendimento, :responsavel, :tipo, 
							:data_vencimento, :especificar, :origem, :origem_texto, :credenciado_usuario_id, :tid )", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("data_vencimento", (item.DataVencimento.HasValue && item.DataVencimento.Value != DateTime.MinValue) ? item.DataVencimento : null, DbType.DateTime);
						comando.AdicionarParametroEntrada("especificar", (String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto), DbType.String);
						comando.AdicionarParametroEntrada("credenciado_usuario_id", item.CredenciadoUsuarioId, DbType.Int32);
						comando.AdicionarParametroEntrada("origem", item.Origem, DbType.Int32);
						comando.AdicionarParametroEntrada("origem_texto", DbType.String, 200, item.OrigemTexto);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_empreendimento_responsavel p where p.empreendimento = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Endereço

				if (empreendimento.Enderecos != null && empreendimento.Enderecos.Count > 0)
				{
					foreach (Endereco item in empreendimento.Enderecos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_endereco e set e.empreendimento = :empreendimento, e.correspondencia = :correspondencia, e.zona = :zona, 
							e.cep = :cep, e.logradouro = :logradouro, e.bairro = :bairro, e.estado = :estado, e.municipio = :municipio, e.numero = :numero, e.distrito =:distrito, e.corrego = :corrego, 
							e.caixa_postal = :caixa_postal, e.complemento = :complemento, e.tid =:tid where e.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_endereco (id, empreendimento, correspondencia, zona, cep, logradouro, bairro, estado, municipio, numero, distrito, corrego,
							caixa_postal, complemento, tid) values({0}seq_empreendimento_endereco.nextval, :empreendimento, :correspondencia, :zona, :cep, :logradouro, :bairro, :estado, :municipio, :numero, :distrito, :corrego, :caixa_postal, :complemento, :tid )", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("correspondencia", item.Correspondencia, DbType.Int32);
						comando.AdicionarParametroEntrada("zona", item.ZonaLocalizacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("cep", DbType.String, 15, item.Cep);
						comando.AdicionarParametroEntrada("logradouro", DbType.String, 500, item.Logradouro);
						comando.AdicionarParametroEntrada("bairro", DbType.String, 100, item.Bairro);
						comando.AdicionarParametroEntrada("estado", item.EstadoId, DbType.Int32);
						comando.AdicionarParametroEntrada("municipio", item.MunicipioId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", DbType.String, 6, item.Numero);
						comando.AdicionarParametroEntrada("distrito", DbType.String, 100, item.DistritoLocalizacao);
						comando.AdicionarParametroEntrada("corrego", DbType.String, 100, item.Corrego);
						comando.AdicionarParametroEntrada("caixa_postal", DbType.String, 50, item.CaixaPostal);
						comando.AdicionarParametroEntrada("complemento", DbType.String, 500, item.Complemento);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Coordenadas/Geometria

				if (empreendimento.Coordenada.Tipo.Id > 0)
				{
					if (empreendimento.Coordenada.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_coord tc set tipo_coordenada = :tipo_coordenada, datum = :datum, easting_utm = :easting_utm, 
						northing_utm = :northing_utm, fuso_utm = :fuso_utm, hemisferio_utm = :hemisferio_utm, latitude_gms = :latitude_gms, longitude_gms = :longitude_gms, latitude_gdec = :latitude_gdec, 
						longitude_gdec = :longitude_gdec, forma_coleta = :forma_coleta, local_coleta = :local_coleta, tid = :tid where empreendimento = :empreendimento", EsquemaBanco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_coord
						(id, empreendimento, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, latitude_gms, longitude_gms, latitude_gdec, longitude_gdec, forma_coleta, local_coleta, tid)
						values ({0}seq_empreendimento_coord.nextval, :empreendimento, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :latitude_gms, 
						:longitude_gms, :latitude_gdec, :longitude_gdec, :forma_coleta, :local_coleta, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_coordenada", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("easting_utm", empreendimento.Coordenada.EastingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("northing_utm", empreendimento.Coordenada.NorthingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("fuso_utm", empreendimento.Coordenada.FusoUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio_utm", empreendimento.Coordenada.HemisferioUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("latitude_gms", DbType.String, 15, empreendimento.Coordenada.LatitudeGms);
					comando.AdicionarParametroEntrada("longitude_gms", DbType.String, 15, empreendimento.Coordenada.LongitudeGms);
					comando.AdicionarParametroEntrada("latitude_gdec", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("longitude_gdec", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("forma_coleta", empreendimento.Coordenada.FormaColeta, DbType.Int32);
					comando.AdicionarParametroEntrada("local_coleta", empreendimento.Coordenada.LocalColeta, DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					#region Geometria

					comando = bancoDeDados.CriarComando(@"declare"
						+ " v_x number;"
						+ " v_y number;"
						+ " v_id number;"
						+ " v_fuso number;"
						+ " v_hemisferio number;"
						+ " geo mdsys.sdo_geometry;"
					+ " begin"
						+ " case :tipo"
							+ " when 1 then /*Grau, minuto e Segundo : Geografico*/"
								+ " {0}coordenada.gms2gdec({0}coordenada.formatagms(:x), {0}coordenada.formatagms(:y), v_x, v_y);"
							+ " when 2 then /*Grau decimal: Geografico*/"
								+ " v_x := :x;"
								+ " v_y := :y;"
							+ " when 3 then /*UTM: Projetado*/"
								+ " {0}coordenada.utm2gdec(:datum_sigla, :x, :y, :fuso, :hemisferio, v_x, v_y);"
						+ " end case;"
						+ " geo := {0}coordenada.gdec2spatialpoint(v_x, v_y, :datum);"
						+ " select count(*) into v_id from {0}geo_emp_localizacao g where g.empreendimento = :empreendimento;"
						+ " if(v_id=0)then"
							+ " insert into {0}geo_emp_localizacao(id,empreendimento,geometry,tid) values ({0}seq_geo_emp_localizacao.nextval, :empreendimento, geo, :tid);"
						+ " else"
							+ " update {0}geo_emp_localizacao g set g.geometry = geo, g.tid = tid where g.empreendimento = :empreendimento;"
						+ " end if;"
					+ " end;", EsquemaBancoGeo);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", empreendimento.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", empreendimento.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum_sigla", empreendimento.Coordenada.Datum.Sigla, DbType.String);
					comando.AdicionarParametroEntrada("fuso", DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio", DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					if (empreendimento.Coordenada.Tipo.Id == 1)
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGms, DbType.String);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGms, DbType.String);
					}
					else if (empreendimento.Coordenada.Tipo.Id == 2)
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.LongitudeGdec, DbType.Double);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.LatitudeGdec, DbType.Double);
					}
					else
					{
						comando.AdicionarParametroEntrada("x", empreendimento.Coordenada.EastingUtm, DbType.Double);
						comando.AdicionarParametroEntrada("y", empreendimento.Coordenada.NorthingUtm, DbType.Double);

						comando.SetarValorParametro("fuso", empreendimento.Coordenada.FusoUtm.Value);
						comando.SetarValorParametro("hemisferio", empreendimento.Coordenada.HemisferioUtm.Value);

					}

					bancoDeDados.ExecutarNonQuery(comando);
					Comando cmd = bancoDeDados.CriarComando("alter session set NLS_NUMERIC_CHARACTERS = ',.'");
					bancoDeDados.ExecutarNonQuery(cmd);

					#endregion
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}tab_empreendimento_coord ae where ae.empreendimento = :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", empreendimento.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Meios de contato

				if (empreendimento.MeiosContatos != null && empreendimento.MeiosContatos.Count > 0)
				{
					foreach (Contato item in empreendimento.MeiosContatos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento_contato tec set tec.meio_contato = :meio_contato, tec.valor = :valor, tec.tid = :tid
							where tec.empreendimento = :empreendimento and tec.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_empreendimento_contato(id, empreendimento, meio_contato, valor, tid) values 
							({0}seq_empreendimento_contato.nextval, :empreendimento, :meio_contato, :valor, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("meio_contato", Convert.ToInt32(item.TipoContato), DbType.Int32);
						comando.AdicionarParametroEntrada("valor", DbType.String, 1000, item.Valor);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico/Consulta/Posse

				Historico.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, eHistoricoAcao.atualizar, bancoDeDados);
				Consulta.Gerar(empreendimento.Id, eHistoricoArtefato.empreendimento, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_empreendimento c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.empreendimento, eHistoricoAcao.excluir, bancoDeDados);
				Consulta.Deletar(id, eHistoricoArtefato.empreendimento, bancoDeDados);

				#endregion

				#region Apaga os dados de empreendimento
				//tmp_projeto_geo_arquivos
				//tmp_projeto_geo_ortofoto
				comando = bancoDeDados.CriarComando("begin " +
					" delete {0}tmp_projeto_geo_arquivos t where t.projeto in (select a.id from {0}tmp_projeto_geo a where a.empreendimento = :empreendimento); " +
					" delete {0}tmp_projeto_geo_ortofoto t where t.projeto in (select a.id from {0}tmp_projeto_geo a where a.empreendimento = :empreendimento); " +
					" delete {0}tmp_projeto_geo a where a.empreendimento = :empreendimento; " +
					" delete {1}geo_emp_localizacao a where a.empreendimento = :empreendimento;" +
					" delete {0}tmp_projeto_geo a where a.empreendimento = :empreendimento;" +
					" delete {0}tab_empreendimento_responsavel a where a.empreendimento = :empreendimento;" +
					" delete {0}tab_empreendimento_endereco a where a.empreendimento = :empreendimento;" +
					" delete {0}tab_empreendimento_coord a where a.empreendimento = :empreendimento;" +
					" delete {0}tab_empreendimento_contato a where a.empreendimento = :empreendimento;" +
					" delete {0}tab_empreendimento a where a.id = :empreendimento;" +
				"end;", EsquemaBanco, EsquemaBancoGeo);
				comando.AdicionarParametroEntrada("empreendimento", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Validações

		public bool EmPosse(int empreendimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select (select(select count(*) from {0}tab_protocolo p where p.empreendimento = :empreendimento 
				and p.emposse = :funcionario and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id))
				+ (select count(*) from {0}tab_protocolo_associado pa, {0}tab_protocolo p, {0}tab_protocolo a where pa.protocolo = p.id and 
				p.emposse = :funcionario and pa.associado = a.id and a.empreendimento = :empreendimento and not exists (select 1 from {0}tab_tramitacao t where t.protocolo = p.id)            
				) valor from dual) emposse,(select count(*) from {0}tab_protocolo t where t.empreendimento = :empreendimento) existe from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", User.FuncionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						return (Convert.ToBoolean(reader["existe"]) && Convert.ToBoolean(reader["emposse"])) || (!Convert.ToBoolean(reader["existe"]));
					}

					reader.Close();
				}
			}

			return false;
		}

		public bool ExisteMunicipio(int MunicipioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}lov_municipio lm where lm.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", MunicipioId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ExisteEstado(int EstadoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}lov_estado le where le.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", EstadoId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool EstaNaAbrangencia(Coordenada coord)
		{
			bool flag = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Geometria

				Comando comando = bancoDeDados.CriarComando(@"declare"
				+ " v_x number;"
				+ " v_y number;"
				+ " v_cont number:=0; "
				+ " geo mdsys.sdo_geometry;"
				+ " begin"
				+ " if(:tipo = 1) then"
				+ " {0}coordenada.gms2gdec(validacoes.formatagms(:x), validacoes.formatagms(:y), v_x, v_y);"
				+ " end if;"
				+ " if(:tipo = 3) then"
				+ " {0}coordenada.utm2gdec(:datum_sigla, :x, :y, :fuso, 1, v_x, v_y);"
				+ " end if;"
				+ "   v_cont := {0}coordenada.EstaNaAbrangencia(v_x, v_y, true);"
				+ "   :result := v_cont;"
				+ " end;", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("datum", coord.Datum.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("fuso", coord.FusoUtm, DbType.Int32);
				comando.AdicionarParametroEntrada("datum_sigla", coord.Datum.Sigla, DbType.String);
				comando.AdicionarParametroEntrada("tipo", coord.Tipo.Id, DbType.Int32);
				comando.AdicionarParametroSaida("result", DbType.Int32);

				if (coord.Tipo.Id == 1)
				{
					comando.AdicionarParametroEntrada("x", coord.LongitudeGms, DbType.Double);
					comando.AdicionarParametroEntrada("y", coord.LatitudeGms, DbType.Double);
				}
				else if (coord.Tipo.Id == 2)
				{
					comando.AdicionarParametroEntrada("x", coord.LongitudeGdec, DbType.Double);
					comando.AdicionarParametroEntrada("y", coord.LatitudeGdec, DbType.Double);
				}
				else
				{
					comando.AdicionarParametroEntrada("x", coord.EastingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("y", coord.NorthingUtm, DbType.Double);
				}

				#endregion

				bancoDeDados.ExecutarNonQuery(comando);
				flag = comando.ObterValorParametro("result").ToString() != "0";
			}

			return flag;
		}

		public bool ExisteCnpj(string cnpj, Int32? id = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(cnpj) from {0}tab_empreendimento te where te.cnpj = :cnpj", EsquemaBanco);

				if ((id ?? 0) > 0)
				{
					comando.DbCommand.CommandText += " and te.id != :id";
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public int Existe(string cnpj, Int32? id = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select te.id from {0}tab_empreendimento te where te.cnpj = :cnpj", EsquemaBanco);

				if ((id ?? 0) > 0)
				{
					comando.DbCommand.CommandText += " and te.id != :id";
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);

				object ret = bancoDeDados.ExecutarScalar(comando);

				return (ret != null && !Convert.IsDBNull(ret) && Convert.ToInt32(ret) > 0) ? Convert.ToInt32(ret) : 0;
			}
		}

		public int ObterId(string cnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select te.id from {0}tab_empreendimento te where te.cnpj = :cnpj", EsquemaBanco);

				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);

				object ret = bancoDeDados.ExecutarScalar(comando);

				return (ret != null && !Convert.IsDBNull(ret) && Convert.ToInt32(ret) > 0) ? Convert.ToInt32(ret) : 0;
			}
		}

		public List<String> VerificarEmpreendimentoRequerimento(int id, BancoDeDados banco = null)
		{
			List<String> emp = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.numero from {0}tab_requerimento r where r.empreendimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emp.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
			}

			return emp;
		}

		public List<String> VerificarEmpreendimentoDocumento(int id, BancoDeDados banco = null)
		{
			List<String> emp = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.numero||'/'||d.ano numero from {0}tab_protocolo d where d.protocolo = 2 and d.empreendimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emp.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
			}

			return emp;
		}

		public List<String> VerificarEmpreendimentoProcesso(int id, BancoDeDados banco = null)
		{
			List<String> emp = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.numero||'/'||d.ano numero from {0}tab_protocolo d where d.protocolo = 1 and d.empreendimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emp.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
			}

			return emp;
		}

		public List<String> VerificarEmpreendimentoTitulo(int id, BancoDeDados banco = null)
		{
			List<String> lista = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select m.nome||case when t.numero is not null then ': '||t.numero||'/'||t.ano else null end numero 
				from {0}tab_titulo e, {0}tab_titulo_modelo m, {0}tab_titulo_numero t where e.modelo = m.id and e.id = t.titulo(+) and e.empreendimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
			}

			return lista;
		}

		public List<String> VerificarEmpreendimentoFiscalizacoes(int id, BancoDeDados banco = null)
		{
			List<String> lista = new List<string>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id numero from {0}tab_fiscalizacao f, {0}tab_fisc_local_infracao l where f.id = l.fiscalizacao and l.empreendimento = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(reader["numero"].ToString());
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal bool PontoForaMBR(double easting, double northing, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare
					resposta number(1) := 0;
					ordenadas sdo_ordinate_array;
				begin
					ordenadas := sdo_ordinate_array(:easting, :northing);

					if (ordenadas(1)<-3720618.6798069715 
					or ordenadas(2)<5916693.2686790377 
					or ordenadas(1)>1670563.45910363365
					or ordenadas(2)>10704234.897805285) 
					then
						resposta := 1;
					end if;

					select resposta into :saida from dual;
				end;", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("easting", easting, DbType.Double);
				comando.AdicionarParametroEntrada("northing", northing, DbType.Double);
				comando.AdicionarParametroSaida("saida", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				return Convert.ToInt32(comando.ObterValorParametro("saida")) > 0;
			}
		}

		#endregion

		#region Obter / Filtrar

		public Municipio ObterMunicipio(int MunicipioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Municipio municipio = new Municipio();
				Comando comando = bancoDeDados.CriarComando(@"select m.id, m.texto, m.estado, m.cep, m.ibge from {0}lov_municipio m where m.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", MunicipioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						municipio.Id = Convert.ToInt32(reader["id"]);
						municipio.Estado.Id = Convert.ToInt32(reader["estado"]);
						municipio.Ibge = Convert.IsDBNull(reader["ibge"]) ? 0 : Convert.ToInt32(reader["ibge"]);
						municipio.Texto = reader["texto"].ToString();
						municipio.Cep = reader["texto"].ToString();
						municipio.IsAtivo = true;
					}

					reader.Close();
				}

				return municipio;
			}
		}

		public Empreendimento Obter(int id, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento, ls.texto segmento_texto, ls.denominador segmento_denominador, e.cnpj, e.denominador, 
				e.nome_fantasia, e.atividade, a.atividade atividade_texto, e.tid from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls 
				where e.atividade = a.id(+) and e.segmento = ls.id and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.Codigo = reader.GetValue<long>("codigo");

						if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
						{
							empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
							empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
							empreendimento.SegmentoDenominador = reader["segmento_denominador"].ToString();
						}

						empreendimento.CNPJ = reader["cnpj"].ToString();
						empreendimento.Denominador = reader["denominador"].ToString();
						empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
							empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.id, pr.empreendimento, pr.responsavel, nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo, 
				lr.texto tipo_texto, pr.data_vencimento, pr.especificar, pr.origem, pr.origem_texto, pr.credenciado_usuario_id 
				from {0}tab_empreendimento_responsavel pr, {0}tab_pessoa p, {0}lov_empreendimento_tipo_resp lr where pr.responsavel = p.id and pr.tipo = lr.id 
				and pr.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Responsavel responsavel;

					while (reader.Read())
					{
						responsavel = new Responsavel();
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
						responsavel.Tipo = Convert.ToInt32(reader["tipo"]);
						responsavel.TipoTexto = reader["tipo_texto"].ToString();

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
						}

						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.EspecificarTexto = reader["especificar"].ToString();

						responsavel.Origem = reader.GetValue<int>("origem");
						responsavel.OrigemTexto = reader.GetValue<string>("origem_texto");
						responsavel.CredenciadoUsuarioId = reader.GetValue<int>("credenciado_usuario_id");

						empreendimento.Responsaveis.Add(responsavel);
					}

					reader.Close();
				}

				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"select te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, le.id estado_id, le.texto estado_texto, lm.id municipio_id, 
				lm.texto municipio_texto, te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid from {0}tab_empreendimento_endereco te, {0}lov_estado le, {0}lov_municipio lm where te.estado = le.id(+) 
				and te.municipio = lm.id(+) and te.empreendimento = :empreendimento order by te.correspondencia", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Endereco end;

					while (reader.Read())
					{
						end = new Endereco();
						end.Id = Convert.ToInt32(reader["id"]);
						end.Tid = reader["tid"].ToString();
						end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
						end.ZonaLocalizacaoId = Convert.IsDBNull(reader["zona"]) ? 0 : Convert.ToInt32(reader["zona"]);
						end.Cep = reader["cep"].ToString();
						end.Logradouro = reader["logradouro"].ToString();
						end.Bairro = reader["bairro"].ToString();
						end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						end.EstadoTexto = reader["estado_texto"].ToString();
						end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						end.MunicipioTexto = reader["municipio_texto"].ToString();
						end.Numero = reader["numero"].ToString();
						end.DistritoLocalizacao = reader["distrito"].ToString();
						end.Corrego = reader["corrego"].ToString();
						end.CaixaPostal = reader["caixa_postal"].ToString();
						end.Complemento = reader["complemento"].ToString();
						empreendimento.Enderecos.Add(end);
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, lct.texto tipoTexto, aec.datum, lcd.texto datumTexto, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, lch.texto hemisferioTexto, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
				aec.forma_coleta,   aec.local_coleta  from tab_empreendimento_coord aec, lov_coordenada_datum lcd, lov_coordenada_tipo lct, lov_coordenada_hemisferio lch
				where aec.datum = lcd.id(+) and aec.hemisferio_utm = lch.id(+) and aec.tipo_coordenada = lct.id(+) and aec.empreendimento = :empreendimentoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

						if (!Convert.IsDBNull(reader["datumtexto"]))
						{
							empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["tipotexto"]))
						{
							empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["hemisferiotexto"]))
						{
							empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Meio de Contato

				comando = bancoDeDados.CriarComando(@"select a.id, b.mascara, a.empreendimento, b.id tipo_contato_id, b.texto contato_texto,
				a.valor, a.tid from {0}tab_empreendimento_contato a, {0}tab_meio_contato b where a.meio_contato = b.id and a.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Contato contato;

					while (reader.Read())
					{
						contato = new Contato();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = Convert.ToInt32(reader["empreendimento"]);
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.TipoTexto = reader["contato_texto"].ToString();
						contato.Mascara = reader["mascara"].ToString();
						contato.Valor = reader["valor"].ToString();
						contato.Tid = reader["tid"].ToString();
						empreendimento.MeiosContatos.Add(contato);
					}

					reader.Close();
				}

				#endregion

				empreendimento.TemCorrespondencia = (empreendimento.Enderecos.Any(x => x.Correspondencia == 1) ? 1 : 0);
			}

			return empreendimento;
		}

		public Empreendimento ObterSimplificado(int id, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento, ls.texto segmento_texto, e.cnpj, e.denominador, e.nome_fantasia, e.atividade, 
				a.atividade atividade_texto, e.tid from {0}tab_empreendimento e, {0}tab_empreendimento_atividade a, {0}lov_empreendimento_segmento ls 
				where e.atividade = a.id(+) and e.segmento = ls.id and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = reader.GetValue<string>("tid");
						empreendimento.Codigo = reader.GetValue<long>("codigo");

						if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
						{
							empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
							empreendimento.SegmentoTexto = reader["segmento"].ToString();
						}

						empreendimento.CNPJ = reader["cnpj"].ToString();
						empreendimento.Denominador = reader["denominador"].ToString();
						empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
							empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, lct.texto tipoTexto, aec.datum, lcd.texto datumTexto, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, lch.texto hemisferioTexto, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
				aec.forma_coleta,   aec.local_coleta  from tab_empreendimento_coord aec, lov_coordenada_datum lcd, lov_coordenada_tipo lct, lov_coordenada_hemisferio lch
				where aec.datum = lcd.id(+) and aec.hemisferio_utm = lch.id(+) and aec.tipo_coordenada = lct.id(+) and aec.empreendimento = :empreendimentoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

						if (!Convert.IsDBNull(reader["datumtexto"]))
						{
							empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["tipotexto"]))
						{
							empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["hemisferiotexto"]))
						{
							empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		internal Empreendimento Obter(String cnpj, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_empreendimento p where p.cnpj = :cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					empreendimento = Obter(Convert.ToInt32(id));
				}
			}

			return empreendimento;
		}

		internal Empreendimento ObterSimplificado(String cnpj, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_empreendimento p where p.cnpj = :cnpj", EsquemaBanco);
				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);
				Object id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					empreendimento = ObterSimplificado(Convert.ToInt32(id));
				}
			}

			return empreendimento;
		}

		internal String ObterProfissao(int id, BancoDeDados banco = null)
		{
			String profissaoTexto = string.Empty;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select texto from tab_profissao p where p.id = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				profissaoTexto = bancoDeDados.ExecutarScalar(comando).ToString();

			}

			return profissaoTexto;
		}

		public List<EmpreendimentoAtividade> Atividades(BancoDeDados banco = null)
		{
			List<EmpreendimentoAtividade> atividades = new List<EmpreendimentoAtividade>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.secao, a.divisao, a.atividade from {0}tab_empreendimento_atividade a order by a.atividade ASC", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						EmpreendimentoAtividade atividade = new EmpreendimentoAtividade();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.Atividade = reader["atividade"].ToString();
						atividade.Divisao = Convert.IsDBNull(reader["divisao"]) ? 0 : Convert.ToInt32(reader["divisao"]);
						atividade.Secao = Convert.IsDBNull(reader["secao"]) ? "" : reader["secao"].ToString();
						atividades.Add(atividade);
					}

					reader.Close();
				}
			}

			return atividades;
		}

		public Resultados<Empreendimento> Filtrar(Filtro<ListarEmpreendimentoFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Empreendimento> retorno = new Resultados<Empreendimento>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.codigo", "codigo", filtros.Dados.Codigo);

				comandtxt += comando.FiltroAndLike("e.denominador", "denominador", filtros.Dados.Denominador, true);

				comandtxt += comando.FiltroAndLike("e.cnpj", "cnpj", filtros.Dados.CNPJ);

				comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select r.empreendimento from {0}tab_empreendimento_responsavel r, {0}lst_pessoa p where p.pessoa_id = r.responsavel 
				and p.cpf_cnpj like :responsavel_cpf_cnpj ||'%'", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "responsavel_cpf_cnpj", filtros.Dados.Responsavel.CpfCnpj);

				if (!ValidacoesGenericasBus.Cpf(filtros.Dados.Responsavel.CpfCnpj) && !ValidacoesGenericasBus.Cnpj(filtros.Dados.Responsavel.CpfCnpj))
				{
					comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select r.empreendimento from {0}tab_empreendimento_responsavel r, {0}lst_pessoa p  
					where p.pessoa_id = r.responsavel and upper(p.nome_razao_social) like upper('%'|| :responsavel_nome ||'%')", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "responsavel_nome", filtros.Dados.Responsavel.NomeRazao);
				}

				comandtxt += comando.FiltroAnd("e.segmento_id", "segmento_id", filtros.Dados.Segmento);

				comandtxt += comando.FiltroAnd("e.municipio_id", "municipio_id", filtros.Dados.MunicipioId);

				if (filtros.Dados.MunicipioId.GetValueOrDefault() <= 0)
				{
					comandtxt += comando.FiltroAnd("e.estado_id", "estado_id", filtros.Dados.EstadoId);
				}

				comandtxt += comando.FiltroAnd("e.atividade_id", "atividade_id", filtros.Dados.Atividade.Id);

				comandtxt += comando.FiltroAnd("e.cnpj", "cnpj_empreemdimento", filtros.Dados.CnpjEmpreemdimento);

				comandtxt += comando.FiltroIn("e.empreendimento_id", String.Format(@"select p.empreendimento_id from {0}lst_protocolo p where p.numero_completo = :protocolo_numero", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "protocolo_numero", filtros.Dados.Protocolo.NumeroTexto);

				//Filtro de abrangencia
				if (!string.IsNullOrEmpty(filtros.Dados.AreaAbrangencia) && filtros.Dados.Coordenada != null)
				{
					//Transforma as coordenadas
					filtros.Dados.Coordenada = CoordenadaBus.TransformarCoordenada(filtros.Dados.Coordenada);
					Double abrangencia = Convert.ToDouble(filtros.Dados.AreaAbrangencia);

					comandtxt += String.Format(@" and e.empreendimento_id in (select e.empreendimento from {0}geo_emp_localizacao e where sdo_relate(e.geometry, 
					{0}coordenada.utm2spatialrect(:x1, :y1, :f1, :x2, :y2, :f2, 0), 'MASK=ANYINTERACT QUERYTYPE=WINDOW')='TRUE')",
					(string.IsNullOrEmpty(EsquemaBancoGeo) ? "" : EsquemaBancoGeo + "."));

					comando.AdicionarParametroEntrada("x1", (filtros.Dados.Coordenada.EastingUtm - abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("y1", (filtros.Dados.Coordenada.NorthingUtm - abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("f1", filtros.Dados.Coordenada.FusoUtm, DbType.Int32);

					comando.AdicionarParametroEntrada("x2", (filtros.Dados.Coordenada.EastingUtm + abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("y2", (filtros.Dados.Coordenada.NorthingUtm + abrangencia), DbType.Double);
					comando.AdicionarParametroEntrada("f2", filtros.Dados.Coordenada.FusoUtm, DbType.Int32);
				}

				if (filtros.Dados.EmpreendimentoCompensacao > 0)
				{
					comandtxt += string.Format(@" and e.empreendimento_id in (select empreendimento from crt_dominialidade where id in (select dominialidade from 
					crt_dominialidade_dominio where id in (select dominio from crt_dominialidade_reserva where cedente_receptor = 1 and emp_compensacao = :emp_compensacao))) ", (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

					comando.AdicionarParametroEntrada("emp_compensacao", filtros.Dados.EmpreendimentoCompensacao, DbType.Int32);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "codigo", "segmento_texto", "denominador", "cnpj" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("denominador");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_empreendimento e where e.id > 0" + comandtxt,
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.empreendimento_id id, e.codigo, e.denominador, e.cnpj, e.segmento_texto from {0}lst_empreendimento e where e.id > 0"
				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Empreendimento empreendimento;

					while (reader.Read())
					{
						empreendimento = new Empreendimento();
						empreendimento.Id = reader.GetValue<Int32>("id");
						empreendimento.Codigo = reader.GetValue<Int64>("codigo");
						empreendimento.Denominador = reader.GetValue<String>("denominador");
						empreendimento.CNPJ = reader.GetValue<String>("cnpj");
						empreendimento.SegmentoTexto = reader.GetValue<String>("segmento_texto");
						retorno.Itens.Add(empreendimento);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		public List<PessoaLst> ObterResponsaveis(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, nvl(p.nome, p.razao_social) nome_razao_social, r.tipo
				from {0}tab_empreendimento_responsavel r, {0}tab_pessoa p where r.responsavel = p.id and r.empreendimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst pessoa = null;

					while (reader.Read())
					{
						pessoa = new PessoaLst();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Texto = reader["nome_razao_social"].ToString();
						pessoa.VinculoTipo = Convert.ToInt32(reader["tipo"]);
						responsaveis.Add(pessoa);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		public List<PessoaLst> ObterResponsaveisComTipo(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, l.texto||' - '||nvl(p.nome, p.razao_social) texto, r.tipo
				from {0}tab_empreendimento_responsavel r, {0}tab_pessoa p, {0}lov_empreendimento_tipo_resp l
				where r.responsavel = p.id and l.id = r.tipo and r.empreendimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst pessoa = null;

					while (reader.Read())
					{
						pessoa = new PessoaLst();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Texto = reader["texto"].ToString();
						pessoa.VinculoTipo = Convert.ToInt32(reader["tipo"]);
						responsaveis.Add(pessoa);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		public List<PessoaLst> ObterListaResponsavelTecnicoEmpreendimento(int empreendimento, int requerimento, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				if (empreendimento > 0 && requerimento <= 0)
				{
					comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
															from tab_requerimento_responsavel r, tab_pessoa p, tab_requerimento req 
															where r.responsavel = p.id and r.requerimento = req.id 
															and req.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				}
				else
				{
					if (empreendimento <= 0 && requerimento > 0)
					{
						comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
															from tab_requerimento_responsavel r, tab_pessoa p, tab_requerimento req 
															where r.responsavel = p.id and r.requerimento = req.id 
															and req.id = :requerimento and req.empreendimento is null", EsquemaBanco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
															from tab_requerimento_responsavel r, tab_pessoa p, tab_requerimento req 
															where r.responsavel = p.id and r.requerimento = req.id 
															and req.id = :requerimento", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst responsavel = null;

					while (reader.Read())
					{
						responsavel = new PessoaLst();
						responsavel.Id = Convert.ToInt32(reader["id"]);
						responsavel.Texto = reader["nome_razao_social"].ToString();
						responsavel.IsAtivo = true;
						responsaveis.Add(responsavel);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		public List<PessoaLst> ObterListaInteressadoEmpreendimento(int empreendimento, int requerimento, BancoDeDados banco = null)
		{
			List<PessoaLst> interessados = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				if (empreendimento > 0 && requerimento <= 0)
				{
					comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
														from tab_pessoa p, tab_requerimento req where p.id = req.interessado 
														and req.empreendimento = :empreendimento", EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				}

				else
				{
					if (empreendimento <= 0 && requerimento > 0)
					{
						comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
															from tab_pessoa p, tab_requerimento req where p.id = req.interessado 
															and req.id = :requerimento and req.empreendimento is null", EsquemaBanco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"select distinct(p.id), nvl(p.nome, p.razao_social) nome_razao_social
															from tab_pessoa p, tab_requerimento req where p.id = req.interessado 
															and req.id = :requerimento", EsquemaBanco);

					}
					comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst interessado = null;

					while (reader.Read())
					{
						interessado = new PessoaLst();
						interessado.Id = Convert.ToInt32(reader["id"]);
						interessado.Texto = reader["nome_razao_social"].ToString();
						interessado.IsAtivo = true;
						interessados.Add(interessado);
					}

					reader.Close();
				}
			}

			return interessados;
		}

		public Int32 ObterEnderecoZona(int id, BancoDeDados banco = null)
		{
			int zona = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Endereços

				Comando comando = bancoDeDados.CriarComando(@"select te.zona from {0}tab_empreendimento_endereco te 
				where te.correspondencia = 0 and te.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						zona = Convert.IsDBNull(reader["zona"]) ? 0 : Convert.ToInt32(reader["zona"]);
					}

					reader.Close();
				}

				#endregion
			}

			return zona;
		}

		public List<Pessoa> ObterEmpreendimentoResponsaveis(int empreendimento)
		{
			List<Pessoa> retorno = new List<Pessoa>();
			List<string> conj = new List<string>();
			Pessoa pessoa;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsáveis do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, nvl(p.nome, p.razao_social) NomeRazaoSocial,
				nvl(p.cpf, p.cnpj) CPFCNPJ, (select c.conjuge||'@'||c.pessoa from {0}tab_pessoa_conjuge c where c.pessoa = p.id or c.conjuge = p.id) conjuge 
				from {0}tab_pessoa p, {0}tab_empreendimento_responsavel pc where p.id = pc.responsavel and pc.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa = new Pessoa();
						pessoa.Id = reader.GetValue<int>("Id");
						pessoa.Tipo = reader.GetValue<int>("Tipo");
						pessoa.NomeRazaoSocial = reader.GetValue<string>("NomeRazaoSocial");
						pessoa.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
						if (!string.IsNullOrEmpty(reader.GetValue<string>("conjuge")))
						{
							conj = reader.GetValue<string>("conjuge").Split('@').ToList();
							pessoa.Fisica.ConjugeId = (Convert.ToInt32(conj[0]) == pessoa.Id) ? Convert.ToInt32(conj[1]) : Convert.ToInt32(conj[0]);
						}
						retorno.Add(pessoa);
					}

					reader.Close();
				}

				#endregion

				//Obter CPF Conjuges
				comando = bancoDeDados.CriarComando(@"select p.id, p.cpf from {0}tab_pessoa p ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("where", "p.id", DbType.Int32, retorno.Where(x => x.Fisica.ConjugeId > 0).Select(x => x.Fisica.ConjugeId).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa = retorno.FirstOrDefault(x => x.Fisica.ConjugeId == reader.GetValue<int>("id"));

						if (pessoa != null)
						{
							pessoa.Fisica.ConjugeCPF = reader.GetValue<string>("cpf");
						}
					}

					reader.Close();
				}
			}

			return retorno;
		}

		public Empreendimento ObterHistorico(int id, string tid, BancoDeDados banco = null)
		{
			Empreendimento empreendimento = new Empreendimento();

			int id_hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.codigo, e.segmento_id segmento, e.segmento_texto, e.cnpj, ls.denominador 
															segmento_denominador, e.denominador, e.nome_fantasia, e.atividade_id atividade, 
															a.atividade atividade_texto from {0}hst_empreendimento e, {0}hst_empreendimento_atividade a, 
															lov_empreendimento_segmento  ls where e.atividade_id = a.atividade_id(+) 
															and ls.id = e.segmento_id and e.empreendimento_id = :id and e.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = id;
						empreendimento.Tid = tid;
						id_hst = reader.GetValue<Int32>("id");
						empreendimento.Codigo = reader.GetValue<Int64>("codigo");

						if (reader["segmento"] != null && !Convert.IsDBNull(reader["segmento"]))
						{
							empreendimento.Segmento = Convert.ToInt32(reader["segmento"]);
							empreendimento.SegmentoTexto = reader["segmento_texto"].ToString();
							empreendimento.SegmentoDenominador = reader["segmento_denominador"].ToString();
						}

						empreendimento.CNPJ = reader["cnpj"].ToString();
						empreendimento.Denominador = reader["denominador"].ToString();
						empreendimento.NomeFantasia = reader["nome_fantasia"].ToString();

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							empreendimento.Atividade.Id = Convert.ToInt32(reader["atividade"]);
							empreendimento.Atividade.Atividade = reader["atividade_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis

				comando = bancoDeDados.CriarComando(@"select pr.tid, pr.emp_resp_id id, pr.empreendimento_id, pr.responsavel_id responsavel, pr.responsavel_tid,
				nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, pr.tipo_id tipo, pr.tipo_texto, pr.data_vencimento, pr.especificar, pr.origem, pr.origem_texto, 
				pr.credenciado_usuario_id from {0}hst_empreendimento_responsavel pr, {0}hst_pessoa p where pr.responsavel_id = p.pessoa_id and p.tid = pr.responsavel_tid 
				and pr.empreendimento_id = :empreendimento and pr.tid = :tid and pr.id_hst = :id_hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Responsavel responsavel;

					while (reader.Read())
					{
						responsavel = new Responsavel();
						responsavel.Id = Convert.ToInt32(reader["responsavel"]);
						responsavel.IdRelacionamento = Convert.ToInt32(reader["id"]);
						responsavel.Tipo = Convert.ToInt32(reader["tipo"]);
						responsavel.TipoTexto = reader["tipo_texto"].ToString();
						responsavel.Tid = reader["responsavel_tid"].ToString();

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							responsavel.DataVencimento = Convert.ToDateTime(reader["data_vencimento"]);
						}

						responsavel.NomeRazao = reader["nome"].ToString();
						responsavel.CpfCnpj = reader["cpf_cnpj"].ToString();
						responsavel.EspecificarTexto = reader["especificar"].ToString();

						responsavel.Origem = reader.GetValue<int>("origem");
						responsavel.OrigemTexto = reader.GetValue<string>("origem_texto");
						responsavel.CredenciadoUsuarioId = reader.GetValue<int>("credenciado_usuario_id");

						empreendimento.Responsaveis.Add(responsavel);
					}

					reader.Close();
				}

				#endregion

				#region Endereços

				comando = bancoDeDados.CriarComando(@"select te.endereco_id id, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro,
													te.estado_id, te.estado_texto, te.municipio_id, te.municipio_texto, te.numero, te.distrito,
													te.corrego, te.caixa_postal, te.complemento, te.tid from {0}hst_empreendimento_endereco te
													where te.id_hst = :id_hst and te.tid = :tid order by te.correspondencia", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Endereco end;

					while (reader.Read())
					{
						end = new Endereco();
						end.Id = Convert.ToInt32(reader["id"]);
						end.Tid = reader["tid"].ToString();
						end.Correspondencia = Convert.IsDBNull(reader["correspondencia"]) ? 0 : Convert.ToInt32(reader["correspondencia"]);
						end.ZonaLocalizacaoId = Convert.IsDBNull(reader["zona"]) ? 0 : Convert.ToInt32(reader["zona"]);
						end.Cep = reader["cep"].ToString();
						end.Logradouro = reader["logradouro"].ToString();
						end.Bairro = reader["bairro"].ToString();
						end.EstadoId = Convert.IsDBNull(reader["estado_id"]) ? 0 : Convert.ToInt32(reader["estado_id"]);
						end.EstadoTexto = reader["estado_texto"].ToString();
						end.MunicipioId = Convert.IsDBNull(reader["municipio_id"]) ? 0 : Convert.ToInt32(reader["municipio_id"]);
						end.MunicipioTexto = reader["municipio_texto"].ToString();
						end.Numero = reader["numero"].ToString();
						end.DistritoLocalizacao = reader["distrito"].ToString();
						end.Corrego = reader["corrego"].ToString();
						end.CaixaPostal = reader["caixa_postal"].ToString();
						end.Complemento = reader["complemento"].ToString();
						empreendimento.Enderecos.Add(end);
					}

					reader.Close();
				}

				#endregion

				#region Coordenada

				comando = bancoDeDados.CriarComando(@"select aec.coordenada_id id, aec.tid, aec.tipo_coordenada_id tipo_coordenada, aec.tipo_coordenada_texto tipotexto,
													aec.datum_id datum, aec.datum_texto datumTexto, aec.easting_utm, aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm_id hemisferio_utm,
													aec.hemisferio_utm_texto hemisferioTexto, aec.latitude_gms, aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec,
													aec.forma_coleta_id forma_coleta, aec.local_coleta_id local_coleta from {0}hst_empreendimento_coord  aec where aec.id_hst = :id_hst
													and aec.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

						if (!Convert.IsDBNull(reader["datumtexto"]))
						{
							empreendimento.Coordenada.Datum.Texto = reader["datumtexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["tipotexto"]))
						{
							empreendimento.Coordenada.Tipo.Texto = reader["tipotexto"].ToString();
						}

						if (!Convert.IsDBNull(reader["hemisferiotexto"]))
						{
							empreendimento.Coordenada.HemisferioUtmTexto = reader["hemisferiotexto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion

				#region Meio de Contato

				comando = bancoDeDados.CriarComando(@"select a.emp_contato_id id, b.mascara, b.id tipo_contato_id, b.texto contato_texto,
													a.valor, a.tid from {0}hst_empreendimento_contato a, {0}tab_meio_contato b
													where a.meio_contato_id = b.id and a.id_hst = :id_hst and a.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", empreendimento.Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Contato contato;

					while (reader.Read())
					{
						contato = new Contato();
						contato.Id = Convert.ToInt32(reader["id"]);
						contato.PessoaId = empreendimento.Id;
						contato.TipoContato = (eTipoContato)Enum.Parse(contato.TipoContato.GetType(), reader["tipo_contato_id"].ToString());
						contato.TipoTexto = reader["contato_texto"].ToString();
						contato.Mascara = reader["mascara"].ToString();
						contato.Valor = reader["valor"].ToString();
						contato.Tid = reader["tid"].ToString();
						empreendimento.MeiosContatos.Add(contato);
					}

					reader.Close();
				}

				#endregion

				empreendimento.TemCorrespondencia = (empreendimento.Enderecos.Any(x => x.Correspondencia == 1) ? 1 : 0);
			}

			return empreendimento;
		}

		public Endereco ObterEndereco(int id, BancoDeDados banco = null)
		{
			Endereco retorno = new Endereco();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Endereços

				Comando comando = bancoDeDados.CriarComando(@"
				select te.id, te.empreendimento, te.correspondencia, te.zona, te.cep, te.logradouro, te.bairro, le.id estado_id, le.texto estado_texto, 
				lm.id municipio_id, lm.texto municipio_texto, te.numero, te.distrito, te.corrego, te.caixa_postal, te.complemento, te.tid 
				from {0}tab_empreendimento_endereco te, {0}lov_estado le, {0}lov_municipio lm 
				where te.estado = le.id(+) and te.municipio = lm.id(+) and te.empreendimento = :empreendimento and te.correspondencia = 0", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = reader.GetValue<int>("id");
						retorno.Tid = reader.GetValue<string>("tid");
						retorno.ZonaLocalizacaoId = reader.GetValue<int>("zona");
						retorno.Cep = reader.GetValue<string>("cep");
						retorno.Logradouro = reader.GetValue<string>("logradouro");
						retorno.Bairro = reader.GetValue<string>("bairro");
						retorno.EstadoId = reader.GetValue<int>("estado_id");
						retorno.EstadoTexto = reader.GetValue<string>("estado_texto");
						retorno.MunicipioId = reader.GetValue<int>("municipio_id");
						retorno.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						retorno.Numero = reader.GetValue<string>("numero");
						retorno.DistritoLocalizacao = reader.GetValue<string>("distrito");
						retorno.Corrego = reader.GetValue<string>("corrego");
						retorno.CaixaPostal = reader.GetValue<string>("caixa_postal");
						retorno.Complemento = reader.GetValue<string>("complemento");
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

		#endregion
	}
}