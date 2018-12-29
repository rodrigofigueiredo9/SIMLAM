using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloOutros.Data
{
	public class OutrosInformacaoCorteDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public OutrosInformacaoCorteDa(string strBancoDeDados = null)
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OutrosInformacaoCorte certidao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_out_informacao_corte e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update esp_out_informacao_corte t set
					t.informacao_corte = :informacao_corte,
					t.validade = :validade,
					t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_out_informacao_corte (id, tid, titulo, informacao_corte, validade)
					values (seq_esp_out_informacao_corte.nextval, :tid, :titulo, :informacao_corte, :validade) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("informacao_corte", certidao.InformacaoCorte, DbType.Int32);
				comando.AdicionarParametroEntrada("validade", certidao.Validade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certidao = certidao ?? new OutrosInformacaoCorte();
					certidao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(Convert.ToInt32(certidao.Titulo.Id), eHistoricoArtefatoEspecificidade.outrosinformacaocorte, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_out_informacao_corte c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.outrosinformacaocorte, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_out_informacao_corte e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal CertidaoDispensaLicenciamentoAmbiental Obter(int titulo, BancoDeDados banco = null)
		{
			CertidaoDispensaLicenciamentoAmbiental especificidade = new CertidaoDispensaLicenciamentoAmbiental();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				Comando comando = bancoDeDados.CriarComando(@"
				select
					e.id Id,
					e.tid Tid,
					tta.atividade Atividade,
					e.vinculo_propriedade VinculoPropriedade,
					e.vinculo_propriedade_outro VinculoPropriedadeOutro,
					tt.requerimento RequerimentoId,
					(case when tt.credenciado is null then (select nvl(p.nome, p.razao_social) from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = tt.requerimento) else 
					(select nvl(p.nome, p.razao_social) from cre_requerimento r, cre_pessoa p where p.id = r.interessado and r.id = tt.requerimento) end) Interessado
				from
					esp_out_informacao_corte     e,
					tab_titulo            tt,
					tab_titulo_atividades tta
				where
					e.titulo = tt.id
				and
					tta.titulo = tt.id(+)
				and
					e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade = bancoDeDados.ObterEntity<CertidaoDispensaLicenciamentoAmbiental>(comando);

				#endregion
			}

			return especificidade;
		}

		internal CertidaoDispensaLicenciamentoAmbientalPDF ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			CertidaoDispensaLicenciamentoAmbientalPDF certidao = new CertidaoDispensaLicenciamentoAmbientalPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				certidao.Titulo = dados.Titulo;
				certidao.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select l.texto vinculo_propriedade, vinculo_propriedade_outro 
				from esp_out_informacao_corte e, tab_titulo t, lov_esp_out_informacao_corte l where t.id = e.titulo and l.id  = e.vinculo_propriedade and e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.VinculoPropriedade = reader.GetValue<string>("vinculo_propriedade");
						certidao.VinculoPropriedadeOutro = reader.GetValue<string>("vinculo_propriedade_outro");
					}

					reader.Close();
				}

				#endregion
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf 
				from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = :requerimento", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("requerimento", certidao.Titulo.Requerimento.Numero, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.Interessado.NomeRazaoSocial = reader.GetValue<string>("nome");
						certidao.Interessado.CPFCNPJ = reader.GetValue<string>("cpf");
					}

					reader.Close();
				}

				#endregion

				#region Pesssoas

				List<PessoaPDF> pessoas = new List<PessoaPDF>();

				comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome_razao, 'Interessado' vinculo_tipo 
				from tab_requerimento r, tab_pessoa p where p.id = r.interessado and r.id = :requerimento
				union all 
				select nvl(p.nome, p.razao_social) nome_razao, 'Responsável Técnico' vinculo_tipo 
				from tab_requerimento_responsavel r, tab_pessoa p where p.id = r.responsavel and r.requerimento = :requerimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("requerimento", certidao.Titulo.Requerimento.Numero, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaPDF pessoa = null;

					while (reader.Read())
					{
						pessoa = new PessoaPDF();
						pessoa.VinculoTipoTexto = reader.GetValue<string>("vinculo_tipo");
						pessoa.NomeRazaoSocial = reader.GetValue<string>("nome_razao");
						pessoas.Add(pessoa);
					}

					reader.Close();
				}

				pessoas.ForEach(item =>
				{
					certidao.Titulo.AssinanteSource.Add(new AssinanteDefault { Cargo = item.VinculoTipoTexto, Nome = item.NomeRazaoSocial });
				});

				#endregion Pesssoas
			}

			return certidao;
		}

		#endregion
	}
}