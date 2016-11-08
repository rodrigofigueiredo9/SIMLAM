using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LoteMsg _loteMsg = new LoteMsg();
		public static LoteMsg Lote { get { return _loteMsg; } }
	}

	public class LoteMsg
	{
		public Mensagem Salvar(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = String.Format("Lote Nº {0} salvo com sucesso.", numero) };
		}

		public Mensagem ExcluirMensagem { get { return new Mensagem() { Texto = "Excluir o Lote ?" }; } }

		public Mensagem LoteNaoPodeSerExcluir(string cfo)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O lote não pode ser excluído, pois está sendo utilizado no CFOC {0}.", cfo) };
		}

		public Mensagem ExcluidoSucesso { get { return new Mensagem() { Texto = "Lote excluído com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }
		public Mensagem SituacaoAlterdoSucesso { get { return new Mensagem() { Texto = "Situacao Lote alterado com sucesso.", Tipo = eTipoMensagem.Sucesso }; } }

		public Mensagem EditarSituacaoInvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A situação do Lote deve ser \"Não utilizado\"." }; } }
		public Mensagem HabilitacaoEmissaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Habilitação de emissão de CFO/CFOC é obrigatória." }; } }
		public Mensagem TipoNumeroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "rbTipoCfocfoc", Texto = "Como será a emissão do CFOC é obrigatório." }; } }
		public Mensagem EmpreendimentoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Empreendimento", Texto = "Empreendimento é obrigatório." }; } }
		public Mensagem CodigoUCObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CodigoUC", Texto = "Código da UC é obrigatório." }; } }
		public Mensagem AnoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Ano", Texto = "Ano é obrigatório." }; } }
		public Mensagem NumeroSequencialObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Numero", Texto = "N° sequencial é obrigatório." }; } }
		public Mensagem NumeroJaExiste { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "O número do lote já existe no sistema." }; } }

		public Mensagem CulturaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cultura", Texto = "Cultura é obrigatória." }; } }
		public Mensagem CultivarObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cultivar", Texto = "Cultivar é obrigatória." }; } }
		public Mensagem CultivarUnico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cultivar", Texto = "Só é permitido adicionar um mesmo cultivar por lote." }; } }

		public Mensagem UnidadeMedidaUnico { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Unidade", Texto = "Os cultivares devem possuir a mesma unidade de medida." }; } }

		public Mensagem CultivarQuantidadeSomaSuperior { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cultivar", Texto = "O saldo da cultivar já foi inteiramente utilizado." }; } }
		public Mensagem QuantidadeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Quantidade", Texto = "Quantidade é obrigatória." }; } }
		public Mensagem LoteAdicionarObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Pelo menos uma origem é obrigatório." }; } }
		public Mensagem OrigemDataMaiorLoteData { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A data do documento de origem deve ser menor ou igual a data de criação do lote." }; } }

		public Mensagem OrigemObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Origem_Tipo", Texto = "Origem é obrigatório." }; } }

		public Mensagem OrigemNumeroObrigatorio(string origem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Origem", Texto = String.Format("Número {0} é obrigatório.", origem) };
		}

		public Mensagem OrigemNumeroInexistente(string origem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "OrigemNumero", Texto = String.Format("Número {0} inexistente.", origem) };
		}

		public Mensagem OrigemSituacaoInvalida(string origem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} não está na situação \"Válido\".", origem) };
		}

		public Mensagem OrigemVencida(string origem)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} está vencido.", origem) };
		}

		public Mensagem OrigemJaAdicionada(string origem, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} {1} já está adicionado.", origem, numero) };
		}

		public Mensagem LoteSendoUtilizado(string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Lote utilizado, no CFOC {0}, não pode alterar a situação.", numero) };
		}

		public Mensagem OrigemEmpreendimentoUtilizado(string origem, string numero)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} Nº {1} já foi utilizado por outra Unidade de Consolidação e não poderá ser utilizado neste cadastro.", origem, numero) };
		}

		public Mensagem OrigemEmpreendimentoUtilizadoOutroUF(string origem, string numero, string empreendimento)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O {0} Nº {1} foi destinado para Unidade de Consolidação {2}.", origem, numero, empreendimento) };
		}

		public Mensagem CultivarDesassociadoUC(string cultivar)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("O cultivar {0} não está adicionado na caracterização UC do empreendimento.", cultivar) };
		}
	}
}