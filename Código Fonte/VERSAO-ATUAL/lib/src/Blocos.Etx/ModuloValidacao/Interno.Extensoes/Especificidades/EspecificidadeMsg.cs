

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EspecificidadeMsg _especificidadeMsg = new EspecificidadeMsg();
		public static EspecificidadeMsg Especificidade
		{
			get { return _especificidadeMsg; }
			set { _especificidadeMsg = value; }
		}
	}

	public class EspecificidadeMsg
	{
		public Mensagem RequerimentoPradroObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessosDocumentos", Texto = "O requerimento padrão é obrigatório." }; } }
		public Mensagem RepresentanteDesassociadoPJ { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Representante não está mais associado ao interessado." }; } }
		public Mensagem AtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = "A atividade é obrigatória." }; } }
		public Mensagem AtividadesSituacaoAndamento { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Nenhuma das atividades do processo solicitado possui a situação \"Em andamento\"." }; } }
		public Mensagem ProtocoloReqFoiDesassociado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O protocolo que estava associado ao requerimento da especificidade foi desassociado." }; } }

		public Mensagem TituloAnteriorNaoEncerrado(string atividade, string modelo)
		{
			return new Mensagem() { Campo = "", Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O título anterior da atividade \"{0}\", modelo \"{1}\" deve estar encerrado.", atividade, modelo) };
		}

		public Mensagem AtividadeSituacaoInvalida(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade \"{0}\" deve estar na situação \"em andamento\".", atividade) };
		}

		public Mensagem AtividadeNaoConfiguradaNaAtividadeCaracte(string atividade, string modelo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O modelo de título \"{1}\" não pode ser utilizado para atividade \"{0}\".", atividade, modelo) };
		}

		public Mensagem AtividadeDiferenteCaracterizacao(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade selecionada no título deve ser a mesma selecionada na caracterização de {0}.", caracterizacao) };
		}

		public Mensagem CaracterizacaoNaoPreenchida(string caracterizacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A caracterização de \"{0}\" deve estar cadastrada.", caracterizacao) };
		}

		public Mensagem CaracterizacaoDependencias(string caracterizacaoDependencias)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para cadastrar este modelo de título é necessário ter os dados da caracterização \"{0}\" válidos.", caracterizacaoDependencias) };
		}

		public Mensagem AtividadeNaoAssociadaRequerimento(string atividade, string numeroProcDoc, bool isProcesso = true)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("A atividade \"{0}\" não está mais associada ao {2} {1}. Favor salvar as alterações no título.", atividade, numeroProcDoc, isProcesso ? "processo" : "documento") };
		}

		public Mensagem DestinatarioObrigatorio(string campo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = "Destinatário é obrigatório." };
		}

		public Mensagem DestinatarioJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Destinatário já adicionado." }; } }

		public Mensagem InteressadoObrigatorio(string campo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = "Interessado é obrigatório." };
		}

		public Mensagem InteressadoJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Interessado já adicionado." }; } }

		public Mensagem InteressadoIgualResponsavel { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O interessado adicionado já está associado como responsável do empreendimento. Não é possivel transferir o domínio do imóvel para a mesma pessoa." }; } }

		public Mensagem ResponsavelIgualInteressado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O responsável do empreendimento adicionado já está associado como interessado. Não é possivel transferir o domínio do imóvel para a mesma pessoa." }; } }

		public Mensagem ResponsavelObrigatorio(string campo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = "Responsável é obrigatório." };
		}

		public Mensagem DestinatarioNaoAssociadoMais(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = String.Format("O destinatário do título \"{0}\" não está mais associado ao processo.", nome) };
		}

		public Mensagem ResponsavelNaoAssociadoMais(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = String.Format("O responsável do empreendimento \"{0}\" não está mais associado ao empreendimento.", nome) };
		}

		public Mensagem InteressadoNaoAssociadoMais(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = String.Format("O interessado \"{0}\" não está mais associado ao processo.", nome) };
		}

		public Mensagem ResponsavelJaAdicionado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Responsável já adicionado." }; } }

		public Mensagem DestinatarioDesassociado(string campo, string nome = null)
		{
			if (string.IsNullOrWhiteSpace(nome))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = "O destinatário selecionado não está mais associado ao empreendimento ou processo/documento." };
			}
			else
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = campo, Texto = string.Format("O destinatário {0} selecionado não está mais associado ao empreendimento ou processo/documento.", nome) };
			}
		}

		public Mensagem AtividadeComOutroModeloTitulo(string nome)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = String.Format("Este modelo de título não foi solicitado para a atividade \"{0}\".", nome) };
		}

		public Mensagem AtividadeEstaAssociadaAOutroTitulo(string numero, string modelo, string atividade)
		{
			if (!String.IsNullOrEmpty(numero))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = String.Format("A atividade {2} não pode ser selecionada, pois já está associada ao título {0} - {1}.", numero, modelo, atividade) };
			}

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = String.Format("A atividade {1} não pode ser selecionada, pois já está associada a outro título \"{0}\".", modelo, atividade) };
		}

		public Mensagem TituloUnicoPorAtividade(string numero, string modelo, string atividade)
		{
			if (!String.IsNullOrEmpty(numero))
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = String.Format("A atividade {2} não pode ser selecionada, pois já está associada ao título {0} - {1}.", numero, modelo, atividade) };
			}

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "AtividadeSolicitada", Texto = String.Format("A atividade {1} não pode ser selecionada, pois já está associada a outro título \"{0}\".", modelo, atividade) };
		}

		public Mensagem RequerimentoNaoAssociadoProcesso(int numeroReq, string numeroProcDoc)
		{
			if (numeroReq == 0)
			{
				return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessosDocumentos", Texto = String.Format("O processo/documento não tem requerimento.") };
			}

			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ProcessosDocumentos", Texto = String.Format("O requerimento {0} não está mais associado ao processo/documento {1}.", numeroReq, numeroProcDoc) };
		}



		public Mensagem RequerimentoEmpreendimentoObrigatorio(string atividade)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Para a atividade dispensada {0} é obrigatório ter empreendimento associado ao requerimento..", atividade) };
		}
		public Mensagem CaracterizacaoBarragemDisNaoCadastrada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A caracterização Barragem para Dispensa de Licença Ambiental configurada para a atividade solicitada deve estar concluída." }; } }
	}
}