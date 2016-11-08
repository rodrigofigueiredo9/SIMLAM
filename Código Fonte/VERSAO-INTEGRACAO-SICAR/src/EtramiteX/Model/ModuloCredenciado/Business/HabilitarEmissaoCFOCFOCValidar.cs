using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public class HabilitarEmissaoCFOCFOCValidar
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		HabilitarEmissaoCFOCFOCDa _da = new HabilitarEmissaoCFOCFOCDa();

		private HabilitarEmissaoCFOCFOCMsg _msg = Mensagem.HabilitarEmissaoCFOCFOC;
		public HabilitarEmissaoCFOCFOCMsg Msg
		{
			get { return _msg; }
			set { _msg = value; }
		}

		public bool AlterarSituacao(HabilitarEmissaoCFOCFOC habilitar)
		{
			VerificarAlterarSituacao(habilitar);

			return Validacao.EhValido;
		}

		public bool RenovarData(PragaHabilitarEmissao praga)
		{
			VerificarRenovarData(praga);

			return Validacao.EhValido;
		}

		public bool Salvar(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (habilitar.Id > 0)
			{
				VerificarEditar(habilitar);
			}
			else
			{
				VerificarCriar(habilitar);
			}

			return Validacao.EhValido;
		}

		public bool VerificarHabilitarEmissaoCFOCFOC(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (habilitar.Responsavel.Id == 0)
			{
				Validacao.Add(Msg.ResponsavelObrigatorio);
			}

			int auxiliar = 0;
			if (int.TryParse(habilitar.NumeroHabilitacao, out auxiliar) && String.IsNullOrEmpty(habilitar.NumeroHabilitacao))
			{
				Validacao.Add(Msg.NumeroHabilitacaoObrigatorio);
			}
			else
			{
				if (habilitar.NumeroHabilitacao.Length != 8)
				{
					Validacao.Add(Msg.NumeroHabilitacaoTamanhoInvalido);
				}
			}

			if (String.IsNullOrEmpty(habilitar.ValidadeRegistro))
			{
				Validacao.Add(Msg.ValidadeRegistroObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(habilitar.ValidadeRegistro))
				{
					Validacao.Add(Msg.ValidadeRegistroInvalida);
				}
			}

			if (String.IsNullOrEmpty(habilitar.NumeroDua))
			{
				Validacao.Add(Msg.NumeroDuaObrigatorio);
			}

			if(Convert.ToBoolean(habilitar.ExtensaoHabilitacao))
			{
				if(string.IsNullOrWhiteSpace(habilitar.NumeroHabilitacaoOrigem))
				{
					Validacao.Add(Msg.NumeroHabilitacaoOrigemObrigatorio);
				}
				else
				{
					if (habilitar.NumeroHabilitacaoOrigem.Length != 8)
					{
						Validacao.Add(Msg.NumeroHabilitacaoOrigemTamanhoInvalido);
					}

					if (habilitar.NumeroHabilitacao != habilitar.NumeroHabilitacaoOrigem)
					{
						Validacao.Add(Msg.NumeroHabilitacaoOrigemInvalido);
					}
				}
			}

			if (habilitar.UF == 0)
			{
				Validacao.Add(Msg.UFObrigatorio);
			}
			else
			{
				ListaBus listaBus = new ListaBus();
				Estado estadoPadrao = listaBus.Estados.SingleOrDefault(x => String.Equals(x.Texto, _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault), StringComparison.InvariantCultureIgnoreCase));

				if(estadoPadrao.Id != habilitar.UF && string.IsNullOrWhiteSpace(habilitar.NumeroVistoCrea))
				{
					Validacao.Add(Msg.NumeroVistoCreaObrigatorio);
				}
			}

			if (habilitar.Pragas.Count == 0)
			{
				Validacao.Add(Msg.PragaObrigatorio);
			}

			foreach (var item in habilitar.Pragas)
			{
				if (String.IsNullOrEmpty(item.DataInicialHabilitacao))
				{
					Validacao.Add(Msg.DataInicialHabilitacaoObrigatoria);
				}
				else
				{
					if (!ValidacoesGenericasBus.ValidarData(item.DataInicialHabilitacao))
					{
						Validacao.Add(Msg.DataInicialHabilitacaoInvalida);
					}
					else if (Convert.ToDateTime(item.DataInicialHabilitacao) > DateTime.Today.AddDays(1).Subtract(TimeSpan.FromSeconds(1)))
					{
						Validacao.Add(Msg.DataInicialHabilitacaoMaiorAtual);
					}
				}

				if (String.IsNullOrEmpty(item.DataFinalHabilitacao))
				{
					Validacao.Add(Msg.DataFinalHabilitacaoObrigatorio);
				}
				else
				{
					if (!ValidacoesGenericasBus.ValidarData(item.DataFinalHabilitacao))
					{
						Validacao.Add(Msg.DataFinalHabilitacaoInvalida);
					}
				}
			}

			return Validacao.EhValido;
		}

		public bool VerificarAdicionarPraga(HabilitarEmissaoCFOCFOC habilitar, PragaHabilitarEmissao praga)
		{
			if (String.IsNullOrEmpty(praga.Praga.NomeCientifico))
			{
				Validacao.Add(Msg.PragaNomeObrigatorio);
			}

			if (String.IsNullOrEmpty(praga.DataInicialHabilitacao))
			{
				Validacao.Add(Msg.DataInicialHabilitacaoObrigatoria);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(praga.DataInicialHabilitacao))
				{
					Validacao.Add(Msg.DataInicialHabilitacaoInvalida);
				}
				else if (Convert.ToDateTime(praga.DataInicialHabilitacao) > DateTime.Today.AddDays(1).Subtract(TimeSpan.FromSeconds(1)))
				{
					Validacao.Add(Msg.DataInicialHabilitacaoMaiorAtual);
				}
			}

			if (String.IsNullOrEmpty(praga.DataFinalHabilitacao))
			{
				Validacao.Add(Msg.DataFinalHabilitacaoObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(praga.DataFinalHabilitacao))
				{
					Validacao.Add(Msg.DataFinalHabilitacaoInvalida);
				}
			}

			var existe = habilitar.Pragas.Exists(x => !String.IsNullOrWhiteSpace(praga.Praga.NomeCientifico) && (x.Praga.NomeCientifico.Trim() == praga.Praga.NomeCientifico.Trim()));

			if (existe)
			{
				Validacao.Add(Msg.PragaJaAdicionada);
			}

			return Validacao.EhValido;
		}

		public bool VerificarAlterarSituacao(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (String.IsNullOrEmpty(habilitar.SituacaoData))
			{
				Validacao.Add(Msg.SituacaoDataObrigatoria);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(habilitar.SituacaoData))
				{
					Validacao.Add(Msg.SituacaoDataInvalida);
				}
			}

			if (habilitar.Situacao == 0)
			{
				Validacao.Add(Msg.SituacaoObrigatoria);
			}

			if (habilitar.Situacao == 3)
			{
				if (habilitar.Motivo == 0)
				{
					Validacao.Add(Msg.MotivoObrigatorio);
				}
			}

			if (String.IsNullOrEmpty(habilitar.Observacao))
			{
				Validacao.Add(Msg.SituacaoObservacaoObrigatoria);
			}

			return Validacao.EhValido;
		}

		public bool VerificarRenovarData(PragaHabilitarEmissao praga)
		{
			if (String.IsNullOrEmpty(praga.DataInicialHabilitacao))
			{
				Validacao.Add(Msg.DataInicialRenovarObrigatoria);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(praga.DataInicialHabilitacao))
				{
					Validacao.Add(Msg.DataInicialRenovarInvalida);
				}
				else if (Convert.ToDateTime(praga.DataInicialHabilitacao) > DateTime.Today.AddDays(1).Subtract(TimeSpan.FromSeconds(1)))
				{
					Validacao.Add(Msg.DataInicialRenovarMaiorAtual);
				}
			}

			if (String.IsNullOrEmpty(praga.DataFinalHabilitacao))
			{
				Validacao.Add(Msg.DataFinalRenovarObrigatorio);
			}
			else
			{
				if (!ValidacoesGenericasBus.ValidarData(praga.DataFinalHabilitacao))
				{
					Validacao.Add(Msg.DataFinalRenovarInvalida);
				}
			}

			return Validacao.EhValido;
		}

		public bool VerificarCriar(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (_da.ValidarNumeroHabilitacao(habilitar.Id, habilitar.NumeroHabilitacao))
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.NumeroHabilitacaoJaExiste);
			}

			VerificarHabilitarEmissaoCFOCFOC(habilitar);

			if (_da.ValidarResponsavelHabilitado(habilitar.Responsavel.Id))
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.ResponsavelHabilitado);
			}

			return Validacao.EhValido;
		}

		public bool VerificarEditar(HabilitarEmissaoCFOCFOC habilitar)
		{
			if (_da.ValidarNumeroHabilitacao(habilitar.Id, habilitar.NumeroHabilitacao))
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.NumeroHabilitacaoJaExiste);
			}

			VerificarHabilitarEmissaoCFOCFOC(habilitar);

			return Validacao.EhValido;
		}
	}
}