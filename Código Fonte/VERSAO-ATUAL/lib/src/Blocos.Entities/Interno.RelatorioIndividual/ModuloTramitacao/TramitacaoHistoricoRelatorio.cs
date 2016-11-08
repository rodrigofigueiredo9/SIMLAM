using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao
{
	public class TramitacaoHistoricoRelatorio
	{
		private TramitacaoRelatorio _enviar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Enviar
		{
			get { return _enviar; }
			set { _enviar = value; }
		}

		private TramitacaoRelatorio _receber = new TramitacaoRelatorio();
		public TramitacaoRelatorio Receber
		{
			get { return _receber; }
			set { _receber = value; }
		}

		private TramitacaoRelatorio _cancelar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Cancelar
		{
			get { return _cancelar; }
			set { _cancelar = value; }
		}

		private TramitacaoRelatorio _arquivar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Arquivar
		{
			get { return _arquivar; }
			set { _arquivar = value; }
		}

		private TramitacaoRelatorio _desarquivar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Desarquivar
		{
			get { return _desarquivar; }
			set { _desarquivar = value; }
		}

		private TramitacaoRelatorio _converter = new TramitacaoRelatorio();
		public TramitacaoRelatorio Converter
		{
			get { return _converter; }
			set { _converter = value; }
		}

		private TramitacaoRelatorio _apensar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Apensar
		{
			get { return _apensar; }
			set { _apensar = value; }
		}

		private TramitacaoRelatorio _juntar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Juntar
		{
			get { return _juntar; }
			set { _juntar = value; }
		}

		private TramitacaoRelatorio _desapensar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Desapensar
		{
			get { return _desapensar; }
			set { _desapensar = value; }
		}

		private TramitacaoRelatorio _desentranhar = new TramitacaoRelatorio();
		public TramitacaoRelatorio Desentranhar
		{
			get { return _desentranhar; }
			set { _desentranhar = value; }
		}

		public TramitacaoHistoricoRelatorio(TramitacaoRelatorio tramitacao)
		{
			switch (tramitacao.Acao)
			{
				case eHistoricoAcao.enviar:
					Enviar = tramitacao;
					break;

				case eHistoricoAcao.receber:
					Receber = tramitacao;
					break;

				case eHistoricoAcao.cancelar:
					Cancelar = tramitacao;
					break;

				case eHistoricoAcao.arquivar:
					Arquivar = tramitacao;
					break;

				case eHistoricoAcao.desarquivar:
					Desarquivar = tramitacao;
					break;

				case eHistoricoAcao.converter:
					Converter = tramitacao;
					break;

				case eHistoricoAcao.apensar:
					Apensar = tramitacao;
					break;

				case eHistoricoAcao.juntar:
					Juntar = tramitacao;
					break;

				case eHistoricoAcao.desapensar:
					Desapensar = tramitacao;
					break;

				case eHistoricoAcao.desentranhar:
					Desentranhar = tramitacao;
					break;

				default:
					return;
			}
		}
	}
}