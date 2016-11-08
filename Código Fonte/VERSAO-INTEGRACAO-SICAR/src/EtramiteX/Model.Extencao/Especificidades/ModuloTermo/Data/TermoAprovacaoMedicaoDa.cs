using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class TermoAprovacaoMedicaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public TermoAprovacaoMedicaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TermoAprovacaoMedicao termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_termo_aprov_medicao e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"
						update {0}esp_termo_aprov_medicao t
						   set t.protocolo        = :protocolo,
							   t.destinatario     = :destinatario,
							   t.data_medicao     = :data_medicao,
							   t.funcionario      = :funcionario,
							   t.resp_medicao     = :resp_medicao,
							   t.setor_cadastro   = :setor_cadastro,
							   t.tipo_responsavel = :tipo_responsavel,
							   t.tid              = :tid
						 where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}esp_termo_aprov_medicao
					(id, titulo, protocolo, destinatario, data_medicao, funcionario, resp_medicao, setor_cadastro, tipo_responsavel, tid) values
					({0}seq_esp_termo_aprov_medicao.nextval, :titulo, :protocolo, :destinatario, :data_medicao, :funcionario, :resp_medicao, :setor_cadastro, :tipo_responsavel, :tid) 
					returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				//nao precisava da coluna protocolo para essa especificidade, pois nao é selecionado o requerimento na especificidade do titulo.
				comando.AdicionarParametroEntrada("protocolo", termo.Titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", termo.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("data_medicao", termo.DataMedicao.DataTexto, DbType.DateTime);
				comando.AdicionarParametroEntrada("funcionario", termo.Funcionario, DbType.Int32);
				comando.AdicionarParametroEntrada("resp_medicao", termo.ResponsavelMedicao, DbType.Int32);
				comando.AdicionarParametroEntrada("setor_cadastro", termo.SetorCadastro.HasValue && termo.SetorCadastro > 0 ? termo.SetorCadastro : null, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_responsavel", termo.TipoResponsavel, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo = termo ?? new TermoAprovacaoMedicao();
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Histórico

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.termoaprovacaomedicao, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_termo_aprov_medicao c  set c.tid = :tid  where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.termoaprovacaomedicao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_termo_aprov_medicao e where e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal TermoAprovacaoMedicao Obter(int titulo, BancoDeDados banco = null)
		{
			TermoAprovacaoMedicao especificidade = new TermoAprovacaoMedicao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.titulo, e.protocolo, e.destinatario,  e.data_medicao, e.resp_medicao, e.tipo_responsavel,
				e.tid, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.setor_cadastro, e.funcionario, e.destinatario, 
				(select distinct(nvl(pe.nome, pe.razao_social)) from hst_esp_termo_aprov_medicao he, hst_pessoa pe where he.destinatario_id = pe.pessoa_id and he.destinatario_tid = pe.tid 
				and pe.data_execucao = (select max(h.data_execucao) from hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and he.especificidade_id = e.id
				and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = he.acao_executada and l.acao = 3) 
				and he.titulo_tid = (select ht.tid from hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao)
				from hst_titulo htt where htt.titulo_id = e.titulo and htt.data_execucao > (select max(httt.data_execucao) from hst_titulo httt
				where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao from esp_termo_aprov_medicao e, tab_titulo_numero n,
				tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.Destinatario = reader.GetValue<int>("destinatario");
						especificidade.DestinatarioNomeRazao = reader.GetValue<string>("destinatario_nome_razao");
						especificidade.DataMedicao.DataTexto = reader.GetValue<string>("data_medicao");
						especificidade.ResponsavelMedicao = reader.GetValue<int?>("resp_medicao");
						especificidade.TipoResponsavel = reader.GetValue<int>("tipo_responsavel");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
						especificidade.SetorCadastro = reader.GetValue<int>("setor_cadastro");
						especificidade.Funcionario = reader.GetValue<int?>("funcionario");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Termo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Termo termo = new Termo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				termo.Titulo = dados.Titulo;
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				TermoAprovacaoMedicao termoApvMed = Obter(titulo, bancoDeDados);
				termo.DataMedicaoDia = DateTime.Parse(termoApvMed.DataMedicao.DataTexto).ToString("dd");
				termo.DataMedicaoMesTexto = DateTime.Parse(termoApvMed.DataMedicao.DataTexto).ToString("MMMM");
				termo.DataMedicaoAno = DateTime.Parse(termoApvMed.DataMedicao.DataTexto).ToString("yyyy");
				termo.Destinatario = DaEsp.ObterDadosPessoa(termoApvMed.Destinatario, termo.Empreendimento.Id, bancoDeDados);

				if (termoApvMed.TipoResponsavel == 1)
				{
					termo.Funcionario = DaEsp.ObterDadosFuncionario(termoApvMed.Funcionario.Value, bancoDeDados);
					termo.Responsavel = null;
				}
				else if (termoApvMed.TipoResponsavel == 2)
				{
					termo.Responsavel = DaEsp.ObterDadosResponsavel(termoApvMed.ResponsavelMedicao.Value, termo.Protocolo.Id.Value, bancoDeDados);
					termo.Funcionario = null;
				}

				#endregion
			}

			return termo;
		}

		#endregion Obter

		#region Validações

		internal bool ResponsavelContidoProcesso(int? responsavelId, int protocoloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select count(rownum)
					  from {0}tab_protocolo_responsavel t
					 where t.protocolo = :protocoloId
					   and t.responsavel = :responsavelId", EsquemaBanco);

				comando.AdicionarParametroEntrada("responsavelId", responsavelId, DbType.Int32);
				comando.AdicionarParametroEntrada("protocoloId", protocoloId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion Validações
	}
}