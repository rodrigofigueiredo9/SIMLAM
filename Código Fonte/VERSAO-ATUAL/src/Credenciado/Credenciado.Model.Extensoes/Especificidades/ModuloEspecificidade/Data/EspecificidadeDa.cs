using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data
{
	public class EspecificidadeDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public EspecificidadeDa()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		#region Obter

		public DadosPDF ObterDadosTitulo(int titulo, BancoDeDados banco = null)
		{
			DadosPDF dados = new DadosPDF();
			string campo = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título
				Comando comando = bancoDeDados.CriarComando(@"select t.*, ta.*
				from  (select t.titulo_id id, t.modelo_nome, t.modelo_sigla, t.requerimento, 
				(select tms.hierarquia from tab_titulo_modelo_setores tms, tab_titulo t where t.id = :id and tms.modelo = t.modelo and tms.setor = t.setor) modelo_hierarquia,
				t.setor_id, t.setor_nome, t.autor_nome, t.situacao_id, t.situacao_texto, nvl(t.protocolo_id, t.protocolo_id) protocolo_id, 
				(select lp.texto from lov_protocolo lp where lp.id = t.protocolo) protocolo_tipo, t.empreendimento_id from {0}lst_titulo t where t.titulo_id = :id) t,
				(select (select n.numero||'/'||n.ano from {0}tab_titulo_numero n where n.titulo = :id) numero,
				ta.local_emissao local_emissao_id, (select m.texto from {0}lov_municipio m where m.id = ta.local_emissao) local_emissao_texto,
				ta.prazo_unidade, ta.prazo, ta.dias_prorrogados, ta.data_criacao, to_char(ta.data_emissao, 'DD/MM/YYYY') data_emissao,
				(case ta.MODELO when 74 then to_char(ta.data_vencimento - ta.dias_prorrogados, 'dd/MM/yyyy') 
				else to_char(ta.data_vencimento, 'dd/MM/yyyy') end) data_vencimento, ta.data_assinatura, 
				ta.data_inicio, ta.data_encerramento, to_char(ta.data_emissao, 'DD') diaemissao, to_char(ta.data_emissao, 'MM') mesemissao, 
				to_char(ta.data_emissao, 'YYYY') anoemissao, ta.setor from {0}tab_titulo ta where ta.id = :id) ta", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
						{
							dados.Titulo.SetorId = Convert.ToInt32(reader["setor"]);
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							dados.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);
						}

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							dados.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						}

						dados.Protocolo.Tipo = reader["protocolo_tipo"].ToString();

						dados.Titulo.Numero = reader["numero"].ToString();
						dados.Titulo.LocalEmissao = reader["local_emissao_texto"].ToString();
						dados.Titulo.DiaEmissao = reader["diaemissao"].ToString();
						dados.Titulo.MesEmissao = reader["mesemissao"].ToString();
						dados.Titulo.AnoEmissao = reader["anoemissao"].ToString();
						dados.Titulo.DataEmissao = reader["data_emissao"].ToString();
						dados.Titulo.DataVencimento = reader["data_vencimento"].ToString();
						dados.Titulo.ModeloNome = reader["modelo_nome"].ToString();
						dados.Titulo.ModeloSigla = reader["modelo_sigla"].ToString();
						dados.Titulo.AutorNome = reader["autor_nome"].ToString();

						if (reader["modelo_hierarquia"] != null && !Convert.IsDBNull(reader["modelo_hierarquia"]))
						{
							dados.Titulo.ModeloHierarquia = reader["modelo_hierarquia"].ToString();
						}

						dados.Titulo.Prazo = reader["prazo"].ToString();
						dados.Titulo.PrazoTipo = reader["prazo_unidade"].ToString();
						dados.Titulo.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						dados.Titulo.Situacao = reader["situacao_texto"].ToString();
						dados.Titulo.Requerimento.Numero = reader.GetValue<int>("requerimento");
					}

					reader.Close();
				}

				#endregion

				#region Dados do Título - Protocolo

//				if (dados.Protocolo.Id.HasValue)
//				{
//					comando = bancoDeDados.CriarComando(@"select p.id protocolo_id, p.numero||'/'||p.ano protocolo_numero, r.numero requerimento_numero, 
//					to_char(r.data_criacao, 'dd/MM/yyyy') requerimento_data, p.data_autuacao, p.numero_autuacao, data_autuacao, to_char(p.data_criacao) data_criacao, 
//					p.protocolo, (select lt.texto from {0}lov_protocolo_tipo lt where lt.id = p.tipo) protocolo_tipo
//					from {0}tab_protocolo p, {0}tab_requerimento r where p.requerimento = r.id(+) and p.id = :protocolo", EsquemaBanco);

//					comando.AdicionarParametroEntrada("protocolo", dados.Protocolo.Id, DbType.Int32);

//					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//					{
//						if (reader.Read())
//						{
//							dados.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
//							dados.Protocolo.Requerimento.Numero = Convert.ToInt32(reader["requerimento_numero"]);
//							dados.Protocolo.Requerimento.DataCriacao = reader["requerimento_data"].ToString();
//							dados.Protocolo.DataCriacao = reader["data_criacao"].ToString();
//							dados.Protocolo.IsProcesso = (reader["protocolo"].ToString() == "1");
//							dados.Protocolo.Numero = reader["protocolo_numero"].ToString();
//							dados.Protocolo.NumeroAutuacao = reader["numero_autuacao"].ToString();
//							dados.Protocolo.DataAutuacao = reader["data_autuacao"].ToString();
//							dados.Protocolo.Tipo = reader["protocolo_tipo"].ToString();
//							dados.Protocolo.Texto = (dados.Protocolo.IsProcesso) ? ProtocoloPDF.PROCESSO : ProtocoloPDF.DOCUMENTO;

//						}

//						reader.Close();
//					}

//					#region Dados do Interessado

//					comando = bancoDeDados.CriarComando(@"select tp.id, tp.tipo, nvl(tp.nome, tp.razao_social) interessado_nome, nvl(tp.cpf, tp.cnpj) interessado_cpfcnpj,
//					e.cep endcep, e.logradouro endlogradouro, e.bairro endbairro, e.distrito enddistrito, le.texto endestado, le.sigla enduf, lm.texto endmunicipio, e.numero endnumero
//					from {0}tab_protocolo p, {0}tab_pessoa tp, {0}tab_pessoa_endereco e, {0}lov_municipio lm, {0}lov_estado le 
//					where p.interessado = tp.id and e.municipio = lm.id(+) and e.estado = le.id(+) and tp.id = e.pessoa(+) 
//					and p.id = :id", EsquemaBanco);

//					comando.AdicionarParametroEntrada("id", dados.Protocolo.Id, DbType.Int32);

//					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//					{
//						if (reader.Read())
//						{
//							dados.Protocolo.Interessado.Id = Convert.ToInt32(reader["id"]);
//							dados.Protocolo.Interessado.Tipo = Convert.ToInt32(reader["tipo"]);
//							dados.Protocolo.Interessado.NomeRazaoSocial = reader["interessado_nome"].ToString();
//							dados.Protocolo.Interessado.CPFCNPJ = reader["interessado_cpfcnpj"].ToString();
//							dados.Protocolo.Interessado.EndCEP = reader["endcep"].ToString();
//							dados.Protocolo.Interessado.EndLogradouro = reader["endlogradouro"].ToString();
//							dados.Protocolo.Interessado.EndNumero = reader["endnumero"].ToString();
//							dados.Protocolo.Interessado.EndBairro = reader["endbairro"].ToString();
//							dados.Protocolo.Interessado.EndDistrito = reader["enddistrito"].ToString();
//							dados.Protocolo.Interessado.EndMunicipio = reader["endmunicipio"].ToString();
//							dados.Protocolo.Interessado.EndEstado = reader["endestado"].ToString();
//							dados.Protocolo.Interessado.EndUF = reader["enduf"].ToString();
//						}

//						reader.Close();
//					}

//					#endregion

//					#region Dados do Representante do Interessado

//					comando = bancoDeDados.CriarComando(@"select p.nome, p.cpf, pe.logradouro, pe.numero, pe.cep, pe.bairro, lm.texto municipio, le.texto estado, le.sigla uf 
//					from {0}tab_pessoa p, {0}tab_titulo_pessoas tp, {0}tab_pessoa_endereco pe, {0}lov_municipio lm, {0}lov_estado le
//					where tp.titulo = :titulo and tp.pessoa = p.id and tp.pessoa = pe.pessoa(+) and pe.municipio = lm.id(+) and pe.estado = le.id(+) and tp.tipo = 2", EsquemaBanco);

//					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

//					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//					{
//						if (reader.Read())
//						{
//							dados.Protocolo.InteressadoRepresentante.NomeRazaoSocial = reader["nome"].ToString();
//							dados.Protocolo.InteressadoRepresentante.CPFCNPJ = reader["cpf"].ToString();

//							dados.Protocolo.InteressadoRepresentante.EndLogradouro = reader["logradouro"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndCEP = reader["cep"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndNumero = reader["numero"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndBairro = reader["bairro"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndMunicipio = reader["municipio"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndEstado = reader["estado"].ToString();
//							dados.Protocolo.InteressadoRepresentante.EndUF = reader["uf"].ToString();
//						}

//						reader.Close();
//					}

//					#endregion
//				}

				#endregion

				#region Dados do Título - Títulos Associados

				comando = bancoDeDados.CriarComando(@"select ttn.numero || '/' || ttn.ano numero, ttm.nome from {0}tab_titulo_associados t, {0}tab_titulo_numero ttn,
				{0}tab_titulo_modelo ttm where t.associado_id = ttn.titulo and ttn.modelo = ttm.id and t.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						TituloPDF tit = new TituloPDF();

						tit.Numero = reader["numero"].ToString();
						tit.ModeloNome = reader["nome"].ToString();
						dados.Titulo.Associados.Add(tit);
					}

					reader.Close();
				}

				#endregion

				#region Dados do Título - Atividade

				comando = bancoDeDados.CriarComando(@"select a.atividade, a.cod_categoria from {0}tab_titulo_atividades ta, {0}tab_atividade a where ta.atividade = a.id and ta.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						dados.Titulo.Atividades.Add(reader["atividade"].ToString());
						dados.Titulo.AtividadeCodigoCategoria = reader["cod_categoria"].ToString();
					}

					reader.Close();
				}

				#endregion

				#region Dados do Título - Atividade - Finalidade - Modelo

//				comando = bancoDeDados.CriarComando(@"select a.atividade||' para '||f.texto||' - '||m.nome atividade from {0}tab_titulo_atividades ta, {0}tab_protocolo_atividades b,
//				{0}tab_protocolo_ativ_finalida c, {0}tab_atividade a, {0}lov_titulo_finalidade f, {0}tab_titulo_modelo m where ta.atividade = a.id and ta.atividade = b.atividade
//				and b.id = c.protocolo_ativ and c.finalidade = f.id and c.modelo = m.id and b.protocolo = ta.protocolo and ta.titulo = :id", EsquemaBanco);

//				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

//				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//				{
//					while (reader.Read())
//					{
//						dados.Titulo.AtividadesFinalidadeModelo.Add(reader["atividade"].ToString());
//					}

//					reader.Close();
//				}

				#endregion

				#region Dados dos Responsáveis

//				comando = bancoDeDados.CriarComando(@"select nvl(pei.nome, pei.razao_social) nome, lf.texto funcao from {0}tab_protocolo_responsavel t, 
//				{0}lov_protocolo_resp_funcao lf, {0}tab_pessoa pei where t.responsavel = pei.id and t.funcao = lf.id and t.protocolo = :id", EsquemaBanco);

//				comando.AdicionarParametroEntrada("id", dados.Protocolo.Id, DbType.Int32);

//				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
//				{
//					while (reader.Read())
//					{
//						dados.Protocolo.Responsaveis.Add(new ResponsavelPDF()
//						{
//							NomeRazaoSocial = reader["nome"].ToString(),
//							Funcao = reader["funcao"].ToString()
//						});
//					}

//					reader.Close();
//				}

				#endregion
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				#region Dados do Título - Empreendimento

				if (dados.Empreendimento.Id.HasValue)
				{
					Comando comando = bancoDeDados.CriarComando(@"select e.*, ed.*, ed_corresp.*, co.*,
					(select ec.valor from tab_empreendimento_contato ec where ec.empreendimento = :empreendimento and ec.meio_contato = 1)tel,
					(select ec.valor from tab_empreendimento_contato ec where ec.empreendimento = :empreendimento and ec.meio_contato = 3)telfax,
					(select ec.valor from tab_empreendimento_contato ec where ec.empreendimento = :empreendimento and ec.meio_contato = 5)email
					from (select ls.denominador denominador_label, ee.denominador nome, ee.codigo, 
					ee.nome_fantasia, ee.cnpj, ls.texto segmento from  {0}tab_empreendimento ee, {0}lov_empreendimento_segmento ls where 
					ee.id = :empreendimento and ls.id = ee.segmento) e,
					(select e.cep, e.logradouro, e.bairro, (select lm.texto from {0}lov_municipio lm where e.municipio = lm.id) municipio,
					(select lm.texto from {0}lov_estado lm where e.estado = lm.id ) estado,(select lm.sigla from {0}lov_estado lm where e.estado = lm.id ) estado_sigla,
					e.numero, e.distrito, e.corrego, e.complemento, e.zona, e.caixa_postal 
					from {0}tab_empreendimento_endereco e where e.correspondencia = 0 and e.empreendimento = :empreendimento) ed, 

					 (select e.cep corresp_cep, e.logradouro corresp_logradouro, e.bairro corresp_bairro, (select lm.texto from {0}lov_municipio lm
					where e.municipio = lm.id) corresp_municipio, (select lm.texto from {0}lov_estado lm where e.estado = lm.id) corresp_estado,
					(select lm.sigla from {0}lov_estado lm where e.estado = lm.id) corresp_estado_sigla, e.numero corresp_numero, e.distrito corresp_distrito,
					e.corrego corresp_corrego, e.complemento corresp_complemento, e.zona corresp_zona, e.caixa_postal corresp_caixa_postal from {0}tab_empreendimento_endereco e
					where e.correspondencia = 1 and e.empreendimento = :empreendimento) ed_corresp,

					(select (select la.texto from {0}lov_empreendimento_local_colet la where la.id = e.local_coleta) local_coleta,
					(select lb.texto from {0}lov_empreendimento_forma_colet lb where lb.id = e.forma_coleta) forma_coleta,
					(select ld.sigla from {0}lov_coordenada_datum ld where ld.id = e.datum) datum,
					(select lt.texto from {0}lov_coordenada_tipo lt where lt.id = e.tipo_coordenada) tipo_coordenada,
					nvl(e.easting_utm,nvl(e.longitude_gms,e.longitude_gdec)) easting_longitude,
					nvl(e.northing_utm,nvl(e.latitude_gms,e.latitude_gdec)) northing_latitude, e.fuso_utm
					from {0}tab_empreendimento_coord e where e.empreendimento = :empreendimento) co", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("empreendimento", dados.Empreendimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							dados.Empreendimento.Nome = reader["nome"].ToString();
							dados.Empreendimento.Codigo = reader["codigo"].ToString();
							dados.Empreendimento.Denominador = reader["denominador_label"].ToString();
							dados.Empreendimento.CNPJ = reader["cnpj"].ToString();
							dados.Empreendimento.Segmento = reader["segmento"].ToString();

							dados.Empreendimento.EndLogradouro = reader["logradouro"].ToString();
							dados.Empreendimento.EndCEP = reader["cep"].ToString();
							dados.Empreendimento.EndNumero = reader["numero"].ToString();
							dados.Empreendimento.EndBairro = reader["bairro"].ToString();
							dados.Empreendimento.EndComplemento = reader["complemento"].ToString();
							dados.Empreendimento.EndDistrito = reader["distrito"].ToString();
							dados.Empreendimento.EndCorrego = reader["corrego"].ToString();
							dados.Empreendimento.EndZonaId = reader.GetValue<int>("zona");
							dados.Empreendimento.EndZona = ((reader["zona"].ToString() == "1") ? "Urbana" : "Rural");
							dados.Empreendimento.EndCaixaPostal = reader["caixa_postal"].ToString();
							dados.Empreendimento.EndEstado = reader["estado"].ToString();
							dados.Empreendimento.EndUF = reader["estado_sigla"].ToString();
							dados.Empreendimento.EndMunicipio = reader["municipio"].ToString();

							dados.Empreendimento.EndCorrespLogradouro = reader["corresp_logradouro"].ToString();
							dados.Empreendimento.EndCorrespCEP = reader["corresp_cep"].ToString();
							dados.Empreendimento.EndCorrespNumero = reader["corresp_numero"].ToString();
							dados.Empreendimento.EndCorrespBairro = reader["corresp_bairro"].ToString();
							dados.Empreendimento.EndCorrespComplemento = reader["corresp_complemento"].ToString();
							dados.Empreendimento.EndCorrespDistrito = reader["corresp_distrito"].ToString();
							dados.Empreendimento.EndCorrespCorrego = reader["corresp_corrego"].ToString();
							dados.Empreendimento.EndCorrespZonaId = reader.GetValue<int>("corresp_zona");
							dados.Empreendimento.EndCorrespCaixaPostal = reader["corresp_caixa_postal"].ToString();
							dados.Empreendimento.EndCorrespEstado = reader["corresp_estado"].ToString();
							dados.Empreendimento.EndCorrespUF = reader["corresp_estado_sigla"].ToString();
							dados.Empreendimento.EndCorrespMunicipio = reader["corresp_municipio"].ToString();

							dados.Empreendimento.EndLocalColeta = reader["local_coleta"].ToString();
							dados.Empreendimento.EndFormaColeta = reader["forma_coleta"].ToString();
							dados.Empreendimento.EndDatum = reader["datum"].ToString();
							dados.Empreendimento.EndSisCoordenada = reader["tipo_coordenada"].ToString();
							dados.Empreendimento.EndEastingLongitude = reader["easting_longitude"].ToString();
							dados.Empreendimento.EndNorthingLatitude = reader["northing_latitude"].ToString();
							dados.Empreendimento.EndFuso = reader["fuso_utm"].ToString();
							dados.Empreendimento.Telefone = reader.GetValue<string>("tel");
							dados.Empreendimento.TelFax = reader.GetValue<string>("telfax");
							dados.Empreendimento.Email = reader.GetValue<string>("email");
						}

						reader.Close();
					}
				}

				#endregion
			}

			return dados;
		}

		public string ObterAtividadeNome(int atividade)
		{
			string nome = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.atividade from {0}tab_atividade t where t.id = :atividade", EsquemaBanco);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				nome = bancoDeDados.ExecutarScalar(comando).ToString();
			}

			return nome;
		}

		public List<ProcessoAtividadeEsp> ObterAtividadesNome(List<ProcessoAtividadeEsp> atividade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				if (atividade == null)
				{
					return atividade;
				}

				Comando comando = bancoDeDados.CriarComando(@"select t.atividade, t.situacao from {0}tab_atividade t where t.id = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", DbType.Int32);

				foreach (ProcessoAtividadeEsp item in atividade)
				{
					comando.SetarValorParametro("atividade", item.Id);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							item.NomeAtividade = reader.GetValue<string>("atividade");
							item.Ativada = Convert.ToBoolean(reader.GetValue<int>("situacao"));
						}

						reader.Close();
					}
				}
			}

			return atividade;
		}

		internal List<TituloEsp> ObterTitulosAtividadeEmpreendimento(int requerimentoId, int atividadeId, int tituloId, int modeloId, BancoDeDados banco = null)
		{
			int empreedimentoID = 0;
			int empreedimentoCodigo = 0;
			string empreedimentoCNPJ = string.Empty;
			List<TituloEsp> lstTitulos = new List<TituloEsp>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ee.id, ee.codigo, ee.cnpj from tab_requerimento r, tab_empreendimento ee where ee.id = r.empreendimento and r.id = :requerimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreedimentoID = reader.GetValue<int>("id");
						empreedimentoCodigo = reader.GetValue<int>("codigo");
						empreedimentoCNPJ = reader.GetValue<string>("cnpj");
					}

					reader.Close();
				}
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				// 10 - Encerrado(Situação)

				Comando comando = bancoDeDados.CriarComando(@"
				select tt.situacao, tt.situacao_motivo, tn.ano, tn.numero, m.nome modelo
					from {0}tab_titulo           tt,
						{0}tab_titulo_numero     tn,
						{0}tab_titulo_modelo     m,
						{0}tab_titulo_atividades ta,
						{0}tab_empreendimento    e
					where ta.titulo = tt.id
					and tt.id = tn.titulo(+)
					and tt.modelo = m.id
					and e.id(+) = tt.empreendimento
					and ta.atividade = :atividade
					and tt.id <> :titulo
					and m.id = :modelo
					and tt.situacao != 10
					and tt.credenciado is null
					and (e.codigo = :empreendimento_codigo
					or e.cnpj = :empreendimento_cnpj)
				union all
				select tt.situacao, tt.situacao_motivo, tn.ano, tn.numero, m.nome modelo
					from {0}tab_titulo           tt,
						{0}tab_titulo_numero     tn,
						{0}tab_titulo_modelo     m,
						{0}tab_titulo_atividades ta,
						{0}cre_empreendimento    e
					where ta.titulo = tt.id
					and tt.id = tn.titulo(+)
					and tt.modelo = m.id
					and e.id(+) = tt.empreendimento
					and ta.atividade = :atividade
					and tt.id <> :titulo
					and m.id = :modelo
					and tt.situacao != 10
					and tt.credenciado is not null
					and (tt.empreendimento = :empreendimento
					or e.cnpj = :empreendimento_cnpj)", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreedimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento_codigo", empreedimentoCodigo, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento_cnpj", empreedimentoCNPJ, DbType.String);
				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloEsp tituloEsp;

					while (reader.Read())
					{
						tituloEsp = new TituloEsp();

						tituloEsp.Numero.Inteiro = reader.GetValue<int?>("numero");
						tituloEsp.Modelo = reader.GetValue<string>("modelo");
						tituloEsp.Numero.Ano = reader.GetValue<int?>("ano");
						tituloEsp.SituacaoId = reader.GetValue<int>("situacao");
						tituloEsp.MotivoEncerramentoId = reader.GetValue<int?>("situacao_motivo");

						lstTitulos.Add(tituloEsp);
					}

					reader.Close();
				}
			}

			return lstTitulos;
		}

		public SetorEnderecoPDF ObterEndSetor(int setorId, BancoDeDados banco = null)
		{
			SetorEnderecoPDF end = new SetorEnderecoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.setor, e.cep, e.logradouro, e.bairro, e.estado estadoid, 
					le.sigla estadotexto, e.municipio municipioid, lm.texto municipiotexto, e.numero, e.distrito, e.complemento, e.fone, 
					e.fone_fax, e.tid from {0}tab_setor_endereco e, {0}lov_estado le, {0}lov_municipio lm  where e.setor = :setor 
					and e.estado = le.id(+) and e.municipio = lm.id(+)", EsquemaBanco);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				IDataReader reader = bancoDeDados.ExecutarReader(comando);

				if (reader.Read())
				{
					end = new SetorEnderecoPDF();

					if (reader["cep"] != null && !Convert.IsDBNull(reader["cep"]))
					{
						end.CEP = reader["cep"].ToString();
					}

					if (reader["logradouro"] != null && !Convert.IsDBNull(reader["logradouro"]))
					{
						end.Logradouro = reader["logradouro"].ToString();
					}

					if (reader["bairro"] != null && !Convert.IsDBNull(reader["bairro"]))
					{
						end.Bairro = reader["bairro"].ToString();
					}

					if (reader["estadoid"] != null && !Convert.IsDBNull(reader["estadoid"]))
					{
						end.EstadoId = Convert.ToInt32(reader["estadoid"]);
					}

					if (reader["estadotexto"] != null && !Convert.IsDBNull(reader["estadotexto"]))
					{
						end.EstadoTexto = reader["estadotexto"].ToString();
					}

					if (reader["municipioid"] != null && !Convert.IsDBNull(reader["municipioid"]))
					{
						end.MunicipioId = Convert.ToInt32(reader["municipioid"]);
					}

					if (reader["municipiotexto"] != null && !Convert.IsDBNull(reader["municipiotexto"]))
					{
						end.MunicipioTexto = reader["municipiotexto"].ToString();
					}

					if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
					{
						end.Numero = reader["numero"].ToString();
					}

					if (reader["distrito"] != null && !Convert.IsDBNull(reader["distrito"]))
					{
						end.Distrito = reader["distrito"].ToString();
					}

					if (reader["complemento"] != null && !Convert.IsDBNull(reader["complemento"]))
					{
						end.Complemento = reader["complemento"].ToString();
					}

					if (reader["fone"] != null && !Convert.IsDBNull(reader["fone"]))
					{
						end.Fone = reader["fone"].ToString();
					}

					if (reader["fone_fax"] != null && !Convert.IsDBNull(reader["fone_fax"]))
					{
						end.FoneFax = reader["fone_fax"].ToString();
					}

					if (reader["tid"] != null && !Convert.IsDBNull(reader["tid"]))
					{
						end.Tid = reader["tid"].ToString();
					}
				}

				reader.Close();

			}

			return end;
		}

		#endregion

		#region Validação

		public bool ValidarRequerimentoAtividadePossuiModelo(int requerimentoId, int atividadeId, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(paf.modelo) quant from {0}tab_requerimento_atividade pa, {0}tab_requerimento_ativ_finalida paf, 
				{0}tab_titulo_modelo tm where pa.id = paf.requerimento_ativ and pa.requerimento = :requerimento and pa.atividade = :atividade_id and tm.id = :modelo_id 
				and tm.id = paf.modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade_id", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo_id", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool RequerimentoPossuiEmpreendimento(int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select nvl(t.empreendimento, 0) from {0}tab_requerimento t where t.id = :requerimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool EmpreendimentoPossuiCaracterizacaoBarragemDis(int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(*) from crt_barragem_dispensa_lic t where t.empreendimento = (select r.empreendimento from tab_requerimento r where r.id  = :requerimento)", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}