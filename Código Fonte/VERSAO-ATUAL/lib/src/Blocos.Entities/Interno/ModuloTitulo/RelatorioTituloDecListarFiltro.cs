using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class RelatorioTituloDecListarFiltro
	{
		private String _numeroTitulo;
		public String NumeroTitulo {
			get
			{
				return _numeroTitulo;
			}
			set
			{
				_numeroTitulo = value.Trim().ToUpper();
			}
		}

		private String _login;
		public String Login {
			get
			{
				return _login;
			}
			set
			{
				_login = value.Trim().ToUpper();
			}
		}

		private String _nomeUsuario;
		public String NomeUsuario {
			get
			{
				return _nomeUsuario;
			}
			set
			{
				_nomeUsuario = value.Trim().ToUpper();
			}
		}

		private String _nomeInteressado;
		public String NomeInteressado {
			get
			{
				return _nomeInteressado;
			}
			set
			{
				_nomeInteressado = value.Trim().ToUpper();
			}
		}
		public String InteressadoCpfCnpj { get; set; }

		private DateTecno _dataSituacaoAtual = new DateTecno();
		public DateTecno DataSituacaoAtual
		{
			get { return _dataSituacaoAtual; }
			set { _dataSituacaoAtual = value; }
		}

		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }

		public String IP { get; set; }

	}
}
