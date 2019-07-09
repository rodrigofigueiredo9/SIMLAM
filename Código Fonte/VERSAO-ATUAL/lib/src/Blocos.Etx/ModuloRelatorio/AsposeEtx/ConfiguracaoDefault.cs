using System.Collections.Generic;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx.CabecalhoRodape;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public class ConfiguracaoDefault : IConfiguradorPdf, IConfiguracaoEvent
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private bool _exibirSimplesConferencia = true;
		public bool ExibirSimplesConferencia
		{
			get { return _exibirSimplesConferencia; }
			set { _exibirSimplesConferencia = value; }
		}

		private bool _primeiraPaginaDiferente;
		public bool PrimeiraPaginaDiferente
		{
			get { return _primeiraPaginaDiferente; }
			set { _primeiraPaginaDiferente = value; }
		}

		private bool _condicionanteRemovePageBreakAnterior = true;
		public bool CondicionanteRemovePageBreakAnterior
		{
			get { return _condicionanteRemovePageBreakAnterior; }
			set { _condicionanteRemovePageBreakAnterior = value; }
		}

		private bool _exibirAssinate = false;
		public bool ExibirAssinante
		{
			get { return _exibirAssinate; }
			set { _exibirAssinate = value; }
		}

		private bool _exibirAssinantes1 = false;
		public bool ExibirAssinantes1
		{
			get { return _exibirAssinantes1; }
			set { _exibirAssinantes1 = value; }
		}

		private bool _exibirAssinantes2 = false;
		public bool ExibirAssinantes2
		{
			get { return _exibirAssinantes2; }
			set { _exibirAssinantes2 = value; }
		}


		public string TextoSimplesConferencias = "para simples conferência";
		public string TextoTarjaProrrogado = "DataVencimento";
		public string TextoTarjaEncerrado = "EncerradoMotivo";
		public string TextoTagAssinante = "«Titulo.Assinante.Nome»";
		public string TextoTagAssinantes1 = "«TableStart:Titulo.Assinantes1»";
		public string TextoTagAssinantes2 = "«TableStart:Titulo.Assinantes2»";

		private ICabecalhoRodape _cabecalhoRodape = CabecalhoRodapeFactory.Criar();
		public ICabecalhoRodape CabecalhoRodape
		{
			get { return _cabecalhoRodape; }
			set { _cabecalhoRodape = value; }
		}

		private List<IAssinante> _assinantes = new List<IAssinante>();
		public List<IAssinante> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		public virtual void Load(Document doc, object dataSource)
		{
			if (Assinantes != null && Assinantes.Count > 0)
			{
				ExibirAssinante = (Assinantes.Count % 2) != 0;
				ExibirAssinantes1 = Assinantes.Count > 1;
				ExibirAssinantes2 = Assinantes.Count > 1;
			}

			if (OnLoad != null)
			{
				OnLoad(doc, dataSource);
			}
		}

		public virtual void Configurar(Document doc)
		{
			List<Shape> lstShapeRemover = new List<Shape>();

			if (!ExibirSimplesConferencia)
			{
				if (PrimeiraPaginaDiferente)
				{
					lstShapeRemover.AddRange(doc.FindShapes(TextoSimplesConferencias));
				}
				else
				{
					lstShapeRemover.Add(doc.FindShape(TextoSimplesConferencias));
				}
			}

			lstShapeRemover.ForEach(x =>
			{
				if (x != null)
				{
					x.Remove();
				}
			});

			List<Table> lstTableRemover = new List<Table>();

			if (!ExibirAssinante)
			{
				lstTableRemover.Add(doc.LastTable(TextoTagAssinante));
			}

			if (!ExibirAssinantes1)
			{
				lstTableRemover.Add(doc.LastTable(TextoTagAssinantes1));
			}

			if (!ExibirAssinantes2)
			{
				lstTableRemover.Add(doc.LastTable(TextoTagAssinantes2));
			}

			lstTableRemover.ForEach(x =>
			{
				if (x != null)
				{
					x.RemoveAllChildren();
				}
			});
		}

		public virtual void Executed(Document doc, object dataSource)
		{
			if (OnExecuted != null)
			{
				OnExecuted(doc, dataSource);
			}
		}

		public delegate void Acao(Document doc, object dataSource);

		private event Acao OnLoad;
		public void AddLoadAcao(Acao Acao)
		{
			OnLoad += Acao;
		}

		private event Acao OnExecuted;
		public void AddExecutedAcao(Acao Acao)
		{
			OnExecuted += Acao;
		}

		//public ConfiguracaoDefault(bool local = false)
		//{
		//	_cabecalhoRodape = CabecalhoRodapeFactory.Criar(local: local);
		//}
	}
}