using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertidao.Data
{
	public class CertidaoDispensaLicenciamentoAmbientalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }
		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public CertidaoDispensaLicenciamentoAmbientalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertidaoDispensaLicenciamentoAmbiental certidao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título de Certidao de Anuencia

				eHistoricoAcao acao;
				object id;

				bancoDeDados.IniciarTransacao();
				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_cert_disp_amb e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update esp_cert_disp_amb t set t.vinculo_propriedade = :vinculo_propriedade, t.vinculo_propriedade_outro = :vinculo_propriedade_outro, t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					certidao.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into esp_cert_disp_amb (id, tid, titulo, vinculo_propriedade, vinculo_propriedade_outro)
					values (seq_esp_cert_disp_amb.nextval, :tid, :titulo, :vinculo_propriedade, :vinculo_propriedade_outro) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("titulo", certidao.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("vinculo_propriedade", certidao.VinculoPropriedade, DbType.Int32);
				comando.AdicionarParametroEntrada("vinculo_propriedade_outro", DbType.String, 50, certidao.VinculoPropriedadeOutro);

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					certidao = certidao ?? new CertidaoDispensaLicenciamentoAmbiental();
					certidao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(Convert.ToInt32(certidao.Titulo.Id), eHistoricoArtefatoEspecificidade.certdisplicenamb, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_cert_disp_amb c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.certdisplicenamb, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_cert_disp_amb e where e.titulo = :titulo", EsquemaBanco);

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
					nvl(nvl(tp.razao_social, tp.nome_fantasia), tp.nome) Interessado
				from
					esp_cert_disp_amb     e,
					tab_titulo            tt,
					tab_requerimento      tr,
					tab_pessoa            tp,
					tab_titulo_atividades tta
				where
					e.titulo = tt.id
				and
					tt.requerimento = tr.id(+)
				and
					tta.titulo = tt.id(+)
				and
					tr.interessado = tp.id
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

				Comando comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf, l.texto vinculo_propriedade, vinculo_propriedade_outro 
				from esp_cert_disp_amb e, tab_titulo t, tab_requerimento r, tab_pessoa p, lov_esp_cert_disp_amb l
				where t.id = e.titulo and r.id = t.requerimento and p.id = r.interessado and l.id  = e.vinculo_propriedade and e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						certidao.Interessado.NomeRazaoSocial = reader.GetValue<string>("nome");
						certidao.Interessado.CPFCNPJ = reader.GetValue<string>("cpf");
						certidao.VinculoPropriedade = reader.GetValue<string>("vinculo_propriedade");
						certidao.VinculoPropriedadeOutro = reader.GetValue<string>("vinculo_propriedade_outro");
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