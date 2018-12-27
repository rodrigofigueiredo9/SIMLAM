using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteValidar
	{
		#region Propriedades

		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();
		InformacaoCorteDa _da = new InformacaoCorteDa();
		Configuracao.ConfiguracaoSistema _configSys = new Configuracao.ConfiguracaoSistema();

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioConsulta); }
		}

		#endregion

		internal bool Salvar(InformacaoCorte caracterizacao, int projetoDigitalId)
		{
			if (!_caracterizacaoValidar.Basicas(caracterizacao.Empreendimento.Id))
				return false;

			InformacaoCorte auxiliar = _da.ObterPorEmpreendimento(caracterizacao.Empreendimento.Id, true) ?? new InformacaoCorte();

			if (caracterizacao.Id <= 0 && auxiliar.Id > 0)
			{
				Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoCaracterizacaoJaCriada);
				return false;
			}

			if (!Acessar(caracterizacao.Empreendimento.Id, projetoDigitalId))
				return false;

			if (caracterizacao.AreaFlorestaPlantada > 100)
			{
				if (caracterizacao.InformacaoCorteLicenca.Count < 1)
					Validacao.Add(Mensagem.InformacaoCorte.LicencaObrigatoria);
			}

			if (caracterizacao.InformacaoCorteTipo.Count < 1)
				Validacao.Add(Mensagem.InformacaoCorte.InformacaoCorteListaObrigatorio);

			return Validacao.EhValido;
		}

		public bool Acessar(int empreendimentoId, int projetoDigitalId)
		{
			_caracterizacaoValidar.Dependencias(empreendimentoId, projetoDigitalId, (int)eCaracterizacao.InformacaoCorte);

			return Validacao.EhValido;
		}

		internal bool CopiarDadosInstitucional(InformacaoCorte caracterizacao)
		{
			if (caracterizacao.InternoID <= 0)
				Validacao.Add(Mensagem.Dominialidade.CopiarCaractizacaoCadastrada);

			return Validacao.EhValido;
		}
	}
}