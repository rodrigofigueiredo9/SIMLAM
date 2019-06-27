using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data
{
	public class EspecificidadeDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		#region Obter

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

		public DadosPDF ObterDadosTitulo(int titulo, BancoDeDados banco = null, int empreendimento = 0)
		{
			DadosPDF dados = new DadosPDF();
			string campo = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título
				Comando comando = bancoDeDados.CriarComando(@"select t.*, ta.*,
				(select s.autorizacao_sinaflor from tab_integracao_sinaflor s where rownum = 1
						and s.autorizacao_sinaflor is not null and exists
						(select 1 from tab_titulo_exp_florestal tt
							where tt.titulo = t.id
							and tt.id = s.titulo_exp_florestal)) codigo_sinaflor
				from  (select t.titulo_id id, t.modelo_nome, t.modelo_sigla, t.requerimento, 
				(select tms.hierarquia from tab_titulo_modelo_setores tms, tab_titulo t where t.id = :id and tms.modelo = t.modelo and tms.setor = t.setor) modelo_hierarquia,
				t.setor_id, t.setor_nome, t.autor_nome, t.situacao_id, t.situacao_texto,
				nvl(t.protocolo_id, (select esp.protocolo from esp_laudo_vistoria_florestal esp where esp.titulo = t.titulo_id)) protocolo_id, 
				(select lp.texto from lov_protocolo lp where lp.id = t.protocolo) protocolo_tipo, t.empreendimento_id from {0}lst_titulo t where t.titulo_id = :id) t,
				(select (select n.numero||'/'||n.ano from {0}tab_titulo_numero n where n.titulo = :id) numero,
				ta.local_emissao local_emissao_id, (select m.texto from {0}lov_municipio m where m.id = ta.local_emissao) local_emissao_texto,
				ta.prazo_unidade, ta.prazo, ta.dias_prorrogados, ta.data_criacao, to_char(da.data_emissao, 'DD/MM/YYYY') data_emissao,
				(case when ta.modelo = 74 and da.data_emissao is null then null 
					when ta.modelo = 74 and da.data_emissao is not null and ta.dias_prorrogados is not null then to_char(ta.data_vencimento - ta.dias_prorrogados, 'dd/MM/yyyy')
					else to_char(ta.data_vencimento, 'dd/MM/yyyy') end) data_vencimento, ta.data_assinatura, 
				ta.data_inicio, ta.data_encerramento, to_char(da.data_emissao, 'DD') diaemissao, to_char(da.data_emissao, 'MM') mesemissao, 
				to_char(da.data_emissao, 'YYYY') anoemissao, ta.setor from {0}tab_titulo ta
				left join (select h.titulo_id, h.data_emissao from hst_titulo h where h.titulo_id = :id
					and h.data_emissao is not null and rownum = 1) da
					on da.titulo_id = ta.id
				where ta.id = :id) ta", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["setor"] != null && !Convert.IsDBNull(reader["setor"]))
							dados.Titulo.SetorId = Convert.ToInt32(reader["setor"]);

						if (empreendimento > 0)
							dados.Empreendimento.Id = empreendimento;
						else if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
							dados.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
							dados.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);

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
							dados.Titulo.ModeloHierarquia = reader["modelo_hierarquia"].ToString();

						dados.Titulo.Prazo = reader["prazo"].ToString();
						dados.Titulo.PrazoTipo = reader["prazo_unidade"].ToString();
						dados.Titulo.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						dados.Titulo.Situacao = reader["situacao_texto"].ToString();
						dados.Titulo.Requerimento.Numero = reader.GetValue<int>("requerimento");
						dados.Titulo.NumeroSinaflor = reader["codigo_sinaflor"].ToString();
					}

					reader.Close();
				}

				#endregion

				#region Dados do Título - Empreendimento

				if (dados.Empreendimento.Id.HasValue)
				{
					comando = bancoDeDados.CriarComando(@"select e.*, ed.*, ed_corresp.*, co.*,
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
					from {0}tab_empreendimento_coord e where e.empreendimento = :empreendimento) co", EsquemaBanco);

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
							dados.Empreendimento.CodigoImovel = this.ObterCodigoSicarPorEmpreendimento(dados.Empreendimento.Id.GetValueOrDefault(0), bancoDeDados);
							
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

				#region Dados do Título - Protocolo

				if (dados.Protocolo.Id.HasValue)
				{
					comando = bancoDeDados.CriarComando(@"select p.id protocolo_id, p.numero||'/'||p.ano protocolo_numero, r.numero requerimento_numero, 
					to_char(r.data_criacao, 'dd/MM/yyyy') requerimento_data, p.data_autuacao, p.numero_autuacao, data_autuacao, to_char(p.data_criacao) data_criacao, 
					p.protocolo, (select lt.texto from {0}lov_protocolo_tipo lt where lt.id = p.tipo) protocolo_tipo
					from {0}tab_protocolo p, {0}tab_requerimento r where p.requerimento = r.id(+) and p.id = :protocolo", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", dados.Protocolo.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							dados.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
							dados.Protocolo.Requerimento.Numero = Convert.ToInt32(reader["requerimento_numero"]);
							dados.Protocolo.Requerimento.DataCriacao = reader["requerimento_data"].ToString();
							dados.Protocolo.DataCriacao = reader["data_criacao"].ToString();
							dados.Protocolo.IsProcesso = (reader["protocolo"].ToString() == "1");
							dados.Protocolo.Numero = reader["protocolo_numero"].ToString();
							dados.Protocolo.NumeroAutuacao = reader["numero_autuacao"].ToString();
							dados.Protocolo.DataAutuacao = reader["data_autuacao"].ToString();
							dados.Protocolo.Tipo = reader["protocolo_tipo"].ToString();
							dados.Protocolo.Texto = (dados.Protocolo.IsProcesso) ? ProtocoloPDF.PROCESSO : ProtocoloPDF.DOCUMENTO;

						}

						reader.Close();
					}

					#region Dados do Interessado

					comando = bancoDeDados.CriarComando(@"select tp.id, tp.tipo, nvl(tp.nome, tp.razao_social) interessado_nome, nvl(tp.cpf, tp.cnpj) interessado_cpfcnpj,
					e.cep endcep, e.logradouro endlogradouro, e.bairro endbairro, e.distrito enddistrito, le.texto endestado, le.sigla enduf, lm.texto endmunicipio, e.numero endnumero
					from {0}tab_protocolo p, {0}tab_pessoa tp, {0}tab_pessoa_endereco e, {0}lov_municipio lm, {0}lov_estado le 
					where p.interessado = tp.id and e.municipio = lm.id(+) and e.estado = le.id(+) and tp.id = e.pessoa(+) 
					and p.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", dados.Protocolo.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							dados.Protocolo.Interessado.Id = Convert.ToInt32(reader["id"]);
							dados.Protocolo.Interessado.Tipo = Convert.ToInt32(reader["tipo"]);
							dados.Protocolo.Interessado.NomeRazaoSocial = reader["interessado_nome"].ToString();
							dados.Protocolo.Interessado.CPFCNPJ = reader["interessado_cpfcnpj"].ToString();
							dados.Protocolo.Interessado.EndCEP = reader["endcep"].ToString();
							dados.Protocolo.Interessado.EndLogradouro = reader["endlogradouro"].ToString();
							dados.Protocolo.Interessado.EndNumero = reader["endnumero"].ToString();
							dados.Protocolo.Interessado.EndBairro = reader["endbairro"].ToString();
							dados.Protocolo.Interessado.EndDistrito = reader["enddistrito"].ToString();
							dados.Protocolo.Interessado.EndMunicipio = reader["endmunicipio"].ToString();
							dados.Protocolo.Interessado.EndEstado = reader["endestado"].ToString();
							dados.Protocolo.Interessado.EndUF = reader["enduf"].ToString();
						}

						reader.Close();
					}

					#endregion

					#region Dados do Representante do Interessado

					comando = bancoDeDados.CriarComando(@"select p.nome, p.cpf, pe.logradouro, pe.numero, pe.cep, pe.bairro, lm.texto municipio, le.texto estado, le.sigla uf 
					from {0}tab_pessoa p, {0}tab_titulo_pessoas tp, {0}tab_pessoa_endereco pe, {0}lov_municipio lm, {0}lov_estado le
					where tp.titulo = :titulo and tp.pessoa = p.id and tp.pessoa = pe.pessoa(+) and pe.municipio = lm.id(+) and pe.estado = le.id(+) and tp.tipo = 2", EsquemaBanco);

					comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							dados.Protocolo.InteressadoRepresentante.NomeRazaoSocial = reader["nome"].ToString();
							dados.Protocolo.InteressadoRepresentante.CPFCNPJ = reader["cpf"].ToString();

							dados.Protocolo.InteressadoRepresentante.EndLogradouro = reader["logradouro"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndCEP = reader["cep"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndNumero = reader["numero"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndBairro = reader["bairro"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndMunicipio = reader["municipio"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndEstado = reader["estado"].ToString();
							dados.Protocolo.InteressadoRepresentante.EndUF = reader["uf"].ToString();
						}

						reader.Close();
					}

					#endregion
				}

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

				comando = bancoDeDados.CriarComando(@"select a.atividade||' para '||f.texto||' - '||m.nome atividade from {0}tab_titulo_atividades ta, {0}tab_protocolo_atividades b,
				{0}tab_protocolo_ativ_finalida c, {0}tab_atividade a, {0}lov_titulo_finalidade f, {0}tab_titulo_modelo m where ta.atividade = a.id and ta.atividade = b.atividade
				and b.id = c.protocolo_ativ and c.finalidade = f.id and c.modelo = m.id and b.protocolo = ta.protocolo and ta.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						dados.Titulo.AtividadesFinalidadeModelo.Add(reader["atividade"].ToString());
					}

					reader.Close();
				}

				#endregion

				#region Dados dos Responsáveis

				comando = bancoDeDados.CriarComando(@"select nvl(pei.nome, pei.razao_social) nome, lf.texto funcao from {0}tab_protocolo_responsavel t, 
				{0}lov_protocolo_resp_funcao lf, {0}tab_pessoa pei where t.responsavel = pei.id and t.funcao = lf.id and t.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", dados.Protocolo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						dados.Protocolo.Responsaveis.Add(new ResponsavelPDF()
						{
							NomeRazaoSocial = reader["nome"].ToString(),
							Funcao = reader["funcao"].ToString()
						});
					}

					reader.Close();
				}

				#endregion
			}

			return dados;
		}

		public PessoaPDF ObterDadosPessoa(int pessoa, int? empreendimento = null, BancoDeDados banco = null)
		{
			PessoaPDF dados = new PessoaPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Pessoa

				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome_razao_social, p.data_nascimento data_nascimento, lpe.texto estado_civil,
				cp.nome conjuge_nome, cp.cpf conjuge_cpf, nvl(cp.rg, cp.ie) conjuge_rg_ie, cp.mae conjuge_nome_mae, cp.pai conjuge_nome_pai, p.mae nome_mae, p.pai nome_pai, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo, case when p.tipo = 1 then 'Física' else 'Jurídica' end tipo_texto, lm.texto municipio, le.texto estado, le.sigla estado_sigla, 
				pe.cep, pe.logradouro, pe.bairro, pe.distrito, pe.numero," + (empreendimento != null ? @"nvl((select ltr.texto from {0}tab_empreendimento_responsavel er, 
				{0}lov_empreendimento_tipo_resp ltr where er.tipo = ltr.id and er.empreendimento = :empreendimento and er.responsavel = :pessoa), 'Interessado') vinculo_tipo_texto" :
				"null vinculo_tipo_texto") + @", nvl(p.rg, p.ie) rg_ie, p.nacionalidade, prof.texto profissao from {0}tab_pessoa p, {0}tab_pessoa_endereco pe, (select p.pessoa pessoa, 
				p.conjuge conjuge from {0}tab_pessoa_conjuge p union select p.conjuge pessoa, p.pessoa conjuge from {0}tab_pessoa_conjuge p) pc, {0}tab_pessoa cp, {0}lov_municipio lm, 
				{0}lov_estado le, {0}lov_pessoa_estado_civil lpe, {0}tab_pessoa_profissao pr, {0}tab_profissao prof where p.id = pe.pessoa(+) and pe.municipio = lm.id(+) and 
				p.id = pc.pessoa(+) and pc.conjuge = cp.id(+) and pe.estado = le.id(+) and p.estado_civil = lpe.id(+) and pr.profissao = prof.id(+) 
				and p.id = pr.pessoa(+) and p.id = :pessoa", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa, DbType.Int32);

				if (empreendimento != null)
				{
					comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						dados.NomeRazaoSocial = reader["nome_razao_social"].ToString();
						dados.CPFCNPJ = reader["cpf_cnpj"].ToString();
						dados.RGIE = reader["rg_ie"].ToString();
						dados.Tipo = Convert.ToInt32(reader["tipo"]);
						dados.TipoTexto = reader["tipo_texto"].ToString();
						dados.VinculoTipoTexto = reader["vinculo_tipo_texto"].ToString();
						dados.NomeMae = reader["nome_mae"].ToString();
						dados.NomePai = reader["nome_pai"].ToString();
						dados.EstadoCivil = reader["estado_civil"].ToString();

						dados.ConjugeNome = reader["conjuge_nome"].ToString();
						dados.ConjugeCPF = reader["conjuge_cpf"].ToString();
						dados.ConjugeRGIE = reader["conjuge_rg_ie"].ToString();
						dados.ConjugeNomeMae = reader["conjuge_nome_mae"].ToString();
						dados.ConjugeNomePai = reader["conjuge_nome_pai"].ToString();

						dados.Nacionalidade = reader["nacionalidade"].ToString();
						dados.Profissao = reader["profissao"].ToString();

						if (reader["data_nascimento"] != null && !Convert.IsDBNull(reader["data_nascimento"]))
						{
							dados.DataNasc = Convert.ToDateTime(reader["data_nascimento"]).ToShortDateString();
						}

						dados.EndNumero = reader["numero"].ToString();
						dados.EndMunicipio = reader["municipio"].ToString();
						dados.EndEstado = reader["estado"].ToString();
						dados.EndUF = reader["estado_sigla"].ToString();
						dados.EndCEP = reader["cep"].ToString();
						dados.EndLogradouro = reader["logradouro"].ToString();
						dados.EndBairro = reader["bairro"].ToString();
						dados.EndDistrito = reader["distrito"].ToString();
					}

					reader.Close();
				}

				#endregion

				return dados;
			}
		}

		public ResponsavelPDF ObterDadosResponsavel(int pessoa, int protocolo, BancoDeDados banco = null)
		{
			ResponsavelPDF dados = new ResponsavelPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Pessoa

				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome_razao_social, nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo,
				case when p.tipo = 1 then 'Física' else 'Jurídica' end tipo_texto, pf.texto profissao, oc.orgao_sigla orgao_classe_sigla, pp.registro, lrf.texto funcao, pr.numero_art, 
				lm.texto municipio, pe.cep, pe.logradouro, pe.bairro, pe.distrito from {0}tab_pessoa p, {0}tab_pessoa_endereco pe, {0}tab_pessoa_profissao pp, {0}tab_orgao_classe oc, 
				{0}tab_profissao pf, {0}lov_municipio lm, {0}tab_protocolo_responsavel pr, {0}lov_protocolo_resp_funcao lrf where p.id = pe.pessoa and p.id = pp.pessoa(+) 
				and pp.orgao_classe = oc.id(+) and pp.profissao = pf.id(+) and pe.municipio = lm.id(+) and p.id = pr.responsavel(+) and pr.funcao = lrf.id(+) 
				and p.id = :pessoa and pr.protocolo(+) = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", pessoa, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						dados.NomeRazaoSocial = reader["nome_razao_social"].ToString();
						dados.CPFCNPJ = reader["cpf_cnpj"].ToString();
						dados.Tipo = Convert.ToInt32(reader["tipo"]);
						dados.TipoTexto = reader["tipo_texto"].ToString();

						dados.Profissao = reader["profissao"].ToString();
						dados.OrgaoClasseSigla = reader["orgao_classe_sigla"].ToString();
						dados.NumeroRegistro = reader["registro"].ToString();
						dados.Funcao = reader["funcao"].ToString();
						dados.NumeroART = reader["numero_art"].ToString();

						dados.EndMunicipio = reader["municipio"].ToString();
						dados.EndCEP = reader["cep"].ToString();
						dados.EndLogradouro = reader["logradouro"].ToString();
						dados.EndBairro = reader["bairro"].ToString();
						dados.EndDistrito = reader["distrito"].ToString();
					}

					reader.Close();
				}

				#endregion

				return dados;
			}
		}

		public List<AnaliseItemPDF> ObterAnaliseItem(int protocolo, BancoDeDados banco = null)
		{
			List<AnaliseItemPDF> itens = new List<AnaliseItemPDF>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ri.item_id, ri.nome, i.situacao situacao_id, lis.texto situacao_texto, i.motivo
				from {0}tab_protocolo p, {0}tab_analise_itens i, {0}hst_roteiro_item ri, {0}lov_analise_item_situacao lis
				where p.checagem = i.checagem and i.item_id = ri.item_id and i.item_tid = ri.tid and i.situacao = lis.id and p.id = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnaliseItemPDF item;

					while (reader.Read())
					{
						item = new AnaliseItemPDF();
						item.Id = Convert.ToInt32(reader["item_id"]);
						item.Nome = reader["nome"].ToString();
						item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						item.SituacaoTexto = reader["situacao_texto"].ToString();
						item.Motivo = reader["motivo"].ToString();

						itens.Add(item);
					}

					reader.Close();
				}
			}

			return itens;
		}

		public List<CondicionantePDF> ObterCondicionantes(int titulo, BancoDeDados banco = null)
		{
			List<CondicionantePDF> condicionantes = new List<CondicionantePDF>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.descricao, c.prazo, c.periodicidade, c.periodo, pt.texto periodo_tipo from {0}tab_titulo_condicionantes c, 
					{0}lov_titulo_cond_periodo_tipo pt where c.periodo_tipo = pt.id(+) and c.titulo = :titulo order by c.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					CondicionantePDF item;

					while (reader.Read())
					{
						item = new CondicionantePDF();
						item.Descricao = reader["descricao"].ToString();
						item.Prazo = reader["prazo"].ToString();
						item.Periodo = (!Convert.IsDBNull(reader["periodo"])) ? Convert.ToInt32(reader["periodo"]) : 0;
						item.PeriodoTipo = reader["periodo_tipo"].ToString();

						item.PossuiPeriodicidade = Convert.ToBoolean(reader["periodicidade"]);

						if (reader["prazo"] != null && !Convert.IsDBNull(reader["prazo"]))
						{
							Int32 prazo = Convert.ToInt32(reader["prazo"]);
							if (prazo > 1)
							{
								item.PrazoTipo = "Dias";
							}
							else 
							{
								item.PrazoTipo = "Dia";
							}
						}

						condicionantes.Add(item);
					}

					reader.Close();
				}
			}

			return condicionantes;
		}

		public List<AnexoPDF> ObterAnexos(int titulo, BancoDeDados banco = null)
		{
			List<AnexoPDF> anexos = new List<AnexoPDF>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid 
				from {0}tab_titulo_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.titulo = :titulo order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AnexoPDF item;
					while (reader.Read())
					{
						item = new AnexoPDF();
						item.Descricao = reader["descricao"].ToString();
						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();
						anexos.Add(item);
					}

					reader.Close();
				}
			}

			return anexos;
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

		internal List<TituloEsp> ObterTitulosAtividadeProtocolo(int protocoloId, bool isProcesso, int atividadeId, int tituloId, int modeloId, BancoDeDados banco = null)
		{
			List<TituloEsp> lstTitulos = new List<TituloEsp>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				// 1 - Cancelado por falha na elaboração(Situação Motivo)
				// 4 - Encerrado por substituição(Situação Motivo)
				// 5 - Encerrado(Situação)

				Comando comando = bancoDeDados.CriarComando(@"select tt.situacao, tt.situacao_motivo, tn.ano, tn.numero, m.nome modelo 
				from {0}tab_titulo tt, {0}tab_titulo_numero tn, {0}tab_titulo_modelo m, {0}tab_titulo_atividades ta where ta.titulo = tt.id and tt.id = tn.titulo(+) and tt.modelo = m.id
				and ta.atividade = :atividade and tt.id <> :titulo and m.id = :modelo and ((tt.situacao != 5) or (tt.situacao = 5 and tt.situacao_motivo not in (1,4)))
				and ta.protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloEsp tituloEsp;

					while (reader.Read())
					{
						tituloEsp = new TituloEsp();

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							tituloEsp.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["modelo"] != null && !Convert.IsDBNull(reader["modelo"]))
						{
							tituloEsp.Modelo = reader["modelo"].ToString();
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							tituloEsp.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}

						tituloEsp.SituacaoId = Convert.ToInt32(reader["situacao"]);

						if (reader["situacao_motivo"] != null && !Convert.IsDBNull(reader["situacao_motivo"]))
						{
							tituloEsp.MotivoEncerramentoId = Convert.ToInt32(reader["situacao_motivo"]);
						}

						lstTitulos.Add(tituloEsp);
					}

					reader.Close();
				}
			}

			return lstTitulos;
		}

		internal List<TituloEsp> ObterTitulosAtividadeEmpreendimento(int requerimentoId, int atividadeId, int tituloId, int modeloId, BancoDeDados banco = null)
		{
			int empreedimentoID = 0;
			int empreedimentoCodigo = 0;
			string empreedimentoCNPJ = string.Empty;
			List<TituloEsp> lstTitulos = new List<TituloEsp>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				// 10 - Encerrado(Situação)

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

				comando = bancoDeDados.CriarComando(@"
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
					and (tt.empreendimento = :empreendimento
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
					and (e.codigo = :empreendimento_codigo
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

		internal List<PessoaLst> ObterInteressados(int id)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Interessados do título

				Comando comando = bancoDeDados.CriarComando(@"select pe.id, nvl(pe.nome, pe.razao_social) nome_razao from 
				(select r.responsavel id from {0}tab_protocolo p, {0}tab_empreendimento_responsavel r where p.empreendimento = r.empreendimento and p.id = :id
				union select p.interessado id  from {0}tab_protocolo p where p.id = :id) p, tab_pessoa pe where pe.id = p.id order by nvl(pe.nome, pe.razao_social)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

		public TituloAssociadoEsp ObterTituloAssociado(int tituloId)
		{
			TituloAssociadoEsp titulo = new TituloAssociadoEsp();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, t.modelo, ttm.codigo, t.empreendimento
				from {0}tab_titulo t, {0}tab_titulo_modelo ttm where t.modelo = ttm.id and t.id = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo.Id = Convert.ToInt32(reader["id"]);
						titulo.ModeloId = Convert.ToInt32(reader["modelo"]);
						titulo.ModeloCodigo = Convert.ToInt32(reader["codigo"]);
						titulo.Situacao = Convert.ToInt32(reader["situacao"]);

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							titulo.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						}
					}

					reader.Close();
				}

				return titulo;
			}
		}

		internal TituloAssociadoEsp ExisteTituloAssociado(int tituloId, int tituloAssociadoId)
		{
			TituloAssociadoEsp titulo = new TituloAssociadoEsp();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tm.nome, tn.numero || '/' || tn.ano numero from {0}tab_titulo t, {0}tab_titulo_associados ta, 
				{0}tab_titulo_modelo tm, {0}tab_titulo_numero tn where t.id = ta.titulo and t.modelo = tm.id and t.id = tn.titulo(+) 
				and (t.situacao != 5 or t.situacao_motivo not in (1, 4)) and ta.associado_id = :tituloAssociado and t.id <> :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("tituloAssociado", tituloAssociadoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo.Associado = true;
						titulo.ModeloTexto = reader["nome"].ToString();
						titulo.TituloNumero = reader["numero"].ToString();

						if (titulo.TituloNumero.Length == 1)
						{
							titulo.TituloNumero = string.Empty;
						}
					}

					reader.Close();
				}
				return titulo;
			}
		}

		public List<AnexoPDF> ObterArquivosProjetoGeo(int empreendimento, int caracterizacao, BancoDeDados banco = null, bool finalizado = false)
		{
			List<AnexoPDF> arquivos = new List<AnexoPDF>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				#region Projeto Geográfico/Arquivos

				if (finalizado)
				{
					comando = bancoDeDados.CriarComando(@"select a.arquivo, a.tipo from {0}crt_projeto_geo p, {0}crt_projeto_geo_arquivos a
					where p.id = a.projeto and p.caracterizacao = :caracterizacao and p.empreendimento = :empreendimento", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select a.arquivo, a.tipo from {0}tmp_projeto_geo p, {0}tmp_projeto_geo_arquivos a
					where p.id = a.projeto and p.caracterizacao = :caracterizacao and p.empreendimento = :empreendimento", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						AnexoPDF arq = new AnexoPDF();

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arq.Arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arq.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}

			return arquivos;
		}

		public FuncionarioPDF ObterDadosFuncionario(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select f.id,
						   f.usuario UsuarioId,
						   f.nome Nome,
						   f.cpf Cpf,
						   f.situacao Situacao,
						   f.situacao_motivo SituacaoMotivo,
						   f.email Email,
						   f.tipo Tipo,
						   f.tid Tid,
						   u.login UsuarioLogin
					  from {0}tab_funcionario f, 
						   {0}tab_usuario u
					 where f.id = :id
					   and f.usuario = u.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ObterEntity<FuncionarioPDF>(comando);
			}
		}

		internal List<TituloEsp> ObterTituloPorEmpreendimentoAtividade(int tituloId, int empreendimentoId, int atividadeId, int modeloId, BancoDeDados banco = null)
		{
			List<TituloEsp> lstTitulos = new List<TituloEsp>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				// 1 - Cancelado por falha na elaboração(Situação Motivo)
				// 4 - Encerrado por substituição(Situação Motivo)
				// 5 - Encerrado(Situação)

				Comando comando = bancoDeDados.CriarComando(@"select tt.situacao, tt.situacao_motivo, tn.ano, tn.numero, m.nome modelo 
				from {0}tab_titulo tt, {0}tab_titulo_numero tn, {0}tab_titulo_modelo m, {0}tab_titulo_atividades ta where ta.titulo = tt.id and tt.id = tn.titulo(+) and tt.modelo = m.id
				and tt.empreendimento = :empreendimento and ta.atividade = :atividade and tt.id <> :titulo and m.id = :modelo 
				and tt.situacao != 5 ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloEsp tituloEsp;

					while (reader.Read())
					{
						tituloEsp = new TituloEsp();

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							tituloEsp.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["modelo"] != null && !Convert.IsDBNull(reader["modelo"]))
						{
							tituloEsp.Modelo = reader["modelo"].ToString();
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							tituloEsp.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}

						tituloEsp.SituacaoId = Convert.ToInt32(reader["situacao"]);

						if (reader["situacao_motivo"] != null && !Convert.IsDBNull(reader["situacao_motivo"]))
						{
							tituloEsp.MotivoEncerramentoId = Convert.ToInt32(reader["situacao_motivo"]);
						}

						lstTitulos.Add(tituloEsp);
					}

					reader.Close();
				}
			}

			return lstTitulos;
		}

		public List<PessoaLst> ObterEmpreendimentoResponsaveis(int empreendimento)
		{
			List<PessoaLst> retorno = new List<PessoaLst>();
			List<string> conj = new List<string>();
			PessoaLst pessoa;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsáveis do empreendimento

				Comando comando = bancoDeDados.CriarComando(@"select p.tipo Tipo, p.id Id, nvl(p.nome, p.razao_social) NomeRazaoSocial,
															nvl(p.cpf, p.cnpj) CPFCNPJ from {0}tab_pessoa p, {0}tab_empreendimento_responsavel pc
															where p.id = pc.responsavel and pc.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						pessoa = new PessoaLst();
						pessoa.Id = reader.GetValue<int>("Id");
						pessoa.VinculoTipo = reader.GetValue<int>("Tipo");
						pessoa.Texto = reader.GetValue<string>("NomeRazaoSocial");
						pessoa.CPFCNPJ = reader.GetValue<string>("CPFCNPJ");
						retorno.Add(pessoa);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

		internal string ObterCodigoSicarPorEmpreendimento(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select
                       l.codigo_imovel                      
				  from (select tsicar.codigo_imovel
                              from tab_car_solicitacao tcs, tab_protocolo pt, tab_pessoa pe, tab_empreendimento e, tab_empreendimento_endereco ee,
                                   lov_municipio lme, lov_car_solicitacao_situacao lcss, tab_controle_sicar tsicar,lov_situacao_envio_sicar lses
                             where not exists (select lst.solic_tit_id from lst_car_solic_tit lst where lst.tipo=1 and lst.solic_tit_id=tcs.id)
                               and tcs.protocolo=pt.id
                               and tcs.declarante=pe.id
                               and tcs.empreendimento=e.id
                               and e.id=ee.empreendimento(+)
                               and ee.municipio=lme.id(+)
                               and ee.correspondencia(+)=0
                               and tcs.situacao=lcss.id
                               and tcs.id=tsicar.solicitacao_car(+)
                               and tsicar.solicitacao_car_esquema(+)=1
                               and tsicar.situacao_envio=lses.id(+)
                               and e.id = :empreendimento
                                 union all
							select tcs.codigo_imovel
						  from lst_car_solic_tit        s,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses
						 where s.tipo = 1
						   and nvl(tcs.solicitacao_car_esquema, 1) = 1
						   and s.solic_tit_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						   and s.empreendimento_id = :empreendimento
						union all
						select tcs.codigo_imovel
						  from lst_car_solicitacao_cred c,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses,
							   tab_protocolo            tp
						 where nvl(tcs.solicitacao_car_esquema, 2) = 2
						   and c.solicitacao_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						   AND c.requerimento = TP.Requerimento(+)
						   and c.empreendimento_id = :empreendimento) l
				 where l.codigo_imovel is not null and rownum = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		#endregion

		#region Validação

		public object ValidarAtividade(int atividade, int id, int situacao = 0)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select pa.situacao from {0}tab_protocolo_atividades pa where pa.atividade = :atividade and pa.protocolo = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					return Convert.ToInt32(valor) == situacao;
				}

				return 0;
			}
		}

		public bool ValidarAtividadeSetor(int atividade, int setor)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(t.id) from {0}tab_atividade t where t.id = :atividade and t.setor = :setor", EsquemaBanco);
				comando.AdicionarParametroEntrada("setor", setor, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarRequerimentoAssociado(int requerimento, int protocoloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(t.id) from {0}tab_protocolo t where t.id = :id and t.requerimento = :requerimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("id", protocoloId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarRequerimentoAssociado(int requerimento, int processoDocumentoPaiId, int processoDocumentoFilhoId = 0)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				comando = bancoDeDados.CriarComando(@"select (select count(*) from {0}tab_protocolo p where p.requerimento = :requerimento and p.id = :pai_id) +
				(select count(*) valor from {0}tab_protocolo_associado t, {0}tab_protocolo tp where t.associado = tp.id and tp.requerimento = :requerimento
				and t.protocolo = :pai_id and t.associado = :filho_id) from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				comando.AdicionarParametroEntrada("pai_id", processoDocumentoPaiId, DbType.Int32);
				comando.AdicionarParametroEntrada("filho_id", processoDocumentoFilhoId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarPossuiAtividadeEmAndamento(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select ((select count(pa.id) from {0}tab_protocolo_atividades pa where pa.situacao = 1
				and pa.protocolo in (select :id id from dual union all select pa.associado id from {0}tab_protocolo_associado pa
				where pa.protocolo = :id))) qtd from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarProtocoloAtividadePossuiModelo(int protocoloId, int atividadeId, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(paf.modelo) quant from {0}tab_protocolo_atividades pa, {0}tab_protocolo_ativ_finalida paf, 
				{0}tab_titulo_modelo tm where pa.id = paf.protocolo_ativ and pa.protocolo = :protocolo_id and pa.atividade = :atividade_id and tm.id = :modelo_id 
				and tm.id = paf.modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo_id", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade_id", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo_id", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarRequerimentoAtividadePossuiModelo(int requerimentoId, int atividadeId, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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

		public bool ValidarInteressadoRepresentanteAssociado(ProtocoloEsp protocolo, int representante)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select pr.representante from {0}tab_pessoa_representante pr, {0}tab_protocolo t where t.interessado = pr.pessoa and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", protocolo.Id, DbType.Int32);

				List<int> representantes = new List<int>();
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						representantes.Add(Convert.ToInt32(reader["representante"]));
					}

					reader.Close();
				}

				if (representantes.Count == 0)
				{
					return true;
				}
				else
				{
					return representantes.SingleOrDefault(x => x == representante) > 0;
				}
			}
		}

		public bool ValidarTituloAnteriorEncerrado(int processoDocumentoId, int atividadeId, out List<TituloAnterior> titulosAnterires, BancoDeDados banco = null)
		{
			titulosAnterires = new List<TituloAnterior>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ( select a.atividade from tab_atividade a where a.id = tpa.atividade ) ativ_nome, 
				   ( select m.nome from tab_titulo_modelo m where m.id = t.modelo ) modelo_nome, tt.situacao 
								from tab_protocolo_atividades tpa, tab_protocolo_ativ_finalida t, tab_titulo tt
							   where t.titulo_anterior_id = tt.id
								 and tpa.id = t.protocolo_ativ
								 and tpa.protocolo = :protocolo
								 and t.titulo_anterior_tipo = 1
								 and tpa.atividade = :atividade", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", processoDocumentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader.GetValue<decimal>("situacao") != 5)
						{
							titulosAnterires.Add(new TituloAnterior()
							{
								Atividade = reader.GetValue<string>("ativ_nome"),
								Modelo = reader.GetValue<string>("modelo_nome")
							});
						}
					}

					reader.Close();
				}
			}

			return titulosAnterires.Count == 0;
		}

		public bool ExisteProcDocFilhoQueFoiDesassociado(int tituloId, BancoDeDados banco = null)
		{
			if (tituloId <= 0)
			{
				return false;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select sum(qtd) count from (select count(*) qtd from tab_titulo t, tab_titulo_atividades ta, 
															tab_protocolo_associado pa where ta.titulo = t.id and ta.protocolo = pa.associado and 
															ta.protocolo != t.protocolo and t.id = :titulo union all select count(*) qtd from tab_titulo t, 
															tab_titulo_atividades ta where t.id = :titulo and t.protocolo = ta.protocolo)", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);

				return !(Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0);
			}
		}

		public bool ValidarDestinatarioIsRepresentanteEmpreendimento(int protocolo, int destinatario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_empreendimento_responsavel er 
															where er.empreendimento = (select p.empreendimento 
															from {0}tab_protocolo p where p.id = :protocolo)
															and er.responsavel = :destinatario", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", destinatario, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		public bool RequerimentoPossuiEmpreendimento(int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select nvl(t.empreendimento, 0) from {0}tab_requerimento t where t.id = :requerimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("requerimento", requerimento, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool EmpreendimentoPossuiCaracterizacaoBarragemDis(int requerimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
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