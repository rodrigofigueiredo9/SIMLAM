using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using System.Collections;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class TermoCPFARLCRDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public TermoCPFARLCRDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TermoCPFARLCR especificidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_termo_cpfarlcr e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", especificidade.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"
					update {0}esp_termo_cpfarlcr e set e.tid = :tid, e.protocolo = :protocolo, e.dominio_cedente = :dominio_cedente, e.emp_receptor = :emp_receptor, 
					e.dominio_receptor = :dominio_receptor, e.numero_averbacao = :numero_averbacao, e.data_emissao = :data_emissao, 
					e.dominialidade = :dominialidade, e.dominialidade_tid = (select d.tid from crt_dominialidade d where d.id = :dominialidade) where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					especificidade.ID = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_termo_cpfarlcr (id, tid, titulo, protocolo, dominio_cedente, emp_receptor, dominio_receptor, numero_averbacao, data_emissao, dominialidade, dominialidade_tid) 
					values ({0}seq_esp_termo_cpfarlcr.nextval, :tid, :titulo, :protocolo, :dominio_cedente, :emp_receptor, :dominio_receptor, :numero_averbacao, :data_emissao, 
					:dominialidade, (select d.tid from crt_dominialidade d where d.id = :dominialidade)) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", especificidade.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", especificidade.Titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("dominio_cedente", especificidade.CedenteDominioID, DbType.Int32);
				comando.AdicionarParametroEntrada("emp_receptor", especificidade.ReceptorEmpreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("dominio_receptor", especificidade.ReceptorDominioID, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_averbacao", DbType.String, 20, especificidade.NumeroAverbacao);
				comando.AdicionarParametroEntrada("data_emissao", especificidade.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("dominialidade", especificidade.DominialidadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					especificidade.ID = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Reservas Cedente

				comando = bancoDeDados.CriarComando("delete from {0}esp_tcpfarlcr_ced_rese t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, especificidade.CedenteARLCompensacao.Select(x => x.IDRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in especificidade.CedenteARLCompensacao)
				{
					if (item.IDRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_tcpfarlcr_ced_rese t set t.reserva = :reserva, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IDRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}esp_tcpfarlcr_ced_rese (id, especificidade, reserva, tid) 
						values ({0}seq_esp_tcpfarlcr_ced_rese.nextval, :especificidade, :reserva, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("reserva", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Responsáveis do empreendimento cendente

				comando = bancoDeDados.CriarComando("delete from {0}esp_tcpfarlcr_ced_resp t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, especificidade.CedenteResponsaveisEmpreendimento.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in especificidade.CedenteResponsaveisEmpreendimento)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_tcpfarlcr_ced_resp t set t.responsavel = :responsavel, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}esp_tcpfarlcr_ced_resp (id, especificidade, responsavel, tid) 
						values ({0}seq_esp_tcpfarlcr_ced_resp.nextval, :especificidade, :responsavel, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("responsavel", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Responsáveis do empreendimento receptor

				comando = bancoDeDados.CriarComando("delete from {0}esp_tcpfarlcr_rec_resp t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, especificidade.ReceptorResponsaveisEmpreendimento.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in especificidade.ReceptorResponsaveisEmpreendimento)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_tcpfarlcr_rec_resp t set t.responsavel = :responsavel, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}esp_tcpfarlcr_rec_resp (id, especificidade, responsavel, tid) 
						values ({0}seq_esp_tcpfarlcr_rec_resp.nextval, :especificidade, :responsavel, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", especificidade.ID, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("responsavel", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(especificidade.Titulo.Id, eHistoricoArtefatoEspecificidade.termocpfarlcr, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_termo_cpfarlcr c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.termocpfarlcr, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}esp_tcpfarlcr_rec_resp e where e.especificidade = (select t.id from {0}esp_termo_cpfarlcr t where t.titulo = :titulo);
					delete from {0}esp_tcpfarlcr_ced_resp e where e.especificidade = (select t.id from {0}esp_termo_cpfarlcr t where t.titulo = :titulo);
					delete from {0}esp_tcpfarlcr_ced_rese e where e.especificidade = (select t.id from {0}esp_termo_cpfarlcr t where t.titulo = :titulo);
					delete from {0}esp_termo_cpfarlcr e where e.titulo = :titulo;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal TermoCPFARLCR Obter(int titulo, BancoDeDados banco = null)
		{
			TermoCPFARLCR especificidade = new TermoCPFARLCR();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"
				select e.id, e.tid, e.titulo, e.protocolo, p.protocolo protocolo_tipo, p.requerimento, n.numero, n.ano, e.dominio_cedente, e.dominio_receptor, e.emp_receptor, 
				(select t.denominador from tab_empreendimento t where t.id = e.emp_receptor) denominador, e.numero_averbacao, e.data_emissao, e.dominialidade, e.dominialidade_tid 
				from {0}esp_termo_cpfarlcr e, {0}tab_protocolo p, {0}tab_titulo_numero n where n.titulo(+) = e.titulo and e.protocolo = p.id and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.ID = reader.GetValue<int>("id");
						especificidade.TID = reader.GetValue<string>("tid");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");

						especificidade.CedenteDominioID = reader.GetValue<int>("dominio_cedente");
						especificidade.ReceptorDominioID = reader.GetValue<int>("dominio_receptor");
						especificidade.ReceptorEmpreendimentoID = reader.GetValue<int>("emp_receptor");
						especificidade.ReceptorEmpreendimentoDenominador = reader.GetValue<string>("denominador");
						especificidade.NumeroAverbacao = reader.GetValue<string>("numero_averbacao");
						especificidade.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");

						especificidade.DominialidadeID = reader.GetValue<int>("dominialidade");
						especificidade.DominialidadeTID = reader.GetValue<string>("dominialidade_tid");
					}

					reader.Close();
				}

				#endregion

				#region Reservas Cedente

				comando = bancoDeDados.CriarComando(@"
				select r.dominialidade_reserva_id Id, t.id IDRelacionamento, r.identificacao Identificacao, r.arl_croqui ARLCroqui 
				from esp_tcpfarlcr_ced_rese t, esp_termo_cpfarlcr e, hst_crt_dominialidade_reserva r 
				where t.especificidade = e.id and r.dominialidade_reserva_id = t.reserva 
				and r.id_hst in (select d.id from hst_crt_dominialidade_dominio d where d.dominialidade_id = e.dominialidade 
				and d.dominialidade_tid = e.dominialidade_tid) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.CedenteARLCompensacao = bancoDeDados.ObterEntityList<ReservaLegal>(comando);

				#endregion

				#region Responsáveis do empreendimento cendente


				comando = bancoDeDados.CriarComando(@"
					select p.id Id, t.id IdRelacionamento,(case when lv.id != 9 /*Outro*/ then lv.texto else ter.especificar end) TipoTexto, (case when lv.id != 9 /*Outro*/ then lv.texto
					else ter.especificar end)  || ' - ' || nvl(p.nome, p.razao_social) NomeRazao from {0}esp_tcpfarlcr_ced_resp t, {0}esp_termo_cpfarlcr e, {0}tab_titulo tt, {0}tab_pessoa p,
					{0}tab_empreendimento_responsavel ter, {0}lov_empreendimento_tipo_resp lv where t.especificidade = e.id and tt.id = e.titulo and p.id = t.responsavel and ter.responsavel = t.responsavel
					and ter.empreendimento(+) = tt.empreendimento and lv.id(+) = ter.tipo and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.CedenteResponsaveisEmpreendimento = bancoDeDados.ObterEntityList<Responsavel>(comando);

				#endregion

				#region Responsáveis do empreendimento receptor

				comando = bancoDeDados.CriarComando(@"
					select p.id Id, t.id IdRelacionamento, (case when lv.id != 9 /*Outro*/ then lv.texto else ter.especificar end) TipoTexto,
					(case when lv.id != 9 /*Outro*/ then lv.texto else ter.especificar end) || ' - ' || nvl(p.nome, p.razao_social) NomeRazao
					from {0}esp_tcpfarlcr_rec_resp t, {0}esp_termo_cpfarlcr e, {0}tab_pessoa p, {0}tab_empreendimento_responsavel ter,
					{0}lov_empreendimento_tipo_resp lv where t.especificidade = e.id and p.id = t.responsavel and ter.responsavel = t.responsavel
					and ter.empreendimento(+) = e.emp_receptor and lv.id(+) = ter.tipo and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.ReceptorResponsaveisEmpreendimento = bancoDeDados.ObterEntityList<Responsavel>(comando);

				#endregion
			}

			return especificidade;
		}

		internal Termo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Termo termo = new Termo();
			TermoCPFARLCR termoCPFARLCR = Obter(titulo, banco);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				Dominialidade dominialidadeCedente = new DominialidadeBus().ObterPorEmpreendimento(dados.Empreendimento.Id.GetValueOrDefault());
				Dominialidade dominialidadeReceptora = new DominialidadeBus().ObterPorEmpreendimento(termoCPFARLCR.ReceptorEmpreendimentoID);

				DateTime dataEmissao = Convert.ToDateTime(termoCPFARLCR.DataEmissao.DataTexto);

				termo.Titulo = dados.Titulo;
				termo.Titulo.TituloAnteriorDiaEmissao = dataEmissao.Day.ToString();
				termo.Titulo.TituloAnteriorMesEmissao = dataEmissao.Month.ToString();
				termo.Titulo.TituloAnteriorAnoEmissao = dataEmissao.Year.ToString();
				termo.NumeroAverbacao = termoCPFARLCR.NumeroAverbacao;
				termo.Titulo.SetorEndereco = DaEsp.ObterEndSetor(termo.Titulo.SetorId);
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;
				termo.Dominialidade = new DominialidadePDF();

				#endregion

				#region Dados da Especificidade

				#region Empreendimento Cedente

				termo.Interessados = new List<PessoaPDF>();
				foreach (var interessado in termoCPFARLCR.CedenteResponsaveisEmpreendimento)
				{
					termo.Interessados.Add(new PessoaPDF()
					{
						NomeRazaoSocial = interessado.NomeRazao.Remove(0, interessado.NomeRazao.LastIndexOf('-') + 1).Trim(),
						TipoTexto = interessado.TipoTexto
					});
				}

				#endregion

				#region Empreendimento Receptor

				comando = bancoDeDados.CriarComando(@"
					select e.denominador, e.codigo, tee.logradouro, tee.numero, tee.distrito, lm.texto endMunicipio, d.croqui_area from {0}tab_empreendimento e, {0}tab_empreendimento_endereco tee,
					{0}lov_municipio lm, {0}esp_termo_cpfarlcr c, {0}crt_dominialidade d where tee.empreendimento = e.id and lm.id = tee.municipio and e.id = c.emp_receptor 
					and d.empreendimento = c.emp_receptor and c.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						termo.Empreendimento2.Denominador = reader.GetValue<string>("denominador");
						termo.Empreendimento2.Codigo = reader.GetValue<string>("codigo");
						termo.Empreendimento2.EndLogradouro = reader.GetValue<string>("logradouro");
						termo.Empreendimento2.EndNumero = reader.GetValue<string>("numero");
						termo.Empreendimento2.EndDistrito = reader.GetValue<string>("distrito");
						termo.Empreendimento2.EndMunicipio = reader.GetValue<string>("endMunicipio");
						termo.Empreendimento2.ATPCroquiDecimal = reader.GetValue<decimal>("croqui_area");
					}

					reader.Close();
				}

				termo.Interessados2 = new List<PessoaPDF>();
				foreach (var interessado in termoCPFARLCR.ReceptorResponsaveisEmpreendimento)
				{
					termo.Interessados2.Add(new PessoaPDF()
					{
						NomeRazaoSocial = interessado.NomeRazao.Remove(0, interessado.NomeRazao.LastIndexOf('-') + 1).Trim(),
						TipoTexto = interessado.TipoTexto
					});
				}

				#endregion

				#region Matricula/Posse

				DominioPDF dominioCedentePDF = new DominioPDF(dominialidadeCedente.Dominios.SingleOrDefault(d => d.Id == termoCPFARLCR.CedenteDominioID));
				dominioCedentePDF.TipoCompensacao = "Cedente";
				termo.Dominialidade.Dominios.Add(dominioCedentePDF);

				DominioPDF dominioReceptorPDF = new DominioPDF(dominialidadeReceptora.Dominios.SingleOrDefault(x => x.Id == termoCPFARLCR.ReceptorDominioID));
				dominioReceptorPDF.TipoCompensacao = "Receptor";
				termo.Dominialidade.Dominios.Add(dominioReceptorPDF);

				#endregion

				#region ARL

				termo.RLPreservada = new List<AreaReservaLegalPDF>();
				termo.RLFormacao = new List<AreaReservaLegalPDF>();

				List<ReservaLegal> reservas = dominialidadeCedente.Dominios.SelectMany(x => x.ReservasLegais).Where(x => termoCPFARLCR.CedenteARLCompensacao.Select(y => y.Id).Any(y => y == x.Id)).ToList();

				reservas.ForEach(x =>
				{
					AreaReservaLegalPDF areaARLPdf = new AreaReservaLegalPDF()
					{
						Tipo = x.SituacaoVegetalId.GetValueOrDefault(),
						AreaCroqui = x.ARLCroqui.ToStringTrunc(),
						Identificacao = x.Identificacao,
						CoordenadaE = x.Coordenada.EastingUtm.ToString(),
						CoordenadaN = x.Coordenada.NorthingUtm.ToString()
					};

					if (areaARLPdf.Tipo == (int)eReservaLegalSituacaoVegetal.Preservada)
					{
						termo.RLPreservada.Add(areaARLPdf);
					}
					else if (areaARLPdf.Tipo == (int)eReservaLegalSituacaoVegetal.EmRecuperacao)
					{
						termo.RLFormacao.Add(areaARLPdf);
					}
				});

				termo.RLTotalPreservada = termo.RLPreservada.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();
				termo.RLTotalFormacao = termo.RLFormacao.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();
				termo.Empreendimento2.ARLRecebidaDecimal = termo.Dominialidade.ARLCedente;

				#endregion

				#endregion
			}

			return termo;
		}

		#endregion

		internal List<TituloModeloLst> ObterTitulosCedenteReceptor(int dominioCedente, int dominioReceptor, BancoDeDados banco = null)
		{
			List<TituloModeloLst> retorno = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.id titulo_id, tm.sigla, (case when tn.numero is null then null else tn.numero||'/'||tn.ano end) numero, ls.texto situacao 
				from esp_termo_cpfarlcr e, tab_titulo t, lov_titulo_situacao ls, tab_titulo_modelo tm, tab_titulo_numero tn  
				where e.titulo = t.id and ls.id = t.situacao and tm.id = t.modelo and tn.titulo(+) = t.id 
				and e.dominio_cedente = :dominioCedente and e.dominio_receptor = :dominioReceptor and t.situacao != 5", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominioCedente", dominioCedente, DbType.Int32);
				comando.AdicionarParametroEntrada("dominioReceptor", dominioReceptor, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new TituloModeloLst()
						{
							Texto = reader.GetValue<string>("numero"),
							Sigla = reader.GetValue<string>("sigla"),
							Situacao = reader.GetValue<string>("situacao"),
							Id = reader.GetValue<int>("titulo_id")
						});
					}

					reader.Close();
				}
			}

			return retorno;
		}
	}
}